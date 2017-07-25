using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class AddChannelsToPushBuilder
    {     
        private AddChannelsToPushRequestBuilder pubBuilder;
        
        public AddChannelsToPushBuilder Channels(List<string> channels){
            pubBuilder.Channels(channels);
            return this;
        }
        
        public AddChannelsToPushBuilder DeviceIDForPush (string deviceId){ 
            pubBuilder.DeviceId(deviceId);
            return this;
        }

        public AddChannelsToPushBuilder PushType(PNPushType pushType) {
            pubBuilder.PushType = pushType;
            return this;
        }

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
