namespace ExpressionEvaluator;
public static class Evaluator
{
    private static readonly HttpClient SharedClient = new() { Timeout = TimeSpan.FromSeconds(15) };
    public static Value Evaluate(Node expression) => EvaluateAsync(expression).GetAwaiter().GetResult();
    public static async Task<Value> EvaluateAsync(Node expression, HttpClient? client = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(expression);
        if (expression is Literal literal)
            return literal.Value;
        if (expression is not Function function)
            throw new ArgumentException("Unknown expression node", nameof(expression));
        var (minimum, maximum) = function.Name switch
        {
            "add" => (2, int.MaxValue),
            "equals" or "contains" => (2, 2),
            "not" or "fetchGet" => (1, 1),
            _ => throw new ArgumentException($"Unknown function: {function.Name}", nameof(expression))
        };
        if (function.Parameters.Count < minimum || function.Parameters.Count > maximum)
            throw new ArgumentException($"{function.Name} expects {minimum}" +
                (maximum == int.MaxValue ? " or more" : "") + " parameter(s)", nameof(expression));
        var values = new List<Value>(function.Parameters.Count);
        foreach (var parameter in function.Parameters)
            values.Add(await EvaluateAsync(parameter, client, cancellationToken).ConfigureAwait(false));
        return function.Name switch
        {
            "add" => Functions.Add(values),
            "equals" => Functions.Equals(values[0], values[1]),
            "not" => Functions.Not(values[0]),
            "contains" => Functions.Contains(values[0], values[1]),
            "fetchGet" => new Value(await (client ?? SharedClient)
                .GetStringAsync(Functions.Url(values[0]), cancellationToken).ConfigureAwait(false)),
            _ => throw new InvalidOperationException("Unreachable function")
        };
    }
}
