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
        public void TestCBORTokens1 ()
        {
            TestCBORTokensCommon("p0F2AkF0Gl2AX-JDdHRsCkNyZXOkRGNoYW6gQ2dycKBDdXNyoWl1LTMzNTIwNTUPQ3NwY6Fpcy0xNzA3OTgzGB9DcGF0pERjaGFuoENncnCgQ3VzcqBDc3BjoERtZXRhoENzaWdYINqGs2EyEMHPZrp6znVqTBzXNBAD_31hUH3JuUSWE2A6");
        }
        [Test]
        public void TestCBORTokens2 ()
        {
            TestCBORTokensCommon("p0F2AkF0Gl2AaMlDdHRsCkNyZXOkRGNoYW6gQ2dycKBDdXNyoWl1LTE5NzQxMDcPQ3NwY6Fpcy0yMzExMDExGB9DcGF0pERjaGFuoENncnCgQ3VzcqBDc3BjoERtZXRhoENzaWdYIO1ti19DLbEKK-s_COJPlM1xtZCpP8K4sV51nvRPTIxf");
        }
        [Test]
        public void TestCBORTokens3 ()
        {
            TestCBORTokensCommon("p0F2AkF0Gl2CEiRDdHRsA0NyZXOkRGNoYW6gQ2dycKBDdXNyoW50ZXN0dXNlcl8xNjY2ORgfQ3NwY6FvdGVzdHNwYWNlXzE1MDExGB9DcGF0pERjaGFuoENncnCgQ3VzcqBDc3BjoERtZXRhoENzaWdYIMqDoIOYPP9ULfXKLDK3eoGQ-C8nJxPTWFCDAc-Flxu7");
        }
        [Test]
        public void TestCBORTokens4 ()
        {
            TestCBORTokensCommon("p0F2AkF0Gl2CEiVDdHRsA0NyZXOkRGNoYW6gQ2dycKBDdXNyoENzcGOgQ3BhdKREY2hhbqBDZ3JwoEN1c3KhY14uKhgfQ3NwY6FjXi4qGB9EbWV0YaBDc2lnWCDfqMStM0r1GgghNjt1MPeSaA0ADTw6aGsuQgMT3jYylg==");
        }
        public void TestCBORTokensCommon (string token)
        {
            PNConfiguration pnConfiguration = new PNConfiguration ();

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            pnConfiguration.StoreTokensOnGrant = true;
            pnConfiguration.SecretKey = "";

            Debug.Log("Token: " + token);
            TokenManager tm = new TokenManager(pnUnity);
            tm.StoreToken(token);
            var p  = tm.GetPermissions(token);
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