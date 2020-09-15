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
        [Test]
         public void TestCBORTokensCommon ()
        {
            string t1 = "p0F2AkF0Gl2AX-JDdHRsCkNyZXOkRGNoYW6gQ2dycKBDdXNyoWl1LTMzNTIwNTUPQ3NwY6Fpcy0xNzA3OTgzGB9DcGF0pERjaGFuoENncnCgQ3VzcqBDc3BjoERtZXRhoENzaWdYINqGs2EyEMHPZrp6znVqTBzXNBAD_31hUH3JuUSWE2A6";
	        string t2 = "p0F2AkF0Gl2AaMlDdHRsCkNyZXOkRGNoYW6gQ2dycKBDdXNyoWl1LTE5NzQxMDcPQ3NwY6Fpcy0yMzExMDExGB9DcGF0pERjaGFuoENncnCgQ3VzcqBDc3BjoERtZXRhoENzaWdYIO1ti19DLbEKK-s_COJPlM1xtZCpP8K4sV51nvRPTIxf";
	        string t3 = "p0F2AkF0Gl2CEiRDdHRsA0NyZXOkRGNoYW6gQ2dycKBDdXNyoW50ZXN0dXNlcl8xNjY2ORgfQ3NwY6FvdGVzdHNwYWNlXzE1MDExGB9DcGF0pERjaGFuoENncnCgQ3VzcqBDc3BjoERtZXRhoENzaWdYIMqDoIOYPP9ULfXKLDK3eoGQ-C8nJxPTWFCDAc-Flxu7";
	        string t4 = "p0F2AkF0Gl2CEiVDdHRsA0NyZXOkRGNoYW6gQ2dycKBDdXNyoENzcGOgQ3BhdKREY2hhbqBDZ3JwoEN1c3KhY14uKhgfQ3NwY6FjXi4qGB9EbWV0YaBDc2lnWCDfqMStM0r1GgghNjt1MPeSaA0ADTw6aGsuQgMT3jYylg==";

            PNConfiguration pnConfiguration = new PNConfiguration ();

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            pnConfiguration.StoreTokensOnGrant = true;
            pnConfiguration.SecretKey = "";

            PubNub pn = new PubNub(pnConfiguration);

            pn.SetTokens(new List<string>{t1, t2, t3, t4});

            GrantResourcesWithPermissions g = pn.GetTokens();
            Assert.IsTrue(g.Channels.Count.Equals(0));
            Assert.IsTrue(g.Groups.Count.Equals(0));
            Assert.IsTrue(g.ChannelsPattern.Count.Equals(0));
            Assert.IsTrue(g.GroupsPattern.Count.Equals(0));
            foreach(KeyValuePair<string, UserSpacePermissionsWithToken> kvp in g.Users){
                Debug.Log(kvp.Key + "===>" + kvp.Value);
                UserSpacePermissionsWithToken u;
                
                if(g.Users.TryGetValue(kvp.Key, out u)){
                    Debug.Log(kvp.Key + "=======>" + u.Token);
                }
            }
            UserSpacePermissionsWithToken u1;
            if(g.Users.TryGetValue("testuser_16669", out u1)){
                Debug.Log("testuser_16669 =======>" + u1.Token);
            } else {
                Debug.Log("testuser_16669 not found");
            }
            Debug.Log(g.Users.ContainsKey("testuser_16669"));
            Debug.Log(g.Users.ContainsKey("u-1974107"));
            Debug.Log(g.Users.ContainsKey("u-3352055"));
            Assert.IsTrue(g.Users["testuser_16669"].BitMaskPerms.Equals(31));
            Assert.IsTrue(g.Users["testuser_16669"].TTL.Equals(3));
            Assert.IsTrue(g.Users["testuser_16669"].Timestamp.Equals(1568805412));

            Assert.IsTrue(g.Users["testuser_16669"].Token.Equals(t3));
            Assert.IsTrue(g.Users["testuser_16669"].Permissions.Read.Equals(true));
            Assert.IsTrue(g.Users["testuser_16669"].Permissions.Write.Equals(true));
            Assert.IsTrue(g.Users["testuser_16669"].Permissions.Delete.Equals(true));
            Assert.IsTrue(g.Users["testuser_16669"].Permissions.Create.Equals(true));
            Assert.IsTrue(g.Users["testuser_16669"].Permissions.Manage.Equals(true));

            Assert.IsTrue(g.Spaces["testspace_15011"].Token.Equals(t3));
            Assert.IsTrue(g.Spaces["testspace_15011"].Permissions.Read.Equals(true));
            Assert.IsTrue(g.Spaces["testspace_15011"].Permissions.Write.Equals(true));
            Assert.IsTrue(g.Spaces["testspace_15011"].Permissions.Delete.Equals(true));
            Assert.IsTrue(g.Spaces["testspace_15011"].Permissions.Create.Equals(true));
            Assert.IsTrue(g.Spaces["testspace_15011"].Permissions.Manage.Equals(true));

            Assert.IsTrue(g.Users["u-1974107"].Token.Equals(t2));
            Assert.IsTrue(g.Spaces["s-1707983"].Token.Equals(t1));

            Assert.IsTrue(g.UsersPattern["^.*"].Token.Equals(t4));
            Assert.IsTrue(g.SpacesPattern["^.*"].Token.Equals(t4));

            GrantResourcesWithPermissions g2 = pn.GetTokensByResource(PNResourceType.PNUUIDMetadata);
            Assert.IsTrue(g2.Users["testuser_16669"].BitMaskPerms.Equals(31));
            Assert.IsTrue(g2.Users["testuser_16669"].TTL.Equals(3));
            Assert.IsTrue(g2.Users["testuser_16669"].Timestamp.Equals(1568805412));

            Assert.IsTrue(g2.Users["testuser_16669"].Token.Equals(t3));
            Assert.IsTrue(g2.Users["testuser_16669"].Permissions.Read.Equals(true));
            Assert.IsTrue(g2.Users["testuser_16669"].Permissions.Write.Equals(true));
            Assert.IsTrue(g2.Users["testuser_16669"].Permissions.Delete.Equals(true));
            Assert.IsTrue(g2.Users["testuser_16669"].Permissions.Create.Equals(true));
            Assert.IsTrue(g2.Users["testuser_16669"].Permissions.Manage.Equals(true));
            Assert.IsTrue(g2.Users["u-1974107"].Token.Equals(t2));
            Assert.IsTrue(g2.UsersPattern["^.*"].Token.Equals(t4));

            GrantResourcesWithPermissions g3 = pn.GetTokensByResource(PNResourceType.PNChannelMetadata);
            Assert.IsTrue(g3.Spaces["testspace_15011"].BitMaskPerms.Equals(31));
            Assert.IsTrue(g3.Spaces["testspace_15011"].TTL.Equals(3));
            Assert.IsTrue(g3.Spaces["testspace_15011"].Timestamp.Equals(1568805412));

            Assert.IsTrue(g3.Spaces["testspace_15011"].Token.Equals(t3));
            Assert.IsTrue(g3.Spaces["testspace_15011"].Permissions.Read.Equals(true));
            Assert.IsTrue(g3.Spaces["testspace_15011"].Permissions.Write.Equals(true));
            Assert.IsTrue(g3.Spaces["testspace_15011"].Permissions.Delete.Equals(true));
            Assert.IsTrue(g3.Spaces["testspace_15011"].Permissions.Create.Equals(true));
            Assert.IsTrue(g3.Spaces["testspace_15011"].Permissions.Manage.Equals(true));
            Assert.IsTrue(g3.Spaces["s-1707983"].Token.Equals(t1));
            Assert.IsTrue(g3.SpacesPattern["^.*"].Token.Equals(t4));   

            string g4 = pn.GetToken("testspace_15011", PNResourceType.PNChannelMetadata);
            Debug.Log("g4" + g4);
            Assert.IsTrue(g4.Equals(t3));
            string g5 = pn.GetToken("testuser_16669", PNResourceType.PNUUIDMetadata);
            Assert.IsTrue(g5.Equals(t3));
            string g6 = pn.GetToken("^.*", PNResourceType.PNChannelMetadata);
            Assert.IsTrue(g6.Equals(t4));
            string g7 = pn.GetToken("^.*", PNResourceType.PNUUIDMetadata);
            Assert.IsTrue(g7.Equals(t4));
            string g8 = pn.GetToken("NONEXISTENT", PNResourceType.PNChannelMetadata);
            Assert.IsTrue(g8.Equals(t4));
            string g9 = pn.GetToken("NONEXISTENT", PNResourceType.PNUUIDMetadata);
            Assert.IsTrue(g9.Equals(t4));

        }
    }
}