using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class RemoveChannelMetadataBuilder
    {
        private readonly RemoveChannelMetadataRequestBuilder removeChannelMetadataRequestBuilder;

        public RemoveChannelMetadataBuilder(PubNubUnity pn)
        {
            removeChannelMetadataRequestBuilder = new RemoveChannelMetadataRequestBuilder(pn);
        }

        public RemoveChannelMetadataBuilder Channel(string id)
        {
            removeChannelMetadataRequestBuilder.Channel(id);
            return this;
        }

        public RemoveChannelMetadataBuilder QueryParam(Dictionary<string, string> queryParam)
        {
            removeChannelMetadataRequestBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNRemoveChannelMetadataResult, PNStatus> callback)
        {
            removeChannelMetadataRequestBuilder.Async(callback);
        }
    }

}