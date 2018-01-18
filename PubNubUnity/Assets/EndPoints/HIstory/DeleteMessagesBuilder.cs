using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class DeleteMessagesBuilder
    {     
        private readonly DeleteMessagesRequestBuilder pubBuilder;
        
        public DeleteMessagesBuilder(PubNubUnity pn){
            pubBuilder = new DeleteMessagesRequestBuilder(pn);
        }

        public DeleteMessagesBuilder Start(long startTime){
            pubBuilder.Start(startTime);
            return this;
        }

        public DeleteMessagesBuilder End(long endTime){
            pubBuilder.End(endTime);
            return this;
        }

        public DeleteMessagesBuilder Channel(string channelName){
            pubBuilder.Channel(channelName);
            return this;
        }

        public void Async(Action<PNDeleteMessagesResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}