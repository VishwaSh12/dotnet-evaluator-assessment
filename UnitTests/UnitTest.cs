using ExpressionEvaluator;
using System.Net;
using System.Net.Http;

namespace UnitTests;

[TestClass]
public class UnitTest
{
    private static Literal L(object? value) => new(new Value(value));
    private static Function F(string name, params Node[] nodes) => new(name, nodes.ToList());

    [TestMethod]
    public void Literals_And_Nested_Not()
    {
        Assert.AreEqual(13, Evaluator.Evaluate(L(13)).Get<int>());
        Assert.IsTrue(Evaluator.Evaluate(F("not", F("equals", L(1), L(2)))).Get<bool>());
        Assert.ThrowsException<InvalidCastException>(() => Evaluator.Evaluate(L("")).Get<int>());
    }

    [TestMethod]
    public void Add_Supports_Integers_Mixed_Numbers_And_Multiple_Arguments()
    {
        Assert.AreEqual(9, Evaluator.Evaluate(F("add", L(3), L(6))).Get<int>());
        Assert.AreEqual(0.9, Evaluator.Evaluate(F("add", L(0.3), L(0.6))).Get<double>(), 1e-10);
        Assert.AreEqual(6.5, Evaluator.Evaluate(F("add", L(1), L(2.5), L(3))).Get<double>());
        Assert.ThrowsException<ArgumentException>(() => Evaluator.Evaluate(F("add", L("x"), L(1))));
        Assert.ThrowsException<OverflowException>(() => Evaluator.Evaluate(F("add", L(int.MaxValue), L(1))));
    }

    [TestMethod]
    public void Equals_Handles_Null_Numbers_And_Arrays()
    {
        Assert.IsTrue(Evaluator.Evaluate(F("equals", L(null), L(null))).Get<bool>());
        Assert.IsTrue(Evaluator.Evaluate(F("equals", L(2), L(2d))).Get<bool>());
        Assert.IsTrue(Evaluator.Evaluate(F("equals", L(new[] { 1, 2 }), L(new[] { 1, 2 }))).Get<bool>());
        Assert.IsFalse(Evaluator.Evaluate(F("equals", L("2"), L(2))).Get<bool>());
    }

    [TestMethod]
    public void Contains_Uses_Ordinal_Case_Sensitive_Matching()
    {
        Assert.IsTrue(Evaluator.Evaluate(F("contains", L("Hello Bing"), L("Bing"))).Get<bool>());
        Assert.IsFalse(Evaluator.Evaluate(F("contains", L("Hello Bing"), L("bing"))).Get<bool>());
        Assert.ThrowsException<ArgumentException>(() => Evaluator.Evaluate(F("contains", L(1), L("1"))));
    }

    [TestMethod]
    public async Task FetchGet_Composes_With_Contains_Without_External_Network()
    {
        using var client = new HttpClient(new StubHandler());
        var expression = F("contains", F("fetchGet", L("https://example.com")), L("Bing"));
        Assert.IsTrue((await Evaluator.EvaluateAsync(expression, client)).Get<bool>());
    }

    [TestMethod]
    public async Task FetchGet_Rejects_NonHttp_Urls()
    {
        using var client = new HttpClient(new StubHandler());
        await Assert.ThrowsExceptionAsync<ArgumentException>(
            () => Evaluator.EvaluateAsync(F("fetchGet", L("file:///etc/passwd")), client));
    }

    [TestMethod]
    public void Invalid_Functions_And_Arity_Are_Reported()
    {
        Assert.ThrowsException<ArgumentException>(() => Evaluator.Evaluate(F("toString")));
        Assert.ThrowsException<ArgumentException>(() => Evaluator.Evaluate(F("not")));
        Assert.ThrowsException<ArgumentException>(() => Evaluator.Evaluate(F("equals", L(1))));
        Assert.ThrowsException<ArgumentException>(() => Evaluator.Evaluate(F("not", L(1))));
    }

    private sealed class StubHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Assert.AreEqual(HttpMethod.Get, request.Method);
            Assert.AreEqual("https://example.com/", request.RequestUri!.ToString());
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Welcome to Bing")
            });
        }
    }
}
