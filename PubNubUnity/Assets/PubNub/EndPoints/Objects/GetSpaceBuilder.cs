using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class GetSpaceBuilder
    {
        private readonly GetSpaceRequestBuilder getSpaceBuilder;

        public GetSpaceBuilder(PubNubUnity pn)
        {
            getSpaceBuilder = new GetSpaceRequestBuilder(pn);
        }
        public GetSpaceBuilder Include(PNUserSpaceInclude[] include)
        {
            getSpaceBuilder.Include(include);
            return this;
        }

        public GetSpaceBuilder ID(string id)
        {
            getSpaceBuilder.ID(id);
            return this;
        }

        public GetSpaceBuilder QueryParam(Dictionary<string, string> queryParam)
        {
            getSpaceBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNSpaceResult, PNStatus> callback)
        {
            getSpaceBuilder.Async(callback);
        }
    }
}