using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace PubNubAPI
{
    public sealed class PNLatency: MonoBehaviour {
        public float Time; //l_time
        public float Publish; //l_pub
        public float Presence; //l_pres
        public float AccessManager; //l_pam
        public float ChannelGroups; //l_cg
        public float History; //l_hist
        public float MobilePush; //l_push

        private SafeDictionary<long, float> TimeLatency = new SafeDictionary<long, float>(); 
        private SafeDictionary<long, float> PublishLatency = new SafeDictionary<long, float>(); 
        private SafeDictionary<long, float> PresenceLatency = new SafeDictionary<long, float>(); 
        private SafeDictionary<long, float> AccessManagerLatency = new SafeDictionary<long, float>(); 
        private SafeDictionary<long, float> ChannelGroupsLatency = new SafeDictionary<long, float>(); 
        private SafeDictionary<long, float> HistoryLatency = new SafeDictionary<long, float>(); 
        private SafeDictionary<long, float> MobilePushLatency = new SafeDictionary<long, float>(); 

        private static readonly DateTime epoch = new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Local);
        private bool RunUpdateLatencyLoop = false;

        private const float timerConst = 1; 
        private float timer = timerConst; 

        public void CleanUp(){
            RunUpdateLatencyLoop = false;
            Debug.Log("Stopping Latency Updator");
        }

        public DateTime FromUnixTime2(long unixTime)
        {
            return epoch.AddTicks(unixTime);
        }

        void Start(){
            RunUpdateLatencyLoop = true;
            
            Debug.Log("Running Latency Updator");
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
            //while(RunUpdateLatencyLoop){
            Debug.Log("In Latency Updator");
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

            UpdateLatency(ref TimeLatency, t, ref Time, "Time");
            UpdateLatency(ref PublishLatency, t, ref Publish, "Publish");
            UpdateLatency(ref PresenceLatency, t, ref Presence, "Presence");
            UpdateLatency(ref MobilePushLatency, t, ref MobilePush, "MobilePush");
            UpdateLatency(ref HistoryLatency, t, ref History, "History");
            UpdateLatency(ref ChannelGroupsLatency, t, ref ChannelGroups, "ChannelGroups");
            /*Debug.Log("Latency " + Time);
            Debug.Log("Latency " + Publish);
            Debug.Log("Latency " + Presence);
            Debug.Log("Latency " + MobilePush);
            Debug.Log("Latency " + History);
            Debug.Log("Latency " + ChannelGroups);*/
                //yield return new WaitForSeconds(1);
            //}
        }

        void UpdateLatency(ref SafeDictionary<long, float> dict, long t, ref float f, string name){
            List<long> keys = new List<long>(dict.Keys);
            float timeAvg = 0;
            foreach(long key in keys){
                if(key < t){
                    dict.Remove(key);
                    Debug.Log(name + "Latency " + key + " removed");
                    Debug.Log(name + "FromUnixTime removed:" + FromUnixTime2(key));
                } else {
                    timeAvg += dict[key];
                }
            }
            int count = dict.Count();
            if(count > 0){
                timeAvg /= count;
            }
            f = timeAvg;
            Debug.Log(name + "Latency " + f);
        }

        public void StoreLatency(long startTime, long endTime, PNOperationType operationType){
            float latency = (endTime - startTime)/10000000f; // seconds
            Debug.Log("Latency" + operationType.ToString()  + latency);
            //List<string> ls = new List<string>();
            //TODO Add delete history 
            switch(operationType){
                case PNOperationType.PNTimeOperation:
                    TimeLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNPublishOperation:
                    PublishLatency.Add(DateTime.UtcNow.Ticks, latency);
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
                case PNOperationType.PNFetchMessagesOperation:
                case PNOperationType.PNHistoryOperation:
                    HistoryLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
                case PNOperationType.PNAddChannelsToGroupOperation:
                case PNOperationType.PNChannelGroupsOperation:
                case PNOperationType.PNChannelsForGroupOperation:
                case PNOperationType.PNRemoveChannelsFromGroupOperation:
                case PNOperationType.PNRemoveGroupOperation:
                    ChannelGroupsLatency.Add(DateTime.UtcNow.Ticks, latency);
                    break;
            }

        }
    }

}