using System;
using System.Collections.Generic;
using System.Linq;

namespace PubNubAPI
{
    public class GetAllChannelMetadataRequestBuilder : PubNubNonSubBuilder<GetAllChannelMetadataRequestBuilder, PNGetAllChannelMetadataResult>, IPubNubNonSubscribeBuilder<GetAllChannelMetadataRequestBuilder, PNGetAllChannelMetadataResult>
    {
        private int GetAllChannelMetadataLimit { get; set; }
        private string GetAllChannelMetadataEnd { get; set; }
        private string GetAllChannelMetadataStart { get; set; }
        private string GetAllChannelMetadataFilter { get; set; }        
        private bool GetAllChannelMetadataCount { get; set; }
        private PNChannelMetadataInclude[] GetAllChannelMetadataInclude { get; set; }
        private List<string> SortBy { get; set; }
        public GetAllChannelMetadataRequestBuilder(PubNubUnity pn) : base(pn, PNOperationType.PNGetAllChannelMetadataOperation)
        {
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNGetAllChannelMetadataResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public GetAllChannelMetadataRequestBuilder Include(PNChannelMetadataInclude[] include)
        {
            GetAllChannelMetadataInclude = include;
            return this;
        }
        public GetAllChannelMetadataRequestBuilder Limit(int limit)
        {
            GetAllChannelMetadataLimit = limit;
            return this;
        }

        public GetAllChannelMetadataRequestBuilder Start(string start)
        {
            GetAllChannelMetadataStart = start;
            return this;
        }
        public GetAllChannelMetadataRequestBuilder End(string end)
        {
            GetAllChannelMetadataEnd = end;
            return this;
        }
        public GetAllChannelMetadataRequestBuilder Filter(string filter){
            GetAllChannelMetadataFilter = filter;
            return this;
        }
        public GetAllChannelMetadataRequestBuilder Sort(List<string> sortBy){
            SortBy = sortBy;
            return this;
        }        
        public GetAllChannelMetadataRequestBuilder Count(bool count)
        {
            GetAllChannelMetadataCount = count;
            return this;
        }
        protected override void RunWebRequest(QueueManager qm)
        {
            RequestState requestState = new RequestState();
            requestState.OperationType = OperationType;

            string[] includeString = (GetAllChannelMetadataInclude==null) ? new string[]{} : GetAllChannelMetadataInclude.Select(a=>a.GetDescription().ToString()).ToArray();
            List<string> sortFields = SortBy ?? new List<string>();

            Uri request = BuildRequests.BuildObjectsGetAllChannelMetadataRequest(
                    GetAllChannelMetadataLimit,
                    GetAllChannelMetadataStart,
                    GetAllChannelMetadataEnd,
                    GetAllChannelMetadataCount,
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams,
                    GetAllChannelMetadataFilter,
                    string.Join(",", sortFields)
                );
            request = this.PubNubInstance.TokenMgr.AppendTokenToURL( request.OriginalString, "", PNResourceType.PNChannelMetadata, OperationType);
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this);
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState)
        {
            PNGetAllChannelMetadataResult pnSpaceResultList = new PNGetAllChannelMetadataResult();
            pnSpaceResultList.Data = new List<PNChannelMetadataResult>();
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
                                PNChannelMetadataResult pnSpaceResult = ObjectsHelpers.ExtractChannelMetadata(objDataDict);
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