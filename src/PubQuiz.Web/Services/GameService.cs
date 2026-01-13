using Microsoft.EntityFrameworkCore;
using PubQuiz.Web.Data;
using PubQuiz.Web.Models;

namespace PubQuiz.Web.Services;

public class GameService(PubQuizDbContext context, ScoringService scoringService)
{
    private static readonly Random Random = new();

    public async Task<Game> CreateGameAsync(string hostPassword)
    {
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Code = GenerateGameCode(),
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
            PointsAwarded = points
        };

        context.Answers.Add(answer);
        await context.SaveChangesAsync();
        return answer;
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
