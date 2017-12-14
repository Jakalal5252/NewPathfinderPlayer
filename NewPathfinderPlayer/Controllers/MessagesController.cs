
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Storage;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using NewPathfinderPlayer.TopicViews;
using NewPathfinderPlayer.Models;
using NewPathfinderPlayer.Topics;

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

                // create the activity adapter to send/receive Activity objects 
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
                        // --- Bot logic 
                        bool handled = false;
                        // Get the current ActiveTopic from my conversation state
                        var activeTopic = context.State.Conversation[ConversationProperties.ACTIVETOPIC] as ITopic;

                        // if there isn't one 
                        if (activeTopic == null)
                        {
                            // use default topic
                            activeTopic = new DefaultTopic();
                            context.State.Conversation[ConversationProperties.ACTIVETOPIC] = activeTopic;
                            handled = await activeTopic.StartTopic(context);
                        }
                        else
                        {
                            // continue to use the active topic
                            handled = await activeTopic.ContinueTopic(context);
                        }

                        if (handled == false && !(context.State.Conversation[ConversationProperties.ACTIVETOPIC] is DefaultTopic))
                        {
                            // resume default topic
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