namespace PubQuiz.Web.Models;

public class Game
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public GameStatus Status { get; set; } = GameStatus.Waiting;
    public int CurrentQuestionIndex { get; set; } = -1;
    public DateTime? QuestionStartedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string HostPasswordHash { get; set; } = string.Empty;
    public bool UsePredefinedTeams { get; set; }
    public int PredefinedTeamCount { get; set; }

    public List<Team> Teams { get; set; } = [];
    public List<Answer> Answers { get; set; } = [];
}
