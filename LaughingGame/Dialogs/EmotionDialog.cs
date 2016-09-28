using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace LaughingGame.Dialogs
{
    public class EmotionDialog
    {
        public LaughingDataModel laughingData;

        public EmotionDialog(object passedData)
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
            await context.PostAsync($"You said: {message.Text} value = {laughingData.value}");
            context.Wait(new ResultDialog(laughingData).MessageReceivedAsync);
        }
    }
}