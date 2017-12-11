using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using PubNubAPI;
using System.Collections.Generic;
using System;

namespace PubNubAPI.Tests
{
	public class PlayModeTests {
		#region "Time"
		[UnityTest]
		public IEnumerator TestTime() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);

			PubNub pubnub = new PubNub(pnConfiguration);
			bool testReturn = false;
			pubnub.Time ().Async ((result, status) => {
                Assert.True(!status.Error);
				Assert.True(!result.TimeToken.Equals(0));
				testReturn = true;
            });
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeForAsyncResponse);
			Assert.True(testReturn, "test didn't return");
		}
		#endregion

		#region "WhereNow"
		[UnityTest]
		public IEnumerator TestWhereNow() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			PubNub pubnub = new PubNub(pnConfiguration);
			string whereNowChannel = "UnityTestWhereNowChannel";
			pubnub.Subscribe ().SetChannels(new List<string> (){whereNowChannel}).WithPresence().Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			bool testReturn = false;
			pubnub.WhereNow ().Async ((result, status) => {
				Assert.True(!status.Error);
				if(result.Channels!=null){
					Assert.True(result.Channels.Contains(whereNowChannel));
				} else {
					Assert.Fail("result.Channels null");
				}
				testReturn = true;
             });
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeForAsyncResponse);
			Assert.True(testReturn, "test didn't return");
			pubnub.CleanUp();
		}
		#endregion

		#region "HereNow"
		[UnityTest]
		public IEnumerator TestHereNowChannel() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			pnConfiguration.UUID = "UnityTestHereNowUUID";
			PubNub pubnub = new PubNub(pnConfiguration);
			string hereNowChannel = "UnityTestHereNowChannel";
			List<string> channelList = new List<string>();
			channelList.Add(hereNowChannel);
			foreach(string ch in channelList){
				Debug.Log("ch0:" + ch);
			}

			pubnub.Subscribe ().SetChannels(channelList).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			bool testReturn = false;
			foreach(string ch in channelList){
				Debug.Log("ch:" + ch);
			}

			pubnub.HereNow().Channels(channelList).IncludeState(true).IncludeUUIDs(true).Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
                    Assert.True(!status.Error);
					//Assert.True(result.TotalOccupancy.Equals(1));
					MatchHereNowresult(result, channelList, pnConfiguration.UUID, false, false, 0, false, null);
                    testReturn = true;
                });

			yield return new WaitForSeconds (PlayModeCommon.WaitTimeForAsyncResponse);
			Assert.True(testReturn, "test didn't return");
			pubnub.CleanUp();
		}

		[UnityTest]
		public IEnumerator TestHereNowChannels() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			pnConfiguration.UUID = "UnityTestHereNowUUID";
			PubNub pubnub = new PubNub(pnConfiguration);
			string hereNowChannel = "UnityTestHereNowChannel1";
			string hereNowChannel2 = "UnityTestHereNowChannel2";
			List<string> channelList = new List<string>();
			channelList.Add(hereNowChannel);
			channelList.Add(hereNowChannel2);

			pubnub.Subscribe ().SetChannels(channelList).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			bool testReturn = false;
			pubnub.HereNow().Channels(channelList).IncludeState(true).IncludeUUIDs(true).Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
                    Assert.True(!status.Error);
					//Assert.True(result.TotalOccupancy.Equals(1));
					MatchHereNowresult(result, channelList, pnConfiguration.UUID, false, false, 0, false, null);
                    testReturn = true;
                });

			yield return new WaitForSeconds (PlayModeCommon.WaitTimeForAsyncResponse);
			Assert.True(testReturn, "test didn't return");
			pubnub.CleanUp();
		}

		[UnityTest]
		public IEnumerator TestHereNowChannelGroup() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			pnConfiguration.UUID = "UnityTestHereNowUUID";
			PubNub pubnub = new PubNub(pnConfiguration);
			string hereNowChannel = "UnityTestHereNowChannel";
			string channelGroup = "channelGroup1";
			List<string> channelList = new List<string>();
			channelList.Add(hereNowChannel);
			List<string> channelGroupList = new List<string>();
			channelGroupList.Add(channelGroup);

			pubnub.AddChannelsToChannelGroup().ChannelGroup(channelGroup).Channels(channelList).Async((result, status) => {
                Debug.Log ("in AddChannelsToChannelGroup");
            });
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);

			foreach(string ch in channelList){
				Debug.Log("ch0:" + ch);
			}

			pubnub.Subscribe ().SetChannelGroups(channelGroupList).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			bool testReturn = false;
			foreach(string ch in channelList){
				Debug.Log("ch:" + ch);
			}

			pubnub.HereNow().ChannelGroups(channelGroupList).IncludeState(true).IncludeUUIDs(true).Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
                    Assert.True(!status.Error);
					//Assert.True(result.TotalOccupancy.Equals(1));
					MatchHereNowresult(result, channelList, pnConfiguration.UUID, false, false, 0, false, null);
                    testReturn = true;
                });

			yield return new WaitForSeconds (PlayModeCommon.WaitTimeForAsyncResponse);
			Assert.True(testReturn, "test didn't return");
			pubnub.CleanUp();
		}

		[UnityTest]
		public IEnumerator TestHereNowChannelGroups() {
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
			pubnub.AddChannelsToChannelGroup().ChannelGroup(channelGroup).Channels(channelList).Async((result, status) => {
                Debug.Log ("in AddChannelsToChannelGroup");
            });
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);

			pubnub.Subscribe ().SetChannelGroups(channelGroupList).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			bool testReturn = false;
			pubnub.HereNow().ChannelGroups(channelGroupList).IncludeState(true).IncludeUUIDs(true).Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
                    Assert.True(!status.Error);
					//Assert.True(result.TotalOccupancy.Equals(1));
					MatchHereNowresult(result, channelList, pnConfiguration.UUID, false, false, 0, false, null);
                    testReturn = true;
                });

			yield return new WaitForSeconds (PlayModeCommon.WaitTimeForAsyncResponse);
			Assert.True(testReturn, "test didn't return");
			pubnub.CleanUp();
		}

		[UnityTest]
		public IEnumerator TestHereNowChannelsAndChannelGroups() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			pnConfiguration.UUID = "UnityTestHereNowUUID";
			PubNub pubnub = new PubNub(pnConfiguration);
			string hereNowChannel = "UnityTestHereNowChannel3";
			string hereNowChannel2 = "UnityTestHereNowChannel4";
			string hereNowChannel3 = "UnityTestHereNowChannel5";
			string channelGroup = "channelGroup3";
			List<string> channelList = new List<string>();
			channelList.Add(hereNowChannel);
			channelList.Add(hereNowChannel2);
			List<string> channelList2 = new List<string>();
			channelList2.Add(hereNowChannel3);
			List<string> channelGroupList = new List<string>();
			channelGroupList.Add(channelGroup);
			pubnub.AddChannelsToChannelGroup().ChannelGroup(channelGroup).Channels(channelList).Async((result, status) => {
                Debug.Log ("in AddChannelsToChannelGroup");
            });
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);

			pubnub.Subscribe ().SetChannels(channelList2).SetChannelGroups(channelGroupList).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			bool testReturn = false;
			pubnub.HereNow().Channels(channelList2).ChannelGroups(channelGroupList).IncludeState(true).IncludeUUIDs(true).Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
                    Assert.True(!status.Error);
					//Assert.True(result.TotalOccupancy.Equals(1));
					channelList.AddRange(channelList2);
					MatchHereNowresult(result, channelList, pnConfiguration.UUID, false, false, 0, false, null);
                    testReturn = true;
                });

			yield return new WaitForSeconds (PlayModeCommon.WaitTimeForAsyncResponse);
			Assert.True(testReturn, "test didn't return");
			pubnub.CleanUp();
		}

		[UnityTest]
		public IEnumerator TestGlobalHereNow() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			pnConfiguration.UUID = "UnityTestHereNowUUID";
			PubNub pubnub = new PubNub(pnConfiguration);
			string hereNowChannel = "UnityTestHereNowChannel6";
			string hereNowChannel2 = "UnityTestHereNowChannel7";
			string hereNowChannel3 = "UnityTestHereNowChannel8";
			string channelGroup = "channelGroup4";
			List<string> channelList = new List<string>();
			channelList.Add(hereNowChannel);
			channelList.Add(hereNowChannel2);
			List<string> channelList2 = new List<string>();
			channelList2.Add(hereNowChannel3);
			List<string> channelGroupList = new List<string>();
			channelGroupList.Add(channelGroup);
			pubnub.AddChannelsToChannelGroup().ChannelGroup(channelGroup).Channels(channelList).Async((result, status) => {
                Debug.Log ("in AddChannelsToChannelGroup");
            });
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);

			pubnub.Subscribe ().SetChannels(channelList2).SetChannelGroups(channelGroupList).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			bool testReturn = false;
			pubnub.HereNow().IncludeState(true).IncludeUUIDs(true).Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
                    Assert.True(!status.Error);
					//Assert.True(result.TotalOccupancy.Equals(1));
					channelList.AddRange(channelList2);
					MatchHereNowresult(result, channelList, pnConfiguration.UUID, false, false, 0, false, null);
                    testReturn = true;
                });

			yield return new WaitForSeconds (PlayModeCommon.WaitTimeForAsyncResponse);
			Assert.True(testReturn, "test didn't return");
			pubnub.CleanUp();
		}

		[UnityTest]
		public IEnumerator TestGlobalHereNowWithoutUUID() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			pnConfiguration.UUID = "UnityTestHereNowUUID";
			PubNub pubnub = new PubNub(pnConfiguration);
			string hereNowChannel = "UnityTestHereNowChannel9";
			string hereNowChannel2 = "UnityTestHereNowChannel10";
			string hereNowChannel3 = "UnityTestHereNowChannel11";
			string channelGroup = "channelGroup5";
			List<string> channelList = new List<string>();
			channelList.Add(hereNowChannel);
			channelList.Add(hereNowChannel2);
			List<string> channelList2 = new List<string>();
			channelList2.Add(hereNowChannel3);
			List<string> channelGroupList = new List<string>();
			channelGroupList.Add(channelGroup);
			pubnub.AddChannelsToChannelGroup().ChannelGroup(channelGroup).Channels(channelList).Async((result, status) => {
                Debug.Log ("in AddChannelsToChannelGroup");
            });
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);

			pubnub.Subscribe ().SetChannels(channelList2).SetChannelGroups(channelGroupList).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			bool testReturn = false;
			pubnub.HereNow().IncludeState(true).IncludeUUIDs(false).Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
                    Assert.True(!status.Error);
					//Assert.True(resultTotalOccupancy.Equals(1));
					channelList.AddRange(channelList2);
					MatchHereNowresult(result, channelList, pnConfiguration.UUID, false, true, 1, false, null);
                    testReturn = true;
                });

			yield return new WaitForSeconds (PlayModeCommon.WaitTimeForAsyncResponse);
			Assert.True(testReturn, "test didn't return");
			pubnub.CleanUp();
		}

		[UnityTest]
		public IEnumerator TestGlobalHereNowWithoutUUIDWithState() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			pnConfiguration.UUID = "UnityTestHereNowUUID";
			PubNub pubnub = new PubNub(pnConfiguration);
			string hereNowChannel = "UnityTestHereNowChannel12";
			string hereNowChannel2 = "UnityTestHereNowChannel13";
			string hereNowChannel3 = "UnityTestHereNowChannel14";
			string channelGroup = "channelGroup6";
			List<string> channelList = new List<string>();
			channelList.Add(hereNowChannel);
			channelList.Add(hereNowChannel2);
			List<string> channelList2 = new List<string>();
			channelList2.Add(hereNowChannel3);
			List<string> channelGroupList = new List<string>();
			channelGroupList.Add(channelGroup);
			pubnub.AddChannelsToChannelGroup().ChannelGroup(channelGroup).Channels(channelList).Async((result, status) => {
                Debug.Log ("in AddChannelsToChannelGroup");
            });
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);

			pubnub.Subscribe ().SetChannels(channelList2).SetChannelGroups(channelGroupList).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			Dictionary<string, object> state = new Dictionary<string, object>();
			state.Add("k", "v");
			pubnub.SetPresenceState().Channels(channelList).ChannelGroups(channelGroupList).State(state).Async ((result, status) => {
                
            });
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			bool testReturn = false;
			pubnub.HereNow().IncludeState(true).IncludeUUIDs(false).Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
                    Assert.True(!status.Error);
					//Assert.True(resultTotalOccupancy.Equals(1));
					channelList.AddRange(channelList2);
					MatchHereNowresult(result, channelList, pnConfiguration.UUID, false, true, 1, true, state);
                    testReturn = true;
                });

			yield return new WaitForSeconds (PlayModeCommon.WaitTimeForAsyncResponse);
			Assert.True(testReturn, "test didn't return");
			pubnub.CleanUp();
		}

		[UnityTest]
		public IEnumerator TestHereNowWithUUIDWithState() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			pnConfiguration.UUID = "UnityTestHereNowUUID";
			PubNub pubnub = new PubNub(pnConfiguration);
			string hereNowChannel = "UnityTestHereNowChannel15";
			string hereNowChannel2 = "UnityTestHereNowChannel16";
			string hereNowChannel3 = "UnityTestHereNowChannel17";
			string channelGroup = "channelGroup7";
			List<string> channelList = new List<string>();
			channelList.Add(hereNowChannel);
			channelList.Add(hereNowChannel2);
			List<string> channelList2 = new List<string>();
			channelList2.Add(hereNowChannel3);
			List<string> channelGroupList = new List<string>();
			channelGroupList.Add(channelGroup);
			pubnub.AddChannelsToChannelGroup().ChannelGroup(channelGroup).Channels(channelList).Async((result, status) => {
                Debug.Log ("in AddChannelsToChannelGroup");
            });
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);

			pubnub.Subscribe ().SetChannels(channelList2).SetChannelGroups(channelGroupList).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			Dictionary<string, object> state = new Dictionary<string, object>();
			state.Add("k", "v");
			pubnub.SetPresenceState().Channels(channelList).ChannelGroups(channelGroupList).State(state).Async ((result, status) => {
                
            });
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			bool testReturn = false;
			pubnub.HereNow().Channels(channelList2).ChannelGroups(channelGroupList).IncludeState(true).IncludeUUIDs(false).Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
                    Assert.True(!status.Error);
					//Assert.True(resultTotalOccupancy.Equals(1));
					channelList.AddRange(channelList2);
					MatchHereNowresult(result, channelList, pnConfiguration.UUID, false, true, 1, true, state);
                    testReturn = true;
                });

			yield return new WaitForSeconds (PlayModeCommon.WaitTimeForAsyncResponse);
			Assert.True(testReturn, "test didn't return");
			pubnub.CleanUp();
		}

		public void MatchHereNowresult(PNHereNowResult result, List<string> channelList, string uuid, bool checkOccupancy, bool checkOccupancyOnly, int occupancy, bool checkState, Dictionary<string, object> state){
			if(result.Channels!=null){
				Dictionary<string, PNHereNowChannelData> dict = result.Channels;
				PNHereNowChannelData pnHereNowChannelData;
				foreach(string hereNowChannel in channelList){
					if(dict.TryGetValue(hereNowChannel, out pnHereNowChannelData)){
						if(checkOccupancy || checkOccupancyOnly){
							Assert.True(pnHereNowChannelData.Occupancy.Equals(occupancy));
						} else if (!checkOccupancyOnly){
							bool found = false;
							foreach(PNHereNowOccupantData pnHereNowOccupantData in pnHereNowChannelData.Occupants){
								Debug.Log("finding:" + pnHereNowOccupantData.UUID);
								if(checkState){
									Assert.True(pnHereNowOccupantData.State.Equals(state));
								}
								
								if(pnHereNowOccupantData.UUID.Equals(uuid)){
									found = true;
									Debug.Log("found:" + pnHereNowOccupantData.UUID);
									break;
								} 
							}
							Assert.True(found);
						}
					}else {
						Assert.Fail("channel not found" + hereNowChannel);
					}
				}
				
			} else {
				Assert.Fail("Channels null");
			}
		}
		#endregion

		#region "Publish"
		[UnityTest]
		public IEnumerator TestPublishString() {
			string publishChannel = "UnityTestPublishChannel";
			string payload = string.Format("test message {0}", DateTime.Now.Ticks.ToString());
			yield return DoPublishTestProcsssing(payload, publishChannel);
		}

		[UnityTest]
		public IEnumerator TestPublishInt() {
			string publishChannel = "UnityTestPublishChannel";
			object payload = 1;
			yield return DoPublishTestProcsssing(payload, publishChannel);
		}

		[UnityTest]
		public IEnumerator TestPublishDouble() {
			string publishChannel = "UnityTestPublishChannel";
			double payload = 1.1;
			yield return DoPublishTestProcsssing(payload, publishChannel);
		}

		[UnityTest]
		public IEnumerator TestPublishDoubleArr() {
			string publishChannel = "UnityTestPublishChannel";
			double[] payload = {1.1};
			yield return DoPublishTestProcsssing(payload, publishChannel);
		}

		[UnityTest]
		public IEnumerator TestPublishEmptyArr() {
			string publishChannel = "UnityTestPublishChannel";
			object[] payload = {};
			yield return DoPublishTestProcsssing(payload, publishChannel);
		}

		[UnityTest]
		public IEnumerator TestPublishEmptyDict() {
			string publishChannel = "UnityTestPublishChannel";
			Dictionary<string, int> payload = new Dictionary<string, int>();
			yield return DoPublishTestProcsssing(payload, publishChannel);
		}

		[UnityTest]
		public IEnumerator TestPublishDict() {
			string publishChannel = "UnityTestPublishChannel";
			Dictionary<string, string> payload = new Dictionary<string, string>();
			payload.Add("cat", "test");
			yield return DoPublishTestProcsssing(payload, publishChannel);
		}

		[UnityTest]
		public IEnumerator TestPublishLong() {
			string publishChannel = "UnityTestPublishChannel";
			long payload = 14255515120803306;
			yield return DoPublishTestProcsssing(payload, publishChannel);
		}

		[UnityTest]
		public IEnumerator TestPublishLongArr() {
			string publishChannel = "UnityTestPublishChannel";
			long[] payload = {14255515120803306};
			yield return DoPublishTestProcsssing(payload, publishChannel);
		}

		[UnityTest]
		public IEnumerator TestPublishIntArr() {
			string publishChannel = "UnityTestPublishChannel";
			int[] payload = {13, 14};
			yield return DoPublishTestProcsssing(payload, publishChannel);
		}

		[UnityTest]
		public IEnumerator TestPublishStringArr() {
			string publishChannel = "UnityTestPublishChannel";
			string[] payload = {"testarr"};
			yield return DoPublishTestProcsssing(payload, publishChannel);
		}

		[UnityTest]
		public IEnumerator TestPublishComplexMessage() {
			string publishChannel = "UnityTestPublishChannel";
			object payload = new PubnubDemoObject ();
			yield return DoPublishTestProcsssing(payload, publishChannel);
		}

		[UnityTest]
		public IEnumerator TestJoin() {
			//string channelPres = "UnityTestJoinChannel-pnpres";
			string channel = "UnityTestJoinChannel";
			//yield return DoJoinTestProcsssing(channel, channelPres);
			yield return DoJoinTestProcsssing(channel);
		}

		/*[UnityTest]
		public IEnumerator TestConnected() {
			string channelPres = "UnityTestJoinChannel-pnpres";
			string channel = "UnityTestJoinChannel";
			yield return DoJoinTestProcsssing(channel, channelPres);
		}

		[UnityTest]
		public IEnumerator TestUnsubscribe() {
			string channelPres = "UnityTestJoinChannel-pnpres";
			string channel = "UnityTestJoinChannel";
			yield return DoJoinTestProcsssing(channel, channelPres);
		}*/

		public IEnumerator DoJoinTestProcsssing(string channel) {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			pnConfiguration.UUID = "UnityTestJoinUUID";
			PubNub pubnub = new PubNub(pnConfiguration);
			
			PNConfiguration pnConfiguration2 = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration2.UUID = "UnityTestJoinUUID_" + r.Next (100);

			List<string> channelList2 = new List<string>();
			channelList2.Add(channel);
			bool tresult = false;

			pubnub.SusbcribeCallback += (sender, e) => { 
				SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;
				if(!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
					Assert.True(mea.PresenceEventResult.Event.Equals("join") && mea.PresenceEventResult.UUID.Contains(pnConfiguration2.UUID));
					Assert.True(mea.PresenceEventResult.Occupancy > 0);
					Assert.True(mea.PresenceEventResult.Timestamp > 0);
					Debug.Log(mea.PresenceEventResult.UUID.Contains(pnConfiguration.UUID));
					tresult = true;
				} 
			};
			pubnub.Subscribe ().SetChannels(channelList2).WithPresence().Execute();
			//yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			
			PubNub pubnub2 = new PubNub(pnConfiguration2);

			pubnub2.Subscribe ().SetChannels(channelList2).Execute();
			yield return new WaitForSeconds (7);			
			
			Assert.True(tresult, "test didn't return");
			pubnub.CleanUp();
			pubnub2.CleanUp();
		}

		[UnityTest]
		public IEnumerator TestPublishLoadTest() {
			string publishChannel = "UnityTestPublishChannel";
			Dictionary<string, bool> payload = new Dictionary<string, bool>();
			for(int i=0; i<50; i++){
				payload.Add(string.Format("payload {0}", i), false);
			}
			
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			pnConfiguration.UUID = "UnityTestPublishLoadTestUUID";
			PubNub pubnub = new PubNub(pnConfiguration);
			List<string> channelList2 = new List<string>();
			channelList2.Add(publishChannel);
			//bool testReturn = false;
			
			pubnub.SusbcribeCallback += (sender, e) => { 
				SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;
				if(!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
					if(payload.ContainsKey(mea.MessageResult.Payload.ToString())){
						payload[mea.MessageResult.Payload.ToString()] = true;
					}
				}
			};
			pubnub.Subscribe ().SetChannels(channelList2).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			foreach(KeyValuePair<string, bool> kvp in payload){
				pubnub.Publish().Channel(publishChannel).Message(kvp.Key).Async((result, status) => {
					Assert.True(!result.Timetoken.Equals(0));
					Assert.True(status.Error.Equals(false));
					Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
				});
			}
			yield return new WaitForSeconds (20);
			bool tresult = false;
			foreach(KeyValuePair<string, bool> kvp in payload){
				if(!kvp.Value){
					Debug.Log("=======>>>>>>>>" + kvp.Key);
					tresult = true;
				}
			}

			Assert.True(!tresult);
			pubnub.CleanUp();
		}

		public IEnumerator DoPublishTestProcsssing(object payload, string publishChannel){
			Debug.Log("PAYLOAD:"+payload.ToString());
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			pnConfiguration.UUID = "UnityTestPublishUUID";
			PubNub pubnub = new PubNub(pnConfiguration);
			List<string> channelList2 = new List<string>();
			channelList2.Add(publishChannel);
			
			bool testReturn = false;
			
			
			pubnub.SusbcribeCallback += (sender, e) => { 
				SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;
				if(!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
					Debug.Log("PAYLOAD2:" + payload.ToString() + payload.GetType());		
					
					Assert.True(mea.MessageResult.Channel.Equals(publishChannel));
					if(payload.GetType().Equals(typeof(Int64))){
						long expected; 
						if(Int64.TryParse(payload.ToString(), out expected)){
							long response;
							if(Int64.TryParse(mea.MessageResult.Payload.ToString(), out response)){
								Assert.True(expected.Equals(response));
							} else {
								Assert.Fail("response long conversion failed");	
							}
						} else {
							Assert.Fail("expectedlong conversion failed");
						}
					} else if(payload.GetType().Equals(typeof(Int64[]))){
						Debug.Log(mea.MessageResult.Payload.GetType());
						Debug.Log(mea.MessageResult.Payload.GetType().Equals(typeof(string[])));
						Int64[] expected = (Int64[])payload;
						string[] response = (string[])mea.MessageResult.Payload;
						foreach(Int64 iExp in expected){
							bool found = false;
							foreach(string iResp in response){
								if(iExp.ToString().Equals(iResp)){
									found = true;
									break;
								}
							}
							if(!found){
								Assert.Fail("response not found");	
							}
						}
					} else if(payload.GetType().Equals(typeof(double[]))){
						Debug.Log(mea.MessageResult.Payload.GetType());
						Debug.Log(mea.MessageResult.Payload.GetType().Equals(typeof(double[])));
						double[] expected = (double[])payload;
						double[] response = (double[])mea.MessageResult.Payload;
						foreach(double iExp in expected){
							Debug.Log(iExp.ToString());
							bool found = false;
							foreach(double iResp in response){
								Debug.Log(iResp.ToString());
								if(iExp.Equals(iResp)){
									found = true;
									break;
								}
							}
							if(!found){
								Assert.Fail("response not found");	
							}
						}	
					}else if(payload.GetType().Equals(typeof(string[]))){
						string[] expected = (string[])payload;
						string[] response = (string[])mea.MessageResult.Payload;
						foreach(string strExp in expected){
							bool found = false;
							foreach(string strResp in response){
								if(strExp.Equals(strResp)){
									found = true;
									break;
								}
							}
							if(!found){
								Assert.Fail("response not found");	
							}
						}	
					} else if(payload.GetType().Equals(typeof(System.Object[]))){
						System.Object[] expected = (System.Object[])payload;
						System.Object[] response = (System.Object[])mea.MessageResult.Payload;
						// + payload.GetType().Equals(typeof(System.Object[])) + expected[0].Equals(response[0]));
						Assert.True(expected.Length.Equals(response.Length) && expected.Length.Equals(0));
					} else if(payload.GetType().Equals(typeof(Dictionary<string, string>))){
						Dictionary<string, string> expected = (Dictionary<string, string>)payload;
						IDictionary response = mea.MessageResult.Payload as IDictionary;
						Debug.Log("PAYLOAD2:" + payload.ToString() + payload.GetType());
						Assert.True(response["cat"].Equals("test"));
					} else if(payload.GetType().Equals(typeof(Dictionary<string, int>))){
						Dictionary<string, int> expected = (Dictionary<string, int>)payload;
						IDictionary response = mea.MessageResult.Payload as IDictionary;
						Debug.Log("PAYLOAD2:" + payload.ToString() + payload.GetType());
						Assert.True( response == null || response.Count < 1 );
						//Assert.True(expected.Count.Equals(response.Count) && expected.Count.Equals(0));

					} else if(payload.GetType().Equals(typeof(Int32[]))){
						Int32[] expected = (Int32[])payload;
						Int32[] response = (Int32[])mea.MessageResult.Payload;
						foreach(int iExp in expected){
							bool found = false;
							foreach(int iResp in response){
								if(iExp.Equals(iResp)){
									found = true;
									break;
								}
							}
							if(!found){
								Assert.Fail("response not found");	
							}
						}
					} else if(payload.GetType().ToString().Contains(typeof(PubnubDemoObject).ToString())){	
						Debug.Log("PAYLOAD2 PubnubDemoObject:" + payload.ToString() + payload.GetType());
						PubnubDemoObject expected = payload as PubnubDemoObject;
						Debug.Log(mea.MessageResult.Payload==null);
						Debug.Log(mea.MessageResult.Payload.GetType());
						
						Dictionary<string, object> resp = (Dictionary<string, object>)mea.MessageResult.Payload;
						Debug.Log(resp==null);
						//Debug.Log(resp[]);
						PubnubDemoObject response = new PubnubDemoObject();
						Type responseType = resp.GetType();
						foreach (KeyValuePair<string, object> item in resp)
						{
							Debug.Log(item.Key);
							Debug.Log(item.Value);
							Debug.Log(responseType==null);
							Debug.Log(response==null);
							//Debug.Log(responseType.GetProperty(item.Key));
							switch (item.Key){
								case "VersionID":
									response.VersionID = (double)item.Value;
									break;
								case "Timetoken":
									Int64 res;
									if(Int64.TryParse(item.Value.ToString(), out res)){
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
						
						Assert.True(response.VersionID.Equals(expected.VersionID));
						Assert.True(response.Timetoken.Equals(expected.Timetoken));
						Assert.True(response.OperationName.Equals(expected.OperationName));
						Assert.True(response.DemoMessage.DefaultMessage.Equals(expected.DemoMessage.DefaultMessage));
						Assert.True(response.CustomMessage.DefaultMessage.Equals(expected.CustomMessage.DefaultMessage));
					} else {
						Debug.Log("PAYLOAD2:" + payload.ToString() + payload.GetType());
						Assert.True(mea.MessageResult.Payload.Equals(payload));
					}
					testReturn = true;
				}
			};
			pubnub.Subscribe ().SetChannels(channelList2).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			
			pubnub.Publish().Channel(publishChannel).Message(payload).Async((result, status) => {
				Assert.True(!result.Timetoken.Equals(0));
				Assert.True(status.Error.Equals(false));
				Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			Assert.True(testReturn, "test didn't return");
			pubnub.CleanUp();
		}
		#endregion
	}
}
