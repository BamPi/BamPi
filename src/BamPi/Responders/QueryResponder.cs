using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Nancy;

namespace BamPi.Responders
{
    public class QueryResponder<TType> : Responder where TType : class
    {
        private readonly Expression<Func<TType, bool>> filter;

        public QueryResponder() : this(_ => true)
        {
        }

        public QueryResponder(Expression<Func<TType, bool>> filter)
        {
            this.filter = filter;
        }

        public override async Task<object> Execute(NancyModule context, dynamic parameters, ApiDefinition definition)
        {
            return await definition.DataContext.Query<TType>(_ => _.Where(filter));
        }
    }
}