using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class SubscribeEnvelope
    {
        private List<SubscribeMessage> m { get; set;} //JSON messages;
        private TimetokenMetadata t { get; set;} //JSON subscribeMetadata;

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

