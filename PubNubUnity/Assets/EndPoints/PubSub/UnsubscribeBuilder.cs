using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class UnsubscribeBuilder
    {     
        private LeaveRequestBuilder pubBuilder;

        public UnsubscribeBuilder Channels(List<string> channels){
            pubBuilder.Channels(channels);
            return this;
        }

        public UnsubscribeBuilder ChannelGroups(List<string> channelGroup){
            pubBuilder.ChannelGroups(channelGroup);
            return this;
        }
        
        public UnsubscribeBuilder(PubNubUnity pn){
            pubBuilder = new LeaveRequestBuilder(pn);

            Debug.Log ("UnsubscribeBuilder Construct");
        }
        public void Async(Action<PNLeaveRequestResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}