using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace PubNubAPI
{
    public sealed class PNLatency{
        public long Time {get;set;} //l_time
        public long Publish {get;set;} //l_pub
        public long Presence {get;set;} //l_pres
        public long AccessManager {get;set;} //l_pam
        public long ChannelGroups {get;set;} //l_cg
        public long History {get;set;} //l_hist
        public long MobilePush {get;set;} //l_push

        public SafeDictionary<long, long> TimeLatency = new SafeDictionary<long, long>(); 
        public SafeDictionary<long, long> PublishLatency = new SafeDictionary<long, long>(); 
        public SafeDictionary<long, long> PresenceLatency = new SafeDictionary<long, long>(); 
        public SafeDictionary<long, long> AccessManagerLatency = new SafeDictionary<long, long>(); 
        public SafeDictionary<long, long> ChannelGroupsLatency = new SafeDictionary<long, long>(); 
        public SafeDictionary<long, long> HistoryLatency = new SafeDictionary<long, long>(); 
        public SafeDictionary<long, long> MobilePushLatency = new SafeDictionary<long, long>(); 

        private static readonly DateTime epoch = new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Local);

        public DateTime FromUnixTime2(long unixTime)
        {
            return epoch.AddTicks(unixTime);
        }

        public void Update(){
            TimeSpan ts = TimeSpan.FromTicks(DateTime.UtcNow.Ticks);
            //double minutesFromTs = ts.TotalMinutes;
            Debug.Log("FromUnixTime now:" + FromUnixTime2(DateTime.UtcNow.Ticks));
            long t = DateTime.UtcNow.Ticks - 60 * 10000000;
            
            //TimeSpan ts2 = TimeSpan.FromTicks(t);
            Debug.Log("FromUnixTime now - 10:" + FromUnixTime2(t));

            /*TimeSpan ts2 = TimeSpan.FromTicks(t);
            double minutesFromTs2 = ts2.TotalMinutes;
            Debug.Log("t: " + t);
            Debug.Log("epoch: " + epoch.AddSeconds(t).ToString());*/

            List<long> keys = new List<long>(TimeLatency.Keys);
            long timeAvg = 0;
            foreach(long key in keys){
                if(key < t){
                    TimeLatency.Remove(key);
                    Debug.Log("TimeLatency " + key + " removed");
                    Debug.Log("FromUnixTime removed:" + FromUnixTime2(key));
                } else {
                    timeAvg += TimeLatency[key];
                }
            }
            int count = TimeLatency.Count();
            if(count > 0){
                timeAvg /= count;
            }
            Time = timeAvg;
            Debug.Log("TimeLatency " + Time);
            //yield return new WaitForSeconds(1);
        }

        public void StoreLatency(long startTime, long endTime, PNOperationType operationType){
            long latency = endTime - startTime;
            Debug.Log("Latency" + operationType.ToString()  + latency.ToString());
            List<string> ls = new List<string>();
            //TODO Add delete history 
            switch(operationType){
                case PNOperationType.PNTimeOperation:
                    TimeLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNWhereNowOperation:
                    PresenceLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNHistoryOperation:
                    HistoryLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNPublishOperation:
                    PublishLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNHereNowOperation:
                    PresenceLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNLeaveOperation:
                    PresenceLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNSetStateOperation:
                    PresenceLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNGetStateOperation:
                    PresenceLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNRemoveAllPushNotificationsOperation:
                    MobilePushLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNAddPushNotificationsOnChannelsOperation:
                    MobilePushLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNPushNotificationEnabledChannelsOperation:
                    MobilePushLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNRemovePushNotificationsFromChannelsOperation:
                    MobilePushLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNAddChannelsToGroupOperation:
                    ChannelGroupsLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNChannelGroupsOperation:
                    ChannelGroupsLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNChannelsForGroupOperation:
                    ChannelGroupsLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNFetchMessagesOperation:
                    HistoryLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNRemoveChannelsFromGroupOperation:
                    ChannelGroupsLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNRemoveGroupOperation:
                    ChannelGroupsLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
            }

        }
    }

}