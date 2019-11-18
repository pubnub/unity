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
            long actionTimetoken;
            string log;
            Utility.TryCheckKeyAndParseLong(objDataDict, "actionTimetoken", "actionTimetoken", out log, out actionTimetoken);

            pnMessageActionsResult.ActionTimetoken = actionTimetoken;
            pnMessageActionsResult.ActionType = Utility.ReadMessageFromResponseDictionary(objDataDict, "type");
            pnMessageActionsResult.ActionValue = Utility.ReadMessageFromResponseDictionary(objDataDict, "value");
            
            long messageTimetoken;
            Utility.TryCheckKeyAndParseLong(objDataDict, "messageTimetoken", "messageTimetoken", out log, out messageTimetoken);
            pnMessageActionsResult.MessageTimetoken = messageTimetoken;
            pnMessageActionsResult.UUID = Utility.ReadMessageFromResponseDictionary(objDataDict, "uuid");

            return pnMessageActionsResult;
        }
    }
}