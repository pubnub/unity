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
        public string UUID { get; set;}
        public PNLogVerbosity LogVerbosity { get; set;}
        public string AuthKey { get; set;}
        public bool Secure { get; set;}
        public int SubscribeTimeout { get; set;}
        public int NonSubscribeTimeout { get; set;}
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

