using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class RemoveChannelsFromPushBuilder
    {     
        private RemoveChannelsFromPushRequestBuilder pubBuilder;
        
        public RemoveChannelsFromPushBuilder Channels(List<string> channels){
            pubBuilder.Channels(channels);
            return this;
        }

        public RemoveChannelsFromPushBuilder DeviceIDForPush (string deviceId){ 
            pubBuilder.DeviceId(deviceId);
            return this;
        }

        public RemoveChannelsFromPushBuilder PushType(PNPushType pushType) {
            pubBuilder.PushType = pushType;
            return this;
        }
        public RemoveChannelsFromPushBuilder(PubNubUnity pn){
            pubBuilder = new RemoveChannelsFromPushRequestBuilder(pn);

            Debug.Log ("RemoveChannelsFromPushRequestBuilder Construct");
        }
        public void Async(Action<PNPushRemoveChannelResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}
