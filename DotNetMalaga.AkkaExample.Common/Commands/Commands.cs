using Akka.Actor;

namespace DotNetMalaga.AkkaExample.Common.Commands
{
    public class AddHashtag
    {
        public AddHashtag(string hashtag, IActorRef subscriber)
        {
            Hashtag = hashtag;
            Subscriber = subscriber;
        }

        public string Hashtag { get; private set; }
        public IActorRef Subscriber { get; private set; }
    }

    public class RemoveHashtag
    {
        public RemoveHashtag(string hashtag, IActorRef subscriber)
        {
            Hashtag = hashtag;
            Subscriber = subscriber;
        }

        public string Hashtag { get; private set; }
        public IActorRef Subscriber { get; private set; }
    }

    public class StartAnalysis
    {
    }

    public class StopAnalysis
    {
    }
}