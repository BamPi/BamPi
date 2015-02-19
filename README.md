# BamPi
BamPi is a .NET library for quickly creating a REST API based on a existing model.

## Getting Started
Currently BamPi needs NancyFx and the Entity Framework to work. 
Apart of that getting started with BamPi is straight forward:
* Design your model and crated classes for it
* Create an Entity Framework data context
* Definine the routes in an ApiDefinition

BamPi cares about the rest a provides you a full REST API based on your Model.

### Step-by-step
0. Create a new Nancy project
0. Install `BamPi` and `BamPi.EntityFramework` with `Install-Package BamPi` and `Install-Package BamPi.EntityFramework`
0. Create your model classes
0. Create an Entity Frameowrk [`DbContext`  class](https://msdn.microsoft.com/en-us/data/jj729737.aspx)
0. Create a class inherting from `BamPi.ApiDefinition`
```C# 
public class DemoApiDefintion : ApiDefinition
{
  public DemoApiDefintion()
  {
    DataSource = new BamPiEfDataConext(() => new MyDbContext());
  
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
```

To finish register your Api by running  `new DemoApiDefintion().RegisterNancyModule(this);` 
in the constructor of your `NancyModule`.

**Now enjoy your fully working REST API**
