using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class HereNowBuilder
    {     
        private HereNowRequestBuilder pubBuilder;
        
        public HereNowBuilder(PubNubUnity pn){
            pubBuilder = new HereNowRequestBuilder(pn);

            Debug.Log ("HereNowBuilder Construct");
        }
        
        public HereNowBuilder IncludeUUIDs(bool includeUUIDs){
            pubBuilder.IncludeUUIDs(includeUUIDs);
            return this;
        }

        public HereNowBuilder IncludeState(bool includeState){
            pubBuilder.IncludeState(includeState);
            return this;
        }

        public HereNowBuilder Channels(List<string> channels){
            pubBuilder.Channels(channels);
            return this;
        }

        public HereNowBuilder ChannelGroups(List<string> channelGroups){
            pubBuilder.ChannelGroups(channelGroups);
            return this;
        }
        public void Async(Action<PNHereNowResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}