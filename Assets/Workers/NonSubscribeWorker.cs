using System;
using UnityEngine;

namespace PubNubAPI
{
    public class NonSubscribeWorker<T>: IDisposable
    {
        public static int InstanceCount;
        private static object syncRoot = new System.Object();
        #region IDisposable implementation

        public void Dispose ()
        {
            lock (syncRoot) {
                InstanceCount--;
            }
        }

        #endregion


        public NonSubscribeWorker ()
        {
            lock (syncRoot) {
                InstanceCount++;
            }
        }
            
        Action<T, PNStatus> Callback;

        public void Queue(PNConfiguration pnConfig, Action<T, PNStatus> callback){
            

        }

        public void RunTimeRequest(PNConfiguration pnConfig, Action<T, PNStatus> callback){
            //Uri request = BuildRequests.BuildTimeRequest (this.SessionUUID, this.ssl, this.Origin);

            RequestState<T> requestState = new RequestState<T> ();
            //requestState.ChannelEntities = channelEntities;
            requestState.RespType = PNOperationType.PNTimeOperation;
            /*requestState.Reconnect = reconnect;
            requestState.SuccessCallback = userCallback;
            requestState.ErrorCallback = errorCallback;
            requestState.ID = id;
            requestState.Timeout = timeout;
            requestState.Timetoken = timetoken;
            requestState.TypeParameterType = typeParam;
            requestState.UUID = uuid;
            return requestState;*/
            Debug.Log ("RunTimeRequest");



            //save callback
            this.Callback = callback;

            Debug.Log ("RunTimeRequest gobj");
            PNUnityWebRequest webRequest = PubNub.GameObjectRef.AddComponent<PNUnityWebRequest> ();
            webRequest.NonSubCoroutineComplete += CoroutineCompleteHandler;
            Debug.Log ("RunTimeRequest coroutine");
            //PNCallback<T> timeCallback = new PNTimeCallback<T> (callback);
            webRequest.Run<T>("https://pubsub.pubnub.com/time/0", requestState, 10, 0);
            Debug.Log ("after coroutine");

        }

        //public void RunWhereNowRequest(PNConfiguration pnConfig, Action<T, PNStatus> callback, WhereNowOperationParams operationParams){
        public void RunWhereNowRequest(PNConfiguration pnConfig, Action<T, PNStatus> callback, WhereNowBuilder operationParams){
            //Uri request = BuildRequests.BuildTimeRequest (this.SessionUUID, this.ssl, this.Origin);



            RequestState<T> requestState = new RequestState<T> ();
            //requestState.ChannelEntities = channelEntities;
            requestState.RespType = PNOperationType.PNWhereNowOperation;
            /*requestState.Reconnect = reconnect;
            requestState.SuccessCallback = userCallback;
            requestState.ErrorCallback = errorCallback;
            requestState.ID = id;
            requestState.Timeout = timeout;
            requestState.Timetoken = timetoken;
            requestState.TypeParameterType = typeParam;
            requestState.UUID = uuid;
            return requestState;*/
            Debug.Log ("RunWhereNowRequest");

            Debug.Log ("WhereNowBuilder UuidForWhereNow: " + operationParams.UuidForWhereNow);

            //save callback
            this.Callback = callback;

            Debug.Log ("RunWhereNowRequest gobj");
            PNUnityWebRequest webRequest = PubNub.GameObjectRef.AddComponent<PNUnityWebRequest> ();
            webRequest.NonSubCoroutineComplete += CoroutineCompleteHandler;
            Debug.Log ("RunWhereNowRequest coroutine");
            //PNCallback<T> timeCallback = new PNTimeCallback<T> (callback);
            //http://ps.pndsn.com/v2/presence/sub-key/sub-c-5c4fdcc6-c040-11e5-a316-0619f8945a4f/uuid/UUID_WhereNow?pnsdk=PubNub-Go%2F3.14.0&uuid=UUID_WhereNow
            webRequest.Run<T>("https://pubsub.pubnub.com/v2/presence/sub-key/demo/uuid/UUID_WhereNow?uuid=UUID_WhereNow", requestState, 10, 0);
            Debug.Log ("after coroutine");

        }

        public static T ConvertValue<T,U>(U value) where U : IConvertible
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static T ConvertValue<T>(string value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        private void CoroutineCompleteHandler (object sender, EventArgs ea)
        {
            CustomEventArgs<T> cea = ea as CustomEventArgs<T>;


            try {
                if (cea != null) {

                    //TODO identify from T instead of request state
                    RequestState<T> requestState = cea.PubnubRequestState;        
                    Debug.Log ("inCoroutineCompleteHandler " + requestState.RespType);
                    switch(requestState.RespType){
                    case PNOperationType.PNTimeOperation:
                        //PNTimeCallback<T> timeCallback = new PNTimeCallback<T> ();
                        PNTimeResult pnTimeResult = new PNTimeResult();
                        pnTimeResult.TimeToken = cea.Message;
                        PNStatus pnStatus = new PNStatus();
                        pnStatus.Error = false;
                        /*if (pnTimeResult is T) {
                        //return (T)pnTimeResult;
                        //Callback((T)pnTimeResult, pnStatus);
                        } else {*/
                        try {
                            //return (T)Convert.ChangeType(pnTimeResult, typeof(T));
                            Debug.Log ("Callback");
                            Callback((T)Convert.ChangeType(pnTimeResult, typeof(T)), pnStatus);

                            Debug.Log ("After Callback");
                        } catch (InvalidCastException ice) {
                            //return default(T);
                            Debug.Log (ice.ToString());
                            throw ice;
                        }
                        //}

                        //T pnTimeResult2 = (T)pnTimeResult as object;
                        //Callback(pnTimeResult2, pnStatus);
                        //PNTimeResult pnTimeResult2 = (T)pnTimeResult;
                        //timeCallback.OnResponse(pnTimeResult, pnStatus);

                        /*if (cea.PubnubRequestState != null) {
                        ProcessCoroutineCompleteResponse<T> (cea);
                        }*/
                        break;
                    case PNOperationType.PNWhereNowOperation:
                        PNWhereNowResult pnWhereNowResult = new PNWhereNowResult();
                        pnWhereNowResult.Result = cea.Message;
                        PNStatus pnWhereNowStatus = new PNStatus();
                        pnWhereNowStatus.Error = false;
                        /*if (pnTimeResult is T) {
                        //return (T)pnTimeResult;
                        //Callback((T)pnTimeResult, pnStatus);
                        } else {*/
                        try {
                            //return (T)Convert.ChangeType(pnTimeResult, typeof(T));
                            Debug.Log ("Callback");
                            Callback((T)Convert.ChangeType(pnWhereNowResult, typeof(T)), pnWhereNowStatus);

                            Debug.Log ("After Callback");
                        } catch (InvalidCastException ice) {
                            //return default(T);
                            Debug.Log (ice.ToString());
                            throw ice;
                        }
                        break;
                    default:
                        Debug.Log ("default");
                        break;
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
                Debug.Log (ex.ToString());
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CoroutineCompleteHandler: Exception={1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif

                //ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (ex.Message, cea.PubnubRequestState,
                  //  false, false, PubnubErrorLevel);
            }
        }
    }
}

