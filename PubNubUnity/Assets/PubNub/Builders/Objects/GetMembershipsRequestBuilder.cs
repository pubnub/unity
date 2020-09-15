using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetMembershipsRequestBuilder: PubNubNonSubBuilder<GetMembershipsRequestBuilder, PNGetMembershipsResult>, IPubNubNonSubscribeBuilder<GetMembershipsRequestBuilder, PNGetMembershipsResult>
    {    
        private string GetMembershipsUUIDMetadataID { get; set;}    
        private int GetMembershipsLimit { get; set;}
        private string GetMembershipsEnd { get; set;}
        private string GetMembershipsStart { get; set;}
        private bool GetMembershipsCount { get; set;}
        private string GetMembershipsFilter { get; set;}
        private PNMembershipsInclude[] GetMembershipsInclude { get; set;}
        private List<string> SortBy { get; set; }

        public GetMembershipsRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNGetMembershipsOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNGetMembershipsResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public GetMembershipsRequestBuilder UUID(string id){
            GetMembershipsUUIDMetadataID = id;
            return this;
        }

        public GetMembershipsRequestBuilder Include(PNMembershipsInclude[] include){
            GetMembershipsInclude = include;
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
        public GetMembershipsRequestBuilder Filter(string filter){
            GetMembershipsFilter = filter;
            return this;
        }
        public GetMembershipsRequestBuilder Count(bool count){
            GetMembershipsCount = count;
            return this;
        }
        public GetMembershipsRequestBuilder Sort(List<string> sortBy){
            SortBy = sortBy;
            return this;
        }
        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;

            string[] includeString = (GetMembershipsInclude==null) ? new string[]{} : GetMembershipsInclude.Select(a=>a.GetDescription().ToString()).ToArray();
            List<string> sortFields = SortBy ?? new List<string>();

            Uri request = BuildRequests.BuildObjectsGetMembershipsRequest(
                    GetMembershipsUUIDMetadataID,
                    GetMembershipsLimit,
                    GetMembershipsStart,
                    GetMembershipsEnd,
                    GetMembershipsCount,
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams,
                    GetMembershipsFilter,
                   string.Join(",", sortFields)
                );
            request = this.PubNubInstance.TokenMgr.AppendTokenToURL( request.OriginalString, GetMembershipsUUIDMetadataID, PNResourceType.PNUUIDMetadata, OperationType);    
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
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
                                PNMemberships pnMemberships = ObjectsHelpers.ExtractMemberships(objDataDict);
                                pnGetMembershipsResult.Data.Add(pnMemberships);
                            }  else {
                                pnStatus = base.CreateErrorResponseFromException(new PubNubException("objDataDict null"), requestState, PNStatusCategory.PNUnknownCategory);
                            }  
                        }
                    }  else {
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("objData null"), requestState, PNStatusCategory.PNUnknownCategory);
                    }  
                    int totalCount;
                    string next;
                    string prev;
                    ObjectsHelpers.ExtractPagingParamsAndTotalCount(dictionary, "totalCount", "next", "prev", out totalCount, out next, out prev);
                    pnGetMembershipsResult.Next = next;
                    pnGetMembershipsResult.Prev = prev;
                    pnGetMembershipsResult.TotalCount = totalCount;

                }

            } catch (Exception ex){
                pnGetMembershipsResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnGetMembershipsResult, pnStatus);

        }

    }
}