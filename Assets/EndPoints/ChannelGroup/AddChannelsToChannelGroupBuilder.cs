using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class AddChannelsToChannelGroupBuilder
    {     
        private AddChannelsToChannelGroupRequestBuilder pubBuilder;
        
        public AddChannelsToChannelGroupBuilder(PubNubUnity pn){
            pubBuilder = new AddChannelsToChannelGroupRequestBuilder(pn);

            Debug.Log ("AddChannelsToChannelGroupRequestBuilder Construct");
        }
        public void Async(Action<PNChannelGroupsAddChannelResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}