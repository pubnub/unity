using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class ChannelParameters
    {
        public bool IsAwaitingConnectCallback {get; set;}

        public bool IsSubscribed {get; set;}
        public Dictionary<string, object> UserState {get; set;}

        public ChannelParameters(){
            IsAwaitingConnectCallback = false;
            IsSubscribed = false;
            UserState = null;
        }

    }
}

