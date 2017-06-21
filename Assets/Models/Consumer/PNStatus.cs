using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNStatus
    {
        public PNStatus ()
        {
        }

        public PNStatusCategory Category {get; set;}
        public PNErrorData ErrorData  {get; set;}
        public bool Error {get; set;}

        // boolean automaticallyRetry;

        public int StatusCode {get; set;}
        public PNOperationType Operation {get; set;}

        public bool TlsEnabled {get; set;}

        public string UUID {get; set;}
        public string AuthKey {get; set;}
        public string Origin {get; set;}
        public object ClientRequest {get; set;}

        // send back channel, channel groups that were affected by this operation
        public List<String> AffectedChannels {get; set;}
        public List<String> AffectedChannelGroups {get; set;}
        //private Endpoint executedEndpoint;
    }
}

