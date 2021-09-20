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
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add("userid9778", 31);
            d.Add("userid2043", 31);
            d.Add("spaceid9778", 31);
            d.Add("spaceid2043", 31);
            d.Add("UnityTestConnectedUUID_8602", 31);
            TestCBORTokensCommon(d, "p0F2AkF0GmFB9CFDdHRsA0NyZXOlRGNoYW6ianVzZXJpZDk3NzgYH2p1c2VyaWQyMDQzGB9DZ3JwomtzcGFjZWlkOTc3OBgfa3NwYWNlaWQyMDQzGB9DdXNyoENzcGOgRHV1aWSheBtVbml0eVRlc3RDb25uZWN0ZWRVVUlEXzg2MDIYH0NwYXSlRGNoYW6gQ2dycKBDdXNyoENzcGOgRHV1aWSgRG1ldGGgQ3NpZ1gghOCkA7lSx9r7uM78UQLehCMbxauzzrTRJAUOWI0m4J8=");
        }
        [Test]
        public void TestCBORTokens2 ()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add("userid5539", 31);
            d.Add("userid6719", 31);
            d.Add("spaceid5539", 31);
            d.Add("spaceid6719", 31);
            d.Add("UnityTestConnectedUUID_1220", 31);
            TestCBORTokensCommon(d, "qEF2AkF0GmFCDRZDdHRsA0NyZXOlRGNoYW6ianVzZXJpZDU1MzkYH2p1c2VyaWQ2NzE5GB9DZ3JwomtzcGFjZWlkNTUzORgfa3NwYWNlaWQ2NzE5GB9DdXNyoENzcGOgRHV1aWSheBtVbml0eVRlc3RDb25uZWN0ZWRVVUlEXzEyMjAYH0NwYXSlRGNoYW6gQ2dycKBDdXNyoENzcGOgRHV1aWSgRG1ldGGgRHV1aWR4G1VuaXR5VGVzdENvbm5lY3RlZFVVSURfMTIyMENzaWdYIGfRv7AlWGyE20gOABvD9dt4qpg-KvEq2etivD3GpSlr");
        }
        [Test]
        public void TestCBORTokens3 ()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add("userid3412", 31);
            d.Add("userid7657", 31);
            d.Add("spaceid3412", 31);
            d.Add("spaceid7657", 31);
            d.Add("UnityTestConnectedUUID_2913", 31);
            TestCBORTokensCommon(d, "qEF2AkF0GmFCHV9DdHRsA0NyZXOlRGNoYW6ianVzZXJpZDM0MTIYH2p1c2VyaWQ3NjU3GB9DZ3JwomtzcGFjZWlkMzQxMhgfa3NwYWNlaWQ3NjU3GB9DdXNyoENzcGOgRHV1aWSheBtVbml0eVRlc3RDb25uZWN0ZWRVVUlEXzI5MTMYH0NwYXSlRGNoYW6gQ2dycKBDdXNyoENzcGOgRHV1aWSgRG1ldGGgRHV1aWR4G1VuaXR5VGVzdENvbm5lY3RlZFVVSURfMjkxM0NzaWdYILH6RRtdwpp1qpUVtPhuZLnn8tcZeuZucbWzSXZt2i0b");
        }
        [Test]
        public void TestCBORTokens4 ()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add("userid8078", 31);
            d.Add("userid1207", 239);
            d.Add("spaceid8078", 103);
            d.Add("spaceid1207", 231);
            d.Add("UnityTestConnectedUUID_8339", 215);
            TestCBORTokensCommon(d, "qEF2AkF0GmFCKBFDdHRsA0NyZXOlRGNoYW6ianVzZXJpZDgwNzgYH2p1c2VyaWQxMjA3GO9DZ3JwomtzcGFjZWlkODA3OBhna3NwYWNlaWQxMjA3GOdDdXNyoENzcGOgRHV1aWSheBtVbml0eVRlc3RDb25uZWN0ZWRVVUlEXzgzMzkY10NwYXSlRGNoYW6gQ2dycKBDdXNyoENzcGOgRHV1aWSgRG1ldGGgRHV1aWR4G1VuaXR5VGVzdENvbm5lY3RlZFVVSURfODMzOUNzaWdYICDs_hNBIBXto9MMp94oxxhusEfHQpZkYBG6WzjRjS-Y");
        }
        
       
        public void TestCBORTokensCommon (Dictionary<string, int> d, string token)
        {
            PNConfiguration pnConfiguration = new PNConfiguration ();

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            pnConfiguration.StoreTokensOnGrant = true;
            pnConfiguration.SecretKey = "";

            Debug.Log("Token: " + token);
            pnUnity.Token = token;
            var p  = TokenHelpers.ParseToken(pnUnity, token);
            Debug.Log("TTL: " + p.TTL);
            Debug.Log("Version: " + p.Version);
            Debug.Log("Timestamp: " + p.Timestamp);
            Debug.Log("Meta: " + p.Meta);
            Debug.Log("AuthorizedUUID: " + p.AuthorizedUUID);
            Debug.Log("Signature: " + p.Signature);
            foreach(KeyValuePair<string, TokenAuthValues> kvp in p.Patterns.Channels){
                Debug.Log(string.Format("Patterns Channels: key {0}, val {1}", kvp.Key, kvp.Value));
                StringBuilder sbLog = TokenHelpers.PrintTokenPermissions(kvp.Value);
                if(d.ContainsKey(kvp.Key)){
                    Assert.AreEqual(d[kvp.Key], TokenHelpers.PermissionsMapping(kvp.Value), "Permission mismatch");
                }
                Debug.Log(sbLog.ToString());
            }
            foreach(KeyValuePair<string, TokenAuthValues> kvp in p.Patterns.Groups){
                Debug.Log(string.Format("Patterns Groups: key {0}, val {1}", kvp.Key, kvp.Value));
                StringBuilder sbLog = TokenHelpers.PrintTokenPermissions(kvp.Value);
                if(d.ContainsKey(kvp.Key)){
                    Assert.AreEqual(d[kvp.Key], TokenHelpers.PermissionsMapping(kvp.Value), "Permission mismatch");
                }
                Debug.Log(sbLog.ToString());
            }
            foreach(KeyValuePair<string, TokenAuthValues> kvp in p.Patterns.Users){
                Debug.Log(string.Format("Patterns Users: key {0}, val {1}", kvp.Key, kvp.Value));
                StringBuilder sbLog = TokenHelpers.PrintTokenPermissions(kvp.Value);                
                if(d.ContainsKey(kvp.Key)){
                    Assert.AreEqual(d[kvp.Key], TokenHelpers.PermissionsMapping(kvp.Value), "Permission mismatch");
                }
                Debug.Log(sbLog.ToString());
            }
            foreach(KeyValuePair<string, TokenAuthValues> kvp in p.Patterns.Spaces){
                Debug.Log(string.Format("Patterns Spaces: key {0}, val {1}", kvp.Key, kvp.Value));
                StringBuilder sbLog = TokenHelpers.PrintTokenPermissions(kvp.Value);
                if(d.ContainsKey(kvp.Key)){
                    Assert.AreEqual(d[kvp.Key], TokenHelpers.PermissionsMapping(kvp.Value), "Permission mismatch");
                }
                Debug.Log(sbLog.ToString());
            }
            foreach(KeyValuePair<string, TokenAuthValues> kvp in p.Patterns.UUIDs){
                Debug.Log(string.Format("Patterns UUIDs: key {0}, val {1}", kvp.Key, kvp.Value));
                StringBuilder sbLog = TokenHelpers.PrintTokenPermissions(kvp.Value);
                if(d.ContainsKey(kvp.Key)){
                    Assert.AreEqual(d[kvp.Key], TokenHelpers.PermissionsMapping(kvp.Value), "Permission mismatch");
                }
                Debug.Log(sbLog.ToString());
            }
            foreach(KeyValuePair<string, TokenAuthValues> kvp in p.Resources.Channels){
                Debug.Log(string.Format("Resources Channels: key {0}, val {1}", kvp.Key, kvp.Value));
                StringBuilder sbLog = TokenHelpers.PrintTokenPermissions(kvp.Value);
                if(d.ContainsKey(kvp.Key)){
                    Assert.AreEqual(d[kvp.Key], TokenHelpers.PermissionsMapping(kvp.Value), "Permission mismatch");
                }
                Debug.Log(sbLog.ToString());
            }
            foreach(KeyValuePair<string, TokenAuthValues> kvp in p.Resources.Groups){
                Debug.Log(string.Format("Resources Groups: key {0}, val {1}", kvp.Key, kvp.Value));
                StringBuilder sbLog = TokenHelpers.PrintTokenPermissions(kvp.Value);
                if(d.ContainsKey(kvp.Key)){
                    Assert.AreEqual(d[kvp.Key], TokenHelpers.PermissionsMapping(kvp.Value), "Permission mismatch");
                }
                Debug.Log(sbLog.ToString());
            }
            foreach(KeyValuePair<string, TokenAuthValues> kvp in p.Resources.Spaces){
                Debug.Log(string.Format("Resources Spaces: key {0}, val {1}", kvp.Key, kvp.Value));
                StringBuilder sbLog = TokenHelpers.PrintTokenPermissions(kvp.Value);
                if(d.ContainsKey(kvp.Key)){
                    Assert.AreEqual(d[kvp.Key], TokenHelpers.PermissionsMapping(kvp.Value), "Permission mismatch");
                }
                Debug.Log(sbLog.ToString());
            }
            foreach(KeyValuePair<string, TokenAuthValues> kvp in p.Resources.Users){
                Debug.Log(string.Format("Resources Users: key {0}, val {1}", kvp.Key, kvp.Value));
                StringBuilder sbLog = TokenHelpers.PrintTokenPermissions(kvp.Value);
                if(d.ContainsKey(kvp.Key)){
                    Assert.AreEqual(d[kvp.Key], TokenHelpers.PermissionsMapping(kvp.Value), "Permission mismatch");
                }
                Debug.Log(sbLog.ToString());
            }
            foreach(KeyValuePair<string, TokenAuthValues> kvp in p.Resources.UUIDs){
                Debug.Log(string.Format("Resources UUIDs: key {0}, val {1}", kvp.Key, kvp.Value));
                StringBuilder sbLog = TokenHelpers.PrintTokenPermissions(kvp.Value);
                if(d.ContainsKey(kvp.Key)){
                    Assert.AreEqual(d[kvp.Key], TokenHelpers.PermissionsMapping(kvp.Value), "Permission mismatch");
                }
                Debug.Log(sbLog.ToString());
            }

        }
        
    }
}