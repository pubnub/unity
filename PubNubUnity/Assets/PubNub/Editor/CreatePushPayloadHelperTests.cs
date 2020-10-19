using System;
using PubNubAPI;
using NUnit.Framework;
using System.Text;
using System.Collections.Generic;

namespace PubNubAPI.Tests
{
    [TestFixture]
    public class CreatePushPayloadHelperTests
    {
        #if DEBUG    
        [Test]
        public void TestPushPayload ()
        {
            PushPayloadTestCommon(true, true, true, true, true, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlert ()
        {
            PushPayloadTestCommon(true, true, true, true, true, false);
        }
        
        [Test]
        public void TestPushPayloadWithoutAPNS2 ()
        {
            PushPayloadTestCommon(true, false, true, true, true, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndAPNS2 ()
        {
            PushPayloadTestCommon(true, false, true, true, true, false);
        }

        [Test]
        public void TestPushPayloadWithoutMPNS ()
        {
            PushPayloadTestCommon(true, true, false, true, true, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndMPNS ()
        {
            PushPayloadTestCommon(true, true, false, true, true, false);
        }
        
        [Test]
        public void TestPushPayloadWithoutAPNS2AndMPNS ()
        {
            PushPayloadTestCommon(true, false, false, true, true, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndAPNS2AndMPNS ()
        {
            PushPayloadTestCommon(true, false, false, true, true, false);
        }

        [Test]
        public void TestPushPayloadWithoutFCM ()
        {
            PushPayloadTestCommon(true, true, true, false, true, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndFCM ()
        {
            PushPayloadTestCommon(true, true, true, false, true, false);
        }
        
        [Test]
        public void TestPushPayloadWithoutAPNS2AndFCM ()
        {
            PushPayloadTestCommon(true, false, true, false, true, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndAPNS2AndFCM ()
        {
            PushPayloadTestCommon(true, false, true, false, true, false);
        }

        [Test]
        public void TestPushPayloadWithoutMPNSAndFCM ()
        {
            PushPayloadTestCommon(true, true, false, false, true, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndMPNSAndFCM ()
        {
            PushPayloadTestCommon(true, true, false, false, true, false);
        }
        
        [Test]
        public void TestPushPayloadWithoutAPNS2AndMPNSAndFCM ()
        {
            PushPayloadTestCommon(true, false, false, false, true, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndAPNS2AndMPNSAndFCM ()
        {
            PushPayloadTestCommon(true, false, false, false, true, false);
        }
        
        [Test]
        public void TestPushPayloadWithoutCommon ()
        {
            PushPayloadTestCommon(true, true, true, true, false, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndCommon ()
        {
            PushPayloadTestCommon(true, true, true, true, false, false);
        }
        
        [Test]
        public void TestPushPayloadWithoutAPNS2AndCommon ()
        {
            PushPayloadTestCommon(true, false, true, true, false, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndAPNS2AndCommon ()
        {
            PushPayloadTestCommon(true, false, true, true, false, false);
        }

        [Test]
        public void TestPushPayloadWithoutMPNSAndCommon ()
        {
            PushPayloadTestCommon(true, true, false, true, false, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndMPNSAndCommon ()
        {
            PushPayloadTestCommon(true, true, false, true, false, false);
        }
        
        [Test]
        public void TestPushPayloadWithoutAPNS2AndMPNSAndCommon ()
        {
            PushPayloadTestCommon(true, false, false, true, false, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndAPNS2AndMPNSAndCommon ()
        {
            PushPayloadTestCommon(true, false, false, true, false, false);
        }

        [Test]
        public void TestPushPayloadWithoutFCMAndCommon ()
        {
            PushPayloadTestCommon(true, true, true, false, false, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndFCMAndCommon ()
        {
            PushPayloadTestCommon(true, true, true, false, false, false);
        }
        
        [Test]
        public void TestPushPayloadWithoutAPNS2AndFCMAndCommon ()
        {
            PushPayloadTestCommon(true, false, true, false, false, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndAPNS2AndFCMAndCommon ()
        {
            PushPayloadTestCommon(true, false, true, false, false, false);
        }

        [Test]
        public void TestPushPayloadWithoutMPNSAndFCMAndCommon ()
        {
            PushPayloadTestCommon(true, true, false, false, false, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndMPNSAndFCMAndCommon ()
        {
            PushPayloadTestCommon(true, true, false, false, false, false);
        }
        
        [Test]
        public void TestPushPayloadWithoutAPNS2AndMPNSAndFCMAndCommon ()
        {
            PushPayloadTestCommon(true, false, false, false, false, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndAPNS2AndMPNSAndFCMAndCommon ()
        {
            PushPayloadTestCommon(true, false, false, false, false, false);
        }

        [Test]
        public void TestPushPayloadWithoutAPNS ()
        {
            PushPayloadTestCommon(false, true, true, true, true, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndAPNS ()
        {
            PushPayloadTestCommon(false, true, true, true, true, false);
        }
        
        [Test]
        public void TestPushPayloadWithoutAPNS2AndAPNS ()
        {
            PushPayloadTestCommon(false, false, true, true, true, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndAPNS2AndAPNS ()
        {
            PushPayloadTestCommon(false, false, true, true, true, false);
        }

        [Test]
        public void TestPushPayloadWithoutMPNSAndAPNS ()
        {
            PushPayloadTestCommon(false, true, false, true, true, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndMPNSAndAPNS ()
        {
            PushPayloadTestCommon(false, true, false, true, true, false);
        }
        
        [Test]
        public void TestPushPayloadWithoutAPNS2AndMPNSAndAPNS ()
        {
            PushPayloadTestCommon(false, false, false, true, true, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndAPNS2AndMPNSAndAPNS ()
        {
            PushPayloadTestCommon(false, false, false, true, true, false);
        }

        [Test]
        public void TestPushPayloadWithoutFCMAndAPNS ()
        {
            PushPayloadTestCommon(false, true, true, false, true, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndFCMAndAPNS ()
        {
            PushPayloadTestCommon(false, true, true, false, true, false);
        }
        
        [Test]
        public void TestPushPayloadWithoutAPNS2AndFCMAndAPNS ()
        {
            PushPayloadTestCommon(false, false, true, false, true, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndAPNS2AndFCMAndAPNS ()
        {
            PushPayloadTestCommon(false, false, true, false, true, false);
        }

        [Test]
        public void TestPushPayloadWithoutMPNSAndFCMAndAPNS ()
        {
            PushPayloadTestCommon(false, true, false, false, true, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndMPNSAndFCMAndAPNS ()
        {
            PushPayloadTestCommon(false, true, false, false, true, false);
        }
        
        [Test]
        public void TestPushPayloadWithoutAPNS2AndMPNSAndFCMAndAPNS ()
        {
            PushPayloadTestCommon(false, false, false, false, true, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndAPNS2AndMPNSAndFCMAndAPNS ()
        {
            PushPayloadTestCommon(false, false, false, false, true, false);
        }
        
        [Test]
        public void TestPushPayloadWithoutCommonAndAPNS ()
        {
            PushPayloadTestCommon(false, true, true, true, false, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndCommonAndAPNS ()
        {
            PushPayloadTestCommon(false, true, true, true, false, false);
        }
        
        [Test]
        public void TestPushPayloadWithoutAPNS2AndCommonAndAPNS ()
        {
            PushPayloadTestCommon(false, false, true, true, false, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndAPNS2AndCommonAndAPNS ()
        {
            PushPayloadTestCommon(false, false, true, true, false, false);
        }

        [Test]
        public void TestPushPayloadWithoutMPNSAndCommonAndAPNS ()
        {
            PushPayloadTestCommon(false, true, false, true, false, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndMPNSAndCommonAndAPNS ()
        {
            PushPayloadTestCommon(false, true, false, true, false, false);
        }
        
        [Test]
        public void TestPushPayloadWithoutAPNS2AndMPNSAndCommonAndAPNS ()
        {
            PushPayloadTestCommon(false, false, false, true, false, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndAPNS2AndMPNSAndCommonAndAPNS ()
        {
            PushPayloadTestCommon(false, false, false, true, false, false);
        }

        [Test]
        public void TestPushPayloadWithoutFCMAndCommonAndAPNS ()
        {
            PushPayloadTestCommon(false, true, true, false, false, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndFCMAndCommonAndAPNS ()
        {
            PushPayloadTestCommon(false, true, true, false, false, false);
        }
        
        [Test]
        public void TestPushPayloadWithoutAPNS2AndFCMAndCommonAndAPNS ()
        {
            PushPayloadTestCommon(false, false, true, false, false, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndAPNS2AndFCMAndCommonAndAPNS ()
        {
            PushPayloadTestCommon(false, false, true, false, false, false);
        }

        [Test]
        public void TestPushPayloadWithoutMPNSAndFCMAndCommonAndAPNS ()
        {
            PushPayloadTestCommon(false, true, false, false, false, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndMPNSAndFCMAndCommonAndAPNS ()
        {
            PushPayloadTestCommon(false, true, false, false, false, false);
        }
        
        [Test]
        public void TestPushPayloadWithoutAPNS2AndMPNSAndFCMAndCommonAndAPNS ()
        {
            PushPayloadTestCommon(false, false, false, false, false, true);
        }
        
        [Test]
        public void TestPushPayloadWithoutAlertAndAPNS2AndMPNSAndFCMAndCommonAndAPNS ()
        {
            PushPayloadTestCommon(false, false, false, false, false, false);
        }

        public void PushPayloadTestCommon (bool withAPNS, bool withAPNS2, bool withMPNS, bool withFCM, bool withCommonPayload, bool setAPNSAlert)
        {
            PNAPSData aps = new PNAPSData();
            aps.Alert = "alert";
            aps.Badge = 1;
            aps.Sound = "ding";
            aps.Custom = new Dictionary<string, object>(){
                {"aps_key1", "aps_value1"},
                {"aps_key2", "aps_value2"},
            };

            if (!setAPNSAlert) {
                aps.Alert = null;
                aps.Title = "title";
                aps.Subtitle = "subtitle";
                aps.Body = "body";
        	}

            PNAPNSData apns = new PNAPNSData();
            apns.APS = aps;
            apns.Custom = new Dictionary<string, object>(){
                {"apns_key1", "apns_value1"},
                {"apns_key2", "apns_value2"},
            };

            PNAPNS2Data apns2One = new PNAPNS2Data();
            apns2One.CollapseID = "invitations";
		    apns2One.Expiration = "2019-12-13T22:06:09Z";
		    apns2One.Version = "v1";
            apns2One.Targets = new List<PNPushTarget>(){
                new PNPushTarget(){
                    Environment = PNPushEnvironment.Development,
                    Topic = "com.meetings.chat.app",
                    ExcludeDevices = new List<string>(){
                        "device1",
                        "device2",
                    }
                }
            };

            PNAPNS2Data apns2Two = new PNAPNS2Data();
            apns2Two.CollapseID = "invitations";
		    apns2Two.Expiration = "2019-12-15T22:06:09Z";
		    apns2Two.Version = "v2";
            apns2Two.Targets = new List<PNPushTarget>(){
                new PNPushTarget(){
                    Environment = PNPushEnvironment.Production,
                    Topic = "com.meetings.chat.app",
                    ExcludeDevices = new List<string>(){
                        "device3",
                        "device4",
                    }
                }
            };

            List<PNAPNS2Data> apns2 = new List<PNAPNS2Data>(){
                apns2One,
                apns2Two,
            };

            CreatePushPayloadHelper cpph = new CreatePushPayloadHelper();
            if(withAPNS || withAPNS2){
                cpph.SetAPNSPayload(apns, null);
                if(withAPNS){
                    cpph.SetAPNSPayload(apns, apns2);
                }
            }

            PNMPNSData mpns = new PNMPNSData();
            mpns.Custom = new Dictionary<string, object>(){
                {"mpns_key1", "mpns_value1"},
                {"mpns_key2", "mpns_value2"},
            };
            mpns.Title = "title";
            mpns.Type = "type";
            mpns.Count = 1;
            mpns.BackTitle = "BackTitle";
            mpns.BackContent = "BackContent";

            PNFCMData fcm = new PNFCMData();
            fcm.Custom = new Dictionary<string, object>(){
                {"fcm_key1", "fcm_value1"},
                {"fcm_key2", "fcm_value2"},
            };
            fcm.Data = new PNFCMDataFields(){
                Summary = "summary",
                Custom = new Dictionary<string, object>(){
                    {"fcm_data_key1", "fcm_data_value1"},
                    {"fcm_data_key2", "fcm_data_value2"},
                }
            };

            Dictionary<string, object> commonPayload = new Dictionary<string, object>();
            commonPayload = new Dictionary<string, object>(){
                {"common_key1", "common_value1"},
                {"common_key2", "common_value2"},
            };

            if(withFCM){
                cpph.SetFCMPayload(fcm);
            }

            if(withMPNS){
                cpph.SetMPNSPayload(mpns);
            }

            if(withCommonPayload){
                cpph.SetCommonPayload(commonPayload);
            }

            Dictionary<string, object> result = cpph.BuildPayload();
            if(result != null){
                if (withAPNS2 || withAPNS) {
                    Dictionary<string, object> resAPNS = result["pn_apns"] as Dictionary<string, object>;
                    if(resAPNS != null){
                        Dictionary<string, object> resAPS = resAPNS["aps"] as Dictionary<string, object>;
                        if (!setAPNSAlert) {   
                            Dictionary<string, object> resAlert = resAPS["alert"] as Dictionary<string, object>;                     
                            if(resAPS != null){
                                EditorCommon.LogAndCompare (aps.Title, resAlert["title"].ToString());
                                EditorCommon.LogAndCompare (aps.Subtitle, resAlert["subtitle"].ToString());
                                EditorCommon.LogAndCompare (aps.Body, resAlert["body"].ToString());
                            }
                        } else {
                            EditorCommon.LogAndCompare (aps.Alert.ToString(), resAPS["alert"].ToString());
                        }
                        EditorCommon.LogAndCompare (aps.Badge.ToString(), resAPS["badge"].ToString());
                        EditorCommon.LogAndCompare (aps.Sound.ToString(), resAPS["sound"].ToString());
                        EditorCommon.LogAndCompare (aps.Custom["aps_key1"].ToString(), resAPS["aps_key1"].ToString());
                        EditorCommon.LogAndCompare (aps.Custom["aps_key2"].ToString(), resAPS["aps_key2"].ToString());
                        EditorCommon.LogAndCompare (apns.Custom["apns_key1"].ToString(), resAPNS["apns_key1"].ToString());
                        EditorCommon.LogAndCompare (apns.Custom["apns_key2"].ToString(), resAPNS["apns_key2"].ToString());
                    } else {
                        Assert.Fail("apns null");
                    }
                    if (withAPNS2 && withAPNS) {
                        List<Dictionary<string, object>> resAPNS2 = result["pn_push"] as List<Dictionary<string, object>>;
                        if(resAPNS2 != null){
                            EditorCommon.LogAndCompare (apns2One.CollapseID, resAPNS2[0]["collapseId"].ToString());
                            EditorCommon.LogAndCompare (apns2Two.CollapseID, resAPNS2[1]["collapseId"].ToString());
                            EditorCommon.LogAndCompare (apns2One.Expiration, resAPNS2[0]["expiration"].ToString());
                            EditorCommon.LogAndCompare (apns2Two.Expiration, resAPNS2[1]["expiration"].ToString());
                            EditorCommon.LogAndCompare (apns2One.Version, resAPNS2[0]["version"].ToString());
                            EditorCommon.LogAndCompare (apns2Two.Version, resAPNS2[1]["version"].ToString());
                            List<Dictionary<string, object>> resTargets1  = resAPNS2[1]["targets"] as List<Dictionary<string, object>>;
                            List<Dictionary<string, object>> resTargets0  = resAPNS2[0]["targets"] as List<Dictionary<string, object>>;
                            EditorCommon.LogAndCompare (apns2One.Targets[0].Environment.ToString(), resTargets0[0]["environment"].ToString());
                            EditorCommon.LogAndCompare (apns2One.Targets[0].Topic.ToString(), resTargets0[0]["topic"].ToString());
                            EditorCommon.LogAndCompare (apns2Two.Targets[0].Environment.ToString(), resTargets1[0]["environment"].ToString());
                            EditorCommon.LogAndCompare (apns2Two.Targets[0].Topic.ToString(), resTargets1[0]["topic"].ToString());
                            List<string> resExcludeDev0  = resTargets0[0]["exclude_devices"] as List<string>;
                            EditorCommon.LogAndCompare (apns2One.Targets[0].ExcludeDevices[0].ToString(), resExcludeDev0[0].ToString());
                            EditorCommon.LogAndCompare (apns2One.Targets[0].ExcludeDevices[1].ToString(), resExcludeDev0[1].ToString());
                            List<string> resExcludeDev1  = resTargets1[0]["exclude_devices"] as List<string>;
                            EditorCommon.LogAndCompare (apns2Two.Targets[0].ExcludeDevices[0].ToString(), resExcludeDev1[0].ToString());
                            EditorCommon.LogAndCompare (apns2Two.Targets[0].ExcludeDevices[1].ToString(), resExcludeDev1[1].ToString());
                        } else {
                            Assert.Fail("apns2 null");
                        }
                        
                    }

                }
                if(withMPNS){
                    Dictionary<string, object> resMPNS = result["pn_mpns"] as Dictionary<string, object>;
                    if(resMPNS != null){
                        EditorCommon.LogAndCompare (mpns.Title.ToString(), resMPNS["title"].ToString());
                        EditorCommon.LogAndCompare (mpns.BackContent.ToString(), resMPNS["back_content"].ToString());
                        EditorCommon.LogAndCompare (mpns.BackTitle.ToString(), resMPNS["back_title"].ToString());
                        EditorCommon.LogAndCompare (mpns.Count.ToString(), resMPNS["count"].ToString());
                        EditorCommon.LogAndCompare (mpns.Type.ToString(), resMPNS["type"].ToString());
                        EditorCommon.LogAndCompare (mpns.Custom["mpns_key1"].ToString(), resMPNS["mpns_key1"].ToString());
                        EditorCommon.LogAndCompare (mpns.Custom["mpns_key2"].ToString(), resMPNS["mpns_key2"].ToString());
                    } else {
                        Assert.Fail("mpns null");
                    }
                }

                if(withCommonPayload){
                    EditorCommon.LogAndCompare (commonPayload["common_key1"].ToString(), result["common_key1"].ToString());
                    EditorCommon.LogAndCompare (commonPayload["common_key2"].ToString(), result["common_key2"].ToString());
                }

                if(withFCM){
                    Dictionary<string, object> resFCM = result["pn_gcm"] as Dictionary<string, object>;
                    if(resFCM != null){
                        EditorCommon.LogAndCompare (fcm.Custom["fcm_key1"].ToString(), resFCM["fcm_key1"].ToString());
                        EditorCommon.LogAndCompare (fcm.Custom["fcm_key2"].ToString(), resFCM["fcm_key2"].ToString());
                        Dictionary<string, object> resFCMData = resFCM["data"] as Dictionary<string, object>;
                        EditorCommon.LogAndCompare (fcm.Data.Summary.ToString(), resFCMData["summary"].ToString());
                        EditorCommon.LogAndCompare (fcm.Data.Custom["fcm_data_key1"].ToString(), resFCMData["fcm_data_key1"].ToString());
                        EditorCommon.LogAndCompare (fcm.Data.Custom["fcm_data_key2"].ToString(), resFCMData["fcm_data_key2"].ToString());
                        
                    } else {
                        Assert.Fail("fcm null");
                    }
                }
            }

        }
        #endif
    }
}
