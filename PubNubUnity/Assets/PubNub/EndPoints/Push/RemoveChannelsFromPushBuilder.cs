using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class RemoveChannelsFromPushBuilder
    {     
        private readonly RemoveChannelsFromPushRequestBuilder pubBuilder;
        
        public RemoveChannelsFromPushBuilder Channels(List<string> channelNames){
            pubBuilder.Channels(channelNames);
            return this;
        }

        public RemoveChannelsFromPushBuilder DeviceID (string deviceIdForPush){ 
            pubBuilder.DeviceId(deviceIdForPush);
            return this;
        }

        public RemoveChannelsFromPushBuilder PushType(PNPushType pnPushType) {
            pubBuilder.PushType = pnPushType;
            return this;
        }
        public RemoveChannelsFromPushBuilder QueryParam(Dictionary<string, string> queryParam){
            pubBuilder.QueryParam(queryParam);
            return this;
        }
        public RemoveChannelsFromPushBuilder(PubNubUnity pn){
            pubBuilder = new RemoveChannelsFromPushRequestBuilder(pn);
        }

        public RemoveChannelsFromPushBuilder Topic(string topic) {
            pubBuilder.Topic(topic);
            return this;
        }

        public RemoveChannelsFromPushBuilder Environment(PNPushEnvironment environment) {
            pubBuilder.Environment(environment);
            return this;
        }
        
        public void Async(Action<PNPushRemoveChannelResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}
