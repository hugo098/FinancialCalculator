using BuildingBlocks.GoalSeek.Abstractions;

namespace BuildingBlocks.GoalSeek;
public class GoalSeek(IGoalSeek iGoalSeek)
{
    public const decimal DefaultAccuracyLevel = 0.0000001m;
    public const int DefaultMaxIterations = 25;
    public const bool DefaultResultRoundOff = true;

    public int MaxIterations { get; set; } = DefaultMaxIterations;
    public bool ResultRoundOff { get; set; } = DefaultResultRoundOff;
    public List<decimal> AccuracyLevels { get; } = [DefaultAccuracyLevel];

    private readonly Func<decimal, decimal> func = iGoalSeek.Calculate;

    public static GoalSeekResult? TrySeek(
        Func<decimal, decimal> func,
        List<decimal> accuracyLevels,
        decimal targetValue = 0,
        decimal initialGuess = 0,
        int maxIterations = DefaultMaxIterations,
        bool resultRoundOff = DefaultResultRoundOff)
    {
        if (accuracyLevels is null || accuracyLevels.Count == 0)
            throw new Exception($"There should be at least one accuracy level");

        accuracyLevels = accuracyLevels?.OrderBy(o => o).ToList()!;

        int iterations = 0;

        for (var i = 0; i < accuracyLevels.Count; i++)
        {
            var accuracyLevel = accuracyLevels[i];

            var goalSeekResult = TrySeek(
                func: func,
                accuracyLevel: accuracyLevel,
                targetValue: targetValue,
                initialGuess: initialGuess,
                maxIterations: maxIterations,
                resultRoundOff: resultRoundOff);

            iterations += goalSeekResult.Iterations;

            if (goalSeekResult.IsGoalReached || i == (accuracyLevels.Count - 1))
                return new GoalSeekResult(
                    targetValue: goalSeekResult.TargetValue, 
                    accuracyLevel: goalSeekResult.AccuracyLevel,
                    iterations: iterations,
                    isGoalReached: goalSeekResult.IsGoalReached,
                    closestValue: goalSeekResult.ClosestValue);
        }

        return null;
    }

    public static GoalSeekResult TrySeek(
        Func<decimal, decimal> func,
        decimal accuracyLevel = DefaultAccuracyLevel,
        decimal targetValue = 0,
        decimal initialGuess = 0,
        int maxIterations = DefaultMaxIterations,
        bool resultRoundOff = DefaultResultRoundOff)
    {
        const decimal delta = 0.0001m;     

        int iterations = 0;

        decimal result1 = func(initialGuess) - targetValue;

        while (Math.Abs(result1) > accuracyLevel && iterations++ < maxIterations)
        {
            decimal newGuess = initialGuess + delta;
            decimal result2 = func(newGuess) - targetValue;

            if ((result2 - result1) == 0)
            {
                break;
            }

            decimal reciprocalSlope = (newGuess - initialGuess) / (result2 - result1);
            initialGuess -= result1 * reciprocalSlope;

            result1 = func(initialGuess) - targetValue;
        }

        if (iterations > maxIterations)
            iterations = maxIterations;

        if (resultRoundOff)
            initialGuess = Math.Round(initialGuess, accuracyLevel.ToString().Length - (accuracyLevel.ToString().IndexOf('.') + 1));

        return new GoalSeekResult(
            targetValue: targetValue,
            accuracyLevel: accuracyLevel,
            iterations: iterations,
            isGoalReached: Math.Abs(result1) <= accuracyLevel,
            closestValue: initialGuess);
    }

    public GoalSeekResult? TrySeek(decimal targetValue = 0, decimal initialGuess = 0)
    {
        return TrySeek(
            func: func,
            accuracyLevels: AccuracyLevels,
            targetValue: targetValue,
            initialGuess: initialGuess,
            maxIterations: MaxIterations,
            resultRoundOff: ResultRoundOff);
    }
}