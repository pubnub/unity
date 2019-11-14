using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class DeleteSpaceBuilder
    {
        private readonly DeleteSpaceRequestBuilder deleteSpaceBuilder;

        public DeleteSpaceBuilder(PubNubUnity pn)
        {
            deleteSpaceBuilder = new DeleteSpaceRequestBuilder(pn);
        }

        public DeleteSpaceBuilder ID(string id)
        {
            deleteSpaceBuilder.ID(id);
            return this;
        }

        public DeleteSpaceBuilder QueryParam(Dictionary<string, string> queryParam)
        {
            deleteSpaceBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNDeleteSpaceResult, PNStatus> callback)
        {
            deleteSpaceBuilder.Async(callback);
        }
    }

}