
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Storage;
using Microsoft.Bot.Connector;
using Microsoft.Extensions.Configuration;
using NewPathfinderPlayer.Models;
using NewPathfinderPlayer.Topics;
using NewPathfinderPlayer.TopicViews;
using System.Text.RegularExpressions;

namespace NewPathfinderPlayer.Controllers
{
    [Route("api/[controller]")]
    public class MessagesController : Controller
    {
        public static BotFrameworkAdapter activityAdapter = null;
        public static Bot bot = null;

        public MessagesController(IConfiguration configuration)
        {
            if (activityAdapter == null)
            {
                string appId = configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppIdKey)?.Value;
                string appPassword = configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppPasswordKey)?.Value;

                // Create the activity adapter to send/receive Activity objects 
                activityAdapter = new BotFrameworkAdapter(appId, appPassword);
                bot = new Bot(activityAdapter)
                    .Use(new MemoryStorage())
                    .Use(new BotStateManager())
                    .Use(new DefaultTopicView())
                    .Use(new RegExpRecognizerMiddleware()
                        .AddIntent("takeTurn", new Regex("take turn(.*)", RegexOptions.IgnoreCase))
                        .AddIntent("standardAction", new Regex("attack(.*)|standard(.*)", RegexOptions.IgnoreCase))
                        .AddIntent("moveAction", new Regex("move(.*)", RegexOptions.IgnoreCase))
                        .AddIntent("swiftAction", new Regex("swift(.*)", RegexOptions.IgnoreCase))
                        .AddIntent("doubleMoveAction", new Regex("double move(.*)", RegexOptions.IgnoreCase))
                        .AddIntent("endTurn", new Regex("end turn(.*)", RegexOptions.IgnoreCase))
                        .AddIntent("confirmYes", new Regex("(yes|yep)", RegexOptions.IgnoreCase))
                        .AddIntent("confirmNo", new Regex("(no|nope)", RegexOptions.IgnoreCase)))
                    .OnReceive(async (context) =>
                    {
                        // Bot logic 
                        bool handled = false;

                        // Get the current active topic from my conversation state
                        var activeTopic = context.State.Conversation[ConversationProperties.ACTIVETOPIC] as ITopic;

                        // If there isn't an active topic, create a default topic.
                        if (activeTopic == null)
                        {
                            activeTopic = new DefaultTopic();
                            context.State.Conversation[ConversationProperties.ACTIVETOPIC] = activeTopic;
                            handled = await activeTopic.StartTopic(context);
                        }
                        else
                        {
                            // If there is an active topic, continue to use the active topic.
                            handled = await activeTopic.ContinueTopic(context);
                        }

                        if (handled == false && !(context.State.Conversation[ConversationProperties.ACTIVETOPIC] is DefaultTopic))
                        {
                            // Resume default topic if no other topic was started.
                            activeTopic = new DefaultTopic();
                            context.State.Conversation[ConversationProperties.ACTIVETOPIC] = activeTopic;
                            handled = await activeTopic.ResumeTopic(context);
                        }
                    });
            }
        }

        [Authorize(Roles = "Bot")]
        [HttpPost]
        public async void Post([FromBody]Activity activity)
        {
            await activityAdapter.Receive(HttpContext.Request.Headers, activity);
        }
    }
}