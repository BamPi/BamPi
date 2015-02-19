using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Nancy;

namespace BamPi.Responders
{
    public class QueryChildResponder<TParent, TChild> : Responder where TParent : class where TChild : class
    {
        private readonly Expression<Func<TParent, ICollection<TChild>>> childProperty;
        private readonly Expression<Func<TChild, bool>> filter;

        public QueryChildResponder(Expression<Func<TParent, ICollection<TChild>>> childProperty)
            : this(childProperty, _ => true)
        {
        }

        public QueryChildResponder(Expression<Func<TParent, ICollection<TChild>>> childProperty,
            Expression<Func<TChild, bool>> filter)
        {
            this.childProperty = childProperty;
            this.filter = filter;
        }

        public override async Task<object> Execute(NancyModule context, dynamic parameters, ApiDefinition definition)
        {
            if (parameters.parentId == null)
            {
                throw new ArgumentException("parentId parameter must be passed to RemoveChildeResponder");
            }
            return
                await
                    definition.DataContext.QueryChildren((string) parameters.parentId, childProperty,
                        _ => _.Where(filter));
        }
    }
}