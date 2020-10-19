using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNFileEventResult
    {
        public PNFileMessageAndDetails FileEvent { get; set;} 
        public string Subscription { get; set;} 
        public string Channel { get; set;} 
        public long Timetoken { get; set;} 
        public long OriginatingTimetoken { get; set;} 
        public object UserMetadata { get; set;} 
        public string IssuingClientId { get; set;} 
 
    }
}