using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class WhereNowBuilder: PubNubBuilder<WhereNowBuilder>, IPubNubNoChannelsBuilder<WhereNowBuilder, PNWhereNowResult>
    {
        public string UuidForWhereNow { get; private set;}

        public WhereNowBuilder(): base(){
            Debug.Log ("WhereNowBuilder Construct");
        }

        public WhereNowBuilder Uuid(string uuid){
            UuidForWhereNow = uuid;
            return this;
        }

        #region IPubNubBuilder implementation

        public void Async(Action<PNWhereNowResult, PNStatus> callback)
        {
            Debug.Log ("PNWherNowResult Async");
            base.Async<PNWhereNowResult>(callback, PNOperationType.PNWhereNowOperation, CurrentRequestType.NonSubscribe, this);
        }
        #endregion
    }
}

