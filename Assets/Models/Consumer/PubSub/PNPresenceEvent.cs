using System;

namespace PubNubAPI
{
    public class PNPresenceEvent
    {
        public string Action { get; set;} 
        public string UUID { get; set;} 
        public int Occupancy { get; set;} 
        public long Timestamp { get; set;}
        public object State { get; set;}

        public PNPresenceEvent(string action, string uuid, int Occupancy,
            long timestamp, object state){
            this.Action = action;
            this.UUID = uuid;
            this.Occupancy = Occupancy;
            this.Timestamp = timestamp;
            this.State = state;
        }
    }
}

