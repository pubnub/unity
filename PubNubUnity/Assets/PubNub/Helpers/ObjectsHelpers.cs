using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Security.Cryptography;
using System.Linq;

namespace PubNubAPI
{
    public static class ObjectsHelpers
    {
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

    }
}