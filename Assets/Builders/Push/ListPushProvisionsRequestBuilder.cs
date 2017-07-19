using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class ListPushProvisionsRequestBuilder: PubNubNonSubBuilder<ListPushProvisionsRequestBuilder, PNPushListProvisionsResult>, IPubNubNonSubscribeBuilder<ListPushProvisionsRequestBuilder, PNPushListProvisionsResult>
    {      
        public ListPushProvisionsRequestBuilder(PubNubUnity pn):base(pn){

        }
        
        #region IPubNubBuilder implementation

        public void Async(Action<PNPushListProvisionsResult, PNStatus> callback)
        {
            this.Callback = callback;
            Debug.Log ("ListPushProvisionsRequestBuilder Async");
            base.Async(callback, PNOperationType.PNPushNotificationEnabledChannelsOperation, CurrentRequestType.NonSubscribe, this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState<PNPushListProvisionsResult> requestState = new RequestState<PNPushListProvisionsResult> ();
            requestState.RespType = PNOperationType.PNPushNotificationEnabledChannelsOperation;
            
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

