using System;
using System.Threading.Tasks;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;

namespace BamPi.Responders
{
    public class UpdateResponder<T> : Responder where T : class
    {
        public override async Task<object> Execute(NancyModule context, dynamic parameters, ApiDefinition definition)
        {
            if (parameters.id == null)
            {
                throw new ArgumentException("The id parameter must be set in an update operation.");
            }

            var obj = JsonConvert.DeserializeObject<T>(context.Request.Body.AsString());
            return await definition.DataContext.Update(parameters.id, obj);
        }
    }
}