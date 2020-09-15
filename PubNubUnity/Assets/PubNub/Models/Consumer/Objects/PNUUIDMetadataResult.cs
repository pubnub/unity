using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNUUIDMetadataResult
    {
        public string ID { get; set;}
        public string Name { get; set;}
        public string ExternalID { get; set;}
        public string ProfileURL { get; set;}
        public string Email { get; set;}
        public Dictionary<string, object> Custom { get; set;}
        public string Updated { get; set;}
        public string ETag { get; set;}

    }
}

