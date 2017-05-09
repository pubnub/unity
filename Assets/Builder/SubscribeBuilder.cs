using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class SubscribeBuilder: OperationParams ,IPubNubSubcribeBuilder<SubscribeBuilder>
    {
        private PubNubBuilder<SubscribeBuilder> pubNubBuilder;

        #region IPubNubBuilder implementation

        public void Execute(PNOperationType pnOpType, OperationParams operationParams){
            pubNubBuilder.Execute (PNOperationType.PNSubscribeOperation, this);
        }

        public SubscribeBuilder SetChannels(List<string> channels){
            pubNubBuilder.SetChannels (channels);
            return this;
        }

        public SubscribeBuilder SetChannelGroups(List<string> channelGroups){
            pubNubBuilder.SetChannelGroups(channelGroups);
            return this;
        }

        #endregion
    }
}

