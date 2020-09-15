using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNChannelEventResult
    {
        public PNObjectsEvent ObjectsEvent { get; set;} 
        public string ChannelID { get; set;}
        public string Name { get; set;}
        public string Description { get; set;}
        public Dictionary<string, object> Custom { get; set;}
        public string Created { get; set;}
        public string Updated { get; set;}
        public string ETag { get; set;}

        public object Payload { get; set;} 
        public string Subscription { get; set;} 
        public string Channel { get; set;} 
        public string Timestamp { get; set;} 
    }
}