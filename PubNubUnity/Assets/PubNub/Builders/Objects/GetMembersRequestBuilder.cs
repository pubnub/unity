using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetMembersRequestBuilder: PubNubNonSubBuilder<GetMembersRequestBuilder, PNGetMembersResult>, IPubNubNonSubscribeBuilder<GetMembersRequestBuilder, PNGetMembersResult>
    {        
        private string GetMembersSpaceID { get; set;}
        private int GetMembersLimit { get; set;}
        private string GetMembersEnd { get; set;}
        private string GetMembersStart { get; set;}
        private bool GetMembersCount { get; set;}
        private PNMembersInclude[] CreateSpaceInclude { get; set;}
        private Dictionary<string, object> GetUserCustom { get; set;}
        
        public GetMembersRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNGetMembersOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNGetMembersResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public GetMembersRequestBuilder SpaceID(string id){
            GetMembersSpaceID = id;
            return this;
        }

        public GetMembersRequestBuilder Include(PNMembersInclude[] include){
            CreateSpaceInclude = include;
            return this;
        }
        public GetMembersRequestBuilder Limit(int limit){
            GetMembersLimit = limit;
            return this;
        }

        public GetMembersRequestBuilder Start(string start){
            GetMembersStart = start;
            return this;
        }
        public GetMembersRequestBuilder End(string end){
            GetMembersEnd = end;
            return this;
        }
        public GetMembersRequestBuilder Count(bool count){
            GetMembersCount = count;
            return this;
        }
        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;

            string[] includeString = Enum.GetValues(typeof(PNMembersInclude))
                .Cast<int>()
                .Select(x => x.ToString())
                .ToArray();

            Uri request = BuildRequests.BuildObjectsGetMembersRequest(
                    GetMembersSpaceID,
                    GetMembersLimit,
                    GetMembersStart,
                    GetMembersEnd,
                    GetMembersCount,
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams
                );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            object[] c = deSerializedResult as object[];
            
            PNGetMembersResult pnGetMembersResult = new PNGetMembersResult();
            pnGetMembersResult.Data = new List<PNMembers>();
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
                                PNMembers pnMembers = new PNMembers();
                                pnMembers.ID = Utility.ReadMessageFromResponseDictionary(objDataDict, "id");
                                pnMembers.User = ObjectsHelpers.ExtractUser(Utility.ReadDictionaryFromResponseDictionary(objDataDict, "user"));
                                pnMembers.Created = Utility.ReadMessageFromResponseDictionary(objDataDict, "created");
                                pnMembers.Updated = Utility.ReadMessageFromResponseDictionary(objDataDict, "updated");
                                pnMembers.ETag = Utility.ReadMessageFromResponseDictionary(objDataDict, "eTag");
                                pnMembers.Custom = Utility.ReadDictionaryFromResponseDictionary(objDataDict, "custom");
                                pnGetMembersResult.Data.Add(pnMembers);
                            }  else {
                                pnGetMembersResult = null;
                                pnStatus = base.CreateErrorResponseFromException(new PubNubException("objDataDict null"), requestState, PNStatusCategory.PNUnknownCategory);
                            }  
                        }
                    }  else {
                        pnGetMembersResult = null;
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("objData null"), requestState, PNStatusCategory.PNUnknownCategory);
                    }  
                }
            } catch (Exception ex){
                pnGetMembersResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnGetMembersResult, pnStatus);

        }

    }
}