using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNGetAllChannelMetadataResult
    {
        public List<PNChannelMetadataResult> Data { get; set; }
        public int TotalCount {get; set;}
        public string Next {get; set;}
        public string Prev {get; set;}

    }
}