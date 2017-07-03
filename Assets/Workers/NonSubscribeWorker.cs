#define ENABLE_PUBNUB_LOGGING
using System;

namespace PubNubAPI
{
    public class NonSubscribeWorker<U, V>: IDisposable
    {
        private QueueManager queueManager;
        private PubNubNonSubBuilder<U, V> PNBuilder;
        public static int InstanceCount;
        private static object syncRoot = new System.Object();
        #region IDisposable implementation

        public void Dispose ()
        {
            queueManager.PubNubInstance.PNLog.WriteToLog("Disposing NonSubscribeWorker", PNLoggingMethod.LevelInfo);
            webRequest.NonSubWebRequestComplete -= WebRequestCompleteHandler;
            lock (syncRoot) {
                InstanceCount--;
            }
        }

        #endregion
        private PNUnityWebRequest webRequest;

        public NonSubscribeWorker (QueueManager queueManager)
        {
            lock (syncRoot) {
                InstanceCount++;
            }
            this.queueManager = queueManager;
            webRequest = this.queueManager.PubNubInstance.GameObjectRef.AddComponent<PNUnityWebRequest> ();
            webRequest.NonSubWebRequestComplete += WebRequestCompleteHandler;
        }
            
        

        public void Queue(PNConfiguration pnConfig, Action<V, PNStatus> callback){
            

        }

        public void RunWebRequest(string url, RequestState<V> requestState, int timeout, int pause, PubNubNonSubBuilder<U, V> pnBuilder){
            PNBuilder = pnBuilder;
            webRequest.Run<V>(url, requestState, this.queueManager.PubNubInstance.PNConfig.NonSubscribeTimeout, 0); 
        }
            
        /*public void RunTimeRequest(PNConfiguration pnConfig, Action<T, PNStatus> callback){
            RequestState<T> requestState = new RequestState<T> ();
            requestState.RespType = PNOperationType.PNTimeOperation;

            this.Callback = callback;
            
            Uri request = BuildRequests.BuildTimeRequest(
                this.queueManager.PubNubInstance.PNConfig.UUID,
                this.queueManager.PubNubInstance.PNConfig.Secure,
                this.queueManager.PubNubInstance.PNConfig.Origin,
                this.queueManager.PubNubInstance.Version
            );

            this.queueManager.PubNubInstance.PNLog.WriteToLog(string.Format("RunTimeRequest {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            webRequest.Run<T>(request.OriginalString, requestState, this.queueManager.PubNubInstance.PNConfig.NonSubscribeTimeout, 0); 
        }

        //public void RunWhereNowRequest(PNConfiguration pnConfig, Action<T, PNStatus> callback, WhereNowOperationParams operationParams){
        public void RunWhereNowRequest(PNConfiguration pnConfig, Action<T, PNStatus> callback, WhereNowBuilder operationParams){

            RequestState<T> requestState = new RequestState<T> ();
            requestState.RespType = PNOperationType.PNWhereNowOperation;

            Debug.Log ("WhereNowBuilder UuidForWhereNow: " + operationParams.UuidForWhereNow);

            //TODO verify is this uuid is passed
            string uuidForWhereNow = this.queueManager.PubNubInstance.PNConfig.UUID;
            if(!string.IsNullOrEmpty(operationParams.UuidForWhereNow)){
                uuidForWhereNow = operationParams.UuidForWhereNow;
            }
            //save callback
            this.Callback = callback;

            Uri request = BuildRequests.BuildWhereNowRequest(
                uuidForWhereNow,
                this.queueManager.PubNubInstance.PNConfig.UUID,
                this.queueManager.PubNubInstance.PNConfig.Secure,
                this.queueManager.PubNubInstance.PNConfig.Origin,
                this.queueManager.PubNubInstance.PNConfig.AuthKey,
                this.queueManager.PubNubInstance.PNConfig.SubscribeKey,
                this.queueManager.PubNubInstance.Version
            );
            this.queueManager.PubNubInstance.PNLog.WriteToLog(string.Format("RunWhereNowRequest {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            webRequest.Run<T>(request.OriginalString, requestState, this.queueManager.PubNubInstance.PNConfig.NonSubscribeTimeout, 0); 
        }*/


