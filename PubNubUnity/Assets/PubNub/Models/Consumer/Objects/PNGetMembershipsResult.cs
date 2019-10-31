using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNGetMembershipsResult
    {
        public List<PNMemberships> Data;
        public int TotalCount;
        public string Next;
        public string Prev;
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