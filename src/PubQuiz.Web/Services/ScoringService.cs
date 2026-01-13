namespace PubQuiz.Web.Services;

public class ScoringService
{
    private const int BasePoints = 1000;
    private const int PointsPerSecond = 10;
    private const int MinimumPoints = 100;
    private const int OpenEndedPoints = 500;

    public int CalculatePoints(bool isCorrect, double secondsTaken)
    {
        if (!isCorrect)
            return 0;

        var points = BasePoints - (int)(secondsTaken * PointsPerSecond);
        return Math.Max(MinimumPoints, points);
    }

    public int CalculateOpenEndedPoints(bool isApproved)
    {
        return isApproved ? OpenEndedPoints : 0;
    }

    public int CalculateWordlePoints(bool solved, int attemptsUsed, int maxAttempts = 6)
    {
        if (!solved)
            return 0;

        var pointsPerAttempt = (BasePoints - MinimumPoints) / (maxAttempts - 1);
        return BasePoints - ((attemptsUsed - 1) * pointsPerAttempt);
    }

    public (int points, decimal deviationPercent) CalculateEstimatePoints(decimal guess, decimal correctValue, decimal tolerancePercent)
    {
        if (correctValue == 0)
            return (0, 100);

        var deviation = Math.Abs(guess - correctValue);
        var deviationPercent = (deviation / Math.Abs(correctValue)) * 100;

        if (deviationPercent == 0)
            return (BasePoints, 0);

        if (deviationPercent >= tolerancePercent)
            return (0, deviationPercent);

        var ratio = 1 - (deviationPercent / tolerancePercent);
        var points = (int)(BasePoints * ratio);
        return (Math.Max(0, points), deviationPercent);
    }
}
