using System;

namespace PubNubAPI
{
    internal class PNTimeCallback<T>: PNCallback<T>
    {
        Action<T, PNStatus> callbackAction = null;
        string message;

        public PNTimeCallback()
        {
        }

        #region implemented abstract members of PNCallback

        internal override void OnResponse (T result, PNStatus status)
        {
            if(callbackAction != null){
                callbackAction.Invoke(result, status);
            }
        }

        #endregion
    }
}

