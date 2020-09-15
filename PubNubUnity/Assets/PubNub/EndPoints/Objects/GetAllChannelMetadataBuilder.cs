using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class GetAllChannelMetadataBuilder
    {
        private readonly GetAllChannelMetadataRequestBuilder getAllChannelMetadataRequestBuilder;

        public GetAllChannelMetadataBuilder(PubNubUnity pn)
        {
            getAllChannelMetadataRequestBuilder = new GetAllChannelMetadataRequestBuilder(pn);
        }
        public GetAllChannelMetadataBuilder Include(PNChannelMetadataInclude[] include)
        {
            getAllChannelMetadataRequestBuilder.Include(include);
            return this;
        }

        public GetAllChannelMetadataBuilder Limit(int limit)
        {
            getAllChannelMetadataRequestBuilder.Limit(limit);
            return this;
        }

        public GetAllChannelMetadataBuilder Start(string start)
        {
            getAllChannelMetadataRequestBuilder.Start(start);
            return this;
        }
        public GetAllChannelMetadataBuilder End(string end)
        {
            getAllChannelMetadataRequestBuilder.End(end);
            return this;
        }
        public GetAllChannelMetadataBuilder Filter(string filter)
        {
            getAllChannelMetadataRequestBuilder.Filter(filter);
            return this;
        }
        public GetAllChannelMetadataBuilder Sort(List<string> sortBy)
        {
            getAllChannelMetadataRequestBuilder.Sort(sortBy);
            return this;
        }
        public GetAllChannelMetadataBuilder Count(bool count)
        {
            getAllChannelMetadataRequestBuilder.Count(count);
            return this;
        }
        public GetAllChannelMetadataBuilder QueryParam(Dictionary<string, string> queryParam)
        {
            getAllChannelMetadataRequestBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNGetAllChannelMetadataResult, PNStatus> callback)
        {
            getAllChannelMetadataRequestBuilder.Async(callback);
        }
    }
}