/*using System;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public abstract class PubNubSubscribeBuilder<U>
    {
        protected PubNubUnity PubNubInstance { get; set;}
        protected PubNubSubscribeBuilder(PubNubUnity pn){
            PubNubInstance = pn;
        }

        protected RequestState<U> ReqState { get; set;}

        public void Execute (PNOperationType pnOpType, PubNubSubscribeBuilder<U> pnBuilder){
            //HandleSubscribe
            switch (pnOpType) {
            case PNOperationType.PNSubscribeOperation:
            case PNOperationType.PNPresenceOperation:
                //Debug.Log ("pn" + this.PubNubInstance.Test);
                RequestState<SubscribeBuilder> reqStateSubscribeBuilder = ReqState as RequestState<SubscribeBuilder>;
                //this.PubNubInstance.SubWorker.Add (pnOpType, pnBuilder, reqStateSubscribeBuilder, this.PubNubInstance);
                break;
            default:
                //Debug.Log ("pn" + this.PubNubInstance.Test);
                break;
            }
        }
    }
}*/