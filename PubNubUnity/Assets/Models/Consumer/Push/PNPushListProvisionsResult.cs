
using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNPushListProvisionsResult: PNResult
    {
        public List<string> Channels {get; set;}
    }

}