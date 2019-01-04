using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class PresenceHeartbeatBuilder
    {     
        private readonly PresenceHeartbeatRequestBuilder pubBuilder;
        
        public PresenceHeartbeatBuilder(PubNubUnity pn){
            pubBuilder = new PresenceHeartbeatRequestBuilder(pn);
        }
        
        public PresenceHeartbeatBuilder Connected(bool connected){
            pubBuilder.Connected(connected);
            return this;
        }

        public PresenceHeartbeatBuilder State(Dictionary<string, object> state){
            pubBuilder.State(state);
            return this;
        }

        public PresenceHeartbeatBuilder Channels(List<string> channelNames){
            pubBuilder.Channels(channelNames);
            return this;
        }

        public PresenceHeartbeatBuilder ChannelGroups(List<string> channelGroupNames){
            pubBuilder.ChannelGroups(channelGroupNames);
            return this;
        }

        public PresenceHeartbeatBuilder QueryParam(Dictionary<string, string> queryParam){
            pubBuilder.QueryParam(queryParam);
            return this;
        }
        
        public void Async(Action<PNPresenceHeartbeatResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}