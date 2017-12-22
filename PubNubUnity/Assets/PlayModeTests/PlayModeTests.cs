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
		public IEnumerator TestJoinLeave() {
			string channel = "UnityTestJoinChannel";
			yield return DoJoinLeaveTestProcsssing(channel);
		}

		[UnityTest]
		public IEnumerator TestConnected() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next (100);
			string channel = "UnityTestConnectedChannel";

			PubNub pubnub = new PubNub(pnConfiguration);
			List<string> channelList2 = new List<string>();
			channelList2.Add(channel);
			bool tresult = false;

			pubnub.SusbcribeCallback += (sender, e) => { 
				SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;
				if(mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
					Assert.True(mea.Status.UUID.Contains(pnConfiguration.UUID));
					tresult = true;
				} 
			};
			pubnub.Subscribe ().SetChannels(channelList2).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
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

			pubnub.SusbcribeCallback += (sender, e) => { 
				SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;
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
		public IEnumerator TestAlreadySubscribed() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next (100);
			string channel = "UnityTestConnectedChannel";

			PubNub pubnub = new PubNub(pnConfiguration);
			List<string> channelList2 = new List<string>();
			channelList2.Add(channel);
			bool tresult = false;

			pubnub.SusbcribeCallback += (sender, e) => { 
				SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;
				if(mea.Status.Category.Equals(PNStatusCategory.PNUnknownCategory)){
					Assert.True(mea.Status.Error);
					if(mea.Status.ErrorData!=null){
						Debug.Log(mea.Status.ErrorData.Info);
						Assert.True(mea.Status.ErrorData.Info.Contains("Duplicate Channels or Channel Groups"));
					}
					
					tresult = true;
				} 
			};
			pubnub.Subscribe ().SetChannels(channelList2).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			pubnub.Subscribe ().SetChannels(channelList2).Execute();
			Assert.True(tresult, "test didn't return");
			pubnub.CleanUp();

		}

		public IEnumerator DoJoinLeaveTestProcsssing(string channel) {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			pnConfiguration.UUID = "UnityTestJoinUUID";
			PubNub pubnub = new PubNub(pnConfiguration);
			
			PNConfiguration pnConfiguration2 = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration2.UUID = "UnityTestJoinUUID_" + r.Next (100);

			List<string> channelList2 = new List<string>();
			channelList2.Add(channel);
			bool tJoinResult = false;
			bool tLeaveResult = false;

			PubNub pubnub2 = new PubNub(pnConfiguration2);

			pubnub.SusbcribeCallback += (sender, e) => { 
				SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;
				if(!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
					if(mea.PresenceEventResult.Event.Equals("join")){
						Assert.True(mea.PresenceEventResult.UUID.Contains(pnConfiguration2.UUID));
						Assert.True(mea.PresenceEventResult.Occupancy > 0);
						Assert.True(mea.PresenceEventResult.Timestamp > 0);
						Debug.Log(mea.PresenceEventResult.UUID.Contains(pnConfiguration.UUID));
						tJoinResult = true;
						pubnub2.Unsubscribe().Channels(channelList2).Async((result, status) => {
							Debug.Log("status.Error:" + status.Error);
							Assert.True(!status.Error);
						});
						
					} else if (mea.PresenceEventResult.Event.Equals("leave")){
						Assert.True(mea.PresenceEventResult.UUID.Contains(pnConfiguration2.UUID));
						Assert.True(mea.PresenceEventResult.Timestamp > 0);
						Debug.Log(mea.PresenceEventResult.UUID.Contains(pnConfiguration.UUID));
						tLeaveResult = true;
					}					
				} 
			};
			pubnub.Subscribe ().SetChannels(channelList2).WithPresence().Execute();
			//yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			
			pubnub2.Subscribe ().SetChannels(channelList2).Execute();
			//yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			yield return new WaitForSeconds (7);			
			
			Assert.True(tJoinResult, "join test didn't return");
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tLeaveResult, "leave test didn't return");
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

		[UnityTest]
		public IEnumerator TestSubscribeWithTT() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next (100);
			string channel = "UnityTestWithTTLChannel";
			string payload = string.Format("payload {0}", pnConfiguration.UUID);

			PubNub pubnub = new PubNub(pnConfiguration);
			long timetoken = 0;
			pubnub.Time().Async((result, status) => {
				timetoken = result.TimeToken;
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			Assert.True(!timetoken.Equals(0));

			pubnub.Publish().Channel(channel).Message(payload).Async((result, status) => {
				Assert.True(!result.Timetoken.Equals(0));
				Assert.True(status.Error.Equals(false));
				Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
			});

			List<string> channelList2 = new List<string>();
			channelList2.Add(channel);
			bool tresult = false;

			pubnub.SusbcribeCallback += (sender, e) => { 
				SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;
				if(!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
					Assert.True(mea.MessageResult.Channel.Equals(channel));
					Assert.True(mea.MessageResult.Payload.ToString().Equals(payload));

					tresult = true;
				} 
			};
			pubnub.Subscribe ().SetChannels(channelList2).SetTimeToken(timetoken).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return");
			pubnub.CleanUp();

		}

		[UnityTest]
		public IEnumerator TestCG() {
			
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration.UUID = "UnityTestCGUUID_" + r.Next (100);
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
			

			pubnub.AddChannelsToChannelGroup().Channels(channelList).ChannelGroup(channelGroup).Async((result, status) => {
					Debug.Log ("in AddChannelsToChannelGroup " + status.Error);
					if(!status.Error){
						Debug.Log(result.Message);
						Assert.IsTrue(result.Message.Contains("OK"));
					} else {
						Assert.Fail("AddChannelsToChannelGroup failed");
					}
					tresult = true;
				});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return1");
			tresult = false;

			pubnub.ListChannelsForChannelGroup().ChannelGroup(channelGroup).Async((result, status) => {
					if(!status.Error){
						if(result.Channels!=null){
							Assert.IsTrue(result.Channels.Contains(channel));
							Assert.IsTrue(result.Channels.Contains(channel2));
						} else {
							Assert.Fail("result.Channels empty");
						}
					} else {
						Assert.Fail("AddChannelsToChannelGroup failed");
					}
					tresult = true;
				});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return2");
			tresult = false;
			string payload = string.Format("payload {0}", pnConfiguration.UUID);

			pubnub.SusbcribeCallback += (sender, e) => { 
				SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;
				if(!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
					/*Assert.IsTrue(mea.Status.AffectedChannelGroups.Contains(channelGroup));
					tresult = true;
				} else {*/
					if(mea.MessageResult!=null){
						Debug.Log("SusbcribeCallback" + mea.MessageResult.Subscription);
						Debug.Log("SusbcribeCallback" + mea.MessageResult.Channel);
						Debug.Log("SusbcribeCallback" + mea.MessageResult.Payload);
						Debug.Log("SusbcribeCallback" + mea.MessageResult.Timetoken);
						Assert.True(mea.MessageResult.Channel.Equals(channel));
						Assert.True(mea.MessageResult.Subscription.Equals(channelGroup));
						Assert.True(mea.MessageResult.Payload.ToString().Equals(payload));
						tresult = true;
					}
				}
				
			};
			
			pubnub.Subscribe ().SetChannelGroups(channelGroupList).Execute();
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
            pubnub.SetPresenceState().ChannelGroups(channelGroupList).State(state).Async ((result, status) => {
                if(status.Error){
					Assert.Fail("SetPresenceState failed");
                } else {
                    if(result != null){
						if(result.StateByChannels!= null){
							foreach (KeyValuePair<string, object> key in result.StateByChannels){
								Debug.Log("key.Key" + key.Key);
								if(key.Key.Equals(channelGroup)){
									Debug.Log("pubnub.JsonLibrary.SerializeToJsonString(key.Value)" + pubnub.JsonLibrary.SerializeToJsonString(key.Value));
									Assert.IsTrue(pubnub.JsonLibrary.SerializeToJsonString(key.Value).Equals("{\"k1\":\"v1\"}"));
									tresult = true;
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
										Debug.Log("pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value)" + pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value));
										Assert.IsTrue(pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value).Equals("{\"cg\":{\"k1\":\"v1\"}}"));
										found1 = true;
									}
									if(keyInStateDict.Key.Equals(channel2)){
										Debug.Log("pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value)" + pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value));
										Assert.IsTrue(pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value).Equals("{\"cg\":{\"k1\":\"v1\"}}"));
										found2 = true;
									}
								}
							}
							Assert.IsTrue(found1);
							Assert.IsTrue(found2);
							tresult = true;
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
									Debug.Log("pubnub.JsonLibrary.SerializeToJsonString(key.Value)" + pubnub.JsonLibrary.SerializeToJsonString(key.Value));
									found = pubnub.JsonLibrary.SerializeToJsonString(key.Value).Equals("{\"k1\":\"v1\"}");
									break;
								}
							}
							Assert.IsFalse(found);
							tresult = true;
						}
                    }
                }
            });

			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
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
										Debug.Log("pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value)" + pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value));
										Assert.IsFalse(pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value).Equals("{\"cg\":{\"k1\":\"v1\"}}"));
										found1 = true;
									}
									if(keyInStateDict.Key.Equals(channel2)){
										Debug.Log("pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value)" + pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value));
										Assert.IsFalse(pubnub.JsonLibrary.SerializeToJsonString(keyInStateDict.Value).Equals("{\"cg\":{\"k1\":\"v1\"}}"));
										found2 = true;
									}
								}
							}
							Assert.IsTrue(found1);
							Assert.IsTrue(found2);
							tresult = true;
						}
                    }
                }
            });

			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			Assert.True(tresult, "test didn't return 7");

		}

		[UnityTest]
		public IEnumerator TestCGRemove() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration.UUID = "UnityTestCGUUID_" + r.Next (100);
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
			

			pubnub.AddChannelsToChannelGroup().Channels(channelList).ChannelGroup(channelGroup).Async((result, status) => {
					Debug.Log ("in AddChannelsToChannelGroup " + status.Error);
					if(!status.Error){
						Debug.Log(result.Message);
						Assert.IsTrue(result.Message.Contains("OK"));
					} else {
						Assert.Fail("AddChannelsToChannelGroup failed");
					}
					tresult = true;
				});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return1");
			tresult = false;

			pubnub.ListChannelsForChannelGroup().ChannelGroup(channelGroup).Async((result, status) => {
					if(!status.Error){
						if(result.Channels!=null){
							Assert.IsTrue(result.Channels.Contains(channel));
							Assert.IsTrue(result.Channels.Contains(channel2));
						} else {
							Assert.Fail("result.Channels empty");
						}
					} else {
						Assert.Fail("AddChannelsToChannelGroup failed");
					}
					tresult = true;
				});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return2");
			tresult = false;

			List<string> listChannelsRemove = new List<string>{channel};
			listChannelsRemove.Add(channel);
			pubnub.RemoveChannelsFromChannelGroup().Channels(listChannelsRemove).ChannelGroup(channelGroup).Async((result, status) => {
                    Debug.Log ("in RemoveChannelsFromCG");
                    if(!status.Error){
						
                        Assert.IsTrue(result.Message.Equals("OK"));
                    }
					tresult = true;

                });
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return 8");

			tresult = false;
			pubnub.ListChannelsForChannelGroup().ChannelGroup(channelGroup).Async((result, status) => {
					if(!status.Error){
						if(result.Channels!=null){
							Assert.IsTrue(!result.Channels.Contains(channel));
							Assert.IsTrue(result.Channels.Contains(channel2));
							
						} else {
							Assert.Fail("result.Channels empty");
						}
					} else {
						Assert.Fail("AddChannelsToChannelGroup failed");
					}
					tresult = true;
					
				});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return 9");

			tresult = false;
			pubnub.DeleteChannelGroup().ChannelGroup(channelGroup).Async((result, status) => {
                    if(!status.Error){
						
                        Assert.IsTrue(result.Message.Equals("OK"));
                    }
					tresult = true;
                });
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return 10");
			
			pubnub.CleanUp();

		}

		[UnityTest]
		public IEnumerator TestPush() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration.UUID = "UnityTestCGUUID_" + r.Next (100);
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

			string deviceId = "UnityTestDeviceId";
			PNPushType pnPushType = PNPushType.GCM;

			pubnub.AddPushNotificationsOnChannels().Channels(channelList).DeviceIDForPush(deviceId).PushType(pnPushType).Async((result, status) => {
                    Debug.Log ("in AddChannelsToChannelGroup " + status.Error);
                    if(!status.Error){
						Debug.Log(result.Message);
						Assert.IsTrue(result.Message.Contains("Modified Ch"));
					} else {
						Assert.Fail("AddPushNotificationsOnChannels failed");
					}
					tresult = true;
                });
						
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return1");
			tresult = false;

			pubnub.AuditPushChannelProvisions().DeviceIDForPush(deviceId).PushType(pnPushType).Async((result, status) => {
                    if(!status.Error){
						if(result.Channels!=null){
							Assert.IsTrue(result.Channels.Contains(channel));
							Assert.IsTrue(result.Channels.Contains(channel2));							
						} else {
							Assert.Fail("result.Channels empty");
						}
					} else {
						Assert.Fail("AddChannelsToChannelGroup failed");
					}
					tresult = true;
                });

			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return2");
			tresult = false;

			List<string> listChannelsRemove = new List<string>{channel};
			listChannelsRemove.Add(channel);
			pubnub.RemovePushNotificationsFromChannels().Channels(listChannelsRemove).DeviceIDForPush(deviceId).PushType(pnPushType).Async((result, status) => {
                    Debug.Log ("in RemovePushNotificationsFromChannels");
					if(!status.Error){
						
                        Assert.IsTrue(result.Message.Equals("Modified Channels"));
                    }
					tresult = true;
                });

			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return 8");

			tresult = false;
			pubnub.AuditPushChannelProvisions().DeviceIDForPush(deviceId).PushType(pnPushType).Async((result, status) => {
                    if(!status.Error){
						if(result.Channels!=null){
							Assert.IsTrue(!result.Channels.Contains(channel));
							Assert.IsTrue(result.Channels.Contains(channel2));							
						} else {
							Assert.Fail("result.Channels empty");
						}
					} else {
						Assert.Fail("AddChannelsToChannelGroup failed");
					}
					tresult = true;
                });
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			
			Assert.True(tresult, "test didn't return 9");

			tresult = false;
			pubnub.RemoveAllPushNotifications().DeviceIDForPush(deviceId).PushType(pnPushType).Async((result, status) => {
                    if(!status.Error){
						
                        Assert.IsTrue(result.Message.Equals("Removed Device"));
                    }
					tresult = true;
                });
			
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return 10");
			
			pubnub.CleanUp();

		}

		[UnityTest]
		public IEnumerator TestPublishWithMeta() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next (100);
			string channel = "UnityTestWithMetaChannel";
			string payload = string.Format("payload {0}", pnConfiguration.UUID);

			pnConfiguration.FilterExpression = "region=='east'";
			PubNub pubnub = new PubNub(pnConfiguration);

			List<string> channelList2 = new List<string>();
			channelList2.Add(channel);
			bool tresult = false;

			pubnub.SusbcribeCallback += (sender, e) => { 
				SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;
				if(!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
					Assert.True(mea.MessageResult.Channel.Equals(channel));
					Assert.True(mea.MessageResult.Payload.ToString().Equals(payload));

					tresult = true;
				} 
			};
			pubnub.Subscribe ().SetChannels(channelList2).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);

			Dictionary<string, string> metaDict = new Dictionary<string, string>();
            metaDict.Add("region", "east");

			pubnub.Publish().Channel(channel).Meta(metaDict).Message(payload).Async((result, status) => {
				Assert.True(!result.Timetoken.Equals(0));
				Assert.True(status.Error.Equals(false));
				Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return");
			pubnub.CleanUp();
		}

		[UnityTest]
		public IEnumerator TestPublishWithMetaNeg() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next (100);
			string channel = "UnityTestWithMetaNegChannel";
			string payload = string.Format("payload {0}", pnConfiguration.UUID);

			pnConfiguration.FilterExpression = "region=='east'";
			PubNub pubnub = new PubNub(pnConfiguration);

			List<string> channelList2 = new List<string>();
			channelList2.Add(channel);
			bool tresult = false;

			pubnub.SusbcribeCallback += (sender, e) => { 
				SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;
				if(!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
					Assert.True(mea.MessageResult.Channel.Equals(channel));
					Assert.True(mea.MessageResult.Payload.ToString().Equals(payload));

					tresult = true;
				} 
			};
			pubnub.Subscribe ().SetChannels(channelList2).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);

			Dictionary<string, string> metaDict = new Dictionary<string, string>();
            metaDict.Add("region", "east1");

			pubnub.Publish().Channel(channel).Meta(metaDict).Message(payload).Async((result, status) => {
				Assert.True(!result.Timetoken.Equals(0));
				Assert.True(status.Error.Equals(false));
				Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(!tresult, "subscribe returned");
			pubnub.CleanUp();
		}

		[UnityTest]
		public IEnumerator TestPublishAndHistory() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next (100);
			string channel = "UnityTestNoStoreChannel";
			string payload = string.Format("payload no store {0}", pnConfiguration.UUID);

			PubNub pubnub = new PubNub(pnConfiguration);

			List<string> channelList2 = new List<string>();
			channelList2.Add(channel);
			bool tresult = false;

			pubnub.Publish().Channel(channel).Message(payload).Async((result, status) => {
				Assert.True(!result.Timetoken.Equals(0));
				Assert.True(status.Error.Equals(false));
				Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
				tresult = true;
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didnt return 1");

			tresult = false;
			pubnub.History().Channel(channel).Count(1).Async((result, status) => {
				Assert.True(status.Error.Equals(false));
				if(!status.Error){

					if((result.Messages!=null) && (result.Messages.Count>0)){
						PNHistoryItemResult pnHistoryItemResult = result.Messages[0] as PNHistoryItemResult;
						Debug.Log("result.Messages[0]" + result.Messages[0].ToString());
						if(pnHistoryItemResult != null){
							Assert.IsTrue(pnHistoryItemResult.Entry.ToString().Contains(payload));
							tresult = true;
						} else {
							tresult = false;
						}						
					} else {
						tresult = false;
					}
					
                }
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didnt return 2");
			
			pubnub.CleanUp();
		}

		[UnityTest]
		public IEnumerator TestPublishNoStore() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next (100);
			string channel = "UnityTestNoStoreChannel";
			string payload = string.Format("payload no store {0}", pnConfiguration.UUID);

			PubNub pubnub = new PubNub(pnConfiguration);

			List<string> channelList2 = new List<string>();
			channelList2.Add(channel);
			bool tresult = false;

			pubnub.Publish().Channel(channel).Message(payload).ShouldStore(false).Async((result, status) => {
				Assert.True(!result.Timetoken.Equals(0));
				Assert.True(status.Error.Equals(false));
				Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
				tresult = true;
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didnt return 1");

			tresult = false;
			pubnub.History().Channel(channel).Count(1).Async((result, status) => {
				Assert.True(status.Error.Equals(false));
				if(!status.Error){

					if((result.Messages!=null) && (result.Messages.Count>0)){
						PNHistoryItemResult pnHistoryItemResult = result.Messages[0] as PNHistoryItemResult;
						Debug.Log("result.Messages[0]" + result.Messages[0].ToString());
						if(pnHistoryItemResult != null){
							Assert.IsTrue(!pnHistoryItemResult.Entry.ToString().Contains(payload));
							tresult = true;
						} else {
							tresult = false;
						}						
					} else {
						tresult = false;
					}
					
                }
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didnt return 2");
			
			pubnub.CleanUp();
		}

		[UnityTest]
		public IEnumerator TestPublishKeyPresent() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration.UUID = "UnityTestPublishKeyPresentUUID_" + r.Next (100);
			string channel = "UnityPublishKeyPresentChannel";
			string payload = string.Format("payload {0}", pnConfiguration.UUID);

			pnConfiguration.PublishKey = "";
			PubNub pubnub = new PubNub(pnConfiguration);

			bool tresult = false;

			pubnub.Publish().Channel(channel).Message(payload).Async((result, status) => {
				Debug.Log("Publish" + status.Error + status.StatusCode );
				Assert.True(status.Error.Equals(true));
				Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
				tresult = true;
			});

			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return 10");
			
			pubnub.CleanUp();
		}

		[UnityTest]
		public IEnumerator TestNullAsEmptyOnpublish() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration.UUID = "UnityTestPublishKeyPresentUUID_" + r.Next (100);
			string channel = "UnityPublishKeyPresentChannel";

			PubNub pubnub = new PubNub(pnConfiguration);

			bool tresult = false;

			pubnub.Publish().Channel(channel).Message(null).Async((result, status) => {
				Debug.Log("Publish" + status.Error + status.StatusCode );
				Assert.True(status.Error.Equals(true));
				Assert.True(status.StatusCode.Equals(0), status.StatusCode.ToString());
				tresult = true;
			});

			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return 10");
			
			pubnub.CleanUp();
		}
		
		#endregion
	}
}
