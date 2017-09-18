#define ENABLE_PUBNUB_LOGGING
using System;
using UnityEngine.Networking;

namespace PubNubAPI
{
    public class NonSubscribeWorker<U, V>: IDisposable where V : class
    {
        private QueueManager queueManager;
        private PubNubNonSubBuilder<U, V> PNBuilder;
        public static int InstanceCount;
        private long ResponseCode = 0;
        private string URL = "";
        private string webRequestId = "";

        private static object syncRoot = new System.Object();
        #region IDisposable implementation

        public void Dispose ()
        {
            queueManager.PubNubInstance.PNLog.WriteToLog("Disposing NonSubscribeWorker", PNLoggingMethod.LevelInfo);
            webRequest.WebRequestComplete -= WebRequestCompleteHandler;
            webRequest.AbortRequest(webRequestId);
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
            ResponseCode = 0;
            URL = "";
            this.queueManager = queueManager;
            webRequest = this.queueManager.PubNubInstance.GameObjectRef.AddComponent<PNUnityWebRequest> ();
            webRequest.WebRequestComplete += WebRequestCompleteHandler;
            this.webRequest.PNLog = this.queueManager.PubNubInstance.PNLog;
        }
            
        

        public void Queue(PNConfiguration pnConfig, Action<V, PNStatus> callback){
            

        }

