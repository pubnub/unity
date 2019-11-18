using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class AddMessageActionsBuilder
    {     
        private readonly AddMessageActionsRequestBuilder addMessageActionsBuilder;
        
        public AddMessageActionsBuilder(PubNubUnity pn){
            addMessageActionsBuilder = new AddMessageActionsRequestBuilder(pn);
        }
        public AddMessageActionsBuilder Channel(string channel){
            addMessageActionsBuilder.Channel(channel);
            return this;
        }

        public AddMessageActionsBuilder MessageTimetoken(long messageTimetoken){
            addMessageActionsBuilder.MessageTimetoken(messageTimetoken);
            return this;
        }

        public AddMessageActionsBuilder MessageAction(MessageActionAdd action){
            addMessageActionsBuilder.MessageAction(action);
            return this;
        }

        public AddMessageActionsBuilder QueryParam(Dictionary<string, string> queryParam){
            addMessageActionsBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNMessageActionsResult, PNStatus> callback)
        {
            addMessageActionsBuilder.Async(callback);
        }
    }
}