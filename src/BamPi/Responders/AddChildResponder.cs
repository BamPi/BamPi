using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json.Linq;

namespace BamPi.Responders
{
    public class AddChildResponder<TParent, TChild> : Responder where TChild : class where TParent : class
    {
        private readonly Expression<Func<TParent, ICollection<TChild>>> property;

        public AddChildResponder(Expression<Func<TParent, ICollection<TChild>>> property)
        {
            this.property = property;
        }

        public override async Task<object> Execute(NancyModule context, dynamic parameters, ApiDefinition definition)
        {
            var dataSource = definition.DataContext;

            if (parameters.parentId == null)
            {
                throw new ArgumentException("parentId parameter must be passed to RemoveChildeResponder");
            }
            var id = JObject.Parse(context.Request.Body.AsString())["id"];
            if (id == null || string.IsNullOrEmpty(id.Value<string>()))
            {
                throw new ArgumentException("Content of request must contain an object with a id property.");
            }
            var child = await dataSource.Get<TChild>(id.Value<string>());
            return await dataSource.AddChild(parameters.parentId, property, child);
        }
    }
}