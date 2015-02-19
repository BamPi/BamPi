using BamPi.Responders;
using BamPi.TestFramework;
using Moq;
using Nancy.Testing;
using Xunit;

namespace BamPi.Tests.Responders
{
    public class RemoveChildResponderTest
    {
        [Fact]
        public void It_should_remove_child()
        {
            {
                var apiDefinition = new ApiDefinition();
                var dataSourceMoq = new Mock<IBamPiDataContext>();
                var mockUser = new User();
                var mockChildUser = new UserChild {Name = "Yannis"};
                dataSourceMoq.Setup(_ => _.Get<User>("myId")).
                    ReturnsAsync(mockUser);
                dataSourceMoq.Setup(_ => _.Get<UserChild>("childId")).
                    ReturnsAsync(mockChildUser);
                dataSourceMoq.Setup(_ => _.RemoveChild<User, UserChild>("myId", u => u.MyChildren, mockChildUser)).
                    ReturnsAsync(true).Verifiable();
                apiDefinition.DataContext = dataSourceMoq.Object;
                apiDefinition.Delete["/users/{parentId}/children/{id}"] =
                    new RemoveChildResponder<User, UserChild>(_ => _.MyChildren);
                var module = new ConfigurableNancyModule();
                apiDefinition.RegisterNancyModule(module);

                var browser = new BamPiBrowser(_ => _.Module(module));
                var reponse = browser.Delete("/users/myId/children/childId");
                Assert.Equal("true", reponse.Body.AsString());
                dataSourceMoq.Verify();
            }
        }
    }
}