using NewPathfinderPlayer.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewPathfinderPlayer.TopicViews
{
    public class TakeTurnTopicView : TemplateRendererMiddleware
    {
        public TakeTurnTopicView() : base(new DictionaryRenderer(Templates))
        {
        }

        // template ids
        public const string STARTTOPIC = "TakeTurnTopic.StartTopic";
        public const string RESUMETOPIC = "TakeTurnTopic.ResumeTopic";
        public const string HELP = "TakeTurnTopic.Help";
        public const string CONFUSED = "TakeTurnTopic.Confusion";
        public const string ENDTURN = "TakeTurnTopic.EndTurn";
        public const string ENDTURNCANCELED = "TakeTurnTopic.EndTurnCanceled";
        public const string CANCELCANCELED = "TakeTurnTopic.CancelCanceled";
        public const string CANCELREPROMPT = "TakeTurnTopic.CancelReprompt";
        public const string TITLEPROMPT = "TakeTurnTopic.TitlePrompt";
        public const string TITLEVALIDATIONPROMPT = "TakeTurnTopic.TitleValidationPrompt";
        public const string TIMEPROMPT = "TakeTurnTopic.TimePrompt";
        public const string TIMEVALIDATIONPROMPT = "TakeTurnTopic.TimeValidationPrompt";
        public const string ADDEDALARM = "TakeTurnTopic.AddedAlarm";
        public const string ADDCONFIRMATION = "TakeTurnTopic.AddConfirmation";
        public const string TIMEPROMPTFUTURE = "TakeTurnTopic.TimePromptFuture";

        /// <summary>
        /// Standard language alarm description
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string AlarmDescription(BotContext context, CombatRound combatRound)
        {
            return "";
        }


        /// <summary>
        /// table of language functions which render output in various languages
        /// </summary>
        public static TemplateDictionary Templates = new TemplateDictionary
        {
            // Default templates
            ["default"] = new TemplateIdMap
                {
                    { STARTTOPIC, (context, data) => $"Ok, let's add an alarm." },
                    { HELP, (context, data) => $"I am working with you to create an alarm.  To do that I need to know the title and time.\n\n{AlarmDescription(context,data)}"},
                    { CONFUSED, (context, data) => $"I am sorry, I didn't understand: {context.Request.Text}." },
                    { ENDTURN, (context, data) => $"# EndTurn?\n\nDid you want to end your turn?\n\n\n\n(Yes or No)" },
                    { CANCELREPROMPT, (context, data) => $"# Cancel alarm?\n\nPlease answer the question with a \"yes\" or \"no\" reply. Did you want to cancel the alarm?\n\n{AlarmDescription(context,data)}\n\n" },
                    { ENDTURNCANCELED, (context, data) => $"OK, let's continue." },
                    { TIMEPROMPT, (context, data) => $"# Adding alarm\n\n{AlarmDescription(context,data)}\n\nWhat time would you like to set the alarm for?" },
                    { TIMEPROMPTFUTURE, (context, data) => $"# Adding alarm\n\n{AlarmDescription(context,data)}\n\nYou need to specify a time in the future. What time would you like to set the alarm?" },
                    { TITLEPROMPT, (context, data)=> $"# Adding alarm\n\n{AlarmDescription(context,data)}\n\nWhat would you like to call your alarm ?" },
                    { ADDCONFIRMATION, (context, data)=> $"# Adding Alarm\n\n{AlarmDescription(context,data)}\n\nDo you want to save this alarm?" },
                    { ADDEDALARM, (context, data)=> $"# Alarm Added\n\n{AlarmDescription(context,data)}." }
                }
        };

    }


}
