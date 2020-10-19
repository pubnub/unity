using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class ListFilesRequestBuilder: PubNubNonSubBuilder<ListFilesRequestBuilder, PNListFilesResult>, IPubNubNonSubscribeBuilder<ListFilesRequestBuilder, PNListFilesResult>
    {        
        private int ListFilesLimit { get; set;}
        private string ListFilesChannel { get; set;}
        private string ListFilesNext { get; set;}

        public ListFilesRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNListFilesOperation){
            ListFilesLimit = 100;
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNListFilesResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public ListFilesRequestBuilder Channel(string channel)
        {
            ListFilesChannel = channel;
            return this;
        }
        public ListFilesRequestBuilder Limit(int limit){
            ListFilesLimit = limit;
            return this;
        }
        public ListFilesRequestBuilder Next(string next){
            ListFilesNext = next;
            return this;
        }
        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;
            requestState.httpMethod = HTTPMethod.Get;

            Uri request = BuildRequests.BuildListFilesRequest(
                    ListFilesChannel,
                    ListFilesLimit,
                    ListFilesNext,
                    this.PubNubInstance,
                    this.QueryParams
                );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNListFilesResult pnListFilesResult = new PNListFilesResult();
            PNStatus pnStatus = new PNStatus();

            try{
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
                
                if(dictionary != null) {
                    object objData;
                    string log; 
                    
                    dictionary.TryGetValue("data", out objData);
                    if(objData!=null){
                        object[] objArr = objData as object[];
                        pnListFilesResult.Data = new List<PNFileInfo>();
                        foreach (object data in objArr){
                            Dictionary<string, object> objDataDict = data as Dictionary<string, object>;
                            if(objDataDict!=null){
                                PNFileInfo pnFileInfo = new PNFileInfo();
                                pnFileInfo.ID = Utility.ReadMessageFromResponseDictionary(objDataDict, "id");
                                pnFileInfo.Name = Utility.ReadMessageFromResponseDictionary(objDataDict, "name");
                                int size;
                                Utility.TryCheckKeyAndParseInt(objDataDict, "size", "size", out log, out size);
                                pnFileInfo.Size = size;
                                pnFileInfo.Created = Utility.ReadMessageFromResponseDictionary(objDataDict, "created");

                                pnListFilesResult.Data.Add(pnFileInfo);
                            }  else {
                                pnStatus = base.CreateErrorResponseFromException(new PubNubException("objDataDict null"), requestState, PNStatusCategory.PNUnknownCategory);
                            }  
                        }
                    }  else {
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("objData null"), requestState, PNStatusCategory.PNUnknownCategory);
                    }  
                    pnListFilesResult.Next = Utility.ReadMessageFromResponseDictionary(dictionary, "next");
                    int limit;
                    Utility.TryCheckKeyAndParseInt(dictionary, "count", "count", out log, out limit);
                    pnListFilesResult.Count = limit;
                }
            } catch (Exception ex){
                pnListFilesResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnListFilesResult, pnStatus);

        }

    }
}