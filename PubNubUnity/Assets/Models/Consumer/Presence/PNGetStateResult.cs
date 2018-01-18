using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNGetStateResult: PNResult
    {
        public Dictionary<string, object> StateByChannels  { get; set;}
    }

}