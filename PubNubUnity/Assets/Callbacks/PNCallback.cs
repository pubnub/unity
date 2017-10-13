using System;
using System.Collections;

namespace PubNubAPI
{
    internal abstract class PNCallback<T>
    {
        internal abstract void OnResponse(T result, PNStatus status);
    }
}

