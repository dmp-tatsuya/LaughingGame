using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace LaughingGame.Dialogs
{
    [Serializable]
    public class GreetingDialog : IDialog<object>
    {
        public LaughingDataModel laughingData;

        public GreetingDialog(object passedData)
        {
            laughingData = passedData as LaughingDataModel;
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> arguments)
        {
            var message = await arguments;
            if(laughingData != null)
            {
                await context.PostAsync($"You said: {message.Text} value = {laughingData.value}");
                context.Wait(new EmotionDialog(laughingData).MessageReceivedAsync);
            }
            else
            {
                await context.PostAsync($"You said: {message.Text}");
                context.Wait(MessageReceivedAsync);
            }
        }
    }
}