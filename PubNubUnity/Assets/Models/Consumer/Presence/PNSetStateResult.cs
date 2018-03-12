using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNSetStateResult: PNResult
    {
        public Dictionary<string, object> StateByChannels  { get; set;}
    }
}