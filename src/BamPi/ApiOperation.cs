namespace BamPi
{
    public class ApiOperation
    {
        public ApiOperation(string method, string route, IResponder responder)
        {
            Method = method;
            Route = route;
            Responder = responder;
        }

        public string Method { get; private set; }
        public string Route { get; private set; }
        public IResponder Responder { get; private set; }
    }
}