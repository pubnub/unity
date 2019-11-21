using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class RemoveMessageActionsBuilder
    {
        private readonly RemoveMessageActionsRequestBuilder removeMessageActionsBuilder;

        public RemoveMessageActionsBuilder(PubNubUnity pn)
        {
            removeMessageActionsBuilder = new RemoveMessageActionsRequestBuilder(pn);
        }

        public RemoveMessageActionsBuilder Channel(string channel)
        {
            removeMessageActionsBuilder.Channel(channel);
            return this;
        }
        public RemoveMessageActionsBuilder MessageTimetoken(long messageTimetoken){
            removeMessageActionsBuilder.MessageTimetoken(messageTimetoken);
            return this;
        }

        public RemoveMessageActionsBuilder ActionTimetoken(long actionTimetoken){
            removeMessageActionsBuilder.ActionTimetoken(actionTimetoken);
            return this;
        }

        public RemoveMessageActionsBuilder QueryParam(Dictionary<string, string> queryParam)
        {
            removeMessageActionsBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNRemoveMessageActionsResult, PNStatus> callback)
        {
            removeMessageActionsBuilder.Async(callback);
        }
    }

}