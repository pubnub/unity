using UnityEngine;
using PubNubAPI;
using System.Xml;
using System.Collections.Generic;
using System;


namespace PubNubAPI.Tests
{

#if !UNITY_WSA && !UNITY_WSA_10_0
    class PubnubDemoObject
    {
        public double VersionID {get; set;} 
        public long Timetoken {get; set;} 
        public string OperationName {get; set;}
        public string[] Channels {get; set;}
        public PubnubDemoMessage DemoMessage {get; set;}
        public PubnubDemoMessage CustomMessage {get; set;}
        public XmlDocument SampleXml {get; set;}

        public PubnubDemoObject(){
            VersionID = 3.4;
            Timetoken = 13601488652764619;
            OperationName = "Publish";
            Channels = new string[]{ "ch1" };
            DemoMessage = new PubnubDemoMessage ();
            CustomMessage = new PubnubDemoMessage ("This is a demo message");
            SampleXml = new PubnubDemoMessage ().TryXmlDemo ();
        }
        
    }

    class PubnubDemoMessage
    {
        public string DefaultMessage = "~!@#$%^&*()_+ `1234567890-= qwertyuiop[]\\ {}| asdfghjkl;' :\" zxcvbnm,./ <>? ";
        public PubnubDemoMessage ()
        {
        }

        public PubnubDemoMessage (string message)
        {
            DefaultMessage = message;
        }

        public XmlDocument TryXmlDemo ()
        {
            XmlDocument xmlDocument = new XmlDocument ();
            xmlDocument.LoadXml ("<DemoRoot><Person ID='ABCD123'><Name><First>John</First><Middle>P.</Middle><Last>Doe</Last></Name><Address><Street>123 Duck Street</Street><City>New City</City><State>New York</State><Country>United States</Country></Address></Person><Person ID='ABCD456'><Name><First>Peter</First><Middle>Z.</Middle><Last>Smith</Last></Name><Address><Street>12 Hollow Street</Street><City>Philadelphia</City><State>Pennsylvania</State><Country>United States</Country></Address></Person></DemoRoot>");

            return xmlDocument;
        }
    }

    public class PlayModeCommon {
		public static bool SslOn = true;
		public static bool CipherOn = false;
		public static string Origin = "ps.pndsn.com";
        public static string PublishKey = "pub-c-7b22d447-dd68-41ad-903b-d07eb9169960"; 
        public static string SubscribeKey = "sub-c-6e923054-b0f1-411d-ac71-0c59e384fe86"; 
        public static string SecretKey = "";

        public static string PublishKeyPAM = "pub-c-b0a534df-1a8b-43cb-b62e-da6ce6e6ac37"; 
        public static string SubscribeKeyPAM = "sub-c-e48eb113-5e23-47be-bdff-0a5cb5218986"; 
        public static string SecretKeyPAM = "sec-c-ZjA0MzdiMzItYmM0Ni00ZGQyLTkwZDktNjFmNWY2MjMxN2Iy";

        public static string cg1 = "channelGroup1";
        public static string cg2 = "channelGroup2";
        public static string ch1 = "channel1";
        public static string ch2 = "channel2"; 

        public static int WaitTimeForAsyncResponse = 2;       
        public static int WaitTimeBetweenCalls1 = 1;  
        public static int WaitTimeBetweenCalls = 3;   
        public static int WaitTimeBetweenCalls2 = 7;  
        public static int WaitTimeBetweenCalls3 = 3;  
        public static int WaitTimeBetweenCalls4 = 4;   
        public static int WaitTimeBetweenCalls5 = 4;       

        public static PNConfiguration SetPAMPNConfig(bool useCipher){
            return SetPNConfig(useCipher, true);
        }

        public static PNConfiguration SetPNConfig(bool useCipher){
            return SetPNConfig(useCipher, false);
        }

        public static PNConfiguration SetPNConfig(bool useCipher, bool withPAM){
            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = Origin;
            pnConfiguration.SubscribeKey = SubscribeKey;
            pnConfiguration.PublishKey = PublishKey;
            pnConfiguration.NonSubscribeTimeout = 30;
            if(withPAM){
                pnConfiguration.SubscribeKey = SubscribeKeyPAM;
                pnConfiguration.PublishKey = PublishKeyPAM;
                pnConfiguration.SecretKey = SecretKeyPAM;
            }

            if(useCipher){
                pnConfiguration.CipherKey = "enigma";
            }
            
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.Secure = SslOn;
            return pnConfiguration;
        }

        
    }
#endif    
}