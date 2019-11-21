using System;

namespace PubNubAPI
{
    public class PNMessageActionsResult
    {
        public string ActionType { get; set;}
        public string ActionValue { get; set;}
        public long ActionTimetoken { get; set;}
        public long MessageTimetoken { get; set;}
        public string UUID { get; set;}
    }
}

