using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Nancy;

namespace BamPi.Responders
{
    public class RemoveChildResponder<TParent, TChild> : Responder where TChild : class where TParent : class
    {
        private readonly Expression<Func<TParent, ICollection<TChild>>> property;

        public RemoveChildResponder(Expression<Func<TParent, ICollection<TChild>>> property)
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
            if (parameters.id == null)
            {
                throw new ArgumentException("id parameter must be passed to RemoveChildeResponder");
            }
            var child = await dataSource.Get<TChild>(parameters.id);
            return await dataSource.RemoveChild(parameters.parentId, property, child);
        }
    }
}