        private void NonSubscribeHandler (CustomEventArgs<V> cea){
            if (cea.IsTimeout || Utility.CheckRequestTimeoutMessageInError (cea)) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.queueManager.PubNubInstance.PNLog.WriteToLog (string.Format ("DateTime {0}, NonSubscribeHandler: NonSub timeout={1}", DateTime.Now.ToString (), cea.Message.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
                //ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (cea.Message.ToString (), cea.PubnubRequestState, true, false, PubnubErrorLevel);
            } else if (cea.IsError) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.queueManager.PubNubInstance.PNLog.WriteToLog (string.Format ("DateTime {0}, NonSubscribeHandler: NonSub Error={1}", DateTime.Now.ToString (), cea.Message.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
                //ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (cea.Message.ToString (), cea.PubnubRequestState, false, false, PubnubErrorLevel);
            } else {
                ProcessNonSubscribeResult (cea.PubnubRequestState, cea.Message);
                #if (ENABLE_PUBNUB_LOGGING)
                this.queueManager.PubNubInstance.PNLog.WriteToLog (string.Format ("DateTime {0}, NonSubscribeHandler: result=", DateTime.Now.ToString ()), PNLoggingMethod.LevelInfo);
                #endif

                this.queueManager.RaiseRunningRequestEnd(cea.PubnubRequestState.RespType);
                //Helpers.ProcessResponseCallbacks<T> (ref result, cea.PubnubRequestState, this.cipherKey, JsonPluggableLibrary);
            }
        }

        public void ProcessNonSubscribeResult (RequestState<V> pubnubRequestState, string jsonString)
        {
            try {
                
                string multiChannel = Helpers.GetNamesFromChannelEntities(pubnubRequestState.ChannelEntities, false); 
                string multiChannelGroup = Helpers.GetNamesFromChannelEntities(pubnubRequestState.ChannelEntities, true);
                if (!string.IsNullOrEmpty (jsonString)) {
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.queueManager.PubNubInstance.PNLog.WriteToLog (string.Format ("DateTime {0}, ProcessNonSubscribeResult: jsonString = {1} {2}", DateTime.Now.ToString (), jsonString, pubnubRequestState.RespType), PNLoggingMethod.LevelInfo);
                    #endif
                    object deSerializedResult = queueManager.PubNubInstance.JsonLibrary.DeserializeToObject (jsonString);
                    if(deSerializedResult!= null){
                        PNBuilder.RaiseCreateResponse(deSerializedResult);
                    }               
                } 
                #if (ENABLE_PUBNUB_LOGGING)
                else {
                    this.queueManager.PubNubInstance.PNLog.WriteToLog (string.Format ("DateTime {0}, ProcessNonSubscribeResult: json string null ", DateTime.Now.ToString ()), PNLoggingMethod.LevelInfo);
                }
                #endif
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.queueManager.PubNubInstance.PNLog.WriteToLog (string.Format ("DateTime {0}, ProcessNonSubscribeResult: exception: {1} ", DateTime.Now.ToString (), ex.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
                //ProcessWrapResultBasedOnResponseTypeException<T> (pubnubRequestState, errorLevel, ex);
            }
        }

        private void WebRequestCompleteHandler (object sender, EventArgs ea)
        {
            CustomEventArgs<V> cea = ea as CustomEventArgs<V>;
            try {
                if (cea != null) {
                    if (cea.PubnubRequestState != null) {
                        NonSubscribeHandler(cea);
                        //ProcessCoroutineCompleteResponse<T> (cea);
                    }
                    #if (ENABLE_PUBNUB_LOGGING)
                    else {
                        this.queueManager.PubNubInstance.PNLog.WriteToLog (string.Format ("DateTime {0}, WebRequestCompleteHandler: PubnubRequestState null", DateTime.Now.ToString ()), PNLoggingMethod.LevelInfo);
                    }
                    #endif
                }
                #if (ENABLE_PUBNUB_LOGGING)
                else {
                    this.queueManager.PubNubInstance.PNLog.WriteToLog (string.Format ("DateTime {0}, WebRequestCompleteHandler: cea null", DateTime.Now.ToString ()), PNLoggingMethod.LevelInfo);
                }
                #endif
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.queueManager.PubNubInstance.PNLog.WriteToLog(string.Format ("DateTime {0}, WebRequestCompleteHandler: Exception={1}", DateTime.Now.ToString (), ex.ToString ()), PNLoggingMethod.LevelInfo);
                #endif

                //ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (ex.Message, cea.PubnubRequestState, false, false, PubnubErrorLevel);
            }

            
        }
    }
}

