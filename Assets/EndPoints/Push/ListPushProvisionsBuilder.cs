using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class ListPushProvisionsBuilder
    {     
        private ListPushProvisionsRequestBuilder pubBuilder;
        
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
