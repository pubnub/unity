
using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class SetChannelMetadataBuilder
    {
        private readonly SetChannelMetadataRequestBuilder setChannelMetadataRequestBuilder;

        public SetChannelMetadataBuilder(PubNubUnity pn)
        {
            setChannelMetadataRequestBuilder = new SetChannelMetadataRequestBuilder(pn);
        }
        public SetChannelMetadataBuilder Include(PNChannelMetadataInclude[] include)
        {
            setChannelMetadataRequestBuilder.Include(include);
            return this;
        }

        public SetChannelMetadataBuilder Channel(string id)
        {
            setChannelMetadataRequestBuilder.Channel(id);
            return this;
        }

        public SetChannelMetadataBuilder Name(string name)
        {
            setChannelMetadataRequestBuilder.Name(name);
            return this;
        }

        public SetChannelMetadataBuilder Description(string description)
        {
            setChannelMetadataRequestBuilder.Description(description);
            return this;
        }

        public SetChannelMetadataBuilder Custom(Dictionary<string, object> custom)
        {
            setChannelMetadataRequestBuilder.Custom(custom);
            return this;
        }

        public SetChannelMetadataBuilder QueryParam(Dictionary<string, string> queryParam)
        {
            setChannelMetadataRequestBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNChannelMetadataResult, PNStatus> callback)
        {
            setChannelMetadataRequestBuilder.Async(callback);
        }
    }
}