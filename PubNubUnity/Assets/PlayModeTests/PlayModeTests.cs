using UnityEngine;
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
	public class PlayModeTests {
		#if !UNITY_WSA_10_0
		#region "Time"
		[UnityTest]
		public IEnumerator TestTime() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);

			PubNub pubnub = new PubNub(pnConfiguration);
			bool testReturn = false;
			pubnub.Time ().Async ((result, status) => {
				bool statusError = status.Error;
				Debug.Log(statusError);
				bool resultTimeToken = result.TimeToken.Equals(0);
				Debug.Log(resultTimeToken);
				testReturn =  !statusError && !resultTimeToken;
            });
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeForAsyncResponse);
			Assert.True(testReturn, "test didn't return");
			pubnub.CleanUp();
		}
		#endregion

		#region "WhereNow"
		[UnityTest]
		public IEnumerator TestWhereNow() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			PubNub pubnub = new PubNub(pnConfiguration);
			string whereNowChannel = "UnityTestWhereNowChannel";
			pubnub.Subscribe ().Channels(new List<string> (){whereNowChannel}).WithPresence().Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			bool testReturn = false;
			pubnub.WhereNow ().Async ((result, status) => {
				bool statusError = status.Error;
				Debug.Log("statusError:" + statusError);

				if(result.Channels!=null){
					Debug.Log(result.Channels.Contains(whereNowChannel));
					testReturn = !statusError && result.Channels.Contains(whereNowChannel);
				} else {
					Assert.Fail("result.Channels null");
				}
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

			pubnub.Subscribe ().Channels(channelList).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			bool testReturn = false;
			foreach(string ch in channelList){
				Debug.Log("ch:" + ch);
			}

			pubnub.HereNow().Channels(channelList).IncludeState(true).IncludeUUIDs(true).Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
					bool matchResult = MatchHereNowresult(pubnub, result, channelList, pnConfiguration.UUID, false, false, true, 0, false, null);
					testReturn = !status.Error && matchResult;
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

			pubnub.Subscribe ().Channels(channelList).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			bool testReturn = false;
			pubnub.HereNow().Channels(channelList).IncludeState(true).IncludeUUIDs(true).Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
                    Assert.True(!status.Error);
					bool matchResult = MatchHereNowresult(pubnub, result, channelList, pnConfiguration.UUID, false, false, true, 0, false, null);
                    testReturn = !status.Error && matchResult;
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

			pubnub.Subscribe ().ChannelGroups(channelGroupList).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			bool testReturn = false;
			foreach(string ch in channelList){
				Debug.Log("ch:" + ch);
			}

			pubnub.HereNow().ChannelGroups(channelGroupList).IncludeState(true).IncludeUUIDs(true).Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
                    Assert.True(!status.Error);
					//Assert.True(result.TotalOccupancy.Equals(1));
					bool matchResult = MatchHereNowresult(pubnub, result, channelList, pnConfiguration.UUID, false, false, true, 0, false, null);
					testReturn = !status.Error && matchResult;
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

			pubnub.Subscribe ().ChannelGroups(channelGroupList).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			bool testReturn = false;
			pubnub.HereNow().ChannelGroups(channelGroupList).IncludeState(true).IncludeUUIDs(true).Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
                    Assert.True(!status.Error);
					//Assert.True(result.TotalOccupancy.Equals(1));
					bool matchResult = MatchHereNowresult(pubnub, result, channelList, pnConfiguration.UUID, false, false, true, 0, false, null);
                    testReturn = !status.Error && matchResult;
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

			pubnub.Subscribe ().Channels(channelList2).ChannelGroups(channelGroupList).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			bool testReturn = false;
			pubnub.HereNow().Channels(channelList2).ChannelGroups(channelGroupList).IncludeState(true).IncludeUUIDs(true).Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
                    Assert.True(!status.Error);
					//Assert.True(result.TotalOccupancy.Equals(1));
					channelList.AddRange(channelList2);
					bool matchResult = MatchHereNowresult(pubnub, result, channelList, pnConfiguration.UUID, false, false, true, 0, false, null);
                    testReturn = !status.Error && matchResult;
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

			pubnub.Subscribe ().Channels(channelList2).ChannelGroups(channelGroupList).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			bool testReturn = false;
			pubnub.HereNow().IncludeState(true).IncludeUUIDs(true).Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
                    Assert.True(!status.Error);
					//Assert.True(result.TotalOccupancy.Equals(1));
					channelList.AddRange(channelList2);
					bool matchResult = MatchHereNowresult(pubnub, result, channelList, pnConfiguration.UUID, false, false, true, 0, false, null);
					
                    testReturn = !status.Error && matchResult;
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

			pubnub.Subscribe ().Channels(channelList2).ChannelGroups(channelGroupList).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			bool testReturn = false;
			pubnub.HereNow().IncludeState(true).IncludeUUIDs(false).Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
                    Assert.True(!status.Error);
					//Assert.True(resultTotalOccupancy.Equals(1));
					channelList.AddRange(channelList2);
					bool matchResult = MatchHereNowresult(pubnub, result, channelList, pnConfiguration.UUID, false, true, false, 1, false, null);
                    testReturn = !status.Error && matchResult;
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

			pubnub.Subscribe ().Channels(channelList2).ChannelGroups(channelGroupList).Execute();
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
					bool matchResult = MatchHereNowresult(pubnub, result, channelList, pnConfiguration.UUID, false, true, false, 1, true, state);
					testReturn = !status.Error && matchResult;
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

			pubnub.Subscribe ().Channels(channelList2).ChannelGroups(channelGroupList).Execute();
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
					bool matchResult = MatchHereNowresult(pubnub, result, channelList, pnConfiguration.UUID, false, true, false, 1, true, state);
                    testReturn = !status.Error && matchResult;
                });

			yield return new WaitForSeconds (PlayModeCommon.WaitTimeForAsyncResponse);
			Assert.True(testReturn, "test didn't return");
			pubnub.CleanUp();
		}

		public bool MatchHereNowresult(PubNub pubnub, PNHereNowResult result, List<string> channelList, string uuid, bool checkOccupancy, bool checkOccupancyOnly, bool checkOccupantData, int occupancy, bool checkState, Dictionary<string, object> state){
			bool matchResult = false;
			if(result.Channels!=null){
				Dictionary<string, PNHereNowChannelData> dict = result.Channels;
				PNHereNowChannelData pnHereNowChannelData;
				
				foreach(string hereNowChannel in channelList){
					if(dict.TryGetValue(hereNowChannel, out pnHereNowChannelData)){
						if(checkOccupancy || checkOccupancyOnly){
							matchResult = pnHereNowChannelData.Occupancy.Equals(occupancy);
							Debug.Log("Occupancy.Equals:" + matchResult);
						}

						if (checkState || checkOccupantData){
							bool found = false;
							bool checkStateResult = false;
							foreach(PNHereNowOccupantData pnHereNowOccupantData in pnHereNowChannelData.Occupants){
								Debug.Log("finding:" + pnHereNowOccupantData.UUID);
								
								if(checkState){
									Debug.Log(state.ToString());
									
									checkStateResult = pnHereNowOccupantData.State.Equals(pubnub.JsonLibrary.SerializeToJsonString(state));
									Debug.Log("checkStateResult:" + checkStateResult);
								}
								
								if(checkOccupantData){
									if(pnHereNowOccupantData.UUID.Equals(uuid)){
										found = true;
										Debug.Log("found:" + pnHereNowOccupantData.UUID);
										break;
									} 
								}
							}
							if(checkState && checkOccupantData){
								matchResult = checkStateResult && found;
							} else if(checkOccupantData){
								matchResult = found;
							} else if (checkState){
								matchResult = checkState;
							}
							
						}
					}else {
						Assert.Fail("channel not found" + hereNowChannel);
					}
				}
				
			} else {
				Assert.Fail("Channels null");
			}
			Debug.Log("matchResult:" + matchResult);
			return matchResult;
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

		//[UnityTest]
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
					tresult = mea.Status.UUID.Contains(pnConfiguration.UUID);
					Assert.True(tresult);
				} 
			};
			pubnub.Subscribe ().Channels(channelList2).Execute();
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
					Debug.Log("mea.Status.Error:" + mea.Status.Error);
					Assert.True(mea.Status.Error);
					bool errorData = true;
					if(mea.Status.ErrorData!=null){
						Debug.Log(mea.Status.ErrorData.Info);
						errorData = mea.Status.ErrorData.Info.Contains("Duplicate Channels or Channel Groups");
						Assert.True(errorData);
					}
					
					tresult = errorData && mea.Status.Error;
				} 
			};
			pubnub.Subscribe ().Channels(channelList2).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			pubnub.Subscribe ().Channels(channelList2).Execute();
			Assert.True(tresult, "test didn't return");
			pubnub.CleanUp();

		}

		public IEnumerator DoJoinLeaveTestProcsssing(string channel) {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			channel = channel+ r.Next (100);
			pnConfiguration.UUID = "UnityTestJoinUUID_" + r.Next (100);
			PubNub pubnub = new PubNub(pnConfiguration);
			
			PNConfiguration pnConfiguration2 = PlayModeCommon.SetPNConfig(false);
			
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
						Debug.Log(mea.PresenceEventResult.UUID);
						Debug.Log(mea.PresenceEventResult.Timestamp);
						Debug.Log(mea.PresenceEventResult.Occupancy);
						Debug.Log(string.Join(",",mea.PresenceEventResult.Join.ToArray()));
						bool containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration2.UUID);
						Assert.True(containsUUID);
						bool containsOccupancy = mea.PresenceEventResult.Occupancy > 0;
						Assert.True(containsOccupancy);
						bool containsTimestamp = mea.PresenceEventResult.Timestamp > 0;
						Assert.True(containsTimestamp);
						
						tJoinResult = containsTimestamp && containsOccupancy && containsUUID;
						Debug.Log("containsUUID"+containsUUID+tJoinResult);
						
					} else if (mea.PresenceEventResult.Event.Equals("leave")){
						bool containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration2.UUID);
						Assert.True(containsUUID);
						bool containsTimestamp = mea.PresenceEventResult.Timestamp > 0;
						Assert.True(containsTimestamp);
						tLeaveResult = containsTimestamp && containsUUID;
						
						Debug.Log("containsUUID"+containsUUID+tLeaveResult);
					}					
				} 
			};
			pubnub.Subscribe ().Channels(channelList2).WithPresence().Execute();
			//yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			
			pubnub2.Subscribe ().Channels(channelList2).Execute();
			//yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			yield return new WaitForSeconds (7);			
			
			Assert.True(tJoinResult, "join test didn't return");
			pubnub2.Unsubscribe().Channels(channelList2).Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
					Assert.True(!status.Error);
				});
			yield return new WaitForSeconds (7);
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
			pubnub.Subscribe ().Channels(channelList2).Execute();
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
					Debug.Log("PAYLOAD20:" + payload.ToString() + payload.GetType());		
					
					Assert.True(mea.MessageResult.Channel.Equals(publishChannel));
					if(payload.GetType().Equals(typeof(Int64))){
						long expected; 
						if(Int64.TryParse(payload.ToString(), out expected)){
							long response;
							if(Int64.TryParse(mea.MessageResult.Payload.ToString(), out response)){
								bool expectedAndResponseMatch = expected.Equals(response);
								Assert.IsTrue(expectedAndResponseMatch);
								testReturn = expectedAndResponseMatch;
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
							} else {																
								testReturn = found;
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
							} else {																
								testReturn = found;
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
							} else {																
								testReturn = found;
							}
						}	
					} else if(payload.GetType().Equals(typeof(System.Object[]))){
						System.Object[] expected = (System.Object[])payload;
						System.Object[] response = (System.Object[])mea.MessageResult.Payload;
						// + payload.GetType().Equals(typeof(System.Object[])) + expected[0].Equals(response[0]));
						bool expectedAndResponseMatch = expected.Length.Equals(response.Length) && expected.Length.Equals(0);
						Assert.IsTrue(expectedAndResponseMatch);
						testReturn = expectedAndResponseMatch;
					} else if(payload.GetType().Equals(typeof(Dictionary<string, string>))){
						Dictionary<string, string> expected = (Dictionary<string, string>)payload;
						IDictionary response = mea.MessageResult.Payload as IDictionary;
						Debug.Log("PAYLOAD21:" + payload.ToString() + payload.GetType());
						//Assert.True(response["cat"].Equals("test"));
						bool expectedAndResponseMatch = response["cat"].Equals("test");
						testReturn = expectedAndResponseMatch;
					} else if(payload.GetType().Equals(typeof(Dictionary<string, int>))){
						Dictionary<string, int> expected = (Dictionary<string, int>)payload;
						IDictionary response = mea.MessageResult.Payload as IDictionary;
						Debug.Log("PAYLOAD22:" + payload.ToString() + payload.GetType());
						bool expectedAndResponseMatch =  (response == null || response.Count < 1);
						Assert.IsTrue(expectedAndResponseMatch);
						testReturn = expectedAndResponseMatch;

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
							} else {																
								testReturn = found;
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

						bool versionIdMatch = response.VersionID.Equals(expected.VersionID);
						bool timetokenMatch = response.Timetoken.Equals(expected.Timetoken);
						bool operationNameMatch = response.OperationName.Equals(expected.OperationName);
						bool demoMessageMatch = response.DemoMessage.DefaultMessage.Equals(expected.DemoMessage.DefaultMessage);
						bool customMessageMatch = response.CustomMessage.DefaultMessage.Equals(expected.CustomMessage.DefaultMessage);

						testReturn = versionIdMatch && timetokenMatch && operationNameMatch && demoMessageMatch && customMessageMatch;
					} else {
						Debug.Log("PAYLOAD24:" + payload.ToString() + payload.GetType());
						testReturn = mea.MessageResult.Payload.Equals(payload);
					}
					//testReturn = true;
				}
			};
			pubnub.Subscribe ().Channels(channelList2).Execute();
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
					tresult = mea.MessageResult.Channel.Equals(channel) && mea.MessageResult.Payload.ToString().Equals(payload);
				} 
			};
			pubnub.Subscribe ().Channels(channelList2).SetTimeToken(timetoken).Execute();
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
						tresult = result.Message.Contains("OK");
					} else {
						Assert.Fail("AddChannelsToChannelGroup failed");
					}
				});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return1");
			tresult = false;

			pubnub.ListChannelsForChannelGroup().ChannelGroup(channelGroup).Async((result, status) => {
					if(!status.Error){
						if(result.Channels!=null){
							bool matchChannel1 = result.Channels.Contains(channel);
							bool matchChannel2 = result.Channels.Contains(channel2);
							Assert.IsTrue(matchChannel1);
							Assert.IsTrue(matchChannel2);
							tresult = matchChannel1 && matchChannel2;
						} else {
							Assert.Fail("result.Channels empty");
						}
					} else {
						Assert.Fail("AddChannelsToChannelGroup failed");
					}
					
				});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return2");
			tresult = false;
			string payload = string.Format("payload {0}", pnConfiguration.UUID);

			pubnub.SusbcribeCallback += (sender, e) => { 
				SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;
				if(!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
					if(mea.MessageResult!=null){
						Debug.Log("SusbcribeCallback" + mea.MessageResult.Subscription);
						Debug.Log("SusbcribeCallback" + mea.MessageResult.Channel);
						Debug.Log("SusbcribeCallback" + mea.MessageResult.Payload);
						Debug.Log("SusbcribeCallback" + mea.MessageResult.Timetoken);
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
			
			pubnub.Subscribe ().ChannelGroups(channelGroupList).Execute();
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
			pubnub.CleanUp();
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
						tresult = result.Message.Contains("OK");
					} else {
						Assert.Fail("AddChannelsToChannelGroup failed");
					}
				});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return1");
			tresult = false;

			pubnub.ListChannelsForChannelGroup().ChannelGroup(channelGroup).Async((result, status) => {
					if(!status.Error){
						if(result.Channels!=null){
							bool matchChannel1 = result.Channels.Contains(channel);
							bool matchChannel2 = result.Channels.Contains(channel2);
							Assert.IsTrue(matchChannel1);
							Assert.IsTrue(matchChannel2);
							tresult = matchChannel1 && matchChannel2;
						} else {
							Assert.Fail("result.Channels empty");
						}
					} else {
						Assert.Fail("AddChannelsToChannelGroup failed");
					}
				});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return2");
			tresult = false;

			List<string> listChannelsRemove = new List<string>{channel};
			listChannelsRemove.Add(channel);
			pubnub.RemoveChannelsFromChannelGroup().Channels(listChannelsRemove).ChannelGroup(channelGroup).Async((result, status) => {
                    Debug.Log ("in RemoveChannelsFromCG");
                    if(!status.Error){
						
                        tresult = result.Message.Equals("OK");
                    }

                });
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return 8");

			tresult = false;
			pubnub.ListChannelsForChannelGroup().ChannelGroup(channelGroup).Async((result, status) => {
					if(!status.Error){
						if(result.Channels!=null){
							bool matchChannel1 = result.Channels.Contains(channel);
							bool matchChannel2 = result.Channels.Contains(channel2);
							Assert.IsTrue(!matchChannel1);
							Assert.IsTrue(matchChannel2);
							tresult = !matchChannel1 && matchChannel2;
							
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
                        tresult = result.Message.Equals("OK");
                    }
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

			pubnub.AddPushNotificationsOnChannels().Channels(channelList).DeviceID(deviceId).PushType(pnPushType).Async((result, status) => {
                    Debug.Log ("in AddChannelsToChannelGroup " + status.Error);
                    if(!status.Error){
						Debug.Log(result.Message);
						tresult = result.Message.Contains("Modified Ch");
					} else {
						Assert.Fail("AddPushNotificationsOnChannels failed");
					}
                });
						
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return1");
			tresult = false;

			pubnub.AuditPushChannelProvisions().DeviceID(deviceId).PushType(pnPushType).Async((result, status) => {
                    if(!status.Error){
						if(result.Channels!=null){
							bool matchChannel1 = result.Channels.Contains(channel);
							bool matchChannel2 = result.Channels.Contains(channel2);
							Assert.IsTrue(matchChannel1);
							Assert.IsTrue(matchChannel2);
							tresult = matchChannel1 && matchChannel2;
													
						} else {
							Assert.Fail("result.Channels empty");
						}
					} else {
						Assert.Fail("AddChannelsToChannelGroup failed");
					}
                });

			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return2");
			tresult = false;

			List<string> listChannelsRemove = new List<string>{channel};
			listChannelsRemove.Add(channel);
			pubnub.RemovePushNotificationsFromChannels().Channels(listChannelsRemove).DeviceID(deviceId).PushType(pnPushType).Async((result, status) => {
                    Debug.Log ("in RemovePushNotificationsFromChannels");
					if(!status.Error){
                        tresult = result.Message.Equals("Modified Channels");
                    }
                });

			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return 8");

			tresult = false;
			pubnub.AuditPushChannelProvisions().DeviceID(deviceId).PushType(pnPushType).Async((result, status) => {
                    if(!status.Error){
						if(result.Channels!=null){
							bool matchChannel1 = result.Channels.Contains(channel);
							bool matchChannel2 = result.Channels.Contains(channel2);
							Assert.IsTrue(!matchChannel1);
							Assert.IsTrue(matchChannel2);
							tresult = !matchChannel1 && matchChannel2;
														
						} else {
							Assert.Fail("result.Channels empty");
						}
					} else {
						Assert.Fail("AddChannelsToChannelGroup failed");
					}
                });
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			
			Assert.True(tresult, "test didn't return 9");

			tresult = false;
			pubnub.RemoveAllPushNotifications().DeviceID(deviceId).PushType(pnPushType).Async((result, status) => {
                    if(!status.Error){
                        tresult = result.Message.Equals("Removed Device");
                    }
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
					Debug.Log("SusbcribeCallback" + mea.MessageResult.Subscription);
					Debug.Log("SusbcribeCallback" + mea.MessageResult.Channel);
					Debug.Log("SusbcribeCallback" + mea.MessageResult.Payload);
					Debug.Log("SusbcribeCallback" + mea.MessageResult.Timetoken);
					bool matchChannel = mea.MessageResult.Channel.Equals(channel);
					Assert.True(matchChannel);
					bool matchPayload = mea.MessageResult.Payload.ToString().Equals(payload);
					Assert.True(matchPayload);
					tresult = matchPayload  && matchChannel;
					
				} 
			};
			pubnub.Subscribe ().Channels(channelList2).Execute();
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
					Debug.Log("SusbcribeCallback" + mea.MessageResult.Subscription);
					Debug.Log("SusbcribeCallback" + mea.MessageResult.Channel);
					Debug.Log("SusbcribeCallback" + mea.MessageResult.Payload);
					Debug.Log("SusbcribeCallback" + mea.MessageResult.Timetoken);
					bool matchChannel = mea.MessageResult.Channel.Equals(channel);
					Assert.True(matchChannel);
					bool matchPayload = mea.MessageResult.Payload.ToString().Equals(payload);
					Assert.True(matchPayload);
					tresult = matchPayload  && matchChannel;
				} 
			};
			pubnub.Subscribe ().Channels(channelList2).Execute();
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
			string channel = "UnityPublishAndHistoryChannel";
			string payload = string.Format("payload no store {0}", pnConfiguration.UUID);

			PubNub pubnub = new PubNub(pnConfiguration);

			List<string> channelList2 = new List<string>();
			channelList2.Add(channel);
			bool tresult = false;

			pubnub.Publish().Channel(channel).Message(payload).Async((result, status) => {
				bool timetokenMatch = !result.Timetoken.Equals(0);
				bool statusError = status.Error.Equals(false);
				bool statusCodeMatch = status.StatusCode.Equals(0);
				Assert.True(timetokenMatch);
				Assert.True(statusError);
				Assert.True(statusCodeMatch, status.StatusCode.ToString());
				tresult = statusCodeMatch && statusError && timetokenMatch;
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
							tresult = pnHistoryItemResult.Entry.ToString().Contains(payload);
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
				bool timetokenMatch = !result.Timetoken.Equals(0);
				bool statusError = status.Error.Equals(false);
				bool statusCodeMatch = status.StatusCode.Equals(0);
				Assert.True(timetokenMatch);
				Assert.True(statusError);
				Assert.True(statusCodeMatch, status.StatusCode.ToString());
				tresult = statusCodeMatch && statusError && timetokenMatch;
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
							tresult = !pnHistoryItemResult.Entry.ToString().Contains(payload);
						} else {
							tresult = false;
						}						
					} else {
						tresult = true;
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
				bool statusError = status.Error.Equals(true);
				bool statusCodeMatch = status.StatusCode.Equals(0);
				Assert.True(statusError);
				Assert.True(statusCodeMatch, status.StatusCode.ToString());
				tresult = statusCodeMatch && statusError;
				
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
				bool statusError = status.Error.Equals(true);
				bool statusCodeMatch = status.StatusCode.Equals(0);
				Assert.True(statusError);
				Assert.True(statusCodeMatch, status.StatusCode.ToString());
				tresult = statusCodeMatch && statusError;
			});

			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return 10");
			
			pubnub.CleanUp();
		}

		[UnityTest]
		public IEnumerator TestFire() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next (100);
			string channel = "UnityTestFireChannel";
			string payload = string.Format("payload no store {0}", pnConfiguration.UUID);

			PubNub pubnub = new PubNub(pnConfiguration);

			List<string> channelList2 = new List<string>();
			channelList2.Add(channel);
			bool tresult = false;

			pubnub.Fire().Channel(channel).Message(payload).Async((result, status) => {
				bool timetokenMatch = !result.Timetoken.Equals(0);
				bool statusError = status.Error.Equals(false);
				bool statusCodeMatch = status.StatusCode.Equals(0);
				Assert.True(timetokenMatch);
				Assert.True(statusError);
				Assert.True(statusCodeMatch, status.StatusCode.ToString());
				tresult = statusCodeMatch && statusError && timetokenMatch;
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
							tresult = !pnHistoryItemResult.Entry.ToString().Contains(payload);
						} else {
							tresult = false;
						}						
					} else {
						tresult = true;
					}
					
                }
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didnt return 2");
			
			pubnub.CleanUp();
		}

		[UnityTest]
		public IEnumerator TestWildcardSubscribe() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration.UUID = "UnityWildSubscribeUUID_" + r.Next (100);
			string chToPub = "UnityWildSubscribeChannel." + r.Next (100);
			string channel = "UnityWildSubscribeChannel.*";
			string payload = string.Format("payload {0}", pnConfiguration.UUID);
			PubNub pubnub = new PubNub(pnConfiguration);

			List<string> channelList2 = new List<string>();
			channelList2.Add(channel);
			string whatToTest = "join1";
			bool tJoinResult = false;
			bool tLeaveResult = false;
			bool tresult = false;

			PNConfiguration pnConfiguration2 = PlayModeCommon.SetPNConfig(false);
			pnConfiguration2.UUID = "UnityWildSubscribeUUID2_" + r.Next (100);

			pubnub.SusbcribeCallback += (sender, e) => { 
				SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;
				if(!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
					switch (whatToTest){
						case "join1":
						case "join2":
							Debug.Log("join1 or join2");
							if(mea.PresenceEventResult.Event.Equals("join")){
								bool containsUUID = false;
								if(whatToTest.Equals("join1")){
									containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration.UUID);
								} else {
									containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration2.UUID);
								}
								
								Assert.True(containsUUID);
								Debug.Log("containsUUID:" + containsUUID);
								bool containsOccupancy = mea.PresenceEventResult.Occupancy > 0;
								Assert.True(containsOccupancy);
								Debug.Log("containsOccupancy:" + containsOccupancy);

								bool containsTimestamp = mea.PresenceEventResult.Timestamp > 0;
								Assert.True(containsTimestamp);
								Debug.Log("containsTimestamp:" + containsTimestamp);
								
								bool containsSubscription = mea.PresenceEventResult.Subscription.Equals(channel);
								Assert.True(containsSubscription);
								Debug.Log("containsSubscription:" + containsSubscription);

								tJoinResult = containsTimestamp && containsOccupancy && containsUUID && containsSubscription;
							}	
						break;
						case "leave":
							if(mea.PresenceEventResult.Event.Equals("leave")){
								bool containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration2.UUID);
								Assert.True(containsUUID);
								Debug.Log(containsUUID);
								bool containsTimestamp = mea.PresenceEventResult.Timestamp > 0;
								Assert.True(containsTimestamp);
								bool containsSubscription = mea.PresenceEventResult.Subscription.Equals(channel);
								Assert.True(containsSubscription);
								bool containsOccupancy = mea.PresenceEventResult.Occupancy > 0;
								Assert.True(containsOccupancy);
								Debug.Log("containsSubscription:" + containsSubscription);
								Debug.Log("containsTimestamp:" + containsTimestamp);
								Debug.Log("containsOccupancy:" + containsOccupancy);
								Debug.Log("containsUUID:" + containsUUID);

								tLeaveResult = containsTimestamp && containsOccupancy && containsUUID && containsSubscription;
							}
						break;
						default:
							Debug.Log("SusbcribeCallback" + mea.MessageResult.Subscription);
							Debug.Log("SusbcribeCallback" + mea.MessageResult.Channel);
							Debug.Log("SusbcribeCallback" + mea.MessageResult.Payload);
							Debug.Log("SusbcribeCallback" + mea.MessageResult.Timetoken);
							bool matchChannel = mea.MessageResult.Channel.Equals(chToPub);
							Assert.True(matchChannel);
							bool matchPayload = mea.MessageResult.Payload.ToString().Equals(payload);
							Assert.True(matchPayload);

							bool matchSubscription = mea.MessageResult.Subscription.Equals(channel);
							Assert.True(matchSubscription);
							tresult = matchPayload  && matchChannel && matchSubscription;
						break;
					}
				} 
			};
			pubnub.Subscribe ().Channels(channelList2).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			Assert.True(tJoinResult, "subscribe didn't get a join");

			whatToTest = "";

			pubnub.Publish().Channel(chToPub).Message(payload).Async((result, status) => {
				bool timetokenMatch = !result.Timetoken.Equals(0);
				bool statusError = status.Error.Equals(false);
				bool statusCodeMatch = status.StatusCode.Equals(0);
				Assert.True(timetokenMatch);
				Assert.True(statusError);
				Assert.True(statusCodeMatch, status.StatusCode.ToString());
				tresult = statusCodeMatch && statusError && timetokenMatch;
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);

			Assert.True(tresult, "Subcribe didn't get a message");

			PubNub pubnub2 = new PubNub(pnConfiguration2);

			whatToTest = "join2";

			pubnub2.Subscribe ().Channels(channelList2).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			Assert.True(tJoinResult, "subscribe2 didn't get a join");

			whatToTest = "leave";

			tresult = false;
			pubnub2.Unsubscribe().Channels(channelList2).Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
					tresult = !status.Error;
				});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			Assert.True(tresult, "unsubscribe didn't return");
			Assert.True(tLeaveResult, "subscribe didn't get a leave");
			
			pubnub.CleanUp();
			pubnub2.CleanUp();
		}

		[UnityTest]
		public IEnumerator TestUnsubscribeAllAndUnsubscribe() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration.UUID = "UnityWildSubscribeUUID_" + r.Next (100);
			string channel = "UnityWildSubscribeChannel." + r.Next (100);
			string channel2 = "UnityWildSubscribeChannel." + r.Next (100);

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
			pnConfiguration2.UUID = "UnityWildSubscribeUUID2_" + r.Next (100);

			pubnub.SusbcribeCallback += (sender, e) => { 
				SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;
				if(!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
					switch (whatToTest){
						case "join1":
						case "join2":
							if(mea.PresenceEventResult.Event.Equals("join")){
								bool containsUUID = false;
								if(whatToTest.Equals("join1")){
									containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration.UUID);
								} else {
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
							if(mea.PresenceEventResult.Event.Equals("leave")){
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
							Debug.Log("SusbcribeCallback" + mea.MessageResult.Subscription);
							Debug.Log("SusbcribeCallback" + mea.MessageResult.Channel);
							Debug.Log("SusbcribeCallback" + mea.MessageResult.Payload);
							Debug.Log("SusbcribeCallback" + mea.MessageResult.Timetoken);
							bool matchChannel = mea.MessageResult.Channel.Equals(channel);
							Assert.True(matchChannel);
							bool matchPayload = mea.MessageResult.Payload.ToString().Equals(payload);
							Assert.True(matchPayload);

							tresult = matchPayload  && matchChannel;
						break;
					}
				} 
			};
			pubnub.Subscribe ().Channels(channelList2).WithPresence().Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			//Assert.True(tJoinResult, "subscribe didn't get a join");

			whatToTest = "join2";
			PubNub pubnub2 = new PubNub(pnConfiguration2);

			pubnub2.Subscribe ().Channels(channelList2).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			Assert.True(tJoinResult, "subscribe2 didn't get a join");

			whatToTest = "leave";

			tresult = false;
			List<string> channelList = new List<string>();
			channelList.Add(channel);
			pubnub2.Unsubscribe().Channels(channelList).Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
					tresult = !status.Error;
					//Debug.Log("result.Message:" + result.Message);
				});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			Assert.True(tresult, "unsubscribe didn't return");

			tresult = false;
			pubnub2.UnsubscribeAll().Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
					tresult = !status.Error;
				});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			Assert.True(tresult, "unsubscribeAll didn't return");
			Assert.True(tLeaveResult, "subscribe didn't get a leave");
			
			pubnub.CleanUp();
			pubnub2.CleanUp();
		}

		[UnityTest]
		public IEnumerator TestReconnect() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration.UUID = "UnityReconnectUUID" + r.Next (100);
			string channel = "UnityReconnectChannel." + r.Next (100);

			string payload = string.Format("Reconnect payload {0}", pnConfiguration.UUID);
			PubNub pubnub = new PubNub(pnConfiguration);

			List<string> channelList2 = new List<string>();
			channelList2.Add(channel);
			bool tresult = false;
			string whatToTest = "join1";

			PNConfiguration pnConfiguration2 = PlayModeCommon.SetPNConfig(false);
			pnConfiguration2.UUID = "UnityReconnectUUID2" + r.Next (100);

			pubnub.SusbcribeCallback += (sender, e) => { 
				SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;
				
				switch (whatToTest){
					case "connected":
					if(mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
						tresult = true;
					} 
					break;
					case "join1":
					case "join2":
					if(!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
						if(mea.PresenceEventResult.Event.Equals("join")){
							bool containsUUID = false;
							if(whatToTest.Equals("join1")){
								containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration.UUID);
							} else {
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
					if(!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
						if(mea.PresenceEventResult.Event.Equals("leave")){
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
					if(!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
						Debug.Log("SusbcribeCallback" + mea.MessageResult.Subscription);
						Debug.Log("SusbcribeCallback" + mea.MessageResult.Channel);
						Debug.Log("SusbcribeCallback" + mea.MessageResult.Payload);
						Debug.Log("SusbcribeCallback" + mea.MessageResult.Timetoken);
						bool matchChannel = mea.MessageResult.Channel.Equals(channel);
						Assert.True(matchChannel);
						bool matchPayload = mea.MessageResult.Payload.ToString().Equals(payload);
						Assert.True(matchPayload);

						tresult = matchPayload  && matchChannel;
					}
					break;
				}
			};
			pubnub.Subscribe ().Channels(channelList2).WithPresence().Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			Assert.True(tresult, "didn't subscribe");

			whatToTest = "join2";
			PubNub pubnub2 = new PubNub(pnConfiguration2);

			tresult = false;

			pubnub2.Subscribe ().Channels(channelList2).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			Assert.True(tresult, "subscribe2 didn't get a join");

			tresult = false;
			pubnub.Reconnect();

			pubnub2.Publish().Channel(channel).Message(payload).Async((result, status) => {
				bool timetokenMatch = !result.Timetoken.Equals(0);
				bool statusError = status.Error.Equals(false);
				bool statusCodeMatch = status.StatusCode.Equals(0);
				Assert.True(timetokenMatch);
				Assert.True(statusError);
				Assert.True(statusCodeMatch, status.StatusCode.ToString());
				tresult = statusCodeMatch && statusError && timetokenMatch;
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);

			Assert.True(tresult, "publish didn't return");

			whatToTest = "";

			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "subscribe didn't return");
			
			pubnub.CleanUp();
			pubnub2.CleanUp();
		}

		[UnityTest]
		public IEnumerator TestPresenceCG() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration.UUID = "UnityTestCGPresUUID_" + r.Next (100);
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
			pnConfiguration2.UUID = "UnityReconnectUUID2" + r.Next (100);

			pubnub.AddChannelsToChannelGroup().Channels(channelList).ChannelGroup(channelGroup).Async((result, status) => {
					Debug.Log ("in AddChannelsToChannelGroup " + status.Error);
					if(!status.Error){
						Debug.Log(result.Message);
						tresult = result.Message.Contains("OK");
					} else {
						Assert.Fail("AddChannelsToChannelGroup failed");
					}
				});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls3);
			Assert.True(tresult, "test didn't return1");
			tresult = false;
			string whatToTest = "join1";

			pubnub.SusbcribeCallback += (sender, e) => { 
				SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;
				
				switch (whatToTest){					
					case "join1":
					case "join2":
					if(!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
						if(mea.PresenceEventResult.Event.Equals("join")){
							bool containsUUID = false;
							if(whatToTest.Equals("join1")){
								containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration.UUID);
							} else {
								containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration2.UUID);
							}
							bool containsOccupancy = mea.PresenceEventResult.Occupancy > 0;
							Assert.True(containsOccupancy);
							bool containsTimestamp = mea.PresenceEventResult.Timestamp > 0;
							Assert.True(containsTimestamp);
							Debug.Log(containsUUID);
							Debug.Log("mea.PresenceEventResult.Subscription:"+mea.PresenceEventResult.Subscription);
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
					if(!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
						if(mea.PresenceEventResult.Event.Equals("leave")){
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
			
			pubnub.Subscribe ().ChannelGroups(channelGroupList).WithPresence().Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			//Assert.True(tresult, "subscribe1 didn't get a join");

			whatToTest = "join2";
			PubNub pubnub2 = new PubNub(pnConfiguration2);

			tresult = false;

			pubnub2.Subscribe ().ChannelGroups(channelGroupList).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			Assert.True(tresult, "subscribe2 didn't get a join");

			whatToTest = "leave";
			tresult = false;
			pubnub2.Unsubscribe().ChannelGroups(channelGroupList).Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
					//tresult = !status.Error;
				});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			//Assert.True(tresult, "unsubscribeAll didn't return");
			Assert.True(tresult, "subscribe didn't get a leave");
			
			pubnub.CleanUp();
			pubnub2.CleanUp();

		}	
		
		[UnityTest]
		public IEnumerator TestHistory() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next (100);
			string channel = "UnityPublishAndHistoryChannel_" + r.Next (100);
			string payload = string.Format("payload {0}", pnConfiguration.UUID);

			PubNub pubnub = new PubNub(pnConfiguration);

			List<string> channelList2 = new List<string>();
			channelList2.Add(channel);
			bool tresult = false;

			long timetoken1 = 0;
			pubnub.Time().Async((result, status) => {
				timetoken1 = result.TimeToken;
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			
			Assert.True(!timetoken1.Equals(0));

			List<string> payloadList = new List<string>();
			for(int i=0; i<4; i++){
				payloadList.Add(string.Format("{0}, seq: {1}", payload, i));
			}

			//Get Time: t1
			//Publish 2 msg
			//get time: t2
			//Publish 2 msg
			//get time: t3
			
			for(int i=0; i<2; i++){
				tresult = false;
				
				pubnub.Publish().Channel(channel).Message(payloadList[i]).Async((result, status) => {
					bool timetokenMatch = !result.Timetoken.Equals(0);
					bool statusError = status.Error.Equals(false);
					bool statusCodeMatch = status.StatusCode.Equals(0);
					Assert.True(timetokenMatch);
					Assert.True(statusError);
					Assert.True(statusCodeMatch, status.StatusCode.ToString());
					tresult = statusCodeMatch && statusError && timetokenMatch;
				});
				yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);

				Assert.True(tresult, string.Format("test didnt return {0}", i));
			}

			tresult = false;

			long timetoken2 = 0;
			pubnub.Time().Async((result, status) => {
				timetoken2 = result.TimeToken;
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			
			Assert.True(!timetoken2.Equals(0));

			for(int i=2; i<4; i++){
				tresult = false;
				pubnub.Publish().Channel(channel).Message(payloadList[i]).Async((result, status) => {
					bool timetokenMatch = !result.Timetoken.Equals(0);
					bool statusError = status.Error.Equals(false);
					bool statusCodeMatch = status.StatusCode.Equals(0);
					Assert.True(timetokenMatch);
					Assert.True(statusError);
					Assert.True(statusCodeMatch, status.StatusCode.ToString());
					tresult = statusCodeMatch && statusError && timetokenMatch;
				});
				yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);

				Assert.True(tresult, string.Format("test didnt return {0}", i));
			}

			tresult = false;

			long timetoken3 = 0;
			pubnub.Time().Async((result, status) => {
				timetoken3 = result.TimeToken;
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			
			Assert.True(!timetoken3.Equals(0));

			tresult = false;

			//History t1 - t2
			
			int testCount = 2;
			int testStart = 0;
			pubnub.History().Channel(channel).Start(timetoken1).End(timetoken2).IncludeTimetoken(true).Async((result, status) => {
				Assert.True(status.Error.Equals(false));
				if(!status.Error){

					if((result.Messages!=null) && (result.Messages.Count.Equals(testCount))){
						List<PNHistoryItemResult> listPNHistoryItemResult = result.Messages as List<PNHistoryItemResult>;	
						for(int i=0; i<testCount; i++){
							PNHistoryItemResult pnHistoryItemResult = listPNHistoryItemResult[i] as PNHistoryItemResult;
							if(pnHistoryItemResult != null){
								bool found = false;
								for(int j=0; j<testCount; j++){
									if(pnHistoryItemResult.Entry.ToString().Contains(payloadList[j])){
										found = (pnHistoryItemResult.Timetoken>0);
										Debug.Log("found" + payloadList[j] );
										break;
									}
								}
								tresult = found;
								if(!tresult){
									break;
								}
							}
						}						
					} 
                } 
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			Assert.True(tresult, "history test didnt return");

			testCount = 2;
			testStart = 2;
			pubnub.History().Channel(channel).Start(timetoken2).End(timetoken3).Async((result, status) => {
				Assert.True(status.Error.Equals(false));
				if(!status.Error){

					if((result.Messages!=null) && (result.Messages.Count.Equals(testCount))){
						List<PNHistoryItemResult> listPNHistoryItemResult = result.Messages as List<PNHistoryItemResult>;	
						for(int i=0; i<testCount; i++){
							PNHistoryItemResult pnHistoryItemResult = listPNHistoryItemResult[i] as PNHistoryItemResult;
							if(pnHistoryItemResult != null){
								bool found = false;
								for(int j=testStart; j<testCount+testStart; j++){
									if(pnHistoryItemResult.Entry.ToString().Contains(payloadList[j])){
										found = true;
										Debug.Log("found" + payloadList[j] );
										break;
									}
								}
								tresult = found;
								if(!tresult){
									break;
								}
							}
						}						
					} 
                } 
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			Assert.True(tresult, "history test didnt return 2");
			
			pubnub.CleanUp();
		}	

		[UnityTest]
		public IEnumerator TestHistory2() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration.UUID = "UnityTestConnectedUUID_" + r.Next (100);
			string channel = "UnityPublishAndHistoryChannel2_" + r.Next (100);
			string payload = string.Format("payload {0}", pnConfiguration.UUID);

			PubNub pubnub = new PubNub(pnConfiguration);

			List<string> channelList2 = new List<string>();
			channelList2.Add(channel);
			bool tresult = false;

			long timetoken1 = 0;
			pubnub.Time().Async((result, status) => {
				timetoken1 = result.TimeToken;
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			
			Assert.True(!timetoken1.Equals(0));

			List<string> payloadList = new List<string>();
			for(int i=0; i<4; i++){
				payloadList.Add(string.Format("{0}, seq: {1}", payload, i));
			}

			//Get Time: t1
			//Publish 2 msg
			//get time: t2
			//Publish 2 msg
			//get time: t3
			
			for(int i=0; i<2; i++){
				tresult = false;
				
				pubnub.Publish().Channel(channel).Message(payloadList[i]).Async((result, status) => {
					bool timetokenMatch = !result.Timetoken.Equals(0);
					bool statusError = status.Error.Equals(false);
					bool statusCodeMatch = status.StatusCode.Equals(0);
					Assert.True(timetokenMatch);
					Assert.True(statusError);
					Assert.True(statusCodeMatch, status.StatusCode.ToString());
					tresult = statusCodeMatch && statusError && timetokenMatch;
				});
				yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);

				Assert.True(tresult, string.Format("test didnt return {0}", i));
			}

			tresult = false;

			long timetoken2 = 0;
			pubnub.Time().Async((result, status) => {
				timetoken2 = result.TimeToken;
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			
			Assert.True(!timetoken2.Equals(0));

			for(int i=2; i<4; i++){
				tresult = false;
				pubnub.Publish().Channel(channel).Message(payloadList[i]).Async((result, status) => {
					bool timetokenMatch = !result.Timetoken.Equals(0);
					bool statusError = status.Error.Equals(false);
					bool statusCodeMatch = status.StatusCode.Equals(0);
					Assert.True(timetokenMatch);
					Assert.True(statusError);
					Assert.True(statusCodeMatch, status.StatusCode.ToString());
					tresult = statusCodeMatch && statusError && timetokenMatch;
				});
				yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);

				Assert.True(tresult, string.Format("test didnt return {0}", i));
			}

			tresult = false;

			long timetoken3 = 0;
			pubnub.Time().Async((result, status) => {
				timetoken3 = result.TimeToken;
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			
			Assert.True(!timetoken3.Equals(0));

			tresult = false;

			int testCount = 2;
			int testStart = 2;
			pubnub.History().Channel(channel).Start(timetoken2).IncludeTimetoken(true).Reverse(true).Async((result, status) => {
				Assert.True(status.Error.Equals(false));
				if(!status.Error){

					if((result.Messages!=null) && (result.Messages.Count.Equals(testCount))){
						List<PNHistoryItemResult> listPNHistoryItemResult = result.Messages as List<PNHistoryItemResult>;	
						for(int i=0; i<testCount; i++){
							PNHistoryItemResult pnHistoryItemResult = listPNHistoryItemResult[i] as PNHistoryItemResult;
							if(pnHistoryItemResult != null){
								bool found = false;
								Debug.Log("finding:" + pnHistoryItemResult.Entry.ToString() );
								for(int j=testStart; j<testCount+testStart; j++){
									if(pnHistoryItemResult.Entry.ToString().Contains(payloadList[j])){
										found = true;
										Debug.Log("found:" + payloadList[j] );
										break;
									}
								}
								tresult = found;
								if(!tresult){
									break;
								}
							}
						}						
					} 
                } 
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			Assert.True(tresult, "history test didnt return");
			tresult = false;
			testCount = 2;
			testStart = 2;
			pubnub.History().Channel(channel).End(timetoken1).Async((result, status) => {
				Assert.True(status.Error.Equals(false));
				if(!status.Error){

					if((result.Messages!=null) && (result.Messages.Count.Equals(testCount+testStart))){
						List<PNHistoryItemResult> listPNHistoryItemResult = result.Messages as List<PNHistoryItemResult>;	
						for(int i=0; i<testCount; i++){
							PNHistoryItemResult pnHistoryItemResult = listPNHistoryItemResult[i] as PNHistoryItemResult;
							if(pnHistoryItemResult != null){
								bool found = false;
								Debug.Log("finding:" + pnHistoryItemResult.Entry.ToString() );
								for(int j=0; j<testCount+testStart; j++){
									if(pnHistoryItemResult.Entry.ToString().Contains(payloadList[j])){
										found = true;
										Debug.Log("found" + payloadList[j] );
										break;
									}
								}
								tresult = found;
								if(!tresult){
									break;
								}
							}
						}						
					} 
                } 
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			Assert.True(tresult, "history test didnt return 2");

			testCount = 2;
			testStart = 2;
			pubnub.History().Channel(channel).Start(timetoken3).Async((result, status) => {
				Assert.True(status.Error.Equals(false));
				if(!status.Error){

					if((result.Messages!=null) && (result.Messages.Count.Equals(testCount+testStart))){
						List<PNHistoryItemResult> listPNHistoryItemResult = result.Messages as List<PNHistoryItemResult>;	
						for(int i=0; i<testCount; i++){
							PNHistoryItemResult pnHistoryItemResult = listPNHistoryItemResult[i] as PNHistoryItemResult;
							if(pnHistoryItemResult != null){
								bool found = false;
								Debug.Log("finding:" + pnHistoryItemResult.Entry.ToString() );
								for(int j=0; j<testCount+testStart; j++){
									if(pnHistoryItemResult.Entry.ToString().Contains(payloadList[j])){
										found = true;
										Debug.Log("found" + payloadList[j] );
										break;
									}
								}
								tresult = found;
								if(!tresult){
									break;
								}
							}
						}						
					} 
                } 
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			Assert.True(tresult, "history test didnt return 3");
			
			pubnub.CleanUp();
		}	

		//Get Time: t1
		//Publish 2 msg to ch 1
		//get time: t2
		//Publish 2 msg to ch 2
		//get time: t3
		//Fetch ch 1 and ch 2
		[UnityTest]
		public IEnumerator TestFetch() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration.UUID = "UnityTestFetchUUID_" + r.Next (100);
			string channel = "UnityPublishAndFetchChannel_" + r.Next (100);
			string channel2 = "UnityPublishAndFetchChannel2_" + r.Next (100);
			string payload = string.Format("payload {0}", pnConfiguration.UUID);

			PubNub pubnub = new PubNub(pnConfiguration);

			List<string> channelList2 = new List<string>();
			channelList2.Add(channel);
			channelList2.Add(channel2);
			bool tresult = false;

			long timetoken1 = 0;
			pubnub.Time().Async((result, status) => {
				timetoken1 = result.TimeToken;
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			
			Assert.True(!timetoken1.Equals(0));

			List<string> payloadList = new List<string>();
			for(int i=0; i<4; i++){
				payloadList.Add(string.Format("{0}, seq: {1}", payload, i));
			}
			
			for(int i=0; i<2; i++){
				tresult = false;
				
				pubnub.Publish().Channel(channel).Message(payloadList[i]).Async((result, status) => {
					bool timetokenMatch = !result.Timetoken.Equals(0);
					bool statusError = status.Error.Equals(false);
					bool statusCodeMatch = status.StatusCode.Equals(0);
					Assert.True(timetokenMatch);
					Assert.True(statusError);
					Assert.True(statusCodeMatch, status.StatusCode.ToString());
					tresult = statusCodeMatch && statusError && timetokenMatch;
				});
				yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);

				Assert.True(tresult, string.Format("test didnt return {0}", i));
			}

			tresult = false;

			long timetoken2 = 0;
			pubnub.Time().Async((result, status) => {
				timetoken2 = result.TimeToken;
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			
			Assert.True(!timetoken2.Equals(0));

			for(int i=2; i<4; i++){
				tresult = false;
				pubnub.Publish().Channel(channel2).Message(payloadList[i]).Async((result, status) => {
					bool timetokenMatch = !result.Timetoken.Equals(0);
					bool statusError = status.Error.Equals(false);
					bool statusCodeMatch = status.StatusCode.Equals(0);
					Assert.True(timetokenMatch);
					Assert.True(statusError);
					Assert.True(statusCodeMatch, status.StatusCode.ToString());
					tresult = statusCodeMatch && statusError && timetokenMatch;
				});
				yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);

				Assert.True(tresult, string.Format("test didnt return {0}", i));
			}

			tresult = false;

			long timetoken3 = 0;
			pubnub.Time().Async((result, status) => {
				timetoken3 = result.TimeToken;
			});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls);
			
			Assert.True(!timetoken3.Equals(0));

			tresult = false;

			pubnub.FetchMessages().Channels(channelList2).IncludeTimetoken(true).Async((result, status) => {
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
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			Assert.True(tresult, "fetch test didnt return");
			tresult = false;
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
			yield return new WaitForSeconds (5);
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
			yield return new WaitForSeconds (5);
			Assert.True(tresult, "fetch test didnt return 3");
			
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

			pubnub.SusbcribeCallback += (sender, e) => { 
				SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;
				if(!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
					if(mea.MessageResult!=null){
						Debug.Log("SusbcribeCallback" + mea.MessageResult.Subscription);
						Debug.Log("SusbcribeCallback" + mea.MessageResult.Channel);
						Debug.Log("SusbcribeCallback" + mea.MessageResult.Payload);
						Debug.Log("SusbcribeCallback" + mea.MessageResult.Timetoken);
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
		public IEnumerator TestUnsubscribeNoLeave() {
			PNConfiguration pnConfiguration = PlayModeCommon.SetPNConfig(false);
			System.Random r = new System.Random ();
			pnConfiguration.UUID = "UnityUnsubUUID_" + r.Next (100);
			string channel = "UnityUnubscribeChannel." + r.Next (100);
			string channel2 = "UnityUnubscribeChannel." + r.Next (100);

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
			pnConfiguration2.UUID = "UnityUnsubUUID2_" + r.Next (100);
			pnConfiguration2.SuppressLeaveEvents = true;

			pubnub.SusbcribeCallback += (sender, e) => { 
				SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;
				if(!mea.Status.Category.Equals(PNStatusCategory.PNConnectedCategory)){
					switch (whatToTest){
						case "join1":
						case "join2":
							if(mea.PresenceEventResult.Event.Equals("join")){
								bool containsUUID = false;
								if(whatToTest.Equals("join1")){
									containsUUID = mea.PresenceEventResult.UUID.Contains(pnConfiguration.UUID);
								} else {
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
							if(mea.PresenceEventResult.Event.Equals("leave")){
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
							Debug.Log("SusbcribeCallback" + mea.MessageResult.Subscription);
							Debug.Log("SusbcribeCallback" + mea.MessageResult.Channel);
							Debug.Log("SusbcribeCallback" + mea.MessageResult.Payload);
							Debug.Log("SusbcribeCallback" + mea.MessageResult.Timetoken);
							bool matchChannel = mea.MessageResult.Channel.Equals(channel);
							Assert.True(matchChannel);
							bool matchPayload = mea.MessageResult.Payload.ToString().Equals(payload);
							Assert.True(matchPayload);

							tresult = matchPayload  && matchChannel;
						break;
					}
				} 
			};
			pubnub.Subscribe ().Channels(channelList2).WithPresence().Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			//Assert.True(tJoinResult, "subscribe didn't get a join");

			whatToTest = "join2";
			PubNub pubnub2 = new PubNub(pnConfiguration2);

			pubnub2.Subscribe ().Channels(channelList2).Execute();
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			Assert.True(tJoinResult, "subscribe2 didn't get a join");

			whatToTest = "leave";

			tresult = false;
			List<string> channelList = new List<string>();
			channelList.Add(channel);
			pubnub2.Unsubscribe().Channels(channelList).Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
					tresult = !status.Error;
					//Debug.Log("result.Message:" + result.Message);
					
				});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			Assert.True(tresult, "unsubscribe didn't return");
			Assert.True(!tLeaveResult, "subscribe got a leave");

			tresult = false;
			pubnub2.UnsubscribeAll().Async((result, status) => {
					Debug.Log("status.Error:" + status.Error);
					tresult = !status.Error;
					//Debug.Log("result.Message:" + result.Message);
				});
			yield return new WaitForSeconds (PlayModeCommon.WaitTimeBetweenCalls2);
			Assert.True(tresult, "unsubscribeAll didn't return");
			Assert.True(!tLeaveResult, "subscribe got a leave 2");
			
			pubnub.CleanUp();
			pubnub2.CleanUp();
		}
		#endregion
		#endif
	}
}
