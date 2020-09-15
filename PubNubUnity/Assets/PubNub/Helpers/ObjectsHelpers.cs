using System.Collections.Generic;

namespace PubNubAPI
{
    public static class ObjectsHelpers
    {
        public static PNObjectsEventType GetPNObjectsEventTypeFromString(string value)
        {
            switch (value)
            {
                case "uuid":
                    return PNObjectsEventType.PNObjectsUUIDEvent;
                case "channel":
                    return PNObjectsEventType.PNObjectsChannelEvent;
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
                case "set":
                    return PNObjectsEvent.PNObjectsEventSet;
                case "delete":
                    return PNObjectsEvent.PNObjectsEventDelete;
                default:
                    return PNObjectsEvent.PNObjectsNoneEvent;
            }
        }

        public static PNUUIDMetadataResult ExtractUUIDMetadata(Dictionary<string, object> objDataDict){
            PNUUIDMetadataResult pnUserResult = new PNUUIDMetadataResult();
            pnUserResult.ID = Utility.ReadMessageFromResponseDictionary(objDataDict, "id");
            pnUserResult.Name = Utility.ReadMessageFromResponseDictionary(objDataDict, "name");
            pnUserResult.ExternalID = Utility.ReadMessageFromResponseDictionary(objDataDict, "externalId");
            pnUserResult.ProfileURL = Utility.ReadMessageFromResponseDictionary(objDataDict, "profileUrl");
            pnUserResult.Email = Utility.ReadMessageFromResponseDictionary(objDataDict, "email");
            pnUserResult.Updated = Utility.ReadMessageFromResponseDictionary(objDataDict, "updated");
            pnUserResult.ETag = Utility.ReadMessageFromResponseDictionary(objDataDict, "eTag");
            pnUserResult.Custom = Utility.ReadDictionaryFromResponseDictionary(objDataDict, "custom");

            return pnUserResult;
        }

        public static PNChannelMetadataResult ExtractChannelMetadata(Dictionary<string, object> objDataDict){
            PNChannelMetadataResult pnSpaceResult = new PNChannelMetadataResult();
            pnSpaceResult.ID = Utility.ReadMessageFromResponseDictionary(objDataDict, "id");
            pnSpaceResult.Name = Utility.ReadMessageFromResponseDictionary(objDataDict, "name");
            pnSpaceResult.Description = Utility.ReadMessageFromResponseDictionary(objDataDict, "description");
            pnSpaceResult.Updated = Utility.ReadMessageFromResponseDictionary(objDataDict, "updated");
            pnSpaceResult.ETag = Utility.ReadMessageFromResponseDictionary(objDataDict, "eTag");
            pnSpaceResult.Custom = Utility.ReadDictionaryFromResponseDictionary(objDataDict, "custom");

            return pnSpaceResult;
        }

        public static PNMembers ExtractMembers(Dictionary<string, object> objDataDict){
            PNMembers pnMembers = new PNMembers();
            pnMembers.ID = Utility.ReadMessageFromResponseDictionary(objDataDict, "id");
            pnMembers.UUID = ObjectsHelpers.ExtractUUIDMetadata(Utility.ReadDictionaryFromResponseDictionary(objDataDict, "uuid"));
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
            pnMemberships.Channel = ObjectsHelpers.ExtractChannelMetadata(Utility.ReadDictionaryFromResponseDictionary(objDataDict, "channel"));            
            pnMemberships.Created = Utility.ReadMessageFromResponseDictionary(objDataDict, "created");
            pnMemberships.Updated = Utility.ReadMessageFromResponseDictionary(objDataDict, "updated");
            pnMemberships.ETag = Utility.ReadMessageFromResponseDictionary(objDataDict, "eTag");
            pnMemberships.Custom = Utility.ReadDictionaryFromResponseDictionary(objDataDict, "custom");
            
            return pnMemberships;
        }

        internal static PNMembersInputForJSON[] ConvertPNMembersInputForJSON(List<PNChannelMembersSet> input){
            List<PNMembersInputForJSON> pnMembersInputForJSONList = new List<PNMembersInputForJSON>();
            if(input!=null){
                foreach (PNChannelMembersSet pnMembersInput in input){
                    PNMembersInputForJSON pnMembersInputForJSON = new PNMembersInputForJSON();
                    pnMembersInputForJSON.custom = pnMembersInput.Custom;
                    pnMembersInputForJSON.uuid = new PNChannelMembersUUIDForJSON {
                        id = pnMembersInput.UUID.ID
                    };
                    pnMembersInputForJSONList.Add(pnMembersInputForJSON);
                }
            }
            return pnMembersInputForJSONList.ToArray();
        }

        internal static PNMembersRemoveForJSON[] ConvertPNMembersRemoveForJSON(List<PNChannelMembersRemove> input){
            List<PNMembersRemoveForJSON> pnMembersRemoveForJSONList = new List<PNMembersRemoveForJSON>();
            if(input!=null){
                foreach (PNChannelMembersRemove pnMembersRemove in input){
                    PNMembersRemoveForJSON pnMembersRemoveForJSON = new PNMembersRemoveForJSON();
                    pnMembersRemoveForJSON.uuid = new PNChannelMembersUUIDForJSON{
                        id = pnMembersRemove.UUID.ID
                    }; 
                    pnMembersRemoveForJSONList.Add(pnMembersRemoveForJSON);
                }
            }
            return pnMembersRemoveForJSONList.ToArray();
        }

       internal static PNMembershipsInputForJSON[] ConvertPNMembershipsInputForJSON(List<PNMembershipsSet> input){
            List<PNMembershipsInputForJSON> pnMembersInputForJSONList = new List<PNMembershipsInputForJSON>();
            if(input!=null){
                foreach (PNMembershipsSet pnMembersInput in input){
                    PNMembershipsInputForJSON pnMembersInputForJSON = new PNMembershipsInputForJSON();
                    pnMembersInputForJSON.custom = pnMembersInput.Custom;
                    pnMembersInputForJSON.channel = new PNMembershipsChannelForJSON{
                        id = pnMembersInput.Channel.ID
                    }; 
                    pnMembersInputForJSONList.Add(pnMembersInputForJSON);
                }
            }
            return pnMembersInputForJSONList.ToArray();
        }

        internal static PNMembershipsRemoveForJSON[] ConvertPNMembershipsRemoveForJSON(List<PNMembershipsRemove> input){
            List<PNMembershipsRemoveForJSON> pnMembersRemoveForJSONList = new List<PNMembershipsRemoveForJSON>();
            if(input!=null){
                foreach (PNMembershipsRemove pnMembersRemove in input){
                    PNMembershipsRemoveForJSON pnMembersRemoveForJSON = new PNMembershipsRemoveForJSON();
                    pnMembersRemoveForJSON.channel = new PNMembershipsChannelForJSON{
                        id = pnMembersRemove.Channel.ID
                    };
                    pnMembersRemoveForJSONList.Add(pnMembersRemoveForJSON);
                }
            }
            return pnMembersRemoveForJSONList.ToArray();
        } 

    }
}