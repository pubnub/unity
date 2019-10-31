using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetMembershipsRequestBuilder: PubNubNonSubBuilder<GetMembershipsRequestBuilder, PNGetMembershipsResult>, IPubNubNonSubscribeBuilder<GetMembershipsRequestBuilder, PNGetMembershipsResult>
    {    
        private string GetMembershipsUserID { get; set;}    
        private int GetMembershipsLimit { get; set;}
        private string GetMembershipsEnd { get; set;}
        private string GetMembershipsStart { get; set;}
        private bool GetMembershipsCount { get; set;}
        private PNMembershipsInclude[] CreateSpaceInclude { get; set;}
        private Dictionary<string, object> GetUserCustom { get; set;}
        
        public GetMembershipsRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNGetMembershipsOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNGetMembershipsResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public GetMembershipsRequestBuilder UserID(string id){
            GetMembershipsUserID = id;
            return this;
        }

        public GetMembershipsRequestBuilder Include(PNMembershipsInclude[] include){
            CreateSpaceInclude = include;
            return this;
        }
        public GetMembershipsRequestBuilder Limit(int limit){
            GetMembershipsLimit = limit;
            return this;
        }

        public GetMembershipsRequestBuilder Start(string start){
            GetMembershipsStart = start;
            return this;
        }
        public GetMembershipsRequestBuilder End(string end){
            GetMembershipsEnd = end;
            return this;
        }
        public GetMembershipsRequestBuilder Count(bool count){
            GetMembershipsCount = count;
            return this;
        }
        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;

            string[] includeString = Enum.GetValues(typeof(PNMembershipsInclude))
                .Cast<int>()
                .Select(x => x.ToString())
                .ToArray();

            Uri request = BuildRequests.BuildObjectsGetMembershipsRequest(
                    GetMembershipsUserID,
                    GetMembershipsLimit,
                    GetMembershipsStart,
                    GetMembershipsEnd,
                    GetMembershipsCount,
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams
                );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            object[] c = deSerializedResult as object[];
            
            PNGetMembershipsResult pnGetMembershipsResult = new PNGetMembershipsResult();
            pnGetMembershipsResult.Data = new List<PNMemberships>();
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
                                PNMemberships pnMemberships = new PNMemberships();
                                pnMemberships.ID = Utility.ReadMessageFromResponseDictionary(objDataDict, "id");
                                pnMemberships.Space = ObjectsHelpers.ExtractSpace(Utility.ReadDictionaryFromResponseDictionary(objDataDict, "space"));
                                pnMemberships.Created = Utility.ReadMessageFromResponseDictionary(objDataDict, "created");
                                pnMemberships.Updated = Utility.ReadMessageFromResponseDictionary(objDataDict, "updated");
                                pnMemberships.ETag = Utility.ReadMessageFromResponseDictionary(objDataDict, "eTag");
                                pnMemberships.Custom = Utility.ReadDictionaryFromResponseDictionary(objDataDict, "custom");
                                pnGetMembershipsResult.Data.Add(pnMemberships);
                            }  else {
                                pnGetMembershipsResult = null;
                                pnStatus = base.CreateErrorResponseFromException(new PubNubException("objDataDict null"), requestState, PNStatusCategory.PNUnknownCategory);
                            }  
                        }
                    }  else {
                        pnGetMembershipsResult = null;
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("objData null"), requestState, PNStatusCategory.PNUnknownCategory);
                    }  
                }
            } catch (Exception ex){
                pnGetMembershipsResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnGetMembershipsResult, pnStatus);

        }

    }
}