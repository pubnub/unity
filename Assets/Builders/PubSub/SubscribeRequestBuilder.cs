using System;
using System.Collections.Generic;
using UnityEngine;

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

        //private RequestState<SubscribeRequestBuilder> ReqState;

        public SubscribeRequestBuilder(PubNubUnity pn) {//: base(pn){
            PubNubInstance = pn;
        }

        #region IPubNubBuilder implementation

        public void Execute(){
            List<ChannelEntity> subscribedChannels = this.PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups;
            List<ChannelEntity> newChannelEntities;
            List<string> rawChannels = this.Channels;
            List<string> rawChannelGroups = this.ChannelGroups;
            PNOperationType pnOpType = PNOperationType.PNSubscribeOperation;
            long timetokenToUse = this.Timetoken;
            
            bool channelsOrChannelGroupsAdded = this.PubNubInstance.SubscriptionInstance.RemoveDuplicatesCheckAlreadySubscribedAndGetChannels (pnOpType, rawChannels, rawChannelGroups, false, out newChannelEntities);
            if (channelsOrChannelGroupsAdded){
                this.PubNubInstance.SubscriptionInstance.Add (newChannelEntities);
                this.PubNubInstance.SubWorker.Add (pnOpType, timetokenToUse, subscribedChannels);
            }
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                //TODO raise duplicate event
                LoggingMethod.WriteToLog (string.Format ("MultiChannelSubscribeInit: channelsOrChannelGroupsAdded {1}", channelsOrChannelGroupsAdded.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity, PubNubInstance.PNConfig.LogVerbosity);
            }
            #endif

            Debug.Log ("channelsOrChannelGroupsAdded" + channelsOrChannelGroupsAdded);
            Debug.Log ("newChannelEntities" + newChannelEntities.Count);
        }
        
        /*public void Reconnect(bool reconnect) {
            ReconnectSub = reconnect;
        }*/

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
