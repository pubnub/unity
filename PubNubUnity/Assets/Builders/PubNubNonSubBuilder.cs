using System;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public abstract class PubNubNonSubBuilder<U, V> where V : class 
    {
        protected List<string> ChannelGroupsToUse { get; set;}
        protected List<string> ChannelsToUse { get; set;}
        protected Action<V, PNStatus> Callback;

        protected delegate void RunRequestDelegate(QueueManager qm);
        protected event RunRequestDelegate RunRequest;

        protected delegate void CreateResponseDelegate(object deSerializedResult, RequestState pnRequestState);
        protected event CreateResponseDelegate CreateResponse;

        public PNOperationType OperationType {get; set;}

        protected PubNubUnity PubNubInstance;
        protected internal PubNubNonSubBuilder(PubNubUnity pn, PNOperationType pnOperationType){
            PubNubInstance = pn;
            this.OperationType = pnOperationType;
            
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog(string.Format("{0} constructor", pnOperationType.ToString()), PNLoggingMethod.LevelInfo);
            #endif

            this.RunRequest += delegate(QueueManager qm) {
                RunWebRequest(qm);
            };
            this.CreateResponse += delegate(object deSerializedResult, RequestState pnRequestState) {
                CreatePubNubResponse(deSerializedResult, pnRequestState);
            };

        }
        internal void RaiseRunRequest(QueueManager qm){
            this.RunRequest(qm);
        }
        internal void RaiseCreateResponse(object createResponse, RequestState pnRequestState){
            this.CreateResponse(createResponse, pnRequestState);
        }
        internal void RaiseError(PNStatusCategory pnStatusCategory, Exception exception, bool showInCallback, RequestState pnRequestState){
            this.CreateErrorResponse(pnStatusCategory, exception, showInCallback, pnRequestState);
        }
        internal void RaiseError(PNStatus pnStatus){    
            Callback(null, pnStatus);
        }
        protected abstract void CreatePubNubResponse(object deSerializedResult, RequestState pnRequestState);

        protected void CreateErrorResponse(PNStatusCategory pnStatusCategory, Exception exception, bool showInCallback, RequestState pnRequestState){
            PNStatus pnStatus = Helpers.CreatePNStatus(
                pnStatusCategory,
                exception.Message,
                exception,
                true,                
                OperationType,
                ChannelsToUse,
                ChannelGroupsToUse,
                pnRequestState,
                PubNubInstance
            );
            if(showInCallback){
                Callback(null, pnStatus);
            }
        }
       
        protected PNStatus CreateErrorResponseFromMessage(string message, RequestState pnRequestState, PNStatusCategory pnStatusCategory){
            return Helpers.CreatePNStatus(
                    pnStatusCategory,
                    message,
                    new PubNubException(message),
                    true,                
                    OperationType,
                    ChannelsToUse,
                    ChannelGroupsToUse,
                    pnRequestState,
                    PubNubInstance
                );
        }

        protected PNStatus CreateStatusResponseFromMessage(bool isError, string message, RequestState pnRequestState, PNStatusCategory pnStatusCategory){
            return Helpers.CreatePNStatus(
                    pnStatusCategory,
                    message,
                    (isError)?new PubNubException(message):null,
                    isError,                
                    OperationType,
                    ChannelsToUse,
                    ChannelGroupsToUse,
                    pnRequestState,
                    PubNubInstance
                );
        }

        protected PNStatus CreateErrorResponseFromException(Exception ex, RequestState pnRequestState, PNStatusCategory pnStatusCategory){
            return Helpers.CreatePNStatus(
                    pnStatusCategory,
                    ex.Message,
                    ex,
                    true,                
                    OperationType,
                    ChannelsToUse,
                    ChannelGroupsToUse,
                    pnRequestState,
                    PubNubInstance
                );
        }

        protected abstract void RunWebRequest(QueueManager qm);

        public void Async(PubNubNonSubBuilder<U, V> pnBuilder){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog(string.Format("base Async {0}", OperationType.ToString()), PNLoggingMethod.LevelInfo);
            #endif

            if(Callback == null){
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog(string.Format("Callback is null. Stopping request.{0}", OperationType.ToString()), PNLoggingMethod.LevelWarning);
                #endif
                
                return;
            }

            RequestQueue.Instance.Enqueue (Callback, OperationType, pnBuilder, this.PubNubInstance);
        }

        protected void RunWebRequest(QueueManager qm, Uri request, RequestState requestState, int timeout, int pause, PubNubNonSubBuilder<U, V> pnBuilder){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog(string.Format("{1}: {0}", request.OriginalString, requestState.OperationType), PNLoggingMethod.LevelInfo);
            #endif
            NonSubscribeWorker<U, V> nonSubscribeWorker = new NonSubscribeWorker<U, V> (qm);
            requestState.URL = request.OriginalString; 
            requestState.Timeout = timeout;
            requestState.Pause = 0;
            requestState.Reconnect = false;

            nonSubscribeWorker.RunWebRequest(requestState, pnBuilder); 
        }
    }
}

