using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class ListPushProvisionsBuilder
    {     
        private readonly ListPushProvisionsRequestBuilder pubBuilder;
        
        public ListPushProvisionsBuilder DeviceID (string deviceIdForPush){ 
            pubBuilder.DeviceId(deviceIdForPush);
            return this;
        }

        public ListPushProvisionsBuilder PushType(PNPushType pnPushType) {
            pubBuilder.PushType = pnPushType;
            return this;
        }
        public ListPushProvisionsBuilder QueryParam(Dictionary<string, string> queryParam){
            pubBuilder.QueryParam(queryParam);
            return this;
        }

        public ListPushProvisionsBuilder(PubNubUnity pn){
            pubBuilder = new ListPushProvisionsRequestBuilder(pn);
        }

        public ListPushProvisionsBuilder Topic(string topic) {
            pubBuilder.Topic(topic);
            return this;
        }

        public ListPushProvisionsBuilder Environment(PNPushEnvironment environment) {
            pubBuilder.Environment(environment);
            return this;
        }

        public void Async(Action<PNPushListProvisionsResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}
