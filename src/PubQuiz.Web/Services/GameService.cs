using Microsoft.EntityFrameworkCore;
using PubQuiz.Web.Data;
using PubQuiz.Web.Models;

namespace PubQuiz.Web.Services;

public class GameService(PubQuizDbContext context, ScoringService scoringService, WordleService wordleService)
{
    private static readonly Random Random = new();

    public async Task<Game> CreateGameAsync(string hostPassword, string? customCode = null)
    {
        var code = string.IsNullOrWhiteSpace(customCode) ? GenerateGameCode() : customCode.ToUpper().Trim();

        var game = new Game
        {
            Id = Guid.NewGuid(),
            Code = code,
            HostPasswordHash = BCrypt.Net.BCrypt.HashPassword(hostPassword),
            CreatedAt = DateTime.UtcNow
        };

        context.Games.Add(game);
        await context.SaveChangesAsync();
        return game;
    }

    public async Task<Game?> GetGameByCodeAsync(string code)
    {
        return await context.Games
            .AsNoTracking()
            .Include(g => g.Teams)
            .Include(g => g.Answers)
            .FirstOrDefaultAsync(g => g.Code == code.ToUpper());
    }

    public async Task<Game?> GetGameByIdAsync(Guid id)
    {
        return await context.Games
            .AsNoTracking()
            .Include(g => g.Teams)
            .Include(g => g.Answers)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public bool VerifyHostPassword(Game game, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, game.HostPasswordHash);
    }

    public async Task<Team> JoinGameAsync(Guid gameId, string teamName)
    {
        var team = new Team
        {
            Id = Guid.NewGuid(),
            GameId = gameId,
            Name = teamName,
            JoinedAt = DateTime.UtcNow
        };

        context.Teams.Add(team);
        await context.SaveChangesAsync();
        return team;
    }

    public async Task<List<Question>> GetQuestionsAsync()
    {
        return await context.Questions.OrderBy(q => q.Id).ToListAsync();
    }

    public async Task StartQuestionAsync(Guid gameId, int questionIndex)
    {
        var game = await context.Games.FirstOrDefaultAsync(g => g.Id == gameId);
        if (game == null) return;

        game.Status = GameStatus.InProgress;
        game.CurrentQuestionIndex = questionIndex;
        game.QuestionStartedAt = null;
        await context.SaveChangesAsync();
    }

    public async Task ShowQuestionToTeamsAsync(Guid gameId)
    {
        var game = await context.Games.FirstOrDefaultAsync(g => g.Id == gameId);
        if (game == null) return;

        game.QuestionStartedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }

    public async Task<Answer?> SubmitAnswerAsync(Guid gameId, Guid teamId, int questionIndex, int selectedOption, Question question)
    {
        var game = await context.Games.AsNoTracking().FirstOrDefaultAsync(g => g.Id == gameId);
        if (game?.QuestionStartedAt == null) return null;

        var existingAnswer = await context.Answers
            .FirstOrDefaultAsync(a => a.GameId == gameId && a.TeamId == teamId && a.QuestionIndex == questionIndex);
        if (existingAnswer != null) return existingAnswer;

        var submittedAt = DateTime.UtcNow;
        var secondsTaken = (submittedAt - game.QuestionStartedAt.Value).TotalSeconds;
        var isCorrect = selectedOption == question.CorrectOptionIndex;
        var points = scoringService.CalculatePoints(isCorrect, secondsTaken);

        var answer = new Answer
        {
            Id = Guid.NewGuid(),
            GameId = gameId,
            TeamId = teamId,
            QuestionIndex = questionIndex,
            SelectedOptionIndex = selectedOption,
            SubmittedAt = submittedAt,
            IsCorrect = isCorrect,
            PointsAwarded = points,
            Status = AnswerStatus.AutoScored
        };

        context.Answers.Add(answer);
        await context.SaveChangesAsync();
        return answer;
    }

