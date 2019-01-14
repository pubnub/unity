using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class TimeBuilder
    {     
        private readonly TimeRequestBuilder pubBuilder;
        
        public TimeBuilder(PubNubUnity pn){
            pubBuilder = new TimeRequestBuilder(pn);
        }

        public TimeBuilder QueryParam(Dictionary<string, string> queryParam){
            pubBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNTimeResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}