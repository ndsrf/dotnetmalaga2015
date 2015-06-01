using System;
using System.Diagnostics;
using Akka.Actor;
using DotNetMalaga.AkkaExample.Common.Commands;
using DotNetMalaga.AkkaExample.Common.Messages;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace DotNetMalaga.AkkaExample.NancyWebClient.Actors
{

    public class SignalRActor : ReceiveActor
    {
        private SignalRHub _hub;

        public SignalRActor()
        {
            Receive<String>(s =>
            {
                var commandAddHashtag = new AddHashtag(s, Self);
                SystemActors.SentimentAnalyserActorRef.Tell(commandAddHashtag);
            });
            Receive<RemoveHashtag>(s =>
            {
                SystemActors.SentimentAnalyserActorRef.Tell(s);
            });
            Receive<MessageAlreadyAnalysed>(
                analysed =>
                {
                    Debug.WriteLine("ANALYSIS: " + analysed.Hashtag + " MESSAGE: " + analysed.Message + " POINTS: " +
                                    analysed.Sentiment);

                    _hub.SocialEventReceived(analysed);

                });

        }

        protected override void PreStart()
        {
            var hubManager = new DefaultHubManager(GlobalHost.DependencyResolver);
            _hub = hubManager.ResolveHub("sentimentHub") as SignalRHub;
        }


    }
}