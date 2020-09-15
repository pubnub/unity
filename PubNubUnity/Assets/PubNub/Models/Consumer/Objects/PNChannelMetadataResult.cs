using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNChannelMetadataResult
    {
        public string ID { get; set;}
        public string Name { get; set;}
        public string Description { get; set;}
        public Dictionary<string, object> Custom { get; set;}
        public string Updated { get; set;}
        public string ETag { get; set;}

    }
}

