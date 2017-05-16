using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class TimeBuilder: PubNubBuilder<TimeBuilder>, IPubNubNoChannelsBuilder<TimeBuilder, PNTimeResult>
    {
        public TimeBuilder(PubNub pn):base(pn){
            
        }
        #region IPubNubBuilder implementation

        public void Async(Action<PNTimeResult, PNStatus> callback)
        {
            
            Debug.Log ("TimeBuilder Async");
            base.Async<PNTimeResult>(callback, PNOperationType.PNTimeOperation, CurrentRequestType.NonSubscribe, this);
        }
        #endregion
    }
}

