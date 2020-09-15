using System;
using System.Collections.Generic;
using System.Linq;

namespace PubNubAPI
{
    public class RemoveChannelMetadataRequestBuilder : PubNubNonSubBuilder<RemoveChannelMetadataRequestBuilder, PNRemoveChannelMetadataResult>, IPubNubNonSubscribeBuilder<RemoveChannelMetadataRequestBuilder, PNRemoveChannelMetadataResult>
    {
        private string RemoveChannelMetadataID { get; set; }
        public RemoveChannelMetadataRequestBuilder(PubNubUnity pn) : base(pn, PNOperationType.PNRemoveChannelMetadataOperation)
        {
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNRemoveChannelMetadataResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public RemoveChannelMetadataRequestBuilder Channel(string id)
        {
            RemoveChannelMetadataID = id;
            return this;
        }

        protected override void RunWebRequest(QueueManager qm)
        {
            RequestState requestState = new RequestState();
            requestState.OperationType = OperationType;
            requestState.httpMethod = HTTPMethod.Delete;

            Uri request = BuildRequests.BuildObjectsDeleteChannelMetadataRequest(
                    RemoveChannelMetadataID,
                    this.PubNubInstance,
                    this.QueryParams
                );
            request = this.PubNubInstance.TokenMgr.AppendTokenToURL( request.OriginalString, RemoveChannelMetadataID, PNResourceType.PNChannelMetadata, OperationType);
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this);
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState)
        {
            PNRemoveChannelMetadataResult pnSpaceResult = new PNRemoveChannelMetadataResult();
            PNStatus pnStatus = new PNStatus();

            try
            {
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;

                if (dictionary == null)
                {
                    pnSpaceResult = null;
                    pnStatus = base.CreateErrorResponseFromException(new PubNubException("Data not present"), requestState, PNStatusCategory.PNUnknownCategory);
                }
            }
            catch (Exception ex)
            {
                pnSpaceResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnSpaceResult, pnStatus);

        }

    }

}