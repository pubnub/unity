using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class RemoveUUIDMetadataRequestBuilder: PubNubNonSubBuilder<RemoveUUIDMetadataRequestBuilder, PNRemoveUUIDMetadataResult>, IPubNubNonSubscribeBuilder<RemoveUUIDMetadataRequestBuilder, PNRemoveUUIDMetadataResult>
    {        
        private string DeleteUUID { get; set;}
        
        public RemoveUUIDMetadataRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNRemoveUUIDMetadataOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNRemoveUUIDMetadataResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public RemoveUUIDMetadataRequestBuilder UUID(string id){
            DeleteUUID = id;
            return this;
        }

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;
            requestState.httpMethod = HTTPMethod.Delete;

            Uri request = BuildRequests.BuildObjectsDeleteUUIDMetadataRequest(
                    DeleteUUID,
                    this.PubNubInstance,
                    this.QueryParams
                );
            request = this.PubNubInstance.TokenMgr.AppendTokenToURL( request.OriginalString, DeleteUUID, PNResourceType.PNUUIDMetadata, OperationType);    
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNRemoveUUIDMetadataResult pnUserResult = new PNRemoveUUIDMetadataResult();
            PNStatus pnStatus = new PNStatus();

            try{
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
                
                if(dictionary == null) {
                    pnUserResult = null;
                    pnStatus = base.CreateErrorResponseFromException(new PubNubException("Data not present"), requestState, PNStatusCategory.PNUnknownCategory);
                }
            } catch (Exception ex){
                pnUserResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnUserResult, pnStatus);

        }

    }
}