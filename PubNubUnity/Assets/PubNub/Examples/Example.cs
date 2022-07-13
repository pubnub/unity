﻿using UnityEngine;
using System.Collections;
using PubNubAPI;
using System.Collections.Generic;
using System;
using System.Text;

namespace PubNubExample
{
    public class Example : MonoBehaviour {
        PubNub pubnub;
        List<string> listChannelGroups;
        List<string> listChannels;

        string cg1 = "channelGroup1";
        string cg2 = "channelGroup2";
        // string ch1 = "team_readonly.a8e9eff5-eed9-4d42-8686-1ecb6fe9916f";
        string ch1 = "channel1";
        string ch2 = "channel11";        
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
        UnityEngine.UI.Button ButtonMessageCounts;

        UnityEngine.UI.Button ButtonSignal;
        UnityEngine.UI.Button ButtonSendFile;
        UnityEngine.UI.Button ButtonDownloadFile;
        UnityEngine.UI.Button ButtonEncrypt;
        UnityEngine.UI.InputField InputFieldName;
        UnityEngine.UI.InputField InputFieldID;

        string deviceId = "ababababababababababababababababababababababababababababababababababababababababababababab";
        string cipherKey = "";
        string FileName = "";
        string FileID = "";
        PNPushType pnPushType = PNPushType.GCM;

