using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Session;
using Nancy.TinyIoc;

namespace DotNetMalaga.AkkaExample.NancyWebClient
{
    using Nancy;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            this.Conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Scripts"));
            this.Conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Content"));
            this.Conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("app"));
            CookieBasedSessions.Enable(pipelines);
        }
    }
}