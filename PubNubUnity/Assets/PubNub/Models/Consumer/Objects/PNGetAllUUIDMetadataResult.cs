using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNGetAllUUIDMetadataResult
    {
        public List<PNUUIDMetadataResult> Data { get; set; }
        public int TotalCount {get; set;}
        public string Next {get; set;}
        public string Prev {get; set;}

    }
}
