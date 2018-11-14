using System;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class SubscribeRequestBuilder 
    {
        public long Timetoken { get; set;}
        public List<string> ChannelsToUse { get; private set;}
        public List<string> ChannelGroupsToUse { get; private set;}

        protected PubNubUnity PubNubInstance { get; set;}

        public bool IncludePresenceChannel {get; set;}

        public bool SubscribeToPresenceChannelOnly {get; set;}

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

            if(((this.ChannelsToUse == null) || (this.ChannelsToUse.Count <= 0)) && ((this.ChannelGroupsToUse == null) || (this.ChannelGroupsToUse.Count <= 0))){
                PNStatus pnStatus = Helpers.CreatePNStatus(
                    PNStatusCategory.PNUnknownCategory,
                    "Both Channels and ChannelGroups cannot be empty",
                    null,
                    true,                
                    PNOperationType.PNSubscribeOperation,
                    this.ChannelsToUse,
                    this.ChannelGroupsToUse,
                    null,
                    PubNubInstance
                );

                PubNubInstance.SubWorker.CreateEventArgsAndRaiseEvent(pnStatus);
            }

            List<ChannelEntity> subscribedChannels = this.PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups;
            List<ChannelEntity> newChannelEntities;
            List<string> rawChannels = this.ChannelsToUse;
            List<string> rawChannelGroups = this.ChannelGroupsToUse;
            CheckPresenceAndAddSuffix(ref rawChannels, IncludePresenceChannel, SubscribeToPresenceChannelOnly);
            CheckPresenceAndAddSuffix(ref rawChannelGroups, IncludePresenceChannel, SubscribeToPresenceChannelOnly);
            PNOperationType pnOpType = PNOperationType.PNSubscribeOperation;
            long timetokenToUse = this.Timetoken;
            
            bool channelsOrChannelGroupsAdded = this.PubNubInstance.SubscriptionInstance.TryRemoveDuplicatesCheckAlreadySubscribedAndGetChannels (pnOpType, rawChannels, rawChannelGroups, false, out newChannelEntities);
            if (channelsOrChannelGroupsAdded){
                this.PubNubInstance.SubscriptionInstance.Add (newChannelEntities);
                this.PubNubInstance.SubWorker.Add (timetokenToUse, subscribedChannels);
            }
            else {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("MultiChannelSubscribeInit: channelsOrChannelGroupsAdded {0}", channelsOrChannelGroupsAdded.ToString ()), PNLoggingMethod.LevelInfo);
                #endif

                PNStatus pnStatus = Helpers.CreatePNStatus(
                    PNStatusCategory.PNUnknownCategory,
                    CommonText.DuplicateChannelsOrChannelGroups,
                    new PubNubException(CommonText.DuplicateChannelsOrChannelGroups),
                    true,                
                    PNOperationType.PNSubscribeOperation,
                    rawChannels,
                    rawChannelGroups,
                    null,
                    PubNubInstance
                );

                PubNubInstance.SubWorker.CreateEventArgsAndRaiseEvent(pnStatus);
            }

            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog(string.Format ("channelsOrChannelGroupsAdded: {0}\nnewChannelEntities:{1}", channelsOrChannelGroupsAdded, newChannelEntities.Count), PNLoggingMethod.LevelInfo);
            #endif                                    
        }
        
        public void Channels(List<string> channelNames){
            ChannelsToUse = channelNames;
        }

        public void SetTimeToken(long timetoken){
            Timetoken = timetoken;
        }

        public void ChannelGroups(List<string> channelGroupNames){
            ChannelGroupsToUse = channelGroupNames;
        }
        #endregion
    }
}
