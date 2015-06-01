using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Akka.Actor;
using DotNetMalaga.AkkaExample.Common.Messages;
using Newtonsoft.Json;
using Tweetinvi.Core.Interfaces;
using Tweetinvi.Core.Interfaces.Models;

namespace DotNetMalaga.AkkaExample.Service
{
    internal class SentimentAnalyser : ReceiveActor
    {
        private readonly string _sentimentApi;
        private readonly string _sentimentUrl;

        public SentimentAnalyser()
        {
            NameValueCollection configSettings = ConfigurationManager.AppSettings;
            _sentimentUrl = configSettings["SENTIMENT_URL"];
            _sentimentApi = configSettings["SENTIMENT_API"];


            Receive<SocialMediaMaster.MessageToBeAnalysedAndHashtagged>(msg =>
            {
                string uriString = String.Format("{0}={1}{2}", "text", HttpUtility.UrlEncode(msg.Tweet.Text),
                    _sentimentApi);

                var wc = new WebClient();
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                wc.Headers[HttpRequestHeader.Accept] = "application/json";

                wc.UploadStringTaskAsync(new Uri(_sentimentUrl), uriString.ToString(CultureInfo.InvariantCulture))
                    .ContinueWith(task =>
                    {
                        #region getScoreFromTaskResultJSON

                        string html = null;
                        float score = 0;
                        try
                        {
                            var json = JsonConvert.DeserializeObject<dynamic>(task.Result);
                            if (!float.TryParse(Convert.ToString(json.aggregate.score), out score))
                                score = 0;
                        }
                        catch (AggregateException)
                        {
                            // TODO 
                            ;
                        }

                        #endregion

                        #region getTweetDetails

                        string message = msg.Tweet.Text;
                        string hashtag = msg.Hashtag;
                        ITweet tweet = msg.Tweet;
                        IOEmbedTweet embededtweet = tweet.GenerateOEmbedTweet();
                        if (embededtweet != null)
                            html = embededtweet.HTML;

                        #endregion

                        return new MessageAlreadyAnalysed(message, hashtag, score, html);
                    }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously)
                    .PipeTo(Sender);
            });
        }
    }
}