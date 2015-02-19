using System.Collections.Generic;
using System.Threading.Tasks;
using Nancy;

namespace BamPi
{
    public abstract class Responder : IResponder
    {
        private readonly List<IRule> rules;

        protected Responder()
        {
            rules = new List<IRule>();
        }

        public IEnumerable<IRule> Rules
        {
            get { return rules; }
        }

        public abstract Task<object> Execute(NancyModule context, dynamic parameters, ApiDefinition definition);

        public Responder WithRule<T>(T rule) where T : IRule
        {
            rules.Add(rule);
            return this;
        }
    }
}