using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetMessageActionsRequestBuilder: PubNubNonSubBuilder<GetMessageActionsRequestBuilder, PNGetMessageActionsResult>, IPubNubNonSubscribeBuilder<GetMessageActionsRequestBuilder, PNGetMessageActionsResult>
    {        
        private string GetMessageActionsChannel { get; set;}
        private int GetMessageActionsLimit { get; set;}
        private long GetMessageActionsEnd { get; set;}
        private long GetMessageActionsStart { get; set;}
        public GetMessageActionsRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNGetMessageActionsOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNGetMessageActionsResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public GetMessageActionsRequestBuilder Channel(string channel){
            GetMessageActionsChannel = channel;
            return this;
        }
        public GetMessageActionsRequestBuilder Limit(int limit){
            GetMessageActionsLimit = limit;
            return this;
        }

        public GetMessageActionsRequestBuilder Start(long start){
            GetMessageActionsStart = start;
            return this;
        }
        public GetMessageActionsRequestBuilder End(long end){
            GetMessageActionsEnd = end;
            return this;
        }
        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;

            Uri request = BuildRequests.BuildGetMessageActionsRequest(
                    GetMessageActionsChannel,
                    GetMessageActionsStart,
                    GetMessageActionsEnd,
                    GetMessageActionsLimit,
                    this.PubNubInstance,
                    this.QueryParams
                );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNGetMessageActionsResult pnGetMessageActionsResult = new PNGetMessageActionsResult();
            pnGetMessageActionsResult.Data = new List<PNMessageActionsResult>();
            pnGetMessageActionsResult.More = new PNGetMessageActionsMore();
            PNStatus pnStatus = new PNStatus();

            try{
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
                
                if(dictionary != null) {
                    object objData;
                    dictionary.TryGetValue("data", out objData);
                    if(objData!=null){
                        object[] objArr = objData as object[];
                        foreach (object data in objArr){
                            Dictionary<string, object> objDataDict = data as Dictionary<string, object>;
                            if(objDataDict!=null){
                                PNMessageActionsResult pnMessageActionsResult = MessageActionsHelpers.ExtractMessageAction(objDataDict);
                                pnGetMessageActionsResult.Data.Add(pnMessageActionsResult);
                            }  else {
                                pnStatus = base.CreateErrorResponseFromException(new PubNubException("objDataDict null"), requestState, PNStatusCategory.PNUnknownCategory);
                            }  
                        }
                    }  else {
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("objData null"), requestState, PNStatusCategory.PNUnknownCategory);
                    } 
                    object objMore;
                    dictionary.TryGetValue("more", out objMore);
                    if(objMore!=null){
                        Dictionary<string, object> objMoreDict = objMore as Dictionary<string, object>;
                        string log;
                        long start;
                        Utility.TryCheckKeyAndParseLong(objMoreDict, "start", "start", out log, out start);
                        pnGetMessageActionsResult.More.Start =  start;

                        int limit;
                        Utility.TryCheckKeyAndParseInt(objMoreDict, "limit", "limit", out log, out limit);
                        pnGetMessageActionsResult.More.Limit =  limit;

                        long end;
                        Utility.TryCheckKeyAndParseLong(objMoreDict, "end", "end", out log, out end);
                        pnGetMessageActionsResult.More.End =  start;
                        
                        pnGetMessageActionsResult.More.URL =  Utility.ReadMessageFromResponseDictionary(objMoreDict, "url");
                    }
                }
            } catch (Exception ex){
                pnGetMessageActionsResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnGetMessageActionsResult, pnStatus);

        }

    }
}