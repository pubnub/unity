using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public enum PNMessageActionsEvent
    {
        // PNMessageActionsEventAdded is the enum when the event `added` occurs
        PNMessageActionsEventAdded,
        // PNMessageActionsEventRemoved is the enum when the event `removed` occurs
        PNMessageActionsEventRemoved,
        PNMessageActionsNoneEvent,
    }

    public static class PNMessageActionsEventExtensions
    {
        public static string GetDescription(this PNMessageActionsEvent @this)
        {
            if (@this.Equals(PNMessageActionsEvent.PNMessageActionsEventAdded)){
                return "added";
            } else if (@this.Equals(PNMessageActionsEvent.PNMessageActionsEventRemoved)){
                return "removed";
            } else {
                return "";  
            }
        }
    }
}