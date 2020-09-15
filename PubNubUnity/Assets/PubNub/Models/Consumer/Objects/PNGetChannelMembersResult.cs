using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNGetChannelMembersResult
    {
        public List<PNMembers> Data {get; set;}
        public int TotalCount {get; set;}
        public string Next {get; set;}
        public string Prev {get; set;}
    }

    public class PNMembers
    {
        public string ID { get; set;}
        public PNUUIDMetadataResult UUID { get; set;}
        public Dictionary<string, object> Custom { get; set;}
        public string Created { get; set;}
        public string Updated { get; set;}
        public string ETag { get; set;}
    }
}