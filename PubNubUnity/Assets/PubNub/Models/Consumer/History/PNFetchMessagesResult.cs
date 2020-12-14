using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNFetchMessagesResult {
        public Dictionary<string, List<PNMessageResult>> Channels { get; set;}
        public Dictionary<string, object> More { get; set;}
    }
}