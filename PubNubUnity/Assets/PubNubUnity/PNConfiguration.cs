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

        public string SubscribeKey {get; set;}
        public string PublishKey {get; set;}
        public string SecretKey {get; set;}
        public string CipherKey {get; set;}
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

        private int messageQueueOverflowCount =100;
        public int MessageQueueOverflowCount
        {
            get {return messageQueueOverflowCount;}
            set {messageQueueOverflowCount = value;}
        }

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

        private PNHeartbeatNotificationOption heartbeatNotificationOption = PNHeartbeatNotificationOption.Failures;

        public PNHeartbeatNotificationOption HeartbeatNotificationOption
        {
            get {return heartbeatNotificationOption;}
            set {heartbeatNotificationOption = value;}
        }

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
        private PNReconnectionPolicy reconnectionPolicy = PNReconnectionPolicy.LINEAR;
        public PNReconnectionPolicy ReconnectionPolicy
        {
            get {return reconnectionPolicy;}
            set {reconnectionPolicy = value;}
        }
        public int PresenceTimeout { get; set;}
        //In seconds, How often the client should announce it's existence via heartbeating.
        public int PresenceInterval { get; set;}

        private int maximumReconnectionRetries = 50;
        public int MaximumReconnectionRetries
        {
            get {return maximumReconnectionRetries;}
            set {maximumReconnectionRetries = value;}
        }

        private bool suppressLeaveEvents = false;

        public bool SuppressLeaveEvents
        {
            get {return suppressLeaveEvents;}
            set {suppressLeaveEvents = value;}
        }
    }
}

