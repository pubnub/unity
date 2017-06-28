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
            pnConfiguration.SubscribeKey = "demo";
            pnConfiguration.PublishKey = "demo";
            pnConfiguration.Secure = true;
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
                pubnub.WhereNow ().Async ((result, status) => {
                    Debug.Log ("in WhereNow");
                    Debug.Log (string.Format("DateTime {0}, In Example, Channels: {1}", DateTime.Now , string.Join(",",result.Channels.ToArray())));
                    Debug.Log (status.Error);

                });
            };

            Debug.Log ("PubNub");
            List<string> listChannelGroups = new List<string> (){"channelGroup1", "channelGroup2"};
            listChannelGroups.Add ("channelGroup1");
            listChannelGroups.Add ("channelGroup2");
            List<string> listChannels = new List<string> (){"channel1", "channel2"};
            listChannels.Add ("channel1");
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

            pubnub.History ().Channel("channel1").Start(14986550510296405).End(14985453001147606).IncludeTimetoken(true).Reverse(false).Async ((result, status) => {
                
                if(status.Error){
                    Debug.Log (string.Format("In Example, History Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("DateTime {0}, In Example, result: {1}", DateTime.Now ,result.EndTimetoken));
                }
            });
            //pubnub.Time ().Async (new PNCallback<PNTimeResult>(){

                //Debug.Log ("in Time")
            //});*/


            /*Debug.Log ("after Time");*/
            
            //pubnub.WhereNow ().Uuid ("test uuid").Async ((result, status) => {
            
            //pubnub.Subscribe ().Async<string> ();
    	}


    	// Update is called once per frame
    	void Update () {
    	
    	}
}
}
