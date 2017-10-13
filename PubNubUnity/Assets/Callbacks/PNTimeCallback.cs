using System;

namespace PubNubAPI
{
    internal class PNTimeCallback<T>: PNCallback<T>
    {
        Action<T, PNStatus> callbackAction = null;
        string message;

        //public PNTimeCallback(Action<T, PNStatus> callback)
        //internal PNCallback(CustomEventArgs<T> cea)
        public PNTimeCallback()
        {
            //this.callbackAction = callback;
            //this.message = cea.Message;
            //this.callbackAction = cea.Callback;
        }

        //public PNTimeCallback(Action<T, PNStatus> callback)
        //internal PNTimeCallback(CustomEventArgs cea)
        //{
            //this.callbackAction = callback;
        //}

        #region implemented abstract members of PNCallback

        internal override void OnResponse (T result, PNStatus status)
        {
            if(callbackAction != null)
                callbackAction.Invoke(result, status);
        }

        #endregion
    }
}

