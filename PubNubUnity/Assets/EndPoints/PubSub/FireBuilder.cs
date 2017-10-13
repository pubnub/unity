using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class FireBuilder
    {     
        private PublishRequestBuilder pubBuilder;
        
        public FireBuilder(PubNubUnity pn, uint counter){
            pubBuilder = new PublishRequestBuilder(pn, counter);

            Debug.Log ("FireBuilder Construct");
        }
        public FireBuilder Message(object message){
            pubBuilder.Message(message);
            return this;
        }

        public FireBuilder Channel(string channel){
            pubBuilder.Channel(channel);
            return this;
        }

        public FireBuilder UsePost(bool usePost){
            pubBuilder.UsePost(usePost);
            return this;
        }

        public FireBuilder Meta(Dictionary<string, string> meta){
            pubBuilder.Meta(meta);
            return this;
        }

        public FireBuilder PublishAsIs(bool publishAsIs){
            pubBuilder.PublishAsIs(publishAsIs);
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