using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class CreatePushPayloadHelper
    {
        private Dictionary<string, object> CommonPayload { get; set; }
        private PNAPNSData PushAPNSData { get; set; }
        private PNFCMData PushFCMData { get; set; }
        private PNMPNSData PushMPNSData { get; set; }
        private List<PNAPNS2Data> PushAPNS2Data { get; set; }

        public CreatePushPayloadHelper SetAPNSPayload(PNAPNSData PushAPNSData, List<PNAPNS2Data> PushAPNS2Data){
            this.PushAPNS2Data = PushAPNS2Data;
            this.PushAPNSData = PushAPNSData;
            return this;
        }
        
        public CreatePushPayloadHelper SetMPNSPayload(PNMPNSData PushMPNSData){
            this.PushMPNSData = PushMPNSData;
            return this;
        }
        
        public CreatePushPayloadHelper SetCommonPayload(Dictionary<string, object> CommonPayload){
            this.CommonPayload = CommonPayload;
            return this;
        }
        
        public CreatePushPayloadHelper SetFCMPayload(PNFCMData PushFCMData){
            this.PushFCMData = PushFCMData;
            return this;
        }
        
        public Dictionary<string, object> BuildPayload(){
            Dictionary<string, object> response = new Dictionary<string, object>();
            Dictionary<string, object> apns = BuildAPNSPayload();
            if(apns != null){
                response.Add("pn_apns", apns);
                List<Dictionary<string, object>> apns2 = BuildAPNS2Payload();
                if(apns2 != null){
                    response.Add("pn_push", apns2);
                }
            }

            Dictionary<string, object> mpns = BuildMPNSPayload();
            if(mpns != null){
                response.Add("pn_mpns", mpns);
            }

            Dictionary<string, object> fcm = BuildFCMPayload();
            if(fcm != null){
                response.Add("pn_gcm", fcm);
            }

            if((CommonPayload != null) && (CommonPayload.Count>0)){
                foreach(KeyValuePair<string, object> kvp in CommonPayload){
                    response[kvp.Key] = kvp.Value; 
                }
            }

            return response;
        }

        private Dictionary<string, object> BuildMPNSPayload()
        {            
            if(PushMPNSData != null){
                Dictionary<string, object> mpns = new Dictionary<string, object>();
                if(!string.IsNullOrEmpty(PushMPNSData.Title)){
                    mpns["title"] = PushMPNSData.Title;
                }
                if(!string.IsNullOrEmpty(PushMPNSData.Title)){
                    mpns["type"] = PushMPNSData.Type;
                }
                if(!string.IsNullOrEmpty(PushMPNSData.Title)){
                    mpns["back_title"] = PushMPNSData.BackTitle;
                }
                if(!string.IsNullOrEmpty(PushMPNSData.Title)){
                    mpns["back_content"] = PushMPNSData.BackContent;
                }
                mpns["count"] = PushMPNSData.Count;
                if(PushMPNSData.Custom != null){
                    foreach(KeyValuePair<string, object> kvp in PushMPNSData.Custom){
                        mpns[kvp.Key] = kvp.Value; 
                    }
                }
                return mpns;
            }
            return null;
        }

        private Dictionary<string, object> BuildAPNSPayload()
        {
            if(PushAPNSData != null){
                Dictionary<string, object> apns = new Dictionary<string, object>();
                if(PushAPNSData.APS != null){
                    Dictionary<string, object> aps = new Dictionary<string, object>();
                    if(PushAPNSData.APS.Alert != null){
                        aps["alert"] = PushAPNSData.APS.Alert;
                    } else if (!string.IsNullOrEmpty(PushAPNSData.APS.Title) || !string.IsNullOrEmpty(PushAPNSData.APS.Body) || !string.IsNullOrEmpty(PushAPNSData.APS.Subtitle)){
                        Dictionary<string, object> alert = new Dictionary<string, object>();
                        if (!string.IsNullOrEmpty(PushAPNSData.APS.Title)){
                            alert["title"] = PushAPNSData.APS.Title;
                        }

                        if (!string.IsNullOrEmpty(PushAPNSData.APS.Subtitle)){
                            alert["subtitle"] = PushAPNSData.APS.Subtitle;
                        }

                        if (!string.IsNullOrEmpty(PushAPNSData.APS.Body)){
                            alert["body"] = PushAPNSData.APS.Body;
                        }

                        aps["alert"] = alert;
                    }
                    aps["badge"] = PushAPNSData.APS.Badge;

                    if(!string.IsNullOrEmpty(PushAPNSData.APS.Sound)){
                        aps["sound"] = PushAPNSData.APS.Sound;
                    }

                    if(PushAPNSData.APS.Custom != null){
                        foreach(KeyValuePair<string, object> kvp in PushAPNSData.APS.Custom){
                            aps[kvp.Key] = kvp.Value; 
                        }
                    }

                    apns["aps"] = aps;
                }
                if(PushAPNSData.Custom != null){
                    foreach(KeyValuePair<string, object> kvp in PushAPNSData.Custom){
                        apns[kvp.Key] = kvp.Value; 
                    }
                }
                return apns;
            }
            return null;
        }

        private List<Dictionary<string, object>> BuildAPNS2Payload()
        {            
            if(PushAPNS2Data != null){                
                List<Dictionary<string, object>> lstAPNS2 = new List<Dictionary<string, object>>();
                foreach (PNAPNS2Data apns2Item in PushAPNS2Data){
                    Dictionary<string, object> apns2Data = new Dictionary<string, object>();
                    apns2Data["collapseId"] = apns2Item.CollapseID;
                    apns2Data["expiration"] = apns2Item.Expiration;
                    apns2Data["version"] = apns2Item.Version;
                    if(apns2Item.Targets != null){
                        List<Dictionary<string, object>> lstTargets = new List<Dictionary<string, object>>();
                        foreach(PNPushTarget pnPushTarget in apns2Item.Targets){
                            Dictionary<string, object> target = new Dictionary<string, object>();
                            target["topic"] = pnPushTarget.Topic;
                            target["environment"] = pnPushTarget.Environment;
                            target["exclude_devices"] = pnPushTarget.ExcludeDevices;
                            lstTargets.Add(target);
                        }
                        apns2Data["targets"] = lstTargets;
                    }
                    lstAPNS2.Add(apns2Data);
                }
                return lstAPNS2;  
            }

            return null;
        }

        private Dictionary<string, object> BuildFCMPayload()
        {            
            if(PushFCMData != null){
                Dictionary<string, object> fcm = new Dictionary<string, object>();
                if(PushFCMData.Data != null){
                    Dictionary<string, object> fcmData = new Dictionary<string, object>();
                    if(PushFCMData.Data.Summary != null){
                        fcmData["summary"]= PushFCMData.Data.Summary;
                    }
                    if(PushFCMData.Data.Custom != null){
                        foreach(KeyValuePair<string, object> kvp in PushFCMData.Data.Custom){
                            fcmData[kvp.Key] = kvp.Value; 
                        }
                    }
                    fcm["data"]= fcmData;
                }
                if(PushFCMData.Custom != null){
                    foreach(KeyValuePair<string, object> kvp in PushFCMData.Custom){
                        fcm[kvp.Key] = kvp.Value; 
                    }
                }
                return fcm;
            }

            return null;
        }

    }
    public class PNAPNS2Data
    {
        public string CollapseID { get; set; }
        public string Expiration { get; set; }
        public List<PNPushTarget> Targets { get; set; }

        public string Version { get; set;} 
    }

    public class PNPushTarget
    {
        public string Topic { get; set; }
        public List<string> ExcludeDevices { get; set; }
        
        public PNPushEnvironment Environment { get; set; }
    }

    public class PNAPNSData
    {
        public PNAPSData APS { get; set; }
        public Dictionary<string, object> Custom { get; set; }
    }

    public class PNAPSData
    {
        public object Alert { get; set; }
        public Dictionary<string, object> Custom { get; set; }
        public int Badge { get; set; }
        public string Sound { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Body { get; set; }
    }

    public class PNMPNSData
    {
        public Dictionary<string, object> Custom { get; set; }
        public int Count { get; set; }
        public string BackTitle { get; set; }
        public string Title { get; set; }
        public string BackContent { get; set; }
        public string Type { get; set; }
    }

    public class PNFCMData
    {
        public Dictionary<string, object> Custom { get; set; }
        public PNFCMDataFields Data { get; set; }
    }

    public class PNFCMDataFields
    {
        public object Summary { get; set; }
        public Dictionary<string, object> Custom { get; set; }
    }

}