using Nancy.Testing;
using Newtonsoft.Json;

namespace BamPi.TestFramework
{
    public static class NancyExtesion
    {
        public static T AsJson<T>(this BrowserResponse resp)
        {
            return JsonConvert.DeserializeObject<T>(resp.Body.AsString());
        }
    }
}