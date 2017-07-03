using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class PublishBuilder
    {     
        private PublishRequestBuilder pubBuilder;
        
        public PublishBuilder(PubNubUnity pn, uint counter){
            pubBuilder = new PublishRequestBuilder(pn, counter);

            Debug.Log ("PublishBuilder Construct");
        }
        public PublishBuilder Message(object message){
            pubBuilder.Message(message);
            return this;
        }

        public PublishBuilder Channel(string channel){
            pubBuilder.Channel(channel);
            return this;
        }

        public PublishBuilder ShouldStore(bool shouldStore){
            pubBuilder.ShouldStore(shouldStore);
            return this;
        }

        public PublishBuilder UsePost(bool usePost){
            pubBuilder.UsePost(usePost);
            return this;
        }

        public PublishBuilder Meta(Dictionary<string, string> meta){
            pubBuilder.Meta(meta);
            return this;
        }

        public PublishBuilder PublishAsIs(bool publishAsIs){
            pubBuilder.PublishAsIs(publishAsIs);
            return this;
        }


        public PublishBuilder Replicate(bool replicate){
            pubBuilder.Replicate(replicate);
            return this;
        }

        public PublishBuilder Ttl(int ttl){
            pubBuilder.Ttl(ttl);
            return this;
        }

        public void Async(Action<PNPublishResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}