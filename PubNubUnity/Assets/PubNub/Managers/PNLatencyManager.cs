using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace PubNubAPI
{
    public sealed class PNLatencyManager: MonoBehaviour {
        private float time; //l_time
        public float Time{
            get{return time;}
            set{time = value;}
        }
        private float publish; //l_pub
        public float Publish{
            get{return publish;}
            set{publish = value;}
        }
        private float presence; //l_pres
        public float Presence{
            get{return presence;}
            set{presence = value;}
        }
        private float channelGroups; //l_cg
        public float ChannelGroups{
            get{return channelGroups;}
            set{channelGroups = value;}
        }
        private float history; //l_hist
        public float History{
            get{return history;}
            set{history = value;}
        }
        private float messageCounts; //l_mc
        public float MessageCounts{
            get{return messageCounts;}
            set{messageCounts = value;}
        }
        private float mobilePush; //l_push
        public float MobilePush{
            get{return mobilePush;}
            set{mobilePush = value;}
        }

        private float signal; //l_sig
        public float Signal{
            get{return signal;}
            set{signal = value;}
        }

        private float messageActions; //l_msga
        public float MessageActions{
            get{return messageActions;}
            set{messageActions = value;}
        } 
        private float objects; //l_obj
        public float Objects{
            get{return objects;}
            set{objects = value;}
        }
        private float files; //l_file
        public float Files{
            get{return files;}
            set{objects = files;}
        }
        private SafeDictionary<long, float> TimeLatency = new SafeDictionary<long, float>(); 
        private SafeDictionary<long, float> PublishLatency = new SafeDictionary<long, float>(); 
        private SafeDictionary<long, float> PresenceLatency = new SafeDictionary<long, float>(); 
        private SafeDictionary<long, float> ChannelGroupsLatency = new SafeDictionary<long, float>(); 
        private SafeDictionary<long, float> HistoryLatency = new SafeDictionary<long, float>(); 
        private SafeDictionary<long, float> MessageCountsLatency = new SafeDictionary<long, float>(); 
        private SafeDictionary<long, float> MobilePushLatency = new SafeDictionary<long, float>(); 

        private SafeDictionary<long, float> SignalLatency = new SafeDictionary<long, float>();
        private SafeDictionary<long, float> MessageActionsLatency = new SafeDictionary<long, float>();
        private SafeDictionary<long, float> ObjectsLatency = new SafeDictionary<long, float>();
        private SafeDictionary<long, float> FileLatency = new SafeDictionary<long, float>();

        private static readonly DateTime epoch = new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Local);
        private bool RunUpdateLatencyLoop;

        private const float timerConst = 1; 
        private float timer = timerConst; 

        public void CleanUp(){
            RunUpdateLatencyLoop = false;
        }

        public DateTime FromUnixTime2(long unixTime)
        {
            return epoch.AddTicks(unixTime);
        }

        void Start(){
            RunUpdateLatencyLoop = true;
        }

        void Update(){
            if(RunUpdateLatencyLoop){
                timer -= UnityEngine.Time.deltaTime;
                if(timer <= 0)
                {
                    UpdateLatency();
                    timer = timerConst;
                }
            }
        }

        void UpdateLatency(){
            long t = DateTime.UtcNow.Ticks - 60 * 10000000;
            
            UpdateLatency(ref TimeLatency, t, ref time, "Time");
            UpdateLatency(ref PublishLatency, t, ref publish, "Publish");
            UpdateLatency(ref PresenceLatency, t, ref presence, "Presence");
            UpdateLatency(ref MobilePushLatency, t, ref mobilePush, "MobilePush");
            UpdateLatency(ref HistoryLatency, t, ref history, "History");
            UpdateLatency(ref MessageCountsLatency, t, ref messageCounts, "MessageCounts");
            UpdateLatency(ref ChannelGroupsLatency, t, ref channelGroups, "ChannelGroups");
            UpdateLatency(ref SignalLatency, t, ref signal, "Signal");
            UpdateLatency(ref MessageActionsLatency, t, ref messageActions, "MessageActions");
            UpdateLatency(ref ObjectsLatency, t, ref objects, "Objects");
            UpdateLatency(ref FileLatency, t, ref files, "File");
        }

        void UpdateLatency(ref SafeDictionary<long, float> dict, long t, ref float f, string name){
            List<long> keys = new List<long>(dict.Keys);
            float timeAvg = 0;
            foreach(long key in keys){
                if(key < t){
                    dict.Remove(key);
                } else {
                    timeAvg += dict[key];
                }
            }
            int count = dict.Count();
            if(count > 0){
                timeAvg /= count;
            }
            f = timeAvg;
        }

        public void StoreLatency(long startTime, long endTime, PNOperationType operationType){
            float latency = (endTime - startTime)/10000000f; // seconds
            //TODO Add delete history 
            switch(operationType){
                case PNOperationType.PNTimeOperation:
                    TimeLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNPublishOperation:
                case PNOperationType.PNPublishFileMessageOperation:
                    PublishLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNSignalOperation:
                    SignalLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNWhereNowOperation:
                case PNOperationType.PNHereNowOperation:
                case PNOperationType.PNLeaveOperation:
                case PNOperationType.PNSetStateOperation:
                case PNOperationType.PNGetStateOperation:
                    PresenceLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNRemoveAllPushNotificationsOperation:
                case PNOperationType.PNAddPushNotificationsOnChannelsOperation:
                case PNOperationType.PNPushNotificationEnabledChannelsOperation:
                case PNOperationType.PNRemovePushNotificationsFromChannelsOperation:
                    MobilePushLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNMessageCountsOperation:
                    MessageCountsLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNFetchMessagesOperation:
                case PNOperationType.PNHistoryOperation:
                case PNOperationType.PNHistoryWithActionsOperation:
                    HistoryLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNGetMessageActionsOperation:
                case PNOperationType.PNAddMessageActionsOperation:
                case PNOperationType.PNRemoveMessageActionsOperation:
                    MessageActionsLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;    
                case PNOperationType.PNSetUUIDMetadataOperation:
                case PNOperationType.PNGetAllUUIDMetadataOperation:
                case PNOperationType.PNGetUUIDMetadataOperation:
                case PNOperationType.PNRemoveUUIDMetadataOperation:
                case PNOperationType.PNGetAllChannelMetadataOperation:
                case PNOperationType.PNSetChannelMetadataOperation:
                case PNOperationType.PNRemoveChannelMetadataOperation:
                case PNOperationType.PNGetMembershipsOperation:
                case PNOperationType.PNGetChannelMembersOperation:
                case PNOperationType.PNManageMembershipsOperation:
                case PNOperationType.PNManageChannelMembersOperation:
                    ObjectsLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;    
                case PNOperationType.PNAddChannelsToGroupOperation:
                case PNOperationType.PNChannelGroupsOperation:
                case PNOperationType.PNChannelsForGroupOperation:
                case PNOperationType.PNRemoveChannelsFromGroupOperation:
                case PNOperationType.PNRemoveGroupOperation:
                    ChannelGroupsLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNDeleteFileOperation:
                case PNOperationType.PNDownloadFileOperation:
                case PNOperationType.PNGetFileURLOperation:
                case PNOperationType.PNListFilesOperation:
                case PNOperationType.PNSendFileOperation:
                    FileLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                default:
                    break;    
            }

        }
    }

}