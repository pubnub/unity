using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class FetchBuilder
    {     
        private FetchMessagesRequestBuilder pubBuilder;
        
        public FetchBuilder(PubNubUnity pn){
            pubBuilder = new FetchMessagesRequestBuilder(pn);

            Debug.Log ("RemoveChannelsFromPushRequestBuilder Construct");
        }
        public void Async(Action<PNFetchMessagesResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}
