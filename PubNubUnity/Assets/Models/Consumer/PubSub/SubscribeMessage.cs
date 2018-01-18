using System;

namespace PubNubAPI
{
    public class SubscribeMessage
    {
        private string a { get; set;} //JSON shard;
        private string b { get; set;} //JSON subscriptionMatch
        private string c { get; set;} //JSON channel
        private object d { get; set;} //JSON payload
        private string f { get; set;} //JSON flags
        private string i { get; set;} //JSON issuingClientId
        private string k { get; set;} //JSON subscribeKey
        private long s { get; set;} //JSON sequenceNumber
        private TimetokenMetadata o { get; set;} //JSON originatingTimetoken
        private TimetokenMetadata p { get; set;} //JSON publishMetadata
        private object u { get; set;} //JSON userMetadata

        internal SubscribeMessage(string shard, string subscriptionMatch, string channel, object payload,
            string flags, string issuingClientId, string subscribeKey, long sequenceNumber, TimetokenMetadata originatingTimetoken,
            TimetokenMetadata publishMetadata, object userMetadata
        )
        {
            this.a = shard;
            this.b = subscriptionMatch;
            this.c = channel;
            this.d = payload;
            this.f = flags;
            this.i = issuingClientId;
            this.k = subscribeKey;
            this.s = sequenceNumber;
            this.o = originatingTimetoken;
            this.p = publishMetadata;
            this.u = userMetadata;
        }

        public string Shard{
            get{
                return a;
            }
        }

        public string SubscriptionMatch{
            get{
                return b;
            }
        }

        public string Channel{
            get{
                return c;
            }
        }

        public object Payload{
            get{
                return d;
            }
        }

        public string Flags{
            get{
                return f;
            }
        }

        public string IssuingClientId{
            get{
                return i;
            }
        }

        public string SubscribeKey{
            get{
                return k;
            }
        }

        public long SequenceNumber{
            get{
                return s;
            }
        }

        public TimetokenMetadata OriginatingTimetoken{
            get{
                return o;
            }
        }

        public TimetokenMetadata PublishTimetokenMetadata{
            get{
                return p;
            }
        }

        public object UserMetadata{
            get{
                return u;
            }
        }

    }
}

