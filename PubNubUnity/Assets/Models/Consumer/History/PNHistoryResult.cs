using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNHistoryResult: PNResult
    {
        public List<PNHistoryItemResult> Messages { get; set;}
        public long StartTimetoken { get; set;}
        public long EndTimetoken { get; set;}
    }

    public class PNHistoryItemResult {

        public long Timetoken { get; set;}
        public object Entry { get; set;}

    }
}