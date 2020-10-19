using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class DeleteFileRequestBuilder: PubNubNonSubBuilder<DeleteFileRequestBuilder, PNDeleteFileResult>, IPubNubNonSubscribeBuilder<DeleteFileRequestBuilder, PNDeleteFileResult>
    {        
        private string DeleteFileID { get; set;}
        private string DeleteFileChannel { get; set;}
        private string DeleteFileName { get; set;}

        public DeleteFileRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNDeleteFileOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNDeleteFileResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public DeleteFileRequestBuilder Channel(string channel)
        {
            DeleteFileChannel = channel;
            return this;
        }
        public DeleteFileRequestBuilder ID(string id){
            DeleteFileID = id;
            return this;
        }
        public DeleteFileRequestBuilder Name(string name){
            DeleteFileName = name;
            return this;
        }
        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;
            requestState.httpMethod = HTTPMethod.Delete;

            Uri request = BuildRequests.BuildDeleteFileRequest(
                    DeleteFileChannel,
                    DeleteFileID.ToString(),
                    DeleteFileName.ToString(),                     
                    this.PubNubInstance,
                    this.QueryParams
                );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNDeleteFileResult pnDeleteFileResult = new PNDeleteFileResult();
            PNStatus pnStatus = new PNStatus();

            try{
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
                
                if(dictionary == null) {
                    pnDeleteFileResult = null;
                    pnStatus = base.CreateErrorResponseFromException(new PubNubException("Data not present"), requestState, PNStatusCategory.PNUnknownCategory);
                }
            } catch (Exception ex){
                pnDeleteFileResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnDeleteFileResult, pnStatus);

        }

    }
}