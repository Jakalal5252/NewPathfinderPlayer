using Microsoft.Bot.Builder;
using System.Threading.Tasks;

namespace NewPathfinderPlayer.Topics
{
    public class BaseTopic
    {
        public string Name { get; set; } = "BaseTopic";

        public virtual Task<bool> ContinueTopic(BotContext context)
        {
            return Task.FromResult(false);
        }

        public virtual Task<bool> ResumeTopic(BotContext context)
        {
            return Task.FromResult(false);
        }

        public virtual Task<bool> StartTopic(BotContext context)
        {
            return Task.FromResult(false);
        }
    }
}
