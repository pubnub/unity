using System;
using System.Collections.Generic;
using System.Linq;

namespace PubNubAPI
{    public class GetSpaceRequestBuilder : PubNubNonSubBuilder<GetSpaceRequestBuilder, PNSpaceResult>, IPubNubNonSubscribeBuilder<GetSpaceRequestBuilder, PNSpaceResult>
    {
        private string GetSpaceID { get; set; }
        private PNUserSpaceInclude[] GetSpaceInclude { get; set; }
        public GetSpaceRequestBuilder(PubNubUnity pn) : base(pn, PNOperationType.PNGetSpaceOperation)
        {
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNSpaceResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public GetSpaceRequestBuilder Include(PNUserSpaceInclude[] include)
        {
            GetSpaceInclude = include;
            return this;
        }

        public GetSpaceRequestBuilder ID(string id)
        {
            GetSpaceID = id;
            return this;
        }

        protected override void RunWebRequest(QueueManager qm)
        {
            RequestState requestState = new RequestState();
            requestState.OperationType = OperationType;

            string[] includeString = (GetSpaceInclude==null) ? new string[]{} : GetSpaceInclude.Select(a=>a.GetDescription().ToString()).ToArray();

            Uri request = BuildRequests.BuildObjectsGetSpaceRequest(
                    GetSpaceID,
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams
                );
            request = this.PubNubInstance.TokenMgr.AppendTokenToURL( request.OriginalString, GetSpaceID, PNResourceType.PNSpaces, OperationType);    
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this);
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState)
        {
            PNSpaceResult pnSpaceResult = new PNSpaceResult();
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
                            pnSpaceResult = ObjectsHelpers.ExtractSpace(objDataDict);
                        }
                        else
                        {
                            pnSpaceResult = null;
                            pnStatus = base.CreateErrorResponseFromException(new PubNubException("objDataDict not present"), requestState, PNStatusCategory.PNUnknownCategory);
                        }
                    }
                    else
                    {
                        pnSpaceResult = null;
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("Data not present"), requestState, PNStatusCategory.PNUnknownCategory);
                    }
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