using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class HereNowRequestBuilder: PubNubNonSubBuilder<HereNowRequestBuilder, PNHereNowResult>, IPubNubNonSubscribeBuilder<HereNowRequestBuilder, PNHereNowResult>
    {
        //private List<string> ChannelsToUse { get; set;}
        //private List<string> ChannelGroupsToUse { get; set;}

        private bool IncludeStateInHereNow { get; set;}
        private bool IncludeUUIDsInHereNow { get; set;}

        public HereNowRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNHereNowOperation){
        }

        public void IncludeUUIDs(bool includeUUIDs){
            IncludeUUIDsInHereNow = includeUUIDs;
        }

        public void IncludeState(bool includeState){
            IncludeStateInHereNow = includeState;
        }

        public void Channels(List<string> channels){
            ChannelsToUse = channels;
        }

        public void ChannelGroups(List<string> channelGroups){
            ChannelGroupsToUse = channelGroups;
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNHereNowResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;

            string channels = "";
            if((ChannelsToUse != null) && (ChannelsToUse.Count>0)){
                channels = String.Join(",", ChannelsToUse.ToArray());
            }

            string channelGroups = "";
            if((ChannelGroupsToUse != null) && (ChannelGroupsToUse.Count>0)){
                channelGroups = String.Join(",", ChannelGroupsToUse.ToArray());
            }
            /* Uri request = BuildRequests.BuildHereNowRequest(
                channels,
                channelGroups,
                IncludeUUIDsInHereNow,
                IncludeStateInHereNow,
                this.PubNubInstance.PNConfig.UUID,
                this.PubNubInstance.PNConfig.Secure,
                this.PubNubInstance.PNConfig.Origin,
                this.PubNubInstance.PNConfig.AuthKey,
                this.PubNubInstance.PNConfig.SubscribeKey,
                this.PubNubInstance.Version
            ); */
            Uri request = BuildRequests.BuildHereNowRequest(
                channels,
                channelGroups,
                IncludeUUIDsInHereNow,
                IncludeStateInHereNow,
                ref this.PubNubInstance
            );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //{"status": 200, "message": "OK", "payload": {"channels": {"channel1": {"occupancy": 1, "uuids": ["a"]}, "channel2": {"occupancy": 1, "uuids": ["a"]}}, "total_channels": 2, "total_occupancy": 2}, "service": "Presence"} 
            //TODO read all values.
            PNHereNowResult pnHereNowResult = new PNHereNowResult();
            
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();

            if(dictionary != null) {
                string message = Utility.ReadMessageFromResponseDictionary(dictionary, "message");
                if(Utility.CheckDictionaryForError(dictionary, "error")){
                    pnHereNowResult = null;
                    pnStatus = base.CreateErrorResponseFromMessage(message, requestState, PNStatusCategory.PNUnknownCategory);
                } else {
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
                        pnHereNowResult = null;
                        pnStatus = base.CreateErrorResponseFromMessage("Payload dictionary is null", requestState, PNStatusCategory.PNMalformedResponseCategory);
                    }
                }
            } else {
                pnHereNowResult = null;
                pnStatus = base.CreateErrorResponseFromMessage("Response dictionary is null", requestState, PNStatusCategory.PNMalformedResponseCategory);

            }
            
            Callback(pnHereNowResult, pnStatus);
        }

        // protected override void CreateErrorResponse(Exception exception, bool showInCallback, bool level){
            
        // }

        protected bool CreateHereNowResult(object objChannelsDict, out Dictionary<string, PNHereNowChannelData> channelsResult ){
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

