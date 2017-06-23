using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class NonSubscribeWorker<T>: IDisposable
    {
        private QueueManager queueManager;
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
            
        Action<T, PNStatus> Callback;

        public void Queue(PNConfiguration pnConfig, Action<T, PNStatus> callback){
            

        }
            
        public void RunTimeRequest(PNConfiguration pnConfig, Action<T, PNStatus> callback){
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
        }

        public static U ConvertValue<U,V>(V value) where V : IConvertible
        {
            return (U)Convert.ChangeType(value, typeof(U));
        }

        public static U ConvertValue<U>(string value)
        {
            return (U)Convert.ChangeType(value, typeof(U));
        }

        private void NonSubscribeHandler (CustomEventArgs<T> cea){
            if (cea.IsTimeout || Utility.CheckRequestTimeoutMessageInError (cea)) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, NonSubscribeHandler: NonSub timeout={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelError);
                #endif
                //ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (cea.Message.ToString (), cea.PubnubRequestState, true, false, PubnubErrorLevel);
            } else if (cea.IsError) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, NonSubscribeHandler: NonSub Error={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelError);
                #endif
                //ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (cea.Message.ToString (), cea.PubnubRequestState, false, false, PubnubErrorLevel);
            } else {
                PNResult result = new PNResult();
                ProcessNonSubscribeResult (cea.PubnubRequestState, cea.Message, ref result);
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, NonSubscribeHandler: result={1}", DateTime.Now.ToString (), (result!=null)?result.Count.ToString():"null"), LoggingMethod.LevelInfo);
                #endif

                PNStatus pnStatus = new PNStatus();
                pnStatus.Error = false;
                //Debug.Log("C4" + c [0]);
                try {
                    //TODO: add null check
                    Callback((T)Convert.ChangeType(result, typeof(T)), pnStatus);
                } catch (InvalidCastException ice) {
                    Debug.Log (ice.ToString());
                    throw ice;
                }
                this.queueManager.RaiseRunningRequestEnd(cea.PubnubRequestState.RespType);
                //Helpers.ProcessResponseCallbacks<T> (ref result, cea.PubnubRequestState, this.cipherKey, JsonPluggableLibrary);
            }
        }

        public void ProcessNonSubscribeResult (RequestState<T> pubnubRequestState, string jsonString, ref PNResult result)
        {
            try {
                
                string multiChannel = Helpers.GetNamesFromChannelEntities(pubnubRequestState.ChannelEntities, false); 
                string multiChannelGroup = Helpers.GetNamesFromChannelEntities(pubnubRequestState.ChannelEntities, true);
                if (!string.IsNullOrEmpty (jsonString)) {
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, ProcessNonSubscribeResult: jsonString = {1}", DateTime.Now.ToString (), jsonString), LoggingMethod.LevelInfo);
                    #endif
                    object deSerializedResult = queueManager.PubNubInstance.JsonLibrary.DeserializeToObject (jsonString);
                    /*List<object> result1 = ((IEnumerable)deSerializedResult).Cast<object> ().ToList ();
                    Debug.Log("C2" + result1[0]);
                    if (result1 != null && result1.Count > 0) {
                        result = result1;
                    }*/

                    switch (pubnubRequestState.RespType) {
                    /*case ResponseType.DetailedHistory:
                        result = DecodeDecryptLoop (result, pubnubRequestState.ChannelEntities, cipherKey, jsonPluggableLibrary, errorLevel);
                        result.Add (multiChannel);
                        break;*/
                    case PNOperationType.PNTimeOperation:
                        Int64[] c = deSerializedResult as Int64[];
                        if ((c != null) && (c.Length > 0)) {
                            PNTimeResult pnTimeResult = new PNTimeResult();
                            pnTimeResult.TimeToken = c [0];
                            result = pnTimeResult;
                        }
                        
                        break;
                    case PNOperationType.PNWhereNowOperation:
                        PNWhereNowResult pnWhereNowResult = new PNWhereNowResult();
                        Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;//queueManager.PubNubInstance.JsonLibrary.DeserializeToDictionaryOfObject (jsonString);
                        Dictionary<string, object> dictionary2 = dictionary["payload"] as Dictionary<string, object>;
                        string[] ch = dictionary2["channels"] as string[];
                        List<string> result1 = ch.ToList<string>();//new List<string> ();
                        /*foreach(KeyValuePair<string, object> key in dictionary["payload"] as Dictionary<string, object>){
                            Debug.Log(key.Key + key.Value);
                            result1.Add (key.Value as string);
                        }*/
                        foreach(string key in result1){
                            Debug.Log(key);
                        }

                        //result1.Add (multiChannel);
                        //List<string> result1 = ((IEnumerable)deSerializedResult).Cast<string> ().ToList ();
                        pnWhereNowResult.Channels = result1;
                        result = pnWhereNowResult;
                        break;    
                    
                    /*case ResponseType.Leave:
                        if (!string.IsNullOrEmpty(multiChannelGroup))
                        {
                            result.Add(multiChannelGroup);
                        }
                        if (!string.IsNullOrEmpty(multiChannel))
                        {
                            result.Add(multiChannel);
                        }
                        break;
                    case ResponseType.SubscribeV2:
                    case ResponseType.PresenceV2:
                        break;
                    case ResponseType.Publish:
                    case ResponseType.PushRegister:
                    case ResponseType.PushRemove:
                    case ResponseType.PushGet:
                    case ResponseType.PushUnregister:
                        result.Add (multiChannel);
                        break;
                    case ResponseType.GrantAccess:
                    case ResponseType.AuditAccess:
                    case ResponseType.RevokeAccess:
                    case ResponseType.GetUserState:
                    case ResponseType.SetUserState:
                    case ResponseType.WhereNow:
                    case ResponseType.HereNow:
                        result = DeserializeAndAddToResult (jsonString, multiChannel, jsonPluggableLibrary, true);
                        break;
                    case ResponseType.GlobalHereNow:
                        result = DeserializeAndAddToResult (jsonString, multiChannel, jsonPluggableLibrary, false);
                        break;
                    case ResponseType.ChannelGroupAdd:
                    case ResponseType.ChannelGroupRemove:
                    case ResponseType.ChannelGroupGet:
                        result = DeserializeAndAddToResult (jsonString, multiChannel, jsonPluggableLibrary, false);

                        break;
                    case ResponseType.ChannelGroupGrantAccess:
                    case ResponseType.ChannelGroupAuditAccess:
                    case ResponseType.ChannelGroupRevokeAccess:
                        result = DeserializeAndAddToResult (jsonString, "", jsonPluggableLibrary, false);
                        result.Add(multiChannelGroup);
                        break;*/

                    default:
                        break;
                    }
                } 
                #if (ENABLE_PUBNUB_LOGGING)
                else {
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, ProcessNonSubscribeResult: json string null ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                }
                #endif
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, ProcessNonSubscribeResult: exception: {1} ", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif
                //ProcessWrapResultBasedOnResponseTypeException<T> (pubnubRequestState, errorLevel, ex);
            }
        }

        private void WebRequestCompleteHandler (object sender, EventArgs ea)
        {
            CustomEventArgs<T> cea = ea as CustomEventArgs<T>;
            try {
                if (cea != null) {
                    if (cea.PubnubRequestState != null) {
                        NonSubscribeHandler(cea);
                        //ProcessCoroutineCompleteResponse<T> (cea);
                    }
                    #if (ENABLE_PUBNUB_LOGGING)
                    else {
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CoroutineCompleteHandler: PubnubRequestState null", DateTime.Now.ToString ()), LoggingMethod.LevelError);
                    }
                    #endif
                }
                #if (ENABLE_PUBNUB_LOGGING)
                else {
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CoroutineCompleteHandler: cea null", DateTime.Now.ToString ()), LoggingMethod.LevelError);
                }
                #endif
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CoroutineCompleteHandler: Exception={1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif

                //ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (ex.Message, cea.PubnubRequestState, false, false, PubnubErrorLevel);
            }

            
        }
    }
}

