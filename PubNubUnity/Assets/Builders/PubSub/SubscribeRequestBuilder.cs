using System;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class SubscribeRequestBuilder 
    {
        private bool ReconnectSub = true;
        public long Timetoken { get; set;}
        public List<string> Channels { get; private set;}
        public List<string> ChannelGroups { get; private set;}

        protected PubNubUnity PubNubInstance { get; set;}

        public bool IncludePresenceChannel = false;

        public bool SubscribeToPresenceChannelOnly = false;

        public SubscribeRequestBuilder(PubNubUnity pn) {
            PubNubInstance = pn;
        }

        #region IPubNubBuilder implementation

        public void WithPresence(){
            IncludePresenceChannel = true;
        }

        public void PresenceOnly(){
            SubscribeToPresenceChannelOnly = true;
        }

        void CheckPresenceAndAddSuffix(ref List<string> rawChannels, bool includePresenceChannel, bool subscribeToPresenceChannelOnly){
            if(includePresenceChannel || subscribeToPresenceChannelOnly){
                List<string> newChannels = new List<string>();
                if(rawChannels != null){
                    foreach (string ch in rawChannels){
                        string presenceChannel = string.Format("{0}{1}", ch, Utility.PresenceChannelSuffix);
                        if(!ch.Contains(Utility.PresenceChannelSuffix)){
                            if(!rawChannels.Contains(presenceChannel)){
                                newChannels.Add(presenceChannel);
                            }
                        } else if (ch.Contains(Utility.PresenceChannelSuffix) && (subscribeToPresenceChannelOnly)){
                        newChannels.Add(presenceChannel);     
                        }
                    }
                    if(includePresenceChannel){
                        rawChannels.AddRange(newChannels);
                    } else {
                        rawChannels = newChannels;
                    }
                }
            }
        }

        public void Execute(){

            if(((this.Channels == null) || (this.Channels.Count <= 0)) && ((this.ChannelGroups == null) || (this.ChannelGroups.Count <= 0))){
                PNStatus pnStatus = Helpers.CreatePNStatus(
                    PNStatusCategory.PNUnknownCategory,
                    "Both Channels and ChannelGroups cannot be empty",
                    null,
                    true,                
                    PNOperationType.PNSubscribeOperation,
                    this.Channels,
                    this.ChannelGroups,
                    null,
                    PubNubInstance
                );

                PubNubInstance.SubWorker.CreateEventArgsAndRaiseEvent(pnStatus);
            }

            List<ChannelEntity> subscribedChannels = this.PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups;
            List<ChannelEntity> newChannelEntities;
            List<string> rawChannels = this.Channels;
            List<string> rawChannelGroups = this.ChannelGroups;
            CheckPresenceAndAddSuffix(ref rawChannels, IncludePresenceChannel, SubscribeToPresenceChannelOnly);
            CheckPresenceAndAddSuffix(ref rawChannelGroups, IncludePresenceChannel, SubscribeToPresenceChannelOnly);
            PNOperationType pnOpType = PNOperationType.PNSubscribeOperation;
            long timetokenToUse = this.Timetoken;
            
            bool channelsOrChannelGroupsAdded = this.PubNubInstance.SubscriptionInstance.TryRemoveDuplicatesCheckAlreadySubscribedAndGetChannels (pnOpType, rawChannels, rawChannelGroups, false, out newChannelEntities);
            if (channelsOrChannelGroupsAdded){
                this.PubNubInstance.SubscriptionInstance.Add (newChannelEntities);
                this.PubNubInstance.SubWorker.Add (timetokenToUse, subscribedChannels);
            }
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("MultiChannelSubscribeInit: channelsOrChannelGroupsAdded {0}", channelsOrChannelGroupsAdded.ToString ()), PNLoggingMethod.LevelInfo);
                #endif

                PNStatus pnStatus = Helpers.CreatePNStatus(
                    PNStatusCategory.PNUnknownCategory,
                    CommonText.DuplicateChannelsOrChannelGroups,
                    new Exception(CommonText.DuplicateChannelsOrChannelGroups),
                    true,                
                    PNOperationType.PNSubscribeOperation,
                    rawChannels,
                    rawChannelGroups,
                    null,
                    PubNubInstance
                );

                PubNubInstance.SubWorker.CreateEventArgsAndRaiseEvent(pnStatus);
            }
            #endif

            Debug.Log ("channelsOrChannelGroupsAdded" + channelsOrChannelGroupsAdded);
            Debug.Log ("newChannelEntities" + newChannelEntities.Count);
        }
        
        public void SetChannels(List<string> channels){
            Channels = channels;
        }

        public void SetTimeToken(long timetoken){
            Timetoken = timetoken;
        }

        public void SetChannelGroups(List<string> channelGroups){
            ChannelGroups = channelGroups;
        }
        #endregion
    }
}
