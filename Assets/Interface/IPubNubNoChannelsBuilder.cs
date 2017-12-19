using System;

namespace PubNubAPI
{
    internal interface IPubNubNonSubscribeBuilder<U, V>
    {
        void Async(Action<V, PNStatus> callback);

    }
}

