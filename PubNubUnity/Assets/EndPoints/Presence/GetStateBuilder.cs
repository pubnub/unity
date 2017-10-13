using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetStateBuilder
    {     
        private GetStateRequestBuilder pubBuilder;
        
        public GetStateBuilder(PubNubUnity pn){
            pubBuilder = new GetStateRequestBuilder(pn);

            Debug.Log ("GetStateBuilder Construct");
        }
        
        public GetStateBuilder UUID(string uuid){
            pubBuilder.UUID(uuid);
            return this;
        }

        public GetStateBuilder Channels(List<string> channels){
            pubBuilder.Channels(channels);
            return this;
        }

        public GetStateBuilder ChannelGroups(List<string> channelGroups){
            pubBuilder.ChannelGroups(channelGroups);
            return this;
        }
        public void Async(Action<PNGetStateResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}