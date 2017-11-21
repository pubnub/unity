using UnityEngine;
using PubNubAPI;

namespace PubNubAPI.Tests
{
    public class PlayModeCommon {
		public static bool SslOn = false;
		public static bool CipherOn = false;
		public static string Origin = "ps.pndsn.com";
        public static string PublishKey = "pub-c-94691e07-c8aa-42f9-a838-bea61ac6655e";
        public static string SubscribeKey = "sub-c-b05d4a0c-708d-11e7-96c9-0619f8945a4f";
        public static string SecretKey = "sec-c-ZmIyZjFjMjQtZTNmZC00MmIwLWFhNzUtNDUyNmIwYWU1YzRl";
        public static string cg1 = "channelGroup1";
        public static string cg2 = "channelGroup2";
        public static string ch1 = "channel1";
        public static string ch2 = "channel2"; 

        public static int WaitTimeForAsyncResponse = 2;       
        public static int WaitTimeBetweenCalls = 2;       


        public static PNConfiguration SetPNConfig(){
            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = Origin;
            pnConfiguration.SubscribeKey = SubscribeKey;
            pnConfiguration.PublishKey = PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.Secure = SslOn;
            return pnConfiguration;
        }
    }
}