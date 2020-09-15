using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class GetChannelMetadataBuilder
    {
        private readonly GetChannelMetadataRequestBuilder getChannelMetadataRequestBuilder;

        public GetChannelMetadataBuilder(PubNubUnity pn)
        {
            getChannelMetadataRequestBuilder = new GetChannelMetadataRequestBuilder(pn);
        }
        public GetChannelMetadataBuilder Include(PNChannelMetadataInclude[] include)
        {
            getChannelMetadataRequestBuilder.Include(include);
            return this;
        }

        public GetChannelMetadataBuilder Channel(string id)
        {
            getChannelMetadataRequestBuilder.Channel(id);
            return this;
        }

        public GetChannelMetadataBuilder QueryParam(Dictionary<string, string> queryParam)
        {
            getChannelMetadataRequestBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNChannelMetadataResult, PNStatus> callback)
        {
            getChannelMetadataRequestBuilder.Async(callback);
        }
    }
}