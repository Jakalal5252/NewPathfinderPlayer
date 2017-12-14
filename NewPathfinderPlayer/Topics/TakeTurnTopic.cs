using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using NewPathfinderPlayer.Models;
using NewPathfinderPlayer.TopicViews;
using System;
using System.Threading.Tasks;

namespace NewPathfinderPlayer.Topics
{
    public class TakeTurnTopic : ITopic
    {
        /// <summary>
        /// enumeration of states of the converation
        /// </summary>
        public enum TopicStates
        {
            // initial state
            Started,

            // we asked for title
            ActionPrompt,

            // we asked for confirmation to cancel
            EndTurn,

            // we asked for confirmation to add
            EndTurnConfirmation,
        };


        public TakeTurnTopic()
        {
        }

        /// <summary>
        /// Alarm object representing the information being gathered by the conversation before it is committed
        /// </summary>
        public CombatRound CombatRound { get; set; }

        /// <summary>
        /// Current state of the topic conversation
        /// </summary>
        public TopicStates TopicState { get; set; } = TopicStates.Started;

        public string Name { get; set; } = "TakeTurn";

        /// <summary>
        /// Called when the add alarm topic is started
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<bool> StartTopic(BotContext context)
        {

            this.CombatRound = new CombatRound()
            {
                // initialize from intent entities
                combatActions = CombatRound.CombatActions.Move | CombatRound.CombatActions.Standard | CombatRound.CombatActions.Swift | CombatRound.CombatActions.DoubleMove
            };

            return PromptForNextMove(context);
        }


        /// <summary>
        /// we call for every turn while the topic is still active
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<bool> ContinueTopic(BotContext context)
        {
            // for messages
            if (context.Request.Type == ActivityTypes.Message)
            {

                switch (context.TopIntent?.Name)
                {
                    case "endTurn":
                        TopicState = TopicStates.EndTurnConfirmation;
                        return await ProcessTopicState(context);
                    default:
                        return await ProcessTopicState(context);
                }
            }
            return true;
        }

        private async Task<bool> ProcessTopicState(BotContext context)
        {
            string utterance = (context.Request.Text ?? "").Trim();

            // we ar eusing TopicState to remember what we last asked
            switch (this.TopicState)
            {
                case TopicStates.EndTurnConfirmation:
                    switch (context.TopIntent?.Name)
                    {
                        case "confirmYes":

                            context.ReplyWith(TakeTurnTopicView.ENDTURN, this.CombatRound);
                            // end topic
                            TopicState = TopicStates.EndTurn;
                            return false;

                        case "confirmNo":
                            context.ReplyWith(TakeTurnTopicView.ENDTURNCANCELED, this.CombatRound);
                            TopicState = TopicStates.ActionPrompt;
                            // TODO Should there be another call here?
                            return true;
                        default:
                            return await this.PromptForNextMove(context);
                    }
                case TopicStates.ActionPrompt:
                    switch (context.TopIntent.Name)
                    {
                        case ("endTurn"):
                            this.TopicState = TopicStates.EndTurnConfirmation;
                            return await this.PromptForNextMove(context);
                        case ("attackAction"):
                            this.CombatRound.combatActions -= CombatRound.CombatActions.Standard;
                            return await this.PromptForNextMove(context);
                        case ("moveAction"):
                            this.CombatRound.combatActions -= CombatRound.CombatActions.Move - CombatRound.CombatActions.DoubleMove;
                            return await this.PromptForNextMove(context);
                        case ("swiftAction"):
                            this.CombatRound.combatActions -= CombatRound.CombatActions.Swift - CombatRound.CombatActions.DoubleMove - CombatRound.CombatActions.Move;
                            return await this.PromptForNextMove(context);
                        case ("doubleMoveAction"):
                            this.CombatRound.combatActions -= CombatRound.CombatActions.DoubleMove;
                            return await this.PromptForNextMove(context);

                        default:
                            return true;
                    };
                default:
                    {
                        return true;
                    };

            };
        }

        private CombatRound.CombatActions GetActionFromInput(string utterance)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called when this topic is resumed after being interrupted
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<bool> ResumeTopic(BotContext context)
        {
            // simply prompt again based on our state
            return this.PromptForNextMove(context);
        }

        /// <summary>
        /// Shared method to get missing information
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task<bool> PromptForNextMove(BotContext context)
        {

            if (this.CombatRound.combatActions != 0)
            {
                this.TopicState = TopicStates.ActionPrompt;
                context.Reply(ActionListToString(CombatRound.combatActions));
                return true;
            }
            else
            {
                this.TopicState = TopicStates.EndTurn;
                return true;
            }
        }

        private string ActionListToString(CombatRound.CombatActions combatActions)
        {
            string temp = "You can do one of the following actions: ";
            foreach (Enum value in Enum.GetValues(combatActions.GetType()))
            {
                if (combatActions.HasFlag(value))
                {
                    temp += Environment.NewLine + Environment.NewLine;
                    temp += value.ToString();
                }
            }
            return temp;
        }
    }
}
