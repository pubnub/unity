using System;

namespace PubNubAPI
{
    public class PNConfiguration
    {
        public PNConfiguration ()
        {
            SetSecure = true;
        }

        public bool SetSecure { get; set;}

        public int ConcurrentNonSubscribeWorkers { get; set;}
    }
}

