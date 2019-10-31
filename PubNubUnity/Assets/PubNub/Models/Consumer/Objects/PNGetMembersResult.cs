using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNGetMembersResult
    {
        public List<PNMembers> Data;
        public int TotalCount;
        public string Next;
        public string Prev;
    }

    public class PNMembers
    {
        public string ID { get; set;}
        public PNUserResult User { get; set;}
        public Dictionary<string, object> Custom { get; set;}
        public string Created { get; set;}
        public string Updated { get; set;}
        public string ETag { get; set;}
    }
}