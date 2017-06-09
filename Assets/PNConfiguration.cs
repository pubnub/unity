using System;

namespace PubNubAPI
{
    public class PNConfiguration
    {
        public PNConfiguration ()
        {
            Secure = true;
        }

        private const string build = "4.0";
        private string pnsdkVersion = string.Format ("PubNub-CSharp-Unity/{0}", build);
        public string Version {
            get {
                return pnsdkVersion;
            }
            set {
                pnsdkVersion = value;
            }
        }
        
        public int ConcurrentNonSubscribeWorkers { get; set;}

        public string SubscribeKey { get; set;}
        public string PublishKey { get; set;}
        public string SecretKey { get; set;}
        public string CipherKey { get; set;}
        public string UUID { get; set;}
        public PNLogVerbosity LogVerbosity { get; set;}
        public string AuthKey { get; set;}
        public bool Secure { get; set;}
        public int SubscribeTimeout { get; set;}
        public int NonSubscribeTimeout { get; set;}
        public string FilterExpression { get; set;}
        public PNHeartbeatNotificationOption HeartbeatNotificationOption { get; set;}
        public string Origin { get; set;}
        public PNReconnectionPolicy ReconnectionPolicy { get; set;}
        public int PresenceTimeout { get; set;}
        public int PresenceInterval { get; set;}
        public int MaximumReconnectionRetries { get; set;}
    }
}

