using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class AddChannelsToPushBuilder
    {     
        private readonly AddChannelsToPushRequestBuilder pubBuilder;
        
        public AddChannelsToPushBuilder Channels(List<string> channelNames){
            pubBuilder.Channels(channelNames);
            return this;
        }
        
        public AddChannelsToPushBuilder DeviceID (string deviceIdForPush){ 
            pubBuilder.DeviceId(deviceIdForPush);
            return this;
        }

        public AddChannelsToPushBuilder PushType(PNPushType pnPushType) {
            pubBuilder.PushType = pnPushType;
            return this;
        }

        public AddChannelsToPushBuilder(PubNubUnity pn){
            pubBuilder = new AddChannelsToPushRequestBuilder(pn);
        }
        public void Async(Action<PNPushAddChannelResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}
