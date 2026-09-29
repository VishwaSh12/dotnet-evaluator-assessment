namespace ExpressionEvaluator;
public abstract class Node;
public sealed class Value
{
    private readonly object? _value;
    public Value(object? value)
    {
        if (value is not null && value is not string and not bool and not int and not double and not float and not DateTime and not Array)
            throw new ArgumentException($"Unsupported value type: {value.GetType().Name}", nameof(value));
        _value = value;
    }
    public T Get<T>() => (T)_value!;
    internal object? Raw => _value;
}
public sealed class Literal(Value value) : Node
{
    public Value Value { get; } = value ?? throw new ArgumentNullException(nameof(value));
}
public sealed class Function(string name, List<Node> parameters) : Node
{
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));
    public IReadOnlyList<Node> Parameters { get; } = parameters ?? throw new ArgumentNullException(nameof(parameters));
}
