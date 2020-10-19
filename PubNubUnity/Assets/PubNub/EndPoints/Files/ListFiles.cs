using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class ListFilesBuilder
    {
        private readonly ListFilesRequestBuilder listFilesBuilder;

        public ListFilesBuilder(PubNubUnity pn)
        {
            listFilesBuilder = new ListFilesRequestBuilder(pn);
        }

        public ListFilesBuilder Channel(string channel)
        {
            listFilesBuilder.Channel(channel);
            return this;
        }
        public ListFilesBuilder Limit(int limit){
            listFilesBuilder.Limit(limit);
            return this;
        }

        public ListFilesBuilder Next(string next){
            listFilesBuilder.Next(next);
            return this;
        }

        public ListFilesBuilder QueryParam(Dictionary<string, string> queryParam)
        {
            listFilesBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNListFilesResult, PNStatus> callback)
        {
            listFilesBuilder.Async(callback);
        }
    }

}