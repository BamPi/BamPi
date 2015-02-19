using System;
using Moq;
using Nancy.Testing;
using Xunit;

namespace BamPi.Tests
{
    public class ApiDefinitionTests
    {
        [Fact]
        public void Should_return_all_added_operations()
        {
            var apiDefinition = new ApiDefinition();
            var moq = new Mock<IResponder>();
            apiDefinition.Get["/users"] = moq.Object;
            apiDefinition.Post["/users"] = moq.Object;
            apiDefinition.Options["/users"] = moq.Object;
            apiDefinition.Delete["/users"] = moq.Object;
            apiDefinition.Put["/users"] = moq.Object;

            Func<ApiOperation, string, bool> isMatch =
                (operation, method) =>
                    operation.Method == method && operation.Route == "/users" &&
                    operation.Responder == moq.Object;

            Assert.Equal(5, apiDefinition.Routes.Count);
            Assert.Contains(apiDefinition.Routes, _ => isMatch(_, "GET"));
            Assert.Contains(apiDefinition.Routes, _ => isMatch(_, "POST"));
            Assert.Contains(apiDefinition.Routes, _ => isMatch(_, "OPTIONS"));
            Assert.Contains(apiDefinition.Routes, _ => isMatch(_, "PUT"));
            Assert.Contains(apiDefinition.Routes, _ => isMatch(_, "DELETE"));
        }

        [Fact]
        public void Nancy_api_should_respond()
        {
            var moqResponder = new Mock<IResponder>();

            var apiDefinition = new ApiDefinition();
            apiDefinition.Get["/users"] = moqResponder.Object;
            var module = new ConfigurableNancyModule();
            apiDefinition.RegisterNancyModule(module);

            moqResponder.Setup(_ => _.Execute(module, It.IsAny<object>(), apiDefinition)).ReturnsAsync(null).
                Verifiable();

            var boot = new ConfigurableBootstrapper(_ => _.Module(module));
            var browser = new Browser(boot).Get("/users");

            moqResponder.Verify();
        }
    }
}