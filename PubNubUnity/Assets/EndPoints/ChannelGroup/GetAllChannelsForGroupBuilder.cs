using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetAllChannelsForGroupBuilder
    {     
        private readonly GetAllChannelsForGroupRequestBuilder pubBuilder;
        
        public GetAllChannelsForGroupBuilder ChannelGroup(string channelGroupName){
            pubBuilder.ChannelGroup(channelGroupName);
            return this;
        }
        
        public GetAllChannelsForGroupBuilder(PubNubUnity pn){
            pubBuilder = new GetAllChannelsForGroupRequestBuilder(pn);
        }
        public void Async(Action<PNChannelGroupsAllChannelsResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}