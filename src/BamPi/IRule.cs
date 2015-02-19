using Nancy;

namespace BamPi
{
    public interface IRule
    {
    }

    public interface IAfterRule : IRule
    {
        void After(NancyModule context, dynamic parameters, ApiDefinition apiDefinition, object returnValue);
    }

    public interface IBeforeRule : IRule
    {
        void Before(NancyModule context, dynamic parameters, ApiDefinition apiDefinition);
    }
}