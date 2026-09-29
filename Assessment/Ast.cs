namespace ExpressionEvaluator
{
    public abstract class Node;

    public class Value
    {
        private readonly object _value;

        public Value(string value)
        {
            _value = value;
        }

        public Value(int value)
        {
            _value = value;
        }

        public Value(double value)
        {
            _value = value;
        }

        public Value(DateTime value)
        {
            _value = value;
        }

        public Value(bool value)
        {
            _value = value;
        }

        public Value(Array value)
        {
            _value = value;
        }

        public T Get<T>() => (T)_value;
    }

    public class Literal(Value value) : Node
    {
        public Value Value = value;
    }

    public class Function(string name, List<Node> parameters) : Node
    {
        public string Name = name;
        public List<Node> Parameters = parameters;
    }
}
