using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class RemoveAllPushChannelsForDeviceBuilder
    {     
        private RemoveAllPushChannelsForDeviceRequestBuilder pubBuilder;
        
        public RemoveAllPushChannelsForDeviceBuilder(PubNubUnity pn){
            pubBuilder = new RemoveAllPushChannelsForDeviceRequestBuilder(pn);

            Debug.Log ("RemoveAllPushChannelsForDeviceBuilder Construct");
        }
        public void Async(Action<PNPushRemoveAllChannelsResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}
