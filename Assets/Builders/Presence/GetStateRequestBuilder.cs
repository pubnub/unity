using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetStateRequestBuilder: PubNubNonSubBuilder<GetStateRequestBuilder, PNGetStateResult>, IPubNubNonSubscribeBuilder<GetStateRequestBuilder, PNGetStateResult>
    {
        //private List<string> ChannelsToUse { get; set;}
        //private List<string> ChannelGroupsToUse { get; set;}

        private string uuid { get; set;}

        public GetStateRequestBuilder(PubNubUnity pn): base(pn){
            Debug.Log ("PNGetStateResult Construct");
        }

        public void UUID(string uuid){
            this.uuid = uuid;
        }

        public void Channels(List<string> channels){
            ChannelsToUse = channels;
        }

        public void ChannelGroups(List<string> channelGroups){
            ChannelGroupsToUse = channelGroups;
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNGetStateResult, PNStatus> callback)
        {
            this.Callback = callback;
            Debug.Log ("PNGetStateResult Async");
            base.Async(callback, PNOperationType.PNGetStateOperation, PNCurrentRequestType.NonSubscribe, this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.RespType = PNOperationType.PNGetStateOperation;

            if (string.IsNullOrEmpty (uuid)) {
                uuid = this.PubNubInstance.PNConfig.UUID;
            }

            string channels = "";
            if((ChannelsToUse != null) && (ChannelsToUse.Count>0)){
                channels = String.Join(",", ChannelsToUse.ToArray());
            }

            string channelGroups = "";
            if((ChannelGroupsToUse != null) && (ChannelGroupsToUse.Count>0)){
                channelGroups = String.Join(",", ChannelGroupsToUse.ToArray());
            }

            /* Uri request = BuildRequests.BuildGetStateRequest(
                channels,
                channelGroups,
                uuid,
                this.PubNubInstance.PNConfig.UUID,
                this.PubNubInstance.PNConfig.Secure,
                this.PubNubInstance.PNConfig.Origin,
                this.PubNubInstance.PNConfig.AuthKey,
                this.PubNubInstance.PNConfig.SubscribeKey,
                this.PubNubInstance.Version
            ); */
            Uri request = BuildRequests.BuildGetStateRequest(
                channels,
                channelGroups,
                uuid,
                ref this.PubNubInstance
            );
            this.PubNubInstance.PNLog.WriteToLog(string.Format("RunGetStateRequest {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        // protected override void CreateErrorResponse(Exception exception, bool showInCallback, bool level){
            
        // }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //{"status": 200, "message": "OK", "payload": {"channels": {"channel1": {"k": "v"}, "channel2": {}}}, "uuid": "pn-c5a12d424054a3688066572fb955b7a0", "service": "Presence"}

            //TODO read all values.
            
            PNGetStateResult pnGetStateResult = new PNGetStateResult();
            //pnGetStateResult
            
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();

            if (dictionary!=null && dictionary.ContainsKey("error") && dictionary["error"].Equals(true)){
                pnGetStateResult = null;
                pnStatus.Error = true;
                //TODO create error data
            } else if(dictionary!=null) {
                string log = "";
                object objPayload;
                dictionary.TryGetValue("payload", out objPayload);

                if(objPayload!=null){
                    Dictionary<string, object> payload = objPayload as Dictionary<string, object>;
                    object objChannelsDict;
                    payload.TryGetValue("channels", out objChannelsDict);
                    //TODO NO CG
                    //payload.TryGetValue("channelGroups", out objChannelsDict);

                    if(objChannelsDict!=null){
                        Dictionary<string, object> channelsDict = objPayload as Dictionary<string, object>;
                        foreach(KeyValuePair<string, object> kvp in channelsDict){
                            Debug.Log("KVP:" + kvp.Key + kvp.Value);
                        }
                    } 
               
                } else {
                    pnStatus.Error = true;
                }
            } else {
                pnGetStateResult = null;
                pnStatus.Error = true;
            }
            Callback(pnGetStateResult, pnStatus);
        }

    }
}

