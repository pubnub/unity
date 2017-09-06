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

        protected PubNubUnity PubNubInstance { get; set;}
        protected internal PubNubNonSubBuilder(PubNubUnity pn){
            PubNubInstance = pn;
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
        internal void RaiseError(PNStatusCategory pnStatusCategory, Exception exception, bool showInCallback, bool level, RequestState pnRequestState){
            this.CreateErrorResponse(pnStatusCategory, exception, showInCallback, level, pnRequestState);
        }
        internal void RaiseError(PNStatus pnStatus){    
            //this.CreateErrorResponse(pnStatusCategory, exception, showInCallback, level, pnRequestState);
            //if(showInCallback){
            Callback(null, pnStatus);
            //}
        }
        //protected RequestState ReqState { get; set;}

        protected abstract void CreatePubNubResponse(object deSerializedResult, RequestState pnRequestState);

        protected void CreateErrorResponse(PNStatusCategory pnStatusCategory, Exception exception, bool showInCallback, bool level, RequestState pnRequestState){
            //PNStatus pnStatus;
            //if(Helpers.CheckErrorTypeAndCallback(cea, this.PubNubInstance, out pnStatus)){

            PNStatus pnStatus = Helpers.CreatePNStatus(
                pnStatusCategory,
                exception.Message,
                exception,
                true,                
                PNOperationType.PNSubscribeOperation,
                ChannelsToUse,
                ChannelGroupsToUse,
                pnRequestState,
                PubNubInstance
            );
            if(showInCallback){
                Callback(null, pnStatus);
            }
        }

        protected abstract void RunWebRequest(QueueManager qm);

        public void Async(Action<V, PNStatus> callback, PNOperationType pnOpType, PNCurrentRequestType crt, PubNubNonSubBuilder<U, V> pnBuilder){
            RequestQueue.Instance.Enqueue (callback, pnOpType, pnBuilder, this.PubNubInstance);
        }

        /*protected void CreatePubNubResponse<T>(object deSerializedResult){

        }*/

        protected void RunWebRequest(QueueManager qm, Uri request, RequestState requestState, int timeout, int pause, PubNubNonSubBuilder<U, V> pnBuilder){
            NonSubscribeWorker<U, V> nonSubscribeWorker = new NonSubscribeWorker<U, V> (qm);
            nonSubscribeWorker.RunWebRequest(request.OriginalString, requestState, timeout, 0, pnBuilder); 
        }
    }
}

