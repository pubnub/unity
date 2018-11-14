using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetChannelGroupsBuilder
    {     
        private readonly GetChannelGroupsRequestBuilder pubBuilder;
        
        public GetChannelGroupsBuilder(PubNubUnity pn){
            pubBuilder = new GetChannelGroupsRequestBuilder(pn);
        }
        public void Async(Action<PNChannelGroupsListAllResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}
