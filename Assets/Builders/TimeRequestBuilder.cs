using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class TimeRequestBuilder: PubNubNonSubBuilder<TimeRequestBuilder, PNTimeResult>, IPubNubNonSubscribeBuilder<TimeRequestBuilder, PNTimeResult>
    {      
        //protected Action<PNTimeResult, PNStatus> Callback;  
        public TimeRequestBuilder(PubNubUnity pn):base(pn){

        }
        
        #region IPubNubBuilder implementation

        public void Async(Action<PNTimeResult, PNStatus> callback)
        {
            this.Callback = callback;
            Debug.Log ("TimeBuilder Async");
            base.Async(callback, PNOperationType.PNTimeOperation, PNCurrentRequestType.NonSubscribe, this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState<PNTimeResult> requestState = new RequestState<PNTimeResult> ();
            requestState.RespType = PNOperationType.PNTimeOperation;
            
            Uri request = BuildRequests.BuildTimeRequest(
                this.PubNubInstance.PNConfig.UUID,
                this.PubNubInstance.PNConfig.Secure,
                this.PubNubInstance.PNConfig.Origin,
                this.PubNubInstance.Version
            );

            this.PubNubInstance.PNLog.WriteToLog(string.Format("RunTimeRequest {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this);
        }

        protected override void CreatePubNubResponse(object deSerializedResult){
            Int64[] c = deSerializedResult as Int64[];
            PNTimeResult pnTimeResult = new PNTimeResult();
            PNStatus pnStatus = new PNStatus();
            if ((c != null) && (c.Length > 0)) {
                
                pnTimeResult.TimeToken = c [0];
                pnStatus.Error = false;
            } else {
                pnStatus.Error = true;
            }
            Callback(pnTimeResult, pnStatus);
        }

        // protected override void CreateErrorResponse(Exception exception, bool showInCallback, bool level){
            
        // }
        
    }
}

