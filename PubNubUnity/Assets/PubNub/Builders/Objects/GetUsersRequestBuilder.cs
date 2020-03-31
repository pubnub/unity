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
        private string GetUsersFilter { get; set;}
        private bool GetUsersCount { get; set;}
        private PNUserSpaceInclude[] GetUsersInclude { get; set;}
        private List<string> SortBy { get; set;}        
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
            GetUsersInclude = include;
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
        public GetUsersRequestBuilder Filter(string filter){
            GetUsersFilter = filter;
            return this;
        }
        public GetUsersRequestBuilder Sort(List<string> sortBy){
            SortBy = sortBy;
            return this;
        }        
        public GetUsersRequestBuilder Count(bool count){
            GetUsersCount = count;
            return this;
        }
        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;

            string[] includeString = (GetUsersInclude==null) ? new string[]{} : GetUsersInclude.Select(a=>a.GetDescription().ToString()).ToArray();
            List<string> sortFields = SortBy ?? new List<string>();

            Uri request = BuildRequests.BuildObjectsGetUsersRequest(
                    GetUsersLimit,
                    GetUsersStart,
                    GetUsersEnd,
                    GetUsersCount,
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams,
                    GetUsersFilter,
                    string.Join(",", sortFields)
                );
            request = this.PubNubInstance.TokenMgr.AppendTokenToURL( request.OriginalString, "", PNResourceType.PNUsers, OperationType);    
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
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
                                PNUserResult pnUserResult = ObjectsHelpers.ExtractUser(objDataDict);                                
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