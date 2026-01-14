namespace PubQuiz.Web.Models;

public class DinoProgress
{
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public int Score { get; set; }
}
