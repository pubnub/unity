using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNTimeResult: PNResult
    {
        public long TimeToken { get; set;}
        public PNTimeResult ()
        {
            
        }
    }

    public class PNWhereNowResult: PNResult
    {
        public List<String> Channels { get; set;}
        public PNWhereNowResult ()
        {

        }
    }

}

