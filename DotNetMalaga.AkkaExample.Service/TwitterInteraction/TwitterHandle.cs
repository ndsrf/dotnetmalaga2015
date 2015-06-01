using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using Akka.Actor;
using Tweetinvi;
using Tweetinvi.Core.Enum;
using Tweetinvi.Core.Events.EventArguments;
using Tweetinvi.Core.Extensions;
using Tweetinvi.Core.Interfaces;
using Tweetinvi.Core.Interfaces.Streaminvi;

namespace DotNetMalaga.AkkaExample.Service.TwitterInteraction
{
    internal class TwitterHandle
    {
        private readonly IActorRef twitterActorRef;
        private IFilteredStream filteredStream;


        public TwitterHandle(IActorRef theActor)
        {
            twitterActorRef = theActor;
            NameValueCollection configSettings = ConfigurationManager.AppSettings;
            string userAccessSecret = configSettings["TWITTER_ACCESSSECRET"];
            string userAccessToken = configSettings["TWITTER_ACCESSTOKEN"];
            string consumerKey = configSettings["TWITTER_CONSUMERKEY"];
            string consumerSecret = configSettings["TWITTER_CONSUMERSECRET"];

            TwitterCredentials.SetCredentials(userAccessToken, userAccessSecret, consumerKey, consumerSecret);

            ReCreateFilteredStream();
        }

        private void ReCreateFilteredStream()
        {
            if (filteredStream != null)
            {
                filteredStream.StopStream();
                filteredStream.MatchingTweetReceived -= FilteredStreamOnMatchingTweetReceived;
            }

            filteredStream = Stream.CreateFilteredStream();
            filteredStream.AddTweetLanguageFilter(Language.Spanish);
            filteredStream.MatchingTweetReceived += FilteredStreamOnMatchingTweetReceived;
        }

        private void FilteredStreamOnMatchingTweetReceived(object sender,
            MatchedTweetReceivedEventArgs matchedTweetReceivedEventArgs)
        {
            twitterActorRef.Tell(new SocialMediaMaster.MessageToBeAnalysed(matchedTweetReceivedEventArgs.Tweet));
        }

        public void AddTrack(string hashtag)
        {
            if (filteredStream.TracksCount > 0)
            {
                filteredStream.StopStream();

                Dictionary<string, Action<ITweet>> tracks = filteredStream.Tracks;

                ReCreateFilteredStream();

                tracks.Keys.ForEach(s => filteredStream.AddTrack(s));
            }

            filteredStream.AddTrack(hashtag);

            filteredStream.StartStreamMatchingAnyConditionAsync();

            Debug.WriteLine("ADDED HASHTAG - " + hashtag);
        }

        public void RemoveTrack(string hashtag)
        {
            if (filteredStream.TracksCount > 0)
            {
                filteredStream.StopStream();

                Dictionary<string, Action<ITweet>> tracks = filteredStream.Tracks;

                ReCreateFilteredStream();

                tracks.Keys.ForEach(s => filteredStream.AddTrack(s));
            }

            filteredStream.RemoveTrack(hashtag);

            if (filteredStream.TracksCount > 0)
            {
                filteredStream.StartStreamMatchingAnyConditionAsync();
            }

            Debug.WriteLine("REMOVED HASHTAG - " + hashtag);
        }

        public void StopStream()
        {
            filteredStream.StopStream();
            ReCreateFilteredStream();
        }
    }
}