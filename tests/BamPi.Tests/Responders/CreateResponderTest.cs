using System;
using BamPi.Responders;
using BamPi.TestFramework;
using Moq;
using Nancy.Testing;
using Xunit;

namespace BamPi.Tests.Responders
{
    public class CreateResponderTest
    {
        [Fact]
        public void When_creating_it_should_call_updater()
        {
            var apiDefinition = new ApiDefinition();
            var dataSourceMoq = new Mock<IBamPiDataContext>();

            dataSourceMoq.Setup(_ => _.Add
                (It.Is<User>(u => u.Name == "Changed")))
                .ReturnsAsync(new User {Name = "Changed"})
                .Verifiable();

            apiDefinition.DataContext = dataSourceMoq.Object;

            Action<User> action = _ =>
            {
                Assert.Equal("Yannis", _.Name);
                _.Name = "Changed";
            };
            apiDefinition.Post["/users"] = new CreateResponder<User>(action);
            var module = new ConfigurableNancyModule();
            apiDefinition.RegisterNancyModule(module);

            var browser = new BamPiBrowser(_ => _.Module(module));
            var reponse = browser.Post("/users",
                _ => _.JsonBody(new User {Name = "Yannis"}));
            Assert.Equal("Changed", reponse.AsJson<User>().Name);

            dataSourceMoq.Verify();
        }
    }
}