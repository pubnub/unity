using System;

namespace PubNubAPI
{
    public class TimetokenMetadata
    {
        private long t { get; set;} //JSON timetoken;
        private string r { get; set;} //JSON region;

        internal TimetokenMetadata(long timetoken, string region)
        {
            t = timetoken;
            r = region;
        }

        public long Timetoken { 
            get{
                return t;
            }
        }
        public string Region {
            get {
                return r;
            }
        }
    }
}

