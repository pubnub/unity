using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class WhereNowBuilder
    {     
        private WhereNowRequestBuilder pubBuilder;
        
        public WhereNowBuilder(PubNubUnity pn){
            pubBuilder = new WhereNowRequestBuilder(pn);

            Debug.Log ("WhereNowBuilder Construct");
        }
        public WhereNowBuilder Uuid(string uuid){
            pubBuilder.Uuid(uuid);
            return this;
        }

        public void Async(Action<PNWhereNowResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}