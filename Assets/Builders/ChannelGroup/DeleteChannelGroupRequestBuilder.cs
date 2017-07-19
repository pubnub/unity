using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class DeleteChannelGroupRequestBuilder: PubNubNonSubBuilder<DeleteChannelGroupRequestBuilder, PNChannelGroupsDeleteGroupResult>, IPubNubNonSubscribeBuilder<DeleteChannelGroupRequestBuilder, PNChannelGroupsDeleteGroupResult>
    {      
        //protected Action<PNTimeResult, PNStatus> Callback;  
        public DeleteChannelGroupRequestBuilder(PubNubUnity pn):base(pn){

        }
        
        #region IPubNubBuilder implementation

        public void Async(Action<PNChannelGroupsDeleteGroupResult, PNStatus> callback)
        {
            this.Callback = callback;
            Debug.Log ("DeleteChannelGroupRequestBuilder Async");
            base.Async(callback, PNOperationType.PNRemoveGroupOperation, CurrentRequestType.NonSubscribe, this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState<PNChannelGroupsDeleteGroupResult> requestState = new RequestState<PNChannelGroupsDeleteGroupResult> ();
            requestState.RespType = PNOperationType.PNRemoveGroupOperation;
            
            /*Uri request = BuildRequests.BuildTimeRequest(
                this.PubNubInstance.PNConfig.UUID,
                this.PubNubInstance.PNConfig.Secure,
                this.PubNubInstance.PNConfig.Origin,
                this.PubNubInstance.Version
            );

            this.PubNubInstance.PNLog.WriteToLog(string.Format("RunTimeRequest {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this);*/
        }

        protected override void CreatePubNubResponse(object deSerializedResult){
            /*Int64[] c = deSerializedResult as Int64[];
            PNTimeResult pnTimeResult = new PNTimeResult();
            PNStatus pnStatus = new PNStatus();
            if ((c != null) && (c.Length > 0)) {
                
                pnTimeResult.TimeToken = c [0];
                pnStatus.Error = false;
            } else {
                pnStatus.Error = true;
            }
            Callback(pnTimeResult, pnStatus);*/
        }
        
    }
}

