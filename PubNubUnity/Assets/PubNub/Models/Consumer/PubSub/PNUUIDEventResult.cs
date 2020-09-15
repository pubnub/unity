using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNUUIDEventResult
    {
        public PNObjectsEvent ObjectsEvent { get; set;} 
        public string Subscription { get; set;} 
        public string Channel { get; set;} 
        public string Timestamp { get; set;} 
        public string UUID { get; set;} 
        public string Name { get; set;}
        public string ExternalID { get; set;}
        public string ProfileURL { get; set;}
        public string Email { get; set;}
        public Dictionary<string, object> Custom { get; set;}
        public string Created { get; set;}
        public string Updated { get; set;}
        public string ETag { get; set;}

    }
}