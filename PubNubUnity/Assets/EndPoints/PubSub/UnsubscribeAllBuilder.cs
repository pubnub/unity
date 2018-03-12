using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class UnsubscribeAllBuilder
    {     
        private readonly LeaveRequestBuilder pubBuilder;

        public UnsubscribeAllBuilder(PubNubUnity pn){
            pubBuilder = new LeaveRequestBuilder(pn);
        }
        public void Async(Action<PNLeaveRequestResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}