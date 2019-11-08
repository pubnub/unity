using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNMembershipsResult
    {
        public List<PNMemberships> Data {get; set;}
        public int TotalCount {get; set;}
        public string Next {get; set;}
        public string Prev {get; set;}
    }

    public class PNMemberships
    {
        public string ID { get; set;}
        public PNSpaceResult Space { get; set;}
        public Dictionary<string, object> Custom { get; set;}
        public string Created { get; set;}
        public string Updated { get; set;}
        public string ETag { get; set;}
    }
}