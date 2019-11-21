using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNGetMessageActionsResult
    {
        public List<PNMessageActionsResult> Data { get; set;}
        public PNGetMessageActionsMore More { get; set;}
    }

    public class PNGetMessageActionsMore
    {
        public string URL { get; set;}
        public long Start { get; set;}
        public long End { get; set;}
        public int Limit { get; set;}

    }

}

