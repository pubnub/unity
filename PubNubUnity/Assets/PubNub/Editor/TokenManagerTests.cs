using System;
using PubNubAPI;
using NUnit.Framework;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI.Tests
{
    [TestFixture]
    public class TokenManagerTests
    {
        [Test]
        public void TestBuildTimeRequest ()
        {
            PNConfiguration pnConfiguration = new PNConfiguration ();

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            TokenManager tm = new TokenManager(pnUnity);
            var p  = tm.GetPermissions("p0F2AkF0Gl2AX-JDdHRsCkNyZXOkRGNoYW6gQ2dycKBDdXNyoWl1LTMzNTIwNTUPQ3NwY6Fpcy0xNzA3OTgzGB9DcGF0pERjaGFuoENncnCgQ3VzcqBDc3BjoERtZXRhoENzaWdYINqGs2EyEMHPZrp6znVqTBzXNBAD_31hUH3JuUSWE2A6");
            Debug.Log("TTL: " + p.TTL);
            Debug.Log("Version: " + p.Version);
            Debug.Log("Timestamp: " + p.Timestamp);
            Debug.Log("Meta: " + p.Meta);
            Debug.Log("Signature: " + System.Text.Encoding.ASCII.GetString(p.Signature));
            foreach(KeyValuePair<string, int> kvp in p.Patterns.Channels){
                Debug.Log(string.Format("Patterns Channels: key {0}, val {1}", kvp.Key, kvp.Value));
            }
            foreach(KeyValuePair<string, int> kvp in p.Patterns.Groups){
                Debug.Log(string.Format("Patterns Groups: key {0}, val {1}", kvp.Key, kvp.Value));
            }
            foreach(KeyValuePair<string, int> kvp in p.Patterns.Users){
                Debug.Log(string.Format("Patterns Users: key {0}, val {1}", kvp.Key, kvp.Value));
            }
            foreach(KeyValuePair<string, int> kvp in p.Patterns.Spaces){
                Debug.Log(string.Format("Patterns Spaces: key {0}, val {1}", kvp.Key, kvp.Value));
            }
            foreach(KeyValuePair<string, int> kvp in p.Resources.Channels){
                Debug.Log(string.Format("Resources Channels: key {0}, val {1}", kvp.Key, kvp.Value));
            }
            foreach(KeyValuePair<string, int> kvp in p.Resources.Groups){
                Debug.Log(string.Format("Resources Groups: key {0}, val {1}", kvp.Key, kvp.Value));
            }
            foreach(KeyValuePair<string, int> kvp in p.Resources.Spaces){
                Debug.Log(string.Format("Resources Spaces: key {0}, val {1}", kvp.Key, kvp.Value));
            }
            foreach(KeyValuePair<string, int> kvp in p.Resources.Users){
                Debug.Log(string.Format("Resources Users: key {0}, val {1}", kvp.Key, kvp.Value));
            }
        }
    }
}