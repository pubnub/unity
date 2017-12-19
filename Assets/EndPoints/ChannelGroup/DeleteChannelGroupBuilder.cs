using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class DeleteChannelGroupBuilder
    {     
        private DeleteChannelGroupRequestBuilder pubBuilder;
        public DeleteChannelGroupBuilder ChannelGroup(string channelGroup){
            pubBuilder.ChannelGroup(channelGroup);
            return this;
        }
        
        public DeleteChannelGroupBuilder(PubNubUnity pn){
            pubBuilder = new DeleteChannelGroupRequestBuilder(pn);

            Debug.Log ("DeleteChannelGroupRequestBuilder Construct");
        }
        public void Async(Action<PNChannelGroupsDeleteGroupResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}