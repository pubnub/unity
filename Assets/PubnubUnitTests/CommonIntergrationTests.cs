#define USE_JSONFX_UNITY_IOS
using System;
using PubNubMessaging.Core;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

#if (USE_JSONFX) || (USE_JSONFX_UNITY)
using JsonFx.Json;

#elif (USE_JSONFX_UNITY_IOS)
using Pathfinding.Serialization.JsonFx;

#elif (USE_DOTNET_SERIALIZATION)
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;

#elif (USE_MiniJSON)
using MiniJSON;

#else
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
#endif


namespace PubNubMessaging.Tests
{
	class CustomClass
	{
		public string foo = "hi!";
		public int[] bar = { 1, 2, 3, 4, 5 };
	}

	[Serializable]
	class PubnubDemoObject
	{
		public double VersionID = 3.4;
		public long Timetoken = 13601488652764619;
		public string OperationName = "Publish";
		public string[] Channels = { "ch1" };
		public PubnubDemoMessage DemoMessage = new PubnubDemoMessage ();
		public PubnubDemoMessage CustomMessage = new PubnubDemoMessage ("This is a demo message");
		public XmlDocument SampleXml = new PubnubDemoMessage ().TryXmlDemo ();
	}

	[Serializable]
	class PubnubDemoMessage
	{
		public string DefaultMessage = "~!@#$%^&*()_+ `1234567890-= qwertyuiop[]\\ {}| asdfghjkl;' :\" zxcvbnm,./ <>? ";
		//public string DefaultMessage = "\"";
		public PubnubDemoMessage ()
		{
		}

		public PubnubDemoMessage (string message)
		{
			DefaultMessage = message;
		}

		public XmlDocument TryXmlDemo ()
		{
			XmlDocument xmlDocument = new XmlDocument ();
			xmlDocument.LoadXml ("<DemoRoot><Person ID='ABCD123'><Name><First>John</First><Middle>P.</Middle><Last>Doe</Last></Name><Address><Street>123 Duck Street</Street><City>New City</City><State>New York</State><Country>United States</Country></Address></Person><Person ID='ABCD456'><Name><First>Peter</First><Middle>Z.</Middle><Last>Smith</Last></Name><Address><Street>12 Hollow Street</Street><City>Philadelphia</City><State>Pennsylvania</State><Country>United States</Country></Address></Person></DemoRoot>");

			return xmlDocument;
		}
	}
	public class CommonIntergrationTests
	{
		public static string PublishKey = "demo-36";
		public static string SubscribeKey = "demo-36";
		public static string SecretKey = "demo-36";
		public static float WaitTimeBetweenCalls = 5;
		public static float WaitTimeToReadResponse = 15;
		public static float WaitTime = 20;
		Pubnub pubnub;

		public bool TimedOut {
			get;
			set;
		}

		public Pubnub SetPubnub{
			set{
				pubnub = value;
			}
		}

		public object Response { get; set; }

		public object Name { get; set; }

		public string ErrorResponse { get; set; }

		public bool DeliveryStatus  { get; set; }

		public void DisplayErrorMessage (PubnubClientError result)
		{
			ErrorResponse = result.Description;
			//DeliveryStatus = true;
			UnityEngine.Debug.Log ("DisplayErrorMessage:" + result.ToString ());
		}

		public void DisplayReturnMessageDummy (object result)
		{
			//deliveryStatus = true;
			//Response = result;
			ErrorResponse = result.ToString ();
			UnityEngine.Debug.Log ("DisplayReturnMessageDummy:" + result.ToString ());
		}

		public void DisplayReturnMessage (object result)
		{
			UnityEngine.Debug.Log ("DisplayReturnMessageO:" + result.ToString ());
			Response = result;
			DeliveryStatus = true;
		}

		public void DisplayReturnMessage (string result)
		{
			UnityEngine.Debug.Log ("DisplayReturnMessageS: " + Name + " " + result.ToString ());
			Response = (object)result.ToString ();
			DeliveryStatus = true;
		}

		private string Init(string testName, bool ssl, bool withCipher, bool secretEmpty){
			this.DeliveryStatus = false;
			this.TimedOut = false;
			this.Response = null;
			this.Name = testName;

			System.Random r = new System.Random ();
			string channel = "hello_world_hn" + r.Next (100);
			var cipher = "";
			if (withCipher) {
				cipher = "enigma";
			}
			if (!secretEmpty) {
				pubnub = new Pubnub (CommonIntergrationTests.PublishKey,
					CommonIntergrationTests.SubscribeKey,
					CommonIntergrationTests.SecretKey, 
					cipher, 
					ssl);
			} else {
				pubnub = new Pubnub (CommonIntergrationTests.PublishKey,
					CommonIntergrationTests.SubscribeKey);
			}

			return channel;
		}

