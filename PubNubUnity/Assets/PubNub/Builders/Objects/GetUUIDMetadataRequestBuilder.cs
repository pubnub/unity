using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetUUIDMetadataRequestBuilder: PubNubNonSubBuilder<GetUUIDMetadataRequestBuilder, PNUUIDMetadataResult>, IPubNubNonSubscribeBuilder<GetUUIDMetadataRequestBuilder, PNUUIDMetadataResult>
    {        
        private string GetUUIDMetadataID { get; set;}
        private PNUUIDMetadataInclude[] GetUUIDMetadataInclude { get; set;}
        
        public GetUUIDMetadataRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNGetUUIDMetadataOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNUUIDMetadataResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public GetUUIDMetadataRequestBuilder Include(PNUUIDMetadataInclude[] include){
            GetUUIDMetadataInclude = include;
            return this;
        }

        public GetUUIDMetadataRequestBuilder UUID(string id){
            GetUUIDMetadataID = id;
            return this;
        }

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;

            string[] includeString = (GetUUIDMetadataInclude==null) ? new string[]{} : GetUUIDMetadataInclude.Select(a=>a.GetDescription().ToString()).ToArray();

            Uri request = BuildRequests.BuildObjectsGetUUIDMetadataRequest(
                    GetUUIDMetadataID,
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams
                );
            request = this.PubNubInstance.TokenMgr.AppendTokenToURL( request.OriginalString, GetUUIDMetadataID, PNResourceType.PNUUIDMetadata, OperationType);    
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNUUIDMetadataResult pnUUIDMetadataResult = new PNUUIDMetadataResult();
            PNStatus pnStatus = new PNStatus();

            try{
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
                
                if(dictionary != null) {
                    object objData;
                    if(dictionary.TryGetValue("data", out objData)){
                        Dictionary<string, object> objDataDict = objData as Dictionary<string, object>;
                        if(objDataDict!=null){
                            pnUUIDMetadataResult = ObjectsHelpers.ExtractUUIDMetadata(objDataDict);
                        } else {
                            pnStatus = base.CreateErrorResponseFromException(new PubNubException("objDataDict not present"), requestState, PNStatusCategory.PNUnknownCategory);
                        }
                    } else {
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("Data not present"), requestState, PNStatusCategory.PNUnknownCategory);
                    }
                }
            } catch (Exception ex){
                pnUUIDMetadataResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnUUIDMetadataResult, pnStatus);

        }

    }
}