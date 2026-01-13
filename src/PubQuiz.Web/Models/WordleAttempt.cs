namespace PubQuiz.Web.Models;

public class WordleAttempt
{
    public Guid Id { get; set; }
    public Guid AnswerId { get; set; }
    public int AttemptNumber { get; set; }
    public string Guess { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public Answer Answer { get; set; } = null!;
}
