using System.Collections.Generic;
using System.Threading.Tasks;
using Nancy;

namespace BamPi
{
    public interface IResponder
    {
        IEnumerable<IRule> Rules { get; }
        Task<object> Execute(NancyModule context, dynamic parameters, ApiDefinition definition);
    }
}