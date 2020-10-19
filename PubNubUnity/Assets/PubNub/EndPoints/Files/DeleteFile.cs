using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class DeleteFileBuilder
    {
        private readonly DeleteFileRequestBuilder deleteFileBuilder;

        public DeleteFileBuilder(PubNubUnity pn)
        {
            deleteFileBuilder = new DeleteFileRequestBuilder(pn);
        }

        public DeleteFileBuilder Channel(string channel)
        {
            deleteFileBuilder.Channel(channel);
            return this;
        }
        public DeleteFileBuilder ID(string id){
            deleteFileBuilder.ID(id);
            return this;
        }

        public DeleteFileBuilder Name(string name){
            deleteFileBuilder.Name(name);
            return this;
        }

        public DeleteFileBuilder QueryParam(Dictionary<string, string> queryParam)
        {
            deleteFileBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNDeleteFileResult, PNStatus> callback)
        {
            deleteFileBuilder.Async(callback);
        }
    }

}