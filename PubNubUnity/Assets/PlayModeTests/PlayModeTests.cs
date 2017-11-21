using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using PubNubAPI;
using System.Collections.Generic;

namespace PubNubAPI.Tests
{
	public class PlayModeTests {
		[UnityTest]
		public IEnumerator TestTime() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig();

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

		[UnityTest]
		public IEnumerator TestWhereNow() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig();
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

		[UnityTest]
		public IEnumerator TestHereNowChannel() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig();
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
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig();
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
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig();
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
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig();
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
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig();
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
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig();
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
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig();
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
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig();
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
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig();
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
	}
}
