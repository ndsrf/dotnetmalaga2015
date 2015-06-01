using System;
using System.Collections.Generic;
using Akka.Actor;
using DotNetMalaga.AkkaExample.Common.Commands;
using DotNetMalaga.AkkaExample.Common.Messages;
using DotNetMalaga.AkkaExample.Service.TwitterInteraction;
using Tweetinvi.Core.Extensions;
using Tweetinvi.Core.Interfaces;

namespace DotNetMalaga.AkkaExample.Service
{
    internal class SocialMediaMaster : ReceiveActor
    {
        #region messages

        internal class MessageToBeAnalysed
        {
            public MessageToBeAnalysed(ITweet tweet)
            {
                Tweet = tweet;
            }

            public ITweet Tweet { get; private set; }
        }

        internal class MessageToBeAnalysedAndHashtagged : MessageToBeAnalysed
        {
            public MessageToBeAnalysedAndHashtagged(ITweet tweet, string hashtag) : base(tweet)
            {
                Hashtag = hashtag;
            }

            public string Hashtag { get; private set; }
        }

        #endregion

        private readonly MultiValueDictionary<string, IActorRef> _hashtagSubscribers =
            new MultiValueDictionary<string, IActorRef>();

        private readonly IActorRef _sentimentAnalyserRef;
        private readonly IActorRef _twitterActorRef = Context.ActorOf<StreamWoker>("twitterStreamWorker");

        public SocialMediaMaster()
        {
            // 1 - normal
            _sentimentAnalyserRef = Context.ActorOf<SentimentAnalyser>("sentimentAnalyser");

            //// 2 - round robin (code)
            //var props = Props.Create<SentimentAnalyser>().WithRouter(new RoundRobinPool(5));
            //_sentimentAnalyserRef = Context.ActorOf(props, "analyzerRouter");

            // 3 - round robin (hocon configuration)
            //var props = Props.Create<SentimentAnalyser>().WithRouter(FromConfig.Instance);
            //_sentimentAnalyserRef = Context.ActorOf(props, "analyzerRouter");

            Receive<AddHashtag>(hashtag =>
            {
                if (hashtag.Hashtag.IsNullOrEmpty())
                    return;

                if (!_hashtagSubscribers.ContainsKey(hashtag.Hashtag))
                {
                    _twitterActorRef.Tell(hashtag, Self);
                }
                _hashtagSubscribers.Add(hashtag.Hashtag, hashtag.Subscriber);
            });
            Receive<RemoveHashtag>(hashtag =>
            {
                if (hashtag.Hashtag.IsNullOrEmpty())
                    return;

                if (_hashtagSubscribers.ContainsKey(hashtag.Hashtag))
                {
                    _twitterActorRef.Tell(hashtag, Self);
                }
                _hashtagSubscribers.RemoveWithKeyValue(hashtag.Hashtag, hashtag.Subscriber);
            });
            Receive<StartAnalysis>(analysis => _twitterActorRef.Tell(analysis, Self));
            Receive<StopAnalysis>(analysis => _twitterActorRef.Tell(analysis, Self));
            Receive<MessageToBeAnalysed>(msg => _hashtagSubscribers.Keys.ForEach(
                s =>
                {
                    if (msg.Tweet.Text.Contains(s))
                        _sentimentAnalyserRef.Tell(new MessageToBeAnalysedAndHashtagged(msg.Tweet, s));
                }));
            Receive<MessageAlreadyAnalysed>(msg =>
            {
                Console.WriteLine("Tweet received: " + msg.Hashtag + " Sentiment: " + msg.Sentiment);
                foreach (IActorRef actor in GetInterestedActorRefs(msg.Hashtag))
                {
                    actor.Tell(msg);
                }
            });
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(0, 100, exception => Directive.Resume);
        }

        private IEnumerable<IActorRef> GetInterestedActorRefs(string hashtag)
        {
            return _hashtagSubscribers[hashtag];
        }

        public class MultiValueDictionary<Key, Value> : Dictionary<Key, List<Value>>
        {
            public void Add(Key key, Value value)
            {
                List<Value> values;
                if (!TryGetValue(key, out values))
                {
                    values = new List<Value>();
                    Add(key, values);
                }
                if (!values.Contains(value))
                    values.Add(value);
            }

            public void RemoveWithKeyValue(Key key, Value value)
            {
                List<Value> values;
                if (TryGetValue(key, out values))
                {
                    values.Remove(value);
                    if (values.Count == 0)
                    {
                        Remove(key);
                    }
                }
            }
        }
    }
}