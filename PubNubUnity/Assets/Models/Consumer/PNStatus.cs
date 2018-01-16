using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNStatus
    {
        public PNStatus ()
        {
            Error = false;
        }

        public PNStatus(PNStatusCategory category, PNErrorData errorData, bool error, long statusCode, PNOperationType operation, bool tlsEnabled, string uuid, string authKey, string origin, List<String> affectedChannels, List<String> affectedChannelGroups, object clientRequest){
            this.Category = category;
            this.ErrorData = errorData;
            this.Error = error;
            this.StatusCode = statusCode;
            this.Operation = operation;
            this.TlsEnabled = tlsEnabled;
            this.UUID = uuid;
            this.AuthKey = authKey;
            this.Origin = origin;
            this.ClientRequest = clientRequest;
            this.AffectedChannelGroups = affectedChannelGroups;
            this.AffectedChannels = affectedChannels;            
        }

        public PNStatusCategory Category {get; set;}
        public PNErrorData ErrorData  {get; set;}
        public bool Error {get; set;}

        public long StatusCode {get; set;}
        public PNOperationType Operation {get; set;}

        public bool TlsEnabled {get; set;}

        public string UUID {get; set;}
        public string AuthKey {get; set;}
        public string Origin {get; set;}
        public object ClientRequest {get; set;}

        // send back channel, channel groups that were affected by this operation
        public List<String> AffectedChannels {get; set;}
        public List<String> AffectedChannelGroups {get; set;}
    }
}

