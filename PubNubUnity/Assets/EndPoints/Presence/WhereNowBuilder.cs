using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class WhereNowBuilder
    {     
        private readonly WhereNowRequestBuilder pubBuilder;
        
        public WhereNowBuilder(PubNubUnity pn){
            pubBuilder = new WhereNowRequestBuilder(pn);
        }
        public WhereNowBuilder Uuid(string uuidForWhereNow){
            pubBuilder.Uuid(uuidForWhereNow);
            return this;
        }

        public void Async(Action<PNWhereNowResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}