        //public void RunWebRequest(string url, RequestState requestState, int timeout, int pause, PubNubNonSubBuilder<U, V> pnBuilder){
        public void RunWebRequest(RequestState requestState, PubNubNonSubBuilder<U, V> pnBuilder){
            PNBuilder = pnBuilder;

            //webRequest.Run<V>(url, requestState, this.queueManager.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, false); 
            webRequestId = webRequest.Run(requestState); 
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


        /*private void NonSubscribeHandler (CustomEventArgs<V> cea){
            if (cea.IsTimeout || Helpers.CheckRequestTimeoutMessageInError (cea.IsError, cea.Message)) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.queueManager.PubNubInstance.PNLog.WriteToLog (string.Format ("NonSubscribeHandler: NonSub timeout={0}", cea.Message.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
                PNBuilder.RaiseError(PNStatusCategory.PNTimeoutCategory, null, true, PNLoggingMethod.LevelInfo, cea.PubNubRequestState);
                //ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (cea.Message.ToString (), cea.PubnubRequestState, true, false, PubnubErrorLevel);
            } else if (cea.IsError) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.queueManager.PubNubInstance.PNLog.WriteToLog (string.Format ("NonSubscribeHandler: NonSub Error={0}", cea.Message.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
                PNBuilder.RaiseError(PNStatusCategory.PNUnknownCategory, null, true, PNLoggingMethod.LevelInfo, cea.PubNubRequestState);
                //ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (cea.Message.ToString (), cea.PubnubRequestState, false, false, PubnubErrorLevel);
            } else {
                ProcessNonSubscribeResult (cea.PubNubRequestState, cea.Message);
                #if (ENABLE_PUBNUB_LOGGING)
                this.queueManager.PubNubInstance.PNLog.WriteToLog (string.Format ("NonSubscribeHandler: result="), PNLoggingMethod.LevelInfo);
                #endif

                this.queueManager.RaiseRunningRequestEnd(cea.PubNubRequestState.RespType);
                //Helpers.ProcessResponseCallbacks<T> (ref result, cea.PubnubRequestState, this.cipherKey, JsonPluggableLibrary);
            }
        }*/

        public void ProcessNonSubscribeResult (RequestState pubnubRequestState, string jsonString)
        {
            //try {
                
                //string multiChannel = Helpers.GetNamesFromChannelEntities(pubnubRequestState.ChannelEntities, false); 
                //string multiChannelGroup = Helpers.GetNamesFromChannelEntities(pubnubRequestState.ChannelEntities, true);
                if (!string.IsNullOrEmpty (jsonString)) {
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.queueManager.PubNubInstance.PNLog.WriteToLog (string.Format ("ProcessNonSubscribeResult: jsonString = {0} {1}", jsonString, pubnubRequestState.RespType), PNLoggingMethod.LevelInfo);
                    #endif
                    object deSerializedResult = queueManager.PubNubInstance.JsonLibrary.DeserializeToObject (jsonString);
                    if(deSerializedResult!= null){
                        PNBuilder.RaiseCreateResponse(deSerializedResult, pubnubRequestState);
                    }  
                    this.queueManager.PubNubInstance.Latency.StoreLatency(pubnubRequestState.StartRequestTicks, pubnubRequestState.EndRequestTicks, pubnubRequestState.RespType);             
                } 
                #if (ENABLE_PUBNUB_LOGGING)
                else {
                    this.queueManager.PubNubInstance.PNLog.WriteToLog (string.Format ("ProcessNonSubscribeResult: json string null "), PNLoggingMethod.LevelInfo);
                }
                #endif
            /*} catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.queueManager.PubNubInstance.PNLog.WriteToLog (string.Format ("ProcessNonSubscribeResult: exception: {0} ", ex.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
                //ProcessWrapResultBasedOnResponseTypeException<T> (pubnubRequestState, errorLevel, ex);
            }*/
        }

        private void WebRequestCompleteHandler (object sender, EventArgs ea)
        { 
            CustomEventArgs cea = ea as CustomEventArgs;
            try {
               
                if ((cea != null) && (cea.CurrRequestType.Equals(PNCurrentRequestType.NonSubscribe))) {
                    PNStatus pnStatus;
                    if(Helpers.CheckErrorTypeAndCallback<V>(cea, this.queueManager.PubNubInstance, out pnStatus)){
                        #if (ENABLE_PUBNUB_LOGGING)
                        this.queueManager.PubNubInstance.PNLog.WriteToLog(string.Format ("WebRequestCompleteHandler: Is Error true "), PNLoggingMethod.LevelInfo);
                        #endif
                        PNBuilder.RaiseError(pnStatus);
                    } else {
                        //NonSubscribeHandler(cea);  
                        ProcessNonSubscribeResult (cea.PubNubRequestState, cea.Message);
                        #if (ENABLE_PUBNUB_LOGGING)
                        this.queueManager.PubNubInstance.PNLog.WriteToLog (string.Format ("NonSubscribeHandler: result="), PNLoggingMethod.LevelInfo);
                        #endif

                        this.queueManager.RaiseRunningRequestEnd(cea.PubNubRequestState.RespType);
                    }
                    /*}
                    #if (ENABLE_PUBNUB_LOGGING)
                    else {
                        this.PubNubInstance.PNLog.WriteToLog (string.Format ("DateTime {0}, WebRequestCompleteHandler: PubnubRequestState null", DateTime.Now.ToString ()), PNLoggingMethod.LevelError);
                    }
                    #endif*/
                }
                /*#if (ENABLE_PUBNUB_LOGGING)
                else {
                    this.queueManager.PubNubInstance.PNLog.WriteToLog (string.Format ("WebRequestCompleteHandler: cea null"), PNLoggingMethod.LevelInfo);
                }
                #endif*/
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.queueManager.PubNubInstance.PNLog.WriteToLog(string.Format ("WebRequestCompleteHandler: Exception={0}", ex.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
                PNBuilder.RaiseError(PNStatusCategory.PNUnknownCategory, ex, false, PNLoggingMethod.LevelInfo, cea.PubNubRequestState);
                //PNStatus pnStatus = Helpers.Crea;
                //PNBuilder.RaiseError(pnStatus);
                //ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (ex.Message, cea.PubnubRequestState, false, false, PubnubErrorLevel);
            }
            /*try {
                if (cea != null) {
                    if (cea.PubNubRequestState != null) {
                        ResponseCode = cea.PubNubRequestState.ResponseCode;
                        URL = cea.PubNubRequestState.URL;
                        //ProcessCoroutineCompleteResponse<T> (cea);
                    }
                    #if (ENABLE_PUBNUB_LOGGING)
                    else {
                        this.queueManager.PubNubInstance.PNLog.WriteToLog (string.Format ("WebRequestCompleteHandler: PubnubRequestState null"), PNLoggingMethod.LevelInfo);
                    }
                    #endif
                    NonSubscribeHandler(cea);
                }
                #if (ENABLE_PUBNUB_LOGGING)
                else {
                    this.queueManager.PubNubInstance.PNLog.WriteToLog (string.Format ("WebRequestCompleteHandler: cea null"), PNLoggingMethod.LevelInfo);
                }
                #endif
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.queueManager.PubNubInstance.PNLog.WriteToLog(string.Format ("WebRequestCompleteHandler: Exception={0}", ex.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
                PNBuilder.RaiseError(PNStatusCategory.PNUnknownCategory, ex, false, PNLoggingMethod.LevelInfo, cea.PubNubRequestState);
                //ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (ex.Message, cea.PubnubRequestState, false, false, PubnubErrorLevel);
            }*/

            
        }
    }
}

