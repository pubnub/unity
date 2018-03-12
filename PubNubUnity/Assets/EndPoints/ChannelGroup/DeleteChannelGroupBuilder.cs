using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class DeleteChannelGroupBuilder
    {     
        private readonly DeleteChannelGroupRequestBuilder pubBuilder;
        public DeleteChannelGroupBuilder ChannelGroup(string channelGroupName){
            pubBuilder.ChannelGroup(channelGroupName);
            return this;
        }
        
        public DeleteChannelGroupBuilder(PubNubUnity pn){
            pubBuilder = new DeleteChannelGroupRequestBuilder(pn);
        }
        public void Async(Action<PNChannelGroupsDeleteGroupResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}