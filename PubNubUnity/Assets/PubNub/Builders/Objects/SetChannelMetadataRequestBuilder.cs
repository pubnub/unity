using System;
using System.Collections.Generic;
using System.Linq;

namespace PubNubAPI
{
    public class SetChannelMetadataRequestBuilder : PubNubNonSubBuilder<SetChannelMetadataRequestBuilder, PNChannelMetadataResult>, IPubNubNonSubscribeBuilder<SetChannelMetadataRequestBuilder, PNChannelMetadataResult>
    {
        private PNChannelMetadataInclude[] ChannelMetadataInclude { get; set; }
        private string ChannelToUse { get; set; }
        private string ChannelMetadataName { get; set; }
        private string ChannelMetadataDescription { get; set; }
        private Dictionary<string, object> ChannelMetadataCustom { get; set; }

        public SetChannelMetadataRequestBuilder(PubNubUnity pn) : base(pn, PNOperationType.PNSetChannelMetadataOperation)
        {
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNChannelMetadataResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public SetChannelMetadataRequestBuilder Include(PNChannelMetadataInclude[] include)
        {
            ChannelMetadataInclude = include;
            return this;
        }

        public SetChannelMetadataRequestBuilder Channel(string id)
        {
            ChannelToUse = id;
            return this;
        }

        public SetChannelMetadataRequestBuilder Name(string name)
        {
            ChannelMetadataName = name;
            return this;
        }

        public SetChannelMetadataRequestBuilder Description(string description)
        {
            ChannelMetadataDescription = description;
            return this;
        }

        public SetChannelMetadataRequestBuilder Custom(Dictionary<string, object> custom)
        {
            ChannelMetadataCustom = custom;
            return this;
        }

        protected override void RunWebRequest(QueueManager qm)
        {
            RequestState requestState = new RequestState();
            requestState.OperationType = OperationType;
            requestState.httpMethod = HTTPMethod.Patch;

            var cub = new
            {
                id = ChannelToUse,
                name = ChannelMetadataName,
                description = ChannelMetadataDescription,
                custom = ChannelMetadataCustom,
            };

            string jsonUserBody = Helpers.JsonEncodePublishMsg(cub, "", this.PubNubInstance.JsonLibrary, this.PubNubInstance.PNLog);
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog(string.Format("jsonUserBody: {0}", jsonUserBody), PNLoggingMethod.LevelInfo);
            #endif
            requestState.POSTData = jsonUserBody;

            string[] includeString = (ChannelMetadataInclude==null) ? new string[]{} : ChannelMetadataInclude.Select(a=>a.GetDescription().ToString()).ToArray();

            Uri request = BuildRequests.BuildObjectsSetChannelMetadataRequest(
                    ChannelToUse,
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams
                );
            request = this.PubNubInstance.TokenMgr.AppendTokenToURL( request.OriginalString, ChannelToUse, PNResourceType.PNChannelMetadata, OperationType);    
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
                    dictionary.TryGetValue("data", out objData);
                    if (objData != null)
                    {
                        Dictionary<string, object> objDataDict = objData as Dictionary<string, object>;
                        if (objDataDict != null)
                        {
                            pnChannelMetadataResult = ObjectsHelpers.ExtractChannelMetadata(objDataDict);
                        }
                        else
                        {
                            pnChannelMetadataResult = null;
                            pnStatus = base.CreateErrorResponseFromException(new PubNubException("objDataDict null"), requestState, PNStatusCategory.PNUnknownCategory);
                        }
                    }
                    else
                    {
                        pnChannelMetadataResult = null;
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("objData null"), requestState, PNStatusCategory.PNUnknownCategory);
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
