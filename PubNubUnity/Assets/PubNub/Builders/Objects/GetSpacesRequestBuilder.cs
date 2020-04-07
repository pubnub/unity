using System;
using System.Collections.Generic;
using System.Linq;

namespace PubNubAPI
{
    public class GetSpacesRequestBuilder : PubNubNonSubBuilder<GetSpacesRequestBuilder, PNGetSpacesResult>, IPubNubNonSubscribeBuilder<GetSpacesRequestBuilder, PNGetSpacesResult>
    {
        private int GetSpacesLimit { get; set; }
        private string GetSpacesEnd { get; set; }
        private string GetSpacesStart { get; set; }
        private string GetSpacesFilter { get; set; }        
        private bool GetSpacesCount { get; set; }
        private PNUserSpaceInclude[] GetSpacesInclude { get; set; }
        private List<string> SortBy { get; set; }
        public GetSpacesRequestBuilder(PubNubUnity pn) : base(pn, PNOperationType.PNGetSpacesOperation)
        {
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNGetSpacesResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public GetSpacesRequestBuilder Include(PNUserSpaceInclude[] include)
        {
            GetSpacesInclude = include;
            return this;
        }
        public GetSpacesRequestBuilder Limit(int limit)
        {
            GetSpacesLimit = limit;
            return this;
        }

        public GetSpacesRequestBuilder Start(string start)
        {
            GetSpacesStart = start;
            return this;
        }
        public GetSpacesRequestBuilder End(string end)
        {
            GetSpacesEnd = end;
            return this;
        }
        public GetSpacesRequestBuilder Filter(string filter){
            GetSpacesFilter = filter;
            return this;
        }
        public GetSpacesRequestBuilder Sort(List<string> sortBy){
            SortBy = sortBy;
            return this;
        }        
        public GetSpacesRequestBuilder Count(bool count)
        {
            GetSpacesCount = count;
            return this;
        }
        protected override void RunWebRequest(QueueManager qm)
        {
            RequestState requestState = new RequestState();
            requestState.OperationType = OperationType;

            string[] includeString = (GetSpacesInclude==null) ? new string[]{} : GetSpacesInclude.Select(a=>a.GetDescription().ToString()).ToArray();
            List<string> sortFields = SortBy ?? new List<string>();

            Uri request = BuildRequests.BuildObjectsGetSpacesRequest(
                    GetSpacesLimit,
                    GetSpacesStart,
                    GetSpacesEnd,
                    GetSpacesCount,
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams,
                    GetSpacesFilter,
                    string.Join(",", sortFields)
                );
            request = this.PubNubInstance.TokenMgr.AppendTokenToURL( request.OriginalString, "", PNResourceType.PNSpaces, OperationType);
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this);
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState)
        {
            PNGetSpacesResult pnSpaceResultList = new PNGetSpacesResult();
            pnSpaceResultList.Data = new List<PNSpaceResult>();
            PNStatus pnStatus = new PNStatus();

            try
            {
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;

                if (dictionary != null)
                {
                    object objData;
                    dictionary.TryGetValue("data", out objData);
                    if (objData != null)
                    {
                        object[] objArr = objData as object[];
                        foreach (object data in objArr)
                        {
                            Dictionary<string, object> objDataDict = data as Dictionary<string, object>;
                            if (objDataDict != null)
                            {
                                PNSpaceResult pnSpaceResult = ObjectsHelpers.ExtractSpace(objDataDict);
                                pnSpaceResultList.Data.Add(pnSpaceResult);
                            }
                            else
                            {
                                pnStatus = base.CreateErrorResponseFromException(new PubNubException("objDataDict null"), requestState, PNStatusCategory.PNUnknownCategory);
                            }
                        }
                    }
                    else
                    {
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("objData null"), requestState, PNStatusCategory.PNUnknownCategory);
                    }
                    int totalCount;
                    string next;
                    string prev;
                    ObjectsHelpers.ExtractPagingParamsAndTotalCount(dictionary, "totalCount", "next", "prev", out totalCount, out next, out prev);
                    pnSpaceResultList.Next = next;
                    pnSpaceResultList.Prev = prev;
                    pnSpaceResultList.TotalCount = totalCount;                    
                }
            }
            catch (Exception ex)
            {
                pnSpaceResultList = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnSpaceResultList, pnStatus);

        }

    }

}