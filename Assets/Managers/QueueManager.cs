using System;
using UnityEngine;

namespace PubNubAPI
{
    public class QueueManager: MonoBehaviour
    {

        bool NoRunningReuqets = true;
        void Update(){
            //TODO: READ pnconfig from pubnub.cs, handle property change

            if ((RequestQueue.Instance.HasItems) && (NoRunningReuqets)) {
                QueueStorage qs =  RequestQueue.Instance.Dequeue ();
                PNOperationType operationType = qs.OperationType;
                OperationParams operationParams = qs.OperationParams;
                switch(operationType){
                    case PNOperationType.PNTimeOperation:
                        Action<PNTimeResult, PNStatus> timeCallback = qs.Callback as Action<PNTimeResult, PNStatus>;
                        NonSubscribeWorker<PNTimeResult> timeNonSubscribeWorker = new NonSubscribeWorker<PNTimeResult> ();
                        timeNonSubscribeWorker.RunTimeRequest (null, timeCallback);
                        break;
                    case PNOperationType.PNWhereNowOperation:
                        Action<PNWhereNowResult, PNStatus> whereNowCallback = qs.Callback as Action<PNWhereNowResult, PNStatus>;
                        NonSubscribeWorker<PNWhereNowResult> whereNowNonSubscribeWorker = new NonSubscribeWorker<PNWhereNowResult> ();
                        //whereNowNonSubscribeWorker.RunWhereNowRequest (null, whereNowCallback, (WhereNowOperationParams)operationParams);
                        whereNowNonSubscribeWorker.RunWhereNowRequest (null, whereNowCallback, (WhereNowBuilder)operationParams);
                        break;

                }
                    
                //NonSubscribeWorker<T> nsw = new NonSubscribeWorker<T> ();
                //nsw.RunTimeRequest (PNConfig, callback);
            }
        }
    }
}

