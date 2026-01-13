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

    public async Task ShowResults(string gameCode, int questionIndex, List<TeamScore> leaderboard)
    {
        await Clients.Group(gameCode).SendAsync("ShowResults", questionIndex, leaderboard);
    }

    public async Task EndGame(string gameCode, List<TeamScore> finalLeaderboard)
    {
        await Clients.Group(gameCode).SendAsync("GameEnded", finalLeaderboard);
    }
}
