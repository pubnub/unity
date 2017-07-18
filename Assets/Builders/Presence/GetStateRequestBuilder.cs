using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetStateRequestBuilder: PubNubNonSubBuilder<GetStateRequestBuilder, PNGetStateResult>, IPubNubNonSubscribeBuilder<GetStateRequestBuilder, PNGetStateResult>
    {
        private List<string> ChannelsForState { get; set;}
        private List<string> ChannelGroupsForState { get; set;}

        private string uuid { get; set;}

        public GetStateRequestBuilder(PubNubUnity pn): base(pn){
            Debug.Log ("PNGetStateResult Construct");
        }

        public void UUID(string uuid){
            this.uuid = uuid;
        }

        public void Channels(List<string> channels){
            ChannelsForState = channels;
        }

        public void ChannelGroups(List<string> channelGroups){
            ChannelGroupsForState = channelGroups;
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNGetStateResult, PNStatus> callback)
        {
            this.Callback = callback;
            Debug.Log ("PNGetStateResult Async");
            base.Async(callback, PNOperationType.PNGetStateOperation, CurrentRequestType.NonSubscribe, this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState<PNGetStateResult> requestState = new RequestState<PNGetStateResult> ();
            requestState.RespType = PNOperationType.PNGetStateOperation;

            if (string.IsNullOrEmpty (uuid)) {
                uuid = this.PubNubInstance.PNConfig.UUID;
            }

            string channels = "";
            if((ChannelsForState != null) && (ChannelsForState.Count>0)){
                channels = String.Join(",", ChannelsForState.ToArray());
            }

            string channelGroups = "";
            if((ChannelGroupsForState != null) && (ChannelGroupsForState.Count>0)){
                channelGroups = String.Join(",", ChannelGroupsForState.ToArray());
            }

            Uri request = BuildRequests.BuildGetStateRequest(
                channels,
                channelGroups,
                uuid,
                this.PubNubInstance.PNConfig.UUID,
                this.PubNubInstance.PNConfig.Secure,
                this.PubNubInstance.PNConfig.Origin,
                this.PubNubInstance.PNConfig.AuthKey,
                this.PubNubInstance.PNConfig.SubscribeKey,
                this.PubNubInstance.Version
            );
            this.PubNubInstance.PNLog.WriteToLog(string.Format("RunGetStateRequest {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult){
            //{"status": 200, "message": "OK", "payload": {"channels": {"channel1": {"k": "v"}, "channel2": {}}}, "uuid": "pn-c5a12d424054a3688066572fb955b7a0", "service": "Presence"}

            //TODO read all values.
            

            /*PNHereNowResult pnHereNowResult = new PNHereNowResult();
            
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();

            if (dictionary!=null && dictionary.ContainsKey("error") && dictionary["error"].Equals(true)){
                pnHereNowResult = null;
                pnStatus.Error = true;
                //TODO create error data
            } else if(dictionary!=null) {
                int totalChannels, total_occupancy;
                string log = "";
                object objPayload;
                dictionary.TryGetValue("payload", out objPayload);

                if(objPayload!=null){
                    Dictionary<string, object> payload = objPayload as Dictionary<string, object>;
                    object objChannelsDict;
                    payload.TryGetValue("channels", out objChannelsDict);

                    if(objChannelsDict!=null){
                        Dictionary<string, PNHereNowChannelData> channelsResult;
                        pnStatus.Error = CreateHereNowResult(objChannelsDict, out channelsResult);
                        
                        pnHereNowResult.Channels = channelsResult;
                    } 
                } else if(Utility.CheckKeyAndParseInt(dictionary, "total_channels", "total_channels", out log, out totalChannels)){
                        pnHereNowResult.TotalChannels = totalChannels;
                        Debug.Log(log);
                } else if(Utility.CheckKeyAndParseInt(dictionary, "total_occupancy", "total_occupancy", out log, out total_occupancy)){
                        pnHereNowResult.TotalOccupancy = total_occupancy;
                        Debug.Log(log);
                } else {
                    pnStatus.Error = true;
                }
            } else {
                pnHereNowResult = null;
                pnStatus.Error = true;
            }
            Callback(pnHereNowResult, pnStatus);*/
        }

        public bool CreateHereNowResult(object objChannelsDict, out Dictionary<string, PNHereNowChannelData> channelsResult ){
            Dictionary<string, object> channelsDict = objChannelsDict as Dictionary<string, object>;
            channelsResult = new Dictionary<string, PNHereNowChannelData>();
            //List<string> channels = ch.ToList<string>();//new List<string> ();
            if(channelsDict!=null){
                foreach(KeyValuePair<string, object> kvpair in channelsDict){
                    string channelName = kvpair.Key;
                    PNHereNowChannelData channelData = new PNHereNowChannelData();
                    channelData.Occupants = new List<PNHereNowOccupantData>();
                    channelData.ChannelName = channelName;
                    Debug.Log("channelName:" + channelName);
                    Dictionary<string, object> channelDetails = kvpair.Value as Dictionary<string, object>;
                    if(channelDetails!=null){
                        object objOccupancy;
                        channelDetails.TryGetValue("occupancy", out objOccupancy);
                        int occupancy;
                        if(int.TryParse(objOccupancy.ToString(), out occupancy)){
                            channelData.Occupancy = occupancy;
                            Debug.Log("occupancy:" + occupancy.ToString());
                        }

                        object uuids;
                        channelDetails.TryGetValue("uuids", out uuids);
                        
                        if(uuids!=null){
                            //occupantData.UUID 
                            string[] arrUuids = uuids as string[];
                            
                            if(arrUuids!=null){
                                foreach (string uuid in arrUuids){
                                    PNHereNowOccupantData occupantData = new PNHereNowOccupantData();
                                    occupantData.UUID = uuid;
                                    Debug.Log("uuid:" + uuid);
                                    channelData.Occupants.Add(occupantData);
                                }
                            } else {
                                Dictionary<string, object>[] dictUuidsState = uuids as Dictionary<string, object>[];
                                foreach (Dictionary<string, object> objUuidsState in dictUuidsState){
                                    PNHereNowOccupantData occupantData = new PNHereNowOccupantData();
                                //if(objUuidsState!=null){
                                    //Dictionary<string, object>[] objUuidsState = uuids as Dictionary<string, object>[];
                                    
                                    object objUuid;
                                    bool bUuid = false;
                                    if(objUuidsState.TryGetValue("uuid", out objUuid)){
                                        bUuid= true;
                                        occupantData.UUID = objUuid.ToString();
                                    }
                                    object objState;
                                    bool bState = false;
                                    if(objUuidsState.TryGetValue("state", out objState)){
                                        bState = true;
                                        occupantData.State = objState;
                                    }
                                    if(!bState && !bUuid){
                                        occupantData.State = objUuidsState;
                                    }
                                    channelData.Occupants.Add(occupantData);
                                }
                            }
                            

                        }
                        //Debug.Log("uuids:" + uuids.ToString());
                    }
                    channelsResult.Add(channelName, channelData);
                }
            }
            return false; 
        }
       
    }
}

