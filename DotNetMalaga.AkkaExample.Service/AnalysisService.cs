using Akka.Actor;

namespace DotNetMalaga.AkkaExample.Service
{
    internal class AnalysisService
    {
        protected ActorSystem AnalysisActorSystem;
        protected IActorRef SocialMediaMaster;

        public bool Start()
        {
            AnalysisActorSystem = ActorSystem.Create("analyzer");
            SocialMediaMaster = AnalysisActorSystem.ActorOf(Props.Create(() => new SocialMediaMaster()), "main");

            return true;
        }

        public bool Stop()
        {
            AnalysisActorSystem.Shutdown();
            return true;
        }
    }
}