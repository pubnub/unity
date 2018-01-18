using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class FireBuilder
    {     
        private readonly PublishRequestBuilder pubBuilder;
        
        public FireBuilder(PubNubUnity pn, uint counter){
            pubBuilder = new PublishRequestBuilder(pn, counter);
        }
        public FireBuilder Message(object messageToPublish){
            pubBuilder.Message(messageToPublish);
            return this;
        }

        public FireBuilder Channel(string channelName){
            pubBuilder.Channel(channelName);
            return this;
        }

        public FireBuilder UsePost(bool usePostRequest){
            pubBuilder.UsePost(usePostRequest);
            return this;
        }

        public FireBuilder Meta(Dictionary<string, string> metadata){
            pubBuilder.Meta(metadata);
            return this;
        }

        public FireBuilder PublishAsIs(bool publishMessageAsIs){
            pubBuilder.PublishAsIs(publishMessageAsIs);
            return this;
        }

        public void Async(Action<PNPublishResult, PNStatus> callback)
        {
            pubBuilder.Replicate(false);
            pubBuilder.ShouldStore(false);
            pubBuilder.Async(callback);
        }
    }
}