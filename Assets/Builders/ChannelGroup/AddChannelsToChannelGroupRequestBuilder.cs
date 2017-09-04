using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class AddChannelsToChannelGroupRequestBuilder: PubNubNonSubBuilder<AddChannelsToChannelGroupRequestBuilder, PNChannelGroupsAddChannelResult>, IPubNubNonSubscribeBuilder<AddChannelsToChannelGroupRequestBuilder, PNChannelGroupsAddChannelResult>
    {
        private List<string> ChannelsToAdd { get; set;}
        private string ChannelGroupToAdd { get; set;}

        public AddChannelsToChannelGroupRequestBuilder(PubNubUnity pn): base(pn){
            Debug.Log ("AddChannelsToGroupRequestBuilder Construct");
        }

        public void Channels(List<string> channels){
            ChannelsToAdd = channels;
        }

        public void ChannelGroup(string channelGroup){
            ChannelGroupToAdd = channelGroup;
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNChannelGroupsAddChannelResult, PNStatus> callback)
        {
            this.Callback = callback;

            if((ChannelsToAdd == null) || ((ChannelsToAdd != null) && (ChannelsToAdd.Count <= 0))){
                Debug.Log("ChannelsToAdd null or empty");

                //TODO Send callback
                return;
            }

            if (string.IsNullOrEmpty (ChannelGroupToAdd)) {
                Debug.Log("ChannelGroup to add to empty");

                //TODO Send callback
                return;
            }

            Debug.Log ("PNChannelGroupsAddChannelResult Async");
            base.Async(callback, PNOperationType.PNAddChannelsToGroupOperation, PNCurrentRequestType.NonSubscribe, this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState<PNChannelGroupsAddChannelResult> requestState = new RequestState<PNChannelGroupsAddChannelResult> ();
            requestState.RespType = PNOperationType.PNAddChannelsToGroupOperation;

            Uri request = BuildRequests.BuildAddChannelsToChannelGroupRequest(
                ChannelsToAdd.ToArray(), 
                "", 
                ChannelGroupToAdd,
                this.PubNubInstance.PNConfig.UUID,
                this.PubNubInstance.PNConfig.Secure,
                this.PubNubInstance.PNConfig.Origin,
                this.PubNubInstance.PNConfig.AuthKey,
                this.PubNubInstance.PNConfig.SubscribeKey,
                this.PubNubInstance.Version
            );
            this.PubNubInstance.PNLog.WriteToLog(string.Format("RunPNChannelGroupsAddChannel {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        // protected override void CreateErrorResponse(Exception exception, bool showInCallback, bool level){
            
        // }

        protected override void CreatePubNubResponse(object deSerializedResult){
            //{"service" : "channel-registry","status"  : 200,"error"   : false,"message" : "OK"}

            //TODO read all values.
            PNChannelGroupsAddChannelResult pnChannelGroupsAddChannelResult = new PNChannelGroupsAddChannelResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if (dictionary!=null && dictionary.ContainsKey("error") && dictionary["error"].Equals(true)){
                pnChannelGroupsAddChannelResult = null;
                pnStatus.Error = true;
                //TODO create error data
            } else if(dictionary!=null) {
                object objMessage;
                dictionary.TryGetValue("message", out objMessage);
                if(objMessage!=null){
                    pnChannelGroupsAddChannelResult.Message = objMessage.ToString();
                }
            } else {
                pnChannelGroupsAddChannelResult = null;
                pnStatus.Error = true;
            }
            Callback(pnChannelGroupsAddChannelResult, pnStatus);
        }
       
    }
}

