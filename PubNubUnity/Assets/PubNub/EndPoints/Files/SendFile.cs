using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class SendFileBuilder
    {
        private readonly SendFileRequestBuilder sendFileBuilder;

        public SendFileBuilder(PubNubUnity pn)
        {
            sendFileBuilder = new SendFileRequestBuilder(pn);
        }

        public SendFileBuilder Channel(string channel)
        {
            sendFileBuilder.Channel(channel);
            return this;
        }

        public SendFileBuilder Message(string messageToPublish){
            sendFileBuilder.Message(messageToPublish);
            return this;
        }

        public SendFileBuilder FilePath(string path){
            sendFileBuilder.FilePath(path);
            return this;
        }

        public SendFileBuilder ShouldStore(bool shouldStoreInHistory){
            sendFileBuilder.ShouldStore(shouldStoreInHistory);
            return this;
        }

        public SendFileBuilder Meta(Dictionary<string, string> metadata){
            sendFileBuilder.Meta(metadata);
            return this;
        }

        public SendFileBuilder TTL(int publishTTL){
            sendFileBuilder.TTL(publishTTL);
            return this;
        }

        public SendFileBuilder Name(string name){
            sendFileBuilder.Name(name);
            return this;
        }

        public SendFileBuilder CipherKey(string cipherKey)
        {
            sendFileBuilder.CipherKey(cipherKey);
            return this;
        }
        
        public SendFileBuilder QueryParam(Dictionary<string, string> queryParam)
        {
            sendFileBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNSendFileResult, PNStatus> callback)
        {
            sendFileBuilder.Async(callback);
        }
    }

}