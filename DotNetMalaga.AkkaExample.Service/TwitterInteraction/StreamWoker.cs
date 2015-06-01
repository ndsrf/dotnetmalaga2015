using Akka.Actor;
using DotNetMalaga.AkkaExample.Common.Commands;

namespace DotNetMalaga.AkkaExample.Service.TwitterInteraction
{
    internal class StreamWoker : ReceiveActor, IWithUnboundedStash
    {
        #region messages

        internal class TweetReceived
        {
            public TweetReceived(string tweet)
            {
                Tweet = tweet;
            }

            public string Tweet { get; private set; }
        }

        #endregion

        private readonly TwitterHandle filteredStream;

        public StreamWoker()
        {
            filteredStream = new TwitterHandle(Self);
            Streaming();
        }

        public IStash Stash { get; set; }

        private void Streaming()
        {
            Receive<AddHashtag>(hashtag => filteredStream.AddTrack(hashtag.Hashtag));
            Receive<RemoveHashtag>(hashtag => filteredStream.RemoveTrack(hashtag.Hashtag));
            Receive<SocialMediaMaster.MessageToBeAnalysed>(received =>
            {
                //Console.WriteLine("TWEET -- " + received.Message);
                Context.Parent.Tell(received);
            });
            Receive<StopAnalysis>(streaming => filteredStream.StopStream());
        }
    }
}