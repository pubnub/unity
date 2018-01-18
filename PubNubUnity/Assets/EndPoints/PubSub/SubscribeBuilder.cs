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

        public SubscribeBuilder SetChannels(List<string> channels){
            pubBuilder.SetChannels (channels);
            return this;
        }

        public SubscribeBuilder SetTimeToken(long timetoken){
            pubBuilder.SetTimeToken(timetoken);
            return this;
        }

        public SubscribeBuilder SetChannelGroups(List<string> channelGroups){
            pubBuilder.SetChannelGroups(channelGroups);
            return this;
        }
        #endregion
    }
}

