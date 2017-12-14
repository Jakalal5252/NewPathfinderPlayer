using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Templates;
using NewPathfinderPlayer.Models;

namespace NewPathfinderPlayer.TopicViews
{
    public class TakeTurnTopicView : TemplateRendererMiddleware
    {
        public TakeTurnTopicView() : base(new DictionaryRenderer(Templates))
        {
        }

        // Template ids
        public const string STARTTOPIC = "TakeTurnTopic.StartTopic";
        public const string RESUMETOPIC = "TakeTurnTopic.ResumeTopic";
        public const string HELP = "TakeTurnTopic.Help";
        public const string CONFUSED = "TakeTurnTopic.Confusion";
        public const string ENDTURN = "TakeTurnTopic.EndTurn";
        public const string ENDTURNCANCELED = "TakeTurnTopic.EndTurnCanceled";
        public const string CANCELCANCELED = "TakeTurnTopic.CancelCanceled";
        public const string CANCELREPROMPT = "TakeTurnTopic.CancelReprompt";
        public const string ENDTURNCONFIRMATION = "TakeTurnTopic.EndTurnConfirmation";
        public const string TITLEPROMPT = "TakeTurnTopic.TitlePrompt";
        public const string TITLEVALIDATIONPROMPT = "TakeTurnTopic.TitleValidationPrompt";
        public const string TIMEPROMPT = "TakeTurnTopic.TimePrompt";
        public const string TIMEVALIDATIONPROMPT = "TakeTurnTopic.TimeValidationPrompt";
        public const string ADDCONFIRMATION = "TakeTurnTopic.AddConfirmation";
        public const string TIMEPROMPTFUTURE = "TakeTurnTopic.TimePromptFuture";

        /// <summary>
        /// Table of language functions which render output in various languages
        /// </summary>
        public static TemplateDictionary Templates = new TemplateDictionary
        {
            // Default templates
            ["default"] = new TemplateIdMap
                {
                    { STARTTOPIC, (context, data) => $"Ok, let's add an alarm." },
                    { CONFUSED, (context, data) => $"I am sorry, I didn't understand: {context.Request.Text}." },
                    { ENDTURNCONFIRMATION, (context, data) => $"# EndTurn?\n\nDid you want to end your turn?\n\n\n\n(Yes or No)" },
                    { ENDTURN, (context, data) => $"# EndTurn?\n\nDid you want to end your turn?\n\n\n\n(Yes or No)" },
                    { ENDTURNCANCELED, (context, data) => $"OK, let's continue." }
                }
        };
    }
}