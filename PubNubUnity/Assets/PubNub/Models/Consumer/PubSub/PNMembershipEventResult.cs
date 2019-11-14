using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNMembershipEventResult
    {
        public PNObjectsEvent ObjectsEvent { get; set;} 
        public string SpaceID { get; set;}
        public string UserID { get; set;}
        public string Description { get; set;}
        public Dictionary<string, object> Custom { get; set;}
        public string Subscription { get; set;} 
        public string Channel { get; set;} 
        public string Timestamp { get; set;} 
    }
}