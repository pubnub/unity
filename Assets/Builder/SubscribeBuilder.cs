using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class SubscribeBuilder: PubNubSubscribeBuilder<SubscribeBuilder>, IPubNubSubcribeBuilder<SubscribeBuilder>
    {
        //private PubNubBuilder<SubscribeBuilder> pubNubBuilder;
        private bool Reconnect { get; set;}
        public long Timetoken { get; set;}
        public List<string> Channels { get; private set;}
        public List<string> ChannelGroups { get; private set;}

        //public long TimetokenToUse { get; private set;}
        public SubscribeBuilder(PubNubUnity pn): base(pn){
        }

        #region IPubNubBuilder implementation

        public void Execute(){
            Reconnect = true;
            base.ReqState = new RequestState<SubscribeBuilder> ();
            base.ReqState.Reconnect = true;

            base.Execute (PNOperationType.PNSubscribeOperation, this);
        }

        public SubscribeBuilder SetChannels(List<string> channels){
            Channels = channels;
            //pubNubBuilder.SetChannels (channels);
            return this;
        }

        public SubscribeBuilder SetTimeToken(long timetoken){
            Timetoken = timetoken;
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

