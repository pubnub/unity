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
        protected PubNub PubNubInstance { get; set;}
        protected PubNubBuilder(PubNub pn){
            PubNubInstance = pn;
        }

        protected RequestState<U> ReqState { get; set;}

        public void Execute (PNOperationType pnOpType, PubNubBuilder<U> pnBuilder){
            //HandleSubscribe
            Debug.Log("pn"+this.PubNubInstance.Test);
            SubscriptionWorker<U>.Instance.Add(pnOpType, pnBuilder, ReqState, this.PubNubInstance);
        }

        public void Async<T>(Action<T, PNStatus> callback, PNOperationType pnOpType, CurrentRequestType crt, PubNubBuilder<U> pnBuilder){
            RequestQueue.Instance.Enqueue (callback, pnOpType, pnBuilder, this.PubNubInstance);
        }
    }
}

