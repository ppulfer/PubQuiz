namespace PubQuiz.Web.Models;

public class Team
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public Game Game { get; set; } = null!;
    public List<Answer> Answers { get; set; } = [];
}
