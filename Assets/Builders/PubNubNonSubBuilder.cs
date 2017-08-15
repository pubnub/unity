using System;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public abstract class PubNubNonSubBuilder<U, V>
    {
        protected Action<V, PNStatus> Callback;

        protected delegate void RunRequestDelegate(QueueManager qm);
        protected event RunRequestDelegate RunRequest;

        protected delegate void CreateResponseDelegate(object deSerializedResult);
        protected event CreateResponseDelegate CreateResponse;

        protected PubNubUnity PubNubInstance { get; set;}
        protected internal PubNubNonSubBuilder(PubNubUnity pn){
            PubNubInstance = pn;
            this.RunRequest += delegate(QueueManager qm) {
                RunWebRequest(qm);
            };
            this.CreateResponse += delegate(object deSerializedResult) {
                CreatePubNubResponse(deSerializedResult);
            };

        }
        internal void RaiseRunRequest(QueueManager qm){
            this.RunRequest(qm);
        }
        internal void RaiseCreateResponse(object createResponse){
            this.CreateResponse(createResponse);
        }
        protected RequestState<U> ReqState { get; set;}

        protected abstract void CreatePubNubResponse(object deSerializedResult);

        protected abstract void RunWebRequest(QueueManager qm);

        public void Async(Action<V, PNStatus> callback, PNOperationType pnOpType, CurrentRequestType crt, PubNubNonSubBuilder<U, V> pnBuilder){
            RequestQueue.Instance.Enqueue (callback, pnOpType, pnBuilder, this.PubNubInstance);
        }

        /*protected void CreatePubNubResponse<T>(object deSerializedResult){

        }*/

        protected void RunWebRequest(QueueManager qm, Uri request, RequestState<V> requestState, int timeout, int pause, PubNubNonSubBuilder<U, V> pnBuilder){
            NonSubscribeWorker<U, V> nonSubscribeWorker = new NonSubscribeWorker<U, V> (qm);
            nonSubscribeWorker.RunWebRequest(request.OriginalString, requestState, timeout, 0, pnBuilder); 
        }
    }
}

