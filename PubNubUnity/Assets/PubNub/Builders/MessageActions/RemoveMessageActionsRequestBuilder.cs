using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class RemoveMessageActionsRequestBuilder: PubNubNonSubBuilder<RemoveMessageActionsRequestBuilder, PNRemoveMessageActionsResult>, IPubNubNonSubscribeBuilder<RemoveMessageActionsRequestBuilder, PNRemoveMessageActionsResult>
    {        
        private long  RemoveMessageActionsActionTimetoken { get; set;}
        private string  RemoveMessageActionsChannel { get; set;}
        private long  RemoveMessageActionsMessageTimetoken { get; set;}

        public RemoveMessageActionsRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNRemoveMessageActionsOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNRemoveMessageActionsResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public RemoveMessageActionsRequestBuilder Channel(string channel)
        {
             RemoveMessageActionsChannel = channel;
            return this;
        }
        public RemoveMessageActionsRequestBuilder MessageTimetoken(long messageTimetoken){
             RemoveMessageActionsMessageTimetoken = messageTimetoken;
            return this;
        }
        public RemoveMessageActionsRequestBuilder ActionTimetoken(long actionTimetoken){
             RemoveMessageActionsActionTimetoken = actionTimetoken;
            return this;
        }
        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;
            requestState.httpMethod = HTTPMethod.Delete;

            Uri request = BuildRequests.BuildRemoveMessageActionsRequest(
                     RemoveMessageActionsChannel,
                     RemoveMessageActionsMessageTimetoken.ToString(),
                     RemoveMessageActionsActionTimetoken.ToString(),
                    this.PubNubInstance,
                    this.QueryParams
                );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNRemoveMessageActionsResult pnRemoveMessageActionsResult = new PNRemoveMessageActionsResult();
            PNStatus pnStatus = new PNStatus();

            try{
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
                
                if(dictionary == null) {
                    pnRemoveMessageActionsResult = null;
                    pnStatus = base.CreateErrorResponseFromException(new PubNubException("Data not present"), requestState, PNStatusCategory.PNUnknownCategory);
                }
            } catch (Exception ex){
                pnRemoveMessageActionsResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnRemoveMessageActionsResult, pnStatus);

        }

    }
}