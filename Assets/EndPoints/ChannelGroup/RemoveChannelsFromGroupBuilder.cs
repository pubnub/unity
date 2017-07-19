using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class RemoveChannelsFromGroupBuilder
    {     
        private RemoveChannelsFromGroupRequestBuilder pubBuilder;
        
        public RemoveChannelsFromGroupBuilder(PubNubUnity pn){
            pubBuilder = new RemoveChannelsFromGroupRequestBuilder(pn);

            Debug.Log ("RemoveChannelsFromGroupBuilder Construct");
        }
        public void Async(Action<PNChannelGroupsRemoveChannelResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}
