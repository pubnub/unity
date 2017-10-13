using System;

namespace PubNubAPI
{
    internal class TimetokenMetadata
    {
        private long t { get; set;} //timetoken;
        private string r { get; set;} //region;

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

