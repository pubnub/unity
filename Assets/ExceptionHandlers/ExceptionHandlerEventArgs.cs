using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    internal class ExceptionHandlerEventArgs<T> : EventArgs
    {
        internal List<ChannelEntity> channelEntities;
        internal bool reconnectMaxTried;
        //internal bool resumeOnReconnect;
        internal PNOperationType responseType;
    }
}