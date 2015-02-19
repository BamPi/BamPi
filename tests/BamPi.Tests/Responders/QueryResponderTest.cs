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
    public class QueryResponderTest
    {
        private readonly IQueryable<User> defaultUsers =
            new List<User>
            {
                new User {Name = "Yannis"},
                new User {Name = "Paul"}
            }.AsQueryable();

        [Fact]
        public void When_configured_it_should_return_all_items()
        {
            var apiDefinition = new ApiDefinition();
            var dataSourceMoq = new Mock<IBamPiDataContext>();
            dataSourceMoq.Setup(_ => _.Query(It.IsAny<Func<IQueryable<User>, IQueryable<User>>>()))
                .ReturnsAsync(defaultUsers);
            apiDefinition.DataContext = dataSourceMoq.Object;
            apiDefinition.Get["/users"] = new QueryResponder<User>();
            var module = new ConfigurableNancyModule();
            apiDefinition.RegisterNancyModule(module);

            var browser = new BamPiBrowser(_ => _.Module(module));
            var reponse = browser.Get("/users");
            var users = reponse.AsJson<List<User>>();
            Assert.Equal(defaultUsers.Count(), users.Count());
            for (var i = 0; i < defaultUsers.Count(); i++)
            {
                Assert.Equal(defaultUsers.ToList()[i].Name, users[i].Name);
            }
        }
    }

    public class User
    {
        public string Name { get; set; }
        public ICollection<UserChild> MyChildren { get; set; }
    }

    public class UserChild
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}