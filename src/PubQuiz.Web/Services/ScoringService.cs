namespace PubQuiz.Web.Services;

public class ScoringService
{
    private const int BasePoints = 1000;
    private const int PointsPerSecond = 10;
    private const int MinimumPoints = 100;

    public int CalculatePoints(bool isCorrect, double secondsTaken)
    {
        if (!isCorrect)
            return 0;

        var points = BasePoints - (int)(secondsTaken * PointsPerSecond);
        return Math.Max(MinimumPoints, points);
    }
}
