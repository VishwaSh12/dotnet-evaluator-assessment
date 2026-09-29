namespace ExpressionEvaluator
{
    public static class Evaluator
    {
        public static Value Evaluate(Node expression)
        {
            if (expression.GetType() == typeof(Literal))
            {
                var literal = (Literal)expression;
                return literal.Value;
            }
            else
            {
                var function = expression as Function;
                if (function!.Name == "add")
                {
                    var param1 = Evaluate(function.Parameters[0]);
                    var param2 = Evaluate(function.Parameters[1]);
                    return Functions.Add(param1, param2);
                }
                else if (function.Name == "equals")
                {
                    var param1 = Evaluate(function.Parameters[0]);
                    var param2 = Evaluate(function.Parameters[1]);
                    return Functions.Equals(param1, param2);
                }
                else if (function.Name == "not")
                {
                    var param1 = Evaluate(function.Parameters[0]);
                    return Functions.Not(function.Parameters[0]);
                }

                throw new Exception("Unknown function");
            }
        }


    }
}
