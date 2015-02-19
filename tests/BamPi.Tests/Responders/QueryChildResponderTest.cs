using System;
using System.Collections.Generic;
using System.Linq;
using BamPi.Responders;
using BamPi.TestFramework;
using Moq;
using Nancy.Testing;
using Xunit;

namespace BamPi.Tests.Responders
{
    public class QueryChildResponderTest
    {
        [Fact]
        private void Should_index_all_children()
        {
            var apiDefinition = new ApiDefinition();
            var dataSourceMoq = new Mock<IBamPiDataContext>();

            dataSourceMoq.Setup(
                _ =>
                    _.QueryChildren<User, UserChild>("myId", u => u.MyChildren,
                        It.IsAny<Func<IQueryable<UserChild>, IQueryable<UserChild>>>())).
                ReturnsAsync(new List<UserChild>
                {
                    new UserChild {Name = "Yannis"},
                    new UserChild {Name = "Paul"}
                }).Verifiable();

            apiDefinition.DataContext = dataSourceMoq.Object;
            apiDefinition.Get["/users/{parentId}/children"] = new QueryChildResponder<User, UserChild>(_ => _.MyChildren);
            var module = new ConfigurableNancyModule();
            apiDefinition.RegisterNancyModule(module);

            var browser = new BamPiBrowser(_ => _.Module(module));
            var reponse = browser.Get("/users/myId/children");
            var children = reponse.AsJson<List<UserChild>>();
            Assert.Equal(2, children.Count);
            Assert.Equal("Yannis", children[0].Name);
            Assert.Equal("Paul", children[1].Name);
            dataSourceMoq.Verify();
        }
    }
}