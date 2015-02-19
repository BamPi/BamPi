using System;
using System.Threading.Tasks;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;

namespace BamPi.Responders
{
    public class CreateResponder<T> : Responder where T : class
    {
        private readonly Action<T> updater;

        public CreateResponder(Action<T> updater = null)
        {
            this.updater = updater;
        }

        public override async Task<object> Execute(NancyModule context, dynamic parameters, ApiDefinition definition)
        {
            var obj = JsonConvert.DeserializeObject<T>(context.Request.Body.AsString());
            if (updater != null)
            {
                updater(obj);
            }
            return await definition.DataContext.Add(obj);
        }
    }
}