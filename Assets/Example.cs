using UnityEngine;
using System.Collections;
using PubNubAPI;
using System.Collections.Generic;
using System;

public class Example : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log ("Starting");
        PNConfiguration pnConfiguration = new PNConfiguration ();
        pnConfiguration.SetSecure = true;
        Debug.Log ("PNConfiguration");
        PubNub pubnub = new PubNub (pnConfiguration);
        Debug.Log ("PubNub");
        List<string> listChannelGroups = new List<string> (){"channelGroup1", "channelGroup2"};
        List<string> listChannels = new List<string> (){"channel1", "channel2"};
        //pubnub.Subscribe ().SetChannelGroups (listChannelGroups).SetChannels(listChannels).Execute();
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
        //});


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
