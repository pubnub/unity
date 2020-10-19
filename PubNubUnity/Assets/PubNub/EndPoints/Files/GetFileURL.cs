using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class GetFileURLBuilder
    {
        private readonly GetFileURLRequestBuilder getFileURLBuilder;

        public GetFileURLBuilder(PubNubUnity pn)
        {
            getFileURLBuilder = new GetFileURLRequestBuilder(pn);
        }

        public GetFileURLBuilder Channel(string channel)
        {
            getFileURLBuilder.Channel(channel);
            return this;
        }
        public GetFileURLBuilder ID(string id){
            getFileURLBuilder.ID(id);
            return this;
        }

        public GetFileURLBuilder Name(string name){
            getFileURLBuilder.Name(name);
            return this;
        }

        public GetFileURLBuilder QueryParam(Dictionary<string, string> queryParam)
        {
            getFileURLBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNGetFileURLResult, PNStatus> callback)
        {
            getFileURLBuilder.Async(callback);
        }
    }

}