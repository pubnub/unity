using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetAllChannelsForGroupBuilder
    {     
        private GetAllChannelsForGroupRequestBuilder pubBuilder;
        
        public GetAllChannelsForGroupBuilder(PubNubUnity pn){
            pubBuilder = new GetAllChannelsForGroupRequestBuilder(pn);

            Debug.Log ("GetAllChannelsForGroupRequestBuilder Construct");
        }
        public void Async(Action<PNChannelGroupsAllChannelsResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}