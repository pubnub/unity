#define USE_JSONFX_UNITY_IOS
using System;
using PubNubMessaging.Core;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

		public IEnumerator DoSubscribeHereNowAndParse (bool ssl, string testName)
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
				false);

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

			UnityEngine.Debug.Log (string.Format("{0} {1}: Waiting ", DateTime.Now.ToString (), testName));
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls); 

			pubnub.HereNow (channel, this.DisplayReturnMessage, this.DisplayErrorMessage);

			UnityEngine.Debug.Log (string.Format("{0} {1}: After Wait ", DateTime.Now.ToString (), testName));
			if (this.Response.Equals (null)) {
				IntegrationTest.Fail (string.Format("{0}: Null response", testName)); 
			} else {
				bool found = false;
				IList<object> responseFields = this.Response as IList<object>;
				UnityEngine.Debug.Log (string.Format("{0}: responseFields: {1}", testName, responseFields.ToString ())); 
				foreach (object item in responseFields) {
					UnityEngine.Debug.Log (string.Format("{0}: item: {1}", testName, item.ToString ())); 
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
							found = ParseDict (pubnub, uuids);
						}
						#endif
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

		public void DoSubscribeHereNowAndParse (bool ssl, string testName, string channel, Pubnub pn)
		{
			this.DeliveryStatus = false;
			this.TimedOut = false;
			this.Response = null;
			this.Name = testName;

			pn.HereNow (channel, this.DisplayReturnMessage, this.DisplayErrorMessage);

			UnityEngine.Debug.Log (string.Format("{0} {1}: After Wait ", DateTime.Now.ToString (), testName));
			if (this.Response.Equals (null)) {
				IntegrationTest.Fail (string.Format("{0}: Null response", testName)); 
			} else {
				bool found = false;
				IList<object> responseFields = this.Response as IList<object>;
				UnityEngine.Debug.Log (string.Format("{0}: responseFields: {1}", testName, responseFields.ToString ())); 
				foreach (object item in responseFields) {
					UnityEngine.Debug.Log (string.Format("{0}: item: {1}", testName, item.ToString ())); 
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
							found = ParseDict (pubnub, uuids);
						}
						#endif
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

			pn.Unsubscribe<string> (channel, this.DisplayReturnMessage, this.DisplayReturnMessage, this.DisplayReturnMessage, this.DisplayErrorMessage);
			pn.EndPendingRequests ();
		}

		bool ParseDict (Pubnub pubnub, object uuids)
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
				if (obj.Equals (pubnub.SessionUUID)) {
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