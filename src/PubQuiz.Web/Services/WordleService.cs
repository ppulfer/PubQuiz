namespace PubQuiz.Web.Services;

public class WordleService
{
    public string EvaluateGuess(string guess, string targetWord)
    {
        guess = guess.ToUpper();
        targetWord = targetWord.ToUpper();

        var result = new char[guess.Length];
        var targetChars = targetWord.ToCharArray();
        var used = new bool[targetWord.Length];

        for (var i = 0; i < guess.Length; i++)
        {
            if (i < targetWord.Length && guess[i] == targetWord[i])
            {
                result[i] = 'C';
                used[i] = true;
            }
            else
            {
                result[i] = 'N';
            }
        }

        for (var i = 0; i < guess.Length; i++)
        {
            if (result[i] == 'C') continue;

            for (var j = 0; j < targetWord.Length; j++)
            {
                if (!used[j] && guess[i] == targetWord[j])
                {
                    result[i] = 'Y';
                    used[j] = true;
                    break;
                }
            }
        }

        return new string(result);
    }

    public bool IsValidGuess(string guess, int wordLength)
    {
        return !string.IsNullOrWhiteSpace(guess) &&
               guess.Length == wordLength &&
               guess.All(char.IsLetter);
    }

    public bool IsSolved(string result)
    {
        return result.All(c => c == 'C');
    }
}
