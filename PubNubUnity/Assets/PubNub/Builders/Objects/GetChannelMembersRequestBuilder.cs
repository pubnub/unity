using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetChannelMembersRequestBuilder: PubNubNonSubBuilder<GetChannelMembersRequestBuilder, PNGetChannelMembersResult>, IPubNubNonSubscribeBuilder<GetChannelMembersRequestBuilder, PNGetChannelMembersResult>
    {        
        private string GetChannelMembersChannelID { get; set;}
        private int GetChannelMembersLimit { get; set;}
        private string GetChannelMembersEnd { get; set;}
        private string GetChannelMembersStart { get; set;}
        private string GetChannelMembersFilter { get; set;}
        private bool GetChannelMembersCount { get; set;}
        private PNChannelMembersInclude[] GetChannelMembersInclude { get; set;}
        private List<string> SortBy { get; set; }
        
        public GetChannelMembersRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNGetChannelMembersOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNGetChannelMembersResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public GetChannelMembersRequestBuilder Channel(string id){
            GetChannelMembersChannelID = id;
            return this;
        }

        public GetChannelMembersRequestBuilder Include(PNChannelMembersInclude[] include){
            GetChannelMembersInclude = include;
            return this;
        }
        public GetChannelMembersRequestBuilder Limit(int limit){
            GetChannelMembersLimit = limit;
            return this;
        }

        public GetChannelMembersRequestBuilder Start(string start){
            GetChannelMembersStart = start;
            return this;
        }
        public GetChannelMembersRequestBuilder End(string end){
            GetChannelMembersEnd = end;
            return this;
        }
        public GetChannelMembersRequestBuilder Filter(string filter){
            GetChannelMembersFilter = filter;
            return this;
        }
        public GetChannelMembersRequestBuilder Sort(List<string> sortBy){
            SortBy = sortBy;
            return this;
        }
        public GetChannelMembersRequestBuilder Count(bool count){
            GetChannelMembersCount = count;
            return this;
        }
        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;

            string[] includeString = (GetChannelMembersInclude==null) ? new string[]{} : GetChannelMembersInclude.Select(a=>a.GetDescription().ToString()).ToArray();
            List<string> sortFields = SortBy ?? new List<string>();

            Uri request = BuildRequests.BuildObjectsGetChannelMembersRequest(
                    GetChannelMembersChannelID,
                    GetChannelMembersLimit,
                    GetChannelMembersStart,
                    GetChannelMembersEnd,
                    GetChannelMembersCount,
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams,
                    GetChannelMembersFilter,
                    string.Join(",", sortFields)
                );
            request = this.PubNubInstance.TokenMgr.AppendTokenToURL( request.OriginalString, GetChannelMembersChannelID, PNResourceType.PNChannelMetadata, OperationType);    
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNGetChannelMembersResult pnGetChannelMembersResult = new PNGetChannelMembersResult();
            pnGetChannelMembersResult.Data = new List<PNMembers>();
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
                                pnGetChannelMembersResult.Data.Add(pnMembers);
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
                    pnGetChannelMembersResult.Next = next;
                    pnGetChannelMembersResult.Prev = prev;
                    pnGetChannelMembersResult.TotalCount = totalCount;
 
                }
            } catch (Exception ex){
                pnGetChannelMembersResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnGetChannelMembersResult, pnStatus);

        }

    }
}