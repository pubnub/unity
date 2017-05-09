using System;

namespace PubNubAPI
{
    public class PNTimeResult: PNResult
    {
        public string TimeToken { get; set;}
        public PNTimeResult ()
        {
            
        }
    }

    public class PNWhereNowResult: PNResult
    {
        public string Result { get; set;}
        public PNWhereNowResult ()
        {

        }
    }

}

