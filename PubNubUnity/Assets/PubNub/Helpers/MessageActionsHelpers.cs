using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public static class MessageActionsHelpers
    {
        public static PNMessageActionsEvent GetPNMessageActionsEventFromString(string value)
        {
            switch (value)
            {
                case "added":
                    return PNMessageActionsEvent.PNMessageActionsEventAdded;
                case "removed":
                    return PNMessageActionsEvent.PNMessageActionsEventRemoved;
                default:
                    return PNMessageActionsEvent.PNMessageActionsNoneEvent;
            }
        }

        public static PNMessageActionsResult ExtractMessageAction(Dictionary<string, object> objDataDict){
            PNMessageActionsResult pnMessageActionsResult = new PNMessageActionsResult();
            pnMessageActionsResult.ActionTimetoken = Utility.ReadMessageFromResponseDictionary(objDataDict, "actionTimetoken");
            pnMessageActionsResult.ActionType = Utility.ReadMessageFromResponseDictionary(objDataDict, "type");
            pnMessageActionsResult.ActionValue = Utility.ReadMessageFromResponseDictionary(objDataDict, "value");
            pnMessageActionsResult.MessageTimetoken = Utility.ReadMessageFromResponseDictionary(objDataDict, "messageTimetoken");

            return pnMessageActionsResult;
        }
    }
}