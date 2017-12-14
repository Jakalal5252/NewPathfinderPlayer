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
        /// Enumeration of the states of the converation
        /// </summary>
        public enum TopicStates
        {
            // Initial state
            Started,

            // Asks for the user's next move
            ActionPrompt,

            // Ends the turn due to no more actions being left in the pool
            EndTurn,

            // We ask for confirmation that the user wants to end their turn
            EndTurnConfirmation,
        };

        public TakeTurnTopic()
        {
        }

        /// <summary>
        /// CombatRound representing the information being gathered by the conversation
        /// </summary>
        public CombatRound CombatRound { get; set; }

        /// <summary>
        /// Current state of the topic conversation
        /// </summary>
        public TopicStates TopicState { get; set; } = TopicStates.Started;

        public string Name { get; set; } = "TakeTurn";

        /// <summary>
        /// Called when the add take turn topic is started
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<bool> StartTopic(BotContext context)
        {
            CombatRound = new CombatRound()
            {
                // Initialize fresh set of moves
                combatActions = CombatRound.CombatActions.Move | CombatRound.CombatActions.Standard | CombatRound.CombatActions.Swift | CombatRound.CombatActions.DoubleMove
            };

            return PromptForNextMove(context);
        }

        /// <summary>
        /// Called for every turn while the topic is still active
        /// </summary>
        /// <param name="context">Current bot context</param>
        /// <returns></returns>
        public async Task<bool> ContinueTopic(BotContext context)
        {
            // Process an incoming message from the user
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

            switch (TopicState)
            {
                case TopicStates.EndTurnConfirmation:
                    switch (context.TopIntent?.Name)
                    {
                        case "confirmYes":

                            context.ReplyWith(TakeTurnTopicView.ENDTURN, CombatRound);
                            // End topic
                            TopicState = TopicStates.EndTurn;
                            return false;

                        case "confirmNo":
                            context.ReplyWith(TakeTurnTopicView.ENDTURNCANCELED, CombatRound);
                            TopicState = TopicStates.ActionPrompt;
                            // TODO Should there be another call here?
                            return true;
                        default:
                            return await PromptForNextMove(context);
                    }
                case TopicStates.EndTurn:
                    return false;
                case TopicStates.ActionPrompt:
                    switch (context.TopIntent.Name)
                    {
                        case ("endTurn"):
                            TopicState = TopicStates.EndTurnConfirmation;
                            return await PromptForNextMove(context);
                        case ("standardAction"):
                            CombatRound.combatActions -= CombatRound.CombatActions.Standard;
                            if (CombatRound.combatActions.HasFlag(CombatRound.CombatActions.DoubleMove))
                                CombatRound.combatActions -= CombatRound.CombatActions.DoubleMove;
                            return await PromptForNextMove(context);
                        case ("moveAction"):
                            CombatRound.combatActions -= CombatRound.CombatActions.Move;
                            if (CombatRound.combatActions.HasFlag(CombatRound.CombatActions.DoubleMove))
                                CombatRound.combatActions -= CombatRound.CombatActions.DoubleMove;
                            if (CombatRound.combatActions.HasFlag(CombatRound.CombatActions.Swift))
                                CombatRound.combatActions -= CombatRound.CombatActions.Swift;
                            return await PromptForNextMove(context);
                        case ("swiftAction"):
                            CombatRound.combatActions -= CombatRound.CombatActions.Swift;
                            if (CombatRound.combatActions.HasFlag(CombatRound.CombatActions.DoubleMove))
                                CombatRound.combatActions -= CombatRound.CombatActions.DoubleMove;
                            if (CombatRound.combatActions.HasFlag(CombatRound.CombatActions.Move))
                                CombatRound.combatActions -= CombatRound.CombatActions.Move;
                            return await PromptForNextMove(context);
                        case ("doubleMoveAction"):
                            CombatRound.combatActions -= CombatRound.CombatActions.DoubleMove;
                            if (CombatRound.combatActions.HasFlag(CombatRound.CombatActions.Swift))
                                CombatRound.combatActions -= CombatRound.CombatActions.Swift;
                            if (CombatRound.combatActions.HasFlag(CombatRound.CombatActions.Move))
                                CombatRound.combatActions -= CombatRound.CombatActions.Move;
                            if (CombatRound.combatActions.HasFlag(CombatRound.CombatActions.Standard))
                                CombatRound.combatActions -= CombatRound.CombatActions.Standard;
                            return await PromptForNextMove(context);

                        default:
                            return true;
                    };
                default:
                    return true;
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
            // Prompt again based on our state
            return PromptForNextMove(context);
        }

        /// <summary>
        /// Shared method to get next action
        /// </summary>
        /// <param name="context">Current bot context</param>
        /// <returns></returns>
        private async Task<bool> PromptForNextMove(BotContext context)
        {
            if (CombatRound.combatActions != 0)
            {
                TopicState = TopicStates.ActionPrompt;
                context.Reply(ActionListToString(CombatRound.combatActions));
                return true;
            }
            else
            {
                TopicState = TopicStates.EndTurn;
                context.Reply("You are out of moves.");
                return true;
            }
        }

        /// <summary>
        /// Creates string based on the actions remaining for this turn
        /// </summary>
        /// <param name="combatActions">Current list of actions</param>
        /// <returns>String of remaining actions</returns>
        private string ActionListToString(CombatRound.CombatActions combatActions)
        {
            string temp = "You can do one of the following actions:";
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