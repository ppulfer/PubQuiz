namespace PubQuiz.Web.Models;

public class Question
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public List<string> Options { get; set; } = [];
    public int CorrectOptionIndex { get; set; }
    public int TimeLimitSeconds { get; set; } = 30;
}
