using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class DeleteMessagesBuilder
    {     
        private DeleteMessagesRequestBuilder pubBuilder;
        
        public DeleteMessagesBuilder(PubNubUnity pn){
            pubBuilder = new DeleteMessagesRequestBuilder(pn);

            Debug.Log ("DeleteBuilder Construct");
        }

        public DeleteMessagesBuilder Start(long start){
            pubBuilder.Start(start);
            return this;
        }

        public DeleteMessagesBuilder End(long end){
            pubBuilder.End(end);
            return this;
        }

        public DeleteMessagesBuilder Channel(string channel){
            pubBuilder.Channel(channel);
            return this;
        }

        public void Async(Action<PNDeleteMessagesResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}