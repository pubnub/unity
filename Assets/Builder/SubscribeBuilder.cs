using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class SubscribeBuilder: PubNubBuilder<SubscribeBuilder>, IPubNubSubcribeBuilder<SubscribeBuilder>
    {
        //private PubNubBuilder<SubscribeBuilder> pubNubBuilder;

        public List<string> Channels { get; private set;}
        public List<string> ChannelGroups { get; private set;}

        public long TimetokenToUse { get; private set;}

        #region IPubNubBuilder implementation

        public void Execute(){
            
            base.Execute (PNOperationType.PNSubscribeOperation, this);
        }

        public SubscribeBuilder SetChannels(List<string> channels){
            Channels = channels;
            //pubNubBuilder.SetChannels (channels);
            return this;
        }

        public SubscribeBuilder SetTimeToken(long timetoken){
            TimetokenToUse = timetoken;
            return this;
        }

        public SubscribeBuilder SetChannelGroups(List<string> channelGroups){
            ChannelGroups = channelGroups;
            //pubNubBuilder.SetChannelGroups(channelGroups);
            return this;
        }

        #endregion
    }
}

