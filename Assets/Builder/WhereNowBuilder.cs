using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class WhereNowBuilder: OperationParams, IPubNubNoChannelsBuilder<WhereNowBuilder, PNWhereNowResult>
    {
        private PubNubBuilder<WhereNowBuilder> pubNubBuilder;

        /*public TimeBuilder(PNConfiguration pnConfig){
            Debug.Log ("TimeBuilder Construct");
            pubNubBuilder = new PubNubBuilder<TimeBuilder> (pnConfig);
        }*/
        public string UuidForWhereNow { get; private set;}

        public WhereNowBuilder(){
            Debug.Log ("WhereNowBuilder Construct");
            pubNubBuilder = new PubNubBuilder<WhereNowBuilder> ();
        }

        public WhereNowBuilder Uuid(string uuid){
            UuidForWhereNow = uuid;
            return this;
        }

        #region IPubNubBuilder implementation

        public void Async(Action<PNWhereNowResult, PNStatus> callback)
        {
            Debug.Log ("PNWherNowResult Async");
            //WhereNowOperationParams whereNowOperationParams = new WhereNowOperationParams (UuidForWhereNow);

            //RequestQueue.Instance.Enqueue<PNTimeResult>(PNConfig, callback, PNOperationType.PNTimeOperation, null);
            //pubNubBuilder.Async<PNWhereNowResult>(callback, PNOperationType.PNWhereNowOperation, whereNowOperationParams);
            pubNubBuilder.Async<PNWhereNowResult>(callback, PNOperationType.PNWhereNowOperation, this, CurrentRequestType.NonSubscribe);
        }
        #endregion
    }
}

