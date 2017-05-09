using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    internal interface IPubNubSubcribeBuilder<U>
    {
        void Execute(PNOperationType pnOpType, OperationParams operationParams);

        U SetChannels (List<string> channels);
        U SetChannelGroups (List<string> channelGroups);
    }
}

