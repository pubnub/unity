using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class GetSpacesBuilder
    {
        private readonly GetSpacesRequestBuilder getSpacesBuilder;

        public GetSpacesBuilder(PubNubUnity pn)
        {
            getSpacesBuilder = new GetSpacesRequestBuilder(pn);
        }
        public GetSpacesBuilder Include(PNUserSpaceInclude[] include)
        {
            getSpacesBuilder.Include(include);
            return this;
        }

        public GetSpacesBuilder Limit(int limit)
        {
            getSpacesBuilder.Limit(limit);
            return this;
        }

        public GetSpacesBuilder Start(string start)
        {
            getSpacesBuilder.Start(start);
            return this;
        }
        public GetSpacesBuilder End(string end)
        {
            getSpacesBuilder.End(end);
            return this;
        }
        public GetSpacesBuilder Filter(string filter)
        {
            getSpacesBuilder.Filter(filter);
            return this;
        }
        public GetSpacesBuilder Sort(List<string> sortBy)
        {
            getSpacesBuilder.Sort(sortBy);
            return this;
        }
        public GetSpacesBuilder Count(bool count)
        {
            getSpacesBuilder.Count(count);
            return this;
        }
        public GetSpacesBuilder QueryParam(Dictionary<string, string> queryParam)
        {
            getSpacesBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNGetSpacesResult, PNStatus> callback)
        {
            getSpacesBuilder.Async(callback);
        }
    }
}