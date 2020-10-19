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
        private ushort RunningRequests;

        void Start(){
            this.RunningRequestEnd += delegate(PNOperationType operationType) {
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
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog(string.Format("RunningRequests+RequestComplete {0} -- {1}", RunningRequests.ToString(), RequestComplete.ToString()), PNLoggingMethod.LevelInfo);
                #endif
                
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
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog(string.Format("operationType.ToString() {0}", operationType.ToString()), PNLoggingMethod.LevelInfo);
                    #endif
                    
                    object operationParams = qs.OperationParams;
                    switch(operationType){
                        case PNOperationType.PNTimeOperation:
                            TimeRequestBuilder timebuilder  = operationParams as TimeRequestBuilder;
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
                        case PNOperationType.PNPublishFileMessageOperation:
                            PublishFileMessageRequestBuilder publishFileBuilder  = operationParams as PublishFileMessageRequestBuilder;
                            publishFileBuilder.RaiseRunRequest(this);

                            break;
                        case PNOperationType.PNSignalOperation:
                            SignalRequestBuilder signalBuilder  = operationParams as SignalRequestBuilder;
                            signalBuilder.RaiseRunRequest(this);

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
                            #if (ENABLE_PUBNUB_LOGGING)
                            this.PubNubInstance.PNLog.WriteToLog((operationParams == null)? "operationParams null" : "operationParams not null", PNLoggingMethod.LevelInfo);
                            #endif
                            
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
                        case PNOperationType.PNMessageCountsOperation:
                            MessageCountsRequestBuilder messageCountsRequestBuilder = operationParams as MessageCountsRequestBuilder;
                            messageCountsRequestBuilder.RaiseRunRequest(this);

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
                        case PNOperationType.PNSetUUIDMetadataOperation:
                            SetUUIDMetadataRequestBuilder createUserBuilder = operationParams as SetUUIDMetadataRequestBuilder;
                            createUserBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNGetAllUUIDMetadataOperation:
                            GetAllUUIDMetadataRequestBuilder getUsersRequestBuilder = operationParams as GetAllUUIDMetadataRequestBuilder;
                            getUsersRequestBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNGetUUIDMetadataOperation:
                            GetUUIDMetadataRequestBuilder getUUIDMetadataRequestBuilder = operationParams as GetUUIDMetadataRequestBuilder;
                            getUUIDMetadataRequestBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNRemoveUUIDMetadataOperation:
                            RemoveUUIDMetadataRequestBuilder deleteUserBuilder = operationParams as RemoveUUIDMetadataRequestBuilder;
                            deleteUserBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNSetChannelMetadataOperation:
                            SetChannelMetadataRequestBuilder updateSpaceBuilder = operationParams as SetChannelMetadataRequestBuilder;
                            updateSpaceBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNRemoveChannelMetadataOperation:
                            RemoveChannelMetadataRequestBuilder deleteSpaceBuilder = operationParams as RemoveChannelMetadataRequestBuilder;
                            deleteSpaceBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNGetChannelMetadataOperation:
                            GetChannelMetadataRequestBuilder getChannelMetadataRequestBuilder = operationParams as GetChannelMetadataRequestBuilder;
                            getChannelMetadataRequestBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNGetAllChannelMetadataOperation:
                            GetAllChannelMetadataRequestBuilder getSpaceBuilder = operationParams as GetAllChannelMetadataRequestBuilder;
                            getSpaceBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNGetMembershipsOperation:
                            GetMembershipsRequestBuilder getMembershipsRequestBuilder = operationParams as GetMembershipsRequestBuilder;
                            getMembershipsRequestBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNGetChannelMembersOperation:
                            GetChannelMembersRequestBuilder getMembersRequestBuilder = operationParams as GetChannelMembersRequestBuilder;
                            getMembersRequestBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNManageMembershipsOperation:
                            ManageMembershipsRequestBuilder manageMembershipsRequestBuilder = operationParams as ManageMembershipsRequestBuilder;
                            manageMembershipsRequestBuilder.RaiseRunRequest(this);
                            break;                                                                                    
                        case PNOperationType.PNManageChannelMembersOperation:
                            ManageChannelMembersRequestBuilder manageMembersRequestBuilder = operationParams as ManageChannelMembersRequestBuilder;
                            manageMembersRequestBuilder.RaiseRunRequest(this);
                            break;                         
                        case PNOperationType.PNAddMessageActionsOperation:
                            AddMessageActionsRequestBuilder addMessageActionsRequestBuilder = operationParams as AddMessageActionsRequestBuilder;
                            addMessageActionsRequestBuilder.RaiseRunRequest(this);
                            break;                         
                        case PNOperationType.PNRemoveMessageActionsOperation:
                            RemoveMessageActionsRequestBuilder removeMessageActionsRequestBuilder = operationParams as RemoveMessageActionsRequestBuilder;
                            removeMessageActionsRequestBuilder.RaiseRunRequest(this);
                            break;                         
                        case PNOperationType.PNGetMessageActionsOperation:
                            GetMessageActionsRequestBuilder getMessageActionsRequestBuilder = operationParams as GetMessageActionsRequestBuilder;
                            getMessageActionsRequestBuilder.RaiseRunRequest(this);
                            break;                         
                        case PNOperationType.PNGrantTokenOperation:
                            GrantTokenBuilder grantTokenBuilder = operationParams as GrantTokenBuilder;
                            grantTokenBuilder.RaiseRunRequest(this);
                            break;  
                        case PNOperationType.PNSendFileOperation:
                            SendFileRequestBuilder sendFileRequestBuilder = operationParams as SendFileRequestBuilder;
                            sendFileRequestBuilder.RaiseRunRequest(this);
                            break;  
                        case PNOperationType.PNDeleteFileOperation:
                            DeleteFileRequestBuilder deleteFileRequestBuilder = operationParams as DeleteFileRequestBuilder;
                            deleteFileRequestBuilder.RaiseRunRequest(this);
                            break;  
                        case PNOperationType.PNDownloadFileOperation:
                            // DownloadFileRequestBuilder downloadFileRequestBuilder = operationParams as DownloadFileRequestBuilder;
                            // downloadFileRequestBuilder.RaiseRunRequest(this);
                            break;  
                        case PNOperationType.PNListFilesOperation:
                            ListFilesRequestBuilder listFilesRequestBuilder = operationParams as ListFilesRequestBuilder;
                            listFilesRequestBuilder.RaiseRunRequest(this);
                            break;  
                                                   
                        default:
                        break;
                    }
                } 
            } else {                
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog("PN instance null", PNLoggingMethod.LevelInfo);
                #endif
            }
        }
    }
}
