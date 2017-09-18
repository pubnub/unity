using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetAllChannelsForGroupRequestBuilder: PubNubNonSubBuilder<GetAllChannelsForGroupRequestBuilder, PNChannelGroupsAllChannelsResult>, IPubNubNonSubscribeBuilder<GetAllChannelsForGroupRequestBuilder, PNChannelGroupsAllChannelsResult>
    {      
        public GetAllChannelsForGroupRequestBuilder(PubNubUnity pn):base(pn){

        }
        private string ChannelGroupToList { get; set;}

        public void ChannelGroup(string channelGroup){
            ChannelGroupToList = channelGroup;
            ChannelGroupsToUse = new List<string>(){ChannelGroupToList};
        }
        
        #region IPubNubBuilder implementation

        public void Async(Action<PNChannelGroupsAllChannelsResult, PNStatus> callback)
        {
            this.Callback = callback;
            Debug.Log ("GetAllChannelsForGroupRequestBuilder Async");
            if (string.IsNullOrEmpty (ChannelGroupToList)) {
                Debug.Log("ChannelGroup to list to empty");
                return;
            }
            base.Async(callback, PNOperationType.PNChannelsForGroupOperation, PNCurrentRequestType.NonSubscribe, this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.RespType = PNOperationType.PNChannelsForGroupOperation;
            
            /* Uri request = BuildRequests.BuildGetChannelsForChannelGroupRequest(
                "",
                ChannelGroupToList,
                false,
                this.PubNubInstance.PNConfig.UUID,
                this.PubNubInstance.PNConfig.Secure,
                this.PubNubInstance.PNConfig.Origin,
                this.PubNubInstance.PNConfig.AuthKey,
                this.PubNubInstance.PNConfig.SubscribeKey,
                this.PubNubInstance.Version
            ); */
            Uri request = BuildRequests.BuildGetChannelsForChannelGroupRequest(
                "",
                ChannelGroupToList,
                false,
                ref this.PubNubInstance
            );
            
            this.PubNubInstance.PNLog.WriteToLog(string.Format("Run BuildGetChannelsForChannelGroupRequest {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        
        }

        // protected override void CreateErrorResponse(Exception exception, bool showInCallback, bool level){
            
        // }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //{"status": 200, "payload": {"channels": ["channel1", "channel2"], "group": "channelGroup1"}, "service": "channel-registry", "error": false} 
            PNChannelGroupsAllChannelsResult pnChannelGroupsAllChannelsResult = new PNChannelGroupsAllChannelsResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if (dictionary!=null && dictionary.ContainsKey("error") && dictionary["error"].Equals(true)){
                pnChannelGroupsAllChannelsResult = null;
                pnStatus.Error = true;
                //TODO create error data
            } else if(dictionary!=null) {
                object objPayload;
                dictionary.TryGetValue("payload", out objPayload);
                if(objPayload!=null){
                    
                    Dictionary<string, object> payload = objPayload as Dictionary<string, object>;
                    object objChannelsArray;
                    payload.TryGetValue("channels", out objChannelsArray);
                    if(objChannelsArray != null){
                        string[] channelsArray = objChannelsArray as string[];
                        if(channelsArray != null){
                            pnChannelGroupsAllChannelsResult.Channels = new List<string>();
                            foreach(string str in channelsArray){
                                Debug.Log("strchannelsArray:" + str);
                                pnChannelGroupsAllChannelsResult.Channels.Add(str);
                            }
                        }
                    } 
                }
            } else {
                pnChannelGroupsAllChannelsResult = null;
                pnStatus.Error = true;
            }
            Callback(pnChannelGroupsAllChannelsResult, pnStatus);
        }
        
    }
}

