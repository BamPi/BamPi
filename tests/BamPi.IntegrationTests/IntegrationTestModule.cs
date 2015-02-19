using Nancy;

namespace BamPi.IntegrationTests
{
    public class IntegrationTestModule : NancyModule
    {
        public IntegrationTestModule(IBamPiDataContext dataContext)
        {
            new IntegrationTestsApiDefinition {DataContext = dataContext}.RegisterNancyModule(this);
        }
    }
}