using System;
using System.Collections.Generic;
using System.Linq;

namespace PubNubAPI
{
    public class DeleteSpaceRequestBuilder : PubNubNonSubBuilder<DeleteSpaceRequestBuilder, PNDeleteSpaceResult>, IPubNubNonSubscribeBuilder<DeleteSpaceRequestBuilder, PNDeleteSpaceResult>
    {
        private string DeleteSpaceID { get; set; }
        public DeleteSpaceRequestBuilder(PubNubUnity pn) : base(pn, PNOperationType.PNDeleteSpaceOperation)
        {
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNDeleteSpaceResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public DeleteSpaceRequestBuilder ID(string id)
        {
            DeleteSpaceID = id;
            return this;
        }

        protected override void RunWebRequest(QueueManager qm)
        {
            RequestState requestState = new RequestState();
            requestState.OperationType = OperationType;
            requestState.httpMethod = HTTPMethod.Delete;

            Uri request = BuildRequests.BuildObjectsDeleteSpaceRequest(
                    DeleteSpaceID,
                    this.PubNubInstance,
                    this.QueryParams
                );
            request = this.PubNubInstance.TokenMgr.AppendTokenToURL( request.OriginalString, DeleteSpaceID, PNResourceType.PNSpaces, OperationType);
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this);
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState)
        {
            PNDeleteSpaceResult pnSpaceResult = new PNDeleteSpaceResult();
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