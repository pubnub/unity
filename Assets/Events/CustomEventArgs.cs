using System;

namespace PubNubAPI
{
    public class CustomEventArgs : EventArgs
    {
        public string Message;
        public RequestState PubNubRequestState;
        //internal Action<T, PNStatus> Callback;
        public bool IsError;
        public bool IsTimeout;
        public PNCurrentRequestType CurrRequestType;
        //internal long ResponseCode;
        //internal string URL;
    }

    internal class CurrentRequestTypeEventArgs : EventArgs
    {
        internal UnityWebRequestWrapper WebRequestWrapper;
        internal bool IsTimeout;
        /* internal PNCurrentRequestType CurrRequestType;*/
    }

}

