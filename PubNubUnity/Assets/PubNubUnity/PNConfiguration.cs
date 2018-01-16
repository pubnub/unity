using System;

namespace PubNubAPI
{
    public class PNConfiguration
    {
        public event EventHandler<EventArgs> UUIDChanged; 
        public event EventHandler<EventArgs> FilterExpressionChanged; 
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

        public string SubscribeKey = "";
        public string PublishKey = "";
        public string SecretKey = "";
        public string CipherKey = "";
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
                if(UUIDChanged!=null){
                    UUIDChanged.Invoke(this, null);
                }
            }
        }

        public PNLogVerbosity LogVerbosity { get; set;}
        public string AuthKey { get; set;}
        public bool Secure { get; set;}

        public int MessageQueueOverflowCount =100;

        private int subscribeTimeout = 310;
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

        private int nonSubscribeTimeout = 15;
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
        string filterExpr;
        public string FilterExpression{
            get { return filterExpr; }
            set{
                filterExpr = value;
                if(FilterExpressionChanged!=null){
                    FilterExpressionChanged.Invoke(this, null);
                }
            }
        }

        public PNHeartbeatNotificationOption HeartbeatNotificationOption = PNHeartbeatNotificationOption.Failures;
        private string origin = "ps.pndsn.com";
        public string Origin { 
            get{
                return origin;
            } 
            set{
                origin = value;
            }
        }
        //In seconds, how long the server will consider this client to be online before issuing a leave event.
        public PNReconnectionPolicy ReconnectionPolicy = PNReconnectionPolicy.LINEAR;//PNReconnectionPolicy.NONE;
        public int PresenceTimeout { get; set;}
        //In seconds, How often the client should announce it's existence via heartbeating.
        public int PresenceInterval { get; set;}

        public int MaximumReconnectionRetries = 50;

        public bool SuppressLeaveEvents = false;
    }
}

