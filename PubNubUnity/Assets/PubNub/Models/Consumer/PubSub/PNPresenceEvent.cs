using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNPresenceEvent
    {
        public string Action { get; set;} 
        public string UUID { get; set;} 
        public int Occupancy { get; set;} 
        public long Timestamp { get; set;}
        public object State { get; set;}
        internal List<string> Join { get; set;}
        internal List<string> Timeout { get; set;}
        internal List<string> Leave { get; set;}
        internal bool HereNowRefresh { get; set;}        

        public PNPresenceEvent(string action, string uuid, int Occupancy, long timestamp, object state, List<string> joins, List<string> leaves, List<string> timeouts, bool hereNowRefresh){
            this.Action = action;
            this.UUID = uuid;
            this.Occupancy = Occupancy;
            this.Timestamp = timestamp;
            this.State = state;
            this.Join = joins;
            this.Leave = leaves;
            this.Timeout = timeouts;
            this.HereNowRefresh = hereNowRefresh;
        }
    }
}

