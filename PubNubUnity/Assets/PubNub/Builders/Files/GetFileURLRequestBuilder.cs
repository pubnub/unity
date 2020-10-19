using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetFileURLRequestBuilder: PubNubNonSubBuilder<GetFileURLRequestBuilder, PNGetFileURLResult>, IPubNubNonSubscribeBuilder<GetFileURLRequestBuilder, PNGetFileURLResult>
    {        
        private string GetFileURLID { get; set;}
        private string GetFileURLChannel { get; set;}
        private string GetFileURLName { get; set;}

        public GetFileURLRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNGetFileURLOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNGetFileURLResult, PNStatus> callback)
        {
            this.Callback = callback;
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;
            requestState.httpMethod = HTTPMethod.Get;

            Uri request = BuildRequests.BuildDownloadFileRequest(
                    GetFileURLChannel,
                    GetFileURLID.ToString(),
                    GetFileURLName.ToString(),
                    this.PubNubInstance,
                    this.QueryParams
                );
            PNGetFileURLResult pnGetFileURLResult = new PNGetFileURLResult();
            PNStatus pnStatus = new PNStatus();
            pnGetFileURLResult.URL = request.ToString();
            Callback(pnGetFileURLResult, pnStatus);
        }
        #endregion

        public GetFileURLRequestBuilder Channel(string channel)
        {
            GetFileURLChannel = channel;
            return this;
        }
        public GetFileURLRequestBuilder ID(string id){
            GetFileURLID = id;
            return this;
        }
        public GetFileURLRequestBuilder Name(string name){
            GetFileURLName = name;
            return this;
        }
        protected override void RunWebRequest(QueueManager qm){
            //Intentionally not implemented.
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //Intentionally not implemented.
        }

    }
}