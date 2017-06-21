using System;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class PubNubBuilder<U>
    {
        //private PNConfiguration PNConfig { get; set;}

        /*public PubNubBuilder(PNConfiguration pnConfig){
            PNConfig = pnConfig;    
        }*/
        protected PubNubUnity PubNubInstance { get; set;}
        protected PubNubBuilder(PubNubUnity pn){
            PubNubInstance = pn;
        }

        protected RequestState<U> ReqState { get; set;}

        public void Execute (PNOperationType pnOpType, PubNubBuilder<U> pnBuilder){
            //HandleSubscribe
            switch (pnOpType) {
            case PNOperationType.PNSubscribeOperation:
            case PNOperationType.PNPresenceOperation:
                Debug.Log ("pn" + this.PubNubInstance.Test);
                RequestState<SubscribeBuilder> reqStateSubscribeBuilder = ReqState as RequestState<SubscribeBuilder>;
                this.PubNubInstance.SubWorker.Add (pnOpType, pnBuilder, reqStateSubscribeBuilder, this.PubNubInstance);
                break;
            default:
                Debug.Log ("pn" + this.PubNubInstance.Test);
                break;
            }
        }

        public void Async<T>(Action<T, PNStatus> callback, PNOperationType pnOpType, CurrentRequestType crt, PubNubBuilder<U> pnBuilder){
            RequestQueue.Instance.Enqueue (callback, pnOpType, pnBuilder, this.PubNubInstance);
        }
    }
}

