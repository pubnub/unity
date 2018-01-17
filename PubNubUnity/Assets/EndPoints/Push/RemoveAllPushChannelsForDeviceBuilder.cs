using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class RemoveAllPushChannelsForDeviceBuilder
    {     
        private readonly RemoveAllPushChannelsForDeviceRequestBuilder pubBuilder;
        
        public RemoveAllPushChannelsForDeviceBuilder DeviceIDForPush (string deviceIdForPush){ 
            pubBuilder.DeviceId(deviceIdForPush);
            return this;
        }

        public RemoveAllPushChannelsForDeviceBuilder PushType(PNPushType pnPushType) {
            pubBuilder.PushType = pnPushType;
            return this;
        }

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
