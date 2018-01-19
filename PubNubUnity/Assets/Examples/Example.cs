using UnityEngine;
using System.Collections;
using PubNubAPI;
using System.Collections.Generic;
using System;

namespace PubNubExample
{
    public class Example : MonoBehaviour {
        PubNub pubnub;
        List<string> listChannelGroups;
        List<string> listChannels;

        string cg1 = "channelGroup1";
        string cg2 = "channelGroup2";
        string ch1 = "channel1";
        string ch2 = "channel2";        
        UnityEngine.UI.Text TextContent;
        UnityEngine.UI.Button ButtonClear;
        UnityEngine.UI.Button ButtonReset;
        UnityEngine.UI.Button ButtonSubscribe;
        UnityEngine.UI.Button ButtonPublishPost;
        UnityEngine.UI.Button ButtonPublish;
        UnityEngine.UI.Button ButtonFire;
        UnityEngine.UI.Button ButtonWhereNow;
        UnityEngine.UI.Button ButtonHereNow;
        UnityEngine.UI.Button ButtonTime;
        UnityEngine.UI.Button ButtonHistory;
        UnityEngine.UI.Button ButtonGetPresenceState;
        UnityEngine.UI.Button ButtonSetPresenceState;
        UnityEngine.UI.Button ButtonUnsubscribe;
        UnityEngine.UI.Button ButtonFetch;
        UnityEngine.UI.Button ButtonDeleteHistory;
        UnityEngine.UI.Button ButtonUnsubscribeAll;
        UnityEngine.UI.Button ButtonCleanUp;
        UnityEngine.UI.Button ButtonReconnect;
        UnityEngine.UI.Button ButtonAddChannelsToChannelGroup;
        UnityEngine.UI.Button ButtonDeleteChannelGroup;
        UnityEngine.UI.Button ButtonListChannelsForChannelGroup;
        UnityEngine.UI.Button ButtonRemoveChannelsFromChannelGroup;
        UnityEngine.UI.Button ButtonAddPushNotificationsOnChannels;
        UnityEngine.UI.Button ButtonAuditPushChannelProvisions;
        UnityEngine.UI.Button ButtonRemoveAllPushNotifications;
        UnityEngine.UI.Button ButtonRemovePushNotificationsFromChannels;

        string deviceId = "aaa";
        PNPushType pnPushType = PNPushType.GCM;

        void Awake(){
            
        }

        void ButtonClearHandler(){
            TextContent.text = "";
        }

        void ResetHandler(){
            Debug.Log ("in ButtonReconnect");
            if(pubnub!=null){
                ButtonCleanUpHandler();                
            }
            ButtonClearHandler();
            Init();
        }

