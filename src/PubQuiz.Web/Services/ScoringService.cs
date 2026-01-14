namespace PubQuiz.Web.Services;

public class ScoringService
{
    private const int MaxPoints = 10;

    public int CalculatePoints(bool isCorrect, double secondsTaken)
    {
        return isCorrect ? MaxPoints : 0;
    }

    public int CalculateOpenEndedPoints(bool isApproved)
    {
        return isApproved ? MaxPoints : 0;
    }

    public int CalculateWordlePoints(bool solved, int attemptsUsed, int maxAttempts = 6)
    {
        if (!solved)
            return 0;

        // Base 10 points for solving, +2 for each attempt saved
        var attemptsSaved = maxAttempts - attemptsUsed;
        return MaxPoints + (attemptsSaved * 2);
    }

    public (int points, decimal deviationPercent) CalculateEstimatePoints(decimal guess, decimal correctValue, decimal tolerancePercent)
    {
        if (correctValue == 0)
            return (0, 100);

        var deviation = Math.Abs(guess - correctValue);
        var deviationPercent = (deviation / Math.Abs(correctValue)) * 100;

        if (deviationPercent == 0)
            return (MaxPoints, 0);

        if (deviationPercent >= tolerancePercent)
            return (0, deviationPercent);

        // Linear distribution: 10 points at 0% deviation, 0 points at tolerance%
        var ratio = 1 - (deviationPercent / tolerancePercent);
        var points = (int)(MaxPoints * ratio);
        return (Math.Max(0, points), deviationPercent);
    }
}
