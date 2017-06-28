using System;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public abstract class PubNubNonSubBuilder<U>
    {
        //private PNConfiguration PNConfig { get; set;}

        /*public PubNubBuilder(PNConfiguration pnConfig){
            PNConfig = pnConfig;    
        }*/

        protected delegate void RunRequestDelegate(QueueManager qm);
        protected event RunRequestDelegate RunRequest;

        protected delegate void CreateResponseDelegate(object deSerializedResult);
        protected event CreateResponseDelegate CreateResponse;

        protected PubNubUnity PubNubInstance { get; set;}
        protected PubNubNonSubBuilder(PubNubUnity pn){
            PubNubInstance = pn;
            this.RunRequest += delegate(QueueManager qm) {
                RunWebRequest(qm);
            };
            this.CreateResponse += delegate(object deSerializedResult) {
                CreatePubNubResponse(deSerializedResult);
            };

        }
        protected internal void RaiseRunRequest(QueueManager qm){
            this.RunRequest(qm);
        }
        protected internal void RaiseCreateResponse(object CreateResponse){
            this.CreateResponse(CreateResponse);
        }
        protected RequestState<U> ReqState { get; set;}

        protected abstract void CreatePubNubResponse(object deSerializedResult);

        protected abstract void RunWebRequest(QueueManager qm);

        public void Async<T>(Action<T, PNStatus> callback, PNOperationType pnOpType, CurrentRequestType crt, PubNubNonSubBuilder<U> pnBuilder){
            RequestQueue.Instance.Enqueue (callback, pnOpType, pnBuilder, this.PubNubInstance);
        }

        protected void CreatePubNubResponse<T>(object deSerializedResult){

        }

        protected void RunWebRequest<T>(QueueManager qm, Uri request, RequestState<T> requestState, int timeout, int pause, PubNubNonSubBuilder<U> pnBuilder){
            NonSubscribeWorker<T, U> nonSubscribeWorker = new NonSubscribeWorker<T, U> (qm);
            nonSubscribeWorker.RunWebRequest(request.OriginalString, requestState, timeout, 0, pnBuilder); 
        }
    }
}

