using UnityEngine;
using System.Collections;
using PubNubAPI;
using System.Collections.Generic;
using System;

namespace PubNubExample
{
    public class Example : MonoBehaviour {

    	// Use this for initialization
    	void Start () {
            Debug.Log ("Starting");
            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.SubscribeKey = "demo-36";
            pnConfiguration.PublishKey = "demo-36";
            pnConfiguration.Secure = true;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 

            //TODO: remove
            pnConfiguration.UUID = "a";
            Debug.Log ("PNConfiguration");  
            PubNub pubnub = new PubNub (pnConfiguration);

            pubnub.AddListener (
                (s) => {
                    
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
            List<string> listChannelGroups = new List<string> (){"channelGroup1", "channelGroup2"};
            listChannelGroups.Add ("channelGroup1");
            listChannelGroups.Add ("channelGroup2");
            List<string> listChannels = new List<string> (){"channel1", "channel2"};
            listChannels.Add ("channel1");


            pubnub.SusbcribeCallback += (sender, e) => { //; //+= (pnStatus, pnMessageResut, pnPresenceEventResult) => {
                SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;

                Debug.Log ("In Example, SusbcribeCallback");
                if(mea.pnStatus != null){
                    //Debug.Log ("SusbcribeCallback in status" + String.Join(", ", mea.pnStatus.AffectedChannelGroups.ToArray()) + String.Join(", ", mea.pnStatus.AffectedChannels.ToArray()));
                }
                if(mea.pnmr != null){
                    Debug.Log ("In Example, SusbcribeCallback in message" + mea.pnmr.Channel + mea.pnmr.Payload);
                }
                if(mea.pnper != null){
                    Debug.Log ("In Example, SusbcribeCallback in presence" + mea.pnper.Channel + mea.pnper.Occupancy + mea.pnper.Event);
                }
                /*pubnub.WhereNow ().Async ((result, status) => {
                    Debug.Log ("in WhereNow");
                    Debug.Log (string.Format("DateTime {0}, In Example, Channels: {1}", DateTime.Now , string.Join(",",result.Channels.ToArray())));
                    Debug.Log (status.Error);

                });
                pubnub.Publish().Channel("channel1").Message("test message").Async((result, status) => {
                    Debug.Log ("in Publish");
                    Debug.Log (string.Format("DateTime {0}, In Publish Example, Timetoken: {1}", DateTime.Now , result.Timetoken));
                    Debug.Log (status.Error);

                });*/
                //herenow
                pubnub.HereNow().Channels(listChannels).ChannelGroups(listChannelGroups).IncludeState(true).IncludeUUIDs(true).Async((result, status) => {
                    Debug.Log ("in HereNow1");
                    Debug.Log (string.Format("DateTime {0}, In Example, Channels: {1} {2}", DateTime.Now , result.TotalChannels, result.TotalOccupancy));
                    DisplayHereNowResult(result);
                    
                    Debug.Log (status.Error);

                });
                //globalherenow
                pubnub.HereNow().IncludeState(true).IncludeUUIDs(true).Async((result, status) => {
                    Debug.Log ("in HereNow2");
                    
                    Debug.Log (string.Format("DateTime {0}, In Example, Channels: {1} {2}", DateTime.Now , result.TotalChannels, result.TotalOccupancy));
                    DisplayHereNowResult(result);
                    Debug.Log (status.Error);

                });

                pubnub.HereNow().IncludeState(false).IncludeUUIDs(true).Async((result, status) => {
                    Debug.Log ("in HereNow3");
                    Debug.Log (string.Format("DateTime {0}, In Example, Channels: {1} {2}", DateTime.Now , result.TotalChannels, result.TotalOccupancy));
                    DisplayHereNowResult(result);
                    Debug.Log (status.Error);

                });

                pubnub.HereNow().IncludeState(false).IncludeUUIDs(false).Async((result, status) => {
                    Debug.Log ("in HereNow4");
                    Debug.Log (string.Format("DateTime {0}, In Example, Channels: {1} {2}", DateTime.Now , result.TotalChannels, result.TotalOccupancy));
                    DisplayHereNowResult(result);
                    Debug.Log (status.Error);

                });

                pubnub.HereNow().IncludeState(true).IncludeUUIDs(false).Async((result, status) => {
                    Debug.Log ("in HereNow5");
                    Debug.Log (string.Format("DateTime {0}, In Example, Channels: {1} {2}", DateTime.Now , result.TotalChannels, result.TotalOccupancy));
                    DisplayHereNowResult(result);
                    Debug.Log (status.Error);

                });

            };
            
  

        

            Debug.Log ("PubNub");
            pubnub.Subscribe ().SetChannelGroups (listChannelGroups).SetChannels(listChannels).Execute();

            Debug.Log ("before Time");
            /*pubnub.Time ().Async (new PNTimeCallback<PNTimeResult>(
                (r, s) => {
                    Debug.Log ("in Time");
                }
            ));*/
            pubnub.Time ().Async ((result, status) => {
                
                if(status.Error){
                    Debug.Log (string.Format("In Example, Time Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("DateTime {0}, In Example, result: {1}", DateTime.Now ,result.TimeToken));
                }
            });

            //pubnub.History ().Channel("channel1").Start(14987439725282000).End(14985453001147606).IncludeTimetoken(false).Reverse(false).Async ((result, status) => {
            pubnub.History ().Channel("channel1").Async ((result, status) => {
                
                if(status.Error){
                    Debug.Log (string.Format("In Example, History Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("DateTime {0}, In Example, result: {1}", DateTime.Now ,result.EndTimetoken, result.Messages[0].ToString()));
                }
            });
            //pubnub.Time ().Async (new PNCallback<PNTimeResult>(){

                //Debug.Log ("in Time")
            //});*/


            /*Debug.Log ("after Time");*/
            
            //pubnub.WhereNow ().Uuid ("test uuid").Async ((result, status) => {
            
            //pubnub.Subscribe ().Async<string> ();
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


    	// Update is called once per frame
    	void Update () {
    	
    	}
}
}
