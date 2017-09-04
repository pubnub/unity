using System;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public abstract class PubNubNonSubBuilder<U, V> where V : class 
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
        internal void RaiseError(Exception exception, bool showInCallback, bool level){
            this.CreateErrorResponse(exception, showInCallback, level);
        }
        protected RequestState<U> ReqState { get; set;}

        protected abstract void CreatePubNubResponse(object deSerializedResult);

        protected void CreateErrorResponse(Exception exception, bool showInCallback, bool level){
            PNStatus pnStatus = Helpers.CreatePNStatus(
                PNStatusCategory.PNConnectedCategory,
                "",
                null,
                false,
                0,
                PNOperationType.PNSubscribeOperation,
                PubNubInstance.PNConfig.Secure,
                PubNubInstance.PNConfig.UUID,
                PubNubInstance.PNConfig.AuthKey,
                PubNubInstance.PNConfig.Origin,
                null,
                null
            );
            Callback(null, pnStatus);
        }

        protected abstract void RunWebRequest(QueueManager qm);

        public void Async(Action<V, PNStatus> callback, PNOperationType pnOpType, PNCurrentRequestType crt, PubNubNonSubBuilder<U, V> pnBuilder){
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

