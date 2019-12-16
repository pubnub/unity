using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class RemoveAllPushChannelsForDeviceBuilder
    {     
        private readonly RemoveAllPushChannelsForDeviceRequestBuilder pubBuilder;
        
        public RemoveAllPushChannelsForDeviceBuilder DeviceID (string deviceIdForPush){ 
            pubBuilder.DeviceId(deviceIdForPush);
            return this;
        }

        public RemoveAllPushChannelsForDeviceBuilder PushType(PNPushType pnPushType) {
            pubBuilder.PushType = pnPushType;
            return this;
        }
        public RemoveAllPushChannelsForDeviceBuilder QueryParam(Dictionary<string, string> queryParam){
            pubBuilder.QueryParam(queryParam);
            return this;
        }

        public RemoveAllPushChannelsForDeviceBuilder(PubNubUnity pn){
            pubBuilder = new RemoveAllPushChannelsForDeviceRequestBuilder(pn);
        }

        public RemoveAllPushChannelsForDeviceBuilder Topic(string topic) {
            pubBuilder.Topic(topic);
            return this;
        }

        public RemoveAllPushChannelsForDeviceBuilder Environment(PNPushEnvironment environment) {
            pubBuilder.Environment(environment);
            return this;
        }

        public void Async(Action<PNPushRemoveAllChannelsResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}
