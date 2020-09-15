using System;
using System.Collections.Generic;
using System.Linq;

namespace PubNubAPI
{    public class GetChannelMetadataRequestBuilder : PubNubNonSubBuilder<GetChannelMetadataRequestBuilder, PNChannelMetadataResult>, IPubNubNonSubscribeBuilder<GetChannelMetadataRequestBuilder, PNChannelMetadataResult>
    {
        private string GetChannelMetadataID { get; set; }
        private PNChannelMetadataInclude[] GetChannelMetadataInclude { get; set; }
        public GetChannelMetadataRequestBuilder(PubNubUnity pn) : base(pn, PNOperationType.PNGetChannelMetadataOperation)
        {
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNChannelMetadataResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public GetChannelMetadataRequestBuilder Include(PNChannelMetadataInclude[] include)
        {
            GetChannelMetadataInclude = include;
            return this;
        }

        public GetChannelMetadataRequestBuilder Channel(string id)
        {
            GetChannelMetadataID = id;
            return this;
        }

        protected override void RunWebRequest(QueueManager qm)
        {
            RequestState requestState = new RequestState();
            requestState.OperationType = OperationType;

            string[] includeString = (GetChannelMetadataInclude==null) ? new string[]{} : GetChannelMetadataInclude.Select(a=>a.GetDescription().ToString()).ToArray();

            Uri request = BuildRequests.BuildObjectsGetChannelMetadataRequest(
                    GetChannelMetadataID,
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams
                );
            request = this.PubNubInstance.TokenMgr.AppendTokenToURL( request.OriginalString, GetChannelMetadataID, PNResourceType.PNChannelMetadata, OperationType);    
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this);
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState)
        {
            PNChannelMetadataResult pnChannelMetadataResult = new PNChannelMetadataResult();
            PNStatus pnStatus = new PNStatus();

            try
            {
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;

                if (dictionary != null)
                {
                    object objData;
                    if (dictionary.TryGetValue("data", out objData))
                    {
                        Dictionary<string, object> objDataDict = objData as Dictionary<string, object>;
                        if (objDataDict != null)
                        {
                            pnChannelMetadataResult = ObjectsHelpers.ExtractChannelMetadata(objDataDict);
                        }
                        else
                        {
                            pnChannelMetadataResult = null;
                            pnStatus = base.CreateErrorResponseFromException(new PubNubException("objDataDict not present"), requestState, PNStatusCategory.PNUnknownCategory);
                        }
                    }
                    else
                    {
                        pnChannelMetadataResult = null;
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("Data not present"), requestState, PNStatusCategory.PNUnknownCategory);
                    }
                }
            }
            catch (Exception ex)
            {
                pnChannelMetadataResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnChannelMetadataResult, pnStatus);

        }

    }

}