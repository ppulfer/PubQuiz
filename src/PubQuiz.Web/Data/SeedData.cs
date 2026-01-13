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
                Text = "What is the capital of France?",
                Options = ["Berlin", "Paris", "London", "Madrid"],
                CorrectOptionIndex = 1,
                TimeLimitSeconds = 20
            },
            new()
            {
                Id = Guid.NewGuid(),
                Text = "Which planet is known as the Red Planet?",
                Options = ["Venus", "Jupiter", "Mars", "Saturn"],
                CorrectOptionIndex = 2,
                TimeLimitSeconds = 20
            },
            new()
            {
                Id = Guid.NewGuid(),
                Text = "What year did World War II end?",
                Options = ["1943", "1944", "1945", "1946"],
                CorrectOptionIndex = 2,
                TimeLimitSeconds = 20
            },
            new()
            {
                Id = Guid.NewGuid(),
                Text = "Who painted the Mona Lisa?",
                Options = ["Michelangelo", "Leonardo da Vinci", "Raphael", "Donatello"],
                CorrectOptionIndex = 1,
                TimeLimitSeconds = 20
            },
            new()
            {
                Id = Guid.NewGuid(),
                Text = "What is the chemical symbol for gold?",
                Options = ["Go", "Gd", "Au", "Ag"],
                CorrectOptionIndex = 2,
                TimeLimitSeconds = 15
            },
            new()
            {
                Id = Guid.NewGuid(),
                Text = "Which ocean is the largest?",
                Options = ["Atlantic", "Indian", "Arctic", "Pacific"],
                CorrectOptionIndex = 3,
                TimeLimitSeconds = 15
            },
            new()
            {
                Id = Guid.NewGuid(),
                Text = "What is the smallest country in the world?",
                Options = ["Monaco", "Vatican City", "San Marino", "Liechtenstein"],
                CorrectOptionIndex = 1,
                TimeLimitSeconds = 20
            },
            new()
            {
                Id = Guid.NewGuid(),
                Text = "How many bones are in the adult human body?",
                Options = ["186", "206", "226", "246"],
                CorrectOptionIndex = 1,
                TimeLimitSeconds = 20
            },
            new()
            {
                Id = Guid.NewGuid(),
                Text = "What is the largest mammal on Earth?",
                Options = ["African Elephant", "Blue Whale", "Giraffe", "Hippopotamus"],
                CorrectOptionIndex = 1,
                TimeLimitSeconds = 15
            },
            new()
            {
                Id = Guid.NewGuid(),
                Text = "In what year did the Titanic sink?",
                Options = ["1910", "1911", "1912", "1913"],
                CorrectOptionIndex = 2,
                TimeLimitSeconds = 20
            }
        };

        context.Questions.AddRange(questions);
        await context.SaveChangesAsync();
    }
}
