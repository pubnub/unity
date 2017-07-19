using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class AddChannelsToPushBuilder
    {     
        private AddChannelsToPushRequestBuilder pubBuilder;
        
        public AddChannelsToPushBuilder(PubNubUnity pn){
            pubBuilder = new AddChannelsToPushRequestBuilder(pn);

            Debug.Log ("AddChannelsToPushRequestBuilder Construct");
        }
        public void Async(Action<PNPushAddChannelResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}
