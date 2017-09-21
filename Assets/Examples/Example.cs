using UnityEngine;
using System.Collections;
using PubNubAPI;
using System.Collections.Generic;
using System;

//delete
//using UnityEngine.Networking;

namespace PubNubExample
{
    public class Example : MonoBehaviour {
         PubNub pubnub;
    	// Use this for initialization
    	void Start () {
            /*UnityWebRequest wr = UnityWebRequest.Post ("http://localhost:8082?s=a", "sss");
            wr.Send();*/

            Debug.Log ("Starting");
            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.SubscribeKey = "sub-c-b05d4a0c-708d-11e7-96c9-0619f8945a4f";
            pnConfiguration.PublishKey = "pub-c-94691e07-c8aa-42f9-a838-bea61ac6655e";
            pnConfiguration.SecretKey = "sec-c-ZmIyZjFjMjQtZTNmZC00MmIwLWFhNzUtNDUyNmIwYWU1YzRl";
            /*pnConfiguration.SubscribeKey = "demo-36";
            pnConfiguration.PublishKey = "demo-36";
            pnConfiguration.SecretKey = "demo-36";*/
            pnConfiguration.Secure = true;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            //pnConfiguration.Origin = "localhost:8082";

            //TODO: remove
            pnConfiguration.UUID = "PubNubUnityExample";
            Debug.Log ("PNConfiguration");  
            pubnub = new PubNub (pnConfiguration);

            pubnub.AddListener (
                (s) => {
                    PrintStatus(s);
                    //Debug.Log ("AddListener in status" + String.Join(", ", s.AffectedChannelGroups.ToArray()) + String.Join(", ", s.AffectedChannels.ToArray()));
                },
                (m) => {
                    Debug.Log ("AddListener in message" + m.Channel + m.Payload);
                    //example to check channel
                    //example to cast message

                },
                (p) => {
                    Debug.Log ("AddListener in presence" + p.Channel + p.Occupancy + p.Event);
                }

            );
            string cg1 = "channelGroup1";
            string cg2 = "channelGroup2";
            string ch1 = "channel1";
            string ch2 = "channel2";
            List<string> listChannelGroups = new List<string> (){cg1, cg2};
            List<string> listChannels = new List<string> (){ch1, ch2};
            
            /*Dictionary<string, object> state = new Dictionary<string, object>();
            state.Add  ("k1", "v1");
            pubnub.SetPresenceState().ChannelGroups(listChannelGroups).Channels(listChannels).State(state).Async ((result, status) => {
                if(status.Error){
                    Debug.Log (string.Format("In Example, SetPresenceState Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("DateTime {0}, In Example SetPresenceState, result:", DateTime.UtcNow));
                }
            });*/
            pubnub.Publish().Channel("channel1").Message("test message").UsePost(true).Async((result, status) => {
                    Debug.Log ("in Publish");
                    if(!status.Error){
                        Debug.Log (string.Format("DateTime {0}, In Publish Example, Timetoken: {1}", DateTime.UtcNow , result.Timetoken));
                    } else {
                        Debug.Log (status.Error);
                        Debug.Log (status.ErrorData.Info);
                    }

                });

            pubnub.DeleteMessages().Channel("channel1").Start(15059180223219815).End(15059180231056976).Async((result, status) => {
                Debug.Log ("in DeleteMessages");
                if(!status.Error){
                    Debug.Log (string.Format("DateTime {0}, In DeleteMessages Example, Timetoken: {1}", DateTime.UtcNow , result.Message));
                } else {
                    Debug.Log (status.Error);
                    Debug.Log (status.ErrorData.Info);
                }

            });

            /*pubnub.SusbcribeCallback += (sender, e) => { //; //+= (pnStatus, pnMessageResut, pnPresenceEventResult) => {
                SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;

                Debug.Log ("In Example, SusbcribeCallback");
                if(mea.pnStatus != null){
                    //Debug.Log ("SusbcribeCallback in status" + String.Join(", ", mea.pnStatus.AffectedChannelGroups.ToArray()) + String.Join(", ", mea.pnStatus.AffectedChannels.ToArray()));
                    PrintStatus(mea.pnStatus);
                    if(mea.pnStatus.Category.Equals(PNStatusCategory.PNConnectedCategory)){
                        pubnub.Publish().Channel("channel1").Message("test message").UsePost(true).Async((result, status) => {
                            Debug.Log ("in Publish");
                            if(!status.Error){
                                Debug.Log (string.Format("DateTime {0}, In Publish Example, Timetoken: {1}", DateTime.UtcNow , result.Timetoken));
                            } else {
                                Debug.Log (status.Error);
                                Debug.Log (status.ErrorData.Info);
                            }

                        });
                    }
                }
                if(mea.pnMessageResult != null){
                    Debug.Log ("In Example, SusbcribeCallback in message" + mea.pnMessageResult.Channel + mea.pnMessageResult.Payload);
                    Display(string.Format("SusbcribeCallback Result: {0}", pubnub.JsonLibrary.SerializeToJsonString(mea.pnMessageResult.Payload)));
                }
                if(mea.pnPresenceEventResult != null){
                    Debug.Log ("In Example, SusbcribeCallback in presence" + mea.pnPresenceEventResult.Channel + mea.pnPresenceEventResult.Occupancy + mea.pnPresenceEventResult.Event);
                }
                /*pubnub.Fire().Channel("channel1").Message("test fire essage").Async((result, status) => {
                    Debug.Log ("in Fire");
                    Debug.Log (string.Format("DateTime {0}, In Fire Example, Timetoken: {1}", DateTime.UtcNow , result.Timetoken));
                    Debug.Log (status.Error);

                });
                
                pubnub.WhereNow ().Async ((result, status) => {
                    Debug.Log ("in WhereNow");
                    Debug.Log (string.Format("DateTime {0}, In Example, Channels: {1}", DateTime.UtcNow , (result.Channels!=null)?string.Join(",",result.Channels.ToArray()):""));
                    Debug.Log (status.Error);

                });*/
                
                //herenow
                /*pubnub.HereNow().Channels(listChannels).ChannelGroups(listChannelGroups).IncludeState(true).IncludeUUIDs(true).Async((result, status) => {
                    Debug.Log ("in HereNow1");
                    Debug.Log (string.Format("DateTime {0}, In Example, Channels: {1} {2}", DateTime.UtcNow , result.TotalChannels, result.TotalOccupancy));
                    DisplayHereNowResult(result);
                    
                    Debug.Log (status.Error);

                });
                //globalherenow
                pubnub.HereNow().IncludeState(true).IncludeUUIDs(true).Async((result, status) => {
                    Debug.Log ("in HereNow2");
                    
                    Debug.Log (string.Format("DateTime {0}, In Example, Channels: {1} {2}", DateTime.UtcNow , result.TotalChannels, result.TotalOccupancy));
                    DisplayHereNowResult(result);
                    Debug.Log (status.Error);

                });

                pubnub.HereNow().IncludeState(false).IncludeUUIDs(true).Async((result, status) => {
                    Debug.Log ("in HereNow3");
                    Debug.Log (string.Format("DateTime {0}, In Example, Channels: {1} {2}", DateTime.UtcNow , result.TotalChannels, result.TotalOccupancy));
                    DisplayHereNowResult(result);
                    Debug.Log (status.Error);

                });

                pubnub.HereNow().IncludeState(false).IncludeUUIDs(false).Async((result, status) => {
                    Debug.Log ("in HereNow4");
                    Debug.Log (string.Format("DateTime {0}, In Example, Channels: {1} {2}", DateTime.UtcNow , result.TotalChannels, result.TotalOccupancy));
                    DisplayHereNowResult(result);
                    Debug.Log (status.Error);

                });

                pubnub.HereNow().IncludeState(true).IncludeUUIDs(false).Async((result, status) => {
                    Debug.Log ("in HereNow5");
                    Debug.Log (string.Format("DateTime {0}, In Example, Channels: {1} {2}", DateTime.UtcNow , result.TotalChannels, result.TotalOccupancy));
                    DisplayHereNowResult(result);
                    Debug.Log (status.Error);

                });*/

            //};

            //Debug.Log ("PubNub");
            //pubnub.Subscribe ().SetChannelGroups (listChannelGroups).SetChannels(listChannels).WithPresence().Execute();

            Debug.Log ("before Time");
            /*pubnub.Time ().Async (new PNTimeCallback<PNTimeResult>(
                (r, s) => {
                    Debug.Log ("in Time");
                }
            ));*/
           pubnub.Time ().Async ((result, status) => {
                
                if(status.Error){
                    PrintStatus(status);
                    Debug.Log (string.Format("In Example, Time Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("DateTime {0}, In Example, result: {1}", DateTime.UtcNow ,result.TimeToken));
                    Display(string.Format("Time Result: {0}", result.TimeToken.ToString()));
                }
            });

            //pubnub.History ().Channel("channel1").Start(14987439725282000).End(14985453001147606).IncludeTimetoken(false).Reverse(false).Async ((result, status) => {
            /*pubnub.History ().Channel("channel1").Async ((result, status) => {
                
                if(status.Error){
                    Debug.Log (string.Format("In Example, History Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("DateTime {0}, In Example, result: {1}", DateTime.UtcNow ,result.EndTimetoken, (result.Messages.Count>0)?result.Messages[0].ToString():""));
                }
            });

            pubnub.GetPresenceState().Channels(listChannels).ChannelGroups(listChannelGroups).Async ((result, status) => {
            //pubnub.GetPresenceState().Channels(listChannels).ChannelGroups(listChannelGroups).Async ((result, status) => {
                
                if(status.Error){
                    Debug.Log (string.Format("In Example, GetPresenceState Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("DateTime {0}, In Example GetPresenceState, result:", DateTime.UtcNow));
                }
            });
            //pubnub.Time ().Async (new PNCallback<PNTimeResult>(){

                //Debug.Log ("in Time")
            //});*/

            /*FetchMessages(pubnub, listChannels);

            pubnub.AddChannelsToChannelGroup().Channels(listChannels).ChannelGroup(cg1).Async((result, status) => {
                Debug.Log ("in AddChannelsToChannelGroup");
                if(status.Error){
                    Debug.Log (string.Format("In Example, AddChannelsToChannelGroup Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("DateTime {0}, In Example AddChannelsToChannelGroup, result: {1}", DateTime.UtcNow, result.Message));
                }

                ListAllChannelsOfGroup(pubnub, cg1);

                List<string> listChannelsRemove = new List<string> (){ch1};
                RemoveChannelsFromCG(pubnub, cg1, listChannelsRemove);

                
                
                
                pubnub.DeleteChannelsFromChannelGroup().ChannelGroup(cg1).Async((result1, status1) => {
                    if(status1.Error){
                        Debug.Log (string.Format("In Example, DeleteChannelsFromChannelGroup Error: {0} {1} {2}", status1.StatusCode, status1.ErrorData, status1.Category));
                    } else {
                        Debug.Log (string.Format("DateTime {0}, In Example DeleteChannelsFromChannelGroup, result: {1}", DateTime.UtcNow, result1.Message));
                    }
                
                });
                ListAllChannelsOfGroup(pubnub, cg1);
            });
            string deviceId = "aaa";
            PNPushType pnPushType = PNPushType.GCM;

            /*pubnub.Unsubscribe().ChannelGroups(listChannelGroups).Channels(listChannels).Async((result, status) => {
                Debug.Log ("in Unsubscribe");
                if(status.Error){
                    Debug.Log (string.Format("In Example, Unsubscribe Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("DateTime {0}, In Unsubscribe, result: {1}", DateTime.UtcNow, result.Message));
                }
            });

            pubnub.UnsubscribeAll().Async((result, status) => {
                Debug.Log ("in UnsubscribeAll");
                if(status.Error){
                    Debug.Log (string.Format("In Example, UnsubscribeAll Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("DateTime {0}, In UnsubscribeAll, result: {1}", DateTime.UtcNow, result.Message));
                }
            });
            */

            /*pubnub.AddPushNotificationsOnChannels().Channels(listChannels).DeviceIDForPush(deviceId).PushType(pnPushType).Async((result, status) => {
                    Debug.Log ("in AddPushNotificationsOnChannels");
                    if(status.Error){
                        Debug.Log (string.Format("In Example, AddPushNotificationsOnChannels Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    } else {
                        Debug.Log (string.Format("DateTime {0}, In AddPushNotificationsOnChannels, result: {1}", DateTime.UtcNow, result.Message));
                    }
                    AuditPushChannelProvisions(pubnub, deviceId, pnPushType);
                    RemoveChannelsFromPush(listChannels, pubnub, deviceId, pnPushType);
                });
            /*Debug.Log ("after Time");*/
            
            //pubnub.WhereNow ().Uuid ("test uuid").Async ((result, status) => {
            
            //pubnub.Subscribe ().Async<string> ();
    	}

        void RemoveChannelsFromPush(List<string> listChannels, PubNub pubnub, string deviceId, PNPushType pnPushType){
            pubnub.RemovePushNotificationsFromChannels().Channels(listChannels).DeviceIDForPush(deviceId).PushType(pnPushType).Async((result, status) => {
                    Debug.Log ("in RemovePushNotificationsFromChannels");
                    if(status.Error){
                        Debug.Log (string.Format("In Example, RemovePushNotificationsFromChannels Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    } else {
                        Debug.Log (string.Format("DateTime {0}, In RemovePushNotificationsFromChannels, result: {1}", DateTime.UtcNow, result.Message));
                    }
                });
        }

        void AuditPushChannelProvisions(PubNub pubnub, string deviceId, PNPushType pnPushType){
            pubnub.AuditPushChannelProvisions().DeviceIDForPush(deviceId).PushType(pnPushType).Async((result, status) => {
                    Debug.Log ("in AuditPushChannelProvisions");
                    if(status.Error){
                        Debug.Log (string.Format("In Example, AuditPushChannelProvisions Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    } else {
                        Debug.Log (string.Format("DateTime {0}, In AuditPushChannelProvisions, result: {1}", DateTime.UtcNow, string.Join(",", result.Channels.ToArray())));
                    }

                });
                
                RemoveAllPushNotificationsFromChannels(pubnub, deviceId, pnPushType);
        }

        void RemoveAllPushNotificationsFromChannels(PubNub pubnub, string deviceId, PNPushType pnPushType){
            pubnub.RemoveAllPushNotifications().DeviceIDForPush(deviceId).PushType(pnPushType).Async((result, status) => {
                    Debug.Log ("in RemoveAllPushNotificationsFromChannels");
                    if(status.Error){
                        Debug.Log (string.Format("In Example, RemoveAllPushNotificationsFromChannels Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    } else {
                        Debug.Log (string.Format("DateTime {0}, In RemoveAllPushNotificationsFromChannels, result: {1}", DateTime.UtcNow, result.Message));
                    }

                });
        }

        void RemoveChannelsFromCG(PubNub pubnub, string cg, List<string> listChannelsRemove){
            pubnub.RemoveChannelsFromChannelGroup().Channels(listChannelsRemove).ChannelGroup(cg).Async((result, status) => {
                    Debug.Log ("in RemoveChannelsFromCG");
                    if(status.Error){
                        Debug.Log (string.Format("In Example, RemoveChannelsFromCG Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    } else {
                        Debug.Log (string.Format("DateTime {0}, In RemoveChannelsFromCG, result: {1}", DateTime.UtcNow, result.Message));
                    }

                });
        }

        void ListAllChannelsOfGroup(PubNub pubnub, string cg){
            pubnub.ListChannelsForChannelGroup().ChannelGroup(cg).Async((result, status) => {
                    if(status.Error){
                        Debug.Log (string.Format("In Example, ListAllChannelsOfGroup Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    } else {
                        Debug.Log (string.Format("In Example ListAllChannelsOfGroup, result: {0}", (result.Channels!=null)?string.Join(",", result.Channels.ToArray()):""));
                    }
                
                });
        }

        void FetchMessages(PubNub pubnub, List<string> listChannels){
            pubnub.FetchMessages().Channels(listChannels).Async ((result, status) => {
                
                if(status.Error){
                    Debug.Log (string.Format("In Example, FetchMessages Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("In FetchMessages, result: "));//,result.EndTimetoken, result.Messages[0].ToString()));
                    foreach(KeyValuePair<string, List<PNMessageResult>> kvp in result.Channels){
                        Debug.Log("kvp channelname" + kvp.Key);
                        foreach(PNMessageResult pnMessageResut in kvp.Value){
                            Debug.Log("Channel: " + pnMessageResut.Channel);
                            Debug.Log("payload: " + pnMessageResut.Payload.ToString());
                            Debug.Log("timetoken: " + pnMessageResut.Timetoken.ToString());
                        }
                    }
                }
            });
        }

        void DisplayHereNowResult(PNHereNowResult result){
            if(result.Channels!= null){
                foreach (KeyValuePair<string, PNHereNowChannelData> kvp in result.Channels){
                    Debug.Log ("in HereNow channel: " + kvp.Key);
                    if(kvp.Value != null){
                        PNHereNowChannelData hereNowChannelData = kvp.Value as PNHereNowChannelData;
                        if(hereNowChannelData != null){
                            Debug.Log ("in HereNow channelName: " + hereNowChannelData.ChannelName);
                            Debug.Log ("in HereNow channel occupancy: " + hereNowChannelData.Occupancy.ToString());
                            List<PNHereNowOccupantData> hereNowOccupantData = hereNowChannelData.Occupants as List<PNHereNowOccupantData>;
                            if(hereNowOccupantData != null){
                                foreach(PNHereNowOccupantData pnHereNowOccupantData in hereNowOccupantData){
                                    if(pnHereNowOccupantData.State != null){
                                        Debug.Log ("in HereNow channel State: " + pnHereNowOccupantData.State.ToString());
                                    }
                                    if(pnHereNowOccupantData.UUID != null){
                                        Debug.Log ("in HereNow channel UUID: " + pnHereNowOccupantData.UUID.ToString());
                                    }
                                }
                            } else {
                                Debug.Log ("in HereNow hereNowOccupantData null"); 
                            }
                        } else {
                            Debug.Log ("in HereNow hereNowChannelData null"); 
                        }
                    } else {
                        Debug.Log ("in HereNow kvp null"); 
                    }
                }
            } else {
                Debug.Log ("in HereNow channels null");    
            }
        }

        void PrintStatus(PNStatus s){
            if(s!=null){
                Debug.Log (string.Format ("PrintStatus: \n" + 
                    "category={0} \n" +
                    "errorData={1} \n" +
                    "error={2} \n" +
                    "statusCode={3} \n" +
                    "operation={4} \n" +
                    "tlsEnabled={5} \n" +
                    "uuid={6} \n" +
                    "authKey={7} \n" +
                    "origin={8} \n" +
                    "channels={9} \n" +
                    "channelGroups={10} \n" +
                    "clientRequest={11} \n" , 
                    s.Category.ToString(), 
                    ((s.ErrorData != null) && (s.ErrorData.Ex != null)) ? s.ErrorData.Info : "null", 
                    s.Error.ToString(),
                    s.StatusCode.ToString(),
                    s.Operation.ToString(),
                    s.TlsEnabled,
                    s.UUID,
                    s.AuthKey,
                    s.Origin,
                    (s.AffectedChannels != null) ? string.Join(",", s.AffectedChannels.ToArray()) : "null",
                    (s.AffectedChannelGroups != null) ? string.Join(",", s.AffectedChannelGroups.ToArray()) : "null",
                    (s.ClientRequest != null) ? s.ClientRequest.ToString() : "null"
                    ));
            }
        }

        void Display(string textToDisplay){
            stringToEdit = string.Format("{0}\n{1}", stringToEdit, textToDisplay);
            //TextScroll 
            //Canvas.
        }

        public string stringToEdit = "Hello World";
        void OnGUI() {
            stringToEdit = GUI.TextField(new Rect(10, 10, 600, 600), stringToEdit, 600);
        }

    	// Update is called once per frame
    	void Update () {
    	
    	}

        void OnApplicationQuit(){
            pubnub.CleanUp();
        }
    }
}
