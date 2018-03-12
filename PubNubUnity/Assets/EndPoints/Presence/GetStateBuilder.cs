using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetStateBuilder
    {     
        private readonly GetStateRequestBuilder pubBuilder;
        
        public GetStateBuilder(PubNubUnity pn){
            pubBuilder = new GetStateRequestBuilder(pn);

        }
        
        public GetStateBuilder UUID(string uuidForState){
            pubBuilder.UUID(uuidForState);
            return this;
        }

        public GetStateBuilder Channels(List<string> channelNames){
            pubBuilder.Channels(channelNames);
            return this;
        }

        public GetStateBuilder ChannelGroups(List<string> channelGroupNames){
            pubBuilder.ChannelGroups(channelGroupNames);
            return this;
        }
        public void Async(Action<PNGetStateResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}