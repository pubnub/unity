using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    internal class SubscribeEnvelope
    {
        private List<SubscribeMessage> m { get; set;} //messages;
        private TimetokenMetadata t { get; set;} //subscribeMetadata;

        public List<SubscribeMessage> Messages{
            get{
                return m;
            }
            set {
                m = value;
            }
        }

        public TimetokenMetadata TimetokenMeta{
            get{
                return t;
            }
            set {
                t = value;
            }
        }
    }
}

