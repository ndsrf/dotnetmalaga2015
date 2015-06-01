using Akka.Actor;
using DotNetMalaga.AkkaExample.Common.Commands;
using DotNetMalaga.AkkaExample.Common.Messages;
using DotNetMalaga.AkkaExample.WebClient.Actors;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace DotNetMalaga.AkkaExample.WebClient.SignarlR
{
    [HubName("sentimentHub")]
    public class SignalRHub : Hub
    {
        private void GroupAdd(string connectionId, string hashtag)
        {
            Groups.Add(connectionId, hashtag).Wait();
        }

        private void GroupRemove(string connectionId, string hashtag)
        {
            Groups.Remove(connectionId, hashtag).Wait();
        }

        public void AddHashtag(string message)
        {
            GroupAdd(Context.ConnectionId, message);
            SystemActors.SignalRActor.Tell(message, ActorRefs.Nobody);
        }

        public void RemoveHashtag(string message)
        {
            GroupRemove(Context.ConnectionId, message);
            SystemActors.SignalRActor.Tell(new RemoveHashtag(message, ActorRefs.Nobody));
        }

        public void SocialEventReceived(MessageAlreadyAnalysed message)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SignalRHub>();
            dynamic interestedClients = context.Clients.Group(message.Hashtag).SocialEventReceived(message);
        }
    }
}