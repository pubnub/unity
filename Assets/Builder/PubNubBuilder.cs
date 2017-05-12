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
        public void Execute (PNOperationType pnOpType, PubNubBuilder<U> pnBuilder){
            //HandleSubscribe
            SubscriptionWorker.Instance.Add(pnOpType, pnBuilder);
        }

        //public void Async<T>(Action<T, PNStatus> callback, PNOperationType pnOpType, OperationParams operationParams, CurrentRequestType crt){
        public void Async<T>(Action<T, PNStatus> callback, PNOperationType pnOpType, CurrentRequestType crt, PubNubBuilder<U> pnBuilder){
            /*switch (crt) {
            case CurrentRequestType.Heartbeat:
                break;
            case CurrentRequestType.PresenceHeartbeat:
                break;
            default:*/

            //RequestQueue.Instance.Enqueue<T, U> (callback, pnOpType, pnBuilder);
            RequestQueue.Instance.Enqueue (callback, pnOpType, pnBuilder);


            //RequestQueue.Instance.Enqueue<T> (callback, pnOpType, this);
                /*break;
            }
            //Handle presence heartbeat
            //Handle heartbeat
            //NonSubscribe
            //RequestQueue.Instance.Enqueue<T>(callback, pnOpType, operationParams);
            /*switch(pnOpType)
            {
            case PNOperationType.PNTimeOperation:
                    Debug.Log ("In Async");
                    //RequestQueue.Instance.Enqueue<T>(PNConfig, callback, PNOperationType.PNTimeOperation, null);

                    break;
                default:
                    break;
            }*/
            //return U;
        }


    }
}

