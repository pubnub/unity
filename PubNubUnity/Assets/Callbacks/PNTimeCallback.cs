using System;

namespace PubNubAPI
{
    internal class PNTimeCallback<T>: PNCallback<T>
    {
        Action<T, PNStatus> callbackAction;

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

