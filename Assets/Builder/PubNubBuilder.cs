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

        public void Execute (PNOperationType pnOpType, OperationParams operationParams){
            //HandleSubscribe
        }

        public void Async<T>(Action<T, PNStatus> callback, PNOperationType pnOpType, OperationParams operationParams, CurrentRequestType crt){
            switch (crt) {
            case CurrentRequestType.Heartbeat:
                break;
            case CurrentRequestType.PresenceHeartbeat:
                break;
            default:
                RequestQueue.Instance.Enqueue<T> (callback, pnOpType, operationParams);
                break;
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

        public void SetChannels(List<string> channels){
            //return U;
        }

        public void SetChannelGroups(List<string> channelGroups){
            //return U;
        }
    }
}

