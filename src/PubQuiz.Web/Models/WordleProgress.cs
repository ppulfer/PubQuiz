namespace PubQuiz.Web.Models;

public class WordleProgress
{
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public int AttemptsUsed { get; set; }
    public bool Solved { get; set; }
    public bool OutOfAttempts { get; set; }
}