        void Awake(){
            Application.runInBackground = true;
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
                    Debug.Log ("in UnsubscribeAll Error");
                    Debug.Log (string.Format("In Example, UnsubscribeAll Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("DateTime {0}, In UnsubscribeAll, result: {1}", DateTime.UtcNow, result.Message));
                }
            });
        }
        void ButtonDeleteHistoryHandler(){
            pubnub.DeleteMessages().Channel("channel1").Start(15078932998876451).End(15078933628583256).Async((result, status) => {
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
            pubnub.SetPresenceState().Channels(new List<string> (){ch1}).State(state).Async ((result, status) => {
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
            
            pubnub.History ().Channel("channel1").IncludeTimetoken(true).IncludeMeta(true).Async ((result, status) => {
                
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

        bool connected = false;
        void ButtonTimeHandler(){
            if(connected){
                connected = false;
            } else {
                connected = true;
            }
            Debug.Log("connected:" + connected);

            pubnub.Presence().Connected(connected).Channels(new List<string>{"hb1"}).Async ((result, status) => {
                Debug.Log("result:" + result);
            });

            pubnub.Time().Async ((result, status) => {
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
            Dictionary<string,string> dict = new Dictionary<string, string>();
            dict.Add("d","f");
            pubnub.HereNow().IncludeState(true).IncludeUUIDs(true).QueryParam(dict).Async((result, status) => {
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
            pubnub.WhereNow ().Uuid("some-other-uuid").Async ((result, status) => {
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
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add  ("k1", "v1");

            Dictionary<string, string> meta = new Dictionary<string, string>();
            meta.Add  ("k1", "v1");

            pubnub.Publish().Channel("channel1").Meta(meta).Message("Text with  emoji 🙀" + DateTime.Now.Ticks.ToString()).QueryParam(dict).Async((result, status) => {    
                    if(!status.Error){
                        Debug.Log (string.Format("DateTime {0}, In Publish Example, Timetoken: {1}", DateTime.UtcNow , result.Timetoken));
                        Display(string.Format("Published: {0}", result.Timetoken));
                    } else {
                        Debug.Log (status.Error);
                        Debug.Log (status.ErrorData.Info);
                    }

                });
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
            pubnub.AddPushNotificationsOnChannels().Channels(listChannels).DeviceID(deviceId).PushType(PNPushType.APNS2).Topic("a").Async((result, status) => {
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

        void ButtonMessageCountsHandler(){
            MessageCounts(listChannels, pubnub);
        }

        void ButtonSignalHandler(){
            pubnub.Signal().Channel("channel1").Message("test signal").Async((result, status) => {
                    if(status.Error){
                        PrintStatus(status);
                    } else {
                        Debug.Log (string.Format("DateTime {0}, In signal Example, Timetoken: {1}", DateTime.UtcNow , result.Timetoken));
                        Display(string.Format("Published: {0}", result.Timetoken));
                    }
                });
        }

        void SubscribeHandler(){
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add  ("k1", "v1");
            pubnub.SubscribeCallback += SubscribeCallbackHandler;

            pubnub.Subscribe().Channels(new List<string> (){ch1}).QueryParam(dict).Execute();
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
            ButtonMessageCounts = GameObject.Find("ButtonMessageCounts").GetComponent<UnityEngine.UI.Button>();
            ButtonMessageCounts.onClick.AddListener(ButtonMessageCountsHandler);
            ButtonSignal = GameObject.Find("ButtonSignal").GetComponent<UnityEngine.UI.Button>();
            ButtonSignal.onClick.AddListener(ButtonSignalHandler);

            ButtonSendFile = GameObject.Find("ButtonSendFile").GetComponent<UnityEngine.UI.Button>();
            ButtonSendFile.onClick.AddListener(ButtonSendFileHandler);
            ButtonDownloadFile = GameObject.Find("ButtonDownloadFile").GetComponent<UnityEngine.UI.Button>();
            ButtonDownloadFile.onClick.AddListener(ButtonDownloadFileHandler);
            ButtonEncrypt = GameObject.Find("ButtonEncrypt").GetComponent<UnityEngine.UI.Button>();
            ButtonEncrypt.onClick.AddListener(ButtonEncryptHandler);
            InputFieldName = GameObject.Find("InputFieldName").GetComponent<UnityEngine.UI.InputField>();
            InputFieldID = GameObject.Find("InputFieldID").GetComponent<UnityEngine.UI.InputField>();
        }

        void ButtonSendFileHandler(){
            string publishMessage = string.Format("publishMessage_{0}{1}", "id_", "constString");
            string filePath = Application.persistentDataPath + "/test.txt";
            FileName = InputFieldName.text.ToString();
            if(!string.IsNullOrEmpty(FileName)){
                pubnub.SendFile().CipherKey(this.cipherKey).Channel(ch1).Message(publishMessage).Name(FileName).FilePath(filePath).Async((result, status) => {
                    Debug.Log(result);
                    if(!status.Error){
                        Debug.Log("result.ID:" + result.Data.ID);
                        Debug.Log("result.Timetoken:" + result.Timetoken);
                    }

                });
            } else {
                Debug.Log("FileID or FileName empty");
            }

        }

        void ButtonDownloadFileHandler(){
            string savePath = string.Format("{0}/{1}", Application.persistentDataPath, "test.txt");
            FileID = InputFieldID.text.ToString();
            FileName = InputFieldName.text.ToString();            
            if(!string.IsNullOrEmpty(FileID) && !string.IsNullOrEmpty(FileName)){
                pubnub.DownloadFile().CipherKey(this.cipherKey).Channel(ch1).ID(FileID).Name(FileName).SavePath(savePath).Async((result, status) => {
                    byte[] save = System.IO.File.ReadAllBytes(savePath);
                    Debug.Log(Encoding.UTF8.GetString(save));

                });
            } else {
                Debug.Log("FileID or FileName empty");
            }
        }

        void ButtonEncryptHandler(){
            cipherKey = "enigma";
            Debug.Log("cipher key set");
        }

    	// Use this for initialization
    	void Start () {
            AddComponents();

            Init();
    	}

        void Init(){
            Debug.Log ("Starting");
            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.SubscribeKey = "demo";
            pnConfiguration.PublishKey = "demo";
            pnConfiguration.SecretKey = "demo";

            pnConfiguration.CipherKey = cipherKey;
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 120;    
            pnConfiguration.PresenceInterval= 60;
            pnConfiguration.HeartbeatNotificationOption = PNHeartbeatNotificationOption.All;

            //TODO: remove
            pnConfiguration.UserId = "PubNubUnityExample";
            Debug.Log ("PNConfiguration");  
            pubnub = new PubNub (pnConfiguration);

            listChannelGroups = new List<string> (){cg1, cg2};
            listChannels = new List<string> (){ch1, ch2};
            
        }

        void MessageCounts(List<string> listChannels, PubNub pubnub){
            pubnub.MessageCounts().Channels(listChannels).ChannelsTimetoken(new List<long>{1549982652}).Async((result, status) =>{    
                    if(status.Error){
                        Debug.Log (string.Format("In Example, MessageCounts Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    } else {
                        Debug.Log(string.Format("MessageCounts," + result.Channels.Count));
                        foreach(KeyValuePair<string, int> kvp in result.Channels){
                            Display(string.Format("MessageCounts:-> {0}:{1}", kvp.Key, kvp.Value));
                        }                        
                    }
                });
            pubnub.MessageCounts().Channels(listChannels).ChannelsTimetoken(new List<long>{1551795013294,155179501329433}).Async((result, status) =>{    
                    if(status.Error){
                        Debug.Log (string.Format("In Example, MessageCounts Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    } else {
                        Debug.Log(string.Format("MessageCounts," + result.Channels.Count));
                        foreach(KeyValuePair<string, int> kvp in result.Channels){
                            Display(string.Format("MessageCounts:-> {0}:{1}", kvp.Key, kvp.Value));
                        }                        
                    }
                });    
        }

        Dictionary<string, Dictionary<string, object>> messageList = new Dictionary<string, Dictionary<string, object>>();
 
        void SubscribeCallbackHandler2(object sender, EventArgs e) {
            SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;

            if (mea.Status != null) {
                Debug.Log("mea.Status: " + mea.Status);
                switch (mea.Status.Category) {
                    case PNStatusCategory.PNUnexpectedDisconnectCategory:
                    case PNStatusCategory.PNTimeoutCategory:
                        
                    break;
                }
            }
            if (mea.MessageResult != null) {
                Debug.Log("SubscribeCallback in message" + mea.MessageResult.Channel + mea.MessageResult.Payload);
            }
            if (mea.PresenceEventResult != null) {
                Debug.Log("SubscribeCallback in presence" + mea.PresenceEventResult.Channel + mea.PresenceEventResult.Occupancy + mea.PresenceEventResult.Event);
            }
        }

        void DisplayMessages(Dictionary<string, object> message, string channel) {
            UnityEngine.Debug.Log(channel);
        
            // if new message, add to list
            if (!messageList.ContainsKey(channel)) {
                Dictionary<string, object> messageIdAndVal = new Dictionary<string, object> ();
                messageIdAndVal.Add (message["message_id"].ToString(), message);
                messageList.Add(channel, messageIdAndVal);
                UnityEngine.Debug.Log (string.Format ("Add: {0}", message ["message_id"]));
            } else {
                bool deleted = (bool)message ["deleted"];
                if (deleted) {
                    // delete from message list
                    messageList.Remove(channel);
        
                    // update UI, remove message from display
                    // ...
                    UnityEngine.Debug.Log (string.Format ("Remove: {0}", message ["message_id"]));
                } else {
                    // replace content (same as adding new one, because of identical message_id)
                    Dictionary<string, object> messageIdAndVal = new Dictionary<string, object> ();
                    messageIdAndVal.Add (message ["message_id"].ToString(), message);
        
                    messageList[channel] = messageIdAndVal;
                    // update UI, update display content
                    // ...
                    UnityEngine.Debug.Log (string.Format ("Update: {0}", message ["message_id"]));
                }
            }
        }


        void FetchRecursive(long start, List<string> listChannels){
            
            pubnub.FetchMessages().Channels(listChannels).Start(start).Async ((result, status) => {
                if(status.Error){
                    Debug.Log (string.Format("In Example, FetchMessages Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("In FetchMessages, result: "));
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

        void GetHistoryRecursive(long start, string channel){   
            pubnub.History ().Channel(channel).Start(start).Reverse(true).IncludeTimetoken(true).Async ((result, status) => {
                
                if(status.Error){
                    Debug.Log (string.Format("In Example, History Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    if((result.Messages!=null) && (result.Messages.Count>0)){
                        foreach (PNHistoryItemResult histItem in result.Messages){
                            Debug.Log(string.Format("histItem: {0}, {1}", histItem.Entry.ToString(), histItem.Timetoken.ToString()));
                        }
                        GetHistoryRecursive(result.EndTimetoken, channel);
                    }
                }
            });
        }

        void SubscribeCallbackHandler(object sender, EventArgs e){
            Debug.Log("SubscribeCallbackHandler Event handler");
            SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;

            if(mea.Status != null){
                switch (mea.Status.Category){
                    case PNStatusCategory.PNConnectedCategory:
                    PrintStatus(mea.Status);
                    break;
                    case PNStatusCategory.PNUnexpectedDisconnectCategory:
                    case PNStatusCategory.PNTimeoutCategory:
                    pubnub.Reconnect();
                    pubnub.CleanUp();
                    break;
                }
            } else {
                Debug.Log("mea.Status null" + e.GetType().ToString() + mea.GetType().ToString());
            }
            if(mea.MessageResult != null){
                Debug.Log ("In Example, SubscribeCallback in message" + mea.MessageResult.Channel + mea.MessageResult.Payload);
                Display(string.Format("SubscribeCallback Result: {0}", pubnub.JsonLibrary.SerializeToJsonString(mea.MessageResult.Payload)));
            }
            if(mea.PresenceEventResult != null){
                Debug.Log ("In Example, SubscribeCallback in presence" + mea.PresenceEventResult.Channel + mea.PresenceEventResult.Occupancy + mea.PresenceEventResult.Event + mea.PresenceEventResult.State);
            }
            if(mea.SignalEventResult != null){
                Debug.Log ("In Example, SubscribeCallback in SignalEventResult" + mea.SignalEventResult.Channel + mea.SignalEventResult.Payload);
                Display(string.Format("SubscribeCallback SignalEventResult: {0}", pubnub.JsonLibrary.SerializeToJsonString(mea.SignalEventResult.Payload)));
            }
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
            pubnub.RemovePushNotificationsFromChannels().Channels(listChannels).DeviceID(deviceId).PushType(PNPushType.APNS2).Topic("a").Async((result, status) => {
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
            pubnub.AuditPushChannelProvisions().DeviceID(deviceId).PushType(pnPushType).PushType(PNPushType.APNS2).Topic("a").Async((result, status) => {
                    Debug.Log ("in AuditPushChannelProvisions");
                    if(status.Error){
                        Debug.Log (string.Format("In Example, AuditPushChannelProvisions Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    } else {
                        Debug.Log (string.Format("DateTime {0}, In AuditPushChannelProvisions, result: {1}", DateTime.UtcNow, (result.Channels!=null)?string.Join(",", result.Channels.ToArray()):""));
                        Display(string.Format("AuditPushChannelProvisions: {0}", (result.Channels!=null)?string.Join(",", result.Channels.ToArray()):""));
                    }

                });                
                
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
            pubnub.RemoveAllPushNotifications().DeviceID(deviceId).PushType(PNPushType.APNS2).Topic("a").Async((result, status) => {
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
            pubnub.FetchMessages().Channels(new List<string>(){ch1}).IncludeMeta(true).Async ((result, status) => {    
                if(status.Error){
                    Debug.Log (string.Format("In Example, FetchMessages Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("In FetchMessages, result: "));//,result.EndTimetoken, result.Messages[0].ToString()));
                    foreach(KeyValuePair<string, List<PNMessageResult>> kvp in result.Channels){
                        Debug.Log("kvp channelname" + kvp.Key);
                        int count = 0;
                        foreach(PNMessageResult pnMessageResut in kvp.Value){
                            count++;
                            try{
                                
                                Debug.Log("Channel: " + pnMessageResut.Channel);
                                Debug.Log("Timetoken: " + pnMessageResut.Timetoken.ToString());
                                Debug.Log("Payload: " + pnMessageResut.Payload.ToString());
                                
                                Display("Payload: " + pnMessageResut.Payload.ToString());
                            }catch (Exception ex){
                                Debug.Log("payload: " + pnMessageResut.Payload.ToString());
                                Debug.Log("ex: " + ex.ToString());
                            }
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
                            StringBuilder sb = new StringBuilder();
                            sb.Append("in HereNow channelName: " + hereNowChannelData.ChannelName);
                            Display(string.Format("channelName: {0}", hereNowChannelData.ChannelName));
                            sb.Append("in HereNow channel occupancy: " + hereNowChannelData.Occupancy.ToString());
                            Display(string.Format("channelName: {0}", hereNowChannelData.Occupancy));
                            List<PNHereNowOccupantData> hereNowOccupantData = hereNowChannelData.Occupants as List<PNHereNowOccupantData>;
                            Display(string.Format("hereNowOccupantData: {0}", hereNowOccupantData.Count));
                            if(hereNowOccupantData != null){
                                foreach(PNHereNowOccupantData pnHereNowOccupantData in hereNowOccupantData){
                                    if(pnHereNowOccupantData.State != null){
                                        sb.Append ("in HereNow channel State: " + pnHereNowOccupantData.State.ToString());
                                        Dictionary<string, object> state = pnHereNowOccupantData.State as Dictionary<string, object>;
                                        foreach (KeyValuePair<string, object> kvpState in state){
                                            sb.Append (kvp.Key);
                                            sb.Append ("=====>");
                                            sb.Append (kvp.Value.ToString());
                                        }
                                        Display(string.Format("State: {0}", pnHereNowOccupantData.State.ToString()));
                                    }
                                    if(pnHereNowOccupantData.UUID != null){
                                        sb.Append ("in HereNow channel UUID: " + pnHereNowOccupantData.UUID.ToString());
                                        Display(string.Format("UUID: {0}", pnHereNowOccupantData.UUID.ToString()));
                                    }
                                }
                            } else {
                                sb.Append ("in HereNow hereNowOccupantData null"); 
                            }
                            Debug.Log(sb.ToString());
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
            TextContent.text  = string.Format("{0}\n{1}", TextContent.text, textToDisplay);
        }

        void OnGUI() {
            
        }

    	// Update is called once per frame
    	void Update () {
    	
    	}

        void OnApplicationQuit(){
            pubnub.CleanUp();
        }
    }
}
