using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class TimeBuilder: OperationParams,IPubNubNoChannelsBuilder<TimeBuilder, PNTimeResult>
    {
        private PubNubBuilder<TimeBuilder> pubNubBuilder;

        /*public TimeBuilder(PNConfiguration pnConfig){
            Debug.Log ("TimeBuilder Construct");
            pubNubBuilder = new PubNubBuilder<TimeBuilder> (pnConfig);
        }*/
        public TimeBuilder(){
            Debug.Log ("TimeBuilder Construct");
            pubNubBuilder = new PubNubBuilder<TimeBuilder> ();
        }

        #region IPubNubBuilder implementation

        public void Async(Action<PNTimeResult, PNStatus> callback)
        {
            Debug.Log ("TimeBuilder Async");
            //RequestQueue.Instance.Enqueue<PNTimeResult>(PNConfig, callback, PNOperationType.PNTimeOperation, null);
            pubNubBuilder.Async<PNTimeResult>(callback, PNOperationType.PNTimeOperation, null, CurrentRequestType.NonSubscribe);
        }
        #endregion
    }
}

