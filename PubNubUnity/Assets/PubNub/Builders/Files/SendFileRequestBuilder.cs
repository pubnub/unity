
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PubNubAPI
{
    public class SendFileRequestBuilder: PubNubNonSubBuilder<SendFileRequestBuilder, PNSendFileResult>, IPubNubNonSubscribeBuilder<SendFileRequestBuilder, PNSendFileResult>
    {        
        private string SendFileMessage { get; set;}
        private string SendFilePath { get; set;}
        private string SendFileName { get; set;}
        private string SendFileCipherKey { get; set;}
        private bool ShouldStoreInHistory  { get; set;}
        private Dictionary<string, string> Metadata { get; set;}
        private int PublishTTL { get; set;}

        private string SendFileChannel { get; set;}
        SendFileToS3RequestBuilder s3;

        public SendFileRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNSendFileOperation){
            s3 = pn.GameObjectRef.AddComponent<SendFileToS3RequestBuilder> ();
            s3.PubNubInstance = pn;
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNSendFileResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public SendFileRequestBuilder Channel(string channel)
        {
            SendFileChannel = channel;
            return this;
        }
        public SendFileRequestBuilder Message(string message){
            SendFileMessage = message;
            return this;
        }
        public SendFileRequestBuilder Name(string name){
            SendFileName = name;
            return this;
        }
        public SendFileRequestBuilder CipherKey(string cipherKey)
        {
            SendFileCipherKey = cipherKey;
            return this;
        }
        public SendFileRequestBuilder ShouldStore(bool shouldStoreInHistory){
            ShouldStoreInHistory = shouldStoreInHistory;
            return this;
        }
        public SendFileRequestBuilder Meta(Dictionary<string, string> metadata){
            Metadata = metadata;
            return this;
        }
        public SendFileRequestBuilder TTL(int publishTTL){
            PublishTTL = publishTTL;
            return this;
        }
        public SendFileRequestBuilder FilePath(string path){
            SendFilePath = path;
            return this;
        }

        public void AbortRequest(){
            s3.AbortUploadRequest();
        }

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;
            requestState.httpMethod = HTTPMethod.Post;
            var cub = new { 
                name = SendFileName, 
            };
            string jsonUserBody = Helpers.JsonEncodePublishMsg (cub, "", this.PubNubInstance.JsonLibrary, this.PubNubInstance.PNLog);
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("jsonUserBody: {0}", jsonUserBody), PNLoggingMethod.LevelInfo);
            #endif
            requestState.POSTData = jsonUserBody;

            Uri request = BuildRequests.BuildSendFileRequest(
                SendFileChannel,
                this.PubNubInstance,
                this.QueryParams
            );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNSendFileResult pnSendFileResult = new PNSendFileResult();
            PNStatus pnStatus = new PNStatus();

            try{
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
                PNSendFileInputsToS3 pnSendFileInputsToS3 = new PNSendFileInputsToS3();

                if(dictionary != null) {
                    pnSendFileInputsToS3.Data = ExtractPNFileData(dictionary);
                    pnSendFileInputsToS3.FileUploadRequest = ExtractFileUploadRequest(dictionary);
                    
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog(string.Format("pnSendFileToS3Result.Data: {0}", pnSendFileInputsToS3.Data.ID), PNLoggingMethod.LevelInfo);
                    this.PubNubInstance.PNLog.WriteToLog(string.Format("pnSendFileToS3Result.FileUploadRequest.URL: {0}", pnSendFileInputsToS3.FileUploadRequest.URL), PNLoggingMethod.LevelInfo);
                    this.PubNubInstance.PNLog.WriteToLog(string.Format("pnSendFileToS3Result.FileUploadRequest.Method: {0}", pnSendFileInputsToS3.FileUploadRequest.Method), PNLoggingMethod.LevelInfo);
                    foreach(PNFormField f in pnSendFileInputsToS3.FileUploadRequest.FormFields){
                        this.PubNubInstance.PNLog.WriteToLog(string.Format("pnSendFileToS3Result.FileUploadRequest.FormFields Key: {0}", f.Key), PNLoggingMethod.LevelInfo);
                        this.PubNubInstance.PNLog.WriteToLog(string.Format("pnSendFileToS3Result.FileUploadRequest.FormFields Value: {0}", f.Value), PNLoggingMethod.LevelInfo);
                    }
                    #endif
                    
                    // read file, encrypt, save file to temp, send enc file path
                    string upFilePath = SendFilePath;
                    string tempFilePath = string.Format("{0}/{1}.up.enc", Application.temporaryCachePath, SendFileName);
                    bool delTempFile = false;
                    if(!string.IsNullOrEmpty(SendFileCipherKey)){
                        upFilePath = tempFilePath;
                        PubnubCrypto pubnubCrypto = new PubnubCrypto (SendFileCipherKey, this.PubNubInstance.PNLog);
                        pubnubCrypto.EncryptFile(null, SendFilePath, upFilePath);
                        delTempFile = true;
                    } else if(!string.IsNullOrEmpty(PubNubInstance.PNConfig.CipherKey)){
                        upFilePath = tempFilePath;
                        PubnubCrypto pubnubCrypto = new PubnubCrypto (PubNubInstance.PNConfig.CipherKey, this.PubNubInstance.PNLog);
                        pubnubCrypto.EncryptFile(null, SendFilePath, upFilePath);
                        delTempFile = true;
                    } 

                    s3.FileName(SendFileName).File(upFilePath).FileUploadRequestData(pnSendFileInputsToS3.FileUploadRequest);
                    s3.Execute((pnSendFileToS3Result, status) => {
                        if(status.StatusCode != 204){
                            pnSendFileResult = null;
                            requestState.ResponseCode = status.StatusCode;
                            pnStatus = base.CreateErrorResponseFromException(new PubNubException(string.Format("File Upload Failed {0}", pnSendFileToS3Result.Text)), requestState, PNStatusCategory.PNUnknownCategory);
                            Callback(pnSendFileResult, pnStatus);
                        } else{
                            pnSendFileResult.Data = new PNFileData();
                            pnSendFileResult.Data.ID = pnSendFileInputsToS3.Data.ID;
                            PublishFileMessage(pnSendFileResult, pnStatus, requestState, 0);
                        }
                        if(delTempFile){
                            File.Delete(tempFilePath);
                        }
                    });

                } else {
                    pnSendFileResult = null;
                    pnStatus = base.CreateErrorResponseFromException(new PubNubException("Data not present"), requestState, PNStatusCategory.PNUnknownCategory);
                    Callback(pnSendFileResult, pnStatus);
                }
            } catch (Exception ex){
                pnSendFileResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
                Callback(pnSendFileResult, pnStatus);
            }
        }

        public void PublishFileMessage(PNSendFileResult pnSendFileResult, PNStatus pnStatus, RequestState requestState, int tryCount){
            base.PubNubInstance.PublishFileMessage().Channel(SendFileChannel).FileID(pnSendFileResult.Data.ID).FileName(SendFileName).MessageText(SendFileMessage).TTL(PublishTTL).ShouldStore(ShouldStoreInHistory).Meta(Metadata).Async((presult, pstatus) => {
                if(pstatus.Error){
                    if (tryCount >= base.PubNubInstance.PNConfig.FileMessagePublishRetryLimit) {
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("Unable to send Publish File Message"), requestState, PNStatusCategory.PNUnknownCategory);
                        PNFileInfoForPublish pnFileInfoForPublish = new PNFileInfoForPublish();
                        pnFileInfoForPublish.ID = pnSendFileResult.Data.ID;
                        pnFileInfoForPublish.Name = SendFileName;
                        pnStatus.AdditionalData = pnFileInfoForPublish;
                        pnSendFileResult = new PNSendFileResult();
                        Callback(pnSendFileResult, pnStatus);
                    } else {
                        tryCount++;
                        PublishFileMessage(pnSendFileResult, pnStatus, requestState, tryCount);
                    }
                } else {
                    pnSendFileResult.Timetoken = presult.Timetoken;
                    Callback(pnSendFileResult, pnStatus);
                }
            });
            
        }
        private PNFileData ExtractPNFileData(Dictionary<string, object> dictionary){
            PNFileData pnFileData = new PNFileData();
            object objData;
            dictionary.TryGetValue("data", out objData);
            if(objData!=null){    
                Dictionary<string, object> objDataDict = objData as Dictionary<string, object>;    
                pnFileData.ID = Utility.ReadMessageFromResponseDictionary(objDataDict, "id");
            }
            return pnFileData;
        }

        private PNFileUploadRequest ExtractFileUploadRequest(Dictionary<string, object> dictionary){
            PNFileUploadRequest pnFileUploadRequest = new PNFileUploadRequest();
            object objData;
            dictionary.TryGetValue("file_upload_request", out objData);
            if(objData!=null){    
                Dictionary<string, object> objDataDict = objData as Dictionary<string, object>;    
                pnFileUploadRequest.URL = Utility.ReadMessageFromResponseDictionary(objDataDict, "url");
                pnFileUploadRequest.Method = Utility.ReadMessageFromResponseDictionary(objDataDict, "method");
                pnFileUploadRequest.FormFields = ExtractPNFormFields(objDataDict);
            }
            return pnFileUploadRequest;
        }

        private List<PNFormField> ExtractPNFormFields(Dictionary<string, object> dictionary){
            List<PNFormField> pnFormFields = new List<PNFormField>();
            object objData;
            dictionary.TryGetValue("form_fields", out objData);
            if(objData!=null){    
                object[] objArr = objData as object[];
                foreach (object data in objArr){
                    Dictionary<string, object> objDataDict = data as Dictionary<string, object>;
                    if(objDataDict!=null){
                        Dictionary<string, object> kvDictionary = objDataDict as Dictionary<string, object>; 
                        PNFormField pnFormField = new PNFormField();
                        pnFormField.Key = Utility.ReadMessageFromResponseDictionary(kvDictionary, "key");
                        pnFormField.Value = Utility.ReadMessageFromResponseDictionary(kvDictionary, "value");
                        pnFormFields.Add(pnFormField);

                    }
                }
            }
            return pnFormFields;
        }

    }
}