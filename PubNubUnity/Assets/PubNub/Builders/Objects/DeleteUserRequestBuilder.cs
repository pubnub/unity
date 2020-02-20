using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class DeleteUserRequestBuilder: PubNubNonSubBuilder<DeleteUserRequestBuilder, PNDeleteUserResult>, IPubNubNonSubscribeBuilder<DeleteUserRequestBuilder, PNDeleteUserResult>
    {        
        private string DeleteUserID { get; set;}
        
        public DeleteUserRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNDeleteUserOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNDeleteUserResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public DeleteUserRequestBuilder ID(string id){
            DeleteUserID = id;
            return this;
        }

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;
            requestState.httpMethod = HTTPMethod.Delete;

            Uri request = BuildRequests.BuildObjectsDeleteUserRequest(
                    DeleteUserID,
                    this.PubNubInstance,
                    this.QueryParams
                );
            request = this.PubNubInstance.TokenMgr.AppendTokenToURL( request.OriginalString, DeleteUserID, PNResourceType.PNUsers, OperationType);    
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNDeleteUserResult pnUserResult = new PNDeleteUserResult();
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