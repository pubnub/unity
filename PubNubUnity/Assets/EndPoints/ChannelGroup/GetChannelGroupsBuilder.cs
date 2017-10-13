using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetChannelGroupsBuilder
    {     
        private GetChannelGroupsRequestBuilder pubBuilder;
        
        public GetChannelGroupsBuilder(PubNubUnity pn){
            pubBuilder = new GetChannelGroupsRequestBuilder(pn);

            Debug.Log ("GetChannelGroupsBuilder Construct");
        }
        public void Async(Action<PNChannelGroupsListAllResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}
