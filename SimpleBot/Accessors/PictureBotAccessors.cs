using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBot
{
    public class PictureBotAccessors
    {
        public PictureBotAccessors(ConversationState conversationState)
        {
            ConversationState = conversationState;
        }

        public ConversationState ConversationState { get; }
        public IStatePropertyAccessor<PictureBotState> PictureBotState { get; set; }
        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }
    }
}
