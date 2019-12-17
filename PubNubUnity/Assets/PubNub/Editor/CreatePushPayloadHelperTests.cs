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
            PushPayloadTestCommon(true, true, false);
        }
        
        public void PushPayloadTestCommon (bool withAPNS, bool withAPNS2, bool setAPNSAlert)
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

            Dictionary<string, object> result = cpph.BuildPayload();
            if(result != null){
                if (withAPNS2 || withAPNS) {
                    Dictionary<string, object> resAPNS = result["pn_apns"] as Dictionary<string, object>;
                    UnityEngine.Debug.Log(resAPNS["aps"]);
                    if (withAPNS2) {
                        List<Dictionary<string, object>> resAPNS2 = result["pn_push"] as List<Dictionary<string, object>>;
                        UnityEngine.Debug.Log(resAPNS2[0]["collapseId"]);
                    }
                }
            }

        }
        #endif
    }
}