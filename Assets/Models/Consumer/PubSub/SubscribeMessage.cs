using System;

namespace PubNubAPI
{
    internal class SubscribeMessage
    {
        private string a { get; set;} //shard;
        private string b { get; set;} //subscriptionMatch
        private string c { get; set;} //channel
        private object d { get; set;} //payload
        //private bool ear { get; set;} //eat after reading
        private string f { get; set;} //flags
        private string i { get; set;} //issuingClientId
        private string k { get; set;} //subscribeKey
        private long s { get; set;} //sequenceNumber
        private TimetokenMetadata o { get; set;} //originatingTimetoken
        private TimetokenMetadata p { get; set;} //publishMetadata
        //private string r { get; set;} //replicationMap
        private object u { get; set;} //userMetadata
        //private string w { get; set;} //waypointList

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

        /*public bool EatAfterReading{
            get{
                return ear;
            }
        }*/

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

        /*public object ReplicationMap{
            get{
                return r;
            }
        }*/

        public object UserMetadata{
            get{
                return u;
            }
        }

        /*public string WaypointList{
            get{
                return w;
            }
        }*/


    }
}

