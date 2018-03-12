using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class SetStateBuilder
    {     
        private readonly SetStateRequestBuilder pubBuilder;
        
        public SetStateBuilder(PubNubUnity pn){
            pubBuilder = new SetStateRequestBuilder(pn);
        }
        
        public SetStateBuilder UUID(string uuidForState){
            pubBuilder.UUID(uuidForState);
            return this;
        }

        public SetStateBuilder State(Dictionary<string, object> stateToSet){
            pubBuilder.State(stateToSet);
            return this;
        }

        public SetStateBuilder Channels(List<string> channelNames){
            pubBuilder.Channels(channelNames);
            return this;
        }

        public SetStateBuilder ChannelGroups(List<string> channelGroupNames){
            pubBuilder.ChannelGroups(channelGroupNames);
            return this;
        }
        public void Async(Action<PNSetStateResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}