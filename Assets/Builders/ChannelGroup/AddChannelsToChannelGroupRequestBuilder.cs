using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class AddChannelsToChannelGroupRequestBuilder: PubNubNonSubBuilder<AddChannelsToChannelGroupRequestBuilder, PNChannelGroupsAddChannelResult>, IPubNubNonSubscribeBuilder<AddChannelsToChannelGroupRequestBuilder, PNChannelGroupsAddChannelResult>
    {
        private List<string> ChannelsForState { get; set;}
        private List<string> ChannelGroupsForState { get; set;}

        private string uuid { get; set;}
        private Dictionary<string, object> state { get; set;}

        public AddChannelsToChannelGroupRequestBuilder(PubNubUnity pn): base(pn){
            Debug.Log ("AddChannelsToGroupRequestBuilder Construct");
        }

        public void UUID(string uuid){
            this.uuid = uuid;
        }

        public void State(Dictionary<string, object> state){
            this.state = state;
        }

        public void Channels(List<string> channels){
            ChannelsForState = channels;
        }

        public void ChannelGroups(List<string> channelGroups){
            ChannelGroupsForState = channelGroups;
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNChannelGroupsAddChannelResult, PNStatus> callback)
        {
            this.Callback = callback;
            //validate state here
            try{
                if(state!=null){
                    Type t = state.GetType();
                    bool isDict = t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>);
                    if(!isDict){
                        throw new MissingMemberException ("State is not of type Dictionary<,>");
                    } else {
                        //TODO process state
                    }

                }
                /*if (!this.PubNubInstance.JsonLibrary.IsDictionaryCompatible (state)) {
                    
                } else {
                    Dictionary<string, object> deserializeUserState = this.PubNubInstance.JsonLibrary.DeserializeToDictionaryOfObject (jsonUserState);
                    if (deserializeUserState == null) {
                        throw new MissingMemberException ("Missing JSON formatted user state");
                    } else {
                        string userState = "";
                        List<ChannelEntity> channelEntities;
                        if (Helpers.CheckAndAddExistingUserState<T> (channel, channelGroup,
                            deserializeUserState, userCallback, errorCallback, errorLevel, false,
                            uuid, this.SessionUUID,
                            out userState, out channelEntities
                        )) {
                            SharedSetUserState<T> (channel, channelGroup,
                                channelEntities, uuid, userState);
                        }
                    }
                }*/

            } catch (Exception ex){
                Debug.Log(ex.ToString());
            }
            

            Debug.Log ("PNChannelGroupsAddChannelResult Async");
            base.Async(callback, PNOperationType.PNAddChannelsToGroupOperation, CurrentRequestType.NonSubscribe, this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState<PNSetStateResult> requestState = new RequestState<PNSetStateResult> ();
            requestState.RespType = PNOperationType.PNAddChannelsToGroupOperation;

            string channels = "";
            if((ChannelsForState != null) && (ChannelsForState.Count>0)){
                channels = String.Join(",", ChannelsForState.ToArray());
            }

            string channelGroups = "";
            if((ChannelGroupsForState != null) && (ChannelGroupsForState.Count>0)){
                channelGroups = String.Join(",", ChannelGroupsForState.ToArray());
            }

            if (string.IsNullOrEmpty (uuid)) {
                uuid = this.PubNubInstance.PNConfig.UUID;
            }

            /*Uri request = BuildRequests.BuildSetStateRequest(
                channels,
                channelGroups,
                jsonUserState,
                uuid,
                this.PubNubInstance.PNConfig.UUID,
                this.PubNubInstance.PNConfig.Secure,
                this.PubNubInstance.PNConfig.Origin,
                this.PubNubInstance.PNConfig.AuthKey,
                this.PubNubInstance.PNConfig.SubscribeKey,
                this.PubNubInstance.Version
            );
            this.PubNubInstance.PNLog.WriteToLog(string.Format("RunHereNowRequest {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); */
        }

        protected override void CreatePubNubResponse(object deSerializedResult){
            //{"status": 200, "message": "OK", "payload": {"channels": {"channel1": {"occupancy": 1, "uuids": ["a"]}, "channel2": {"occupancy": 1, "uuids": ["a"]}}, "total_channels": 2, "total_occupancy": 2}, "service": "Presence"} 
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
            Callback(pnHereNowResult, pnStatus);
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
            return false; */
        }
       
    }
}

