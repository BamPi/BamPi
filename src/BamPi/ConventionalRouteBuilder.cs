namespace BamPi
{
    public class ConventionalRouteBuilder
    {
        private readonly ApiDefinition apiDefinition;
        private readonly string method;

        public ConventionalRouteBuilder(string method, ApiDefinition apiDefinition)
        {
            this.method = method;
            this.apiDefinition = apiDefinition;
        }

        public IResponder this[string route]
        {
            set { apiDefinition.Routes.Add(new ApiOperation(method, route, value)); }
        }
    }
}