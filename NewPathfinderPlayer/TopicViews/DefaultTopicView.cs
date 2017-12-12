using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Templates;

namespace NewPathfinderPlayer.TopicViews
{
    public class DefaultTopicView : TemplateRendererMiddleware
    {
        public DefaultTopicView() : base(new DictionaryRenderer(ReplyTemplates))
        {
        }

        // template ids
        public const string GREETING = "DefaultTopic.StartTopic";
        public const string RESUMETOPIC = "DefaultTopic.ResumeTopic";
        public const string HELP = "DefaultTopic.Help";
        public const string CONFUSED = "DefaultTopic.Confusion";

        // template functions for rendeing responses in different a languages
        public static TemplateDictionary ReplyTemplates = new TemplateDictionary
        {
            ["default"] = new TemplateIdMap
                {
                    { GREETING, (context, data) => $"Hello, I'm here to help you play." },
                    { HELP, (context, data) => $"I can help you take your turn. " },
                    { RESUMETOPIC, (context, data) => $"What would you like to do next?" },
                    { CONFUSED, (context, data) => $"I am sorry, I didn't understand that." },
                },
            ["en"] = new TemplateIdMap { },
            ["fr"] = new TemplateIdMap { }
        };
    }
}