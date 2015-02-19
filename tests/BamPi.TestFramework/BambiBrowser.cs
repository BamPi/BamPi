using System;
using Nancy.Bootstrapper;
using Nancy.Responses.Negotiation;
using Nancy.Testing;

namespace BamPi.TestFramework
{
    public class BamPiBrowser : Browser
    {
        private Action<ConfigurableBootstrapper.ConfigurableBootstrapperConfigurator> myAction;

        public BamPiBrowser(Action<ConfigurableBootstrapper.ConfigurableBootstrapperConfigurator> action,
            Action<BrowserContext> defaults = null) : base(GetAction(action), defaults)
        {
        }

        public BamPiBrowser(INancyBootstrapper bootstrapper, Action<BrowserContext> defaults = null)
            : base(bootstrapper, defaults)
        {
        }

        private static Action<ConfigurableBootstrapper.ConfigurableBootstrapperConfigurator> GetAction(
            Action<ConfigurableBootstrapper.ConfigurableBootstrapperConfigurator> action)
        {
            return _ =>
            {
                _.ResponseProcessors(typeof (JsonProcessor));
                action(_);
            };
        }
    }
}