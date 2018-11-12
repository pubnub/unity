using System;
using PubNubAPI;
using NUnit.Framework;
using System.Text;

namespace PubNubAPI.Tests
{
    [TestFixture]
    public class SubscribeBuildRequestsTests
    {
        #if DEBUG    
        [Test]
        public void TestBuildSubscribeRequest ()
        {
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, "", "", false, "");
        }

        [Test]
        public void TestBuildSubscribeRequestSSL ()
        {
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, "", "", true, "");
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannel ()
        {
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, "", "", false, "");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannel ()
        {
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, "", "", true, "");
        }

        [Test]
        public void TestBuildSubscribeRequestTT ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, timetoken, "", false, "");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLTT ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, timetoken, "", true, "");
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelTT ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, timetoken, "", false, "");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelTT ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, timetoken, "", true, "");
        }

        [Test]
        public void TestBuildSubscribeRequestState ()
        {
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, "", userState, false, "");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLState ()
        {
            string[] channels = { "test" };
            string userState = "{\"k\":\"v\"}";
            TestBuildSubscribeRequestCommon (channels, "", userState, true, "");
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelState ()
        {
            string[] channels = { "test", "test2" };
            string userState = "{\"k\":\"v\"}";
            TestBuildSubscribeRequestCommon (channels, "", userState, false, "");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelState ()
        {
            string[] channels = { "test", "test2" };
            string userState = "{\"k\":\"v\"}";
            TestBuildSubscribeRequestCommon (channels, "", userState, true, "");
        }

        [Test]
        public void TestBuildSubscribeRequestTTState ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, timetoken, userState, false, "");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLTTState ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, timetoken, userState, true, "");
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelTTState ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, timetoken, userState, false, "");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelTTState ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, timetoken, userState, true, "");
        }

        [Test]
        public void TestBuildSubscribeRequestAuth ()
        {
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, "", "", false, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLAuth ()
        {
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, "", "", true, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelAuth ()
        {
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, "", "", false, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelAuth ()
        {
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, "", "", true, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestTTAuth ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, timetoken, "", false, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLTTAuth ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, timetoken, "", true, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelTTAuth ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, timetoken, "", false, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelTTAuth ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, timetoken, "", true, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestStateAuth ()
        {
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, "", userState, false, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLStateAuth ()
        {
            string[] channels = { "test" };
            string userState = "{\"k\":\"v\"}";
            TestBuildSubscribeRequestCommon (channels, "", userState, true, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelStateAuth ()
        {
            string[] channels = { "test", "test2" };
            string userState = "{\"k\":\"v\"}";
            TestBuildSubscribeRequestCommon (channels, "", userState, false, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelStateAuth ()
        {
            string[] channels = { "test", "test2" };
            string userState = "{\"k\":\"v\"}";
            TestBuildSubscribeRequestCommon (channels, "", userState, true, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestTTStateAuth ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, timetoken, userState, false, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLTTStateAuth ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, timetoken, userState, true, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelTTStateAuth ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, timetoken, userState, false, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelTTStateAuth ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, timetoken, userState, true, "authKey");
        }

        //======================

        [Test]
        public void TestBuildSubscribeRequestCGFliterExp ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";

            TestBuildSubscribeRequestCommon (channels, channelGroups, "", 
                "", false, "", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLCGFliterExp ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";

            TestBuildSubscribeRequestCommon (channels, channelGroups, "", 
                "", true, "", filterExpr, "", 0);
            
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelCGFliterExp ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";

            TestBuildSubscribeRequestCommon (channels, channelGroups, "", 
                "", false, "", filterExpr, "", 0);
            

        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelCGFliterExp ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";

            TestBuildSubscribeRequestCommon (channels, channelGroups, "", 
                "", true, "", filterExpr, "", 0);
            
        }

        [Test]
        public void TestBuildSubscribeRequestTTCGFliterExp ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test" };
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";

            TestBuildSubscribeRequestCommon (channels, channelGroups, timetoken, 
                "", false, "", filterExpr, "", 0);
            
        }

        [Test]
        public void TestBuildSubscribeRequestSSLTTCGFliterExp ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test" };

            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, timetoken, 
                "", true, "", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelTTCGFliterExp ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";

            TestBuildSubscribeRequestCommon (channels, channelGroups, timetoken, 
                "", false, "", filterExpr, "", 0);
            
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelTTCGFliterExp ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test", "test2" };

            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, timetoken, 
                "", true, "", filterExpr, "", 0);
            
        }

        [Test]
        public void TestBuildSubscribeRequestStateCGFliterExp ()
        {
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };

            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";

            TestBuildSubscribeRequestCommon (channels, channelGroups, "", 
                userState, false, "", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLStateCGFliterExp ()
        {
            string[] channels = { "test" };
            string userState = "{\"k\":\"v\"}";
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, "", 
                userState, true, "", filterExpr, "", 0);
            
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelStateCGFliterExp ()
        {
            string[] channels = { "test", "test2" };
            string userState = "{\"k\":\"v\"}";
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";

            TestBuildSubscribeRequestCommon (channels, channelGroups, "", 
                userState, false, "", filterExpr, "", 0);
            
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelStateCGFliterExp ()
        {
            string[] channels = { "test", "test2" };
            string userState = "{\"k\":\"v\"}";
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, "", 
                userState, true, "", filterExpr, "", 0);
            
        }

        [Test]
        public void TestBuildSubscribeRequestTTStateCGFliterExp ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";

            TestBuildSubscribeRequestCommon (channels, channelGroups, timetoken, 
                userState, false, "", filterExpr, "", 0);
            
        }

        [Test]
        public void TestBuildSubscribeRequestSSLTTStateCGFliterExp ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, timetoken, 
                userState, true, "", filterExpr, "", 0);
            
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelTTStateCGFliterExp ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test", "test2" };

            string[] channelGroups = { "cg", "cg2" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, timetoken, 
                userState, false, "", filterExpr, "", 0);
            
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelTTStateCGFliterExp ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, timetoken, 
                userState, true, "", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestAuthCGFliterExp ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, "",
                "", false, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLAuthCGFliterExp ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, "",
                "", true, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelAuthCGFliterExp ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, "",
                "", false, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelAuthCGFliterExp ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, "",
                "", true, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestTTAuthCGFliterExp ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test" };
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, timetoken,
                "", false, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLTTAuthCGFliterExp ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test" };
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, timetoken,
                "", true, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelTTAuthCGFliterExp ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, timetoken,
                "", false, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelTTAuthCGFliterExp ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, timetoken,
                "", true, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestStateAuthCGFliterExp ()
        {
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, "",
                userState, false, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLStateAuthCGFliterExp ()
        {
            string[] channels = { "test" };
            string userState = "{\"k\":\"v\"}";
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, "",
                userState, true, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelStateAuthCGFliterExp ()
        {
            string[] channels = { "test", "test2" };
            string userState = "{\"k\":\"v\"}";
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, "",
                userState, false, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelStateAuthCGFliterExp ()
        {
            string[] channels = { "test", "test2" };
            string userState = "{\"k\":\"v\"}";
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, "",
                userState, true, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestTTStateAuthCGFliterExp ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, timetoken,
                userState, false, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLTTStateAuthCGFliterExp ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            string[] channelGroups = { "cg" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, timetoken,
                userState, true, "authKey", filterExpr, "", 0);
            
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelTTStateAuthCGFliterExp ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg", "cg2" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, timetoken,
                userState, false, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelTTStateAuthCGFliterExp ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg", "cg2" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (channels, channelGroups, timetoken,
                userState, true, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelTTStateAuthCG ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg", "cg2" };

            TestBuildSubscribeRequestCommon (channels, channelGroups, timetoken,
                userState, true, "authKey", "", "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelTTStateAuthCGPHB ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg", "cg2" };

            TestBuildSubscribeRequestCommon (channels, channelGroups, timetoken,
                userState, true, "authKey", "", "", 30);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelTTStateAuthPHB ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test", "test2" };


            TestBuildSubscribeRequestCommon (channels, null, timetoken,
                userState, true, "authKey", "", "", 30);
        }

        [Test]
        public void TestBuildSubscribeRequestCGOnlyFliterExp ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";

            TestBuildSubscribeRequestCommon (null, channelGroups, "", 
                "", false, "", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLCGOnlyFliterExp ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";

            TestBuildSubscribeRequestCommon (null, channelGroups, "", 
                "", true, "", filterExpr, "", 0);

        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelCGOnlyFliterExp ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";

            TestBuildSubscribeRequestCommon (null, channelGroups, "", 
                "", false, "", filterExpr, "", 0);


        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelCGOnlyFliterExp ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";

            TestBuildSubscribeRequestCommon (null, channelGroups, "", 
                "", true, "", filterExpr, "", 0);

        }

        [Test]
        public void TestBuildSubscribeRequestTTCGOnlyFliterExp ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test" };
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";

            TestBuildSubscribeRequestCommon (null, channelGroups, timetoken, 
                "", false, "", filterExpr, "", 0);

        }

        [Test]
        public void TestBuildSubscribeRequestSSLTTCGOnlyFliterExp ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test" };

            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, timetoken, 
                "", true, "", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelTTCGOnlyFliterExp ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";

            TestBuildSubscribeRequestCommon (null, channelGroups, timetoken, 
                "", false, "", filterExpr, "", 0);

        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelTTCGOnlyFliterExp ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test", "test2" };

            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, timetoken, 
                "", true, "", filterExpr, "", 0);

        }

        [Test]
        public void TestBuildSubscribeRequestStateCGOnlyFliterExp ()
        {
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };

            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";

            TestBuildSubscribeRequestCommon (null, channelGroups, "", 
                userState, false, "", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLStateCGOnlyFliterExp ()
        {
            string[] channels = { "test" };
            string userState = "{\"k\":\"v\"}";
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, "", 
                userState, true, "", filterExpr, "", 0);

        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelStateCGOnlyFliterExp ()
        {
            string[] channels = { "test", "test2" };
            string userState = "{\"k\":\"v\"}";
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";

            TestBuildSubscribeRequestCommon (null, channelGroups, "", 
                userState, false, "", filterExpr, "", 0);

        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelStateCGOnlyFliterExp ()
        {
            string[] channels = { "test", "test2" };
            string userState = "{\"k\":\"v\"}";
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, "", 
                userState, true, "", filterExpr, "", 0);

        }

        [Test]
        public void TestBuildSubscribeRequestTTStateCGOnlyFliterExp ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";

            TestBuildSubscribeRequestCommon (null, channelGroups, timetoken, 
                userState, false, "", filterExpr, "", 0);

        }

        [Test]
        public void TestBuildSubscribeRequestSSLTTStateCGOnlyFliterExp ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, timetoken, 
                userState, true, "", filterExpr, "", 0);

        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelTTStateCGOnlyFliterExp ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test", "test2" };

            string[] channelGroups = { "CGOnly", "CGOnly2" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, timetoken, 
                userState, false, "", filterExpr, "", 0);

        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelTTStateCGOnlyFliterExp ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, timetoken, 
                userState, true, "", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestAuthCGOnlyFliterExp ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, "",
                "", false, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLAuthCGOnlyFliterExp ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, "",
                "", true, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelAuthCGOnlyFliterExp ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, "",
                "", false, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelAuthCGOnlyFliterExp ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, "",
                "", true, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestTTAuthCGOnlyFliterExp ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test" };
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, timetoken,
                "", false, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLTTAuthCGOnlyFliterExp ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test" };
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, timetoken,
                "", true, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelTTAuthCGOnlyFliterExp ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, timetoken,
                "", false, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelTTAuthCGOnlyFliterExp ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, timetoken,
                "", true, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestStateAuthCGOnlyFliterExp ()
        {
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, "",
                userState, false, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLStateAuthCGOnlyFliterExp ()
        {
            string[] channels = { "test" };
            string userState = "{\"k\":\"v\"}";
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, "",
                userState, true, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelStateAuthCGOnlyFliterExp ()
        {
            string[] channels = { "test", "test2" };
            string userState = "{\"k\":\"v\"}";
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, "",
                userState, false, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelStateAuthCGOnlyFliterExp ()
        {
            string[] channels = { "test", "test2" };
            string userState = "{\"k\":\"v\"}";
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, "",
                userState, true, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestTTStateAuthCGOnlyFliterExp ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, timetoken,
                userState, false, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLTTStateAuthCGOnlyFliterExp ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            string[] channelGroups = { "CGOnly" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, timetoken,
                userState, true, "authKey", filterExpr, "", 0);

        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelTTStateAuthCGOnlyFliterExp ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "CGOnly", "CGOnly2" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, timetoken,
                userState, false, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelTTStateAuthCGOnlyFliterExp ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "CGOnly", "CGOnly2" };
            string filterExpr = "region == \"east\"";
            TestBuildSubscribeRequestCommon (null, channelGroups, timetoken,
                userState, true, "authKey", filterExpr, "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelTTStateAuthCGOnly ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "CGOnly", "CGOnly2" };

            TestBuildSubscribeRequestCommon (null, channelGroups, timetoken,
                userState, true, "authKey", "", "", 0);
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelTTStateAuthCGOnlyPHB ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "CGOnly", "CGOnly2" };

            TestBuildSubscribeRequestCommon (null, channelGroups, timetoken,
                userState, true, "authKey", "", "", 30);
        }

        public void TestBuildSubscribeRequestCommon(string[] channels, object timetoken, string userState,
            bool ssl, string authKey){
            TestBuildSubscribeRequestCommon(channels, null, timetoken, userState, ssl, authKey, "", "", 0);
        }

        public void TestBuildSubscribeRequestCommon(string[] channels, string[] channelGroups, 
            object timetoken, string userState,
            bool ssl, string authKey, string filterExpr, string region, int presenceHeartbeat){
            string uuid = "customuuid";

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.Secure = ssl;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            if(presenceHeartbeat !=0 ){
                pnConfiguration.PresenceTimeout = presenceHeartbeat;
            }
            pnConfiguration.AuthKey = authKey;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pnConfiguration.AuthKey);
            }

            string tt = "0";
            if (timetoken == null) {
                tt = "0";
            } else {
                tt = timetoken.ToString();
            }
            if(string.IsNullOrEmpty(tt)){
                tt = "0";
            }
            string ttStr = string.Format("&tt={0}", tt);

            string cgStr = "";
            string cg = ""; 
            if (channelGroups != null)
            {
                cg = string.Join (",", channelGroups);
                cgStr = string.Format("&channel-group={0}", Utility.EncodeUricomponent (cg, PNOperationType.PNSubscribeOperation, true, false));
            }                   

            string phb = "";
            if (presenceHeartbeat != 0) {
                phb = string.Format("&heartbeat={0}", presenceHeartbeat);
            }

            string chStr = ",";
            string ch = "";
            if (channels != null){
                ch = string.Join (",", channels);
                chStr = ch;
            }

            Uri uri = BuildRequests.BuildSubscribeRequest (ch, cg, tt, userState, region, filterExpr, pnUnity);

            string filterExpression = "";
            if(!string.IsNullOrEmpty (filterExpr)){
                filterExpression = string.Format ("&filter-expr=({0})", Utility.EncodeUricomponent(filterExpr, PNOperationType.PNSubscribeOperation, false, false));
            }

            string reg = "";
            if (!string.IsNullOrEmpty (region)) {
                reg = string.Format ("&tr=({0})", Utility.EncodeUricomponent(region, PNOperationType.PNSubscribeOperation, false, false));
            }
                
            //http://ps.pndsn.com/v2/subscribe/demo-36/test/0?uuid=customuuid&tt=21221&state={"k":"v"}&auth=authKey&pnsdk=PubNub-CSharp-UnityIOS/3.6.9.0
            //http://ps.pndsn.com/v2/subscribe/demo-36/test/0?uuid=customuuid&tt=0&filter-expr=(region%20%3D%3D%20%22east%22)&channel-group=cg&auth=authKey&pnsdk=PubNub-CSharp-UnityOSX/3.7
            string expected = string.Format ("http{0}://{1}/v2/subscribe/{2}/{3}/0?uuid={5}{4}{10}{11}{6}{7}{12}{8}{13}&pnsdk={9}",
                ssl?"s":"", pnConfiguration.Origin, pnConfiguration.SubscribeKey, 
                chStr, 
                ttStr,
                uuid, 
                (userState=="")?"":"&state=", 
                Utility.EncodeUricomponent(userState, PNOperationType.PNSubscribeOperation, false, false), 
                authKeyString, 
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNSubscribeOperation, false, false),
                filterExpression,
                reg,
                cgStr,
                phb
            );
            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }
        #endif
    }
}        