namespace ExpressionEvaluator;
internal static class Functions
{
    public static Value Add(IReadOnlyList<Value> arguments)
    {
        if (arguments.All(value => value.Raw is int))
        {
            var result = 0;
            foreach (var value in arguments)
                result = checked(result + value.Get<int>());
            return new Value(result);
        }
        var sum = 0d;
        foreach (var value in arguments)
            sum += value.Raw switch
            {
                int integer => integer,
                double number => number,
                float number => number,
                _ => throw new ArgumentException("add requires numeric arguments")
            };
        return new Value(sum);
    }
    public static Value Equals(Value left, Value right) => new(AreEqual(left.Raw, right.Raw));
    private static bool AreEqual(object? left, object? right)
    {
        if (left is Array leftArray && right is Array rightArray)
            return leftArray.Length == rightArray.Length
                && leftArray.Cast<object?>().Zip(rightArray.Cast<object?>()).All(pair => AreEqual(pair.First, pair.Second));
        if (left is int or double or float && right is int or double or float)
            return Convert.ToDouble(left) == Convert.ToDouble(right);
        return object.Equals(left, right);
    }
    public static Value Not(Value argument)
    {
        if (argument.Raw is not bool boolean)
            throw new ArgumentException("not requires a boolean argument");
        return new Value(!boolean);
    }
    public static Value Contains(Value text, Value substring)
    {
        if (text.Raw is not string source || substring.Raw is not string search)
            throw new ArgumentException("contains requires two string arguments");
        return new Value(source.Contains(search, StringComparison.Ordinal));
    }
    public static string Url(Value value)
    {
        if (value.Raw is not string address || !Uri.TryCreate(address, UriKind.Absolute, out var uri)
            || uri.Scheme is not ("http" or "https"))
            throw new ArgumentException("fetchGet requires an absolute HTTP or HTTPS URL");
        return address;
    }
}
