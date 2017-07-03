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
            if(PubNubInstance != null){
                if ((RequestQueue.Instance.HasItems) && (NoRunningRequests)) {
                    UpdateRunningRequests(false);
                    QueueStorage qs =  RequestQueue.Instance.Dequeue ();
                    PNOperationType operationType = qs.OperationType;
                    object operationParams = qs.OperationParams;
                    switch(operationType){
                        case PNOperationType.PNTimeOperation:
                            TimeRequestBuilder timebuilder  = operationParams as TimeRequestBuilder;//((TimeBuilder)operationParams);
                            timebuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNWhereNowOperation:
                            WhereNowRequestBuilder whereNowBuilder  = operationParams as WhereNowRequestBuilder;//((WhereNowBuilder)operationParams);
                            whereNowBuilder.RaiseRunRequest(this);

                            break;
                        case PNOperationType.PNHistoryOperation:
                            HistoryRequestBuilder historyBuilder  = operationParams as HistoryRequestBuilder;//((HistoryBuilder)operationParams);
                            historyBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNFireOperation:
                            break;
                        case PNOperationType.PNPublishOperation:
                            PublishRequestBuilder publishBuilder  = operationParams as PublishRequestBuilder;//((HistoryBuilder)operationParams);
                            publishBuilder.RaiseRunRequest(this);

                            break;
                        case PNOperationType.PNHereNowOperation:
                        //Herenow,GlobalHerenow
                            break;
                        case PNOperationType.PNLeaveOperation:
                            break;
                            
                        case PNOperationType.PNUnsubscribeOperation:
                            break;
                        case PNOperationType.PNPresenceUnsubscribeOperation:
                            break;
                        case PNOperationType.PNSetStateOperation:
                            break;
                        case PNOperationType.PNGetStateOperation:
                            break;
                        case PNOperationType.PNRemoveAllPushNotificationsOperation:
                            break;
                        case PNOperationType.PNAddPushNotificationsOnChannelsOperation:
                            break;
                        case PNOperationType.PNPushNotificationEnabledChannelsOperation:
                            break;
                        case PNOperationType.PNRemovePushNotificationsFromChannelsOperation:
                            break;
                        case PNOperationType.PNAddChannelsToGroupOperation:
                            break;
                        case PNOperationType.PNChannelGroupsOperation:
                            break;
                        case PNOperationType.PNChannelsForGroupOperation:
                            break;
                        case PNOperationType.PNFetchMessagesOperation:
                            break;
                        case PNOperationType.PNRemoveChannelsFromGroupOperation:
                            break;
                        case PNOperationType.PNRemoveGroupOperation:
                            break;
                    }
                }
            } else {
                Debug.Log("PN instance null");
            }
        }
    }
}

