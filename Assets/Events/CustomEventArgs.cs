using System;

namespace PubNubAPI
{
    internal class CustomEventArgs<T> : EventArgs
    {
        internal string Message;
        internal RequestState<T> PubnubRequestState;
        //internal Action<T, PNStatus> Callback;
        internal bool IsError;
        internal bool IsTimeout;
        internal CurrentRequestType CurrRequestType;
    }

    internal class CurrentRequestTypeEventArgs : EventArgs
    {
        internal bool IsTimeout;
        internal CurrentRequestType CurrRequestType;
    }

}

