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
}
