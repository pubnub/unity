using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class ListPushProvisionsBuilder
    {     
        private readonly ListPushProvisionsRequestBuilder pubBuilder;
        
        public ListPushProvisionsBuilder DeviceIDForPush (string deviceIdForPush){ 
            pubBuilder.DeviceId(deviceIdForPush);
            return this;
        }

        public ListPushProvisionsBuilder PushType(PNPushType pnPushType) {
            pubBuilder.PushType = pnPushType;
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
