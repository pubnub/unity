using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetAllUUIDMetadataRequestBuilder: PubNubNonSubBuilder<GetAllUUIDMetadataRequestBuilder, PNGetAllUUIDMetadataResult>, IPubNubNonSubscribeBuilder<GetAllUUIDMetadataRequestBuilder, PNGetAllUUIDMetadataResult>
    {        
        private int GetUUIDMetadataLimit { get; set;}
        private string GetUUIDMetadataEnd { get; set;}
        private string GetUUIDMetadataStart { get; set;}
 
        private string GetUUIDMetadataFilter { get; set;}
        private bool GetUUIDMetadataCount { get; set;}
        private PNUUIDMetadataInclude[] GetUUIDMetadataInclude { get; set;}
        private List<string> SortBy { get; set;}        
        public GetAllUUIDMetadataRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNGetAllUUIDMetadataOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNGetAllUUIDMetadataResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public GetAllUUIDMetadataRequestBuilder Include(PNUUIDMetadataInclude[] include){
            GetUUIDMetadataInclude = include;
            return this;
        }
        public GetAllUUIDMetadataRequestBuilder Limit(int limit){
            GetUUIDMetadataLimit = limit;
            return this;
        }

        public GetAllUUIDMetadataRequestBuilder Start(string start){
            GetUUIDMetadataStart = start;
            return this;
        }
        public GetAllUUIDMetadataRequestBuilder End(string end){
            GetUUIDMetadataEnd = end;
            return this;
        }
        public GetAllUUIDMetadataRequestBuilder Filter(string filter){
            GetUUIDMetadataFilter = filter;
            return this;
        }
        public GetAllUUIDMetadataRequestBuilder Sort(List<string> sortBy){
            SortBy = sortBy;
            return this;
        }        
        public GetAllUUIDMetadataRequestBuilder Count(bool count){
            GetUUIDMetadataCount = count;
            return this;
        }
        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;

            string[] includeString = (GetUUIDMetadataInclude==null) ? new string[]{} : GetUUIDMetadataInclude.Select(a=>a.GetDescription().ToString()).ToArray();
            List<string> sortFields = SortBy ?? new List<string>();

            Uri request = BuildRequests.BuildObjectsGetAllUUIDMetadataRequest(
                    GetUUIDMetadataLimit,
                    GetUUIDMetadataStart,
                    GetUUIDMetadataEnd,
                    GetUUIDMetadataCount,
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams,
                    GetUUIDMetadataFilter,
                    string.Join(",", sortFields)
                );
            request = this.PubNubInstance.TokenMgr.AppendTokenToURL( request.OriginalString, "", PNResourceType.PNUUIDMetadata, OperationType);    
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNGetAllUUIDMetadataResult pnUserResultList = new PNGetAllUUIDMetadataResult();
            pnUserResultList.Data = new List<PNUUIDMetadataResult>();
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
                                PNUUIDMetadataResult pnUserResult = ObjectsHelpers.ExtractUUIDMetadata(objDataDict);                                
                                pnUserResultList.Data.Add(pnUserResult);
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
                    pnUserResultList.Next = next;
                    pnUserResultList.Prev = prev;
                    pnUserResultList.TotalCount = totalCount;                     
                }
            } catch (Exception ex){
                pnUserResultList = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnUserResultList, pnStatus);

        }

    }
}