    public async Task<Answer?> SubmitTextAnswerAsync(Guid gameId, Guid teamId, int questionIndex, string textAnswer, Question question)
    {
        var existingAnswer = await context.Answers
            .FirstOrDefaultAsync(a => a.GameId == gameId && a.TeamId == teamId && a.QuestionIndex == questionIndex);
        if (existingAnswer != null) return existingAnswer;

        var normalizedAnswer = textAnswer.Trim().ToLowerInvariant();
        var isCorrect = question.AcceptedAnswers
            .Any(a => a.Trim().ToLowerInvariant() == normalizedAnswer);

        var answer = new Answer
        {
            Id = Guid.NewGuid(),
            GameId = gameId,
            TeamId = teamId,
            QuestionIndex = questionIndex,
            TextAnswer = textAnswer,
            SubmittedAt = DateTime.UtcNow,
            Status = isCorrect ? AnswerStatus.Approved : AnswerStatus.Rejected,
            IsCorrect = isCorrect,
            PointsAwarded = isCorrect ? scoringService.CalculateOpenEndedPoints(true) : 0
        };

        context.Answers.Add(answer);
        await context.SaveChangesAsync();
        return answer;
    }

    public async Task<Answer?> SubmitEstimateAnswerAsync(Guid gameId, Guid teamId, int questionIndex, decimal estimate, Question question)
    {
        var existingAnswer = await context.Answers
            .FirstOrDefaultAsync(a => a.GameId == gameId && a.TeamId == teamId && a.QuestionIndex == questionIndex);
        if (existingAnswer != null) return existingAnswer;

        var (points, deviationPercent) = scoringService.CalculateEstimatePoints(
            estimate,
            question.CorrectValue ?? 0,
            question.TolerancePercent ?? 50);

        var answer = new Answer
        {
            Id = Guid.NewGuid(),
            GameId = gameId,
            TeamId = teamId,
            QuestionIndex = questionIndex,
            EstimateValue = estimate,
            DeviationPercent = deviationPercent,
            SubmittedAt = DateTime.UtcNow,
            Status = AnswerStatus.AutoScored,
            IsCorrect = points > 0,
            PointsAwarded = points
        };

        context.Answers.Add(answer);
        await context.SaveChangesAsync();
        return answer;
    }

    public async Task<List<Answer>> GetPendingAnswersAsync(Guid gameId, int questionIndex)
    {
        return await context.Answers
            .Include(a => a.Team)
            .Where(a => a.GameId == gameId && a.QuestionIndex == questionIndex && a.Status == AnswerStatus.Pending)
            .OrderBy(a => a.SubmittedAt)
            .ToListAsync();
    }

    public async Task<List<Answer>> GetAllAnswersForQuestionAsync(Guid gameId, int questionIndex)
    {
        return await context.Answers
            .Include(a => a.Team)
            .Where(a => a.GameId == gameId && a.QuestionIndex == questionIndex)
            .OrderBy(a => a.SubmittedAt)
            .ToListAsync();
    }

    public async Task ApproveAnswerAsync(Guid answerId, bool approved)
    {
        var answer = await context.Answers.FirstOrDefaultAsync(a => a.Id == answerId);
        if (answer == null) return;

        answer.Status = approved ? AnswerStatus.Approved : AnswerStatus.Rejected;
        answer.IsCorrect = approved;
        answer.PointsAwarded = scoringService.CalculateOpenEndedPoints(approved);
        await context.SaveChangesAsync();
    }

    public async Task<Answer?> GetOrCreateWordleAnswerAsync(Guid gameId, Guid teamId, int questionIndex)
    {
        var existingAnswer = await context.Answers
            .Include(a => a.WordleAttempts)
            .FirstOrDefaultAsync(a => a.GameId == gameId && a.TeamId == teamId && a.QuestionIndex == questionIndex);

        if (existingAnswer != null) return existingAnswer;

        var answer = new Answer
        {
            Id = Guid.NewGuid(),
            GameId = gameId,
            TeamId = teamId,
            QuestionIndex = questionIndex,
            SubmittedAt = DateTime.UtcNow,
            Status = AnswerStatus.AutoScored,
            AttemptsUsed = 0,
            IsCorrect = false,
            PointsAwarded = 0
        };

        context.Answers.Add(answer);
        await context.SaveChangesAsync();
        return answer;
    }

