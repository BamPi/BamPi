using BamPi.Responders;
using BamPi.TestFramework;
using Moq;
using Nancy.Testing;
using Xunit;

namespace BamPi.Tests.Responders
{
    public class DeleteResponderTest
    {
        [Fact]
        public void It_should_delete_when_passing_id()
        {
            {
                var apiDefinition = new ApiDefinition();
                var dataSourceMoq = new Mock<IBamPiDataContext>();
                dataSourceMoq.Setup(_ => _.Delete<User>("myId")).
                    ReturnsAsync(true).Verifiable();
                apiDefinition.DataContext = dataSourceMoq.Object;
                apiDefinition.Delete["/users/{id}"] = new DeleteResponder<User>();
                var module = new ConfigurableNancyModule();
                apiDefinition.RegisterNancyModule(module);

                var browser = new BamPiBrowser(_ => _.Module(module));
                var reponse = browser.Delete("/users/myId");
                Assert.Equal("true", reponse.Body.AsString());
                dataSourceMoq.Verify();
            }
        }
    }
}