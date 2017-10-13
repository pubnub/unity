using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNPresenceEventResult
    {
        public string Event { get; set;} 
        public string Subscription { get; set;} 
        public string Channel { get; set;} 
        public string UUID { get; set;} 
        public long Timestamp { get; set;} 
        public long Timetoken { get; set;} 
        public int Occupancy { get; set;} 
        public object State { get; set;} 
        public object UserMetadata { get; set;} 
        public string IssuingClientId { get; set;} 
        public List<string> Join { get; set;}
        public List<string> Timeout { get; set;}
        public List<string> Leave { get; set;}

        public PNPresenceEventResult(string subscribedChannel, string actualchannel, string presenceEvent, long timetoken, long timestamp, object userMetadata, object state, string uuid, int occupancy, string issuingClientId, List<string> joins, List<string> leaves, List<string> timeouts){
            this.Subscription = subscribedChannel;// change to channel group
            this.Channel = actualchannel; // change to channel
            this.Event = presenceEvent;
            this.UUID = uuid;
            this.Occupancy = occupancy;
            this.Timetoken = timetoken;
            this.Timestamp = timestamp;
            this.State = state;
            this.UserMetadata = userMetadata;
            this.IssuingClientId = issuingClientId;
            this.Join = joins;
            this.Leave = leaves;
            this.Timeout = timeouts;            
        }
    }
}

