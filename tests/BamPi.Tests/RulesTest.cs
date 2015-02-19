using System.Collections.Generic;
using Moq;
using Nancy.Testing;
using Xunit;

namespace BamPi.Tests
{
    public class RulesTest
    {
        [Fact]
        public void It_should_call_after_and_before_rules()
        {
            var moqResponder = new Mock<IResponder>();
            var moqBeforeRule = new Mock<IBeforeRule>();
            var moqAfterRule = new Mock<IAfterRule>();
            moqResponder.SetupGet(_ => _.Rules).
                Returns(new List<IRule> {moqAfterRule.Object, moqBeforeRule.Object});

            var apiDefinition = new ApiDefinition();
            apiDefinition.Get["/users"] = moqResponder.Object;
            var module = new ConfigurableNancyModule();
            apiDefinition.RegisterNancyModule(module);

            moqResponder.Setup(_ => _.Execute(module, It.IsAny<object>(), apiDefinition)).ReturnsAsync(null).
                Verifiable();
            moqBeforeRule.Setup(_ => _.Before(module, It.IsAny<object>(), apiDefinition)).
                Verifiable();
            moqAfterRule.Setup(_ => _.After(module, It.IsAny<object>(), apiDefinition, It.IsAny<object>())).
                Verifiable();

            var boot = new ConfigurableBootstrapper(_ => _.Module(module));
            new Browser(boot).Get("/users");

            moqResponder.Verify();
            moqBeforeRule.Verify();
            moqAfterRule.Verify();
        }
    }
}