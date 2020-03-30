using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetMembersRequestBuilder: PubNubNonSubBuilder<GetMembersRequestBuilder, PNMembersResult>, IPubNubNonSubscribeBuilder<GetMembersRequestBuilder, PNMembersResult>
    {        
        private string GetMembersSpaceID { get; set;}
        private int GetMembersLimit { get; set;}
        private string GetMembersEnd { get; set;}
        private string GetMembersStart { get; set;}
        private string GetMembersFilter { get; set;}
        private bool GetMembersCount { get; set;}
        private PNMembersInclude[] GetMembersInclude { get; set;}
        private List<string> SortBy { get; set; }
        
        public GetMembersRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNGetMembersOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNMembersResult, PNStatus> callback)
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
            GetMembersInclude = include;
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
        public GetMembersRequestBuilder Filter(string filter){
            GetMembersFilter = filter;
            return this;
        }
        public GetMembersRequestBuilder Sort(List<string> sortBy){
            SortBy = sortBy;
            return this;
        }
        public GetMembersRequestBuilder Count(bool count){
            GetMembersCount = count;
            return this;
        }
        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;

            string[] includeString = (GetMembersInclude==null) ? new string[]{} : GetMembersInclude.Select(a=>a.GetDescription().ToString()).ToArray();
            List<string> sortFields = SortBy ?? new List<string>();

            Uri request = BuildRequests.BuildObjectsGetMembersRequest(
                    GetMembersSpaceID,
                    GetMembersLimit,
                    GetMembersStart,
                    GetMembersEnd,
                    GetMembersCount,
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams,
                    GetMembersFilter,
                    string.Join(",", sortFields)
                );
            request = this.PubNubInstance.TokenMgr.AppendTokenToURL( request.OriginalString, GetMembersSpaceID, PNResourceType.PNSpaces, OperationType);    
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNMembersResult pnGetMembersResult = new PNMembersResult();
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
                                PNMembers pnMembers = ObjectsHelpers.ExtractMembers(objDataDict);
                                pnGetMembersResult.Data.Add(pnMembers);
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
                    pnGetMembersResult.Next = next;
                    pnGetMembersResult.Prev = prev;
                    pnGetMembersResult.TotalCount = totalCount;
 
                }
            } catch (Exception ex){
                pnGetMembersResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnGetMembersResult, pnStatus);

        }

    }
}