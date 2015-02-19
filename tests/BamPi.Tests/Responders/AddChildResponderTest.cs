using BamPi.Responders;
using BamPi.TestFramework;
using Moq;
using Nancy.Testing;
using Xunit;

namespace BamPi.Tests.Responders
{
    public class AddChildResponderTest
    {
        [Fact]
        private void Shoud_add_child()
        {
            var apiDefinition = new ApiDefinition();
            var dataSourceMoq = new Mock<IBamPiDataContext>();
            var mockUser = new User();
            var mockChildUser = new UserChild {Id = 2, Name = "Yannis"};
            dataSourceMoq.Setup(_ => _.Get<User>("myId")).
                ReturnsAsync(mockUser);
            dataSourceMoq.Setup(_ => _.Get<UserChild>("2")).
                ReturnsAsync(mockChildUser);
            dataSourceMoq.Setup(_ => _.AddChild<User, UserChild>("myId", u => u.MyChildren, mockChildUser)).
                ReturnsAsync(mockChildUser).Verifiable();
            apiDefinition.DataContext = dataSourceMoq.Object;
            apiDefinition.Post["/users/{parentId}/children"] = new AddChildResponder<User, UserChild>(_ => _.MyChildren);
            var module = new ConfigurableNancyModule();
            apiDefinition.RegisterNancyModule(module);

            var browser = new BamPiBrowser(_ => _.Module(module));
            var reponse = browser.Post("/users/myId/children", _ => _.JsonBody(mockChildUser));
            Assert.Equal("Yannis", reponse.AsJson<UserChild>().Name);
            dataSourceMoq.Verify();
        }
    }
}