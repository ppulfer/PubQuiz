using Microsoft.EntityFrameworkCore;
using PubQuiz.Web.Models;

namespace PubQuiz.Web.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PubQuizDbContext>();

        await context.Database.MigrateAsync();

        if (await context.Questions.AnyAsync())
            return;

        var questions = new List<Question>
        {
            new()
            {
                Id = Guid.NewGuid(),
                SortOrder = 1,
                Type = QuestionType.MultipleChoice,
                Text = "Kubernetes originates from the Greek language. What does it translate to?",
                Options = ["Captain", "Navigator", "Steersman", "Shipbuilder"],
                CorrectOptionIndex = 2,
                TimeLimitSeconds = 120
            },
            new()
            {
                Id = Guid.NewGuid(),
                SortOrder = 2,
                Type = QuestionType.RealOrFake,
                Text = "Which Hero is the real one?",
                ImageUrls = ["/images/realorfake/inno_1.png", "/images/realorfake/inno_2.png", "/images/realorfake/inno_3.png", "/images/realorfake/inno_4.png"],
                CorrectImageIndex = 1,
                TimeLimitSeconds = 120
            },
            new()
            {
                Id = Guid.NewGuid(),
                SortOrder = 3,
                Type = QuestionType.RealOrFake,
                Text = "Which Hero is the real one?",
                ImageUrls = ["/images/realorfake/eigen_1.png", "/images/realorfake/eigen_2.png", "/images/realorfake/eigen_3.png", "/images/realorfake/eigen_4.png"],
                CorrectImageIndex = 0,
                TimeLimitSeconds = 60
            },
            new()
            {
                Id = Guid.NewGuid(),
                SortOrder = 4,
                Type = QuestionType.Wordle,
                Text = "Guess a word related to Digitec Galaxus",
                CorrectAnswer = "MIGORS",
                MaxAttempts = 6,
                TimeLimitSeconds = 300
            },
            new()
            {
                Id = Guid.NewGuid(),
                SortOrder = 5,
                Type = QuestionType.Wordle,
                Text = "Guess a word - it's related to Programming",
                CorrectAnswer = "MEMORY",
                MaxAttempts = 6,
                TimeLimitSeconds = 300
            },
            new()
            {
                Id = Guid.NewGuid(),
                SortOrder = 6,
                Type = QuestionType.MultipleChoice,
                Text = "How many bones are in the adult human body?",
                Options = ["186", "206", "226", "246"],
                CorrectOptionIndex = 1,
                TimeLimitSeconds = 60
            },
        };

        context.Questions.AddRange(questions);
        await context.SaveChangesAsync();
    }
}
