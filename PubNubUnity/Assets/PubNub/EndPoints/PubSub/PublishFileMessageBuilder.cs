using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class PublishFileMessageBuilder
    {     
        private readonly PublishFileMessageRequestBuilder pubBuilder;
        
        public PublishFileMessageBuilder(PubNubUnity pn, uint counter){
            pubBuilder = new PublishFileMessageRequestBuilder(pn, counter);
        }

        public PublishFileMessageBuilder Channel(string channelName){
            pubBuilder.Channel(channelName);
            return this;
        }

        public PublishFileMessageBuilder ShouldStore(bool shouldStoreInHistory){
            pubBuilder.ShouldStore(shouldStoreInHistory);
            return this;
        }

        public PublishFileMessageBuilder Meta(Dictionary<string, string> metadata){
            pubBuilder.Meta(metadata);
            return this;
        }

        public PublishFileMessageBuilder TTL(int publishFileMessageTTL){
            pubBuilder.TTL(publishFileMessageTTL);
            return this;
        }

        public PublishFileMessageBuilder FileID(string fileID){
            pubBuilder.FileID(fileID);
            return this;
        }

        public PublishFileMessageBuilder MessageText(string message){
            pubBuilder.MessageText(message);
            return this;
        }

        public PublishFileMessageBuilder FileName(string fileName){
            pubBuilder.FileName(fileName);

            return this;
        }        

        public PublishFileMessageBuilder QueryParam(Dictionary<string, string> queryParam){
            pubBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNPublishResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}