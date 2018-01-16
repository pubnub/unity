using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace PubNubAPI
{
    public class RequestState
    {
        public PNOperationType OperationType;

        internal long StartRequestTicks;
        internal long EndRequestTicks;

        public long ResponseCode;
        public string URL;

        public string WebRequestId;
        public HTTPMethod httpMethod;

        public string POSTData = "";

        public int Timeout;
        public int Pause;
        public bool Reconnect;

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