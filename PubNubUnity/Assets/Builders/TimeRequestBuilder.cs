using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class TimeRequestBuilder: PubNubNonSubBuilder<TimeRequestBuilder, PNTimeResult>, IPubNubNonSubscribeBuilder<TimeRequestBuilder, PNTimeResult>
    {      
        public TimeRequestBuilder(PubNubUnity pn):base(pn, PNOperationType.PNTimeOperation){
        }
        
        #region IPubNubBuilder implementation

        public void Async(Action<PNTimeResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;
            
            Uri request = BuildRequests.BuildTimeRequest(
                this.PubNubInstance
            );

            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog(string.Format("RunTimeRequest {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            #endif
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this);
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            Int64[] c = deSerializedResult as Int64[];
            PNTimeResult pnTimeResult = new PNTimeResult();
            PNStatus pnStatus = new PNStatus();
            if ((c != null) && (c.Length > 0)) {
                pnTimeResult.TimeToken = c [0];
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog(string.Format("CreatePubNubResponse (c == null) || (c.Length < 0) {0}", deSerializedResult.ToString()), PNLoggingMethod.LevelInfo);
                #endif
                pnStatus.Error = true;
                pnStatus = base.CreateErrorResponseFromMessage("Response is null", requestState, PNStatusCategory.PNMalformedResponseCategory);
            }
            Callback(pnTimeResult, pnStatus);
        }

    }
}

