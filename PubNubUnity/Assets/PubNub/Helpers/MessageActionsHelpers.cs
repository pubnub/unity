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
        
        public static Dictionary<string, PNHistoryMessageActionsTypeValues> ExtractMessageActions(Dictionary<string, object> messageDataDict){
            object objMessageActions;
            messageDataDict.TryGetValue("actions", out objMessageActions);

            Dictionary<string, object> objDataDict = objMessageActions as Dictionary<string, object>;
            Dictionary<string, PNHistoryMessageActionsTypeValues> actionsReturn = new Dictionary<string, PNHistoryMessageActionsTypeValues>();
            if(objDataDict!=null){
                foreach (KeyValuePair<string, object> kvpAction in objDataDict)
                {
                    Dictionary<string, object> objActionValues = kvpAction.Value as Dictionary<string, object>; 
                    PNHistoryMessageActionsTypeValues pnHistoryMessageActionsTypeValues = new PNHistoryMessageActionsTypeValues();
                    pnHistoryMessageActionsTypeValues.MessageActionsTypeValues = new Dictionary<string, List<PNHistoryMessageActionsTypeValueAttributes>>();
                    foreach (KeyValuePair<string, object> kvpActionValues in objActionValues)
                    {
                        object[] actionAttributes = kvpActionValues.Value as object[];
                        List<PNHistoryMessageActionsTypeValueAttributes> actionValueAttributes = new List<PNHistoryMessageActionsTypeValueAttributes>();
                        foreach(object actionValueAttribute in actionAttributes){
                            Dictionary<string, object>  attributeValues= actionValueAttribute as Dictionary<string, object>;
                            string UUID = Utility.ReadMessageFromResponseDictionary(attributeValues, "uuid");
                            long actionTimetoken;
                            string log;
                            Utility.TryCheckKeyAndParseLong(attributeValues, "actionTimetoken", "actionTimetoken", out log, out actionTimetoken);

                            PNHistoryMessageActionsTypeValueAttributes pnHistoryMessageActionsTypeValueAttributes = new PNHistoryMessageActionsTypeValueAttributes();
                            pnHistoryMessageActionsTypeValueAttributes.UUID = UUID;
                            pnHistoryMessageActionsTypeValueAttributes.ActionTimetoken = actionTimetoken;
                            actionValueAttributes.Add(pnHistoryMessageActionsTypeValueAttributes);
                        }
                        pnHistoryMessageActionsTypeValues.MessageActionsTypeValues.Add(kvpActionValues.Key, actionValueAttributes);  
                    }
                    
                    actionsReturn.Add(kvpAction.Key, pnHistoryMessageActionsTypeValues);
                }
                return actionsReturn;
            } else {
                return null;
            }

        }



    }
}