        void ButtonReconnectHandler(){
            Debug.Log ("in ButtonReconnect");
            pubnub.Reconnect();
        }
        void ButtonCleanUpHandler(){
            Debug.Log ("in ButtonCleanUp");
            pubnub.CleanUp();
        }
        void ButtonUnsubscribeAllHandler(){
            pubnub.UnsubscribeAll().Async((result, status) => {
                Debug.Log ("in UnsubscribeAll");
                if(status.Error){
                    Debug.Log (string.Format("In Example, UnsubscribeAll Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("DateTime {0}, In UnsubscribeAll, result: {1}", DateTime.UtcNow, result.Message));
                }
            });
        }
        void ButtonDeleteHistoryHandler(){
            pubnub.DeleteMessages().Channel("channel1").Start(15078932998876451).End(15078933628583256).Async((result, status) => {
            //pubnub.DeleteMessages().Channel("channel1").Async((result, status) => {                
                Debug.Log ("in DeleteMessages");
                if(!status.Error){
                    Debug.Log (string.Format("DateTime {0}, In DeleteMessages Example, Timetoken: {1}", DateTime.UtcNow , result.Message));
                    Display(string.Format("result.Message: ", result.Message));
                    Display(string.Format("deleted"));
                } else {
                    Debug.Log (status.Error);
                    Debug.Log (status.StatusCode);
                    Debug.Log (status.ErrorData.Info);
                    
                }

            });
        }
        void ButtonFetchHandler(){
            FetchMessages(pubnub, listChannels);
        }
        void ButtonUnsubscribeHandler(){
            pubnub.Unsubscribe().ChannelGroups(listChannelGroups).Channels(listChannels).Async((result, status) => {
                Debug.Log ("in Unsubscribe");
                if(status.Error){
                    Debug.Log (string.Format("In Example, Unsubscribe Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("DateTime {0}, In Unsubscribe, result: {1}", DateTime.UtcNow, result.Message));
                }
            });

        }

        void ShowStateResult(Dictionary<string, object> dict){
            if(dict!= null){
                foreach (KeyValuePair<string, object> key in dict){
                    Display(string.Format("Channel:{0}, State:{1}", key.Key, pubnub.JsonLibrary.SerializeToJsonString(key.Value)));
                }
            }
        }
        void ButtonSetPresenceStateHandler(){
            Dictionary<string, object> state = new Dictionary<string, object>();
            state.Add  ("k1", "v1");
            pubnub.SetPresenceState().Channels(listChannels).ChannelGroups(listChannelGroups).State(state).Async ((result, status) => {
                if(status.Error){
                    Debug.Log (string.Format("In Example, SetPresenceState Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("DateTime {0}, In Example SetPresenceState, result:", DateTime.UtcNow));
                    if(result != null){
                        ShowStateResult(result.StateByChannels);
                    }
                }
            });
        }
        void ButtonGetPresenceStateHandler(){
            pubnub.GetPresenceState().Channels(listChannels).ChannelGroups(listChannelGroups).Async ((result, status) => {
                
                if(status.Error){
                    Debug.Log (string.Format("In Example, GetPresenceState Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("DateTime {0}, In Example GetPresenceState, result:", DateTime.UtcNow));
                    if(result != null){
                        ShowStateResult(result.StateByChannels);
                    }
                }
            });
        }
        void ButtonHistoryHandler(){
            pubnub.History ().Channel("channel1").IncludeTimetoken(true).Async ((result, status) => {
                
                if(status.Error){
                    Debug.Log (string.Format("In Example, History Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    foreach (PNHistoryItemResult histItem in result.Messages){
                        Debug.Log(string.Format("histItem: {0}, {1}", histItem.Entry.ToString(), histItem.Timetoken.ToString()));
                        Display(string.Format("histItem: {0}, {1}", histItem.Entry.ToString(), histItem.Timetoken.ToString()));
                    }
                }
            });
        }
        void ButtonTimeHandler(){
            pubnub.Time ().Async ((result, status) => {
                
                if(status.Error){
                    PrintStatus(status);
                    Debug.Log (string.Format("In Example, Time Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("DateTime {0}, In Example, result: {1}", DateTime.UtcNow ,result.TimeToken));
                    Display(string.Format("Time Result: {0}", result.TimeToken.ToString()));
                }
            });
        }
        void ButtonHereNowHandler(){
            pubnub.HereNow().Channels(listChannels).ChannelGroups(listChannelGroups).IncludeState(true).IncludeUUIDs(true).Async((result, status) => {
                    Debug.Log ("in HereNow1");
                    if(status.Error){
                        PrintStatus(status);
                        Debug.Log (string.Format("In Example, Time Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    } else {                
                        Debug.Log (string.Format("DateTime {0}, In Example, Channels: {1} {2}", DateTime.UtcNow , result.TotalChannels, result.TotalOccupancy));
                        DisplayHereNowResult(result);
                    }
                    
                    Debug.Log (status.Error);

                });
        }
        
        void ButtonWhereNowHandler(){
            pubnub.WhereNow ().Async ((result, status) => {
                Debug.Log ("in WhereNow");
                if(status.Error){
                    PrintStatus(status);
                } else {
                    Debug.Log (string.Format("DateTime {0}, In Example, Channels: {1}", DateTime.UtcNow , (result.Channels!=null)?string.Join(",",result.Channels.ToArray()):""));
                    Display(string.Format("WhereNow: {0}", (result.Channels!=null)?string.Join(",",result.Channels.ToArray()):""));
                }
             });
        }
        void ButtonFireHandler(){
            pubnub.Fire().Channel("channel1").Message("test fire message" + DateTime.Now.Ticks.ToString()).Async((result, status) => {
                    Debug.Log ("in Fire");
                    if(status.Error){
                        PrintStatus(status);
                    } else {
                        Debug.Log (string.Format("DateTime {0}, In Fire Example, Timetoken: {1}", DateTime.UtcNow , result.Timetoken));
                        Display(string.Format("Published: {0}", result.Timetoken));
                    }
                });
        }
        void ButtonPublishHandler(){
            //for(int i =0; i<1000; i++){
            //pubnub.Publish().Channel("channel1").Message("test message" +i+ " " + DateTime.Now.Ticks.ToString()).Async((result, status) => {
            pubnub.Publish().Channel("channel1").Message("test message" + DateTime.Now.Ticks.ToString()).Async((result, status) => {    
                    Debug.Log ("in Publish");
                    if(!status.Error){
                        Debug.Log (string.Format("DateTime {0}, In Publish Example, Timetoken: {1}", DateTime.UtcNow , result.Timetoken));
                        Display(string.Format("Published: {0}", result.Timetoken));
                    } else {
                        Debug.Log (status.Error);
                        Debug.Log (status.ErrorData.Info);
                    }

                });
            //}
        }
        void ButtonPublishPostHandler(){
            pubnub.Publish().Channel("channel1").Message("test message").UsePost(true).Async((result, status) => {
                    Debug.Log ("in Publish");
                    if(!status.Error){
                        Debug.Log (string.Format("DateTime {0}, In Publish Example, Timetoken: {1}", DateTime.UtcNow , result.Timetoken));
                        Display(string.Format("Published: {0}", result.Timetoken));
                    } else {
                        Debug.Log (status.Error);
                        Debug.Log (status.ErrorData.Info);
                    }

                });
        }

        void ButtonAddChannelsToChannelGroupHandler(){
            pubnub.AddChannelsToChannelGroup().Channels(listChannels).ChannelGroup(cg1).Async((result, status) => {
                Debug.Log ("in AddChannelsToChannelGroup");
                if(status.Error){
                    Debug.Log (string.Format("In Example, AddChannelsToChannelGroup Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("DateTime {0}, In Example AddChannelsToChannelGroup, result: {1}", DateTime.UtcNow, result.Message));
                    Display(string.Format("AddChannelsToChannelGroup: {0}", result.Message));
                }
            });
        }

        void ButtonDeleteChannelGroupHandler(){
            pubnub.DeleteChannelGroup().ChannelGroup(cg1).Async((result1, status1) => {
                if(status1.Error){
                    Debug.Log (string.Format("In Example, DeleteChannelsFromChannelGroup Error: {0} {1} {2}", status1.StatusCode, status1.ErrorData, status1.Category));
                } else {
                    Debug.Log (string.Format("DateTime {0}, In Example DeleteChannelsFromChannelGroup, result: {1}", DateTime.UtcNow, result1.Message));
                    Display(string.Format("DeleteChannelsFromChannelGroup: {0}", result1.Message));
                }
            
            });  
        }

        void ButtonRemoveChannelsFromChannelGroupHandler(){
            List<string> listChannelsRemove = new List<string> (){ch1};
            RemoveChannelsFromCG(pubnub, cg1, listChannelsRemove);
        }
        void ButtonListChannelsForChannelGroupHandler(){
             ListAllChannelsOfGroup(pubnub, cg1);
        }
        void ButtonAddPushNotificationsOnChannelsHandler(){
            pubnub.AddPushNotificationsOnChannels().Channels(listChannels).DeviceID(deviceId).PushType(pnPushType).Async((result, status) => {
                    Debug.Log ("in AddPushNotificationsOnChannels");
                    if(status.Error){
                        Debug.Log (string.Format("In Example, AddPushNotificationsOnChannels Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    } else {
                        Debug.Log (string.Format("DateTime {0}, In AddPushNotificationsOnChannels, result: {1}", DateTime.UtcNow, result.Message));
                        Display(string.Format("AddPushNotificationsOnChannels: {0}", result.Message));
                    }
                });
        }
        void ButtonAuditPushChannelProvisionsHandler(){
            AuditPushChannelProvisions(pubnub, deviceId, pnPushType);
        }

        void ButtonRemoveAllPushNotificationsHandler(){
            RemoveAllPushNotificationsFromChannels(pubnub, deviceId, pnPushType);
        }

        void ButtonRemovePushNotificationsFromChannelsHandler(){
            RemoveChannelsFromPush(listChannels, pubnub, deviceId, pnPushType);
        }

        void SubscribeHandler(){
            pubnub.Subscribe ().Channels(new List<string> (){ch1}).WithPresence().Execute();
        }

        void AddComponents(){
            TextContent = GameObject.Find("TextContent").GetComponent<UnityEngine.UI.Text>();
            ButtonClear = GameObject.Find("ButtonClear").GetComponent<UnityEngine.UI.Button>();
            ButtonClear.onClick.AddListener(ButtonClearHandler);
            ButtonReset = GameObject.Find("ButtonReset").GetComponent<UnityEngine.UI.Button>();
            ButtonReset.onClick.AddListener(ResetHandler);
            ButtonSubscribe = GameObject.Find("ButtonSubscribe").GetComponent<UnityEngine.UI.Button>();
            ButtonSubscribe.onClick.AddListener(SubscribeHandler);
            ButtonPublishPost = GameObject.Find("ButtonPublishPost").GetComponent<UnityEngine.UI.Button>();
            ButtonPublishPost.onClick.AddListener(ButtonPublishPostHandler);
            ButtonPublish = GameObject.Find("ButtonPublish").GetComponent<UnityEngine.UI.Button>();
            ButtonPublish.onClick.AddListener(ButtonPublishHandler);

            ButtonFire = GameObject.Find("ButtonFire").GetComponent<UnityEngine.UI.Button>();
            ButtonFire.onClick.AddListener(ButtonFireHandler);
            ButtonWhereNow = GameObject.Find("ButtonWhereNow").GetComponent<UnityEngine.UI.Button>();
            ButtonWhereNow.onClick.AddListener(ButtonWhereNowHandler);
            ButtonHereNow = GameObject.Find("ButtonHereNow").GetComponent<UnityEngine.UI.Button>();
            ButtonHereNow.onClick.AddListener(ButtonHereNowHandler);
            ButtonTime = GameObject.Find("ButtonTime").GetComponent<UnityEngine.UI.Button>();
            ButtonTime.onClick.AddListener(ButtonTimeHandler);
            ButtonHistory = GameObject.Find("ButtonHistory").GetComponent<UnityEngine.UI.Button>();
            ButtonHistory.onClick.AddListener(ButtonHistoryHandler);
            ButtonGetPresenceState = GameObject.Find("ButtonGetPresenceState").GetComponent<UnityEngine.UI.Button>();
            ButtonGetPresenceState.onClick.AddListener(ButtonGetPresenceStateHandler);
            ButtonSetPresenceState = GameObject.Find("ButtonSetPresenceState").GetComponent<UnityEngine.UI.Button>();
            ButtonSetPresenceState.onClick.AddListener(ButtonSetPresenceStateHandler);
            ButtonUnsubscribe = GameObject.Find("ButtonUnsubscribe").GetComponent<UnityEngine.UI.Button>();
            ButtonUnsubscribe.onClick.AddListener(ButtonUnsubscribeHandler);
            ButtonFetch = GameObject.Find("ButtonFetch").GetComponent<UnityEngine.UI.Button>();
            ButtonFetch.onClick.AddListener(ButtonFetchHandler);
            ButtonDeleteHistory = GameObject.Find("ButtonDeleteHistory").GetComponent<UnityEngine.UI.Button>();
            ButtonDeleteHistory.onClick.AddListener(ButtonDeleteHistoryHandler);
            ButtonUnsubscribeAll = GameObject.Find("ButtonUnsubscribeAll").GetComponent<UnityEngine.UI.Button>();
            ButtonUnsubscribeAll.onClick.AddListener(ButtonUnsubscribeAllHandler);
            ButtonCleanUp = GameObject.Find("ButtonCleanUp").GetComponent<UnityEngine.UI.Button>();
            ButtonCleanUp.onClick.AddListener(ButtonCleanUpHandler);
            ButtonReconnect = GameObject.Find("ButtonReconnect").GetComponent<UnityEngine.UI.Button>();
            ButtonReconnect.onClick.AddListener(ButtonReconnectHandler);
            ButtonAddChannelsToChannelGroup = GameObject.Find("ButtonAddChannelsToChannelGroup").GetComponent<UnityEngine.UI.Button>();
            ButtonAddChannelsToChannelGroup.onClick.AddListener(ButtonAddChannelsToChannelGroupHandler);
            ButtonDeleteChannelGroup = GameObject.Find("ButtonDeleteChannelGroup").GetComponent<UnityEngine.UI.Button>();
            ButtonDeleteChannelGroup.onClick.AddListener(ButtonDeleteChannelGroupHandler);
            ButtonRemoveChannelsFromChannelGroup = GameObject.Find("ButtonRemoveChannelsFromChannelGroup").GetComponent<UnityEngine.UI.Button>();
            ButtonRemoveChannelsFromChannelGroup.onClick.AddListener(ButtonRemoveChannelsFromChannelGroupHandler);
            ButtonListChannelsForChannelGroup = GameObject.Find("ButtonListChannelsForChannelGroup").GetComponent<UnityEngine.UI.Button>();
            ButtonListChannelsForChannelGroup.onClick.AddListener(ButtonListChannelsForChannelGroupHandler);
            ButtonAddPushNotificationsOnChannels = GameObject.Find("ButtonAddPushNotificationsOnChannels").GetComponent<UnityEngine.UI.Button>();
            ButtonAddPushNotificationsOnChannels.onClick.AddListener(ButtonAddPushNotificationsOnChannelsHandler);
            ButtonAuditPushChannelProvisions = GameObject.Find("ButtonAuditPushChannelProvisions").GetComponent<UnityEngine.UI.Button>();
            ButtonAuditPushChannelProvisions.onClick.AddListener(ButtonAuditPushChannelProvisionsHandler);
            ButtonRemoveAllPushNotifications = GameObject.Find("ButtonRemoveAllPushNotifications").GetComponent<UnityEngine.UI.Button>();
            ButtonRemoveAllPushNotifications.onClick.AddListener(ButtonRemoveAllPushNotificationsHandler);
            ButtonRemovePushNotificationsFromChannels = GameObject.Find("ButtonRemovePushNotificationsFromChannels").GetComponent<UnityEngine.UI.Button>();
            ButtonRemovePushNotificationsFromChannels.onClick.AddListener(ButtonRemovePushNotificationsFromChannelsHandler);
        }

    	// Use this for initialization
    	void Start () {
            AddComponents();

            Init();
    	}

        void Init(){
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

            listChannelGroups = new List<string> (){cg1, cg2};
            listChannels = new List<string> (){ch1, ch2};
            
            /*Dictionary<string, object> state = new Dictionary<string, object>();
            state.Add  ("k1", "v1");
            pubnub.SetPresenceState().ChannelGroups(listChannelGroups).Channels(listChannels).State(state).Async ((result, status) => {
                if(status.Error){
                    Debug.Log (string.Format("In Example, SetPresenceState Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("DateTime {0}, In Example SetPresenceState, result:", DateTime.UtcNow));
                }
            });*/

            pubnub.SusbcribeCallback += (sender, e) => { //; //+= (pnStatus, pnMessageResut, pnPresenceEventResult) => {
                SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;

                Debug.Log ("In Example, SusbcribeCallback");
                if(mea.Status != null){
                    PrintStatus(mea.Status);
                    if(mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
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
                if(mea.MessageResult != null){
                    Debug.Log ("In Example, SusbcribeCallback in message" + mea.MessageResult.Channel + mea.MessageResult.Payload);
                    Display(string.Format("SusbcribeCallback Result: {0}", pubnub.JsonLibrary.SerializeToJsonString(mea.MessageResult.Payload)));
                }
                if(mea.PresenceEventResult != null){
                    Debug.Log ("In Example, SusbcribeCallback in presence" + mea.PresenceEventResult.Channel + mea.PresenceEventResult.Occupancy + mea.PresenceEventResult.Event);
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

            };

            //Debug.Log ("PubNub");
            

            Debug.Log ("before Time");
            /*pubnub.Time ().Async (new PNTimeCallback<PNTimeResult>(
                (r, s) => {
                    Debug.Log ("in Time");
                }
            ));*/
           

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
            pubnub.RemovePushNotificationsFromChannels().Channels(listChannels).DeviceID(deviceId).PushType(pnPushType).Async((result, status) => {
                    Debug.Log ("in RemovePushNotificationsFromChannels");
                    if(status.Error){
                        Debug.Log (string.Format("In Example, RemovePushNotificationsFromChannels Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    } else {
                        Debug.Log (string.Format("DateTime {0}, In RemovePushNotificationsFromChannels, result: {1}", DateTime.UtcNow, result.Message));
                        Display(string.Format("RemovePushNotificationsFromChannels: {0}", result.Message));
                    }
                });
        }

        void AuditPushChannelProvisions(PubNub pubnub, string deviceId, PNPushType pnPushType){
            pubnub.AuditPushChannelProvisions().DeviceID(deviceId).PushType(pnPushType).Async((result, status) => {
                    Debug.Log ("in AuditPushChannelProvisions");
                    if(status.Error){
                        Debug.Log (string.Format("In Example, AuditPushChannelProvisions Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    } else {
                        Debug.Log (string.Format("DateTime {0}, In AuditPushChannelProvisions, result: {1}", DateTime.UtcNow, (result.Channels!=null)?string.Join(",", result.Channels.ToArray()):""));
                        Display(string.Format("AuditPushChannelProvisions: {0}", (result.Channels!=null)?string.Join(",", result.Channels.ToArray()):""));
                    }

                });
                
                //RemoveAllPushNotificationsFromChannels(pubnub, deviceId, pnPushType);
        }

        void RemoveAllPushNotificationsFromChannels(PubNub pubnub, string deviceId, PNPushType pnPushType){
            pubnub.RemoveAllPushNotifications().DeviceID(deviceId).PushType(pnPushType).Async((result, status) => {
                    Debug.Log ("in RemoveAllPushNotificationsFromChannels");
                    if(status.Error){
                        Debug.Log (string.Format("In Example, RemoveAllPushNotificationsFromChannels Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    } else {
                        Debug.Log (string.Format("DateTime {0}, In RemoveAllPushNotificationsFromChannels, result: {1}", DateTime.UtcNow, result.Message));
                        Display(string.Format("RemoveAllPushNotificationsFromChannels: {0}", result.Message));
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
                        Display(string.Format("RemoveChannelsFromChannelGroup: {0}", result.Message));
                    }

                });
        }

        void ListAllChannelsOfGroup(PubNub pubnub, string cg){
            pubnub.ListChannelsForChannelGroup().ChannelGroup(cg).Async((result, status) => {
                    if(status.Error){
                        Debug.Log (string.Format("In Example, ListAllChannelsOfGroup Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    } else {
                        Debug.Log (string.Format("In Example ListAllChannelsOfGroup, result: {0}", (result.Channels!=null)?string.Join(",", result.Channels.ToArray()):""));
                        Display(string.Format("ListChannelsForChannelGroup: {0}", (result.Channels!=null)?string.Join(",", result.Channels.ToArray()):""));
                    }
                
                });
        }

        void FetchMessages(PubNub pubnub, List<string> listChannels){
            pubnub.FetchMessages().Channels(listChannels).Async ((result, status) => {
            //pubnub.FetchMessages().Channels(new List<string>{"channel2"}).Async ((result, status) => {    
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
                            Display(string.Format("Channel {0}, payload {1}, timetoken {2}", pnMessageResut.Channel, pnMessageResut.Payload.ToString(), pnMessageResut.Timetoken.ToString()));
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
                            Display(string.Format("channelName: {0}", hereNowChannelData.ChannelName));
                            Debug.Log ("in HereNow channel occupancy: " + hereNowChannelData.Occupancy.ToString());
                            Display(string.Format("channelName: {0}", hereNowChannelData.Occupancy));
                            List<PNHereNowOccupantData> hereNowOccupantData = hereNowChannelData.Occupants as List<PNHereNowOccupantData>;
                            if(hereNowOccupantData != null){
                                foreach(PNHereNowOccupantData pnHereNowOccupantData in hereNowOccupantData){
                                    if(pnHereNowOccupantData.State != null){
                                        Debug.Log ("in HereNow channel State: " + pnHereNowOccupantData.State.ToString());
                                        Display(string.Format("State: {0}", pnHereNowOccupantData.State.ToString()));
                                    }
                                    if(pnHereNowOccupantData.UUID != null){
                                        Debug.Log ("in HereNow channel UUID: " + pnHereNowOccupantData.UUID.ToString());
                                        Display(string.Format("UUID: {0}", pnHereNowOccupantData.UUID.ToString()));
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

                    //Display (s.Category.ToString());
            }
        }

        void Display(string textToDisplay){
            TextContent.text  = string.Format("{0}\n{1}", TextContent.text, textToDisplay);
            //UnityEngine.UI.Text txtRef = (UnityEngine.UI.Text)GameObject.Find("CountText").GetComponent<Text>;
            //Debug.Log("TextContent.text 2:" + TextContent.text);
            //TextScroll 
            //Canvas.
            //Debug.Log("transform:" + transform.Find("TextContent").name);
        }

        //public string stringToEdit = "Hello World";
        void OnGUI() {
            //stringToEdit = GUI.TextField(new Rect(10, 10, 600, 600), stringToEdit, 600);
            
        }

    	// Update is called once per frame
    	void Update () {
    	
    	}

        void OnApplicationQuit(){
            pubnub.CleanUp();
        }
    }
}
