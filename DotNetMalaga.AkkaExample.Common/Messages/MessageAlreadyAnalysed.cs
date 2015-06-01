namespace DotNetMalaga.AkkaExample.Common.Messages
{
    public class MessageAlreadyAnalysed
    {
        public MessageAlreadyAnalysed(string msg, string hashtag, float sentiment, string embededHtml)
        {
            Message = msg;
            Hashtag = hashtag;
            Sentiment = sentiment;
            EmbededHtml = embededHtml;
        }

        public string Message { get; private set; }
        public string Hashtag { get; private set; }
        public float Sentiment { get; private set; }

        public string EmbededHtml { get; private set; }
    }
}