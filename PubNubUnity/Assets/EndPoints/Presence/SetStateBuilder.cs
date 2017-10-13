using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class SetStateBuilder
    {     
        private SetStateRequestBuilder pubBuilder;
        
        public SetStateBuilder(PubNubUnity pn){
            pubBuilder = new SetStateRequestBuilder(pn);

            Debug.Log ("SetStateBuilder Construct");
        }
        
        public SetStateBuilder UUID(string uuid){
            pubBuilder.UUID(uuid);
            return this;
        }

        public SetStateBuilder State(Dictionary<string, object> state){
            pubBuilder.State(state);
            return this;
        }

        public SetStateBuilder Channels(List<string> channels){
            pubBuilder.Channels(channels);
            return this;
        }

        public SetStateBuilder ChannelGroups(List<string> channelGroups){
            pubBuilder.ChannelGroups(channelGroups);
            return this;
        }
        public void Async(Action<PNSetStateResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}