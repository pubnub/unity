using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class AddChannelsToChannelGroupBuilder
    {     
        private readonly AddChannelsToChannelGroupRequestBuilder pubBuilder;

        public AddChannelsToChannelGroupBuilder Channels(List<string> channelNames){
            pubBuilder.Channels(channelNames);
            return this;
        }

        public AddChannelsToChannelGroupBuilder ChannelGroup(string channelGroupName){
            pubBuilder.ChannelGroup(channelGroupName);
            return this;
        }
        
        public AddChannelsToChannelGroupBuilder(PubNubUnity pn){
            pubBuilder = new AddChannelsToChannelGroupRequestBuilder(pn);
        }
        public void Async(Action<PNChannelGroupsAddChannelResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}