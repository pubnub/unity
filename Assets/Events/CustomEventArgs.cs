using System;

namespace PubNubAPI
{
    internal class CustomEventArgs<T> : EventArgs
    {
        internal string Message;
        internal RequestState PubNubRequestState;
        //internal Action<T, PNStatus> Callback;
        internal bool IsError;
        internal bool IsTimeout;
        internal PNCurrentRequestType CurrRequestType;
        //internal long ResponseCode;
        //internal string URL;
    }

    internal class CurrentRequestTypeEventArgs : EventArgs
    {
        internal bool IsTimeout;
        internal PNCurrentRequestType CurrRequestType;
    }

}

