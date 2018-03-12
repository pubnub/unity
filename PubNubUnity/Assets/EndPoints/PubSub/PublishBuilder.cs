using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class PublishBuilder
    {     
        private readonly PublishRequestBuilder pubBuilder;
        
        public PublishBuilder(PubNubUnity pn, uint counter){
            pubBuilder = new PublishRequestBuilder(pn, counter);
        }
        public PublishBuilder Message(object messageToPublish){
            pubBuilder.Message(messageToPublish);
            return this;
        }

        public PublishBuilder Channel(string channelName){
            pubBuilder.Channel(channelName);
            return this;
        }

        public PublishBuilder ShouldStore(bool shouldStoreInHistory){
            pubBuilder.ShouldStore(shouldStoreInHistory);
            return this;
        }

        public PublishBuilder UsePost(bool usePostRequest){
            pubBuilder.UsePost(usePostRequest);
            return this;
        }

        public PublishBuilder Meta(Dictionary<string, string> metadata){
            pubBuilder.Meta(metadata);
            return this;
        }

        public PublishBuilder PublishAsIs(bool publishMessageAsIs){
            pubBuilder.PublishAsIs(publishMessageAsIs);
            return this;
        }


        public PublishBuilder Replicate(bool replicateMessage){
            pubBuilder.Replicate(replicateMessage);
            return this;
        }

        public PublishBuilder Ttl(int publishTTL){
            pubBuilder.Ttl(publishTTL);
            return this;
        }

        public void Async(Action<PNPublishResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}