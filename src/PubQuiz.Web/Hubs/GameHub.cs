using Microsoft.AspNetCore.SignalR;
using PubQuiz.Web.Models;
using PubQuiz.Web.Services;

namespace PubQuiz.Web.Hubs;

public class GameHub : Hub
{
    public async Task JoinGameGroup(string gameCode)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameCode);
    }

    public async Task LeaveGameGroup(string gameCode)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameCode);
    }

    public async Task NotifyTeamJoined(string gameCode, string teamName)
    {
        await Clients.Group(gameCode).SendAsync("TeamJoined", teamName);
    }

    public async Task StartQuestion(string gameCode, int questionIndex, Question question)
    {
        await Clients.Group(gameCode).SendAsync("QuestionStarted", questionIndex, question);
    }

    public async Task NotifyAnswerSubmitted(string gameCode, Guid teamId)
    {
        await Clients.Group(gameCode).SendAsync("AnswerSubmitted", teamId);
    }

    public async Task NotifyOpenEndedSubmitted(string gameCode, Guid teamId, string teamName, string answer)
    {
        await Clients.Group(gameCode).SendAsync("OpenEndedSubmitted", teamId, teamName, answer);
    }

    public async Task NotifyAnswerReviewed(string gameCode, Guid teamId, bool approved, int points)
    {
        await Clients.Group(gameCode).SendAsync("AnswerReviewed", teamId, approved, points);
    }

    public async Task NotifyWordleProgress(string gameCode, Guid teamId, int attemptsUsed, bool solved, bool outOfAttempts)
    {
        await Clients.Group(gameCode).SendAsync("WordleProgress", teamId, attemptsUsed, solved, outOfAttempts);
    }

    public async Task ShowResults(string gameCode, int questionIndex, List<TeamScore> leaderboard)
    {
        await Clients.Group(gameCode).SendAsync("ShowResults", questionIndex, leaderboard);
    }

    public async Task EndGame(string gameCode, List<TeamScore> finalLeaderboard)
    {
        await Clients.Group(gameCode).SendAsync("GameEnded", finalLeaderboard);
    }
}
