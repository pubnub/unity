using System;

namespace PubNubAPI
{
    public class PNMessageActionsResult
    {
        public string ActionType { get; set;}
        public string ActionValue { get; set;}
        public string ActionTimetoken { get; set;}
        public string MessageTimetoken { get; set;}
        public string UUID { get; set;}
    }
}

