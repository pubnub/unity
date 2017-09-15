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
            RequestState requestState = new RequestState ();
            requestState.RespType = PNOperationType.PNTimeOperation;
            
            Uri request = BuildRequests.BuildTimeRequest(
                this.PubNubInstance.PNConfig.UUID,
                this.PubNubInstance.PNConfig.Secure,
                this.PubNubInstance.PNConfig.Origin,
                this.PubNubInstance.Version
            );

            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog(string.Format("RunTimeRequest {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            #endif
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this);
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            Int64[] c = deSerializedResult as Int64[];
            PNTimeResult pnTimeResult = new PNTimeResult();
            
            if ((c != null) && (c.Length > 0)) {
                pnTimeResult.TimeToken = c [0];

                Callback(pnTimeResult, new PNStatus());
                
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog(string.Format("CreatePubNubResponse (c == null) || (c.Length < 0) {0}", deSerializedResult.ToString()), PNLoggingMethod.LevelInfo);
                #endif
                CreateErrorResponse(PNStatusCategory.PNMalformedResponseCategory, null, true, PNLoggingMethod.LevelError, null);
            }
            
        }

        // protected override void CreateErrorResponse(Exception exception, bool showInCallback, bool level){
            
        // }
        
    }
}

