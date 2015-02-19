using System;
using System.Collections.Generic;
using System.Linq;
using BamPi.EntityFramework;
using BamPi.TestFramework;
using Nancy.Testing;
using Shouldly;
using Xunit;

namespace BamPi.IntegrationTests
{
    public class IntegrationTests
    {
        [Fact]
        public void TheBigIntegrationTest()
        {
            using (var dbContext = new IntegrationTestContext())
            {
                dbContext.Users.RemoveRange(dbContext.Users.ToList());
                dbContext.Groups.RemoveRange(dbContext.Groups.ToList());
                dbContext.SaveChanges();
            }

            var browser =
                new BamPiBrowser(
                    _ =>
                    {
                        _.Module(new IntegrationTestModule(new BamPiEfDataConext(() => new IntegrationTestContext())));
                    });
            var newUser = new User {Name = "Peter"};
            var user = browser.Post("/users", _ => _.JsonBody(newUser)).AsJson<User>();
            DbAction(_ => _.Users.Count().ShouldBe(1));

            var newGroup = new Group {Name = "MyGroup"};
            var group = browser.Post("/groups", _ => _.JsonBody(newGroup)).AsJson<Group>();
            DbAction(_ => _.Groups.Count().ShouldBe(1));

            browser.Post("/users/" + user.Id + "/groups", _ => _.JsonBody(group));
            DbAction(_ => _.Users.Include("Groups").First().Groups.Count().ShouldBe(1));
            DbAction(_ => _.Groups.Include("Users").First().Users.Count().ShouldBe(1));

            var groups = browser.Get("/users/" + user.Id + "/groups").AsJson<List<Group>>();
            groups.Count.ShouldBe(1);
            groups[0].Name.ShouldBe("MyGroup");

            var users = browser.Get("/groups/" + group.Id + "/users").AsJson<List<User>>();
            users.Count.ShouldBe(1);
            users[0].Name.ShouldBe("Peter");

            browser.Delete("/users/" + user.Id + "/groups/" + group.Id);
            DbAction(_ => _.Users.Include("Groups").First().Groups.Count().ShouldBe(0));
            DbAction(_ => _.Groups.Include("Users").First().Users.Count().ShouldBe(0));

            var updateUser = browser.Put("/users/" + user.Id, _ => _.JsonBody(new User {Name = "Paul"})).AsJson<User>();
            updateUser.Name.ShouldBe("Paul");
            DbAction(_ => _.Users.Count().ShouldBe(1));
            DbAction(_ => _.Users.First().Name.ShouldBe("Paul"));

            browser.Delete("/users/" + user.Id);
            DbAction(_ => _.Users.Count().ShouldBe(0));

            browser.Delete("/groups/" + group.Id);
            DbAction(_ => _.Groups.Count().ShouldBe(0));
        }

        public static void DbAction(Action<IntegrationTestContext> action)
        {
            using (var dbContext = new IntegrationTestContext())
            {
                action(dbContext);
            }
        }
    }
}