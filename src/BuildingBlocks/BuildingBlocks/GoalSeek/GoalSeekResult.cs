namespace BuildingBlocks.GoalSeek;
public class GoalSeekResult(
    decimal targetValue,
    decimal accuracyLevel,
    int iterations,
    bool isGoalReached,
    decimal closestValue)
{
    public decimal TargetValue { get; private set; } = targetValue;
    public decimal AccuracyLevel { get; private set; } = accuracyLevel;
    public int Iterations { get; private set; } = iterations;
    public bool IsGoalReached { get; private set; } = isGoalReached;
    public decimal ClosestValue { get; private set; } = closestValue;

    public void Deconstruct(out bool isGoalReached, out decimal closestValue)
    {
        isGoalReached = IsGoalReached;
        closestValue = ClosestValue;
    }

    public void Deconstruct(
        out decimal targetValue,
        out decimal accucracyLevel,
        out int iterations,
        out bool isGoalReached,
        out decimal closestValue)
    {
        targetValue = TargetValue;
        accucracyLevel = AccuracyLevel;
        iterations = Iterations;
        isGoalReached = IsGoalReached;
        closestValue = ClosestValue;
    }
}