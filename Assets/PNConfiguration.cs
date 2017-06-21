using System;

namespace PubNubAPI
{
    public class PNConfiguration
    {
        public PNConfiguration ()
        {
            Secure = true;
        }

        private ushort concurrentNonSubscribeWorkers = 1;
        public ushort ConcurrentNonSubscribeWorkers { 
            get {
                return concurrentNonSubscribeWorkers;
            }
            set{
                concurrentNonSubscribeWorkers = value;
            }
        }

        public string SubscribeKey { get; set;}
        public string PublishKey { get; set;}
        public string SecretKey { get; set;}
        public string CipherKey { get; set;}
        private string uuid;
        public string UUID { 
            get{
                if (string.IsNullOrEmpty (uuid) || string.IsNullOrEmpty (uuid.Trim ())) {
                    uuid = string.Format("pn-{0}", Guid.NewGuid ().ToString ());
                }

                return uuid;
            }
            set{
                uuid = value;
            }
            
        }
        public PNLogVerbosity LogVerbosity { get; set;}
        public string AuthKey { get; set;}
        public bool Secure { get; set;}

        private int subscribeTimeout;
        public int SubscribeTimeout { 
            get {
                return subscribeTimeout;
            }

            set {
                #if(UNITY_IOS)
                subscribeTimeout = Utility.CheckTimeoutValue(value);
                #else
                subscribeTimeout = value;
                #endif
            }
        }

        private int nonSubscribeTimeout;
        public int NonSubscribeTimeout {
            get {
                return nonSubscribeTimeout;
            }

            set {
                #if(UNITY_IOS)
                nonSubscribeTimeout = Utility.CheckTimeoutValue(value);
                #else
                nonSubscribeTimeout = value;
                #endif
            }
        }
        
        public string FilterExpression { get; set;}
        public PNHeartbeatNotificationOption HeartbeatNotificationOption { get; set;}
        private string origin = "ps.pndsn.com";
        public string Origin { 
            get{
                return origin;
            } 
            set{
                origin = value;
            }
        }
        public PNReconnectionPolicy ReconnectionPolicy { get; set;}
        public int PresenceTimeout { get; set;}
        public int PresenceInterval { get; set;}
        public int MaximumReconnectionRetries { get; set;}
    }
}

