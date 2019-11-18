using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class MessageActionAdd
    {
        public string ActionType {get; set;}
        public string ActionValue {get; set;}
    }

    public class AddMessageActionsRequestBuilder: PubNubNonSubBuilder<AddMessageActionsRequestBuilder, PNMessageActionsResult>, IPubNubNonSubscribeBuilder<AddMessageActionsRequestBuilder, PNMessageActionsResult>
    {        
        private string AddMessageActionsChannel { get; set;}
        private long AddMessageActionsMessageTimetoken { get; set;}
        private MessageActionAdd MessageActionAdd { get; set;}
        public AddMessageActionsRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNAddMessageActionsOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNMessageActionsResult, PNStatus> callback)
        {
            this.Callback = callback;
            if(MessageActionAdd == null){
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("Message Action null", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);
                return;
            }            
            base.Async(this);
        }
        #endregion

        public AddMessageActionsRequestBuilder Channel(string channel){
            AddMessageActionsChannel = channel;
            return this;
        }

        public AddMessageActionsRequestBuilder MessageTimetoken(long messageTimetoken){
            AddMessageActionsMessageTimetoken = messageTimetoken;
            return this;
        }

        public AddMessageActionsRequestBuilder MessageAction(MessageActionAdd action){
            MessageActionAdd = action;
            return this;
        }

        protected override void RunWebRequest(QueueManager qm){            
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;
            requestState.httpMethod = HTTPMethod.Post;

            var cub = new { 
                type = MessageActionAdd.ActionType, 
                value = MessageActionAdd.ActionValue,
            };

            string jsonUserBody = Helpers.JsonEncodePublishMsg (cub, "", this.PubNubInstance.JsonLibrary, this.PubNubInstance.PNLog);
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("jsonUserBody: {0}", jsonUserBody), PNLoggingMethod.LevelInfo);
            #endif
            requestState.POSTData = jsonUserBody;

            Uri request = BuildRequests.BuildAddMessageActionsRequest(
                    AddMessageActionsChannel,
                    AddMessageActionsMessageTimetoken.ToString(),
                    this.PubNubInstance,
                    this.QueryParams
                );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNMessageActionsResult pnMessageActionsResult = new PNMessageActionsResult();
            PNStatus pnStatus = new PNStatus();

            try{
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
                
                if(dictionary != null) {
                    object objData;
                    dictionary.TryGetValue("data", out objData);
                    if(objData!=null){
                        Dictionary<string, object> objDataDict = objData as Dictionary<string, object>;
                        if(objDataDict != null){
                            pnMessageActionsResult = MessageActionsHelpers.ExtractMessageAction(objDataDict);
                        }  else {
                            pnMessageActionsResult = null;
                            pnStatus = base.CreateErrorResponseFromException(new PubNubException("objDataDict null"), requestState, PNStatusCategory.PNUnknownCategory);
                        }  
                    }  else {
                        pnMessageActionsResult = null;
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("objData null"), requestState, PNStatusCategory.PNUnknownCategory);
                    }                      
                } 
                #if (ENABLE_PUBNUB_LOGGING)
                else {
                    this.PubNubInstance.PNLog.WriteToLog ("dictionary null", PNLoggingMethod.LevelInfo);
                }
                #endif

            } catch (Exception ex){
                pnMessageActionsResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnMessageActionsResult, pnStatus);

        }

    }
}