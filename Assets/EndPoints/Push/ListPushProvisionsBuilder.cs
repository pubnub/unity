using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class ListPushProvisionsBuilder
    {     
        private ListPushProvisionsRequestBuilder pubBuilder;
        
        public ListPushProvisionsBuilder DeviceIDForPush (string deviceId){ 
            pubBuilder.DeviceId(deviceId);
            return this;
        }

        public ListPushProvisionsBuilder PushType(PNPushType pushType) {
            pubBuilder.PushType = pushType;
            return this;
        }

        public ListPushProvisionsBuilder(PubNubUnity pn){
            pubBuilder = new ListPushProvisionsRequestBuilder(pn);

            Debug.Log ("ListPushProvisionsRequestBuilder Construct");
        }
        public void Async(Action<PNPushListProvisionsResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}
