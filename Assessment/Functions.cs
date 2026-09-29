namespace ExpressionEvaluator
{
    internal static class Functions
    {
        public static Value Add(Value param1, Value param2)
        {
            return new Value(param1.Get<double>() + param2.Get<double>());
        }

        public static Value Equals(Value param1, Value param2)
        {
            return new Value(param1.Get<bool>() == param2.Get<bool>());
        }

        public static Value Not(Node arg)
        {
            return new Value(!((Literal)arg).Value.Get<bool>());
        }
    }
}
