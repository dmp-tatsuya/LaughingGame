using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using System.Collections.Generic;


namespace LaughingGame
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {

            // メッセージやり取りを行う ConnectorClient を生成
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            // Emotion API の Subscription Key をセット
            // Emotion API を Call する EmotionServiceClient を生成
            // 入力値(URL) を元に Emotion API を Call
            // ※次以降の項目で作成します

            // デフォルトの返答 (初回、または写真判定ができなかったとき))
            Activity reply = activity.CreateReply("顔の表情を判定します。写真のURLを送ってね。");

            // Call 結果を元に 返答を作成
            // ※次以降の項目で作成します

            // メッセージ、および http ステータス Accepted(=200) を返答
            await connector.Conversations.ReplyToActivityAsync(reply);
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);

        }
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {

            // メッセージやり取りを行う ConnectorClient を生成
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            // Emotion API の Subscription Key をセット
            // お持ちの Subscription Key を YOUR_SUBSCRIPTION_KEY 部分にコピーしてください
            const string emotionApiKey = "e3a71d5333314af38a9f92fc4f0a68bb";

            // Emotion API を Call する EmotionServiceClient を生成
            EmotionServiceClient emotionServiceClient = new EmotionServiceClient(emotionApiKey);
            Emotion[] emotionResult = null;

            // 入力値(URL) を元に Emotion API を Call
            try
            {
                emotionResult = await emotionServiceClient.RecognizeAsync(activity.Text);
            }
            catch (Exception e)
            {
                emotionResult = null;
            }

            // デフォルトの返答 (初回、または写真判定ができなかったとき))
            Activity reply = activity.CreateReply("顔の表情を判定します。写真のURLを送ってね。");

            // Call 結果を元に 返答を作成
            // Call 結果を元に 返答を作成
            if (emotionResult != null)
            {
                //表情スコアを取得
                Scores emotionScores = emotionResult[0].Scores;

                //取得したスコアを KeyValuePair に代入、スコア数値の大きい順に並び替える
                IEnumerable<KeyValuePair<string, float>> emotionList = new Dictionary<string, float>()
        {
            { "怒ってる", emotionScores.Anger},
            { "軽蔑してる", emotionScores.Contempt },
            { "うんざりしてる", emotionScores.Disgust },
            { "怖がってる", emotionScores.Fear },
            { "楽しい", emotionScores.Happiness},
            { "特になし", emotionScores.Neutral},
            { "悲しい", emotionScores.Sadness },
            { "驚いてる", emotionScores.Surprise}
        }
                .OrderByDescending(kv => kv.Value)
                .ThenBy(kv => kv.Key)
                .ToList();

                //スコア数値の一番大きい表情を取得してメッセージにセット
                KeyValuePair<string, float> topEmotion = emotionList.ElementAt(0);
                string topEmotionKey = topEmotion.Key;
                float topEmotionScore = topEmotion.Value;

                reply = activity.CreateReply
                    (
                        "顔の表情を判定しました。"
                        + (int)(topEmotionScore * 100) + "% " + topEmotionKey + "顔だと思うよ。"
                    );
            }

            // メッセージ、および http ステータス Accepted(=200) を返答
            await connector.Conversations.ReplyToActivityAsync(reply);
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);


        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}