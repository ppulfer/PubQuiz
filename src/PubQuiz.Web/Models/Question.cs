namespace PubQuiz.Web.Models;

public class Question
{
    public Guid Id { get; set; }
    public QuestionType Type { get; set; } = QuestionType.MultipleChoice;
    public string Text { get; set; } = string.Empty;
    public List<string> Options { get; set; } = [];
    public int CorrectOptionIndex { get; set; }
    public string? CorrectAnswer { get; set; }
    public List<string> AcceptedAnswers { get; set; } = [];
    public int TimeLimitSeconds { get; set; } = 30;
    public int MaxAttempts { get; set; } = 6;
}
