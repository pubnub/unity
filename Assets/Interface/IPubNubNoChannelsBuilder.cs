using System;

namespace PubNubAPI
{
    internal interface IPubNubNoChannelsBuilder<U, V>
    {
        void Async(Action<V, PNStatus> callback);
    }
}

