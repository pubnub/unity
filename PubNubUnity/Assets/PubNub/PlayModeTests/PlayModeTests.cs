﻿using UnityEngine;
#if !UNITY_WSA_10_0
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using PubNubAPI;
using System.Collections.Generic;
using System;
#endif

namespace PubNubAPI.Tests
{
    public class PlayModeTests
    {
#if !UNITY_WSA_10_0
        #region "Time"
        //[UnityTest]
        public IEnumerator TestTime()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);

            PubNub pubnub = new PubNub(pnConfiguration);
            bool testReturn = false;
            pubnub.Time().Async((result, status) =>
            {
                bool statusError = status.Error;
                Debug.Log(statusError);
                bool resultTimeToken = result.TimeToken.Equals(0);
                Debug.Log(resultTimeToken);
                testReturn = !statusError && !resultTimeToken;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeForAsyncResponse);
            Assert.True(testReturn, "test didn't return");
            pubnub.CleanUp();
        }
        #endregion

        // #region "WhereNow"
        // [UnityTest]
        // public IEnumerator TestWhereNow() {
        // 	PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
        // 	PubNub pubnub = new PubNub(pnConfiguration);
        // 	System.Random r = new System.Random ();

        // 	string whereNowChannel = "UnityTestWhereNowChannel"+ r.Next (100);

        // 	pubnub.Subscribe ().Channels(new List<string> (){whereNowChannel}).WithPresence().Execute();
        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
        // 	pubnub.CleanUp();
        // }
        // #endregion

        #region "HereNow"
        //[UnityTest]
        // public IEnumerator TestHereNowChannel() {
        // 	PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
        // 	pnConfiguration.UUID = "UnityTestHereNowUUID";
        // 	PubNub pubnub = new PubNub(pnConfiguration);
        // 	string hereNowChannel = "UnityTestHereNowChannel";
        // 	List<string> channelList = new List<string>();
        // 	channelList.Add(hereNowChannel);
        // 	foreach(string ch in channelList){
        // 		Debug.Log("ch0:" + ch);
        // 	}

        // 	pubnub.Subscribe ().Channels(channelList).Execute();
        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
        // 	bool testReturn = false;
        // 	foreach(string ch in channelList){
        // 		Debug.Log("ch:" + ch);
        // 	}

