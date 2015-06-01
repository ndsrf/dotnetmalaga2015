using Akka.Actor;

namespace DotNetMalaga.AkkaExample.NancyWebClient.Actors
{
    public static class SystemActors
    {
        public static IActorRef SignalRActor = ActorRefs.Nobody;

        public static ActorSelection SentimentAnalyserActorRef;
    }
}