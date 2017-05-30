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
            pnConfiguration.SetSecure = true;
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

                Debug.Log ("SusbcribeCallback");
                if(mea.pnStatus != null){
                    //Debug.Log ("SusbcribeCallback in status" + String.Join(", ", mea.pnStatus.AffectedChannelGroups.ToArray()) + String.Join(", ", mea.pnStatus.AffectedChannels.ToArray()));
                }
                if(mea.pnmr != null){
                    Debug.Log ("SusbcribeCallback in message" + mea.pnmr.Channel + mea.pnmr.Payload);
                }
                if(mea.pnper != null){
                    Debug.Log ("SusbcribeCallback in presence" + mea.pnper.Channel + mea.pnper.Occupancy + mea.pnper.Event);
                }
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
                Debug.Log ("in Time");
                Debug.Log (string.Format("DateTime {0}, result: {1}", DateTime.Now ,result.TimeToken));
                Debug.Log (status.Error);
            });
            //pubnub.Time ().Async (new PNCallback<PNTimeResult>(){

                //Debug.Log ("in Time")
            //});*/


            Debug.Log ("after Time");

            pubnub.WhereNow ().Uuid ("test uuid").Async ((result, status) => {
                Debug.Log ("in WhereNow");
                Debug.Log (string.Format("DateTime {0}, result: {1}", DateTime.Now ,result.Result));
                Debug.Log (status.Error);

            });
            //pubnub.Subscribe ().Async<string> ();
    	}


    	// Update is called once per frame
    	void Update () {
    	
    	}
}
}
