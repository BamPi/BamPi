using System;

namespace BamPi.IntegrationTests
{
    public class IntegrationTestsApiDefinition : ApiDefinition
    {
        public IntegrationTestsApiDefinition()
        {
            Get["/users"] = Query<User>();
            Post["/users"] = Create<User>(_ => _.Id = Guid.NewGuid());
            Put["/users/{id}"] = Update<User>();
            Delete["/users/{id}"] = Remove<User>();

            Get["/groups"] = Query<Group>();
            Post["/groups"] = Create<Group>(_ => _.Id = Guid.NewGuid());
            Put["/groups/{id}"] = Update<Group>();
            Delete["/groups/{id}"] = Remove<Group>();

            Get["/users/{parentId}/groups"] = QueryChild<User, Group>(_ => _.Groups);
            Post["/users/{parentId}/groups"] = AddChild<User, Group>(_ => _.Groups);
            Delete["/users/{parentId}/groups/{id}"] = RemoveChild<User, Group>(_ => _.Groups);

            Get["/groups/{parentId}/users"] = QueryChild<Group, User>(_ => _.Users);
            Post["/groups/{parentId}/users"] = AddChild<Group, User>(_ => _.Users);
            Delete["/groups/{parentId}/users/{id}"] = RemoveChild<Group, User>(_ => _.Users);
        }
    }
}