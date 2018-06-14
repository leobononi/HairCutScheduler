using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace HairCutScheduler.Dialogs
{
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
        public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(
            modelID: ConfigurationManager.AppSettings["LuisAppId"],
            subscriptionKey: ConfigurationManager.AppSettings["LuisAPIKey"],
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result, "Desculpe, não entendemos... :/");
        }

        [LuisIntent("Scheduling")]
        public async Task SchedulingIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result, "Entendi. E para quando você gostaria de agendar?");
        }

        [LuisIntent("EndScheduling")]
        public async Task EndSchedulingIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result, "Por nada! Volte sempre!!");
        }

        [LuisIntent("SchedulingConfirm")]
        public async Task SchedulingConfirmIntent(IDialogContext context, LuisResult result)
        {
            var dateForScheduling = string.Empty;
            var dateRequested = result.Entities.FirstOrDefault()?.Entity;
            if (!string.IsNullOrEmpty(dateRequested))
            {
                dateForScheduling = dateRequested.Trim().ToUpper().Equals("AMANHÃ")
                    ? DateTime.Now.AddDays(1).ToShortDateString()
                    : DateTime.Now.ToShortDateString();
            }

            await this.ShowLuisResult(context, result, $"Perfeito! O serviço foi agendado para {dateForScheduling}");
        }

        protected override IntentRecommendation BestIntentFrom(LuisResult result)
        {
            return base.BestIntentFrom(result);
        }

        // Go to https://luis.ai and create a new intent, then train/publish your luis app.
        // Finally replace "Greeting" with the name of your newly created intent in the following handler
        [LuisIntent("Greeting")]
        public async Task GreetingIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result, "Olá! Em que posso ajudar?");
        }

        private async Task ShowLuisResult(IDialogContext context, LuisResult result, string message)
        {
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
    }
}