		private string Init(string testName, bool ssl, bool withCipher){
			return Init (testName, ssl, withCipher, false);
		}

		private string Init(string testName, bool ssl){
			return Init (testName, ssl, false);
		}

		public IEnumerator DoPublishThenDetailedHistoryAndParse(bool ssl, string testName, object message, bool asObject, bool withCipher, bool noStore, int numberOfMessages){
			string channel = Init (testName, ssl, withCipher);

			CommonIntergrationTests commonPublish = new CommonIntergrationTests ();
			commonPublish.DeliveryStatus = false;
			commonPublish.Response = null;
			commonPublish.Name = string.Format("{0} Pubs", testName);

			UnityEngine.Debug.Log (string.Format("{0}: {1} Publishing", DateTime.Now.ToString (), testName));

			bool storeInHistory = true;
			if (noStore) {
				storeInHistory = false;
			}

			if (asObject) {
				pubnub.Publish<object> (channel, message, storeInHistory, commonPublish.DisplayReturnMessage, commonPublish.DisplayReturnMessage);
			}else {
				pubnub.Publish<string> (channel, message, storeInHistory, commonPublish.DisplayReturnMessage, commonPublish.DisplayReturnMessage);
			}
			UnityEngine.Debug.Log (string.Format("{0}: {1} After Publish", DateTime.Now.ToString (), testName));
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls); 

