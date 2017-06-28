using System;
using UnityEngine;

namespace PubNubAPI
{
    public class QueueManager: MonoBehaviour
    {
        private readonly object lockObj = new object();

        public delegate void RunningRequestEndDelegate(PNOperationType operationType);
        public event RunningRequestEndDelegate RunningRequestEnd;
        private bool NoRunningRequests = true;
        internal ushort NoOfConcurrentRequests = 1;
        public PubNubUnity PubNubInstance { get; set;}
        private ushort RunningRequests = 0;

        void Start(){
            this.RunningRequestEnd += delegate(PNOperationType operationType) {
                //Debug.Log(operationType + DateTime.Now.ToLongTimeString());
                UpdateRunningRequests(true);
            };
        }

        void UpdateRunningRequests(bool RequestComplete){
            lock(lockObj){
                if (RequestComplete) {
                    RunningRequests--;
                } else {
                    RunningRequests++;
                }
                //Debug.Log(RunningRequests.ToString() + RequestComplete.ToString());
                if (RunningRequests <= NoOfConcurrentRequests) {
                    NoRunningRequests = true;
                } else {
                    NoRunningRequests = false;
                }
            }
        }

        public void RaiseRunningRequestEnd(PNOperationType operationType){
            this.RunningRequestEnd(operationType);
        }

        void Update(){
            //Debug.Log(RunningRequests.ToString() + NoRunningRequests);
            if(PubNubInstance != null){
                //Debug.Log(PubNubInstance.Test);
                if ((RequestQueue.Instance.HasItems) && (NoRunningRequests)) {
                    UpdateRunningRequests(false);
                    QueueStorage qs =  RequestQueue.Instance.Dequeue ();
                    PNOperationType operationType = qs.OperationType;
                    //OperationParams operationParams = qs.OperationParams;
                    object operationParams = qs.OperationParams;
                    switch(operationType){
                        case PNOperationType.PNTimeOperation:
                            Action<PNTimeResult, PNStatus> timeCallback = qs.Callback as Action<PNTimeResult, PNStatus>;

                            //NonSubscribeWorker<PNTimeResult> timeNonSubscribeWorker = new NonSubscribeWorker<PNTimeResult> (this);
                            //timeNonSubscribeWorker.RunTimeRequest (null, timeCallback);
                            TimeBuilder timebuilder  = ((TimeBuilder)operationParams);
                            timebuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNWhereNowOperation:
                            //Action<PNWhereNowResult, PNStatus> whereNowCallback = qs.Callback as Action<PNWhereNowResult, PNStatus>;
                            WhereNowBuilder whereNowBuilder  = ((WhereNowBuilder)operationParams);
                            whereNowBuilder.RaiseRunRequest(this);

                            //NonSubscribeWorker<PNWhereNowResult> whereNowNonSubscribeWorker = new NonSubscribeWorker<PNWhereNowResult> (this);
                            //whereNowNonSubscribeWorker.RunWhereNowRequest (null, whereNowCallback, (WhereNowOperationParams)operationParams);

                            //whereNowNonSubscribeWorker.RunWhereNowRequest (null, whereNowCallback, (WhereNowBuilder)operationParams);
                            break;
                        case PNOperationType.PNHistoryOperation:
                            HistoryBuilder historyBuilder  = ((HistoryBuilder)operationParams);
                            historyBuilder.RaiseRunRequest(this);

                            //Action<PNHistoryResult, PNStatus> historyCallback = qs.Callback as Action<PNHistoryResult, PNStatus>;

                            //NonSubscribeWorker<PNHistoryResult> historyNonSubscribeWorker = new NonSubscribeWorker<PNHistoryResult> (this);

                            //historyNonSubscribeWorker.RunHistoryRequest (null, historyCallback, (HistoryBuilder)operationParams);
                            break;
                    }
                        
                    //NonSubscribeWorker<T> nsw = new NonSubscribeWorker<T> ();
                    //nsw.RunTimeRequest (PNConfig, callback);
                }
            } else {
                Debug.Log("PN instance null");
            }
        }
    }
}

