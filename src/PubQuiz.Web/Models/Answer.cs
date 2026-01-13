namespace PubQuiz.Web.Models;

public class Answer
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Guid GameId { get; set; }
    public int QuestionIndex { get; set; }
    public int SelectedOptionIndex { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public bool IsCorrect { get; set; }
    public int PointsAwarded { get; set; }

    public Team Team { get; set; } = null!;
    public Game Game { get; set; } = null!;
}
