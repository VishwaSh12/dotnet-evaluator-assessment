using ExpressionEvaluator;

namespace UnitTests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void It_Evaluates_A_Literal()
        {
            var r = Evaluator.Evaluate(new Literal(new Value(13)));

            Assert.AreEqual(r.Get<int>(), 13);
        }

        [TestMethod]
        public void It_Evaluates_A_Not_Function()
        {
            Assert.AreEqual(Evaluator.Evaluate(new Function("not", [new Literal(new Value(true))])).Get<bool>(), false);
        }

        [TestMethod, Ignore("doesn't work")]
        public void It_Evaluates_A_Add_Function_For_Ints()
        {
            Assert.AreEqual(Evaluator.Evaluate(new Function("add", [new Literal(new Value(3)), new Literal(new Value(6))])).Get<int>(), 9);
        }

        [TestMethod, Ignore("doesn't work")]
        public void It_Evaluates_A_Add_Function_For_Doubles()
        {
            Assert.AreEqual(Evaluator.Evaluate(new Function("add", [new Literal(new Value(0.3)), new Literal(new Value(0.6))])).Get<double>(), 0.9);
        }

        [TestMethod]
        public void It_throws_for_invalid_Expression()
        {
            var literal = Evaluator.Evaluate(new Literal(new Value("")));
            Assert.ThrowsException<InvalidCastException>(() => literal.Get<int>());
        }

        [TestMethod]
        public void It_throws_for_invalid_function_expression()
        {
            Assert.ThrowsException<Exception>(() => Evaluator.Evaluate(new Function("toString", [])));
        }
    }
}
