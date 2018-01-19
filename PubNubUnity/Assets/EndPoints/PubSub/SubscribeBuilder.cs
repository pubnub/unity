using System;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class SubscribeBuilder
    {
        private readonly SubscribeRequestBuilder pubBuilder;
        public SubscribeBuilder(PubNubUnity pn){
            pubBuilder = new SubscribeRequestBuilder(pn);
        }
        

        #region IPubNubBuilder implementation

        public void Execute(){
            pubBuilder.Execute();
        }

        public SubscribeBuilder WithPresence(){
            pubBuilder.WithPresence();
            return this;
        }

        public SubscribeBuilder Channels(List<string> channelNames){
            pubBuilder.Channels (channelNames);
            return this;
        }

        public SubscribeBuilder SetTimeToken(long timetoken){
            pubBuilder.SetTimeToken(timetoken);
            return this;
        }

        public SubscribeBuilder ChannelGroups(List<string> channelGroupNames){
            pubBuilder.ChannelGroups(channelGroupNames);
            return this;
        }
        #endregion
    }
}

