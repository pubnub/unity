using System;
using UnityEngine;

namespace PubNubAPI
{
    public class QueueManager: MonoBehaviour
    {
        private readonly object lockObj = new object();

        public delegate void RunningRequestEndDelegate(PNOperationType operationType);
        public event RunningRequestEndDelegate RunningRequestEnd;
        private bool RunRequest = true;
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
                Debug.Log("RunningRequests+RequestComplete:" + RunningRequests.ToString() + RequestComplete.ToString());
                if ((NoOfConcurrentRequests.Equals(0)) || (RunningRequests <= NoOfConcurrentRequests)) {
                    RunRequest = true;
                } else {
                    RunRequest = false;
                }
            }
        }

        bool GetRunningRequests(){
            lock(lockObj){
                return RunRequest;
            }
        }

        public void RaiseRunningRequestEnd(PNOperationType operationType){
            this.RunningRequestEnd(operationType);
        }

        void Update(){
            if(PubNubInstance != null){
                bool runRequests = GetRunningRequests();
                if ((RequestQueue.Instance.HasItems) && (runRequests)) {
                    UpdateRunningRequests(false);
                    QueueStorage qs =  RequestQueue.Instance.Dequeue ();
                    PNOperationType operationType = qs.OperationType;
                    Debug.Log(operationType.ToString());
                    object operationParams = qs.OperationParams;
                    switch(operationType){
                        case PNOperationType.PNTimeOperation:
                            TimeRequestBuilder timebuilder  = operationParams as TimeRequestBuilder;//((TimeBuilder)operationParams);
                            timebuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNWhereNowOperation:
                            WhereNowRequestBuilder whereNowBuilder  = operationParams as WhereNowRequestBuilder;
                            whereNowBuilder.RaiseRunRequest(this);

                            break;
                        case PNOperationType.PNHistoryOperation:
                            HistoryRequestBuilder historyBuilder  = operationParams as HistoryRequestBuilder;
                            historyBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNPublishOperation:
                            PublishRequestBuilder publishBuilder  = operationParams as PublishRequestBuilder;
                            publishBuilder.RaiseRunRequest(this);

                            break;
                        case PNOperationType.PNHereNowOperation:
                            HereNowRequestBuilder hereNowBuilder  = operationParams as HereNowRequestBuilder;
                            hereNowBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNLeaveOperation:
                            LeaveRequestBuilder leaveBuilder  = operationParams as LeaveRequestBuilder;
                            leaveBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNSetStateOperation:
                            SetStateRequestBuilder setStateBuilder  = operationParams as SetStateRequestBuilder;
                            setStateBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNGetStateOperation:
                            GetStateRequestBuilder getStateBuilder = operationParams as GetStateRequestBuilder;
                            getStateBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNRemoveAllPushNotificationsOperation:
                            RemoveAllPushChannelsForDeviceRequestBuilder removeAllPushNotificationsRequestBuilder = operationParams as RemoveAllPushChannelsForDeviceRequestBuilder;
                            removeAllPushNotificationsRequestBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNAddPushNotificationsOnChannelsOperation:
                            AddChannelsToPushRequestBuilder addChannelsToGroupBuilder = operationParams as AddChannelsToPushRequestBuilder;
                            addChannelsToGroupBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNPushNotificationEnabledChannelsOperation:
                            ListPushProvisionsRequestBuilder pushNotificationEnabledChannelsRequestBuilder = operationParams as ListPushProvisionsRequestBuilder;
                            pushNotificationEnabledChannelsRequestBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNRemovePushNotificationsFromChannelsOperation:
                            RemoveChannelsFromPushRequestBuilder pushNotificationsFromChannelsRequestBuilder = operationParams as RemoveChannelsFromPushRequestBuilder;
                            pushNotificationsFromChannelsRequestBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNAddChannelsToGroupOperation:
                            
                            AddChannelsToChannelGroupRequestBuilder addChannelsToGroupRequestBuilder = operationParams as AddChannelsToChannelGroupRequestBuilder;
                            addChannelsToGroupRequestBuilder.RaiseRunRequest(this);

                            break;
                        case PNOperationType.PNChannelGroupsOperation:
                            Debug.Log((operationParams == null)? "operationParams null" : "operationParams not null");
                            GetChannelGroupsRequestBuilder getChannelGroupsBuilder = operationParams as GetChannelGroupsRequestBuilder;
                            getChannelGroupsBuilder.RaiseRunRequest(this);

                            break;
                        case PNOperationType.PNChannelsForGroupOperation:
                            GetAllChannelsForGroupRequestBuilder getChannelsForGroupRequestBuilder = operationParams as GetAllChannelsForGroupRequestBuilder;
                            getChannelsForGroupRequestBuilder.RaiseRunRequest(this);

                            break;
                        case PNOperationType.PNFetchMessagesOperation:
                            FetchMessagesRequestBuilder fetchMessagesRequestBuilder = operationParams as FetchMessagesRequestBuilder;
                            fetchMessagesRequestBuilder.RaiseRunRequest(this);

                            break;
                        case PNOperationType.PNDeleteMessagesOperation:
                            DeleteMessagesRequestBuilder deleteMessagesRequestBuilder = operationParams as DeleteMessagesRequestBuilder;
                            deleteMessagesRequestBuilder.RaiseRunRequest(this);

                            break;
                        case PNOperationType.PNRemoveChannelsFromGroupOperation:
                            RemoveChannelsFromGroupRequestBuilder removeChannelsFromGroupRequestBuilder = operationParams as RemoveChannelsFromGroupRequestBuilder;
                            removeChannelsFromGroupRequestBuilder.RaiseRunRequest(this);

                            break;
                        case PNOperationType.PNRemoveGroupOperation:
                            DeleteChannelGroupRequestBuilder removeGroupRequestBuilder = operationParams as DeleteChannelGroupRequestBuilder;
                            removeGroupRequestBuilder.RaiseRunRequest(this);

                            break;
                    }
                } /*else {
                    Debug.Log(RequestQueue.Instance.HasItems.ToString() + runRequests.ToString());    
                }*/
                //this.PubNubInstance.Latency.Update();
            } else {
                Debug.Log("PN instance null");
            }
        }
    }
}
