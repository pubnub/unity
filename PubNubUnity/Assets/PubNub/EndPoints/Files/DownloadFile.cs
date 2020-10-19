using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class DownloadFileBuilder
    {
        private readonly DownloadFileRequestBuilder downloadFileBuilder;

        public DownloadFileBuilder(PubNubUnity pn)
        {
            downloadFileBuilder = new DownloadFileRequestBuilder(pn);
        }

        public DownloadFileBuilder Channel(string channel)
        {
            downloadFileBuilder.Channel(channel);
            return this;
        }
        public DownloadFileBuilder CipherKey(string cipherKey)
        {
            downloadFileBuilder.CipherKey(cipherKey);
            return this;
        }
        public DownloadFileBuilder ID(string id){
            downloadFileBuilder.ID(id);
            return this;
        }

        public DownloadFileBuilder Name(string name){
            downloadFileBuilder.Name(name);
            return this;
        }
        public DownloadFileBuilder SavePath(string path){
            downloadFileBuilder.SavePath(path);
            return this;
        } 

        public void Async(Action<PNDownloadFileResult, PNStatus> callback)
        {
            downloadFileBuilder.Async(callback);
        }
    }

}