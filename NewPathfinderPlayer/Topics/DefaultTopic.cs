using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using NewPathfinderPlayer.Models;
using NewPathfinderPlayer.TopicViews;
using System.Linq;
using System.Threading.Tasks;

namespace NewPathfinderPlayer.Topics
{
    public class DefaultTopic : ITopic
    {
        public DefaultTopic() { }

        public string Name { get; set; } = "Default";

        // track in this topic if we have greeted the user already
        public bool Greeted { get; set; } = false;

        /// <summary>
        /// Called when the default topic is started
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<bool> StartTopic(BotContext context)
        {
            switch (context.Request.Type)
            {
                case ActivityTypes.ConversationUpdate:
                    {
                        // greet when added to conversation
                        var activity = context.Request.AsConversationUpdateActivity();
                        if (activity.MembersAdded.Where(m => m.Id == activity.Recipient.Id).Any())
                        {
                            context.ReplyWith(DefaultTopicView.GREETING);
                            context.ReplyWith(DefaultTopicView.HELP);
                            Greeted = true;
                        }
                    }

                    break;

                case ActivityTypes.Message:
                    // greet on first message if we haven't already 
                    if (!Greeted)
                    {
                        context.ReplyWith(DefaultTopicView.GREETING);
                        Greeted = true;
                    }

                    return ContinueTopic(context);
            }

            return Task.FromResult(true);
        }

        /// <summary>
        /// Continue the topic, method which is routed to while this topic is active
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<bool> ContinueTopic(BotContext context)
        {
            var activeTopic = (ITopic)context.State.Conversation[ConversationProperties.ACTIVETOPIC];

            switch (context.Request.Type)
            {
                case ActivityTypes.Message:
                    switch (context.TopIntent?.Name)
                    {
                        case "takeTurn":
                            activeTopic = new TakeTurnTopic();
                            context.State.Conversation[ConversationProperties.ACTIVETOPIC] = activeTopic;
                            return activeTopic.StartTopic(context);

                        case "help":
                            // show help
                            context.ReplyWith(DefaultTopicView.HELP);
                            return Task.FromResult(true);

                        default:
                            // show our confusion
                            context.ReplyWith(DefaultTopicView.CONFUSED);
                            return Task.FromResult(true);
                    }

                default:
                    break;
            }

            return Task.FromResult(true);
        }

        /// <summary>
        /// Method which is called when this topic is resumed after an interruption
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<bool> ResumeTopic(BotContext context)
        {
            // just prompt the user to ask what they want to do
            context.ReplyWith(DefaultTopicView.RESUMETOPIC);
            return Task.FromResult(true);
        }
    }
}