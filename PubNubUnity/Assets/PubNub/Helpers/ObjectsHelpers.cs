using System.Collections.Generic;

namespace PubNubAPI
{
    public static class ObjectsHelpers
    {
        public static PNObjectsEventType GetPNObjectsEventTypeFromString(string value)
        {
            switch (value)
            {
                case "user":
                    return PNObjectsEventType.PNObjectsUserEvent;
                case "space":
                    return PNObjectsEventType.PNObjectsSpaceEvent;
                case "membership":
                    return PNObjectsEventType.PNObjectsMembershipEvent;
                default:
                    return PNObjectsEventType.PNObjectsNoneEvent;    
            }
        }

        public static PNObjectsEvent GetPNObjectsEventFromString(string value)
        {
            switch (value)
            {
                case "create":
                    return PNObjectsEvent.PNObjectsEventCreate;
                case "update":
                    return PNObjectsEvent.PNObjectsEventUpdate;
                case "delete":
                    return PNObjectsEvent.PNObjectsEventDelete;
                default:
                    return PNObjectsEvent.PNObjectsNoneEvent;
            }
        }

        public static PNUserResult ExtractUser(Dictionary<string, object> objDataDict){
            PNUserResult pnUserResult = new PNUserResult();
            pnUserResult.ID = Utility.ReadMessageFromResponseDictionary(objDataDict, "id");
            pnUserResult.Name = Utility.ReadMessageFromResponseDictionary(objDataDict, "name");
            pnUserResult.ExternalID = Utility.ReadMessageFromResponseDictionary(objDataDict, "externalId");
            pnUserResult.ProfileURL = Utility.ReadMessageFromResponseDictionary(objDataDict, "profileUrl");
            pnUserResult.Email = Utility.ReadMessageFromResponseDictionary(objDataDict, "email");
            pnUserResult.Created = Utility.ReadMessageFromResponseDictionary(objDataDict, "created");
            pnUserResult.Updated = Utility.ReadMessageFromResponseDictionary(objDataDict, "updated");
            pnUserResult.ETag = Utility.ReadMessageFromResponseDictionary(objDataDict, "eTag");
            pnUserResult.Custom = Utility.ReadDictionaryFromResponseDictionary(objDataDict, "custom");

            return pnUserResult;
        }

        public static PNSpaceResult ExtractSpace(Dictionary<string, object> objDataDict){
            PNSpaceResult pnSpaceResult = new PNSpaceResult();
            pnSpaceResult.ID = Utility.ReadMessageFromResponseDictionary(objDataDict, "id");
            pnSpaceResult.Name = Utility.ReadMessageFromResponseDictionary(objDataDict, "name");
            pnSpaceResult.Description = Utility.ReadMessageFromResponseDictionary(objDataDict, "description");
            pnSpaceResult.Created = Utility.ReadMessageFromResponseDictionary(objDataDict, "created");
            pnSpaceResult.Updated = Utility.ReadMessageFromResponseDictionary(objDataDict, "updated");
            pnSpaceResult.ETag = Utility.ReadMessageFromResponseDictionary(objDataDict, "eTag");
            pnSpaceResult.Custom = Utility.ReadDictionaryFromResponseDictionary(objDataDict, "custom");

            return pnSpaceResult;
        }

        public static PNMembers ExtractMembers(Dictionary<string, object> objDataDict){
            PNMembers pnMembers = new PNMembers();
            pnMembers.ID = Utility.ReadMessageFromResponseDictionary(objDataDict, "id");
            pnMembers.User = ObjectsHelpers.ExtractUser(Utility.ReadDictionaryFromResponseDictionary(objDataDict, "user"));
            pnMembers.Created = Utility.ReadMessageFromResponseDictionary(objDataDict, "created");
            pnMembers.Updated = Utility.ReadMessageFromResponseDictionary(objDataDict, "updated");
            pnMembers.ETag = Utility.ReadMessageFromResponseDictionary(objDataDict, "eTag");
            pnMembers.Custom = Utility.ReadDictionaryFromResponseDictionary(objDataDict, "custom");

            return pnMembers;
        }

        public static void ExtractPagingParamsAndTotalCount(Dictionary<string, object> dictionary, string totalCountName, string nextName, string prevName, out int totalCount, out string next, out string prev){
            next = Utility.ReadMessageFromResponseDictionary(dictionary, nextName);
            prev = Utility.ReadMessageFromResponseDictionary(dictionary, prevName);
            string log;
            Utility.TryCheckKeyAndParseInt(dictionary, totalCountName, totalCountName, out log, out totalCount);
        }

        public static PNMemberships ExtractMemberships(Dictionary<string, object> objDataDict){
            PNMemberships pnMemberships = new PNMemberships();
            pnMemberships.ID = Utility.ReadMessageFromResponseDictionary(objDataDict, "id");
            pnMemberships.Space = ObjectsHelpers.ExtractSpace(Utility.ReadDictionaryFromResponseDictionary(objDataDict, "space"));            
            pnMemberships.Created = Utility.ReadMessageFromResponseDictionary(objDataDict, "created");
            pnMemberships.Updated = Utility.ReadMessageFromResponseDictionary(objDataDict, "updated");
            pnMemberships.ETag = Utility.ReadMessageFromResponseDictionary(objDataDict, "eTag");
            pnMemberships.Custom = Utility.ReadDictionaryFromResponseDictionary(objDataDict, "custom");
            
            return pnMemberships;
        }

        internal static PNMembersInputForJSON[] ConvertPNMembersInputForJSON(List<PNMembersInput> input){
            List<PNMembersInputForJSON> pnMembersInputForJSONList = new List<PNMembersInputForJSON>();
            foreach (PNMembersInput pnMembersInput in input){
                PNMembersInputForJSON pnMembersInputForJSON = new PNMembersInputForJSON();
                pnMembersInputForJSON.custom = pnMembersInput.Custom;
                pnMembersInputForJSON.id = pnMembersInput.ID;
                pnMembersInputForJSONList.Add(pnMembersInputForJSON);
            }
            return pnMembersInputForJSONList.ToArray();
        }

        internal static PNMembersRemoveForJSON[] ConvertPNMembersRemoveForJSON(List<PNMembersRemove> input){
            List<PNMembersRemoveForJSON> pnMembersRemoveForJSONList = new List<PNMembersRemoveForJSON>();
            foreach (PNMembersRemove pnMembersRemove in input){
                PNMembersRemoveForJSON pnMembersRemoveForJSON = new PNMembersRemoveForJSON();
                pnMembersRemoveForJSON.id = pnMembersRemove.ID;
                pnMembersRemoveForJSONList.Add(pnMembersRemoveForJSON);
            }
            return pnMembersRemoveForJSONList.ToArray();
        }

    }
}