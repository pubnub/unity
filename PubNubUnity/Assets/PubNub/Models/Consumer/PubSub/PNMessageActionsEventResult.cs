using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNMessageActionsEventResult
    {
        public PNMessageActionsEvent MessageActionsEvent { get; set;} 
        public PNMessageActionsResult Data { get; set;}
        public string Subscription { get; set;} 
        public string Channel { get; set;} 
    }
}