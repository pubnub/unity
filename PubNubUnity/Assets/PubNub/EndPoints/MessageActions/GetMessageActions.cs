using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetMessageActionsBuilder
    {     
        private readonly GetMessageActionsRequestBuilder getMessageActionsBuilder;
        
        public GetMessageActionsBuilder(PubNubUnity pn){
            getMessageActionsBuilder = new GetMessageActionsRequestBuilder(pn);
        }
        public GetMessageActionsBuilder Channel(string channel){
            getMessageActionsBuilder.Channel(channel);
            return this;
        }
        public GetMessageActionsBuilder Limit(int limit){
            getMessageActionsBuilder.Limit(limit);
            return this;
        }
        public GetMessageActionsBuilder Start(long start){
            getMessageActionsBuilder.Start(start);
            return this;
        }
        public GetMessageActionsBuilder End(long end){
            getMessageActionsBuilder.End(end);
            return this;
        }
        public GetMessageActionsBuilder QueryParam(Dictionary<string, string> queryParam){
            getMessageActionsBuilder.QueryParam(queryParam);
            return this;
        }
        public void Async(Action<PNGetMessageActionsResult, PNStatus> callback)
        {
            getMessageActionsBuilder.Async(callback);
        }
    }
}