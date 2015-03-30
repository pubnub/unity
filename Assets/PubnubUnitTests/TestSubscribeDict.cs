using System;
using UnityTest;
using UnityEngine;
using Pathfinding.Serialization.JsonFx;
using PubNubMessaging.Core;
using System.Collections;
using System.Collections.Generic;

namespace PubNubMessaging.Tests
{
	public class TestSubscribeDict: MonoBehaviour
	{
		public bool SslOn = false;
		public bool CipherOn = false;
		public bool AsObject = false;
		public IEnumerator Start ()
		{
			Dictionary<string, long> Message = new Dictionary<string, long>();
			Message.Add("cat", 14255515120803306);
			CommonIntergrationTests common = new CommonIntergrationTests ();
			yield return StartCoroutine(common.DoSubscribeThenPublishAndParse(SslOn, this.name, AsObject, CipherOn, Message, "\"cat\":\"14255515120803306\"", true));
			UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", this.name));
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);
		}
	}
}