    public async Task<(WordleAttempt attempt, bool solved, bool outOfAttempts)> SubmitWordleGuessAsync(
        Guid gameId, Guid teamId, int questionIndex, string guess, Question question)
    {
        var answer = await GetOrCreateWordleAnswerAsync(gameId, teamId, questionIndex);
        if (answer == null || answer.IsCorrect || answer.AttemptsUsed >= question.MaxAttempts)
        {
            return (null!, answer?.IsCorrect ?? false, true);
        }

        var targetWord = question.CorrectAnswer ?? "";
        var result = wordleService.EvaluateGuess(guess, targetWord);
        var solved = wordleService.IsSolved(result);

        answer.AttemptsUsed = (answer.AttemptsUsed ?? 0) + 1;

        var attempt = new WordleAttempt
        {
            Id = Guid.NewGuid(),
            AnswerId = answer.Id,
            AttemptNumber = answer.AttemptsUsed.Value,
            Guess = guess.ToUpper(),
            Result = result,
            SubmittedAt = DateTime.UtcNow
        };

        context.WordleAttempts.Add(attempt);

        if (solved)
        {
            answer.IsCorrect = true;
            answer.PointsAwarded = scoringService.CalculateWordlePoints(true, answer.AttemptsUsed.Value, question.MaxAttempts);
        }

        var outOfAttempts = !solved && answer.AttemptsUsed >= question.MaxAttempts;

        await context.SaveChangesAsync();
        return (attempt, solved, outOfAttempts);
    }

    public async Task<List<WordleAttempt>> GetWordleAttemptsAsync(Guid answerId)
    {
        return await context.WordleAttempts
            .Where(a => a.AnswerId == answerId)
            .OrderBy(a => a.AttemptNumber)
            .ToListAsync();
    }

    public async Task<Answer?> GetAnswerAsync(Guid gameId, Guid teamId, int questionIndex)
    {
        return await context.Answers
            .Include(a => a.WordleAttempts)
            .FirstOrDefaultAsync(a => a.GameId == gameId && a.TeamId == teamId && a.QuestionIndex == questionIndex);
    }

    public async Task EndGameAsync(Guid gameId)
    {
        var game = await context.Games.FirstOrDefaultAsync(g => g.Id == gameId);
        if (game == null) return;

        game.Status = GameStatus.Finished;
        await context.SaveChangesAsync();
    }

    public async Task<List<TeamScore>> GetLeaderboardAsync(Guid gameId)
    {
        var teamsWithScores = await context.Teams
            .Where(t => t.GameId == gameId)
            .Select(t => new TeamScore
            {
                TeamId = t.Id,
                TeamName = t.Name,
                TotalPoints = t.Answers.Where(a => a.GameId == gameId).Sum(a => a.PointsAwarded),
                CorrectAnswers = t.Answers.Where(a => a.GameId == gameId).Count(a => a.IsCorrect)
            })
            .OrderByDescending(t => t.TotalPoints)
            .ThenBy(t => t.TeamName)
            .ToListAsync();

        return teamsWithScores;
    }

    public async Task<List<GameSummary>> GetAllGamesAsync()
    {
        return await context.Games
            .OrderByDescending(g => g.CreatedAt)
            .Select(g => new GameSummary
            {
                Id = g.Id,
                Code = g.Code,
                Status = g.Status,
                CurrentQuestionIndex = g.CurrentQuestionIndex,
                TeamCount = g.Teams.Count,
                CreatedAt = g.CreatedAt
            })
            .ToListAsync();
    }

    public async Task DeleteGameAsync(Guid gameId)
    {
        var game = await context.Games.FirstOrDefaultAsync(g => g.Id == gameId);
        if (game != null)
        {
            context.Games.Remove(game);
            await context.SaveChangesAsync();
        }
    }

    private static string GenerateGameCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
}

public class TeamScore
{
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public int CorrectAnswers { get; set; }
}

public class GameSummary
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public GameStatus Status { get; set; }
    public int CurrentQuestionIndex { get; set; }
    public int TeamCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
