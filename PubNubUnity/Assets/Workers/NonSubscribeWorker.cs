using System;
using UnityEngine.Networking;

namespace PubNubAPI
{
    public class NonSubscribeWorker<U, V>: IDisposable where V : class
    {
        private readonly QueueManager queueManager;
        private PubNubNonSubBuilder<U, V> PNBuilder;
        private static int InstanceCount;
        private string webRequestId = "";

        private static object syncRoot = new System.Object();
        #region IDisposable implementation

        public void Dispose ()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            queueManager.PubNubInstance.PNLog.WriteToLog("Disposing NonSubscribeWorker", PNLoggingMethod.LevelInfo);
            #endif
            webRequest.WebRequestComplete -= WebRequestCompleteHandler;
            webRequest.AbortRequest(webRequestId, false);
            lock (syncRoot) {
                InstanceCount--;
            }
        }

        #endregion
        private readonly PNUnityWebRequest webRequest;

        public NonSubscribeWorker (QueueManager queueManager)
        {
            lock (syncRoot) {
                InstanceCount++;
            }
            this.queueManager = queueManager;
            webRequest = this.queueManager.PubNubInstance.GameObjectRef.AddComponent<PNUnityWebRequest> ();
            webRequest.WebRequestComplete += WebRequestCompleteHandler;
            this.webRequest.PNLog = this.queueManager.PubNubInstance.PNLog;
        }

        public void RunWebRequest(RequestState requestState, PubNubNonSubBuilder<U, V> pnBuilder){
            try{
                PNBuilder = pnBuilder;

                webRequestId = webRequest.Run(requestState); 
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.queueManager.PubNubInstance.PNLog.WriteToLog (string.Format ("ex.ToString() {0}", ex.ToString()), PNLoggingMethod.LevelInfo);
                #endif
            }
        }
            
        public void ProcessNonSubscribeResult (RequestState pubnubRequestState, string jsonString)
        {
            if (!string.IsNullOrEmpty (jsonString)) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.queueManager.PubNubInstance.PNLog.WriteToLog (string.Format ("ProcessNonSubscribeResult: jsonString = {0} {1}", jsonString, pubnubRequestState.OperationType), PNLoggingMethod.LevelInfo);
                #endif
                object deSerializedResult = queueManager.PubNubInstance.JsonLibrary.DeserializeToObject (jsonString);
                if(deSerializedResult!= null){
                    PNBuilder.RaiseCreateResponse(deSerializedResult, pubnubRequestState);
                }  
                this.queueManager.PubNubInstance.Latency.StoreLatency(pubnubRequestState.StartRequestTicks, pubnubRequestState.EndRequestTicks, pubnubRequestState.OperationType);             
            } 
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                this.queueManager.PubNubInstance.PNLog.WriteToLog ("ProcessNonSubscribeResult: json string null ", PNLoggingMethod.LevelInfo);
            }
            #endif
        }

        private void WebRequestCompleteHandler (object sender, EventArgs ea)
        { 
            CustomEventArgs cea = ea as CustomEventArgs;
            this.queueManager.RaiseRunningRequestEnd(cea.PubNubRequestState.OperationType);
            try {
               
                if ((cea != null) && (cea.CurrRequestType.Equals(PNCurrentRequestType.NonSubscribe))) {
                    PNStatus pnStatus;
                    if(Helpers.TryCheckErrorTypeAndCallback<V>(cea, this.queueManager.PubNubInstance, out pnStatus)){
                        #if (ENABLE_PUBNUB_LOGGING)
                        this.queueManager.PubNubInstance.PNLog.WriteToLog("WebRequestCompleteHandler: Is Error true ", PNLoggingMethod.LevelInfo);
                        #endif
                        PNBuilder.RaiseError(pnStatus);
                    } else {
                        ProcessNonSubscribeResult (cea.PubNubRequestState, cea.Message);
                        #if (ENABLE_PUBNUB_LOGGING)
                        this.queueManager.PubNubInstance.PNLog.WriteToLog ("NonSubscribeHandler: result", PNLoggingMethod.LevelInfo);
                        #endif
                    }

                }
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.queueManager.PubNubInstance.PNLog.WriteToLog(string.Format ("WebRequestCompleteHandler: Exception={0}", ex.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
                PNBuilder.RaiseError(PNStatusCategory.PNUnknownCategory, ex, false, cea.PubNubRequestState);
            }

        }
    }
}

