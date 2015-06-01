using Akka.Actor;
using DotNetMalaga.AkkaExample.WebClient.Actors;

namespace DotNetMalaga.AkkaExample.WebClient
{
    public class ActorConfig
    {
        public static void Register(out ActorSystem actorSystem)
        {
            actorSystem = ActorSystem.Create("webclient");

            SystemActors.SignalRActor = actorSystem.ActorOf(Props.Create(() => new SignalRActor()), "signalr");

            SystemActors.SentimentAnalyserActorRef =
                actorSystem.ActorSelection("akka.tcp://analyzer@localhost:8080/user/main");
        }
    }
}