        // 	pubnub.HereNow().Channels(channelList).IncludeState(true).IncludeUUIDs(true).Async((result, status) => {
        // 			Debug.Log("status.Error:" + status.Error);
        // 			bool matchResult = MatchHereNowresult(pubnub, result, channelList, pnConfiguration.UUID, false, false, true, 0, false, null);
        // 			testReturn = !status.Error && matchResult;
        //         });

        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls4);
        // 	Assert.True(testReturn, "test didn't return");

        // 	pubnub.CleanUp();
        // }

        [UnityTest]
        public IEnumerator TestHereNowEmptyChannel()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            pnConfiguration.UUID = "UnityTestHereNowUUID";
            PubNub pubnub = new PubNub(pnConfiguration);
            string hereNowChannel = "EmptyChannel";
            List<string> channelList = new List<string>() { hereNowChannel };
            bool testReturn = false;

            pubnub.HereNow().Channels(channelList).IncludeState(true).IncludeUUIDs(true).Async((result, status) =>
            {
                Debug.Log("status.Error:" + status.Error);
                Assert.True(!status.Error);
                bool matchResult = MatchHereNowresult(pubnub, result, channelList, pnConfiguration.UUID, true, false, false, 0, false, null);// Check occupancy-> It should be empty
                    testReturn = !status.Error && matchResult;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls4);
            Assert.True(testReturn, "test didn't return");
            pubnub.CleanUp();
        }

        [UnityTest]
        public IEnumerator TestHereNowChannels()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            pnConfiguration.UUID = "UnityTestHereNowUUID";
            PubNub pubnub = new PubNub(pnConfiguration);
            string hereNowChannel = "UnityTestHereNowChannel1";
            string hereNowChannel2 = "UnityTestHereNowChannel2";
            List<string> channelList = new List<string>();
            channelList.Add(hereNowChannel);
            channelList.Add(hereNowChannel2);

            pubnub.Subscribe().Channels(channelList).Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            bool testReturn = false;
            pubnub.HereNow().Channels(channelList).IncludeState(true).IncludeUUIDs(true).Async((result, status) =>
            {
                Debug.Log("status.Error:" + status.Error);
                Assert.True(!status.Error);
                bool matchResult = MatchHereNowresult(pubnub, result, channelList, pnConfiguration.UUID, false, false, true, 0, false, null);
                testReturn = !status.Error && matchResult;
            });

            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls4);
            Assert.True(testReturn, "test didn't return");

            testReturn = false;
            pubnub.WhereNow().Async((result, status) =>
            {
                bool statusError = status.Error;
                Debug.Log("statusError:" + statusError);

                if (result.Channels != null)
                {
                    Debug.Log(result.Channels.Contains(hereNowChannel));
                    testReturn = !statusError && result.Channels.Contains(hereNowChannel);
                }
                else
                {
                    Assert.Fail("result.Channels null");
                }
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls3);
            Assert.True(testReturn, "test didn't return");
            pubnub.CleanUp();
        }

        //[UnityTest]
        // public IEnumerator TestHereNowChannelGroup() {
        // 	PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
        // 	pnConfiguration.UUID = "UnityTestHereNowUUID";
        // 	PubNub pubnub = new PubNub(pnConfiguration);
        // 	string hereNowChannel = "UnityTestHereNowChannel";
        // 	string channelGroup = "channelGroup1";
        // 	List<string> channelList = new List<string>();
        // 	channelList.Add(hereNowChannel);
        // 	List<string> channelGroupList = new List<string>();
        // 	channelGroupList.Add(channelGroup);

        // 	pubnub.AddChannelsToChannelGroup().ChannelGroup(channelGroup).Channels(channelList).Async((result, status) => {
        //         Debug.Log ("in AddChannelsToChannelGroup");
        //     });
        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);

        // 	foreach(string ch in channelList){
        // 		Debug.Log("ch0:" + ch);
        // 	}

        // 	pubnub.Subscribe ().ChannelGroups(channelGroupList).Execute();
        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
        // 	bool testReturn = false;
        // 	foreach(string ch in channelList){
        // 		Debug.Log("ch:" + ch);
        // 	}

        // 	pubnub.HereNow().ChannelGroups(channelGroupList).IncludeState(true).IncludeUUIDs(true).Async((result, status) => {
        // 			Debug.Log("status.Error:" + status.Error);
        //             Assert.True(!status.Error);
        // 			//Assert.True(result.TotalOccupancy.Equals(1));
        // 			bool matchResult = MatchHereNowresult(pubnub, result, channelList, pnConfiguration.UUID, false, false, true, 0, false, null);
        // 			testReturn = !status.Error && matchResult;
        //         });

        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeForAsyncResponse);
        // 	Assert.True(testReturn, "test didn't return");
        // 	pubnub.CleanUp();
        // }

        [UnityTest]
        public IEnumerator TestHereNowChannelGroups()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            pnConfiguration.UUID = "UnityTestHereNowUUID";
            PubNub pubnub = new PubNub(pnConfiguration);
            string hereNowChannel = "UnityTestHereNowChannel";
            string hereNowChannel2 = "UnityTestHereNowChannel2";
            string channelGroup = "channelGroup2";
            List<string> channelList = new List<string>();
            channelList.Add(hereNowChannel);
            channelList.Add(hereNowChannel2);
            List<string> channelGroupList = new List<string>();
            channelGroupList.Add(channelGroup);
            pubnub.AddChannelsToChannelGroup().ChannelGroup(channelGroup).Channels(channelList).Async((result, status) =>
            {
                Debug.Log("in AddChannelsToChannelGroup");
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls1);

            pubnub.Subscribe().ChannelGroups(channelGroupList).Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            bool testReturn = false;
            pubnub.HereNow().ChannelGroups(channelGroupList).IncludeState(true).IncludeUUIDs(true).Async((result, status) =>
            {
                Debug.Log("status.Error:" + status.Error);
                Assert.True(!status.Error);
                //Assert.True(result.TotalOccupancy.Equals(1));
                bool matchResult = MatchHereNowresult(pubnub, result, channelList, pnConfiguration.UUID, false, false, true, 0, false, null);
                testReturn = !status.Error && matchResult;
            });

            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls4);
            Assert.True(testReturn, "test didn't return");
            pubnub.CleanUp();
        }

        // [UnityTest]
        // public IEnumerator TestHereNowChannelsAndChannelGroups() {
        // 	PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
        // 	pnConfiguration.UUID = "UnityTestHereNowUUID";
        // 	PubNub pubnub = new PubNub(pnConfiguration);
        // 	string hereNowChannel = "UnityTestHereNowChannel3";
        // 	string hereNowChannel2 = "UnityTestHereNowChannel4";
        // 	string hereNowChannel3 = "UnityTestHereNowChannel5";
        // 	string channelGroup = "channelGroup3";
        // 	List<string> channelList = new List<string>();
        // 	channelList.Add(hereNowChannel);
        // 	channelList.Add(hereNowChannel2);
        // 	List<string> channelList2 = new List<string>();
        // 	channelList2.Add(hereNowChannel3);
        // 	List<string> channelGroupList = new List<string>();
        // 	channelGroupList.Add(channelGroup);
        // 	pubnub.AddChannelsToChannelGroup().ChannelGroup(channelGroup).Channels(channelList).Async((result, status) => {
        //         Debug.Log ("in AddChannelsToChannelGroup");
        //     });
        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);

        // 	pubnub.Subscribe ().Channels(channelList2).ChannelGroups(channelGroupList).Execute();
        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
        // 	bool testReturn = false;
        // 	pubnub.HereNow().Channels(channelList2).ChannelGroups(channelGroupList).IncludeState(true).IncludeUUIDs(true).Async((result, status) => {
        // 			Debug.Log("status.Error:" + status.Error);
        //             Assert.True(!status.Error);
        // 			//Assert.True(result.TotalOccupancy.Equals(1));
        // 			channelList.AddRange(channelList2);
        // 			bool matchResult = MatchHereNowresult(pubnub, result, channelList, pnConfiguration.UUID, false, false, true, 0, false, null);
        //             testReturn = !status.Error && matchResult;
        //         });

        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls4);
        // 	Assert.True(testReturn, "test didn't return");
        // 	pubnub.CleanUp();
        // }

        //[UnityTest]
        // public IEnumerator TestGlobalHereNow() {
        // 	PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
        // 	pnConfiguration.UUID = "UnityTestHereNowUUID";
        // 	PubNub pubnub = new PubNub(pnConfiguration);
        // 	System.Random r = new System.Random ();

        // 	string hereNowChannel = "UnityTestHereNowChannel6"+ r.Next (100);
        // 	string hereNowChannel2 = "UnityTestHereNowChannel7"+ r.Next (100);
        // 	string hereNowChannel3 = "UnityTestHereNowChannel8"+ r.Next (100);
        // 	string channelGroup = "channelGroup4"+ r.Next (100);
        // 	List<string> channelList = new List<string>();
        // 	channelList.Add(hereNowChannel);
        // 	channelList.Add(hereNowChannel2);
        // 	List<string> channelList2 = new List<string>();
        // 	channelList2.Add(hereNowChannel3);
        // 	List<string> channelGroupList = new List<string>();
        // 	channelGroupList.Add(channelGroup);
        // 	pubnub.AddChannelsToChannelGroup().ChannelGroup(channelGroup).Channels(channelList).Async((result, status) => {
        //         Debug.Log ("in AddChannelsToChannelGroup");
        //     });
        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);

        // 	pubnub.Subscribe ().Channels(channelList2).ChannelGroups(channelGroupList).Execute();
        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls1);
        // 	bool testReturn = false;
        // 	pubnub.HereNow().IncludeState(true).IncludeUUIDs(true).Async((result, status) => {
        // 			Debug.Log("status.Error:" + status.Error);
        //             Assert.True(!status.Error);
        // 			//Assert.True(result.TotalOccupancy.Equals(1));
        // 			channelList.AddRange(channelList2);
        // 			bool matchResult = MatchHereNowresult(pubnub, result, channelList, pnConfiguration.UUID, false, false, true, 0, false, null);

        //             testReturn = !status.Error && matchResult;
        //         });

        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeForAsyncResponse);
        // 	Assert.True(testReturn, "test didn't return");
        // 	pubnub.CleanUp();
        // }

        [UnityTest]
        public IEnumerator TestGlobalHereNowWithoutUUID()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            pnConfiguration.UUID = "UnityTestHereNowUUID";
            PubNub pubnub = new PubNub(pnConfiguration);
            System.Random r = new System.Random();

            string hereNowChannel = "UnityTestHereNowChannel6" + r.Next(100);
            string hereNowChannel2 = "UnityTestHereNowChannel7" + r.Next(100);
            string hereNowChannel3 = "UnityTestHereNowChannel8" + r.Next(100);

            string channelGroup = "channelGroup5" + r.Next(100);
            List<string> channelList = new List<string>();
            channelList.Add(hereNowChannel);
            channelList.Add(hereNowChannel2);
            List<string> channelList2 = new List<string>();
            channelList2.Add(hereNowChannel3);
            List<string> channelGroupList = new List<string>();
            channelGroupList.Add(channelGroup);
            pubnub.AddChannelsToChannelGroup().ChannelGroup(channelGroup).Channels(channelList).Async((result, status) =>
            {
                Debug.Log("in AddChannelsToChannelGroup");
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            pubnub.Subscribe().Channels(channelList2).ChannelGroups(channelGroupList).Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            bool testReturn = false;
            pubnub.HereNow().IncludeState(true).IncludeUUIDs(false).Async((result, status) =>
            {
                Debug.Log("status.Error:" + status.Error);
                Assert.True(!status.Error);
                //Assert.True(resultTotalOccupancy.Equals(1));
                channelList.AddRange(channelList2);
                bool matchResult = MatchHereNowresult(pubnub, result, channelList, pnConfiguration.UUID, false, true, false, 1, false, null);
                testReturn = !status.Error && matchResult;
            });

            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls3);
            Assert.True(testReturn, "test didn't return");
            pubnub.CleanUp();
        }

        //[UnityTest]
        // public IEnumerator TestGlobalHereNowWithoutUUIDWithState() {
        // 	PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
        // 	System.Random r = new System.Random ();
        // 	pnConfiguration.UUID = "UnityTestHereNowUUID"+ r.Next (100);
        // 	PubNub pubnub = new PubNub(pnConfiguration);

        // 	string hereNowChannel = "UnityTestHereNowChannel6"+ r.Next (100);
        // 	string hereNowChannel2 = "UnityTestHereNowChannel7"+ r.Next (100);
        // 	string hereNowChannel3 = "UnityTestHereNowChannel8"+ r.Next (100);
        // 	string channelGroup = "channelGroup6"+ r.Next (100);
        // 	List<string> channelList = new List<string>();
        // 	channelList.Add(hereNowChannel);
        // 	channelList.Add(hereNowChannel2);
        // 	List<string> channelList2 = new List<string>();
        // 	channelList2.Add(hereNowChannel3);
        // 	List<string> channelGroupList = new List<string>();
        // 	channelGroupList.Add(channelGroup);
        // 	pubnub.AddChannelsToChannelGroup().ChannelGroup(channelGroup).Channels(channelList).Async((result, status) => {
        //         Debug.Log ("in AddChannelsToChannelGroup");
        //     });
        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);

        // 	pubnub.Subscribe ().Channels(channelList2).ChannelGroups(channelGroupList).Execute();
        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls1);
        // 	Dictionary<string, object> state = new Dictionary<string, object>();
        // 	state.Add("k", "v");
        // 	pubnub.SetPresenceState().Channels(channelList).ChannelGroups(channelGroupList).State(state).Async ((result, status) => {

        //     });
        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
        // 	bool testReturn = false;
        // 	pubnub.HereNow().IncludeState(true).IncludeUUIDs(false).Async((result, status) => {
        // 			Debug.Log("status.Error:" + status.Error);
        //             Assert.True(!status.Error);
        // 			//Assert.True(resultTotalOccupancy.Equals(1));
        // 			channelList.AddRange(channelList2);
        // 			bool matchResult = MatchHereNowresult(pubnub, result, channelList, pnConfiguration.UUID, false, true, false, 1, true, state);
        // 			testReturn = !status.Error && matchResult;
        //         });

        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
        // 	Assert.True(testReturn, "test didn't return");
        // 	pubnub.CleanUp();
        // }

        [UnityTest]
        public IEnumerator TestHereNowWithUUIDWithState()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            pnConfiguration.UUID = "UnityTestHereNowUUID";
            PubNub pubnub = new PubNub(pnConfiguration);
            System.Random r = new System.Random();

            string hereNowChannel = "UnityTestHereNowChannel6" + r.Next(100);
            string hereNowChannel2 = "UnityTestHereNowChannel7" + r.Next(100);
            string hereNowChannel3 = "UnityTestHereNowChannel8" + r.Next(100);
            string channelGroup = "channelGroup6" + r.Next(100);
            List<string> channelList = new List<string>();
            channelList.Add(hereNowChannel);
            channelList.Add(hereNowChannel2);
            List<string> channelList2 = new List<string>();
            channelList2.Add(hereNowChannel3);
            List<string> channelGroupList = new List<string>();
            channelGroupList.Add(channelGroup);
            pubnub.AddChannelsToChannelGroup().ChannelGroup(channelGroup).Channels(channelList).Async((result, status) =>
            {
                Debug.Log("in AddChannelsToChannelGroup");
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            pubnub.Subscribe().Channels(channelList2).ChannelGroups(channelGroupList).Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls1);
            Dictionary<string, object> state = new Dictionary<string, object>();
            state.Add("k", "v");
            pubnub.SetPresenceState().Channels(channelList).ChannelGroups(channelGroupList).State(state).Async((result, status) =>
            {

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            bool testReturn = false;
            pubnub.HereNow().Channels(channelList2).ChannelGroups(channelGroupList).IncludeState(true).IncludeUUIDs(false).Async((result, status) =>
            {
                Debug.Log("status.Error:" + status.Error);
                Assert.True(!status.Error);
                //Assert.True(resultTotalOccupancy.Equals(1));
                channelList.AddRange(channelList2);
                bool matchResult = MatchHereNowresult(pubnub, result, channelList, pnConfiguration.UUID, false, true, false, 1, true, state);
                testReturn = !status.Error && matchResult;
            });

            yield return new WaitForSeconds(PlayModeCommon.WaitTimeForAsyncResponse);
            Assert.True(testReturn, "test didn't return");
            pubnub.CleanUp();
        }

        public bool MatchHereNowresult(PubNub pubnub, PNHereNowResult result, List<string> channelList, string uuid, bool checkOccupancy, bool checkOccupancyOnly, bool checkOccupantData, int occupancy, bool checkState, Dictionary<string, object> state)
        {
            bool matchResult = false;
            if (result.Channels != null)
            {
                Dictionary<string, PNHereNowChannelData> dict = result.Channels;
                PNHereNowChannelData pnHereNowChannelData;

                foreach (string hereNowChannel in channelList)
                {
                    if (dict.TryGetValue(hereNowChannel, out pnHereNowChannelData))
                    {
                        if (checkOccupancy || checkOccupancyOnly)
                        {
                            matchResult = pnHereNowChannelData.Occupancy.Equals(occupancy);
                            Debug.Log("Occupancy.Equals:" + matchResult);
                        }

                        if (checkState || checkOccupantData)
                        {
                            bool found = false;
                            bool checkStateResult = false;
                            foreach (PNHereNowOccupantData pnHereNowOccupantData in pnHereNowChannelData.Occupants)
                            {
                                Debug.Log("finding:" + pnHereNowOccupantData.UUID);

                                if (checkState)
                                {
                                    Debug.Log(state.ToString());

                                    checkStateResult = pnHereNowOccupantData.State.Equals(pubnub.JsonLibrary.SerializeToJsonString(state));
                                    Debug.Log("checkStateResult:" + checkStateResult);
                                }

                                if (checkOccupantData)
                                {
                                    if (pnHereNowOccupantData.UUID.Equals(uuid))
                                    {
                                        found = true;
                                        Debug.Log("found:" + pnHereNowOccupantData.UUID);
                                        break;
                                    }
                                }
                            }
                            if (checkState && checkOccupantData)
                            {
                                matchResult = checkStateResult && found;
                            }
                            else if (checkOccupantData)
                            {
                                matchResult = found;
                            }
                            else if (checkState)
                            {
                                matchResult = checkState;
                            }

                        }
                    }
                    else
                    {
                        Assert.Fail("channel not found" + hereNowChannel);
                    }
                }

            }
            else
            {
                Assert.Fail("Channels null");
            }
            Debug.Log("matchResult:" + matchResult);
            return matchResult;
        }
        #endregion

        #region "Publish"
        [UnityTest]
        public IEnumerator TestPublishString()
        {
            string publishChannel = "UnityTestPublishChannel";
            string payload = string.Format("test message {0}", DateTime.Now.Ticks.ToString());
            yield return DoPublishTestProcsssing(payload, publishChannel);
        }

        [UnityTest]
        public IEnumerator TestPublishInt()
        {
            string publishChannel = "UnityTestPublishChannel";
            object payload = 1;
            yield return DoPublishTestProcsssing(payload, publishChannel);
        }

        [UnityTest]
        public IEnumerator TestPublishDouble()
        {
            string publishChannel = "UnityTestPublishChannel";
            double payload = 1.1;
            yield return DoPublishTestProcsssing(payload, publishChannel);
        }

        [UnityTest]
        public IEnumerator TestPublishDoubleArr()
        {
            string publishChannel = "UnityTestPublishChannel";
            double[] payload = { 1.1 };
            yield return DoPublishTestProcsssing(payload, publishChannel);
        }

        [UnityTest]
        public IEnumerator TestPublishEmptyArr()
        {
            string publishChannel = "UnityTestPublishChannel";
            object[] payload = { };
            yield return DoPublishTestProcsssing(payload, publishChannel);
        }

        [UnityTest]
        public IEnumerator TestPublishEmptyDict()
        {
            string publishChannel = "UnityTestPublishChannel";
            Dictionary<string, int> payload = new Dictionary<string, int>();
            yield return DoPublishTestProcsssing(payload, publishChannel);
        }

        [UnityTest]
        public IEnumerator TestPublishDict()
        {
            string publishChannel = "UnityTestPublishChannel";
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload.Add("cat", "test");
            yield return DoPublishTestProcsssing(payload, publishChannel);
        }

        [UnityTest]
        public IEnumerator TestPublishLong()
        {
            string publishChannel = "UnityTestPublishChannel";
            long payload = 14255515120803306;
            yield return DoPublishTestProcsssing(payload, publishChannel);
        }

        [UnityTest]
        public IEnumerator TestPublishLongArr()
        {
            string publishChannel = "UnityTestPublishChannel";
            long[] payload = { 14255515120803306 };
            yield return DoPublishTestProcsssing(payload, publishChannel);
        }

        [UnityTest]
        public IEnumerator TestPublishIntArr()
        {
            string publishChannel = "UnityTestPublishChannel";
            int[] payload = { 13, 14 };
            yield return DoPublishTestProcsssing(payload, publishChannel);
        }

        [UnityTest]
        public IEnumerator TestPublishStringArr()
        {
            string publishChannel = "UnityTestPublishChannel";
            string[] payload = { "testarr" };
            yield return DoPublishTestProcsssing(payload, publishChannel);
        }

        [UnityTest]
        public IEnumerator TestPublishComplexMessage()
        {
            string publishChannel = "UnityTestPublishChannel";
            object payload = new PubnubDemoObject();
            yield return DoPublishTestProcsssing(payload, publishChannel);
        }

        //[UnityTest]
        public IEnumerator TestJoinLeave()
        {
            string channel = "UnityTestJoinChannel";
            yield return DoJoinLeaveTestProcsssing(channel);
        }

        [UnityTest]
        public IEnumerator TestConnected()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next(100);
            string channel = "UnityTestConnectedChannel";

            PubNub pubnub = new PubNub(pnConfiguration);
            List<string> channelList2 = new List<string>();
            channelList2.Add(channel);
            bool tresult = false;

            pubnub.SubscribeCallback += (sender, e) =>
            {
                SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;
                if (mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory))
                {
                    tresult = mea.Status.UUID.Contains(pnConfiguration.UUID);
                    Assert.True(tresult);
                }
            };
            pubnub.Subscribe().Channels(channelList2).Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "test didn't return");
            pubnub.CleanUp();

        }

        /*[UnityTest]
        public IEnumerator TestAbort() {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random ();
            pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next (100);
            string channel = "UnityTestConnectedChannel";

            PubNub pubnub = new PubNub(pnConfiguration);
            List<string> channelList2 = new List<string>();
            channelList2.Add(channel);
            bool tresult = false;

            pubnub.SubscribeCallback += (sender, e) => {
                SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;
                if(mea.Status.Category.Equals(PNStatusCategory.PNCancelledCategory)){
                    Assert.True(mea.Status.Error);
                    if(mea.Status.ErrorData!=null){
                        Debug.Log(mea.Status.ErrorData.Info);
                        Assert.True(mea.Status.ErrorData.Info.Contains("Aborted"));
                    }

                    tresult = true;
                }
            };
            pubnub.Subscribe ().SetChannels(channelList2).Execute();
            yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
            pubnub.Reconnect();
            pubnub.CleanUp();
            yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
            Assert.True(tresult, "test didn't return");

        }*/

        [UnityTest]
        public IEnumerator TestAlreadySubscribed()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next(100);
            string channel = "UnityTestConnectedChannel";

            PubNub pubnub = new PubNub(pnConfiguration);
            List<string> channelList2 = new List<string>();
            channelList2.Add(channel);
            bool tresult = false;

            pubnub.SubscribeCallback += (sender, e) =>
            {
                SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;
                if (mea.Status.Category.Equals(PNStatusCategory.PNUnknownCategory))
                {
                    Debug.Log("mea.Status.Error:" + mea.Status.Error);
                    Assert.True(mea.Status.Error);
                    bool errorData = true;
                    if (mea.Status.ErrorData != null)
                    {
                        Debug.Log(mea.Status.ErrorData.Info);
                        errorData = mea.Status.ErrorData.Info.Contains("Duplicate Channels or Channel Groups");
                        Assert.True(errorData);
                    }

                    tresult = errorData && mea.Status.Error;
                }
            };
            pubnub.Subscribe().Channels(channelList2).Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls3);
            pubnub.Subscribe().Channels(channelList2).Execute();
            Assert.True(tresult, "test didn't return");
            pubnub.CleanUp();

        }

        public IEnumerator DoJoinLeaveTestProcsssing(string channel)
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            channel = channel + r.Next(100);
            pnConfiguration.UUID = "UnityTestJoinUUID_" + r.Next(100);
            PubNub pubnub = new PubNub(pnConfiguration);

            PNConfiguration pnConfiguration2 = PlayModeCommon.SetPNConfig(false);

            pnConfiguration2.UUID = "UnityTestJoinUUID_" + r.Next(100);

            List<string> channelList2 = new List<string>();
            channelList2.Add(channel);
            bool tJoinResult = false;
            bool tLeaveResult = false;

            PubNub pubnub2 = new PubNub(pnConfiguration2);

            pubnub.SubscribeCallback += (sender, e) =>
            {
                SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;
                if (!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory))
                {
                    if (mea.PresenceEventResult.Event.Equals("join"))
                    {
                        Debug.Log(mea.PresenceEventResult.UUID);
                        Debug.Log(mea.PresenceEventResult.Timestamp);
                        Debug.Log(mea.PresenceEventResult.Occupancy);
                        Debug.Log(string.Join(",", mea.PresenceEventResult.Join.ToArray()));
                        bool containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration2.UUID);
                        Assert.True(containsUUID);
                        bool containsOccupancy = mea.PresenceEventResult.Occupancy > 0;
                        Assert.True(containsOccupancy);
                        bool containsTimestamp = mea.PresenceEventResult.Timestamp > 0;
                        Assert.True(containsTimestamp);

                        tJoinResult = containsTimestamp && containsOccupancy && containsUUID;
                        Debug.Log("containsUUID" + containsUUID + tJoinResult);

                    }
                    else if (mea.PresenceEventResult.Event.Equals("leave"))
                    {
                        bool containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration2.UUID);
                        Assert.True(containsUUID);
                        bool containsTimestamp = mea.PresenceEventResult.Timestamp > 0;
                        Assert.True(containsTimestamp);
                        tLeaveResult = containsTimestamp && containsUUID;

                        Debug.Log("containsUUID" + containsUUID + tLeaveResult);
                    }
                }
            };
            pubnub.Subscribe().Channels(channelList2).WithPresence().Execute();
            //yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);

            pubnub2.Subscribe().Channels(channelList2).Execute();
            //yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            Assert.True(tJoinResult, "join test didn't return");
            pubnub2.Unsubscribe().Channels(channelList2).Async((result, status) =>
            {
                Debug.Log("status.Error:" + status.Error);
                Assert.True(!status.Error);
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tLeaveResult, "leave test didn't return");
            pubnub.CleanUp();
            pubnub2.CleanUp();
        }

        [UnityTest]
        public IEnumerator TestPublishLoadTest()
        {
            string publishChannel = "UnityTestPublishChannel";
            Dictionary<string, bool> payload = new Dictionary<string, bool>();
            for (int i = 0; i < 50; i++)
            {
                payload.Add(string.Format("payload {0}", i), false);
            }

            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            pnConfiguration.UUID = "UnityTestPublishLoadTestUUID";
            PubNub pubnub = new PubNub(pnConfiguration);
            List<string> channelList2 = new List<string>();
            channelList2.Add(publishChannel);
            //bool testReturn = false;

            pubnub.SubscribeCallback += (sender, e) =>
            {
                SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;
                if (!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory))
                {
                    if (payload.ContainsKey(mea.MessageResult.Payload.ToString()))
                    {
                        payload[mea.MessageResult.Payload.ToString()] = true;
                    }
                }
            };
            pubnub.Subscribe().Channels(channelList2).Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls1);
            foreach (KeyValuePair<string, bool> kvp in payload)
            {
                pubnub.Publish().Channel(publishChannel).Message(kvp.Key).Async((result, status) =>
                {
                    Assert.True(!result.Timetoken.Equals(0));
                    Assert.True(status.Error.Equals(false));
                    Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                });
            }
            yield return new WaitForSeconds(5);
            bool tresult = false;
            foreach (KeyValuePair<string, bool> kvp in payload)
            {
                if (!kvp.Value)
                {
                    tresult = true;
                }
            }

            Assert.True(!tresult);
            pubnub.CleanUp();
        }

        [UnityTest]
        public IEnumerator TestPublishCreatePushPayload()
        {
            string publishChannel = "UnityTestPushPayloadHelperChannel";
            CreatePushPayloadHelper cpph = new CreatePushPayloadHelper();
            PNAPSData aps = new PNAPSData();
            aps.Alert = "alert";
            aps.Badge = 1;
            aps.Sound = "ding";
            aps.Custom = new Dictionary<string, object>(){
                    {"aps_key1", "aps_value1"},
                    {"aps_key2", "aps_value2"},
                };

            PNAPNSData apns = new PNAPNSData();
            apns.APS = aps;
            apns.Custom = new Dictionary<string, object>(){
                    {"apns_key1", "apns_value1"},
                    {"apns_key2", "apns_value2"},
                };

            PNAPNS2Data apns2One = new PNAPNS2Data();
            apns2One.CollapseID = "invitations";
            apns2One.Expiration = "2019-12-13T22:06:09Z";
            apns2One.Version = "v1";
            apns2One.Targets = new List<PNPushTarget>(){
                    new PNPushTarget(){
                        Environment = PNPushEnvironment.Development,
                        Topic = "com.meetings.chat.app",
                        ExcludeDevices = new List<string>(){
                            "device1",
                            "device2",
                        }
                    }
                };

            PNAPNS2Data apns2Two = new PNAPNS2Data();
            apns2Two.CollapseID = "invitations";
            apns2Two.Expiration = "2019-12-15T22:06:09Z";
            apns2Two.Version = "v2";
            apns2Two.Targets = new List<PNPushTarget>(){
                    new PNPushTarget(){
                        Environment = PNPushEnvironment.Production,
                        Topic = "com.meetings.chat.app",
                        ExcludeDevices = new List<string>(){
                            "device3",
                            "device4",
                        }
                    }
                };

            List<PNAPNS2Data> apns2 = new List<PNAPNS2Data>(){
                    apns2One,
                    apns2Two,
                };

            PNMPNSData mpns = new PNMPNSData();
            mpns.Custom = new Dictionary<string, object>(){
                    {"mpns_key1", "mpns_value1"},
                    {"mpns_key2", "mpns_value2"},
                };
            mpns.Title = "title";
            mpns.Type = "type";
            mpns.Count = 1;
            mpns.BackTitle = "BackTitle";
            mpns.BackContent = "BackContent";

            PNFCMData fcm = new PNFCMData();
            fcm.Custom = new Dictionary<string, object>(){
                    {"fcm_key1", "fcm_value1"},
                    {"fcm_key2", "fcm_value2"},
                };
            fcm.Data = new PNFCMDataFields()
            {
                Summary = "summary",
                Custom = new Dictionary<string, object>(){
                        {"fcm_data_key1", "fcm_data_value1"},
                        {"fcm_data_key2", "fcm_data_value2"},
                    }
            };

            Dictionary<string, object> commonPayload = new Dictionary<string, object>();
            commonPayload = new Dictionary<string, object>(){
                    {"common_key1", "common_value1"},
                    {"common_key2", "common_value2"},
                };

            Dictionary<string, object> payload = cpph.SetAPNSPayload(apns, apns2).SetMPNSPayload(mpns).SetFCMPayload(fcm).SetCommonPayload(commonPayload).BuildPayload();

            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            pnConfiguration.UUID = "UnityTestPublishUUID";
            PubNub pubnub = new PubNub(pnConfiguration);
            List<string> channelList2 = new List<string>();
            channelList2.Add(publishChannel);
            bool testReturn = false;
            pubnub.SubscribeCallback += (sender, e) =>
            {
                SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;
                if (!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory))
                {
                    Assert.True(mea.MessageResult.Channel.Equals(publishChannel));
                    Dictionary<string, object> result = mea.MessageResult.Payload as Dictionary<string, object>;
                    if (result != null)
                    {
                        Dictionary<string, object> resAPNS = result["pn_apns"] as Dictionary<string, object>;
                        if (resAPNS != null)
                        {
                            Dictionary<string, object> resAPS = resAPNS["aps"] as Dictionary<string, object>;
                            Assert.IsTrue(aps.Alert.ToString().Equals(resAPS["alert"].ToString()));
                            Assert.IsTrue(aps.Badge.ToString().Equals(resAPS["badge"].ToString()));
                            Assert.IsTrue(aps.Sound.ToString().Equals(resAPS["sound"].ToString()));
                            Assert.IsTrue(aps.Custom["aps_key1"].ToString().Equals(resAPS["aps_key1"].ToString()));
                            Assert.IsTrue(aps.Custom["aps_key2"].ToString().Equals(resAPS["aps_key2"].ToString()));
                            Assert.IsTrue(apns.Custom["apns_key1"].ToString().Equals(resAPNS["apns_key1"].ToString()));
                            Assert.IsTrue(apns.Custom["apns_key2"].ToString().Equals(resAPNS["apns_key2"].ToString()));
                        }
                        else
                        {
                            Assert.Fail("apns null");
                        }
                        Debug.Log("PAYLOAD20:" + result["pn_push"].ToString() + result["pn_push"].GetType());
                        Dictionary<string, object>[] resAPNS2 = result["pn_push"] as Dictionary<string, object>[];
                        if (resAPNS2 != null)
                        {
                            Assert.IsTrue(apns2One.CollapseID.Equals(resAPNS2[0]["collapseId"].ToString()));
                            Assert.IsTrue(apns2Two.CollapseID.Equals(resAPNS2[1]["collapseId"].ToString()));
                            Assert.IsTrue(apns2One.Expiration.Equals(resAPNS2[0]["expiration"].ToString()));
                            Assert.IsTrue(apns2Two.Expiration.Equals(resAPNS2[1]["expiration"].ToString()));
                            Assert.IsTrue(apns2One.Version.Equals(resAPNS2[0]["version"].ToString()));
                            Assert.IsTrue(apns2Two.Version.Equals(resAPNS2[1]["version"].ToString()));
                            object[] o1 = resAPNS2[0]["targets"] as object[];
                            Dictionary<string, object> resTargets0 = o1[0] as Dictionary<string, object>;
                            object[] o2 = resAPNS2[1]["targets"] as object[];
                            Dictionary<string, object> resTargets1 = o2[0] as Dictionary<string, object>;
                            Assert.IsTrue(apns2One.Targets[0].Environment.ToString().Equals(resTargets0["environment"].ToString()));
                            Assert.IsTrue(apns2One.Targets[0].Topic.ToString().Equals(resTargets0["topic"].ToString()));
                            Assert.IsTrue(apns2Two.Targets[0].Environment.ToString().Equals(resTargets1["environment"].ToString()));
                            Assert.IsTrue(apns2Two.Targets[0].Topic.ToString().Equals(resTargets1["topic"].ToString()));
                            string[] resExcludeDev0 = resTargets0["exclude_devices"] as string[];
                            Assert.IsTrue(apns2One.Targets[0].ExcludeDevices[0].ToString().Equals(resExcludeDev0[0].ToString()));
                            Assert.IsTrue(apns2One.Targets[0].ExcludeDevices[1].ToString().Equals(resExcludeDev0[1].ToString()));
                            string[] resExcludeDev1 = resTargets1["exclude_devices"] as string[];
                            Assert.IsTrue(apns2Two.Targets[0].ExcludeDevices[0].ToString().Equals(resExcludeDev1[0].ToString()));
                            Assert.IsTrue(apns2Two.Targets[0].ExcludeDevices[1].ToString().Equals(resExcludeDev1[1].ToString()));
                        }
                        else
                        {
                            Assert.Fail("apns2 null");
                        }

                        Dictionary<string, object> resMPNS = result["pn_mpns"] as Dictionary<string, object>;
                        if (resMPNS != null)
                        {
                            Assert.IsTrue(mpns.Title.ToString().Equals(resMPNS["title"].ToString()));
                            Assert.IsTrue(mpns.BackContent.ToString().Equals(resMPNS["back_content"].ToString()));
                            Assert.IsTrue(mpns.BackTitle.ToString().Equals(resMPNS["back_title"].ToString()));
                            Assert.IsTrue(mpns.Count.ToString().Equals(resMPNS["count"].ToString()));
                            Assert.IsTrue(mpns.Type.ToString().Equals(resMPNS["type"].ToString()));
                            Assert.IsTrue(mpns.Custom["mpns_key1"].ToString().Equals(resMPNS["mpns_key1"].ToString()));
                            Assert.IsTrue(mpns.Custom["mpns_key2"].ToString().Equals(resMPNS["mpns_key2"].ToString()));
                        }
                        else
                        {
                            Assert.Fail("mpns null");
                        }

                        Assert.IsTrue(commonPayload["common_key1"].ToString().Equals(result["common_key1"].ToString()));
                        Assert.IsTrue(commonPayload["common_key2"].ToString().Equals(result["common_key2"].ToString()));

                        Dictionary<string, object> resFCM = result["pn_gcm"] as Dictionary<string, object>;
                        if (resFCM != null)
                        {
                            Assert.IsTrue(fcm.Custom["fcm_key1"].ToString().Equals(resFCM["fcm_key1"].ToString()));
                            Assert.IsTrue(fcm.Custom["fcm_key2"].ToString().Equals(resFCM["fcm_key2"].ToString()));
                            Dictionary<string, object> resFCMData = resFCM["data"] as Dictionary<string, object>;
                            Assert.IsTrue(fcm.Data.Summary.ToString().Equals(resFCMData["summary"].ToString()));
                            Assert.IsTrue(fcm.Data.Custom["fcm_data_key1"].ToString().Equals(resFCMData["fcm_data_key1"].ToString()));
                            Assert.IsTrue(fcm.Data.Custom["fcm_data_key2"].ToString().Equals(resFCMData["fcm_data_key2"].ToString()));

                        }
                        else
                        {
                            Assert.Fail("fcm null");
                        }
                    }
                    testReturn = true;
                }
            };
            pubnub.Subscribe().Channels(channelList2).Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            pubnub.Publish().Channel(publishChannel).Message(payload).Async((result, status) =>
            {
                Assert.True(!result.Timetoken.Equals(0));
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(testReturn, "test didn't return");
            pubnub.CleanUp();
        }

        public IEnumerator DoPublishTestProcsssing(object payload, string publishChannel)
        {
            Debug.Log("PAYLOAD:" + payload.ToString());
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            pnConfiguration.UUID = "UnityTestPublishUUID";
            PubNub pubnub = new PubNub(pnConfiguration);
            List<string> channelList2 = new List<string>();
            channelList2.Add(publishChannel);

            bool testReturn = false;

            pubnub.SubscribeCallback += (sender, e) =>
            {
                SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;
                if (!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory))
                {
                    Debug.Log("PAYLOAD20:" + payload.ToString() + payload.GetType());

                    Assert.True(mea.MessageResult.Channel.Equals(publishChannel));
                    if (payload.GetType().Equals(typeof(Int64)))
                    {
                        long expected;
                        if (Int64.TryParse(payload.ToString(), out expected))
                        {
                            long response;
                            if (Int64.TryParse(mea.MessageResult.Payload.ToString(), out response))
                            {
                                bool expectedAndResponseMatch = expected.Equals(response);
                                Assert.IsTrue(expectedAndResponseMatch);
                                testReturn = expectedAndResponseMatch;
                            }
                            else
                            {
                                Assert.Fail("response long conversion failed");
                            }
                        }
                        else
                        {
                            Assert.Fail("expectedlong conversion failed");
                        }
                    }
                    else if (payload.GetType().Equals(typeof(Int64[])))
                    {
                        Debug.Log(mea.MessageResult.Payload.GetType());
                        Debug.Log(mea.MessageResult.Payload.GetType().Equals(typeof(string[])));
                        Int64[] expected = (Int64[])payload;
                        string[] response = (string[])mea.MessageResult.Payload;
                        foreach (Int64 iExp in expected)
                        {
                            bool found = false;
                            foreach (string iResp in response)
                            {
                                if (iExp.ToString().Equals(iResp))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                Assert.Fail("response not found");
                            }
                            else
                            {
                                testReturn = found;
                            }
                        }
                    }
                    else if (payload.GetType().Equals(typeof(double[])))
                    {
                        Debug.Log(mea.MessageResult.Payload.GetType());
                        Debug.Log(mea.MessageResult.Payload.GetType().Equals(typeof(double[])));
                        double[] expected = (double[])payload;
                        double[] response = (double[])mea.MessageResult.Payload;
                        foreach (double iExp in expected)
                        {
                            Debug.Log(iExp.ToString());
                            bool found = false;
                            foreach (double iResp in response)
                            {
                                Debug.Log(iResp.ToString());
                                if (iExp.Equals(iResp))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                Assert.Fail("response not found");
                            }
                            else
                            {
                                testReturn = found;
                            }
                        }
                    }
                    else if (payload.GetType().Equals(typeof(string[])))
                    {
                        string[] expected = (string[])payload;
                        string[] response = (string[])mea.MessageResult.Payload;
                        foreach (string strExp in expected)
                        {
                            bool found = false;
                            foreach (string strResp in response)
                            {
                                if (strExp.Equals(strResp))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                Assert.Fail("response not found");
                            }
                            else
                            {
                                testReturn = found;
                            }
                        }
                    }
                    else if (payload.GetType().Equals(typeof(System.Object[])))
                    {
                        System.Object[] expected = (System.Object[])payload;
                        System.Object[] response = (System.Object[])mea.MessageResult.Payload;
                        // + payload.GetType().Equals(typeof(System.Object[])) + expected[0].Equals(response[0]));
                        bool expectedAndResponseMatch = expected.Length.Equals(response.Length) && expected.Length.Equals(0);
                        Assert.IsTrue(expectedAndResponseMatch);
                        testReturn = expectedAndResponseMatch;
                    }
                    else if (payload.GetType().Equals(typeof(Dictionary<string, string>)))
                    {
                        Dictionary<string, string> expected = (Dictionary<string, string>)payload;
                        IDictionary response = mea.MessageResult.Payload as IDictionary;
                        Debug.Log("PAYLOAD21:" + payload.ToString() + payload.GetType());
                        //Assert.True(response["cat"].Equals("test"));
                        bool expectedAndResponseMatch = response["cat"].Equals("test");
                        testReturn = expectedAndResponseMatch;
                    }
                    else if (payload.GetType().Equals(typeof(Dictionary<string, int>)))
                    {
                        Dictionary<string, int> expected = (Dictionary<string, int>)payload;
                        IDictionary response = mea.MessageResult.Payload as IDictionary;
                        Debug.Log("PAYLOAD22:" + payload.ToString() + payload.GetType());
                        bool expectedAndResponseMatch = (response == null || response.Count < 1);
                        Assert.IsTrue(expectedAndResponseMatch);
                        testReturn = expectedAndResponseMatch;

                        //Assert.True(expected.Count.Equals(response.Count) && expected.Count.Equals(0));

                    }
                    else if (payload.GetType().Equals(typeof(Int32[])))
                    {
                        Int32[] expected = (Int32[])payload;
                        Int32[] response = (Int32[])mea.MessageResult.Payload;
                        foreach (int iExp in expected)
                        {
                            bool found = false;
                            foreach (int iResp in response)
                            {
                                if (iExp.Equals(iResp))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                Assert.Fail("response not found");
                            }
                            else
                            {
                                testReturn = found;
                            }
                        }
                    }
                    else if (payload.GetType().ToString().Contains(typeof(PubnubDemoObject).ToString()))
                    {
                        Debug.Log("PAYLOAD2 PubnubDemoObject:" + payload.ToString() + payload.GetType());
                        PubnubDemoObject expected = payload as PubnubDemoObject;
                        Debug.Log(mea.MessageResult.Payload == null);
                        Debug.Log(mea.MessageResult.Payload.GetType());

                        Dictionary<string, object> resp = (Dictionary<string, object>)mea.MessageResult.Payload;
                        Debug.Log(resp == null);
                        //Debug.Log(resp[]);
                        PubnubDemoObject response = new PubnubDemoObject();
                        Type responseType = resp.GetType();
                        foreach (KeyValuePair<string, object> item in resp)
                        {
                            Debug.Log(item.Key);
                            Debug.Log(item.Value);
                            Debug.Log(responseType == null);
                            Debug.Log(response == null);
                            //Debug.Log(responseType.GetProperty(item.Key));
                            switch (item.Key)
                            {
                                case "VersionID":
                                    response.VersionID = (double)item.Value;
                                    break;
                                case "Timetoken":
                                    Int64 res;
                                    if (Int64.TryParse(item.Value.ToString(), out res))
                                    {
                                        response.Timetoken = res;
                                    }

                                    break;
                                case "OperationName":
                                    response.OperationName = item.Value.ToString();
                                    break;
                                case "DemoMessage":
                                    Debug.Log(item.Value.GetType());
                                    //response.DemoMessage = (PubnubDemoMessage)item.Value;
                                    break;
                                case "CustomMessage":
                                    Debug.Log(item.Value.GetType());
                                    //response.CustomMessage = (PubnubDemoMessage)item.Value;
                                    break;
                                default:
                                    break;
                            }
                            //responseType.GetProperty(item.Key).SetValue(response, item.Value, null);
                        }
                        //PubnubDemoObject response = mea.MessageResult.Payload;
                        Debug.Log("expected.VersionID:" + expected.VersionID);
                        Debug.Log("response.VersionID:" + response.VersionID);

                        bool versionIdMatch = response.VersionID.Equals(expected.VersionID);
                        bool timetokenMatch = response.Timetoken.Equals(expected.Timetoken);
                        bool operationNameMatch = response.OperationName.Equals(expected.OperationName);
                        bool demoMessageMatch = response.DemoMessage.DefaultMessage.Equals(expected.DemoMessage.DefaultMessage);
                        bool customMessageMatch = response.CustomMessage.DefaultMessage.Equals(expected.CustomMessage.DefaultMessage);

                        testReturn = versionIdMatch && timetokenMatch && operationNameMatch && demoMessageMatch && customMessageMatch;
                    }
                    else
                    {
                        Debug.Log("PAYLOAD24:" + payload.ToString() + payload.GetType());
                        testReturn = mea.MessageResult.Payload.Equals(payload);
                    }
                    //testReturn = true;
                }
            };
            pubnub.Subscribe().Channels(channelList2).Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            pubnub.Publish().Channel(publishChannel).Message(payload).Async((result, status) =>
            {
                Assert.True(!result.Timetoken.Equals(0));
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(testReturn, "test didn't return");
            pubnub.CleanUp();
        }

        [UnityTest]
        public IEnumerator TestSubscribeWithTT()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next(100);
            string channel = "UnityTestWithTTLChannel";
            string payload = string.Format("payload {0}", pnConfiguration.UUID);

            PubNub pubnub = new PubNub(pnConfiguration);
            long timetoken = 0;
            pubnub.Time().Async((result, status) =>
            {
                timetoken = result.TimeToken;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(!timetoken.Equals(0));

            pubnub.Publish().Channel(channel).Message(payload).Async((result, status) =>
            {
                Assert.True(!result.Timetoken.Equals(0));
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
            });

            List<string> channelList2 = new List<string>();
            channelList2.Add(channel);
            bool tresult = false;

            pubnub.SubscribeCallback += (sender, e) =>
            {
                SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;
                if (!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory))
                {
                    tresult = mea.MessageResult.Channel.Equals(channel) && mea.MessageResult.Payload.ToString().Equals(payload);
                }
            };
            pubnub.Subscribe().Channels(channelList2).SetTimeToken(timetoken).Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "test didn't return");
            pubnub.CleanUp();

        }

        [UnityTest]
        public IEnumerator TestSignalsAndSubscribe()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next(100);
            string channel = "UnityTestSignalChannel_" + r.Next(100);
            string payload = string.Format("Signal {0}", r.Next(100));

            PubNub pubnub = new PubNub(pnConfiguration);
            List<string> channelList2 = new List<string>();
            channelList2.Add(channel);
            bool tresult = false;

            pubnub.SubscribeCallback += (sender, e) =>
            {
                SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;
                if (mea.SignalEventResult != null)
                {
                    tresult = mea.SignalEventResult.Channel.Equals(channel) && mea.SignalEventResult.Payload.ToString().Equals(payload);
                    Debug.Log("Signal tresult:" + tresult + channel + payload);
                }
            };
            pubnub.Subscribe().Channels(channelList2).Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            pubnub.Signal().Channel(channel).Message(payload).Async((result, status) =>
            {
                Assert.True(!result.Timetoken.Equals(0));
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "test didn't return");
            pubnub.CleanUp();

        }

        [UnityTest]
        public IEnumerator TestMessageActions()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next(100);
            int ran = r.Next(10000);
            string channel = "message_actions_channel_" + ran;
            string message = "message_actions_message_" + ran;

            PubNub pubnubSub = new PubNub(pnConfiguration);
            PNConfiguration pnConfiguration2 = PlayModeCommon.SetPNConfig(false);
            pnConfiguration2.UUID = "UnityTestConnectedUUID_" + r.Next(100);
            PubNub pubnubMA = new PubNub(pnConfiguration2);
            List<string> channelList2 = new List<string>();
            channelList2.Add(channel);

            bool maAdd = false;
            bool maDelete = false;
            long messageTimetoken = 0;
            long messageActionTimetoken = 0;

            // Add MessageActionsEvent listener
            pubnubSub.SubscribeCallback += (sender, e) =>
            {
                SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;
                if (mea.MessageActionsEventResult != null)
                {
                    maAdd = mea.MessageActionsEventResult.Channel.Equals(channel) && mea.MessageActionsEventResult.Data.UUID.Equals(pnConfiguration2.UUID) && mea.MessageActionsEventResult.MessageActionsEvent.Equals(PNMessageActionsEvent.PNMessageActionsEventAdded);
                    maDelete = mea.MessageActionsEventResult.Channel.Equals(channel) && mea.MessageActionsEventResult.Data.UUID.Equals(pnConfiguration2.UUID) && mea.MessageActionsEventResult.MessageActionsEvent.Equals(PNMessageActionsEvent.PNMessageActionsEventRemoved);
                    Debug.Log(mea.MessageActionsEventResult.Channel);
                    if (mea.MessageActionsEventResult.Data != null)
                    {
                        Debug.Log(mea.MessageActionsEventResult.Data.ActionTimetoken);
                        Debug.Log(mea.MessageActionsEventResult.Data.ActionType);
                        Debug.Log(mea.MessageActionsEventResult.Data.ActionValue);
                        Debug.Log(mea.MessageActionsEventResult.Data.MessageTimetoken);
                        Debug.Log(mea.MessageActionsEventResult.Data.UUID);
                    }
                    Debug.Log(mea.MessageActionsEventResult.MessageActionsEvent);
                    Debug.Log(mea.MessageActionsEventResult.Subscription);
                }
            };
            // Subscribe to MessageActionsEvent
            pubnubSub.Subscribe().Channels(channelList2).Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            // Publish with meta
            Dictionary<string, string> metaDict = new Dictionary<string, string>();
            metaDict.Add("region", "east");

            bool testPubReturn = false;
            pubnubSub.Publish().Channel(channel).Message(message).Meta(metaDict).Async((result, status) =>
            {
                // Read tt
                messageTimetoken = result.Timetoken;
                Assert.True(!result.Timetoken.Equals(0));
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                testPubReturn = true;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(testPubReturn, "test didn't return");

            MessageActionAdd maa = new MessageActionAdd();
            maa.ActionType = "reaction";
            maa.ActionValue = "smiley_face";

            bool testAMAReturn = false;

            // Add message actions
            pubnubMA.AddMessageActions().Channel(channel).MessageAction(maa).MessageTimetoken(messageTimetoken).Async((result, status) =>
            {
                // Read MA TT
                Debug.Log("result.ActionTimetoken: " + result.ActionTimetoken);
                Debug.Log("result.ActionType: " + result.ActionType);
                Debug.Log("result.ActionValue: " + result.ActionValue);
                Debug.Log("result.MessageTimetoken: " + result.MessageTimetoken);
                Debug.Log("result.UUID: " + result.UUID);

                messageActionTimetoken = result.ActionTimetoken;
                Assert.True(maa.ActionType.Equals(result.ActionType));
                Assert.True(maa.ActionValue.Equals(result.ActionValue));
                Assert.True(messageTimetoken.Equals(result.MessageTimetoken));
                Assert.True(pnConfiguration2.UUID.Equals(result.UUID));
                testAMAReturn = true;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(testAMAReturn, "test didn't return");

            bool testGMAReturn = false;
            // Get message actions CH only
            pubnubMA.GetMessageActions().Channel(channel).Async((result, status) =>
            {
                testGMAReturn = MatchGMA(result, maa.ActionType, maa.ActionValue, messageActionTimetoken, messageTimetoken, pnConfiguration2.UUID);
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(testGMAReturn, "test didn't return");

            // Get message actions start
            testGMAReturn = false;
            pubnubMA.GetMessageActions().Channel(channel).Start(messageActionTimetoken + 1).Async((result, status) =>
            {
                testGMAReturn = MatchGMA(result, maa.ActionType, maa.ActionValue, messageActionTimetoken, messageTimetoken, pnConfiguration2.UUID);
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(testGMAReturn, "start test didn't return");

            // Get message actions start end
            testGMAReturn = false;
            pubnubMA.GetMessageActions().Channel(channel).Start(messageActionTimetoken + 1).End(messageActionTimetoken).Async((result, status) =>
            {
                testGMAReturn = MatchGMA(result, maa.ActionType, maa.ActionValue, messageActionTimetoken, messageTimetoken, pnConfiguration2.UUID);
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(testGMAReturn, "start end test didn't return");

            // Get message actions ch limit
            testGMAReturn = false;
            pubnubMA.GetMessageActions().Channel(channel).Limit(1).Async((result, status) =>
            {
                testGMAReturn = MatchGMA(result, maa.ActionType, maa.ActionValue, messageActionTimetoken, messageTimetoken, pnConfiguration2.UUID);
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(testGMAReturn, "limit test didn't return");

            // Fetch With Message Actions
            bool tresultMA = false;

            pubnubMA.FetchMessages().Channels(channelList2).IncludeMessageActions(true).Async((result, status) =>
            {
                if (!status.Error)
                {
                    if (result.Channels != null)
                    {
                        tresultMA = MatchFetchMA(result, pnConfiguration2.UUID, messageActionTimetoken, message, channel, maa.ActionType, maa.ActionValue);
                    }

                }

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresultMA, "test didnt return for fetch");

            // Remove message Actions
            bool testRMAReturn = false;
            pubnubMA.RemoveMessageActions().ActionTimetoken(messageActionTimetoken).Channel(channel).MessageTimetoken(messageTimetoken).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                testRMAReturn = true;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(testRMAReturn, "remove test didn't return");
            pubnubSub.CleanUp();
        }

        public bool MatchFetchMA(PNFetchMessagesResult result, string uuid, long messageActionTimetoken, string message, string channel, string actionType1, string actionValue1)
        {
            Dictionary<string, List<PNMessageResult>> fetchResult = result.Channels as Dictionary<string, List<PNMessageResult>>;
            Debug.Log("fetchResult.Count:" + fetchResult.Count);
            foreach (KeyValuePair<string, List<PNMessageResult>> kvp in fetchResult)
            {
                Debug.Log("Channel:" + kvp.Key);
                if (kvp.Key.Equals(channel))
                {

                    foreach (PNMessageResult msg in kvp.Value)
                    {
                        Debug.Log("msg.Channel:" + msg.Channel);
                        Debug.Log("msg.Payload.ToString():" + msg.Payload.ToString());
                        if (msg.Channel.Equals(channel) && (msg.Payload.ToString().Equals(message)))
                        {

                            if (msg.MessageActions != null)
                            {
                                Debug.Log("msg.MessageActions:" + msg.MessageActions.Count);
                                foreach (KeyValuePair<string, PNHistoryMessageActionsTypeValues> kvpActionTypes in msg.MessageActions)
                                {
                                    string actionType = kvpActionTypes.Key;
                                    Debug.Log("actionType:" + actionType);
                                    PNHistoryMessageActionsTypeValues pnHistoryMessageActionsTypeValues = kvpActionTypes.Value;
                                    foreach (KeyValuePair<string, List<PNHistoryMessageActionsTypeValueAttributes>> kvpActionValues in pnHistoryMessageActionsTypeValues.MessageActionsTypeValues)
                                    {
                                        string actionValue = kvpActionValues.Key;
                                        Debug.Log("actionValue:" + actionValue);
                                        foreach (PNHistoryMessageActionsTypeValueAttributes p in kvpActionValues.Value)
                                        {
                                            string UUID = p.UUID;
                                            Debug.Log("UUID:" + UUID);
                                            Debug.Log("UUID:" + uuid);
                                            long actionTimetoken = p.ActionTimetoken;
                                            Debug.Log("actionTimetoken:" + actionTimetoken);
                                            Debug.Log("messageActionTimetoken:" + messageActionTimetoken);
                                            Debug.Log(actionType.Equals(actionType1));
                                            Debug.Log(actionValue.Equals(actionValue1));
                                            Debug.Log(UUID.Equals(uuid));
                                            Debug.Log(actionTimetoken.Equals(messageActionTimetoken));
                                            if (actionType.Equals(actionType1) && actionValue.Equals(actionValue1) && UUID.Equals(uuid) && actionTimetoken.Equals(messageActionTimetoken))
                                            {
                                                Debug.Log("MatchFetchMA: true");
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool MatchGMA(PNGetMessageActionsResult result, string ActionType, string ActionValue, long messageActionTimetoken, long messageTimetoken, string UUID)
        {
            if ((result.Data != null) && (result.Data.Count > 0))
            {
                Debug.Log("result.ActionTimetoken: " + result.Data[0].ActionTimetoken);
                Debug.Log("result.ActionType: " + result.Data[0].ActionType);
                Debug.Log("result.ActionValue: " + result.Data[0].ActionValue);
                Debug.Log("result.MessageTimetoken: " + result.Data[0].MessageTimetoken);
                Debug.Log("result.UUID: " + result.Data[0].UUID);
                Assert.True(ActionType.Equals(result.Data[0].ActionType));
                Assert.True(ActionValue.Equals(result.Data[0].ActionValue));
                Assert.True(messageTimetoken.Equals(result.Data[0].MessageTimetoken));
                Assert.True(messageActionTimetoken.Equals(result.Data[0].ActionTimetoken));
                Assert.True(UUID.Equals(result.Data[0].UUID));
                return true;
            }
            else
            {
                return false;
            }
        }

        [UnityTest]
        public IEnumerator TestMembersAndMemberships()
        {
            yield return TestMembersAndMembershipsCommon(false);
        }

        [UnityTest]
        public IEnumerator TestMembersAndMembershipsWithPAM()
        {
            yield return TestMembersAndMembershipsCommon(true);
        }

        public IEnumerator TestMembersAndMembershipsCommon(bool withPAM)
        {
            //Create user 1
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            PubNub pnPAM = new PubNub(pnConfiguration);
            if (withPAM)
            {
                pnConfiguration = PlayModeCommon.SetPAMPNConfig(false);
                pnPAM = new PubNub(pnConfiguration);
            }

            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next(10000);
            int ran = r.Next(10000);
            int ran2 = r.Next(10000);
            string userid = "userid" + ran;
            string name = string.Format("user name {0}", ran);
            string email = string.Format("user email {0}", ran);
            string externalID = string.Format("user externalID {0}", ran);
            string profileURL = string.Format("user profileURL {0}", ran);
            string spaceid = "spaceid" + ran;
            string spacename = string.Format("space name {0}", ran);
            string spacedesc = string.Format("space desc {0}", ran);
            string userid2 = "userid" + ran2;
            string name2 = string.Format("user name {0}", ran2);
            string email2 = string.Format("user email {0}", ran2);
            string externalID2 = string.Format("user externalID {0}", ran2);
            string profileURL2 = string.Format("user profileURL {0}", ran2);
            string spaceid2 = "spaceid" + ran2;
            string spacename2 = string.Format("space name {0}", ran2);
            string spacedesc2 = string.Format("space desc {0}", ran2);

            PNUserSpaceInclude[] include = new PNUserSpaceInclude[] { PNUserSpaceInclude.PNUserSpaceCustom };

            string[] sort = new string[] { "updated:desc" };

            var resUsers = new Dictionary<string, int>(){
                    {userid, 31},
                    {userid2, 31},
                };

            var resSpaces = new Dictionary<string, int>(){
                    {spaceid, 31},
                    {spaceid2, 31},
                };
            string token = "";

            if (withPAM)
            {
                pnPAM.GrantToken().SetParams(resUsers, resSpaces, new Dictionary<string, int>(), new Dictionary<string, int>(), 3).Async((result, status) =>
                {
                    Debug.Log("GrantToken response:::" + result.Token);
                    token = result.Token;
                });

                yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls4);
                string g4 = pnPAM.GetToken(userid, PNResourceType.PNUsers);

                Debug.Log("GetToken response" + g4);
                Assert.AreEqual(token, g4);

            }

            pnConfiguration.SecretKey = "";
            PubNub pubnub = new PubNub(pnConfiguration);
            if (!string.IsNullOrEmpty(token))
            {
                pubnub.SetToken(token);
            }

            bool tresult = false;

            Dictionary<string, object> userCustom = new Dictionary<string, object>();
            userCustom.Add("usercustomkey1", "ucv1");
            userCustom.Add("usercustomkey2", "ucv2");

            Dictionary<string, object> userCustom2 = new Dictionary<string, object>();
            userCustom2.Add("usercustomkey21", "ucv21");
            userCustom2.Add("usercustomkey22", "ucv22");

            Dictionary<string, object> spaceCustom = new Dictionary<string, object>();
            spaceCustom.Add("spacecustomkey1", "scv1");
            spaceCustom.Add("spacecustomkey2", "scv2");

            Dictionary<string, object> spaceCustom2 = new Dictionary<string, object>();
            spaceCustom2.Add("spacecustomkey21", "scv21");
            spaceCustom2.Add("spacecustomkey22", "scv22");

            pubnub.CreateUser().Email(email).ExternalID(externalID).Name(name).ID(userid).Include(include).Custom(userCustom).ProfileURL(profileURL).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Assert.AreEqual(name, result.Name);
                Assert.AreEqual(email, result.Email);
                Assert.AreEqual(externalID, result.ExternalID);
                Assert.AreEqual(profileURL, result.ProfileURL);
                Assert.AreEqual(userid, result.ID);
                Assert.AreEqual(result.Updated, result.Created);
                Assert.True(!string.IsNullOrEmpty(result.ETag), result.ETag);
                Assert.True("ucv1" == result.Custom["usercustomkey1"].ToString());
                Assert.True("ucv2" == result.Custom["usercustomkey2"].ToString());
                tresult = true;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls4);
            Assert.True(tresult, "CreateUser didn't return");

            tresult = false;

            pubnub.GetUser().ID(userid).Include(include).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Assert.AreEqual(name, result.Name);
                Assert.AreEqual(email, result.Email);
                Assert.AreEqual(externalID, result.ExternalID);
                Assert.AreEqual(profileURL, result.ProfileURL);
                Assert.AreEqual(userid, result.ID);
                Assert.AreEqual(result.Updated, result.Created);
                Assert.True(!string.IsNullOrEmpty(result.ETag), result.ETag);
                Assert.True("ucv1" == result.Custom["usercustomkey1"].ToString());
                Assert.True("ucv2" == result.Custom["usercustomkey2"].ToString());
                tresult = true;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tresult, "GetUser didn't return");

            tresult = false;
            //Create user 2

            pubnub.CreateUser().Email(email2).ExternalID(externalID2).Name(name2).ID(userid2).Include(include).Custom(userCustom2).ProfileURL(profileURL2).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Assert.AreEqual(name2, result.Name);
                Assert.AreEqual(email2, result.Email);
                Assert.AreEqual(externalID2, result.ExternalID);
                Assert.AreEqual(profileURL2, result.ProfileURL);
                Assert.AreEqual(userid2, result.ID);
                Assert.AreEqual(result.Updated, result.Created);
                Assert.True(!string.IsNullOrEmpty(result.ETag), result.ETag);
                Assert.True("ucv21" == result.Custom["usercustomkey21"].ToString());
                Assert.True("ucv22" == result.Custom["usercustomkey22"].ToString());
                tresult = true;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "CreateUser didn't return");

            tresult = false;
            //Create space 1

            pubnub.CreateSpace().Name(spacename).ID(spaceid).Include(include).Custom(spaceCustom).Description(spacedesc).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Assert.AreEqual(spacename, result.Name);
                Assert.AreEqual(spacedesc, result.Description);
                Assert.AreEqual(spaceid, result.ID);
                Assert.AreEqual(result.Updated, result.Created);
                Assert.True(!string.IsNullOrEmpty(result.ETag), result.ETag);
                Assert.True("scv1" == result.Custom["spacecustomkey1"].ToString());
                Assert.True("scv2" == result.Custom["spacecustomkey2"].ToString());
                tresult = true;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "CreateSpace didn't return");

            tresult = false;

            pubnub.GetSpace().ID(spaceid).Include(include).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Assert.AreEqual(spacename, result.Name);
                Assert.AreEqual(spacedesc, result.Description);
                Assert.AreEqual(spaceid, result.ID);
                Assert.AreEqual(result.Updated, result.Created);
                Assert.True(!string.IsNullOrEmpty(result.ETag), result.ETag);
                Assert.True("scv1" == result.Custom["spacecustomkey1"].ToString());
                Assert.True("scv2" == result.Custom["spacecustomkey2"].ToString());
                tresult = true;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "GetSpace didn't return");

            tresult = false;

            //Create space 2
            pubnub.CreateSpace().Name(spacename2).ID(spaceid2).Include(include).Custom(spaceCustom2).Description(spacedesc2).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Assert.AreEqual(spacename2, result.Name);
                Assert.AreEqual(spacedesc2, result.Description);
                Assert.AreEqual(spaceid2, result.ID);
                Assert.AreEqual(result.Updated, result.Created);
                Assert.True(!string.IsNullOrEmpty(result.ETag), result.ETag);
                Assert.True("scv21" == result.Custom["spacecustomkey21"].ToString());
                Assert.True("scv22" == result.Custom["spacecustomkey22"].ToString());
                tresult = true;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls4);
            Assert.True(tresult, "CreateSpace didn't return");

            PNMembersInput input = new PNMembersInput();
            input.ID = userid;
            Dictionary<string, object> membersCustom2 = new Dictionary<string, object>();
            membersCustom2.Add("memberscustomkey21", "mcv21");
            membersCustom2.Add("memberscustomkey22", "mcv22");

            input.Custom = membersCustom2;
            int limit = 100;
            bool count = true;
            tresult = false;

            PNMembersInclude[] inclSm = new PNMembersInclude[] { PNMembersInclude.PNMembersCustom, PNMembersInclude.PNMembersUser, PNMembersInclude.PNMembersUserCustom };
            PNMembershipsInclude[] inclMem = new PNMembershipsInclude[] { PNMembershipsInclude.PNMembershipsCustom, PNMembershipsInclude.PNMembershipsSpace, PNMembershipsInclude.PNMembershipsSpaceCustom };

            //Add Space Memberships
            pubnub.ManageMembers().SpaceID(spaceid).Add(new List<PNMembersInput> { input }).Update(new List<PNMembersInput> { }).Remove(new List<PNMembersRemove> { }).Include(inclSm).Limit(limit).Count(count).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                bool bFound = false;
                Debug.Log("Looking for " + userid);
                Debug.Log("result.Next:" + result.Next);
                Debug.Log("result.Prev:" + result.Prev);
                Debug.Log("result.TotalCount:" + result.TotalCount);
                Assert.True(result.TotalCount > 0);
                foreach (PNMembers mem in result.Data)
                {

                    Debug.Log("Found mem " + mem.User.ID);
                    if (mem.User.ID.Equals(userid))
                    {
                        Assert.AreEqual(name, mem.User.Name);
                        Assert.AreEqual(email, mem.User.Email);
                        Assert.AreEqual(externalID, mem.User.ExternalID);
                        Assert.AreEqual(profileURL, mem.User.ProfileURL);
                        Assert.AreEqual(userid, mem.User.ID);
                        Assert.AreEqual(mem.User.Updated, mem.User.Created);

                        Assert.True(!string.IsNullOrEmpty(mem.User.ETag), mem.User.ETag);
                        Assert.True("ucv1" == mem.User.Custom["usercustomkey1"].ToString());
                        Assert.True("ucv2" == mem.User.Custom["usercustomkey2"].ToString());
                        Assert.True("mcv21" == mem.Custom["memberscustomkey21"].ToString());
                        Assert.True("mcv22" == mem.Custom["memberscustomkey22"].ToString());

                        bFound = true;
                        break;
                    }

                }
                tresult = bFound;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls5);
            Assert.True(tresult, "ManageMembers didn't return");

            //Update Space Memberships
            if (!withPAM)
            {
                PNMembersInput inputUp = new PNMembersInput();
                inputUp.ID = userid;
                Dictionary<string, object> membersCustomUp = new Dictionary<string, object>();
                membersCustomUp.Add("memberscustomkeyup21", "mcvup21");
                membersCustomUp.Add("memberscustomkeyup22", "mcvup22");

                inputUp.Custom = membersCustomUp;

                tresult = false;
                pubnub.ManageMembers().SpaceID(spaceid).Add(new List<PNMembersInput> { }).Update(new List<PNMembersInput> { inputUp }).Remove(new List<PNMembersRemove> { }).Include(inclSm).Limit(limit).Count(count).Async((result, status) =>
                {
                    Assert.True(status.Error.Equals(false));
                    Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                    bool bFound = false;
                    Debug.Log("Looking for " + userid);
                    foreach (PNMembers mem in result.Data)
                    {

                        Debug.Log("Found mem " + mem.User.ID);
                        if (mem.User.ID.Equals(userid))
                        {
                            Assert.AreEqual(name, mem.User.Name);
                            Assert.AreEqual(email, mem.User.Email);
                            Assert.AreEqual(externalID, mem.User.ExternalID);
                            Assert.AreEqual(profileURL, mem.User.ProfileURL);
                            Assert.AreEqual(userid, mem.User.ID);
                            Assert.AreEqual(mem.User.Updated, mem.User.Created);
                            Assert.True(!string.IsNullOrEmpty(mem.User.ETag), mem.User.ETag);
                            Assert.True("ucv1" == mem.User.Custom["usercustomkey1"].ToString());
                            Assert.True("ucv2" == mem.User.Custom["usercustomkey2"].ToString());
                            Assert.True("mcvup21" == mem.Custom["memberscustomkeyup21"].ToString());
                            Assert.True("mcvup22" == mem.Custom["memberscustomkeyup22"].ToString());

                            bFound = true;
                            break;
                        }

                    }
                    tresult = bFound;

                });
                yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls4);
                Assert.True(tresult, "ManageMembers Update didn't return");

                // Sort feature test for ManageMembers
                tresult = false;
                pubnub.ManageMembers().SpaceID(spaceid).Sort(sort).Limit(1).Add(new List<PNMembersInput> { }).Update(new List<PNMembersInput> { inputUp }).Remove(new List<PNMembersRemove> { }).Include(inclSm).Limit(limit).Count(count).Async((result, status) =>
                {
                    Assert.True(status.Error.Equals(false));
                    Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                    bool bFound = false;
                    Debug.Log("Looking for " + userid);
                    foreach (PNMembers mem in result.Data)
                    {
                        Debug.Log("Found mem " + mem.User.ID);
                        if (mem.User.ID.Equals(userid))
                        {
                            Assert.AreEqual(name, mem.User.Name);
                            Assert.AreEqual(email, mem.User.Email);
                            Assert.AreEqual(externalID, mem.User.ExternalID);
                            Assert.AreEqual(profileURL, mem.User.ProfileURL);
                            Assert.AreEqual(userid, mem.User.ID);
                            Assert.AreEqual(mem.User.Updated, mem.User.Created);
                            Assert.True(!string.IsNullOrEmpty(mem.User.ETag), mem.User.ETag);
                            Assert.True("ucv1" == mem.User.Custom["usercustomkey1"].ToString());
                            Assert.True("ucv2" == mem.User.Custom["usercustomkey2"].ToString());
                            Assert.True("mcvup21" == mem.Custom["memberscustomkeyup21"].ToString());
                            Assert.True("mcvup22" == mem.Custom["memberscustomkeyup22"].ToString());
                            bFound = true;
                            break;
                        }

                    }
                    tresult = bFound;
                });
                yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls4);
                Assert.True(tresult, "ManageMembers Sort didn't return");

            }

            //Get Space Memberships
            tresult = false;
            pubnub.GetMemberships().UserID(userid).Include(inclMem).Limit(limit).Count(count).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());

                bool bFound = false;
                Debug.Log("Looking for " + spaceid);
                Debug.Log("result.Next:" + result.Next);
                Debug.Log("result.Prev:" + result.Prev);
                Debug.Log("result.TotalCount:" + result.TotalCount);
                Assert.True(result.TotalCount > 0);

                foreach (PNMemberships mem in result.Data)
                {

                    Debug.Log("Found mem " + mem.Space.ID);
                    if (mem.Space.ID.Equals(spaceid))
                    {
                        Assert.AreEqual(spacename, mem.Space.Name);
                        Assert.AreEqual(spacedesc, mem.Space.Description);
                        Assert.AreEqual(spaceid, mem.Space.ID);
                        Assert.AreEqual(mem.Space.Updated, mem.Space.Created);
                        Assert.True(!string.IsNullOrEmpty(mem.Space.ETag), mem.Space.ETag);
                        foreach (KeyValuePair<string, object> kvp in mem.Space.Custom)
                        {
                            Debug.Log(kvp.Key + kvp.Value);
                        }
                        Assert.True("scv1" == mem.Space.Custom["spacecustomkey1"].ToString());
                        Assert.True("scv2" == mem.Space.Custom["spacecustomkey2"].ToString());
                        if (withPAM)
                        {
                            Assert.True("mcv21" == mem.Custom["memberscustomkey21"].ToString());
                            Assert.True("mcv22" == mem.Custom["memberscustomkey22"].ToString());
                        }
                        else
                        {
                            Assert.True("mcvup21" == mem.Custom["memberscustomkeyup21"].ToString());
                            Assert.True("mcvup22" == mem.Custom["memberscustomkeyup22"].ToString());
                        }


                        bFound = true;
                        break;
                    }

                }

                tresult = bFound;
            });

            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls4);
            Assert.True(tresult, "GetMemberships didn't return");

            string filterMemberships = string.Format("space.name == '{0}'", spacename);

            //Get Space Memberships with filter
            tresult = false;
            pubnub.GetMemberships().UserID(userid).Include(inclMem).Limit(limit).Filter(filterMemberships).Count(count).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());

                bool bFound = false;
                Debug.Log("Looking for " + spaceid);
                Debug.Log("result.Next:" + result.Next);
                Debug.Log("result.Prev:" + result.Prev);
                Debug.Log("result.TotalCount:" + result.TotalCount);
                Assert.True(result.TotalCount > 0);

                foreach (PNMemberships mem in result.Data)
                {

                    Debug.Log("Found mem " + mem.Space.ID);
                    if (mem.Space.ID.Equals(spaceid))
                    {
                        Assert.AreEqual(spacename, mem.Space.Name);
                        Assert.AreEqual(spacedesc, mem.Space.Description);
                        Assert.AreEqual(spaceid, mem.Space.ID);
                        Assert.AreEqual(mem.Space.Updated, mem.Space.Created);
                        Assert.True(!string.IsNullOrEmpty(mem.Space.ETag), mem.Space.ETag);
                        foreach (KeyValuePair<string, object> kvp in mem.Space.Custom)
                        {
                            Debug.Log(kvp.Key + kvp.Value);
                        }
                        Assert.True("scv1" == mem.Space.Custom["spacecustomkey1"].ToString());
                        Assert.True("scv2" == mem.Space.Custom["spacecustomkey2"].ToString());
                        if (withPAM)
                        {
                            Assert.True("mcv21" == mem.Custom["memberscustomkey21"].ToString());
                            Assert.True("mcv22" == mem.Custom["memberscustomkey22"].ToString());
                        }
                        else
                        {
                            Assert.True("mcvup21" == mem.Custom["memberscustomkeyup21"].ToString());
                            Assert.True("mcvup22" == mem.Custom["memberscustomkeyup22"].ToString());
                        }

                        bFound = true;
                        break;
                    }

                }

                tresult = bFound;
            });

            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls4);
            Assert.True(tresult, "GetMembershipsFilter didn't return");

            // Sort feature test for GetMemberships
            tresult = false;
            pubnub.GetMemberships().UserID(userid).Include(inclMem).Sort(sort).Limit(1).Count(count).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                bool bFound = false;
                Debug.Log("Looking for " + spaceid);
                Debug.Log("result.Next:" + result.Next);
                Debug.Log("result.Prev:" + result.Prev);
                Debug.Log("result.TotalCount:" + result.TotalCount);
                Assert.True(result.TotalCount > 0);
                foreach (PNMemberships mem in result.Data)
                {
                    Debug.Log("Found mem " + mem.Space.ID);
                    if (mem.Space.ID.Equals(spaceid))
                    {
                        Assert.AreEqual(spacename, mem.Space.Name);
                        Assert.AreEqual(spacedesc, mem.Space.Description);
                        Assert.AreEqual(spaceid, mem.Space.ID);
                        Assert.AreEqual(mem.Space.Updated, mem.Space.Created);
                        Assert.True(!string.IsNullOrEmpty(mem.Space.ETag), mem.Space.ETag);
                        foreach (KeyValuePair<string, object> kvp in mem.Space.Custom)
                        {
                            Debug.Log(kvp.Key + kvp.Value);
                        }
                        Assert.True("scv1" == mem.Space.Custom["spacecustomkey1"].ToString());
                        Assert.True("scv2" == mem.Space.Custom["spacecustomkey2"].ToString());
                        if (withPAM)
                        {
                            Assert.True("mcv21" == mem.Custom["memberscustomkey21"].ToString());
                            Assert.True("mcv22" == mem.Custom["memberscustomkey22"].ToString());
                        }
                        else
                        {
                            Assert.True("mcvup21" == mem.Custom["memberscustomkeyup21"].ToString());
                            Assert.True("mcvup22" == mem.Custom["memberscustomkeyup22"].ToString());
                        }
                        bFound = true;
                        break;
                    }
                }
                tresult = bFound;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls4);
            Assert.True(tresult, "GetMemberships Sort didn't return");

            //Remove Space Memberships
            PNMembersRemove inputRm = new PNMembersRemove();
            inputRm.ID = userid;

            tresult = false;
            pubnub.ManageMembers().SpaceID(spaceid).Add(new List<PNMembersInput> { }).Update(new List<PNMembersInput> { }).Remove(new List<PNMembersRemove> { inputRm }).Include(inclSm).Limit(limit).Count(count).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                bool bFound = true;
                Debug.Log("Looking for " + userid);
                foreach (PNMembers mem in result.Data)
                {

                    Debug.Log("Found mem " + mem.User.ID);
                    if (mem.User.ID.Equals(userid))
                    {
                        Assert.AreEqual(name, mem.User.Name);
                        Assert.AreEqual(email, mem.User.Email);
                        Assert.AreEqual(externalID, mem.User.ExternalID);
                        Assert.AreEqual(profileURL, mem.User.ProfileURL);
                        Assert.AreEqual(userid, mem.User.ID);
                        Assert.AreEqual(mem.User.Updated, mem.User.Created);
                        Assert.True(!string.IsNullOrEmpty(mem.User.ETag), mem.User.ETag);
                        Assert.True("ucv1" == mem.User.Custom["usercustomkey1"].ToString());
                        Assert.True("ucv2" == mem.User.Custom["usercustomkey2"].ToString());
                        Assert.True("mcv21" == mem.Custom["memberscustomkey21"].ToString());
                        Assert.True("mcv22" == mem.Custom["memberscustomkey22"].ToString());

                        bFound = false;
                        break;
                    }

                }
                tresult = bFound;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls4);
            Assert.True(tresult, "ManageMembers Remove didn't return");

            //Add user memberships
            PNMembersInput inputMemberships = new PNMembersInput();
            inputMemberships.ID = spaceid2;
            Dictionary<string, object> membershipCustom2 = new Dictionary<string, object>();
            membershipCustom2.Add("mememberscustomkey21", "memcv21");
            membershipCustom2.Add("mememberscustomkey22", "memcv22");

            inputMemberships.Custom = membershipCustom2;
            tresult = false;

            pubnub.ManageMemberships().UserID(userid2).Add(new List<PNMembersInput> { inputMemberships }).Update(new List<PNMembersInput> { }).Remove(new List<PNMembersRemove> { }).Include(inclMem).Limit(limit).Count(count).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                bool bFound = false;
                Debug.Log("Looking for " + spaceid2);
                Debug.Log("result.Next:" + result.Next);
                Debug.Log("result.Prev:" + result.Prev);
                Debug.Log("result.TotalCount:" + result.TotalCount);
                Assert.True(result.TotalCount > 0);

                foreach (PNMemberships mem in result.Data)
                {

                    Debug.Log("Found mem " + mem.Space.ID);
                    if (mem.Space.ID.Equals(spaceid2))
                    {
                        Assert.AreEqual(spacename2, mem.Space.Name);
                        Assert.AreEqual(spacedesc2, mem.Space.Description);
                        Assert.AreEqual(spaceid2, mem.Space.ID);
                        Assert.AreEqual(mem.Space.Updated, mem.Space.Created);
                        Assert.True(!string.IsNullOrEmpty(mem.Space.ETag), mem.Space.ETag);
                        foreach (KeyValuePair<string, object> kvp in mem.Space.Custom)
                        {
                            Debug.Log(kvp.Key + kvp.Value);
                        }
                        Assert.True("scv21" == mem.Space.Custom["spacecustomkey21"].ToString());
                        Assert.True("scv22" == mem.Space.Custom["spacecustomkey22"].ToString());
                        Assert.True("memcv21" == mem.Custom["mememberscustomkey21"].ToString());
                        Assert.True("memcv22" == mem.Custom["mememberscustomkey22"].ToString());

                        bFound = true;
                        break;
                    }

                }

                tresult = bFound;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls5);
            Assert.True(tresult, "ManageMemberships didn't return");

            //Update user memberships
            PNMembersInput inputMembershipsUp = new PNMembersInput();
            inputMembershipsUp.ID = spaceid2;
            Dictionary<string, object> membershipCustomUp = new Dictionary<string, object>();
            membershipCustomUp.Add("mememberscustomkeyup21", "memcvup21");
            membershipCustomUp.Add("mememberscustomkeyup22", "memcvup22");

            inputMembershipsUp.Custom = membershipCustomUp;

            tresult = false;

            pubnub.ManageMemberships().UserID(userid2).Add(new List<PNMembersInput> { }).Update(new List<PNMembersInput> { inputMembershipsUp }).Remove(new List<PNMembersRemove> { }).Include(inclMem).Limit(limit).Count(count).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                bool bFound = false;
                Debug.Log("Looking for " + spaceid2);
                foreach (PNMemberships mem in result.Data)
                {

                    Debug.Log("Found mem " + mem.Space.ID);
                    if (mem.Space.ID.Equals(spaceid2))
                    {
                        Assert.AreEqual(spacename2, mem.Space.Name);
                        Assert.AreEqual(spacedesc2, mem.Space.Description);
                        Assert.AreEqual(spaceid2, mem.Space.ID);
                        Assert.AreEqual(mem.Space.Updated, mem.Space.Created);
                        Assert.True(!string.IsNullOrEmpty(mem.Space.ETag), mem.Space.ETag);
                        foreach (KeyValuePair<string, object> kvp in mem.Space.Custom)
                        {
                            Debug.Log(kvp.Key + kvp.Value);
                        }
                        Assert.True("scv21" == mem.Space.Custom["spacecustomkey21"].ToString());
                        Assert.True("scv22" == mem.Space.Custom["spacecustomkey22"].ToString());
                        Assert.True("memcvup21" == mem.Custom["mememberscustomkeyup21"].ToString());
                        Assert.True("memcvup22" == mem.Custom["mememberscustomkeyup22"].ToString());

                        bFound = true;
                        break;
                    }

                }

                tresult = bFound;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls4);
            Assert.True(tresult, "ManageMemberships Update didn't return");

            // Sort feature test for ManageMemberships
            tresult = false;

            pubnub.ManageMemberships().UserID(userid2).Sort(sort).Limit(1).Add(new List<PNMembersInput> { }).Update(new List<PNMembersInput> { inputMembershipsUp }).Remove(new List<PNMembersRemove> { }).Include(inclMem).Limit(limit).Count(count).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                bool bFound = false;
                Debug.Log("Looking for " + spaceid2);
                foreach (PNMemberships mem in result.Data)
                {

                    Debug.Log("Found mem " + mem.Space.ID);
                    if (mem.Space.ID.Equals(spaceid2))
                    {
                        Assert.AreEqual(spacename2, mem.Space.Name);
                        Assert.AreEqual(spacedesc2, mem.Space.Description);
                        Assert.AreEqual(spaceid2, mem.Space.ID);
                        Assert.AreEqual(mem.Space.Updated, mem.Space.Created);
                        Assert.True(!string.IsNullOrEmpty(mem.Space.ETag), mem.Space.ETag);
                        foreach (KeyValuePair<string, object> kvp in mem.Space.Custom)
                        {
                            Debug.Log(kvp.Key + kvp.Value);
                        }
                        Assert.True("scv21" == mem.Space.Custom["spacecustomkey21"].ToString());
                        Assert.True("scv22" == mem.Space.Custom["spacecustomkey22"].ToString());
                        Assert.True("memcvup21" == mem.Custom["mememberscustomkeyup21"].ToString());
                        Assert.True("memcvup22" == mem.Custom["mememberscustomkeyup22"].ToString());

                        bFound = true;
                        break;
                    }
                }
                tresult = bFound;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls4);
            Assert.True(tresult, "ManageMemberships Sort didn't return");


            string filterMembers = string.Format("user.name == '{0}'", name2);

            //Get members with Filter
            tresult = false;
            pubnub.GetMembers().SpaceID(spaceid2).Include(inclSm).Limit(limit).Count(count).Filter(filterMembers).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                bool bFound = false;
                Debug.Log("Looking for " + userid2);
                Debug.Log("result.Next:" + result.Next);
                Debug.Log("result.Prev:" + result.Prev);
                Debug.Log("result.TotalCount:" + result.TotalCount);
                Assert.True(result.TotalCount > 0);

                foreach (PNMembers mem in result.Data)
                {

                    Debug.Log("Found mem " + mem.User.ID);
                    if (mem.User.ID.Equals(userid2))
                    {
                        Assert.AreEqual(name2, mem.User.Name);
                        Assert.AreEqual(email2, mem.User.Email);
                        Assert.AreEqual(externalID2, mem.User.ExternalID);
                        Assert.AreEqual(profileURL2, mem.User.ProfileURL);
                        Assert.AreEqual(userid2, mem.User.ID);
                        Assert.AreEqual(mem.User.Updated, mem.User.Created);
                        Assert.True(!string.IsNullOrEmpty(mem.User.ETag), mem.User.ETag);
                        Assert.True("ucv21" == mem.User.Custom["usercustomkey21"].ToString());
                        Assert.True("ucv22" == mem.User.Custom["usercustomkey22"].ToString());
                        Assert.True("memcvup21" == mem.Custom["mememberscustomkeyup21"].ToString());
                        Assert.True("memcvup22" == mem.Custom["mememberscustomkeyup22"].ToString());

                        bFound = true;
                        break;
                    }

                }
                tresult = bFound;
            });

            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "GetMembersFilter didn't return");

            //Sort feature test for GetMembers
            tresult = false;
            pubnub.GetMembers().SpaceID(spaceid2).Include(inclSm).Sort(sort).Limit(1).Count(count).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                bool bFound = false;
                Debug.Log("Looking for " + userid2);
                Debug.Log("result.Next:" + result.Next);
                Debug.Log("result.Prev:" + result.Prev);
                Debug.Log("result.TotalCount:" + result.TotalCount);
                Assert.True(result.TotalCount > 0);
                foreach (PNMembers mem in result.Data)
                {
                    Debug.Log("Found mem " + mem.User.ID);
                    if (mem.User.ID.Equals(userid2))
                    {
                        Assert.AreEqual(name2, mem.User.Name);
                        Assert.AreEqual(email2, mem.User.Email);
                        Assert.AreEqual(externalID2, mem.User.ExternalID);
                        Assert.AreEqual(profileURL2, mem.User.ProfileURL);
                        Assert.AreEqual(userid2, mem.User.ID);
                        Assert.AreEqual(mem.User.Updated, mem.User.Created);
                        Assert.True(!string.IsNullOrEmpty(mem.User.ETag), mem.User.ETag);
                        Assert.True("ucv21" == mem.User.Custom["usercustomkey21"].ToString());
                        Assert.True("ucv22" == mem.User.Custom["usercustomkey22"].ToString());
                        Assert.True("memcvup21" == mem.Custom["mememberscustomkeyup21"].ToString());
                        Assert.True("memcvup22" == mem.Custom["mememberscustomkeyup22"].ToString());
                        bFound = true;
                        break;
                    }
                }
                tresult = bFound;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "GetMembers Sort didn't return");


            //Get members
            tresult = false;
            pubnub.GetMembers().SpaceID(spaceid2).Include(inclSm).Limit(limit).Count(count).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                bool bFound = false;
                Debug.Log("Looking for " + userid2);
                Debug.Log("result.Next:" + result.Next);
                Debug.Log("result.Prev:" + result.Prev);
                Debug.Log("result.TotalCount:" + result.TotalCount);
                Assert.True(result.TotalCount > 0);

                foreach (PNMembers mem in result.Data)
                {

                    Debug.Log("Found mem " + mem.User.ID);
                    if (mem.User.ID.Equals(userid2))
                    {
                        Assert.AreEqual(name2, mem.User.Name);
                        Assert.AreEqual(email2, mem.User.Email);
                        Assert.AreEqual(externalID2, mem.User.ExternalID);
                        Assert.AreEqual(profileURL2, mem.User.ProfileURL);
                        Assert.AreEqual(userid2, mem.User.ID);
                        Assert.AreEqual(mem.User.Updated, mem.User.Created);
                        Assert.True(!string.IsNullOrEmpty(mem.User.ETag), mem.User.ETag);
                        Assert.True("ucv21" == mem.User.Custom["usercustomkey21"].ToString());
                        Assert.True("ucv22" == mem.User.Custom["usercustomkey22"].ToString());
                        Assert.True("memcvup21" == mem.Custom["mememberscustomkeyup21"].ToString());
                        Assert.True("memcvup22" == mem.Custom["mememberscustomkeyup22"].ToString());

                        bFound = true;
                        break;
                    }

                }
                tresult = bFound;
            });

            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "GetMembers didn't return");

            //Remove user memberships
            PNMembersRemove inputMembershipsRm = new PNMembersRemove();
            inputMembershipsRm.ID = spaceid2;

            tresult = false;

            pubnub.ManageMemberships().UserID(userid2).Add(new List<PNMembersInput> { }).Update(new List<PNMembersInput> { }).Remove(new List<PNMembersRemove> { inputMembershipsRm }).Include(inclMem).Limit(limit).Count(count).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                bool bFound = true;
                Debug.Log("Looking for " + spaceid2);
                foreach (PNMemberships mem in result.Data)
                {

                    Debug.Log("Found mem " + mem.Space.ID);
                    if (mem.Space.ID.Equals(spaceid2))
                    {
                        Assert.AreEqual(spacename2, mem.Space.Name);
                        Assert.AreEqual(spacedesc2, mem.Space.Description);
                        Assert.AreEqual(spaceid2, mem.Space.ID);
                        Assert.AreEqual(mem.Space.Updated, mem.Space.Created);
                        Assert.True(!string.IsNullOrEmpty(mem.Space.ETag), mem.Space.ETag);
                        foreach (KeyValuePair<string, object> kvp in mem.Space.Custom)
                        {
                            Debug.Log(kvp.Key + kvp.Value);
                        }
                        Assert.True("scv21" == mem.Space.Custom["spacecustomkey21"].ToString());
                        Assert.True("scv22" == mem.Space.Custom["spacecustomkey22"].ToString());
                        Assert.True("memcv21" == mem.Custom["mememberscustomkey21"].ToString());
                        Assert.True("memcv22" == mem.Custom["mememberscustomkey22"].ToString());

                        bFound = false;
                        break;
                    }

                }

                tresult = bFound;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls5);
            Assert.True(tresult, "ManageMemberships Remove didn't return");

            //delete user 1
            tresult = false;

            pubnub.DeleteUser().ID(userid).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                tresult = true;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "DeleteUser didn't return");
            //delete space 1
            tresult = false;
            pubnub.DeleteSpace().ID(spaceid).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                tresult = true;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "DeleteSpace didn't return");
            //delete user 1
            tresult = false;
            pubnub.DeleteUser().ID(userid2).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                tresult = true;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "DeleteUser didn't return");
            //delete space 1
            tresult = false;
            pubnub.DeleteSpace().ID(spaceid2).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                tresult = true;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "DeleteSpace didn't return");

            pubnub.CleanUp();
        }

        [UnityTest]
        public IEnumerator TestObjectListeners()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next(100);
            int ran = r.Next(10000);
            string userid = "userid" + ran;
            string name = string.Format("user name {0}", ran);
            string email = string.Format("user email {0}", ran);
            string externalID = string.Format("user externalID {0}", ran);
            string profileURL = string.Format("user profileURL {0}", ran);
            string spaceid = "spaceid" + ran;
            string spacename = string.Format("space name {0}", ran);
            string spacedesc = string.Format("space desc {0}", ran);

            int ran2 = r.Next(1000);
            string name2 = string.Format("name {0}", ran2);
            string email2 = string.Format("email {0}", ran2);
            string externalID2 = string.Format("externalID {0}", ran2);
            string profileURL2 = string.Format("profileURL {0}", ran2);
            string spacename2 = string.Format("space name {0}", ran2);
            string spacedesc2 = string.Format("space desc {0}", ran2);

            PubNub pubnubSub = new PubNub(pnConfiguration);

            PubNub pubnubObjects = new PubNub(pnConfiguration);
            List<string> channelList2 = new List<string>();
            channelList2.Add(spaceid);
            channelList2.Add(userid);

            bool userUpdate = false;
            bool spaceUpdate = false;
            bool userDelete = false;
            bool spaceDelete = false;
            bool addUserToSpace = false;
            bool updateUserMem = false;
            bool removeUserFromSpace = false;

            pubnubSub.SubscribeCallback += (sender, e) =>
            {
                SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;
                if (mea.UserEventResult != null)
                {
                    userUpdate = mea.UserEventResult.Channel.Equals(userid) && mea.UserEventResult.UserID.Equals(userid) && mea.UserEventResult.ObjectsEvent.Equals(PNObjectsEvent.PNObjectsEventUpdate);
                    userDelete = mea.UserEventResult.Channel.Equals(userid) && mea.UserEventResult.UserID.Equals(userid) && mea.UserEventResult.ObjectsEvent.Equals(PNObjectsEvent.PNObjectsEventDelete);
                    Debug.Log(mea.UserEventResult.Name);
                    Debug.Log(mea.UserEventResult.Email);
                    Debug.Log(mea.UserEventResult.ExternalID);
                    Debug.Log(mea.UserEventResult.ProfileURL);
                    Debug.Log(mea.UserEventResult.UserID);
                    Debug.Log(mea.UserEventResult.ETag);
                    Debug.Log(mea.UserEventResult.ObjectsEvent);
                }
                else if (mea.SpaceEventResult != null)
                {
                    spaceUpdate = mea.SpaceEventResult.Channel.Equals(spaceid) && mea.SpaceEventResult.SpaceID.Equals(spaceid) && mea.SpaceEventResult.ObjectsEvent.Equals(PNObjectsEvent.PNObjectsEventUpdate);
                    spaceDelete = mea.SpaceEventResult.Channel.Equals(spaceid) && mea.SpaceEventResult.SpaceID.Equals(spaceid) && mea.SpaceEventResult.ObjectsEvent.Equals(PNObjectsEvent.PNObjectsEventDelete);
                    Debug.Log(mea.SpaceEventResult.Name);
                    Debug.Log(mea.SpaceEventResult.Description);
                    Debug.Log(mea.SpaceEventResult.SpaceID);
                    Debug.Log(mea.SpaceEventResult.ETag);
                    Debug.Log(mea.SpaceEventResult.ObjectsEvent);
                }
                else if (mea.MembershipEventResult != null)
                {
                    addUserToSpace = mea.MembershipEventResult.SpaceID.Equals(spaceid) && mea.MembershipEventResult.UserID.Equals(userid) && mea.MembershipEventResult.ObjectsEvent.Equals(PNObjectsEvent.PNObjectsEventCreate);
                    updateUserMem = mea.MembershipEventResult.SpaceID.Equals(spaceid) && mea.MembershipEventResult.UserID.Equals(userid) && mea.MembershipEventResult.ObjectsEvent.Equals(PNObjectsEvent.PNObjectsEventUpdate);
                    removeUserFromSpace = mea.MembershipEventResult.SpaceID.Equals(spaceid) && mea.MembershipEventResult.UserID.Equals(userid) && mea.MembershipEventResult.ObjectsEvent.Equals(PNObjectsEvent.PNObjectsEventDelete);
                    Debug.Log(mea.MembershipEventResult.UserID);
                    Debug.Log(mea.MembershipEventResult.Description);
                    Debug.Log(mea.MembershipEventResult.SpaceID);
                    Debug.Log(mea.MembershipEventResult.ObjectsEvent);
                }
            };
            pubnubSub.Subscribe().Channels(channelList2).Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            bool tresult = false;
            //Create User
            pubnubObjects.CreateUser().Email(email).ExternalID(externalID).Name(name).ID(userid).ProfileURL(profileURL).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Assert.AreEqual(name, result.Name);
                Assert.AreEqual(email, result.Email);
                Assert.AreEqual(externalID, result.ExternalID);
                Assert.AreEqual(profileURL, result.ProfileURL);
                Assert.AreEqual(userid, result.ID);
                Assert.AreEqual(result.Updated, result.Created);
                Assert.True(!string.IsNullOrEmpty(result.ETag), result.ETag);
                Assert.True(result.Custom == null);
                tresult = true;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "CreateUser didn't return");

            //Update User
            tresult = false;

            pubnubObjects.UpdateUser().Email(email2).ExternalID(externalID2).Name(name2).ID(userid).ProfileURL(profileURL2).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Assert.AreEqual(name2, result.Name);
                Assert.AreEqual(email2, result.Email);
                Assert.AreEqual(externalID2, result.ExternalID);
                Assert.AreEqual(profileURL2, result.ProfileURL);
                Assert.AreEqual(userid, result.ID);
                Assert.AreNotEqual(result.Updated, result.Created);
                Assert.True(!string.IsNullOrEmpty(result.ETag), result.ETag);
                Assert.True(result.Custom == null);
                tresult = true;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls4);
            Assert.True(tresult, "UpdateUser didn't return");

            Assert.True(userUpdate, "userUpdate didn't return");
            //Create Space
            tresult = false;

            pubnubObjects.CreateSpace().Description(spacedesc).Name(spacename).ID(spaceid).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Assert.AreEqual(spacename, result.Name);
                Assert.AreEqual(spacedesc, result.Description);
                Assert.AreEqual(spaceid, result.ID);
                Assert.AreEqual(result.Updated, result.Created);
                Assert.True(!string.IsNullOrEmpty(result.ETag), result.ETag);
                Assert.True(result.Custom == null);
                tresult = true;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "CreateSpace didn't return");

            tresult = false;
            //Update Space

            pubnubObjects.UpdateSpace().Description(spacedesc2).Name(spacename2).ID(spaceid).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Assert.AreEqual(spacename2, result.Name);
                Assert.AreEqual(spacedesc2, result.Description);
                Assert.AreEqual(spaceid, result.ID);
                Assert.AreNotEqual(result.Updated, result.Created);
                Assert.True(!string.IsNullOrEmpty(result.ETag), result.ETag);
                Assert.True(result.Custom == null);
                tresult = true;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls4);
            Assert.True(tresult, "UpdateSpace didn't return");

            Assert.True(spaceUpdate, "spaceUpdate didn't return");

            //Add user to space
            PNMembersInput input = new PNMembersInput();
            input.ID = userid;
            Dictionary<string, object> membersCustom2 = new Dictionary<string, object>();
            membersCustom2.Add("memberscustomkey21", "mcv21");
            membersCustom2.Add("memberscustomkey22", "mcv22");

            input.Custom = membersCustom2;
            int limit = 100;
            bool count = true;
            tresult = false;

            PNMembersInclude[] inclSm = new PNMembersInclude[] { PNMembersInclude.PNMembersCustom, PNMembersInclude.PNMembersUser, PNMembersInclude.PNMembersUserCustom };
            PNMembershipsInclude[] inclMem = new PNMembershipsInclude[] { PNMembershipsInclude.PNMembershipsCustom, PNMembershipsInclude.PNMembershipsSpace, PNMembershipsInclude.PNMembershipsSpaceCustom };

            //Add Space Memberships
            pubnubObjects.ManageMembers().SpaceID(spaceid).Add(new List<PNMembersInput> { input }).Update(new List<PNMembersInput> { }).Remove(new List<PNMembersRemove> { }).Include(inclSm).Limit(limit).Count(count).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Debug.Log("Looking for " + userid);
                bool bFound = false;
                foreach (PNMembers mem in result.Data)
                {

                    Debug.Log("Found mem " + mem.User.ID);
                    if (mem.User.ID.Equals(userid))
                    {
                        Assert.AreEqual(name2, mem.User.Name);
                        Assert.AreEqual(email2, mem.User.Email);
                        Assert.AreEqual(externalID2, mem.User.ExternalID);
                        Assert.AreEqual(profileURL2, mem.User.ProfileURL);
                        Assert.AreEqual(userid, mem.User.ID);
                        //Assert.AreEqual(mem.User.Updated, mem.User.Created);
                        Assert.True(!string.IsNullOrEmpty(mem.User.ETag), mem.User.ETag);
                        Assert.True("mcv21" == mem.Custom["memberscustomkey21"].ToString());
                        Assert.True("mcv22" == mem.Custom["memberscustomkey22"].ToString());

                        bFound = true;
                        break;
                    }

                }
                tresult = bFound;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls4);
            Assert.True(tresult, "ManageMembers didn't return");

            Assert.True(addUserToSpace, "addUserToSpace didn't return");

            //Update user membership

            PNMembersInput inputMembershipsUp = new PNMembersInput();
            inputMembershipsUp.ID = spaceid;
            Dictionary<string, object> membershipCustomUp = new Dictionary<string, object>();
            membershipCustomUp.Add("mememberscustomkeyup21", "memcvup21");
            membershipCustomUp.Add("mememberscustomkeyup22", "memcvup22");

            inputMembershipsUp.Custom = membershipCustomUp;

            tresult = false;

            pubnubObjects.ManageMemberships().UserID(userid).Add(new List<PNMembersInput> { }).Update(new List<PNMembersInput> { inputMembershipsUp }).Remove(new List<PNMembersRemove> { }).Include(inclMem).Limit(limit).Count(count).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                bool bFound = false;
                Debug.Log("Looking for " + spaceid);
                foreach (PNMemberships mem in result.Data)
                {

                    Debug.Log("Found mem " + mem.Space.ID);
                    if (mem.Space.ID.Equals(spaceid))
                    {
                        Assert.AreEqual(spacename2, mem.Space.Name);
                        Assert.AreEqual(spacedesc2, mem.Space.Description);
                        Assert.AreEqual(spaceid, mem.Space.ID);
                        Assert.True(!string.IsNullOrEmpty(mem.Space.ETag), mem.Space.ETag);
                        Assert.True("memcvup21" == mem.Custom["mememberscustomkeyup21"].ToString());
                        Assert.True("memcvup22" == mem.Custom["mememberscustomkeyup22"].ToString());

                        bFound = true;
                        break;
                    }

                }

                tresult = bFound;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls4);
            Assert.True(tresult, "ManageMemberships Update didn't return");

            Assert.True(updateUserMem, "updateUserMem didn't return");

            //Remove user from space
            PNMembersRemove inputMembershipsRm = new PNMembersRemove();
            inputMembershipsRm.ID = spaceid;

            tresult = false;

            pubnubObjects.ManageMemberships().UserID(userid).Add(new List<PNMembersInput> { }).Update(new List<PNMembersInput> { }).Remove(new List<PNMembersRemove> { inputMembershipsRm }).Include(inclMem).Limit(limit).Count(count).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                bool bFound = true;
                Debug.Log("Looking for " + spaceid);
                foreach (PNMemberships mem in result.Data)
                {

                    Debug.Log("Found mem " + mem.Space.ID);
                    if (mem.Space.ID.Equals(spaceid))
                    {
                        Assert.AreEqual(spacename2, mem.Space.Name);
                        Assert.AreEqual(spacedesc2, mem.Space.Description);
                        Assert.AreEqual(spaceid, mem.Space.ID);
                        Assert.AreEqual(mem.Space.Updated, mem.Space.Created);
                        Assert.True(!string.IsNullOrEmpty(mem.Space.ETag), mem.Space.ETag);

                        bFound = false;
                        break;
                    }

                }

                tresult = bFound;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls4);
            Assert.True(tresult, "ManageMemberships Remove didn't return");

            Assert.True(removeUserFromSpace, "removeUserFromSpace didn't return");

            //delete user 1
            tresult = false;
            pubnubObjects.DeleteUser().ID(userid).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                tresult = true;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "DeleteUser didn't return");
            Assert.True(userDelete, "userDelete didn't return");
            //delete space 1
            tresult = false;
            pubnubObjects.DeleteSpace().ID(spaceid).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                tresult = true;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "DeleteSpace didn't return");
            Assert.True(spaceDelete, "spaceDelete didn't return");
            pubnubSub.CleanUp();
            pubnubObjects.CleanUp();
        }

        [UnityTest]
        public IEnumerator TestUserCRUD()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next(1000);
            int ran = r.Next(1000);
            string id = "id" + ran;
            string name = string.Format("name {0}", ran);
            string email = string.Format("email {0}", ran);
            string externalID = string.Format("externalID {0}", ran);
            string profileURL = string.Format("profileURL {0}", ran);

            string filter = string.Format("name like '{0}*'", name);
            string[] sort = new string[] { "created:desc" };

            PNUserSpaceInclude[] include = new PNUserSpaceInclude[] { PNUserSpaceInclude.PNUserSpaceCustom };

            PubNub pubnub = new PubNub(pnConfiguration);
            bool tresult = false;

            pubnub.CreateUser().Email(email).ExternalID(externalID).Name(name).ID(id).Include(include).ProfileURL(profileURL).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Debug.Log("status.StatusCode" + status.StatusCode);
                Assert.AreEqual(name, result.Name);
                Assert.AreEqual(email, result.Email);
                Assert.AreEqual(externalID, result.ExternalID);
                Assert.AreEqual(profileURL, result.ProfileURL);
                Assert.AreEqual(id, result.ID);
                Assert.AreEqual(result.Updated, result.Created);
                Assert.True(!string.IsNullOrEmpty(result.ETag), result.ETag);
                Assert.True(result.Custom == null);
                tresult = true;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "CreateUser didn't return");

            tresult = false;

            pubnub.GetUser().ID(id).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Assert.AreEqual(name, result.Name);
                Assert.AreEqual(email, result.Email);
                Assert.AreEqual(externalID, result.ExternalID);
                Assert.AreEqual(profileURL, result.ProfileURL);
                Assert.AreEqual(id, result.ID);
                Assert.AreEqual(result.Updated, result.Created);
                Assert.True(!string.IsNullOrEmpty(result.ETag), result.ETag);
                Assert.True(result.Custom == null);
                tresult = true;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "GetUser didn't return");

            tresult = false;

            int ran2 = r.Next(1000);
            string name2 = string.Format("name {0}", ran2);
            string email2 = string.Format("email {0}", ran2);
            string externalID2 = string.Format("externalID {0}", ran2);
            string profileURL2 = string.Format("profileURL {0}", ran2);
            tresult = false;

            pubnub.UpdateUser().Email(email2).ExternalID(externalID2).Name(name2).ID(id).Include(include).ProfileURL(profileURL2).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Assert.AreEqual(name2, result.Name);
                Assert.AreEqual(email2, result.Email);
                Assert.AreEqual(externalID2, result.ExternalID);
                Assert.AreEqual(profileURL2, result.ProfileURL);
                Assert.AreEqual(id, result.ID);
                Assert.AreNotEqual(result.Updated, result.Created);
                Assert.True(!string.IsNullOrEmpty(result.ETag), result.ETag);
                Assert.True(result.Custom == null);
                tresult = true;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "UpdateUser didn't return");

            pubnub.GetUsers().Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Debug.Log("result.Next:" + result.Next);
                Debug.Log("result.Prev:" + result.Prev);
                Debug.Log("result.TotalCount:" + result.TotalCount);
                //Assert.True(result.TotalCount>0);

                if (result.Data != null)
                {
                    foreach (PNUserResult pnUserResult in result.Data)
                    {
                        if (pnUserResult.ID.Equals(id))
                        {
                            Assert.AreEqual(name2, pnUserResult.Name);
                            Assert.AreEqual(email2, pnUserResult.Email);
                            Assert.AreEqual(externalID2, pnUserResult.ExternalID);
                            Assert.AreEqual(profileURL2, pnUserResult.ProfileURL);
                            Assert.AreNotEqual(pnUserResult.Updated, pnUserResult.Created);
                            Assert.True(!string.IsNullOrEmpty(pnUserResult.ETag), pnUserResult.ETag);
                            Assert.True(pnUserResult.Custom == null);
                            tresult = true;
                        }
                    }
                }
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "GetUsers didn't return");

            pubnub.GetUsers().Filter(filter).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Debug.Log("result.Next:" + result.Next);
                Debug.Log("result.Prev:" + result.Prev);
                Debug.Log("result.TotalCount:" + result.TotalCount);
                //Assert.True(result.TotalCount>0);

                if (result.Data != null)
                {
                    foreach (PNUserResult pnUserResult in result.Data)
                    {
                        if (pnUserResult.ID.Equals(id))
                        {
                            Assert.AreEqual(name2, pnUserResult.Name);
                            Assert.AreEqual(email2, pnUserResult.Email);
                            Assert.AreEqual(externalID2, pnUserResult.ExternalID);
                            Assert.AreEqual(profileURL2, pnUserResult.ProfileURL);
                            Assert.AreNotEqual(pnUserResult.Updated, pnUserResult.Created);
                            Assert.True(!string.IsNullOrEmpty(pnUserResult.ETag), pnUserResult.ETag);
                            Assert.True(pnUserResult.Custom == null);
                            tresult = true;
                        }
                    }
                }
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "GetUsersFilter didn't return");

            // Sort feature test for GetUsers
            tresult = false;
            pubnub.GetUsers().Sort(sort).Limit(1).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Debug.Log("result.Next:" + result.Next);
                Debug.Log("result.Prev:" + result.Prev);
                Debug.Log("result.TotalCount:" + result.TotalCount);
                    //Assert.True(result.TotalCount>0);

                    if (result.Data != null)
                {
                    foreach (PNUserResult pnUserResult in result.Data)
                    {
                        if (pnUserResult.ID.Equals(id))
                        {
                            Assert.AreEqual(name2, pnUserResult.Name);
                            Assert.AreEqual(email2, pnUserResult.Email);
                            Assert.AreEqual(externalID2, pnUserResult.ExternalID);
                            Assert.AreEqual(profileURL2, pnUserResult.ProfileURL);
                            Assert.AreNotEqual(pnUserResult.Updated, pnUserResult.Created);
                            Assert.True(!string.IsNullOrEmpty(pnUserResult.ETag), pnUserResult.ETag);
                            Assert.True(pnUserResult.Custom == null);
                            tresult = true;
                        }
                    }
                }
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "GetUsers with Sort didn't return");

            tresult = false;

            pubnub.DeleteUser().ID(id).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                tresult = true;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "DeleteUser didn't return");

            pubnub.CleanUp();
        }

        [UnityTest]
        public IEnumerator TestSpaceCRUD()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next(1000);
            int ran = r.Next(1000);
            string id = "id" + ran;
            string name = string.Format("name {0}", ran);
            string description = string.Format("description {0}", ran);

            string filter = string.Format("name like '{0}*'", name);
            string[] sort = new string[] { "created:desc" };
            PNUserSpaceInclude[] include = new PNUserSpaceInclude[] { PNUserSpaceInclude.PNUserSpaceCustom };

            PubNub pubnub = new PubNub(pnConfiguration);
            bool tresult = false;

            pubnub.CreateSpace().Description(description).Name(name).ID(id).Include(include).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Assert.AreEqual(name, result.Name);
                Assert.AreEqual(description, result.Description);
                Assert.AreEqual(id, result.ID);
                Assert.AreEqual(result.Updated, result.Created);
                Assert.True(!string.IsNullOrEmpty(result.ETag), result.ETag);
                Assert.True(result.Custom == null);
                tresult = true;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "CreateSpace didn't return");
            tresult = false;

            pubnub.GetSpace().ID(id).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Assert.AreEqual(name, result.Name);
                Assert.AreEqual(description, result.Description);
                Assert.AreEqual(id, result.ID);
                Assert.AreEqual(result.Updated, result.Created);
                Assert.True(!string.IsNullOrEmpty(result.ETag), result.ETag);
                Assert.True(result.Custom == null);
                tresult = true;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "GetSpace didn't return");

            tresult = false;

            int ran2 = r.Next(1000);
            string name2 = string.Format("name {0}", ran2);
            string description2 = string.Format("description {0}", ran2);
            tresult = false;

            pubnub.UpdateSpace().Description(description2).Name(name2).ID(id).Include(include).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Assert.AreEqual(name2, result.Name);
                Assert.AreEqual(description2, result.Description);
                Assert.AreEqual(id, result.ID);
                Assert.AreNotEqual(result.Updated, result.Created);
                Assert.True(!string.IsNullOrEmpty(result.ETag), result.ETag);
                Assert.True(result.Custom == null);
                tresult = true;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "UpdateSpace didn't return");


            pubnub.GetSpaces().Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Debug.Log("result.Next:" + result.Next);
                Debug.Log("result.Prev:" + result.Prev);
                Debug.Log("result.TotalCount:" + result.TotalCount);
                //Assert.True(result.TotalCount>0);

                if (result.Data != null)
                {
                    foreach (PNSpaceResult pnSpaceResult in result.Data)
                    {
                        if (pnSpaceResult.ID.Equals(id))
                        {
                            Assert.AreEqual(name2, pnSpaceResult.Name);
                            Assert.AreEqual(description2, pnSpaceResult.Description);
                            Assert.AreNotEqual(pnSpaceResult.Updated, pnSpaceResult.Created);
                            Assert.True(!string.IsNullOrEmpty(pnSpaceResult.ETag), pnSpaceResult.ETag);
                            Assert.True(pnSpaceResult.Custom == null);
                            tresult = true;
                        }
                    }
                }
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "GetSpaces didn't return");

            pubnub.GetSpaces().Filter(filter).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Debug.Log("result.Next:" + result.Next);
                Debug.Log("result.Prev:" + result.Prev);
                Debug.Log("result.TotalCount:" + result.TotalCount);
                //Assert.True(result.TotalCount>0);

                if (result.Data != null)
                {
                    foreach (PNSpaceResult pnSpaceResult in result.Data)
                    {
                        if (pnSpaceResult.ID.Equals(id))
                        {
                            Assert.AreEqual(name2, pnSpaceResult.Name);
                            Assert.AreEqual(description2, pnSpaceResult.Description);
                            Assert.AreNotEqual(pnSpaceResult.Updated, pnSpaceResult.Created);
                            Assert.True(!string.IsNullOrEmpty(pnSpaceResult.ETag), pnSpaceResult.ETag);
                            Assert.True(pnSpaceResult.Custom == null);
                            tresult = true;
                        }
                    }
                }
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "GetSpacesFilter didn't return");

            // Sort feature test for GetSpaces
            tresult = false;
            pubnub.GetSpaces().Sort(sort).Limit(1).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                Debug.Log("result.Next:" + result.Next);
                Debug.Log("result.Prev:" + result.Prev);
                Debug.Log("result.TotalCount:" + result.TotalCount);

                if (result.Data != null)
                {
                    foreach (PNSpaceResult pnSpaceResult in result.Data)
                    {
                        if (pnSpaceResult.ID.Equals(id))
                        {
                            Assert.AreEqual(name2, pnSpaceResult.Name);
                            Assert.AreEqual(description2, pnSpaceResult.Description);
                            Assert.AreNotEqual(pnSpaceResult.Updated, pnSpaceResult.Created);
                            Assert.True(!string.IsNullOrEmpty(pnSpaceResult.ETag), pnSpaceResult.ETag);
                            Assert.True(pnSpaceResult.Custom == null);
                            tresult = true;
                        }
                    }
                }
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "GetSpaces sort didn't return");

            tresult = false;

            pubnub.DeleteSpace().ID(id).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
                tresult = true;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "DeleteSpace didn't return");

            pubnub.CleanUp();
        }

        // [UnityTest]
        // public IEnumerator TestDeleteMessagesBuildRequestsSecretKeyError(){
        //     PNConfiguration pnConfiguration = new PNConfiguration ();
        //     pnConfiguration.SubscribeKey = "demo";
        //     pnConfiguration.PublishKey = "demo";
        //     PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
        //     pnUnity.DeleteMessages().Channel("test").Async((result, status) => {
        // 		bool statusError = status.Error;
        // 		Debug.Log("statusError:" + statusError);
        //         Assert.IsTrue (expected.Equals (received), expNRec);
        //      });
        //     yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
        // }

        [UnityTest]
        public IEnumerator TestCG()
        {

            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestCGUUID_" + r.Next(100);
            string channel = "UnityTestWithCGChannel";
            string channel2 = "UnityTestWithCGChannel2";
            List<string> channelList = new List<string>();
            channelList.Add(channel);
            channelList.Add(channel2);

            string channelGroup = "cg";
            List<string> channelGroupList = new List<string>();
            channelGroupList.Add(channelGroup);

            PubNub pubnub = new PubNub(pnConfiguration);
            bool tresult = false;


            pubnub.AddChannelsToChannelGroup().Channels(channelList).ChannelGroup(channelGroup).Async((result, status) =>
            {
                Debug.Log("in AddChannelsToChannelGroup " + status.Error);
                if (!status.Error)
                {
                    Debug.Log(result.Message);
                    tresult = result.Message.Contains("OK");
                }
                else
                {
                    Assert.Fail("AddChannelsToChannelGroup failed");
                }
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "test didn't return1");
            tresult = false;

            pubnub.ListChannelsForChannelGroup().ChannelGroup(channelGroup).Async((result, status) =>
            {
                if (!status.Error)
                {
                    if (result.Channels != null)
                    {
                        bool matchChannel1 = result.Channels.Contains(channel);
                        bool matchChannel2 = result.Channels.Contains(channel2);
                        Assert.IsTrue(matchChannel1);
                        Assert.IsTrue(matchChannel2);
                        tresult = matchChannel1 && matchChannel2;
                    }
                    else
                    {
                        Assert.Fail("result.Channels empty");
                    }
                }
                else
                {
                    Assert.Fail("AddChannelsToChannelGroup failed");
                }

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "test didn't return2");
            tresult = false;
            string payload = string.Format("payload {0}", pnConfiguration.UUID);

            pubnub.SubscribeCallback += (sender, e) =>
            {
                SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;
                if (!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory))
                {
                    if (mea.MessageResult != null)
                    {
                        Debug.Log("SubscribeCallback" + mea.MessageResult.Subscription);
                        Debug.Log("SubscribeCallback" + mea.MessageResult.Channel);
                        Debug.Log("SubscribeCallback" + mea.MessageResult.Payload);
                        Debug.Log("SubscribeCallback" + mea.MessageResult.Timetoken);
                        bool matchChannel = mea.MessageResult.Channel.Equals(channel);
                        Assert.True(matchChannel);
                        bool matchSubscription = mea.MessageResult.Subscription.Equals(channelGroup);
                        Assert.True(matchSubscription);
                        bool matchPayload = mea.MessageResult.Payload.ToString().Equals(payload);
                        Assert.True(matchPayload);
                        tresult = matchPayload && matchSubscription && matchChannel;
                    }
                }

            };

            pubnub.Subscribe().ChannelGroups(channelGroupList).Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            //tresult = false;
            pubnub.Publish().Channel(channel).Message(payload).Async((result, status) =>
            {
                Assert.True(!result.Timetoken.Equals(0));
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "test didn't return 3");
            tresult = false;

            Dictionary<string, object> state = new Dictionary<string, object>();
            state.Add("k1", "v1");
            pubnub.SetPresenceState().ChannelGroups(channelGroupList).State(state).Async((result, status) =>
            {
                if (status.Error)
                {
                    Assert.Fail("SetPresenceState failed");
                }
                else
                {
                    if (result != null)
                    {
                        if (result.StateByChannels != null)
                        {
                            foreach (KeyValuePair<string, object> key in result.StateByChannels)
                            {
                                Debug.Log("key.Key" + key.Key);
                                if (key.Key.Equals(channelGroup))
                                {
                                    Debug.Log("pubnub.JsonLibrary.SerializeToJsonString(key.Value)" + pubnub.JsonLibrary.SerializeToJsonString(key.Value));
                                    bool stateMatch = pubnub.JsonLibrary.SerializeToJsonString(key.Value).Equals("{\"k1\":\"v1\"}");
                                    Assert.IsTrue(stateMatch);
                                    tresult = stateMatch;
                                    break;
                                }
                            }
                        }
                    }
                }
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "test didn't return 4");

            /*tresult = false;

            pubnub.GetPresenceState().ChannelGroups(channelGroupList).Async ((result, status) => {
                if(status.Error){
                    Assert.Fail("GetPresenceState failed");
                } else {
                    if(result != null){
                        if(result.StateByChannels!= null){
                            bool found1= false, found2 = false;
                            foreach (KeyValuePair<string, object> key in result.StateByChannels){
                                Debug.Log("key.Key" + key.Key);
                                Dictionary<string, object> stateDict = key.Value as Dictionary<string, object>;
                                foreach (KeyValuePair<string, object> keyInStateDict in stateDict){
                                    Debug.Log("keyInStateDict.Key" + keyInStateDict.Key);
                                    if(keyInStateDict.Key.Equals(channel)){
                                        bool stateMatch = pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value).Equals("{\"cg\":{\"k1\":\"v1\"}}");
                                        Debug.Log("pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value)" + pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value));
                                        Assert.IsTrue(stateMatch);
                                        found1 = stateMatch;
                                    }
                                    if(keyInStateDict.Key.Equals(channel2)){
                                        bool stateMatch = pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value).Equals("{\"cg\":{\"k1\":\"v1\"}}");
                                        Debug.Log("pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value)" + pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value));
                                        Assert.IsTrue(stateMatch);
                                        found2 = stateMatch;
                                    }
                                }
                            }
                            Assert.IsTrue(found1);
                            Assert.IsTrue(found2);
                            tresult = found1 && found2 ;
                        }
                    }
                }
            });

            yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tresult, "test didn't return 5");

            tresult = false;
            Dictionary<string, object> stateToDel = new Dictionary<string, object> ();
            stateToDel.Add("k1", "");

            pubnub.SetPresenceState().ChannelGroups(channelGroupList).State(stateToDel).Async ((result, status) => {
                if(status.Error){
                    Assert.Fail("SetPresenceState null failed");
                } else {
                    if(result != null){
                        if(result.StateByChannels!= null){
                            bool found = false;
                            foreach (KeyValuePair<string, object> key in result.StateByChannels){
                                Debug.Log("key.Key" + key.Key);
                                if(key.Key.Equals(channelGroup)){
                                    bool stateMatch = pubnub.JsonLibrary.SerializeToJsonString(key.Value).Equals("{\"k1\":\"v1\"}");
                                    Debug.Log("pubnub.JsonLibrary.SerializeToJsonString(key.Value)" + pubnub.JsonLibrary.SerializeToJsonString(key.Value));
                                    found = stateMatch;
                                    break;
                                }
                            }
                            Assert.IsFalse(found);
                            tresult = !found;
                        }
                    }
                }
            });

            yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
            Assert.True(tresult, "test didn't return 6");

            tresult = false;

            pubnub.GetPresenceState().ChannelGroups(channelGroupList).Async ((result, status) => {
            //pubnub.SetPresenceState().Channels(new List<string> (){ch1}).State(state).Async ((result, status) => {
                if(status.Error){
                    Assert.Fail("GetPresenceState failed");
                } else {
                    if(result != null){
                        if(result.StateByChannels!= null){
                            bool found1= false, found2 = false;
                            foreach (KeyValuePair<string, object> key in result.StateByChannels){
                                Debug.Log("key.Key" + key.Key);
                                Dictionary<string, object> stateDict = key.Value as Dictionary<string, object>;
                                foreach (KeyValuePair<string, object> keyInStateDict in stateDict){
                                    Debug.Log("keyInStateDict.Key" + keyInStateDict.Key);
                                    if(keyInStateDict.Key.Equals(channel)){
                                        bool stateMatch = pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value).Equals("{\"cg\":{\"k1\":\"v1\"}}");
                                        Debug.Log("pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value)" + pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value));
                                        Assert.IsFalse(stateMatch);
                                        found1 = stateMatch;
                                    }
                                    if(keyInStateDict.Key.Equals(channel2)){
                                        bool stateMatch = pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value).Equals("{\"cg\":{\"k1\":\"v1\"}}");
                                        Debug.Log("pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value)" + pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value));
                                        Assert.IsFalse(stateMatch);
                                        found2 = stateMatch;
                                    }
                                }
                            }
                            Assert.IsTrue(!found1);
                            Assert.IsTrue(!found2);
                            tresult = !found1 && !found2;
                        }
                    }
                }
            });

            yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tresult, "test didn't return 7");*/
            pubnub.CleanUp();
        }

        [UnityTest]
        public IEnumerator TestCGRemove()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestCGUUID_" + r.Next(100);
            string channel = "UnityTestWithCGChannel";
            string channel2 = "UnityTestWithCGChannel2";
            List<string> channelList = new List<string>();
            channelList.Add(channel);
            channelList.Add(channel2);

            string channelGroup = "cg";
            List<string> channelGroupList = new List<string>();
            channelGroupList.Add(channelGroup);

            PubNub pubnub = new PubNub(pnConfiguration);
            bool tresult = false;


            pubnub.AddChannelsToChannelGroup().Channels(channelList).ChannelGroup(channelGroup).Async((result, status) =>
            {
                Debug.Log("in AddChannelsToChannelGroup " + status.Error);
                if (!status.Error)
                {
                    Debug.Log(result.Message);
                    tresult = result.Message.Contains("OK");
                }
                else
                {
                    Assert.Fail("AddChannelsToChannelGroup failed");
                }
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "test didn't return1");
            tresult = false;

            pubnub.ListChannelsForChannelGroup().ChannelGroup(channelGroup).Async((result, status) =>
            {
                if (!status.Error)
                {
                    if (result.Channels != null)
                    {
                        bool matchChannel1 = result.Channels.Contains(channel);
                        bool matchChannel2 = result.Channels.Contains(channel2);
                        Assert.IsTrue(matchChannel1);
                        Assert.IsTrue(matchChannel2);
                        tresult = matchChannel1 && matchChannel2;
                    }
                    else
                    {
                        Assert.Fail("result.Channels empty");
                    }
                }
                else
                {
                    Assert.Fail("AddChannelsToChannelGroup failed");
                }
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "test didn't return2");
            tresult = false;

            List<string> listChannelsRemove = new List<string> { channel };
            listChannelsRemove.Add(channel);
            pubnub.RemoveChannelsFromChannelGroup().Channels(listChannelsRemove).ChannelGroup(channelGroup).Async((result, status) =>
            {
                Debug.Log("in RemoveChannelsFromCG");
                if (!status.Error)
                {

                    tresult = result.Message.Equals("OK");
                }

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "test didn't return 8");

            tresult = false;
            pubnub.ListChannelsForChannelGroup().ChannelGroup(channelGroup).Async((result, status) =>
            {
                if (!status.Error)
                {
                    if (result.Channels != null)
                    {
                        bool matchChannel1 = result.Channels.Contains(channel);
                        bool matchChannel2 = result.Channels.Contains(channel2);
                        Assert.IsTrue(!matchChannel1);
                        Assert.IsTrue(matchChannel2);
                        tresult = !matchChannel1 && matchChannel2;

                    }
                    else
                    {
                        Assert.Fail("result.Channels empty");
                    }
                }
                else
                {
                    Assert.Fail("AddChannelsToChannelGroup failed");
                }
                tresult = true;

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "test didn't return 9");

            tresult = false;
            pubnub.DeleteChannelGroup().ChannelGroup(channelGroup).Async((result, status) =>
            {
                if (!status.Error)
                {
                    tresult = result.Message.Equals("OK");
                }
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "test didn't return 10");

            pubnub.CleanUp();

        }

        [UnityTest]
        public IEnumerator TestPushAPNS2()
        {
            return TestPushCommon(PNPushType.APNS2);
        }

        [UnityTest]
        public IEnumerator TestPushGCM()
        {
            return TestPushCommon(PNPushType.GCM);
        }

        public IEnumerator TestPushCommon(PNPushType pnPushType)
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            PubNub pubnub = new PubNub(pnConfiguration);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestCGUUID_" + r.Next(100);
            string channel = "UnityTestWithPushChannel";
            string channel2 = "UnityTestWithPushChannel2";
            List<string> listChannels = new List<string>();
            listChannels.Add(channel);
            listChannels.Add(channel2);

            string deviceId = "ababababababababababababababababababababababababababababababababababababababababababababab";
            bool tresult = false;

            // AddPushNotificationsOnChannels
            if (pnPushType.Equals(PNPushType.APNS2))
            {
                pubnub.AddPushNotificationsOnChannels().Channels(listChannels).DeviceID(deviceId).PushType(pnPushType).Topic("a").Async((result, status) =>
                {
                    Debug.Log("in AddPushNotificationsOnChannels");
                    if (status.Error)
                    {
                        Debug.Log(string.Format("In Example, AddPushNotificationsOnChannels Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    }
                    else
                    {
                        tresult = result.Message.Contains("Modified Channels");
                        Debug.Log(string.Format("DateTime {0}, In AddPushNotificationsOnChannels, result: {1}", DateTime.UtcNow, result.Message));
                    }

                });
            }
            else
            {
                pubnub.AddPushNotificationsOnChannels().Channels(listChannels).DeviceID(deviceId).PushType(pnPushType).Async((result, status) =>
                {
                    Debug.Log("in AddPushNotificationsOnChannels");
                    if (status.Error)
                    {
                        Debug.Log(string.Format("In Example, AddPushNotificationsOnChannels Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    }
                    else
                    {
                        tresult = result.Message.Contains("Modified Channels");
                        Debug.Log(string.Format("DateTime {0}, In AddPushNotificationsOnChannels, result: {1}", DateTime.UtcNow, result.Message));
                    }
                });
            }
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tresult, "AddPushNotificationsOnChannels didn't return");
            tresult = false;

            // AuditPushChannelProvisions
            if (pnPushType.Equals(PNPushType.APNS2))
            {
                pubnub.AuditPushChannelProvisions().DeviceID(deviceId).PushType(pnPushType).Topic("a").Async((result, status) =>
                {
                    Debug.Log("in AuditPushChannelProvisions");
                    if (status.Error)
                    {
                        Debug.Log(string.Format("In Example, AuditPushChannelProvisions Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    }
                    else
                    {
                        tresult = true;
                        Debug.Log(string.Format("DateTime {0}, In AuditPushChannelProvisions, result: {1}", DateTime.UtcNow, result));
                    }
                });
            }
            else
            {
                pubnub.AuditPushChannelProvisions().DeviceID(deviceId).PushType(pnPushType).Async((result, status) =>
                {
                    Debug.Log("in AuditPushChannelProvisions");
                    if (status.Error)
                    {
                        Debug.Log(string.Format("In Example, AuditPushChannelProvisions Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    }
                    else
                    {
                        tresult = true;
                        Debug.Log(string.Format("DateTime {0}, In AuditPushChannelProvisions, result: {1}", DateTime.UtcNow, result));
                    }

                });
            }
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tresult, "AuditPushChannelProvisions didn't return");
            tresult = false;

            // RemovePushNotificationsFromChannels
            if (pnPushType.Equals(PNPushType.APNS2))
            {
                pubnub.RemovePushNotificationsFromChannels().Channels(new List<string> { channel }).DeviceID(deviceId).PushType(pnPushType).Topic("a").Async((result, status) =>
                {
                    Debug.Log("in RemovePushNotificationsFromChannels");
                    if (status.Error)
                    {
                        Debug.Log(string.Format("In Example, RemovePushNotificationsFromChannels Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    }
                    else
                    {
                        tresult = result.Message.Contains("Modified Channels");
                        Debug.Log(string.Format("DateTime {0}, In RemovePushNotificationsFromChannels, result: {1}", DateTime.UtcNow, result.Message));
                    }

                });
            }
            else
            {
                pubnub.RemovePushNotificationsFromChannels().Channels(new List<string> { channel }).DeviceID(deviceId).PushType(pnPushType).Async((result, status) =>
                {
                    Debug.Log("in RemovePushNotificationsFromChannels");
                    if (status.Error)
                    {
                        Debug.Log(string.Format("In Example, RemovePushNotificationsFromChannels Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    }
                    else
                    {
                        tresult = result.Message.Contains("Modified Channels");
                        Debug.Log(string.Format("DateTime {0}, In RemovePushNotificationsFromChannels, result: {1}", DateTime.UtcNow, result.Message));
                    }

                });
            }
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tresult, "RemovePushNotificationsFromChannels didn't return");
            tresult = false;

            // RemoveAllPushNotifications
            if (pnPushType.Equals(PNPushType.APNS2))
            {
                pubnub.RemoveAllPushNotifications().DeviceID(deviceId).PushType(pnPushType).Topic("a").Async((result, status) =>
                {
                    Debug.Log("in RemoveAllPushNotifications");
                    if (status.Error)
                    {
                        Debug.Log(string.Format("In Example, RemoveAllPushNotifications Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    }
                    else
                    {
                        tresult = result.Message.Contains("Removed Device");
                        Debug.Log(string.Format("DateTime {0}, In RemoveAllPushNotifications, result: {1}", DateTime.UtcNow, result.Message));
                    }

                });
            }
            else
            {
                pubnub.RemoveAllPushNotifications().DeviceID(deviceId).PushType(pnPushType).Async((result, status) =>
                {
                    Debug.Log("in RemoveAllPushNotifications");
                    if (status.Error)
                    {
                        Debug.Log(string.Format("In Example, RemoveAllPushNotifications Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                    }
                    else
                    {
                        tresult = result.Message.Contains("Removed Device");
                        Debug.Log(string.Format("DateTime {0}, In RemoveAllPushNotifications, result: {1}", DateTime.UtcNow, result.Message));
                    }

                });
            }
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tresult, "RemoveAllPushNotifications didn't return");
            tresult = false;

        }

        //[UnityTest]
        // public IEnumerator TestPush() {
        // 	PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
        // 	System.Random r = new System.Random ();
        // 	pnConfiguration.UUID = "UnityTestCGUUID_" + r.Next (100);
        // 	string channel = "UnityTestWithCGChannel";
        // 	string channel2 = "UnityTestWithCGChannel2";
        // 	List<string> channelList = new List<string>();
        // 	channelList.Add(channel);
        // 	channelList.Add(channel2);

        // 	string channelGroup = "cg";
        // 	List<string> channelGroupList = new List<string>();
        // 	channelGroupList.Add(channelGroup);

        // 	PubNub pubnub = new PubNub(pnConfiguration);
        // 	bool tresult = false;

        // 	string deviceId = "UnityTestDeviceId";
        // 	PNPushType pnPushType = PNPushType.GCM;

        // 	pubnub.AddPushNotificationsOnChannels().Channels(channelList).DeviceID(deviceId).PushType(pnPushType).Async((result, status) => {
        //             Debug.Log ("in AddChannelsToChannelGroup " + status.Error);
        //             if(!status.Error){
        // 				Debug.Log(result.Message);
        // 				tresult = result.Message.Contains("Modified Ch");
        // 			} else {
        // 				Assert.Fail("AddPushNotificationsOnChannels failed");
        // 			}
        //         });

        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
        // 	Assert.True(tresult, "test didn't return1");
        // 	tresult = false;

        // 	pubnub.AuditPushChannelProvisions().DeviceID(deviceId).PushType(pnPushType).Async((result, status) => {
        //             if(!status.Error){
        // 				if(result.Channels!=null){
        // 					bool matchChannel1 = result.Channels.Contains(channel);
        // 					bool matchChannel2 = result.Channels.Contains(channel2);
        // 					Assert.IsTrue(matchChannel1);
        // 					Assert.IsTrue(matchChannel2);
        // 					tresult = matchChannel1 && matchChannel2;

        // 				} else {
        // 					Assert.Fail("result.Channels empty");
        // 				}
        // 			} else {
        // 				Assert.Fail("AddChannelsToChannelGroup failed");
        // 			}
        //         });

        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
        // 	Assert.True(tresult, "test didn't return2");
        // 	tresult = false;

        // 	List<string> listChannelsRemove = new List<string>{channel};
        // 	listChannelsRemove.Add(channel);
        // 	pubnub.RemovePushNotificationsFromChannels().Channels(listChannelsRemove).DeviceID(deviceId).PushType(pnPushType).Async((result, status) => {
        //             Debug.Log ("in RemovePushNotificationsFromChannels");
        // 			if(!status.Error){
        //                 tresult = result.Message.Equals("Modified Channels");
        //             }
        //         });

        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
        // 	Assert.True(tresult, "test didn't return 8");

        // 	tresult = false;
        // 	pubnub.AuditPushChannelProvisions().DeviceID(deviceId).PushType(pnPushType).Async((result, status) => {
        //             if(!status.Error){
        // 				if(result.Channels!=null){
        // 					bool matchChannel1 = result.Channels.Contains(channel);
        // 					bool matchChannel2 = result.Channels.Contains(channel2);
        // 					Assert.IsTrue(!matchChannel1);
        // 					Assert.IsTrue(matchChannel2);
        // 					tresult = !matchChannel1 && matchChannel2;

        // 				} else {
        // 					Assert.Fail("result.Channels empty");
        // 				}
        // 			} else {
        // 				Assert.Fail("AddChannelsToChannelGroup failed");
        // 			}
        //         });
        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);

        // 	Assert.True(tresult, "test didn't return 9");

        // 	tresult = false;
        // 	pubnub.RemoveAllPushNotifications().DeviceID(deviceId).PushType(pnPushType).Async((result, status) => {
        //             if(!status.Error){
        //                 tresult = result.Message.Equals("Removed Device");
        //             }
        //         });

        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
        // 	Assert.True(tresult, "test didn't return 10");

        // 	pubnub.CleanUp();

        // }

        // [UnityTest]
        // public IEnumerator TestPublishWithMeta() {
        // 	PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
        // 	System.Random r = new System.Random ();
        // 	pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next (100);
        // 	string channel = "UnityTestWithMetaChannel";
        // 	string payload = string.Format("payload {0}", pnConfiguration.UUID);

        // 	pnConfiguration.FilterExpression = "region=='east'";
        // 	PubNub pubnub = new PubNub(pnConfiguration);

        // 	List<string> channelList2 = new List<string>();
        // 	channelList2.Add(channel);
        // 	bool tresult = false;

        // 	pubnub.SubscribeCallback += (sender, e) => {
        // 		SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;
        // 		if(!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
        // 			Debug.Log("SubscribeCallback" + mea.MessageResult.Subscription);
        // 			Debug.Log("SubscribeCallback" + mea.MessageResult.Channel);
        // 			Debug.Log("SubscribeCallback" + mea.MessageResult.Payload);
        // 			Debug.Log("SubscribeCallback" + mea.MessageResult.Timetoken);
        // 			bool matchChannel = mea.MessageResult.Channel.Equals(channel);
        // 			Assert.True(matchChannel);
        // 			bool matchPayload = mea.MessageResult.Payload.ToString().Equals(payload);
        // 			Assert.True(matchPayload);
        // 			tresult = matchPayload  && matchChannel;

        // 		}
        // 	};
        // 	pubnub.Subscribe ().Channels(channelList2).Execute();
        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls1);

        // 	Dictionary<string, string> metaDict = new Dictionary<string, string>();
        //     metaDict.Add("region", "east");

        // 	pubnub.Publish().Channel(channel).Meta(metaDict).Message(payload).Async((result, status) => {
        // 		Assert.True(!result.Timetoken.Equals(0));
        // 		Assert.True(status.Error.Equals(false));
        // 		Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
        // 		Assert.True(!result.Timetoken.Equals(0));
        // 	});
        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
        // 	Assert.True(tresult, "test didn't return");
        // 	pubnub.CleanUp();
        // }

        [UnityTest]
        public IEnumerator TestPublishWithMetaNeg()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next(100);
            string channel = "UnityTestWithMetaNegChannel";
            string payload = string.Format("payload {0}", pnConfiguration.UUID);

            pnConfiguration.FilterExpression = "region=='east'";
            PubNub pubnub = new PubNub(pnConfiguration);

            List<string> channelList2 = new List<string>();
            channelList2.Add(channel);
            bool tresult = false;

            pubnub.SubscribeCallback += (sender, e) =>
            {
                SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;
                if (!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory))
                {
                    Debug.Log("SubscribeCallback" + mea.MessageResult.Subscription);
                    Debug.Log("SubscribeCallback" + mea.MessageResult.Channel);
                    Debug.Log("SubscribeCallback" + mea.MessageResult.Payload);
                    Debug.Log("SubscribeCallback" + mea.MessageResult.Timetoken);
                    bool matchChannel = mea.MessageResult.Channel.Equals(channel);
                    Assert.True(matchChannel);
                    bool matchPayload = mea.MessageResult.Payload.ToString().Equals(payload);
                    Assert.True(matchPayload);
                    tresult = matchPayload && matchChannel;
                }
            };
            pubnub.Subscribe().Channels(channelList2).Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls1);

            Dictionary<string, string> metaDict = new Dictionary<string, string>();
            metaDict.Add("region", "east1");

            pubnub.Publish().Channel(channel).Meta(metaDict).Message(payload).Async((result, status) =>
            {
                Assert.True(!result.Timetoken.Equals(0));
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(!tresult, "subscribe returned");
            pubnub.CleanUp();
        }

        // [UnityTest]
        // public IEnumerator TestPublishAndHistory() {
        // 	PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
        // 	System.Random r = new System.Random ();
        // 	pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next (100);
        // 	string channel = "UnityPublishAndHistoryChannel";
        // 	string payload = string.Format("payload no store {0}", pnConfiguration.UUID);

        // 	PubNub pubnub = new PubNub(pnConfiguration);

        // 	List<string> channelList2 = new List<string>();
        // 	channelList2.Add(channel);
        // 	bool tresult = false;

        // 	pubnub.Publish().Channel(channel).Message(payload).Async((result, status) => {
        // 		bool timetokenMatch = !result.Timetoken.Equals(0);
        // 		bool statusError = status.Error.Equals(false);
        // 		bool statusCodeMatch = status.StatusCode.Equals(0);
        // 		Assert.True(timetokenMatch);
        // 		Assert.True(statusError);
        // 		Assert.True(statusCodeMatch, status.StatusCode.ToString());
        // 		tresult = statusCodeMatch && statusError && timetokenMatch;
        // 	});
        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
        // 	Assert.True(tresult, "test didnt return 1");

        // 	tresult = false;
        // 	pubnub.History().Channel(channel).Count(1).Async((result, status) => {
        // 		Assert.True(status.Error.Equals(false));
        // 		if(!status.Error){

        // 			if((result.Messages!=null) && (result.Messages.Count>0)){
        // 				PNHistoryItemResult pnHistoryItemResult = result.Messages[0] as PNHistoryItemResult;
        // 				Debug.Log("result.Messages[0]" + result.Messages[0].ToString());
        // 				if(pnHistoryItemResult != null){
        // 					tresult = pnHistoryItemResult.Entry.ToString().Contains(payload);
        // 				} else {
        // 					tresult = false;
        // 				}
        // 			} else {
        // 				tresult = false;
        // 			}

        //         }
        // 	});
        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
        // 	Assert.True(tresult, "test didnt return 2");

        // 	pubnub.CleanUp();
        // }

        [UnityTest]
        public IEnumerator TestPublishHistoryAndFetchWithMetaAndTT()
        {
            return PublishHistoryAndFetchWithMetaCommon(true, true);
        }

        [UnityTest]
        public IEnumerator TestPublishHistoryAndFetchWithMetaWithoutTT()
        {
            return PublishHistoryAndFetchWithMetaCommon(true, false);
        }

        [UnityTest]
        public IEnumerator TestPublishHistoryAndFetchWithTTWithoutMeta()
        {
            return PublishHistoryAndFetchWithMetaCommon(false, true);
        }

        [UnityTest]
        public IEnumerator TestPublishHistoryAndFetchWithoutMetaAndTT()
        {
            return PublishHistoryAndFetchWithMetaCommon(false, false);
        }

        public IEnumerator PublishHistoryAndFetchWithMetaCommon(bool withMeta, bool withTimetoken)
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next(100);
            string channel = "UnityPublishAndHistoryChannel" + r.Next(100); ;
            string payload = string.Format("payload {0}", pnConfiguration.UUID);

            PubNub pubnub = new PubNub(pnConfiguration);

            List<string> channelList2 = new List<string>();
            channelList2.Add(channel);
            bool tresult = false;
            Dictionary<string, string> metaDict = new Dictionary<string, string>();
            metaDict.Add("region", "east");
            long retTT = 0;

            pubnub.Publish().Channel(channel).Meta(metaDict).Message(payload).Async((result, status) =>
            {
                bool timetokenMatch = !result.Timetoken.Equals(0);
                bool statusError = status.Error.Equals(false);
                bool statusCodeMatch = status.StatusCode.Equals(0);
                retTT = result.Timetoken;
                Assert.True(timetokenMatch);
                Assert.True(statusError);
                Assert.True(statusCodeMatch, status.StatusCode.ToString());
                tresult = statusCodeMatch && statusError && timetokenMatch;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "test didnt return 1");

            tresult = false;
            bool tresultMeta = false;
            bool tresultTimetoken = false;
            pubnub.History().Channel(channel).IncludeMeta(withMeta).IncludeTimetoken(withTimetoken).Count(1).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                if (!status.Error)
                {

                    if ((result.Messages != null) && (result.Messages.Count > 0))
                    {
                        PNHistoryItemResult pnHistoryItemResult = result.Messages[0] as PNHistoryItemResult;
                        Debug.Log("result.Messages[0]" + result.Messages[0].ToString());
                        if (pnHistoryItemResult != null)
                        {
                            tresult = pnHistoryItemResult.Entry.ToString().Contains(payload);

                            if (withMeta)
                            {
                                Dictionary<string, object> metaDataDict = pnHistoryItemResult.Meta as Dictionary<string, object>;
                                object region;
                                metaDataDict.TryGetValue("region", out region);
                                tresultMeta = region.ToString().Equals("east");
                            }
                            else
                            {
                                tresultMeta = true;
                            }
                            if (withTimetoken)
                            {
                                tresultTimetoken = retTT.Equals(pnHistoryItemResult.Timetoken);
                            }
                            else
                            {
                                tresultTimetoken = true;
                            }
                        }
                        else
                        {
                            tresult = false;
                            tresultMeta = false;
                        }
                    }
                    else
                    {
                        tresult = false;
                    }

                }
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "test didnt return 2");
            Assert.True(tresultMeta, "test meta didnt return");
            Assert.True(tresultTimetoken, "tresultTimetoken didnt return");

            tresult = false;
            tresultMeta = false;
            pubnub.FetchMessages().Channels(channelList2).IncludeMeta(withMeta).Async((result, status) =>
            {
                if (!status.Error)
                {
                    if (result.Channels != null)
                    {
                        Dictionary<string, List<PNMessageResult>> fetchResult = result.Channels as Dictionary<string, List<PNMessageResult>>;
                        Debug.Log("fetchResult.Count:" + fetchResult.Count);
                        foreach (KeyValuePair<string, List<PNMessageResult>> kvp in fetchResult)
                        {
                            Debug.Log("Channel:" + kvp.Key);
                            if (kvp.Key.Equals(channel))
                            {

                                foreach (PNMessageResult msg in kvp.Value)
                                {
                                    Debug.Log("msg.Channel:" + msg.Channel);
                                    Debug.Log("msg.Payload.ToString():" + msg.Payload.ToString());
                                    if (msg.Channel.Equals(channel) && (msg.Payload.ToString().Equals(payload)))
                                    {
                                        tresult = true;
                                    }
                                    if (withMeta)
                                    {
                                        Dictionary<string, object> metaDataDict = msg.UserMetadata as Dictionary<string, object>;
                                        object region;
                                        if (metaDataDict != null)
                                        {
                                            metaDataDict.TryGetValue("region", out region);
                                            tresultMeta = region.ToString().Equals("east");
                                        }
                                        else
                                        {
                                            Debug.Log("metaDataDict null" + msg.UserMetadata);
                                        }
                                    }
                                    else
                                    {
                                        tresultMeta = true;
                                    }

                                }
                                if (!tresult && !tresultMeta)
                                {
                                    break;
                                }
                            }
                        }
                    }

                }

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "test didnt return for fetch");
            Assert.True(tresultMeta, "test meta didnt return for fetch");


            pubnub.CleanUp();
        }

        [UnityTest]
        public IEnumerator TestPublishNoStore()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next(100);
            string channel = "UnityTestNoStoreChannel";
            string payload = string.Format("payload no store {0}", pnConfiguration.UUID);

            PubNub pubnub = new PubNub(pnConfiguration);

            List<string> channelList2 = new List<string>();
            channelList2.Add(channel);
            bool tresult = false;

            pubnub.Publish().Channel(channel).Message(payload).ShouldStore(false).Async((result, status) =>
            {
                bool timetokenMatch = !result.Timetoken.Equals(0);
                bool statusError = status.Error.Equals(false);
                bool statusCodeMatch = status.StatusCode.Equals(0);
                Assert.True(timetokenMatch);
                Assert.True(statusError);
                Assert.True(statusCodeMatch, status.StatusCode.ToString());
                tresult = statusCodeMatch && statusError && timetokenMatch;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "test didnt return 1");

            tresult = false;
            pubnub.History().Channel(channel).Count(1).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                if (!status.Error)
                {

                    if ((result.Messages != null) && (result.Messages.Count > 0))
                    {
                        PNHistoryItemResult pnHistoryItemResult = result.Messages[0] as PNHistoryItemResult;
                        Debug.Log("result.Messages[0]" + result.Messages[0].ToString());
                        if (pnHistoryItemResult != null)
                        {
                            tresult = !pnHistoryItemResult.Entry.ToString().Contains(payload);
                        }
                        else
                        {
                            tresult = false;
                        }
                    }
                    else
                    {
                        tresult = true;
                    }

                }
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "test didnt return 2");

            pubnub.CleanUp();
        }

        // [UnityTest]
        // public IEnumerator TestPublishKeyPresent() {
        // 	PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
        // 	System.Random r = new System.Random ();
        // 	pnConfiguration.UUID = "UnityTestPublishKeyPresentUUID_" + r.Next (100);
        // 	string channel = "UnityPublishKeyPresentChannel";
        // 	string payload = string.Format("payload {0}", pnConfiguration.UUID);

        // 	pnConfiguration.PublishKey = "";
        // 	PubNub pubnub = new PubNub(pnConfiguration);

        // 	bool tresult = false;

        // 	pubnub.Publish().Channel(channel).Message(payload).Async((result, status) => {
        // 		Debug.Log("Publish" + status.Error + status.StatusCode );
        // 		bool statusError = status.Error.Equals(true);
        // 		bool statusCodeMatch = status.StatusCode.Equals(0);
        // 		Assert.True(statusError);
        // 		Assert.True(statusCodeMatch, status.StatusCode.ToString());
        // 		tresult = statusCodeMatch && statusError;

        // 	});

        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
        // 	Assert.True(tresult, "test didn't return 10");

        // 	pubnub.CleanUp();
        // }

        [UnityTest]
        public IEnumerator TestNullAsEmptyOnpublish()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestPublishKeyPresentUUID_" + r.Next(100);
            string channel = "UnityPublishKeyPresentChannel";

            PubNub pubnub = new PubNub(pnConfiguration);

            bool tresult = false;

            pubnub.Publish().Channel(channel).Message(null).Async((result, status) =>
            {
                Debug.Log("Publish" + status.Error + status.StatusCode);
                bool statusError = status.Error.Equals(true);
                bool statusCodeMatch = status.StatusCode.Equals(0);
                Assert.True(statusError);
                Assert.True(statusCodeMatch, status.StatusCode.ToString());
                tresult = statusCodeMatch && statusError;
            });

            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "test didn't return 10");

            pubnub.CleanUp();
        }

        [UnityTest]
        public IEnumerator TestFire()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next(100);
            string channel = "UnityTestFireChannel";
            string payload = string.Format("payload no store {0}", pnConfiguration.UUID);

            PubNub pubnub = new PubNub(pnConfiguration);

            List<string> channelList2 = new List<string>();
            channelList2.Add(channel);
            bool tresult = false;

            pubnub.Fire().Channel(channel).Message(payload).Async((result, status) =>
            {
                bool timetokenMatch = !result.Timetoken.Equals(0);
                bool statusError = status.Error.Equals(false);
                bool statusCodeMatch = status.StatusCode.Equals(0);
                Assert.True(timetokenMatch);
                Assert.True(statusError);
                Assert.True(statusCodeMatch, status.StatusCode.ToString());
                tresult = statusCodeMatch && statusError && timetokenMatch;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "test didnt return 1");

            tresult = false;
            pubnub.History().Channel(channel).Count(1).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                if (!status.Error)
                {

                    if ((result.Messages != null) && (result.Messages.Count > 0))
                    {
                        PNHistoryItemResult pnHistoryItemResult = result.Messages[0] as PNHistoryItemResult;
                        Debug.Log("result.Messages[0]" + result.Messages[0].ToString());
                        if (pnHistoryItemResult != null)
                        {
                            tresult = !pnHistoryItemResult.Entry.ToString().Contains(payload);
                        }
                        else
                        {
                            tresult = false;
                        }
                    }
                    else
                    {
                        tresult = true;
                    }

                }
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);
            Assert.True(tresult, "test didnt return 2");

            pubnub.CleanUp();
        }

        //[UnityTest]
        // public IEnumerator TestWildcardSubscribe() {
        // 	PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
        // 	System.Random r = new System.Random ();
        // 	pnConfiguration.UUID = "UnityWildSubscribeUUID_" + r.Next (100);
        // 	string chToPub = "UnityWildSubscribeChannel." + r.Next (100);
        // 	string channel = "UnityWildSubscribeChannel.*";
        // 	string payload = string.Format("payload {0}", pnConfiguration.UUID);
        // 	PubNub pubnub = new PubNub(pnConfiguration);

        // 	List<string> channelList2 = new List<string>();
        // 	channelList2.Add(channel);
        // 	string whatToTest = "join1";
        // 	bool tJoinResult = false;
        // 	bool tLeaveResult = false;
        // 	bool tresult = false;

        // 	PNConfiguration pnConfiguration2 = PlayModeCommon.SetPNConfig(false);
        // 	pnConfiguration2.UUID = "UnityWildSubscribeUUID2_" + r.Next (100);

        // 	pubnub.SubscribeCallback += (sender, e) => {
        // 		SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;
        // 		if(!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
        // 			switch (whatToTest){
        // 				case "join1":
        // 				case "join2":
        // 					Debug.Log("join1 or join2");
        // 					if(mea.PresenceEventResult.Event.Equals("join")){
        // 						bool containsUUID = false;
        // 						if(whatToTest.Equals("join1")){
        // 							containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration.UUID);
        // 						} else {
        // 							containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration2.UUID);
        // 						}

        // 						Assert.True(containsUUID);
        // 						Debug.Log("containsUUID:" + containsUUID);
        // 						bool containsOccupancy = mea.PresenceEventResult.Occupancy > 0;
        // 						Assert.True(containsOccupancy);
        // 						Debug.Log("containsOccupancy:" + containsOccupancy);

        // 						bool containsTimestamp = mea.PresenceEventResult.Timestamp > 0;
        // 						Assert.True(containsTimestamp);
        // 						Debug.Log("containsTimestamp:" + containsTimestamp);

        // 						bool containsSubscription = mea.PresenceEventResult.Subscription.Equals(channel);
        // 						Assert.True(containsSubscription);
        // 						Debug.Log("containsSubscription:" + containsSubscription);

        // 						tJoinResult = containsTimestamp && containsOccupancy && containsUUID && containsSubscription;
        // 					}
        // 				break;
        // 				case "leave":
        // 					if(mea.PresenceEventResult.Event.Equals("leave")){
        // 						bool containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration2.UUID);
        // 						Assert.True(containsUUID);
        // 						Debug.Log(containsUUID);
        // 						bool containsTimestamp = mea.PresenceEventResult.Timestamp > 0;
        // 						Assert.True(containsTimestamp);
        // 						bool containsSubscription = mea.PresenceEventResult.Subscription.Equals(channel);
        // 						Assert.True(containsSubscription);
        // 						bool containsOccupancy = mea.PresenceEventResult.Occupancy > 0;
        // 						Assert.True(containsOccupancy);
        // 						Debug.Log("containsSubscription:" + containsSubscription);
        // 						Debug.Log("containsTimestamp:" + containsTimestamp);
        // 						Debug.Log("containsOccupancy:" + containsOccupancy);
        // 						Debug.Log("containsUUID:" + containsUUID);

        // 						tLeaveResult = containsTimestamp && containsOccupancy && containsUUID && containsSubscription;
        // 					}
        // 				break;
        // 				default:
        // 					Debug.Log("SubscribeCallback" + mea.MessageResult.Subscription);
        // 					Debug.Log("SubscribeCallback" + mea.MessageResult.Channel);
        // 					Debug.Log("SubscribeCallback" + mea.MessageResult.Payload);
        // 					Debug.Log("SubscribeCallback" + mea.MessageResult.Timetoken);
        // 					bool matchChannel = mea.MessageResult.Channel.Equals(chToPub);
        // 					Assert.True(matchChannel);
        // 					bool matchPayload = mea.MessageResult.Payload.ToString().Equals(payload);
        // 					Assert.True(matchPayload);

        // 					bool matchSubscription = mea.MessageResult.Subscription.Equals(channel);
        // 					Assert.True(matchSubscription);
        // 					tresult = matchPayload  && matchChannel && matchSubscription;
        // 				break;
        // 			}
        // 		}
        // 	};
        // 	pubnub.Subscribe ().Channels(channelList2).Execute();
        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
        // 	Assert.True(tJoinResult, "subscribe didn't get a join");

        // 	whatToTest = "";

        // 	pubnub.Publish().Channel(chToPub).Message(payload).Async((result, status) => {
        // 		bool timetokenMatch = !result.Timetoken.Equals(0);
        // 		bool statusError = status.Error.Equals(false);
        // 		bool statusCodeMatch = status.StatusCode.Equals(0);
        // 		Assert.True(timetokenMatch);
        // 		Assert.True(statusError);
        // 		Assert.True(statusCodeMatch, status.StatusCode.ToString());
        // 		tresult = statusCodeMatch && statusError && timetokenMatch;
        // 	});
        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);

        // 	Assert.True(tresult, "Subcribe didn't get a message");

        // 	PubNub pubnub2 = new PubNub(pnConfiguration2);

        // 	whatToTest = "join2";

        // 	pubnub2.Subscribe ().Channels(channelList2).Execute();
        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
        // 	Assert.True(tJoinResult, "subscribe2 didn't get a join");

        // 	whatToTest = "leave";

        // 	tresult = false;
        // 	pubnub2.Unsubscribe().Channels(channelList2).Async((result, status) => {
        // 			Debug.Log("status.Error:" + status.Error);
        // 			tresult = !status.Error;
        // 		});
        // 	yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
        // 	Assert.True(tresult, "unsubscribe didn't return");
        // 	Assert.True(tLeaveResult, "subscribe didn't get a leave");

        // 	pubnub.CleanUp();
        // 	pubnub2.CleanUp();
        // }

        [UnityTest]
        public IEnumerator TestUnsubscribeAllAndUnsubscribe()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityWildSubscribeUUID_" + r.Next(100);
            string channel = "UnityWildSubscribeChannel." + r.Next(100);
            string channel2 = "UnityWildSubscribeChannel." + r.Next(100);

            string payload = string.Format("payload {0}", pnConfiguration.UUID);
            PubNub pubnub = new PubNub(pnConfiguration);

            List<string> channelList2 = new List<string>();
            channelList2.Add(channel);
            channelList2.Add(channel2);
            string whatToTest = "join1";
            bool tJoinResult = false;
            bool tLeaveResult = false;
            bool tresult = false;

            PNConfiguration pnConfiguration2 = PlayModeCommon.SetPNConfig(false);
            pnConfiguration2.UUID = "UnityWildSubscribeUUID2_" + r.Next(100);

            pubnub.SubscribeCallback += (sender, e) =>
            {
                SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;
                if (!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory))
                {
                    switch (whatToTest)
                    {
                        case "join1":
                        case "join2":
                            if (mea.PresenceEventResult.Event.Equals("join"))
                            {
                                bool containsUUID = false;
                                if (whatToTest.Equals("join1"))
                                {
                                    containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration.UUID);
                                }
                                else
                                {
                                    containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration2.UUID);
                                }
                                bool containsOccupancy = mea.PresenceEventResult.Occupancy > 0;
                                Assert.True(containsOccupancy);
                                bool containsTimestamp = mea.PresenceEventResult.Timestamp > 0;
                                Assert.True(containsTimestamp);
                                Debug.Log(containsUUID);
                                bool containsChannel = mea.PresenceEventResult.Channel.Equals(channel) || mea.PresenceEventResult.Channel.Equals(channel2);
                                Assert.True(containsChannel);
                                Debug.Log("containsChannel:" + containsChannel);
                                Debug.Log("containsTimestamp:" + containsTimestamp);
                                Debug.Log("containsOccupancy:" + containsOccupancy);
                                Debug.Log("containsUUID:" + containsUUID);

                                tJoinResult = containsTimestamp && containsOccupancy && containsUUID && containsChannel;
                            }
                            break;
                        case "leave":
                            if (mea.PresenceEventResult.Event.Equals("leave"))
                            {
                                bool containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration2.UUID);
                                Assert.True(containsUUID);
                                Debug.Log(containsUUID);
                                bool containsTimestamp = mea.PresenceEventResult.Timestamp > 0;
                                Assert.True(containsTimestamp);
                                bool containsChannel = mea.PresenceEventResult.Channel.Equals(channel) || mea.PresenceEventResult.Channel.Equals(channel2);
                                Assert.True(containsChannel);
                                bool containsOccupancy = mea.PresenceEventResult.Occupancy > 0;
                                Assert.True(containsOccupancy);
                                Debug.Log("containsChannel:" + containsChannel);
                                Debug.Log("containsTimestamp:" + containsTimestamp);
                                Debug.Log("containsOccupancy:" + containsOccupancy);
                                Debug.Log("containsUUID:" + containsUUID);

                                tLeaveResult = containsTimestamp && containsOccupancy && containsUUID && containsChannel;
                            }
                            break;
                        default:
                            Debug.Log("SubscribeCallback" + mea.MessageResult.Subscription);
                            Debug.Log("SubscribeCallback" + mea.MessageResult.Channel);
                            Debug.Log("SubscribeCallback" + mea.MessageResult.Payload);
                            Debug.Log("SubscribeCallback" + mea.MessageResult.Timetoken);
                            bool matchChannel = mea.MessageResult.Channel.Equals(channel);
                            Assert.True(matchChannel);
                            bool matchPayload = mea.MessageResult.Payload.ToString().Equals(payload);
                            Assert.True(matchPayload);

                            tresult = matchPayload && matchChannel;
                            break;
                    }
                }
            };
            pubnub.Subscribe().Channels(channelList2).WithPresence().Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            //Assert.True(tJoinResult, "subscribe didn't get a join");

            whatToTest = "join2";
            PubNub pubnub2 = new PubNub(pnConfiguration2);

            pubnub2.Subscribe().Channels(channelList2).Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tJoinResult, "subscribe2 didn't get a join");

            whatToTest = "leave";

            tresult = false;
            List<string> channelList = new List<string>();
            channelList.Add(channel);
            pubnub2.Unsubscribe().Channels(channelList).Async((result, status) =>
            {
                Debug.Log("status.Error:" + status.Error);
                tresult = !status.Error;
                //Debug.Log("result.Message:" + result.Message);
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tresult, "unsubscribe didn't return");

            tresult = false;
            pubnub2.UnsubscribeAll().Async((result, status) =>
            {
                Debug.Log("status.Error:" + status.Error);
                tresult = !status.Error;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tresult, "unsubscribeAll didn't return");
            Assert.True(tLeaveResult, "subscribe didn't get a leave");

            pubnub.CleanUp();
            pubnub2.CleanUp();
        }

        public IEnumerator TestReconnect()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityReconnectUUID" + r.Next(100);
            string channel = "UnityReconnectChannel." + r.Next(100);

            string payload = string.Format("Reconnect payload {0}", pnConfiguration.UUID);
            PubNub pubnub = new PubNub(pnConfiguration);

            List<string> channelList2 = new List<string>();
            channelList2.Add(channel);
            bool tresult = false;
            string whatToTest = "join1";

            PNConfiguration pnConfiguration2 = PlayModeCommon.SetPNConfig(false);
            pnConfiguration2.UUID = "UnityReconnectUUID2" + r.Next(100);

            pubnub.SubscribeCallback += (sender, e) =>
            {
                SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;

                switch (whatToTest)
                {
                    case "connected":
                        if (mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory))
                        {
                            tresult = true;
                        }
                        break;
                    case "join1":
                    case "join2":
                        if (!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory))
                        {
                            if ((mea.PresenceEventResult != null) && (mea.PresenceEventResult.Event.Equals("join")))
                            {
                                bool containsUUID = false;
                                if (whatToTest.Equals("join1"))
                                {
                                    containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration.UUID);
                                }
                                else
                                {
                                    containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration2.UUID);
                                }
                                bool containsOccupancy = mea.PresenceEventResult.Occupancy > 0;
                                Assert.True(containsOccupancy);
                                bool containsTimestamp = mea.PresenceEventResult.Timestamp > 0;
                                Assert.True(containsTimestamp);
                                Debug.Log(containsUUID);
                                bool containsChannel = mea.PresenceEventResult.Channel.Equals(channel);// || mea.PresenceEventResult.Channel.Equals(channel2);
                                Assert.True(containsChannel);
                                Debug.Log("containsChannel:" + containsChannel);
                                Debug.Log("containsTimestamp:" + containsTimestamp);
                                Debug.Log("containsOccupancy:" + containsOccupancy);
                                Debug.Log("containsUUID:" + containsUUID);

                                tresult = containsTimestamp && containsOccupancy && containsUUID && containsChannel;
                            }
                        }
                        break;
                    case "leave":
                        if (!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory))
                        {
                            if ((mea.PresenceEventResult != null) && (mea.PresenceEventResult.Event.Equals("leave")))
                            {
                                bool containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration2.UUID);
                                Assert.True(containsUUID);
                                Debug.Log(containsUUID);
                                bool containsTimestamp = mea.PresenceEventResult.Timestamp > 0;
                                Assert.True(containsTimestamp);
                                bool containsChannel = mea.PresenceEventResult.Channel.Equals(channel);// || mea.PresenceEventResult.Channel.Equals(channel2);
                                Assert.True(containsChannel);
                                bool containsOccupancy = mea.PresenceEventResult.Occupancy > 0;
                                Assert.True(containsOccupancy);
                                Debug.Log("containsChannel:" + containsChannel);
                                Debug.Log("containsTimestamp:" + containsTimestamp);
                                Debug.Log("containsOccupancy:" + containsOccupancy);
                                Debug.Log("containsUUID:" + containsUUID);

                                tresult = containsTimestamp && containsOccupancy && containsUUID && containsChannel;
                            }
                        }
                        break;
                    default:
                        if (!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory))
                        {
                            Debug.Log("SubscribeCallback" + mea.MessageResult.Subscription);
                            Debug.Log("SubscribeCallback" + mea.MessageResult.Channel);
                            Debug.Log("SubscribeCallback" + mea.MessageResult.Payload);
                            Debug.Log("SubscribeCallback" + mea.MessageResult.Timetoken);
                            bool matchChannel = mea.MessageResult.Channel.Equals(channel);
                            Assert.True(matchChannel);
                            bool matchPayload = mea.MessageResult.Payload.ToString().Equals(payload);
                            Assert.True(matchPayload);

                            tresult = matchPayload && matchChannel;
                        }
                        break;
                }
            };
            pubnub.Subscribe().Channels(channelList2).WithPresence().Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tresult, "didn't subscribe");

            whatToTest = "join2";
            PubNub pubnub2 = new PubNub(pnConfiguration2);

            tresult = false;

            pubnub2.Subscribe().Channels(channelList2).Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tresult, "subscribe2 didn't get a join");

            tresult = false;
            pubnub.Reconnect();

            pubnub2.Publish().Channel(channel).Message(payload).Async((result, status) =>
            {
                bool timetokenMatch = !result.Timetoken.Equals(0);
                bool statusError = status.Error.Equals(false);
                bool statusCodeMatch = status.StatusCode.Equals(0);
                Assert.True(timetokenMatch);
                Assert.True(statusError);
                Assert.True(statusCodeMatch, status.StatusCode.ToString());
                tresult = statusCodeMatch && statusError && timetokenMatch;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls3);

            Assert.True(tresult, "publish didn't return");

            whatToTest = "";

            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls3);
            Assert.True(tresult, "subscribe didn't return");

            pubnub.CleanUp();
            pubnub2.CleanUp();
        }

        public IEnumerator TestPresenceCG()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestCGPresUUID_" + r.Next(100);
            string channel = "UnityTestPresWithCGChannel";
            string channel2 = "UnityTestPresWithCGChannel2";
            List<string> channelList = new List<string>();
            channelList.Add(channel);
            channelList.Add(channel2);

            string channelGroup = "cg";
            List<string> channelGroupList = new List<string>();
            channelGroupList.Add(channelGroup);

            PubNub pubnub = new PubNub(pnConfiguration);
            bool tresult = false;

            PNConfiguration pnConfiguration2 = PlayModeCommon.SetPNConfig(false);
            pnConfiguration2.UUID = "UnityReconnectUUID2" + r.Next(100);

            pubnub.AddChannelsToChannelGroup().Channels(channelList).ChannelGroup(channelGroup).Async((result, status) =>
            {
                Debug.Log("in AddChannelsToChannelGroup " + status.Error);
                if (!status.Error)
                {
                    Debug.Log(result.Message);
                    tresult = result.Message.Contains("OK");
                }
                else
                {
                    Assert.Fail("AddChannelsToChannelGroup failed");
                }
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls3);
            Assert.True(tresult, "test didn't return1");
            tresult = false;
            string whatToTest = "join1";

            pubnub.SubscribeCallback += (sender, e) =>
            {
                SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;

                switch (whatToTest)
                {
                    case "join1":
                    case "join2":
                        if (!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory))
                        {
                            if (mea.PresenceEventResult.Event.Equals("join"))
                            {
                                bool containsUUID = false;
                                if (whatToTest.Equals("join1"))
                                {
                                    containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration.UUID);
                                }
                                else
                                {
                                    containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration2.UUID);
                                }
                                bool containsOccupancy = mea.PresenceEventResult.Occupancy > 0;
                                Assert.True(containsOccupancy);
                                bool containsTimestamp = mea.PresenceEventResult.Timestamp > 0;
                                Assert.True(containsTimestamp);
                                Debug.Log(containsUUID);
                                Debug.Log("mea.PresenceEventResult.Subscription:" + mea.PresenceEventResult.Subscription);
                                bool containsChannel = mea.PresenceEventResult.Subscription.Equals(channelGroup);// || mea.PresenceEventResult.Channel.Equals(channel2);
                                Assert.True(containsChannel);
                                Debug.Log("containsChannel:" + containsChannel);
                                Debug.Log("containsTimestamp:" + containsTimestamp);
                                Debug.Log("containsOccupancy:" + containsOccupancy);
                                Debug.Log("containsUUID:" + containsUUID);

                                tresult = containsTimestamp && containsOccupancy && containsUUID && containsChannel;
                            }
                        }
                        break;
                    case "leave":
                        if (!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory))
                        {
                            if (mea.PresenceEventResult.Event.Equals("leave"))
                            {
                                bool containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration2.UUID);
                                Assert.True(containsUUID);
                                Debug.Log(containsUUID);
                                bool containsTimestamp = mea.PresenceEventResult.Timestamp > 0;
                                Assert.True(containsTimestamp);
                                bool containsChannel = mea.PresenceEventResult.Subscription.Equals(channelGroup);// || mea.PresenceEventResult.Channel.Equals(channel2);
                                Assert.True(containsChannel);
                                bool containsOccupancy = mea.PresenceEventResult.Occupancy > 0;
                                Assert.True(containsOccupancy);
                                Debug.Log("containsChannel:" + containsChannel);
                                Debug.Log("containsTimestamp:" + containsTimestamp);
                                Debug.Log("containsOccupancy:" + containsOccupancy);
                                Debug.Log("containsUUID:" + containsUUID);

                                tresult = containsTimestamp && containsOccupancy && containsUUID && containsChannel;
                            }
                        }
                        break;
                    default:
                        break;
                }

            };

            pubnub.Subscribe().ChannelGroups(channelGroupList).WithPresence().Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            //Assert.True(tresult, "subscribe1 didn't get a join");

            whatToTest = "join2";
            PubNub pubnub2 = new PubNub(pnConfiguration2);

            tresult = false;

            pubnub2.Subscribe().ChannelGroups(channelGroupList).Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tresult, "subscribe2 didn't get a join");

            whatToTest = "leave";
            tresult = false;
            pubnub2.Unsubscribe().ChannelGroups(channelGroupList).Async((result, status) =>
            {
                Debug.Log("status.Error:" + status.Error);
                //tresult = !status.Error;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            //Assert.True(tresult, "unsubscribeAll didn't return");
            Assert.True(tresult, "subscribe didn't get a leave");

            pubnub.CleanUp();
            pubnub2.CleanUp();

        }

        [UnityTest]
        public IEnumerator TestHistory()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next(100);
            string channel = "UnityPublishAndHistoryChannel_" + r.Next(100);
            string payload = string.Format("payload {0}", pnConfiguration.UUID);

            PubNub pubnub = new PubNub(pnConfiguration);

            List<string> channelList2 = new List<string>();
            channelList2.Add(channel);
            bool tresult = false;

            long timetoken1 = 0;
            pubnub.Time().Async((result, status) =>
            {
                timetoken1 = result.TimeToken;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);

            Assert.True(!timetoken1.Equals(0));

            List<string> payloadList = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                payloadList.Add(string.Format("{0}, seq: {1}", payload, i));
            }

            //Get Time: t1
            //Publish 2 msg
            //get time: t2
            //Publish 2 msg
            //get time: t3

            for (int i = 0; i < 2; i++)
            {
                tresult = false;

                pubnub.Publish().Channel(channel).Message(payloadList[i]).Async((result, status) =>
                {
                    bool timetokenMatch = !result.Timetoken.Equals(0);
                    bool statusError = status.Error.Equals(false);
                    bool statusCodeMatch = status.StatusCode.Equals(0);
                    Assert.True(timetokenMatch);
                    Assert.True(statusError);
                    Assert.True(statusCodeMatch, status.StatusCode.ToString());
                    tresult = statusCodeMatch && statusError && timetokenMatch;
                });
                yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);

                Assert.True(tresult, string.Format("test didnt return {0}", i));
            }

            tresult = false;

            long timetoken2 = 0;
            pubnub.Time().Async((result, status) =>
            {
                timetoken2 = result.TimeToken;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);

            Assert.True(!timetoken2.Equals(0));

            for (int i = 2; i < 4; i++)
            {
                tresult = false;
                pubnub.Publish().Channel(channel).Message(payloadList[i]).Async((result, status) =>
                {
                    bool timetokenMatch = !result.Timetoken.Equals(0);
                    bool statusError = status.Error.Equals(false);
                    bool statusCodeMatch = status.StatusCode.Equals(0);
                    Assert.True(timetokenMatch);
                    Assert.True(statusError);
                    Assert.True(statusCodeMatch, status.StatusCode.ToString());
                    tresult = statusCodeMatch && statusError && timetokenMatch;
                });
                yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);

                Assert.True(tresult, string.Format("test didnt return {0}", i));
            }

            tresult = false;

            long timetoken3 = 0;
            pubnub.Time().Async((result, status) =>
            {
                timetoken3 = result.TimeToken;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);

            Assert.True(!timetoken3.Equals(0));

            tresult = false;

            //History t1 - t2

            int testCount = 2;
            int testStart = 0;
            pubnub.History().Channel(channel).Start(timetoken1).End(timetoken2).IncludeTimetoken(true).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                if (!status.Error)
                {

                    if ((result.Messages != null) && (result.Messages.Count.Equals(testCount)))
                    {
                        List<PNHistoryItemResult> listPNHistoryItemResult = result.Messages as List<PNHistoryItemResult>;
                        for (int i = 0; i < testCount; i++)
                        {
                            PNHistoryItemResult pnHistoryItemResult = listPNHistoryItemResult[i] as PNHistoryItemResult;
                            if (pnHistoryItemResult != null)
                            {
                                bool found = false;
                                for (int j = 0; j < testCount; j++)
                                {
                                    if (pnHistoryItemResult.Entry.ToString().Contains(payloadList[j]))
                                    {
                                        found = (pnHistoryItemResult.Timetoken > 0);
                                        Debug.Log("found" + payloadList[j]);
                                        break;
                                    }
                                }
                                tresult = found;
                                if (!tresult)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tresult, "history test didnt return");

            // testCount = 2;
            // testStart = 2;
            // pubnub.History().Channel(channel).Start(timetoken2).End(timetoken3).Async((result, status) => {
            // 	Assert.True(status.Error.Equals(false));
            // 	if(!status.Error){

            // 		if((result.Messages!=null) && (result.Messages.Count.Equals(testCount))){
            // 			List<PNHistoryItemResult> listPNHistoryItemResult = result.Messages as List<PNHistoryItemResult>;
            // 			for(int i=0; i<testCount; i++){
            // 				PNHistoryItemResult pnHistoryItemResult = listPNHistoryItemResult[i] as PNHistoryItemResult;
            // 				if(pnHistoryItemResult != null){
            // 					bool found = false;
            // 					for(int j=testStart; j<testCount+testStart; j++){
            // 						if(pnHistoryItemResult.Entry.ToString().Contains(payloadList[j])){
            // 							found = true;
            // 							Debug.Log("found" + payloadList[j] );
            // 							break;
            // 						}
            // 					}
            // 					tresult = found;
            // 					if(!tresult){
            // 						break;
            // 					}
            // 				}
            // 			}
            // 		}
            //     }
            // });
            //yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
            //Assert.True(tresult, "history test didnt return 2");

            pubnub.CleanUp();
        }

        [UnityTest]
        public IEnumerator TestHistory2()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next(100);
            string channel = "UnityPublishAndHistoryChannel2_" + r.Next(100);
            string payload = string.Format("payload {0}", pnConfiguration.UUID);

            PubNub pubnub = new PubNub(pnConfiguration);

            List<string> channelList2 = new List<string>();
            channelList2.Add(channel);
            bool tresult = false;

            long timetoken1 = 0;
            pubnub.Time().Async((result, status) =>
            {
                timetoken1 = result.TimeToken;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            Assert.True(!timetoken1.Equals(0));

            List<string> payloadList = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                payloadList.Add(string.Format("{0}, seq: {1}", payload, i));
            }

            //Get Time: t1
            //Publish 2 msg
            //get time: t2
            //Publish 2 msg
            //get time: t3

            for (int i = 0; i < 2; i++)
            {
                tresult = false;

                pubnub.Publish().Channel(channel).Message(payloadList[i]).Async((result, status) =>
                {
                    bool timetokenMatch = !result.Timetoken.Equals(0);
                    bool statusError = status.Error.Equals(false);
                    bool statusCodeMatch = status.StatusCode.Equals(0);
                    Assert.True(timetokenMatch);
                    Assert.True(statusError);
                    Assert.True(statusCodeMatch, status.StatusCode.ToString());
                    tresult = statusCodeMatch && statusError && timetokenMatch;
                });
                yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

                Assert.True(tresult, string.Format("test didnt return {0}", i));
            }

            tresult = false;

            long timetoken2 = 0;
            pubnub.Time().Async((result, status) =>
            {
                timetoken2 = result.TimeToken;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            Assert.True(!timetoken2.Equals(0));

            for (int i = 2; i < 4; i++)
            {
                tresult = false;
                pubnub.Publish().Channel(channel).Message(payloadList[i]).Async((result, status) =>
                {
                    bool timetokenMatch = !result.Timetoken.Equals(0);
                    bool statusError = status.Error.Equals(false);
                    bool statusCodeMatch = status.StatusCode.Equals(0);
                    Assert.True(timetokenMatch);
                    Assert.True(statusError);
                    Assert.True(statusCodeMatch, status.StatusCode.ToString());
                    tresult = statusCodeMatch && statusError && timetokenMatch;
                });
                yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

                Assert.True(tresult, string.Format("test didnt return {0}", i));
            }

            tresult = false;

            long timetoken3 = 0;
            pubnub.Time().Async((result, status) =>
            {
                timetoken3 = result.TimeToken;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            Assert.True(!timetoken3.Equals(0));

            tresult = false;

            int testCount = 2;
            int testStart = 2;
            pubnub.History().Channel(channel).Start(timetoken2).IncludeTimetoken(true).Reverse(true).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                if (!status.Error)
                {

                    if ((result.Messages != null) && (result.Messages.Count.Equals(testCount)))
                    {
                        List<PNHistoryItemResult> listPNHistoryItemResult = result.Messages as List<PNHistoryItemResult>;
                        for (int i = 0; i < testCount; i++)
                        {
                            PNHistoryItemResult pnHistoryItemResult = listPNHistoryItemResult[i] as PNHistoryItemResult;
                            if (pnHistoryItemResult != null)
                            {
                                bool found = false;
                                Debug.Log("finding:" + pnHistoryItemResult.Entry.ToString());
                                for (int j = testStart; j < testCount + testStart; j++)
                                {
                                    if (pnHistoryItemResult.Entry.ToString().Contains(payloadList[j]))
                                    {
                                        found = true;
                                        Debug.Log("found:" + payloadList[j]);
                                        break;
                                    }
                                }
                                tresult = found;
                                if (!tresult)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tresult, "history test didnt return");
            // tresult = false;
            // testCount = 2;
            // testStart = 2;
            // pubnub.History().Channel(channel).End(timetoken1).Async((result, status) => {
            // 	Assert.True(status.Error.Equals(false));
            // 	if(!status.Error){

            // 		if((result.Messages!=null) && (result.Messages.Count.Equals(testCount+testStart))){
            // 			List<PNHistoryItemResult> listPNHistoryItemResult = result.Messages as List<PNHistoryItemResult>;
            // 			for(int i=0; i<testCount; i++){
            // 				PNHistoryItemResult pnHistoryItemResult = listPNHistoryItemResult[i] as PNHistoryItemResult;
            // 				if(pnHistoryItemResult != null){
            // 					bool found = false;
            // 					Debug.Log("finding:" + pnHistoryItemResult.Entry.ToString() );
            // 					for(int j=0; j<testCount+testStart; j++){
            // 						if(pnHistoryItemResult.Entry.ToString().Contains(payloadList[j])){
            // 							found = true;
            // 							Debug.Log("found" + payloadList[j] );
            // 							break;
            // 						}
            // 					}
            // 					tresult = found;
            // 					if(!tresult){
            // 						break;
            // 					}
            // 				}
            // 			}
            // 		}
            //     }
            // });
            // yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
            // Assert.True(tresult, "history test didnt return 2");

            // testCount = 2;
            // testStart = 2;
            // pubnub.History().Channel(channel).Start(timetoken3).Async((result, status) => {
            // 	Assert.True(status.Error.Equals(false));
            // 	if(!status.Error){

            // 		if((result.Messages!=null) && (result.Messages.Count.Equals(testCount+testStart))){
            // 			List<PNHistoryItemResult> listPNHistoryItemResult = result.Messages as List<PNHistoryItemResult>;
            // 			for(int i=0; i<testCount; i++){
            // 				PNHistoryItemResult pnHistoryItemResult = listPNHistoryItemResult[i] as PNHistoryItemResult;
            // 				if(pnHistoryItemResult != null){
            // 					bool found = false;
            // 					Debug.Log("finding:" + pnHistoryItemResult.Entry.ToString() );
            // 					for(int j=0; j<testCount+testStart; j++){
            // 						if(pnHistoryItemResult.Entry.ToString().Contains(payloadList[j])){
            // 							found = true;
            // 							Debug.Log("found" + payloadList[j] );
            // 							break;
            // 						}
            // 					}
            // 					tresult = found;
            // 					if(!tresult){
            // 						break;
            // 					}
            // 				}
            // 			}
            // 		}
            //     }
            // });
            // yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
            // Assert.True(tresult, "history test didnt return 3");

            pubnub.CleanUp();
        }

        //Get Time: t1
        //Publish 2 msg to ch 1
        //get time: t2
        //Publish 2 msg to ch 2
        //get time: t3
        //Fetch ch 1 and ch 2
        [UnityTest]
        public IEnumerator TestFetch()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestFetchUUID_" + r.Next(100);
            string channel = "UnityPublishAndFetchChannel_" + r.Next(100);
            string channel2 = "UnityPublishAndFetchChannel2_" + r.Next(100);
            string payload = string.Format("payload {0}", pnConfiguration.UUID);

            PubNub pubnub = new PubNub(pnConfiguration);

            List<string> channelList2 = new List<string>();
            channelList2.Add(channel);
            channelList2.Add(channel2);
            bool tresult = false;

            long timetoken1 = 0;
            pubnub.Time().Async((result, status) =>
            {
                timetoken1 = result.TimeToken;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            Assert.True(!timetoken1.Equals(0));

            List<string> payloadList = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                payloadList.Add(string.Format("{0}, seq: {1}", payload, i));
            }

            for (int i = 0; i < 2; i++)
            {
                tresult = false;

                pubnub.Publish().Channel(channel).Message(payloadList[i]).Async((result, status) =>
                {
                    bool timetokenMatch = !result.Timetoken.Equals(0);
                    bool statusError = status.Error.Equals(false);
                    bool statusCodeMatch = status.StatusCode.Equals(0);
                    Assert.True(timetokenMatch);
                    Assert.True(statusError);
                    Assert.True(statusCodeMatch, status.StatusCode.ToString());
                    tresult = statusCodeMatch && statusError && timetokenMatch;
                });
                yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

                Assert.True(tresult, string.Format("test didnt return {0}", i));
            }

            tresult = false;

            long timetoken2 = 0;
            pubnub.Time().Async((result, status) =>
            {
                timetoken2 = result.TimeToken;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            Assert.True(!timetoken2.Equals(0));

            for (int i = 2; i < 4; i++)
            {
                tresult = false;
                pubnub.Publish().Channel(channel2).Message(payloadList[i]).Async((result, status) =>
                {
                    bool timetokenMatch = !result.Timetoken.Equals(0);
                    bool statusError = status.Error.Equals(false);
                    bool statusCodeMatch = status.StatusCode.Equals(0);
                    Assert.True(timetokenMatch);
                    Assert.True(statusError);
                    Assert.True(statusCodeMatch, status.StatusCode.ToString());
                    tresult = statusCodeMatch && statusError && timetokenMatch;
                });
                yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

                Assert.True(tresult, string.Format("test didnt return {0}", i));
            }

            tresult = false;

            long timetoken3 = 0;
            pubnub.Time().Async((result, status) =>
            {
                timetoken3 = result.TimeToken;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            Assert.True(!timetoken3.Equals(0));

            tresult = false;

            pubnub.FetchMessages().Channels(channelList2).IncludeTimetoken(true).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                if (!status.Error)
                {

                    if ((result.Channels != null) && (result.Channels.Count.Equals(2)))
                    {
                        Dictionary<string, List<PNMessageResult>> fetchResult = result.Channels as Dictionary<string, List<PNMessageResult>>;
                        Debug.Log("fetchResult.Count:" + fetchResult.Count);
                        bool found1 = false, found2 = false;
                        foreach (KeyValuePair<string, List<PNMessageResult>> kvp in fetchResult)
                        {
                            Debug.Log("Channel:" + kvp.Key);
                            if (kvp.Key.Equals(channel))
                            {

                                foreach (PNMessageResult msg in kvp.Value)
                                {
                                    Debug.Log("msg.Channel:" + msg.Channel);
                                    Debug.Log("msg.Payload.ToString():" + msg.Payload.ToString());
                                    if (msg.Channel.Equals(channel) && (msg.Payload.ToString().Equals(payloadList[0]) || (msg.Payload.ToString().Equals(payloadList[1]))))
                                    {
                                        found1 = true;
                                    }
                                }
                                if (!found1)
                                {
                                    break;
                                }
                            }
                            if (kvp.Key.Equals(channel2))
                            {
                                foreach (PNMessageResult msg in kvp.Value)
                                {
                                    Debug.Log("msg.Channel" + msg.Channel);
                                    Debug.Log("msg.Payload.ToString()" + msg.Payload.ToString());

                                    if (msg.Channel.Equals(channel2) && (msg.Payload.Equals(payloadList[2]) || (msg.Payload.Equals(payloadList[3]))))
                                    {
                                        found2 = true;
                                    }
                                }
                                if (!found2)
                                {
                                    break;
                                }
                            }
                        }
                        tresult = found1 && found2;
                    }

                }
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tresult, "fetch test didnt return");
            /*tresult = false;
            pubnub.FetchMessages().Channels(channelList2).Start(timetoken2).Reverse(true).Async((result, status) => {
                Assert.True(status.Error.Equals(false));
                if(!status.Error){

                    if((result.Channels != null) && (result.Channels.Count.Equals(1))){
                        Dictionary<string, List<PNMessageResult>> fetchResult = result.Channels as Dictionary<string, List<PNMessageResult>>;
                        Debug.Log("fetchResult.Count:" + fetchResult.Count);
                        bool found1 = false, found2 = false;
                        foreach(KeyValuePair<string, List<PNMessageResult>> kvp in fetchResult){
                            Debug.Log("Channel:" + kvp.Key);
                            if(kvp.Key.Equals(channel)){

                                foreach(PNMessageResult msg in kvp.Value){
                                    Debug.Log("msg.Channel:" + msg.Channel);
                                    Debug.Log("msg.Payload.ToString():" + msg.Payload.ToString());
                                    if(msg.Channel.Equals(channel) && (msg.Payload.ToString().Equals(payloadList[0]) || (msg.Payload.ToString().Equals(payloadList[1])))){
                                        found1 = true;
                                    }
                                }
                                if(!found1){
                                    break;
                                }
                            }
                        }
                        tresult = found1;
                    }

                }
            });
            yield return new WaitForSeconds (7);
            Assert.True(tresult, "fetch test didnt return 2");
            tresult = false;

            pubnub.FetchMessages().Channels(channelList2).End(timetoken1).Async((result, status) => {
                Assert.True(status.Error.Equals(false));
                if(!status.Error){

                    if((result.Channels != null) && (result.Channels.Count.Equals(2))){
                        Dictionary<string, List<PNMessageResult>> fetchResult = result.Channels as Dictionary<string, List<PNMessageResult>>;
                        Debug.Log("fetchResult.Count:" + fetchResult.Count);
                        bool found1 = false, found2 = false;
                        foreach(KeyValuePair<string, List<PNMessageResult>> kvp in fetchResult){
                            Debug.Log("Channel:" + kvp.Key);
                            if(kvp.Key.Equals(channel)){

                                foreach(PNMessageResult msg in kvp.Value){
                                    Debug.Log("msg.Channel:" + msg.Channel);
                                    Debug.Log("msg.Payload.ToString():" + msg.Payload.ToString());
                                    if(msg.Channel.Equals(channel) && (msg.Payload.ToString().Equals(payloadList[0]) || (msg.Payload.ToString().Equals(payloadList[1])))){
                                        found1 = true;
                                    }
                                }
                                if(!found1){
                                    break;
                                }
                            }
                            if(kvp.Key.Equals(channel2)){
                                foreach(PNMessageResult msg in kvp.Value){
                                    Debug.Log("msg.Channel" + msg.Channel);
                                    Debug.Log("msg.Payload.ToString()" + msg.Payload.ToString());

                                    if(msg.Channel.Equals(channel2) && (msg.Payload.Equals(payloadList[2]) || (msg.Payload.Equals(payloadList[3])))){
                                        found2 = true;
                                    }
                                }
                                if(!found2){
                                    break;
                                }
                            }
                        }
                        tresult = found1 && found2;

                    }

                }
            });
            yield return new WaitForSeconds (7);
            Assert.True(tresult, "fetch test didnt return 3");*/

            pubnub.CleanUp();
        }

        [UnityTest]
        public IEnumerator TestFetch3()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestFetchUUID_" + r.Next(100);
            string channel = "UnityPublishAndFetchChannel_" + r.Next(100);
            string channel2 = "UnityPublishAndFetchChannel2_" + r.Next(100);
            string payload = string.Format("payload {0}", pnConfiguration.UUID);

            PubNub pubnub = new PubNub(pnConfiguration);

            List<string> channelList2 = new List<string>();
            channelList2.Add(channel);
            channelList2.Add(channel2);
            bool tresult = false;

            long timetoken1 = 0;
            pubnub.Time().Async((result, status) =>
            {
                timetoken1 = result.TimeToken;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            Assert.True(!timetoken1.Equals(0));

            List<string> payloadList = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                payloadList.Add(string.Format("{0}, seq: {1}", payload, i));
            }

            for (int i = 0; i < 2; i++)
            {
                tresult = false;

                pubnub.Publish().Channel(channel).Message(payloadList[i]).Async((result, status) =>
                {
                    bool timetokenMatch = !result.Timetoken.Equals(0);
                    bool statusError = status.Error.Equals(false);
                    bool statusCodeMatch = status.StatusCode.Equals(0);
                    Assert.True(timetokenMatch);
                    Assert.True(statusError);
                    Assert.True(statusCodeMatch, status.StatusCode.ToString());
                    tresult = statusCodeMatch && statusError && timetokenMatch;
                });
                yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

                Assert.True(tresult, string.Format("test didnt return {0}", i));
            }

            tresult = false;

            long timetoken2 = 0;
            pubnub.Time().Async((result, status) =>
            {
                timetoken2 = result.TimeToken;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            Assert.True(!timetoken2.Equals(0));

            for (int i = 2; i < 4; i++)
            {
                tresult = false;
                pubnub.Publish().Channel(channel2).Message(payloadList[i]).Async((result, status) =>
                {
                    bool timetokenMatch = !result.Timetoken.Equals(0);
                    bool statusError = status.Error.Equals(false);
                    bool statusCodeMatch = status.StatusCode.Equals(0);
                    Assert.True(timetokenMatch);
                    Assert.True(statusError);
                    Assert.True(statusCodeMatch, status.StatusCode.ToString());
                    tresult = statusCodeMatch && statusError && timetokenMatch;
                });
                yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

                Assert.True(tresult, string.Format("test didnt return {0}", i));
            }

            tresult = false;

            long timetoken3 = 0;
            pubnub.Time().Async((result, status) =>
            {
                timetoken3 = result.TimeToken;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            Assert.True(!timetoken3.Equals(0));

            tresult = false;
            pubnub.FetchMessages().Channels(channelList2).End(timetoken1).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                if (!status.Error)
                {

                    if ((result.Channels != null) && (result.Channels.Count.Equals(2)))
                    {
                        Dictionary<string, List<PNMessageResult>> fetchResult = result.Channels as Dictionary<string, List<PNMessageResult>>;
                        Debug.Log("fetchResult.Count:" + fetchResult.Count);
                        bool found1 = false, found2 = false;
                        foreach (KeyValuePair<string, List<PNMessageResult>> kvp in fetchResult)
                        {
                            Debug.Log("Channel:" + kvp.Key);
                            if (kvp.Key.Equals(channel))
                            {

                                foreach (PNMessageResult msg in kvp.Value)
                                {
                                    Debug.Log("msg.Channel:" + msg.Channel);
                                    Debug.Log("msg.Payload.ToString():" + msg.Payload.ToString());
                                    if (msg.Channel.Equals(channel) && (msg.Payload.ToString().Equals(payloadList[0]) || (msg.Payload.ToString().Equals(payloadList[1]))))
                                    {
                                        found1 = true;
                                    }
                                }
                                if (!found1)
                                {
                                    break;
                                }
                            }
                            if (kvp.Key.Equals(channel2))
                            {
                                foreach (PNMessageResult msg in kvp.Value)
                                {
                                    Debug.Log("msg.Channel" + msg.Channel);
                                    Debug.Log("msg.Payload.ToString()" + msg.Payload.ToString());

                                    if (msg.Channel.Equals(channel2) && (msg.Payload.Equals(payloadList[2]) || (msg.Payload.Equals(payloadList[3]))))
                                    {
                                        found2 = true;
                                    }
                                }
                                if (!found2)
                                {
                                    break;
                                }
                            }
                        }
                        tresult = found1 && found2;

                    }

                }
            });
            yield return new WaitForSeconds(7);
            Assert.True(tresult, "fetch test didnt return 3");
            pubnub.CleanUp();
        }

        [UnityTest]
        public IEnumerator TestFetch2()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestFetchUUID_" + r.Next(100);
            string channel = "UnityPublishAndFetchChannel_" + r.Next(100);
            string channel2 = "UnityPublishAndFetchChannel2_" + r.Next(100);
            string payload = string.Format("payload {0}", pnConfiguration.UUID);

            PubNub pubnub = new PubNub(pnConfiguration);

            List<string> channelList2 = new List<string>();
            channelList2.Add(channel);
            channelList2.Add(channel2);
            bool tresult = false;

            long timetoken1 = 0;
            pubnub.Time().Async((result, status) =>
            {
                timetoken1 = result.TimeToken;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            Assert.True(!timetoken1.Equals(0));

            List<string> payloadList = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                payloadList.Add(string.Format("{0}, seq: {1}", payload, i));
            }

            for (int i = 0; i < 2; i++)
            {
                tresult = false;

                pubnub.Publish().Channel(channel).Message(payloadList[i]).Async((result, status) =>
                {
                    bool timetokenMatch = !result.Timetoken.Equals(0);
                    bool statusError = status.Error.Equals(false);
                    bool statusCodeMatch = status.StatusCode.Equals(0);
                    Assert.True(timetokenMatch);
                    Assert.True(statusError);
                    Assert.True(statusCodeMatch, status.StatusCode.ToString());
                    tresult = statusCodeMatch && statusError && timetokenMatch;
                });
                yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

                Assert.True(tresult, string.Format("test didnt return {0}", i));
            }

            tresult = false;

            long timetoken2 = 0;
            pubnub.Time().Async((result, status) =>
            {
                timetoken2 = result.TimeToken;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            Assert.True(!timetoken2.Equals(0));

            for (int i = 2; i < 4; i++)
            {
                tresult = false;
                pubnub.Publish().Channel(channel2).Message(payloadList[i]).Async((result, status) =>
                {
                    bool timetokenMatch = !result.Timetoken.Equals(0);
                    bool statusError = status.Error.Equals(false);
                    bool statusCodeMatch = status.StatusCode.Equals(0);
                    Assert.True(timetokenMatch);
                    Assert.True(statusError);
                    Assert.True(statusCodeMatch, status.StatusCode.ToString());
                    tresult = statusCodeMatch && statusError && timetokenMatch;
                });
                yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

                Assert.True(tresult, string.Format("test didnt return {0}", i));
            }

            tresult = false;

            long timetoken3 = 0;
            pubnub.Time().Async((result, status) =>
            {
                timetoken3 = result.TimeToken;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            Assert.True(!timetoken3.Equals(0));

            tresult = false;
            pubnub.FetchMessages().Channels(channelList2).Start(timetoken2).Reverse(true).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Debug.Log("status.Error.Equals(false)" + status.Error.Equals(false));
                if (!status.Error)
                {

                    if ((result.Channels != null))
                    {
                        Debug.Log("(result.Channels != null) && (result.Channels.Count.Equals(1))" + ((result.Channels != null) && (result.Channels.Count.Equals(1))));
                        Dictionary<string, List<PNMessageResult>> fetchResult = result.Channels as Dictionary<string, List<PNMessageResult>>;
                        Debug.Log("fetchResult.Count:" + fetchResult.Count);
                        bool found1 = false, found2 = false;
                        foreach (KeyValuePair<string, List<PNMessageResult>> kvp in fetchResult)
                        {
                            Debug.Log("Channel:" + kvp.Key);
                            if (kvp.Key.Equals(channel))
                            {

                                foreach (PNMessageResult msg in kvp.Value)
                                {
                                    Debug.Log("msg.Channel:" + msg.Channel);
                                    Debug.Log("msg.Payload.ToString():" + msg.Payload.ToString());
                                    if (msg.Channel.Equals(channel) && (msg.Payload.ToString().Equals(payloadList[0]) || (msg.Payload.ToString().Equals(payloadList[1]))))
                                    {
                                        found1 = true;
                                    }
                                }
                                if (!found1)
                                {
                                    break;
                                }
                            }
                        }
                        tresult = found1;
                    }
                    else
                    {
                        Debug.Log("(result.Channels == null) && !(result.Channels.Count.Equals(1))" + result.Channels.Count);
                    }

                }
            });
            yield return new WaitForSeconds(10);
            Assert.True(tresult, "fetch test didnt return 2");
            pubnub.CleanUp();
        }


        /*[UnityTest]
        public IEnumerator TestSetGetDeleteState() {

            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random ();
            pnConfiguration.UUID = "UnityTestCGUUID_" + r.Next (100);
            string channel = "UnityTestWithCGChannel";
            string channel2 = "UnityTestWithCGChannel2";
            List<string> channelList = new List<string>();
            channelList.Add(channel);
            channelList.Add(channel2);

            PubNub pubnub = new PubNub(pnConfiguration);
            bool tresult = false;

            tresult = false;
            string payload = string.Format("payload {0}", pnConfiguration.UUID);

            pubnub.SubscribeCallback += (sender, e) => {
                SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;
                if(!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
                    if(mea.MessageResult!=null){
                        Debug.Log("SubscribeCallback" + mea.MessageResult.Subscription);
                        Debug.Log("SubscribeCallback" + mea.MessageResult.Channel);
                        Debug.Log("SubscribeCallback" + mea.MessageResult.Payload);
                        Debug.Log("SubscribeCallback" + mea.MessageResult.Timetoken);
                        bool matchChannel = mea.MessageResult.Channel.Equals(channel);
                        Assert.True(matchChannel);
                        bool matchPayload = mea.MessageResult.Payload.ToString().Equals(payload);
                        Assert.True(matchPayload);
                        tresult = matchPayload && matchChannel;
                    }
                }

            };

            pubnub.Subscribe ().SetChannels(channelList).Execute();
            yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);

            //tresult = false;
            pubnub.Publish().Channel(channel).Message(payload).Async((result, status) => {
                Assert.True(!result.Timetoken.Equals(0));
                Assert.True(status.Error.Equals(false));
                Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
            });
            yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
            Assert.True(tresult, "test didn't return 3");
            tresult = false;

            Dictionary<string, object> state = new Dictionary<string, object>();
            state.Add  ("k1", "v1");
            pubnub.SetPresenceState().Channels(channelList).State(state).Async ((result, status) => {
                if(status.Error){
                    Assert.Fail("SetPresenceState failed");
                } else {
                    if(result != null){
                        if(result.StateByChannels!= null){
                            foreach (KeyValuePair<string, object> key in result.StateByChannels){
                                Debug.Log("key.Key" + key.Key);
                                if(key.Key.Equals(channel)){
                                    Debug.Log("pubnub.JsonLibrary.SerializeToJsonString(key.Value)" + pubnub.JsonLibrary.SerializeToJsonString(key.Value));
                                    bool stateMatch = pubnub.JsonLibrary.SerializeToJsonString(key.Value).Equals("{\"k1\":\"v1\"}");
                                    Assert.IsTrue(stateMatch);
                                    tresult = stateMatch;
                                    break;
                                }
                            }
                        }
                    }
                }
            });
            yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tresult, "test didn't return 4");

            tresult = false;

            pubnub.GetPresenceState().Channels(channelList).Async ((result, status) => {
                if(status.Error){
                    Assert.Fail("GetPresenceState failed");
                } else {
                    if(result != null){
                        if(result.StateByChannels!= null){
                            bool found1= false, found2 = false;
                            foreach (KeyValuePair<string, object> key in result.StateByChannels){
                                Debug.Log("key.Key" + key.Key);
                                Dictionary<string, object> stateDict = key.Value as Dictionary<string, object>;
                                foreach (KeyValuePair<string, object> keyInStateDict in stateDict){
                                    Debug.Log("keyInStateDict.Key" + keyInStateDict.Key);
                                    if(keyInStateDict.Key.Equals(channel)){
                                        bool stateMatch = pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value).Equals("\"UnityTestWithCGChannel\":{\"k1\":\"v1\"}");
                                        Debug.Log("pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value)" + pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value));
                                        Assert.IsTrue(stateMatch);
                                        found1 = stateMatch;
                                    }
                                    if(keyInStateDict.Key.Equals(channel2)){
                                        bool stateMatch = pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value).Equals("\"UnityTestWithCGChannel2\":{\"k1\":\"v1\"}");
                                        Debug.Log("pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value)" + pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value));
                                        Assert.IsTrue(stateMatch);
                                        found2 = stateMatch;
                                    }
                                }
                            }
                            Assert.IsTrue(found1);
                            Assert.IsTrue(found2);
                            tresult = found1 && found2 ;
                        }
                    }
                }
            });

            yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tresult, "test didn't return 5");

            tresult = false;
            Dictionary<string, object> stateToDel = new Dictionary<string, object> ();
            stateToDel.Add("k1", "");

            pubnub.SetPresenceState().Channels(channelList).State(stateToDel).Async ((result, status) => {
                if(status.Error){
                    Assert.Fail("SetPresenceState null failed");
                } else {
                    if(result != null){
                        if(result.StateByChannels!= null){
                            bool found = false;
                            foreach (KeyValuePair<string, object> key in result.StateByChannels){
                                Debug.Log("key.Key" + key.Key);
                                if(key.Key.Equals(channel)){
                                    bool stateMatch = pubnub.JsonLibrary.SerializeToJsonString(key.Value).Equals("{\"k1\":\"v1\"}");
                                    Debug.Log("pubnub.JsonLibrary.SerializeToJsonString(key.Value)" + pubnub.JsonLibrary.SerializeToJsonString(key.Value));
                                    found = stateMatch;
                                    break;
                                }
                            }
                            Assert.IsFalse(found);
                            tresult = !found;
                        }
                    }
                }
            });

            yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tresult, "test didn't return 6");

            tresult = false;

            pubnub.GetPresenceState().Channels(channelList).Async ((result, status) => {
            //pubnub.SetPresenceState().Channels(new List<string> (){ch1}).State(state).Async ((result, status) => {
                if(status.Error){
                    Assert.Fail("GetPresenceState failed");
                } else {
                    if(result != null){
                        if(result.StateByChannels!= null){
                            bool found1= false, found2 = false;
                            foreach (KeyValuePair<string, object> key in result.StateByChannels){
                                Debug.Log("key.Key" + key.Key);
                                Dictionary<string, object> stateDict = key.Value as Dictionary<string, object>;
                                foreach (KeyValuePair<string, object> keyInStateDict in stateDict){
                                    Debug.Log("keyInStateDict.Key" + keyInStateDict.Key);
                                    if(keyInStateDict.Key.Equals(channel)){
                                        bool stateMatch = pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value).Equals("{\"cg\":{\"k1\":\"v1\"}}");
                                        Debug.Log("pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value)" + pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value));
                                        Assert.IsFalse(stateMatch);
                                        found1 = stateMatch;
                                    }
                                    if(keyInStateDict.Key.Equals(channel2)){
                                        bool stateMatch = pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value).Equals("{\"cg\":{\"k1\":\"v1\"}}");
                                        Debug.Log("pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value)" + pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value));
                                        Assert.IsFalse(stateMatch);
                                        found2 = stateMatch;
                                    }
                                }
                            }
                            Assert.IsTrue(!found1);
                            Assert.IsTrue(!found2);
                            tresult = !found1 && !found2;
                        }
                    }
                }
            });

            yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tresult, "test didn't return 7");

        }*/

        [UnityTest]
        public IEnumerator TestUnsubscribeNoLeave()
        {
            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityUnsubUUID_" + r.Next(100);
            string channel = "UnityUnubscribeChannel." + r.Next(100);
            string channel2 = "UnityUnubscribeChannel." + r.Next(100);

            string payload = string.Format("payload {0}", pnConfiguration.UUID);
            //PubNub pubnub = new PubNub(pnConfiguration);

            List<string> channelList2 = new List<string>();
            channelList2.Add(channel);
            channelList2.Add(channel2);
            string whatToTest = "join1";
            bool tJoinResult = false;
            bool tLeaveResult = false;
            bool tresult = false;

            PNConfiguration pnConfiguration2 = PlayModeCommon.SetPNConfig(false);
            pnConfiguration2.UUID = "UnityUnsubUUID2_" + r.Next(100);
            pnConfiguration2.SuppressLeaveEvents = true;
            PubNub pubnub2 = new PubNub(pnConfiguration2);

            pubnub2.SubscribeCallback += (sender, e) =>
            {
                SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;
                if (!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory))
                {
                    switch (whatToTest)
                    {
                        case "join1":
                        case "join2":
                            if (mea.PresenceEventResult.Event.Equals("join"))
                            {
                                bool containsUUID = false;
                                if (whatToTest.Equals("join1"))
                                {
                                    containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration.UUID);
                                }
                                else
                                {
                                    containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration2.UUID);
                                }
                                bool containsOccupancy = mea.PresenceEventResult.Occupancy > 0;
                                Assert.True(containsOccupancy);
                                bool containsTimestamp = mea.PresenceEventResult.Timestamp > 0;
                                Assert.True(containsTimestamp);
                                Debug.Log(containsUUID);
                                bool containsChannel = mea.PresenceEventResult.Channel.Equals(channel) || mea.PresenceEventResult.Channel.Equals(channel2);
                                Assert.True(containsChannel);
                                Debug.Log("containsChannel:" + containsChannel);
                                Debug.Log("containsTimestamp:" + containsTimestamp);
                                Debug.Log("containsOccupancy:" + containsOccupancy);
                                Debug.Log("containsUUID:" + containsUUID);

                                tJoinResult = containsTimestamp && containsOccupancy && containsUUID && containsChannel;
                            }
                            break;
                        case "leave":
                            if (mea.PresenceEventResult.Event.Equals("leave"))
                            {
                                bool containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration2.UUID);
                                Assert.True(containsUUID);
                                Debug.Log(containsUUID);
                                bool containsChannel = mea.PresenceEventResult.Channel.Equals(channel) || mea.PresenceEventResult.Channel.Equals(channel2);
                                Assert.True(containsChannel);
                                Debug.Log("containsChannel:" + containsChannel);
                                Debug.Log("containsUUID:" + containsUUID);

                                tLeaveResult = containsUUID && containsChannel;
                            }
                            break;
                        default:
                            Debug.Log("SubscribeCallback" + mea.MessageResult.Subscription);
                            Debug.Log("SubscribeCallback" + mea.MessageResult.Channel);
                            Debug.Log("SubscribeCallback" + mea.MessageResult.Payload);
                            Debug.Log("SubscribeCallback" + mea.MessageResult.Timetoken);
                            bool matchChannel = mea.MessageResult.Channel.Equals(channel);
                            Assert.True(matchChannel);
                            bool matchPayload = mea.MessageResult.Payload.ToString().Equals(payload);
                            Assert.True(matchPayload);

                            tresult = matchPayload && matchChannel;
                            break;
                    }
                }
            };
            //pubnub2.Subscribe ().Channels(channelList2).WithPresence().Execute();
            //yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
            //Assert.True(tJoinResult, "subscribe didn't get a join");

            whatToTest = "join2";


            pubnub2.Subscribe().Channels(channelList2).WithPresence().Execute();
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tJoinResult, "subscribe2 didn't get a join");

            whatToTest = "leave";

            tresult = false;
            List<string> channelList = new List<string>();
            channelList.Add(channel);
            tLeaveResult = false;
            pubnub2.Unsubscribe().Channels(channelList).Async((result, status) =>
            {
                Debug.Log("status.Error:" + status.Error);
                tresult = !status.Error;
                //Debug.Log("result.Message:" + result.Message);

            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tresult, "unsubscribe didn't return");
            Assert.True(!tLeaveResult, "subscribe got a leave");

            tresult = false;
            tLeaveResult = false;
            pubnub2.UnsubscribeAll().Async((result, status) =>
            {
                Debug.Log("status.Error:" + status.Error);
                tresult = !status.Error;
                //Debug.Log("result.Message:" + result.Message);
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);
            Assert.True(tresult, "unsubscribeAll didn't return");
            Assert.True(!tLeaveResult, "subscribe got a leave 2");

            //pubnub.CleanUp();
            pubnub2.CleanUp();
        }
        #endregion

        #region "MessageCountsTests"


        #endregion
        [UnityTest]
        public IEnumerator TestMessageCounts()
        {

            PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
            pnConfiguration.ConcurrentNonSubscribeWorkers = 5;
            System.Random r = new System.Random();
            pnConfiguration.UUID = "UnityTestMessageCountsUUID_" + r.Next(100);
            string channel = "UnityPublishAndMessageCountsChannel_" + r.Next(100);
            string channel2 = "UnityPublishAndMessageCountsChannel2_" + r.Next(100);
            string payload = string.Format("payload {0}", pnConfiguration.UUID);

            PubNub pubnub = new PubNub(pnConfiguration);

            List<string> channelList2 = new List<string>();
            channelList2.Add(channel);
            channelList2.Add(channel2);
            bool tresult = false;
            pubnub.MessageCounts().Channels(channelList2).ChannelsTimetoken(new List<long> { 10, 11, 12 }).Async((result, status) =>
            {
                tresult = true;
                Assert.True(status.Error.Equals(true));

            });
            yield return new WaitForSeconds(1);

            tresult = false;
            long timetoken1 = 0;
            pubnub.Time().Async((result, status) =>
            {
                timetoken1 = result.TimeToken;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            Assert.True(!timetoken1.Equals(0));

            List<string> payloadList = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                payloadList.Add(string.Format("{0}, seq: {1}", payload, i));
            }

            for (int i = 0; i < 2; i++)
            {
                tresult = false;

                pubnub.Publish().Channel(channel).Message(payloadList[i]).Async((result, status) =>
                {
                    bool timetokenMatch = !result.Timetoken.Equals(0);
                    bool statusError = status.Error.Equals(false);
                    bool statusCodeMatch = status.StatusCode.Equals(0);
                    Assert.True(timetokenMatch);
                    Assert.True(statusError);
                    Assert.True(statusCodeMatch, status.StatusCode.ToString());
                    //Debug.Log(status.ErrorData + "" + status.StatusCode);
                    tresult = statusCodeMatch && statusError && timetokenMatch;
                });
                yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);

                Assert.True(tresult, string.Format("test didnt return {0}", i));
            }

            tresult = false;

            long timetoken2 = 0;
            pubnub.Time().Async((result, status) =>
            {
                timetoken2 = result.TimeToken;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            Assert.True(!timetoken2.Equals(0));

            for (int i = 2; i < 5; i++)
            {
                tresult = false;
                pubnub.Publish().Channel(channel2).Message(payloadList[i]).Async((result, status) =>
                {
                    bool timetokenMatch = !result.Timetoken.Equals(0);
                    bool statusError = status.Error.Equals(false);
                    bool statusCodeMatch = status.StatusCode.Equals(0);
                    Assert.True(timetokenMatch);
                    Assert.True(statusError);
                    Assert.True(statusCodeMatch, status.StatusCode.ToString());
                    tresult = statusCodeMatch && statusError && timetokenMatch;
                });
                yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls2);

                Assert.True(tresult, string.Format("test didnt return {0}", i));
            }

            tresult = false;

            long timetoken3 = 0;
            pubnub.Time().Async((result, status) =>
            {
                timetoken3 = result.TimeToken;
            });
            yield return new WaitForSeconds(PlayModeCommon.WaitTimeBetweenCalls);

            Assert.True(!timetoken3.Equals(0));

            tresult = false;
            pubnub.MessageCounts().Channels(channelList2).ChannelsTimetoken(new List<long> { timetoken2, timetoken3 }).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Debug.Log("status.Error.Equals(false)" + status.Error.Equals(false));
                if (!status.Error)
                {

                    if ((result.Channels != null))
                    {
                        Debug.Log(string.Format("MessageCounts, {0}", result.Channels.Count));
                        foreach (KeyValuePair<string, int> kvp in result.Channels)
                        {
                            Debug.Log(string.Format("==kvp.Key {0}, kvp.Value {1} ", kvp.Key, kvp.Value));
                            if (kvp.Key.Equals(channel))
                            {
                                tresult = true;
                                Debug.Log(string.Format("kvp.Key {0}, kvp.Value {1} ", kvp.Key, kvp.Value));
                                Assert.Equals(2, kvp.Value);
                            }
                            if (kvp.Key.Equals(channel2))
                            {
                                tresult = true;
                                Debug.Log(string.Format("kvp.Key {0}, kvp.Value {1} ", kvp.Key, kvp.Value));
                                Assert.Equals(3, kvp.Value);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("(result.Channels == null) && !(result.Channels.Count.Equals(1))" + result.Channels.Count);
                    }
                }
            });
            yield return new WaitForSeconds(3);
            Assert.True(tresult, "MessageCounts test didnt return 2");

            tresult = false;
            pubnub.MessageCounts().Channels(channelList2).ChannelsTimetoken(new List<long> { timetoken2 }).Async((result, status) =>
            {
                Assert.True(status.Error.Equals(false));
                Debug.Log("status.Error.Equals(false)" + status.Error.Equals(false));
                if (!status.Error)
                {

                    if ((result.Channels != null))
                    {
                        Debug.Log(string.Format("MessageCounts, {0}", result.Channels.Count));
                        foreach (KeyValuePair<string, int> kvp in result.Channels)
                        {
                            Debug.Log(string.Format("==kvp.Key {0}, kvp.Value {1} ", kvp.Key, kvp.Value));
                            if (kvp.Key.Equals(channel))
                            {
                                tresult = true;
                                Debug.Log(string.Format("kvp.Key {0}, kvp.Value {1} ", kvp.Key, kvp.Value));
                                Assert.Equals(0, kvp.Value);
                            }
                            if (kvp.Key.Equals(channel2))
                            {
                                tresult = true;
                                Debug.Log(string.Format("kvp.Key {0}, kvp.Value {1} ", kvp.Key, kvp.Value));
                                Assert.Equals(3, kvp.Value);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("(result.Channels == null) && !(result.Channels.Count.Equals(1))" + result.Channels.Count);
                    }
                }
            });
            yield return new WaitForSeconds(3);
            Assert.True(tresult, "MessageCounts test didnt return 2");
            pubnub.CleanUp();
        }
#endif
    }
}