			if (asObject) {
				pubnub.DetailedHistory<object> (channel, numberOfMessages, this.DisplayReturnMessage, this.DisplayReturnMessageDummy);
			} else {
				pubnub.DetailedHistory<string> (channel, numberOfMessages, this.DisplayReturnMessage, this.DisplayReturnMessageDummy);
			}
			UnityEngine.Debug.Log (string.Format("{0}: {1} Waiting for response", DateTime.Now.ToString (), testName));
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls); 
			UnityEngine.Debug.Log (string.Format("{0}: {1} After wait", DateTime.Now.ToString (), testName));
			if (this.Response == null) {
				IntegrationTest.Fail (string.Format ("{0}: Null response", testName)); 
			} else {
				UnityEngine.Debug.Log (string.Format("{0}: {1} Response {2}", DateTime.Now.ToString (), testName, this.Response.ToString()));
				if (noStore) {
					ParseResponseNoStore (message.ToString(), testName);
				} else {
					ParseDetailedHistoryResponse (0, 0, message.ToString(), testName, asObject);
				}
			}
		}

		public void ParseDetailedHistoryResponse (int messageStart, int messageEnd, string message, string testName, bool asObject)
		{
			if (asObject) {
				IList<object> fields = this.Response as IList<object>;

				UnityEngine.Debug.Log (string.Format ("{0}: {1} fields.Count {2}", DateTime.Now.ToString (), testName, fields.Count));
				if (fields [0] != null) {
					ParseFields (fields, messageStart, messageEnd, message, testName);
				}
			} else {
				/*for (messageStart; messageStart <= messageEnd; messageStart++) {
					if (this.Response.ToString ().Contains (message)) {
					}
				}*/
				if (this.Response.ToString ().Contains (message)) {
					IntegrationTest.Pass ();
				} else {
					IntegrationTest.Fail (string.Format("{0}: Not found in {1}", testName, this.Response.ToString ())); 
				}
			}
		}

		public void ParseFields (IList<object> fields, int messageStart, int messageEnd, string message, string testName)
		{
			string response = "";

			var myObjectArray = (from item in fields
				select item as object).ToArray ();
			IList<object> enumerable = myObjectArray [0] as IList<object>;
			bool found = false;
			if ((enumerable != null) && (enumerable.Count > 0)) {
				int j = messageStart;
				foreach (object element in enumerable) {
					response = element.ToString ();
					if (messageStart != messageEnd) {
						Console.WriteLine (String.Format ("response :{0} :: j: {1}", response, j));
						if (j < messageEnd) {
							if(j.ToString ().Equals(response)){
								found = true;
								break;
							}
						}
						j++;
					} else if (!message.Equals ("")) {
						Console.WriteLine ("Response:" + response);
						if(message.Equals(response)){
							found = true;
							break;
						}
					} else {
						Console.WriteLine ("Response:" + response);
						//Assert.IsNotEmpty (response);
						if(!string.IsNullOrEmpty(response)){
							found = true;
							break;
						}
					}
				}
				if (found) {
					IntegrationTest.Pass ();
				} else {
					IntegrationTest.Fail (string.Format("{0}: Not found in {1}", testName, this.Response.ToString ())); 
				}
			} else {
				IntegrationTest.Fail (string.Format("{0}: {1}", testName, this.Response.ToString ())); 
			}
		}

		public void ParseResponseNoStore (string message, string testName)
		{
			if (!this.Response.ToString ().Contains (message)) {
				IntegrationTest.Pass (); 
			} else {
				IntegrationTest.Fail (string.Format("{0}: {1}", testName, this.Response.ToString ())); 
			}
		}


		public IEnumerator DoPublishAndParse(bool ssl, string testName, object message, string expected, bool asObject){
			string channel = Init (testName, ssl);

			if (asObject) {
				pubnub.Publish<object> (channel, message, this.DisplayReturnMessage, this.DisplayReturnMessage);
			}else {
				pubnub.Publish<string> (channel, message, this.DisplayReturnMessage, this.DisplayReturnMessage);
			}

			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls); 

			ParsePublishResponse (testName, expected, asObject);
		}

		public IEnumerator DoPublishAndParse(bool ssl, string testName, object message, string expected, bool asObject, bool withCipher, bool noSecretKey){
			string channel = "";
			if (noSecretKey) {
				channel = Init (testName, ssl, withCipher, true);
			} else {
				channel = Init (testName, ssl, withCipher);
			}

			if (asObject) {
				pubnub.Publish<object> (channel, message, this.DisplayReturnMessage, this.DisplayReturnMessage);
			}else {
				pubnub.Publish<string> (channel, message, this.DisplayReturnMessage, this.DisplayReturnMessage);
			}

			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls); 

			ParsePublishResponse (testName, expected, asObject);

		}

		public void ParsePublishResponse(string testName, string expected, bool asObject){
			if (this.Response == null) {
				IntegrationTest.Fail (string.Format ("{0}: Null response", testName)); 
			} else {
				bool found = false;
				UnityEngine.Debug.Log (string.Format("{0}: {1} Response {2}", DateTime.Now.ToString (), testName, this.Response.ToString ()));
				if (asObject) {
					IList<object> fields = this.Response as IList<object>;
					string sent = fields [1].ToString ();
					//string one = fields [0].ToString ();
					if (sent.Equals (expected)) {
						found = true;
					}
				} else {
					if (this.Response.ToString ().Contains (expected)) {
						found = true;
					}
				}

				if (found) {
					IntegrationTest.Pass (); 
				} else {
					IntegrationTest.Fail (string.Format("{0}: {1}", testName, this.Response.ToString ())); 
				}
			}

			pubnub.EndPendingRequests ();
		}

		public IEnumerator DoPublishAndParse(bool ssl, string testName, object message, string expected, bool asObject, bool withCipher){
			string channel = Init (testName, ssl, withCipher);

			if (asObject) {
				pubnub.Publish<object> (channel, message, this.DisplayReturnMessage, this.DisplayReturnMessage);
			}else {
				pubnub.Publish<string> (channel, message, this.DisplayReturnMessage, this.DisplayReturnMessage);
			}

			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls); 

			ParsePublishResponse (testName, expected, asObject);	
		}

		private void SubscribeUsingSeparateCommon(string channel, string testName){
			UnityEngine.Debug.Log (string.Format("{0} {1}: Running Subscribe ", DateTime.Now.ToString (), testName));
			CommonIntergrationTests commonSubscribe = new CommonIntergrationTests ();
			commonSubscribe.DeliveryStatus = false;
			commonSubscribe.Response = null;
			commonSubscribe.Name = string.Format("{0} Subs", testName);

			UnityEngine.Debug.Log (string.Format("{0} {1}: Start coroutine ", DateTime.Now.ToString (), testName));
			try {
				pubnub.Subscribe<string> (channel, commonSubscribe.DisplayReturnMessage, commonSubscribe.DisplayReturnMessage, commonSubscribe.DisplayErrorMessage);
			} catch (Exception ex) {
				UnityEngine.Debug.Log (string.Format("{0} {1}: exception ", ex.ToString (), testName));
			}
		}

		private void SetState(string channel, string testName, string state){
			UnityEngine.Debug.Log (string.Format("{0} {1}: Running Set State ", DateTime.Now.ToString (), testName));
			CommonIntergrationTests commonState = new CommonIntergrationTests ();
			commonState.DeliveryStatus = false;
			commonState.Response = null;
			commonState.Name = string.Format("{0} State", testName);

			pubnub.SetUserState<string> (channel, state, commonState.DisplayReturnMessage, commonState.DisplayErrorMessage);
		}

		public IEnumerator SetAndDeleteStateAndParse (bool ssl, string testName)
		{
			string channel = Init (testName, ssl);

			KeyValuePair<string, object> kvp = new KeyValuePair<string, object> ("k", "v");

			UnityEngine.Debug.Log (string.Format("{0} {1}: Running Set State ", DateTime.Now.ToString (), testName));
			CommonIntergrationTests commonState = new CommonIntergrationTests ();
			commonState.DeliveryStatus = false;
			commonState.Response = null;
			commonState.Name = string.Format("{0} State", testName);

			pubnub.SetUserState<string> (channel, kvp, commonState.DisplayReturnMessage, commonState.DisplayErrorMessage);
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

			KeyValuePair<string, object> kvp2 = new KeyValuePair<string, object> ("k2", "v2");
			commonState.DeliveryStatus = false;
			commonState.Response = null;

			pubnub.SetUserState<string> (channel, kvp2, commonState.DisplayReturnMessage, commonState.DisplayErrorMessage);
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls); 

			pubnub.GetUserState<string> (channel, this.DisplayReturnMessage, this.DisplayErrorMessage);
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls); 

			pubnub.SetUserState<string> (channel, new KeyValuePair<string, object> ("k2", null), commonState.DisplayReturnMessage, commonState.DisplayErrorMessage);
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls); 

			pubnub.GetUserState<string> (channel, this.DisplayReturnMessage, this.DisplayErrorMessage);
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls); 

			if (this.Response == null) {
				IntegrationTest.Fail (string.Format ("{0}: Null response", testName)); 
			} else {
				bool found = false;
				UnityEngine.Debug.Log (string.Format("{0}: {1} Response {2}", DateTime.Now.ToString (), testName, this.Response.ToString ()));
				if (this.Response.ToString ().Contains ("{\"k\":\"v\"}")) {
					found = true;
				}
				if (found) {
					IntegrationTest.Pass (); 
				} else {
					IntegrationTest.Fail (string.Format("{0}: {1}", testName, this.Response.ToString ())); 
				}
			}

			pubnub.EndPendingRequests ();
		}

		public IEnumerator SetAndGetStateAndParse (bool ssl, string testName)
		{
			string channel = Init (testName, ssl);

			string state = "{\"testkey\":\"testval\"}";
			SetState (channel, testName, state);
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls); 

			pubnub.GetUserState<string> (channel, this.DisplayReturnMessage, this.DisplayErrorMessage);

			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls); 

			if (this.Response == null) {
				IntegrationTest.Fail (string.Format ("{0}: Null response", testName)); 
			} else {
				bool found = false;
				UnityEngine.Debug.Log (string.Format("{0}: {1} Response {2}", DateTime.Now.ToString (), testName, this.Response.ToString ()));
				if (this.Response.ToString ().Contains (state)) {
					found = true;
				}
				if (found) {
					IntegrationTest.Pass (); 
				} else {
					IntegrationTest.Fail (string.Format("{0}: {1}", testName, this.Response.ToString ())); 
				}
			}

			pubnub.EndPendingRequests ();
		}

		public IEnumerator DoSubscribeThenDoGlobalHereNowAndParse (bool ssl, string testName, bool parseAsString)
		{
			string channel = Init (testName, ssl);

			SubscribeUsingSeparateCommon (channel, testName);

			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls); 

			UnityEngine.Debug.Log (string.Format("{0} {1}: Waiting ", DateTime.Now.ToString (), testName));
			if (parseAsString) {
				pubnub.GlobalHereNow<string> (true, true, this.DisplayReturnMessage, this.DisplayErrorMessage);
			} else {
				pubnub.GlobalHereNow<object> (true, true, this.DisplayReturnMessage, this.DisplayErrorMessage);
			}

			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

			UnityEngine.Debug.Log (string.Format("{0} {1}: After Wait ", DateTime.Now.ToString (), testName));
			if (this.Response == null) {
				IntegrationTest.Fail (string.Format("{0}: Null response", testName)); 
			} else {
				bool found = false;
				if (parseAsString) {
					if (this.Response.ToString ().Contains (pubnub.SessionUUID)
						&& this.Response.ToString ().Contains (channel)) {
						found = true;
					}
				} else {
					//TODO: refactor
					IList responseFields = this.Response as IList;
					UnityEngine.Debug.Log (string.Format ("{0}: responseFields: {1}", testName, responseFields.ToString ())); 
					if (responseFields.Count >= 1) {
						var item = responseFields [0];
						UnityEngine.Debug.Log (string.Format ("{0}: item: {1}", testName, item.ToString ())); 
						if (item is Dictionary<string, object>) {
							Dictionary<string, object> message = (Dictionary<string, object>)item;
							foreach (KeyValuePair<string, object> k in message) {
								//UnityEngine.Debug.Log (string.Format ("objs:{0} {1}", k.Key, k.Value));
								if (k.Key.Equals("payload")) {
									//UnityEngine.Debug.Log (string.Format ("in objs:{0} {1}", k.Key, k.Value));
									Dictionary<string, object> message2 = (Dictionary<string, object>)k.Value;
									//UnityEngine.Debug.Log (string.Format ("objs:{0} {1}", k.Key, message2 ["channels"]));
									Dictionary<string, object> message3 = (Dictionary<string, object>)message2 ["channels"];
									Dictionary<string, object> message4 = (Dictionary<string, object>)message3 [channel];
									if (message4 != null) {
										foreach (KeyValuePair<string, object> k2 in message4) {
											UnityEngine.Debug.Log (string.Format ("objs2:{0} {1}", k2.Key, k2.Value));
											if (k2.Key.Equals ("uuids")) {
												UnityEngine.Debug.Log (string.Format ("in objs2:{0} {1}", k2.Key, k2.Value));
												Dictionary<string, object>[] message5 = (Dictionary<string, object>[])k2.Value;
												var arr = message5 [0] as Dictionary<string, object>;
												foreach (KeyValuePair<string, object> k3 in arr) {
													UnityEngine.Debug.Log (string.Format ("objs3:{0} {1}", k3.Key, k3.Value));
													if (k3.Value.Equals (pubnub.SessionUUID)) {
														found = true;
														break;
													}
												}
											}
										}
									} else {
										UnityEngine.Debug.Log ("msg4 null");
									}
								}
							}
						}
					}
				}
				if (found) {
					IntegrationTest.Pass (); 
				} else {
					IntegrationTest.Fail (string.Format("{0}: {1}", testName, this.Response.ToString ())); 
				}
			}

			pubnub.Unsubscribe<string> (channel, this.DisplayReturnMessage, this.DisplayReturnMessage, this.DisplayReturnMessage, this.DisplayErrorMessage);
			pubnub.EndPendingRequests ();
		}

		public IEnumerator DoSubscribeThenDoWhereNowAndParse (bool ssl, string testName, bool parseAsString)
		{
			string channel = Init (testName, ssl);

			SubscribeUsingSeparateCommon (channel, testName);

			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls); 

			UnityEngine.Debug.Log (string.Format("{0} {1}: Waiting ", DateTime.Now.ToString (), testName));
			if (parseAsString) {
				pubnub.WhereNow<string> ("", this.DisplayReturnMessage, this.DisplayErrorMessage);
			} else {
				pubnub.WhereNow<object> ("", this.DisplayReturnMessage, this.DisplayErrorMessage);
			}

			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

			UnityEngine.Debug.Log (string.Format("{0} {1}: After Wait ", DateTime.Now.ToString (), testName));
			if (this.Response == null) {
				IntegrationTest.Fail (string.Format("{0}: Null response", testName)); 
			} else {
				bool found = false;
				if (parseAsString) {
					if (this.Response.ToString ().Contains (pubnub.SessionUUID)
					    && this.Response.ToString ().Contains (channel)) {
						found = true;
					}
				} else {
					//TODO: refactor
					IList responseFields = this.Response as IList;
					UnityEngine.Debug.Log (string.Format ("{0}: responseFields: {1}", testName, responseFields.ToString ())); 
					if (responseFields.Count >= 2) {
						var item = responseFields [0];
						UnityEngine.Debug.Log (string.Format ("{0}: item: {1}", testName, item.ToString ())); 
						if (item is Dictionary<string, object>) {
							Dictionary<string, object> message = (Dictionary<string, object>)item;
							foreach (KeyValuePair<string, object> k in message) {
								UnityEngine.Debug.Log (string.Format ("objs:{0} {1}", k.Key, k.Value));
								if (k.Key.Equals("payload")) {
									UnityEngine.Debug.Log (string.Format ("in objs:{0} {1}", k.Key, k.Value));
									Dictionary<string, object> message2 = (Dictionary<string, object>)k.Value;
									UnityEngine.Debug.Log (string.Format ("objs:{0} {1}", k.Key, message2 ["channels"]));
									var arr = message2 ["channels"] as string[];
									foreach (string ch in arr) {
										if (ch.Equals(channel)) {
											found = true;
											break;
										}
									}
								}
							}
						}
					}
				}
				if (found) {
					IntegrationTest.Pass (); 
				} else {
					IntegrationTest.Fail (string.Format("{0}: {1}", testName, this.Response.ToString ())); 
				}
			}

			pubnub.Unsubscribe<string> (channel, this.DisplayReturnMessage, this.DisplayReturnMessage, this.DisplayReturnMessage, this.DisplayErrorMessage);
			pubnub.EndPendingRequests ();
		}

		/*public IEnumerator DoSubscribeThenHereNowAsStringAndParse (bool ssl, string testName, bool parseAsString)
		{
			this.DeliveryStatus = false;
			this.TimedOut = false;
			this.Response = null;
			this.Name = testName;

			System.Random r = new System.Random ();
			string channel = "hello_world_hn" + r.Next (100);

			pubnub = new Pubnub (CommonIntergrationTests.PublishKey,
				CommonIntergrationTests.SubscribeKey,
				CommonIntergrationTests.SecretKey, 
				"", 
				ssl);

			UnityEngine.Debug.Log (string.Format("{0} {1}: Running Subscribe ", DateTime.Now.ToString (), testName));
			CommonIntergrationTests commonSubscribe = new CommonIntergrationTests ();
			commonSubscribe.DeliveryStatus = false;
			commonSubscribe.Response = null;
			commonSubscribe.Name = string.Format("{0} Subs", testName);

			UnityEngine.Debug.Log (string.Format("{0} {1}: Start coroutine ", DateTime.Now.ToString (), testName));
			try {
				pubnub.Subscribe<string> (channel, commonSubscribe.DisplayReturnMessage, commonSubscribe.DisplayReturnMessage, commonSubscribe.DisplayErrorMessage);
			} catch (Exception ex) {
				UnityEngine.Debug.Log (string.Format("{0} {1}: exception ", ex.ToString (), testName));
			}

			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls); 

			UnityEngine.Debug.Log (string.Format("{0} {1}: Waiting ", DateTime.Now.ToString (), testName));
			pubnub.HereNow<string> (channel, this.DisplayReturnMessage, this.DisplayErrorMessage);

			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

			UnityEngine.Debug.Log (string.Format("{0} {1}: After Wait ", DateTime.Now.ToString (), testName));
			if (this.Response == null) {
				IntegrationTest.Fail (string.Format("{0}: Null response", testName)); 
			} else {
				if (this.Response.ToString().Contains(pubnub.SessionUUID)) {
					IntegrationTest.Pass (); 
				} else {
					IntegrationTest.Fail (string.Format("{0}: {1}", testName, this.Response.ToString ())); 
				}
			}

			pubnub.Unsubscribe<string> (channel, this.DisplayReturnMessage, this.DisplayReturnMessage, this.DisplayReturnMessage, this.DisplayErrorMessage);
			pubnub.EndPendingRequests ();
		}*/

		public IEnumerator DoSubscribeThenHereNowAndParse (bool ssl, string testName, bool parseAsString, bool doWithState, string customUUID)
		{
			string channel = Init (testName, ssl);

			string matchUUID = pubnub.SessionUUID;
			if (!customUUID.Equals("")) {
				pubnub.SessionUUID = customUUID;
				matchUUID = customUUID;
			}

			SubscribeUsingSeparateCommon (channel, testName);

			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls); 

			UnityEngine.Debug.Log (string.Format("{0} {1}: Waiting ", DateTime.Now.ToString (), testName));
			string state = "{\"testkey\":\"testval\"}";
			if (doWithState) {
				SetState (channel, testName, state);
				yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls); 
				pubnub.HereNow<string> (channel, true, true, this.DisplayReturnMessage, this.DisplayErrorMessage);
			} else if (parseAsString) {
				pubnub.HereNow<string> (channel, this.DisplayReturnMessage, this.DisplayErrorMessage);
			} else {
				pubnub.HereNow<object> (channel, this.DisplayReturnMessage, this.DisplayErrorMessage);
			}

			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

			UnityEngine.Debug.Log (string.Format("{0} {1}: After Wait ", DateTime.Now.ToString (), testName));
			if (this.Response == null) {
				IntegrationTest.Fail (string.Format("{0}: Null response", testName)); 
			} else {
				bool found = false;
				if (doWithState) {
					if (this.Response.ToString ().Contains (matchUUID) && this.Response.ToString().Contains(state)) {
						found = true;
					} 
				} else if (parseAsString) {
					if (this.Response.ToString ().Contains (matchUUID)) {
						found = true;
					} 
				} else {

					IList responseFields = this.Response as IList;

					UnityEngine.Debug.Log (string.Format ("{0}: responseFields: {1}", testName, responseFields.ToString ())); 
					foreach (object item in responseFields) {
						UnityEngine.Debug.Log (string.Format ("{0}: item: {1}", testName, item.ToString ())); 
						if (item is Dictionary<string, object>) {
							Dictionary<string, object> message = (Dictionary<string, object>)item;
							#if (USE_MiniJSON)
						foreach (KeyValuePair<string, object> k in message) {
						UnityEngine.Debug.Log (string.Format ("objs:{0} {1}", k.Key, k.Value));
						if (k.Key.Equals ("uuids")) {
						found = ParseDict (pubnub, k.Value);
						}
						}
							#else
							if (message.ContainsKey ("uuids")) {
								object uuids = message ["uuids"];
								found = ParseDict (matchUUID, uuids);
							}
							#endif
						}
					}
				}
				if (found) {
					IntegrationTest.Pass (); 
				} else {
					IntegrationTest.Fail (string.Format("{0}: {1}", testName, this.Response.ToString ())); 
				}
				/*								foreach (object item in responseFields) {
                                        response = item.ToString ();
                                        Console.WriteLine ("Response:" + response);
                                        Assert.NotNull (response);
                                }
                                Dictionary<string, object> message = (Dictionary<string, object>)responseFields [0];
                                foreach (KeyValuePair<String, object> entry in message) {
                                        Console.WriteLine ("value:" + entry.Value + "  " + "key:" + entry.Key);
                                }*/

				/*object[] objUuid = (object[])message["uuids"];
				foreach (object obj in objUuid)
				{
					Console.WriteLine(obj.ToString()); 
				}*/
				//Assert.AreNotEqual(0, message["occupancy"]);
			}

			pubnub.Unsubscribe<string> (channel, this.DisplayReturnMessage, this.DisplayReturnMessage, this.DisplayReturnMessage, this.DisplayErrorMessage);
			pubnub.EndPendingRequests ();
		}

		bool ParseDict (string matchUUID, object uuids)
		{
			object[] objUuid = null;
			UnityEngine.Debug.Log ("uuids:" + uuids);
			Type valueType = uuids.GetType ();
			var expectedType = typeof(string[]);
			var expectedType2 = typeof(object[]);

			if (expectedType.IsAssignableFrom (valueType)) {
				objUuid = uuids as string[];
			} else if (expectedType2.IsAssignableFrom (valueType)) {
				objUuid = uuids as object[];
			} else if (uuids is IList && uuids.GetType ().IsGenericType) {
				objUuid = ((IEnumerable)uuids).Cast<object> ().ToArray ();
			} else {
				objUuid = CommonIntergrationTests.Deserialize<object[]> (uuids.ToString ());
			}
			foreach (object obj in objUuid) {
				UnityEngine.Debug.Log ("session:" + obj.ToString ()); 
				if (obj.Equals (matchUUID)) {
					return true;
				}
			}
			return false;
		}

		public static T Deserialize<T> (string message)
		{
			object retMessage;
			#if (USE_JSONFX) || (USE_JSONFX_UNITY)
			var reader = new JsonFx.Json.JsonReader ();
			retMessage = reader.Read<T> (message);
			#elif (USE_JSONFX_UNITY_IOS)
			UnityEngine.Debug.Log ("message: " + message);
			retMessage = JsonReader.Deserialize<T> (message);
			#elif (USE_MiniJSON)
			UnityEngine.Debug.Log("message: " + message);
			object retMessage1 = Json.Deserialize(message) as object;
			Type type = typeof(T);
			var expectedType2 = typeof(object[]);
			if(expectedType2.IsAssignableFrom(type)){
			retMessage = ((System.Collections.IEnumerable)retMessage1).Cast<object> ().ToArray ();
			} else {
			retMessage	= retMessage1;
			}
			#else
			retMessage = JsonConvert.DeserializeObject<T> (message);
			#endif
			return (T)retMessage;
		}

		public IEnumerator DoSubscribe (Pubnub pn, string channel, string testName)
		{
			CommonIntergrationTests commonSubscribe = new CommonIntergrationTests ();
			commonSubscribe.DeliveryStatus = false;
			commonSubscribe.Response = null;
			commonSubscribe.Name = string.Format("{0} Subs", testName);

			UnityEngine.Debug.Log (string.Format("{0} {1}: Start coroutine ", DateTime.Now.ToString (), testName));
			try {
				pn.Subscribe<string> (channel, commonSubscribe.DisplayReturnMessage, commonSubscribe.DisplayReturnMessage, commonSubscribe.DisplayErrorMessage);
			} catch (Exception ex) {
				UnityEngine.Debug.Log (string.Format("{0} {1}: exception ", ex.ToString (), testName));
			}

			UnityEngine.Debug.Log (string.Format("{0} {1}: Waiting ", DateTime.Now.ToString (), testName));
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls); 
		}

		public IEnumerator DoPresenceSubscribeAndParse (bool ssl, string testName)
		{
			/*CommonIntergrationTests common = new CommonIntergrationTests ();
			common.DeliveryStatus = false;
			common.TimedOut = false;
			common.Response = null;
			common.Name = "Pres";*/
			this.DeliveryStatus = false;
			this.TimedOut = false;
			this.Response = null;
			this.Name = testName;

			string channel = "hello_world2";

			pubnub = new Pubnub (CommonIntergrationTests.PublishKey,
				CommonIntergrationTests.SubscribeKey,
				CommonIntergrationTests.SecretKey, 
				"", 
				ssl);

			UnityEngine.Debug.Log (string.Format("{0} {1}: Start coroutine ", DateTime.Now.ToString (), testName));
			pubnub.Presence<string> (channel, this.DisplayReturnMessage, this.DisplayReturnMessage, this.DisplayErrorMessage);
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls); 
			CommonIntergrationTests commonSubscribe = new CommonIntergrationTests ();
			commonSubscribe.DeliveryStatus = false;
			commonSubscribe.Response = null;
			commonSubscribe.Name = string.Format("{0} Subs", testName);
			UnityEngine.Debug.Log (string.Format("{0} {1}: Running Subscribe ", DateTime.Now.ToString (), testName));
			try {
				pubnub.Subscribe<string> (channel, commonSubscribe.DisplayReturnMessage, commonSubscribe.DisplayReturnMessage, commonSubscribe.DisplayErrorMessage);
			} catch (Exception ex) {
				UnityEngine.Debug.Log (string.Format("{0} {1}: exception ", ex.ToString (), testName));
			}
			UnityEngine.Debug.Log (string.Format("{0} {1}: Waiting ", DateTime.Now.ToString (), testName));
			//yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeToReadResponse); 

			UnityEngine.Debug.Log (string.Format("{0} {1}: After Wait ", DateTime.Now.ToString (), testName));
			if (this.Response == null) {
				IntegrationTest.Fail (string.Format("{0}: Null response", testName)); 
			} else {
				UnityEngine.Debug.Log (string.Format("{0}: Response: {1}", testName, this.Response));
				object[] serializedMessage = pubnub.JsonPluggableLibrary.DeserializeToListOfObject (this.Response.ToString ()).ToArray ();
				Debug.Log (serializedMessage [0].ToString ());
				Debug.Log (serializedMessage [2].ToString ());

				if (channel.Equals (serializedMessage [2])) {
					UnityEngine.Debug.Log (string.Format("{0}: Pass", testName));
					IntegrationTest.Pass (); 
				} else {
					UnityEngine.Debug.Log (string.Format("{0}: Fail", testName));	
					IntegrationTest.Fail (string.Format("{0}: Channel not found", testName)); 
				}
			}

			pubnub.Unsubscribe<string> (channel, commonSubscribe.DisplayReturnMessage, commonSubscribe.DisplayReturnMessage, commonSubscribe.DisplayReturnMessage, commonSubscribe.DisplayErrorMessage);
			pubnub.PresenceUnsubscribe<string> (channel, commonSubscribe.DisplayReturnMessage, commonSubscribe.DisplayReturnMessage, commonSubscribe.DisplayReturnMessage, commonSubscribe.DisplayErrorMessage);
			pubnub.EndPendingRequests ();

		}
	}
}