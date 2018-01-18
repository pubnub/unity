using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class UnsubscribeBuilder
    {     
        private readonly LeaveRequestBuilder pubBuilder;

        public UnsubscribeBuilder Channels(List<string> channelNames){
            pubBuilder.Channels(channelNames);
            return this;
        }

        public UnsubscribeBuilder ChannelGroups(List<string> channelGroupNames){
            pubBuilder.ChannelGroups(channelGroupNames);
            return this;
        }
        
        public UnsubscribeBuilder(PubNubUnity pn){
            pubBuilder = new LeaveRequestBuilder(pn);
        }
        public void Async(Action<PNLeaveRequestResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}