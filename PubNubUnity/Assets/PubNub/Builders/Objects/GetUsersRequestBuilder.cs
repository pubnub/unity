using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetUsersRequestBuilder: PubNubNonSubBuilder<GetUsersRequestBuilder, PNGetUsersResult>, IPubNubNonSubscribeBuilder<GetUsersRequestBuilder, PNGetUsersResult>
    {        
        private int GetUsersLimit { get; set;}
        private string GetUsersEnd { get; set;}
        private string GetUsersStart { get; set;}
        private bool GetUsersCount { get; set;}
        private PNUserSpaceInclude[] CreateSpaceInclude { get; set;}
        private Dictionary<string, object> GetUserCustom { get; set;}
        
        public GetUsersRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNGetUsersOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNGetUsersResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public GetUsersRequestBuilder Include(PNUserSpaceInclude[] include){
            CreateSpaceInclude = include;
            return this;
        }
        public GetUsersRequestBuilder Limit(int limit){
            GetUsersLimit = limit;
            return this;
        }

        public GetUsersRequestBuilder Start(string start){
            GetUsersStart = start;
            return this;
        }
        public GetUsersRequestBuilder End(string end){
            GetUsersEnd = end;
            return this;
        }
        public GetUsersRequestBuilder Count(bool count){
            GetUsersCount = count;
            return this;
        }
        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;

            string[] includeString = Enum.GetValues(typeof(PNUserSpaceInclude))
                .Cast<int>()
                .Select(x => x.ToString())
                .ToArray();

            Uri request = BuildRequests.BuildObjectsGetUsersRequest(
                    GetUsersLimit,
                    GetUsersStart,
                    GetUsersEnd,
                    GetUsersCount,
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams
                );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            object[] c = deSerializedResult as object[];
            
            PNGetUsersResult pnUserResultList = new PNGetUsersResult();
            pnUserResultList.Data = new List<PNUserResult>();
            PNStatus pnStatus = new PNStatus();

            try{
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
                
                if(dictionary != null) {
                    object objData;
                    dictionary.TryGetValue("data", out objData);
                    if(objData!=null){
                        object[] objArr = objData as object[];
                        foreach (object data in objArr){
                            Dictionary<string, object> objDataDict = data as Dictionary<string, object>;
                            if(objDataDict!=null){
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
                                pnUserResultList.Data.Add(pnUserResult);
                            }  else {
                                pnUserResultList = null;
                                pnStatus = base.CreateErrorResponseFromException(new PubNubException("objDataDict null"), requestState, PNStatusCategory.PNUnknownCategory);
                            }  
                        }
                    }  else {
                        pnUserResultList = null;
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("objData null"), requestState, PNStatusCategory.PNUnknownCategory);
                    }  
                }
            } catch (Exception ex){
                pnUserResultList = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnUserResultList, pnStatus);

        }

    }
}