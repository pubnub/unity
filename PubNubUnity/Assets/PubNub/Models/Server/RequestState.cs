using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace PubNubAPI
{
    public class RequestState
    {
        public PNOperationType OperationType {get; set;}

        internal long StartRequestTicks;
        internal long EndRequestTicks;

        public long ResponseCode {get; set;}
        public string URL {get; set;}

        public string WebRequestId {get; set;}
        public HTTPMethod httpMethod {get; set;}

        public string POSTData  {get; set;}

        public int Timeout {get; set;}
        public int Pause {get; set;}
        public bool Reconnect {get; set;}

        public RequestState ()
        {
            StartRequestTicks = 0;
            EndRequestTicks = 0;
            URL = "";
            httpMethod = HTTPMethod.Get;
            POSTData = "";
            Timeout = 0;
            Pause = 0;
            Reconnect = false;
        }
    }
}