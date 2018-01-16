using System;

namespace PubNubAPI
{
    public class CustomEventArgs : EventArgs
    {
        public string Message;
        public RequestState PubNubRequestState;
        public bool IsError;
        public bool IsTimeout;
        public PNCurrentRequestType CurrRequestType;
    }

    internal class CurrentRequestTypeEventArgs : EventArgs
    {
        internal UnityWebRequestWrapper WebRequestWrapper;
        internal bool IsTimeout;
    }

}

