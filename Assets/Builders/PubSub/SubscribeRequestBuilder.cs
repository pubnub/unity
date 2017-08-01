using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class SubscribeRequestBuilder //: PubNubSubscribeBuilder<SubscribeRequestBuilder>, IPubNubSubcribeBuilder<SubscribeRequestBuilder>
    {
        //private PubNubBuilder<SubscribeBuilder> pubNubBuilder;
        private bool ReconnectSub = true;
        public long Timetoken { get; set;}
        public List<string> Channels { get; private set;}
        public List<string> ChannelGroups { get; private set;}

        protected PubNubUnity PubNubInstance { get; set;}

        private RequestState<SubscribeRequestBuilder> ReqState;

        public SubscribeRequestBuilder(PubNubUnity pn) {//: base(pn){
            PubNubInstance = pn;
        }

        #region IPubNubBuilder implementation

        public void Execute(){
            //base.ReqState = new RequestState<SubscribeRequestBuilder> ();
            //base.ReqState.Reconnect = ReconnectSub;

            //base.Execute (PNOperationType.PNSubscribeOperation, this);
            ReqState = new RequestState<SubscribeRequestBuilder> ();
            this.PubNubInstance.SubWorker.Add (PNOperationType.PNSubscribeOperation, this, ReqState, this.PubNubInstance);
                
        }
        public void Reconnect(bool reconnect) {
            ReconnectSub = reconnect;
        }

        public void SetChannels(List<string> channels){
            Channels = channels;
            //pubNubBuilder.SetChannels (channels);
            //return this;
        }

        public void SetTimeToken(long timetoken){
            Timetoken = timetoken;
            //return this;
        }

        public void SetChannelGroups(List<string> channelGroups){
            ChannelGroups = channelGroups;
            //pubNubBuilder.SetChannelGroups(channelGroups);
            //return this;
        }
        #endregion
    }
}
