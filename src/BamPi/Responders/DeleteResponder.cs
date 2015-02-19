using System;
using System.Threading.Tasks;
using Nancy;

namespace BamPi.Responders
{
    public class DeleteResponder<T> : Responder where T : class
    {
        public override async Task<object> Execute(NancyModule context, dynamic parameters, ApiDefinition definition)
        {
            if (parameters.id.HasValue == null)
            {
                throw new ArgumentException("ID must be set.");
            }
            return await definition.DataContext.Delete<T>(parameters.id);
        }
    }
}