using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Collections;
using System.Text;

namespace PubNubAPI
{
    public class SendFileToS3RequestBuilder: MonoBehaviour
    {      
  
        private PNFileUploadRequest SendFileToS3FileUploadRequestData { get; set;}
        private string SendFileToS3FilePath { get; set;}
        private string SendFileToS3FileName { get; set;}
        private string SendFileToS3ContentType { get; set;}

        private string DownloadFileChannel{ get; set;}
        private string DownloadFileID { get; set;}
        private string DownloadFileName { get; set;}
        private string DownloadFileSavePath { get; set;}
        private string QueryParams { get; set;}
        internal PubNubUnity PubNubInstance;
        private UnityWebRequest uploadWWW;
        private UnityWebRequest downloadWWW; 
        
        private Action<PNSendFileToS3Result, PNStatus> UploadCallback;
        private Action<PNStatus> DownloadCallback;

        public SendFileToS3RequestBuilder(PubNubUnity pn){ 
            PubNubInstance = pn;
        }

        public void AbortUploadRequest(){
            if((uploadWWW != null) && (!uploadWWW.isDone)){
                uploadWWW.Abort();
                uploadWWW.Dispose();
            }
        }

        public void AbortDownloadRequest(){
            if((downloadWWW != null) && (!downloadWWW.isDone)){
                downloadWWW.Abort();
                downloadWWW.Dispose();
            }
        }

        void Start(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("SendFileToS3RequestBuilder set."), PNLoggingMethod.LevelInfo);
            #endif
        }

        void Update() {
        }

        #region IPubNubBuilder implementation
        public void Execute(Action<PNSendFileToS3Result, PNStatus> callback)
        {
            this.UploadCallback = callback;
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("Calling UploadFile"), PNLoggingMethod.LevelInfo);
            #endif

            StartCoroutine(UploadFile());
        }

        public void ExecuteDownloadFile(Action<PNStatus> callback)
        {
            this.DownloadCallback = callback;
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("Calling DownloadFile"), PNLoggingMethod.LevelInfo);
            #endif

            StartCoroutine(DownloadFile());
        }        
        #endregion

         IEnumerator DownloadFile(){
            Uri request = BuildRequests.BuildDownloadFileRequest(
                    DownloadFileChannel,
                    DownloadFileID.ToString(),
                    DownloadFileName.ToString(),
                    this.PubNubInstance,
                    null
                );            
            downloadWWW = new UnityWebRequest(request.OriginalString);
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("Download URL: {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            #endif

            downloadWWW.method = UnityWebRequest.kHttpVerbGET;
            var dh = new DownloadHandlerFile(DownloadFileSavePath);
            dh.removeFileOnAbort = true;
            
            downloadWWW.downloadHandler = dh;

            yield return downloadWWW.SendWebRequest();
        
            DownloadCallback(CreatePNStatus(downloadWWW));

        }

        private PNStatus CreatePNStatus(UnityWebRequest www){
            PNStatus pnStatus = new PNStatus();
            pnStatus.StatusCode = www.responseCode;
            if(www.isNetworkError || www.isHttpError){
                pnStatus.Error = true;
                pnStatus.ErrorData = new PNErrorData(){
                    Info = www.downloadHandler.text
                }; 
            }
            return pnStatus;
        }


        public IEnumerator UploadFile(){
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

            // MultipartFormDataSection not accepting empty values, create a placeholder value and replace later
            string xAmzSecurityTokenFillerKey = "xAmzSecurityToken.pnFormField.Key";

            // Encrypted file is garbled
            string xAmzSecurityTokenFillerFile = "xFile.pnFormField.Value";

            foreach(PNFormField pnFormField in SendFileToS3FileUploadRequestData.FormFields){
                if(string.IsNullOrEmpty(pnFormField.Value)){
                    formData.Add(new MultipartFormDataSection(pnFormField.Key, xAmzSecurityTokenFillerKey));
                } else {
                    formData.Add(new MultipartFormDataSection(pnFormField.Key, pnFormField.Value));
                }
                
                if(pnFormField.Key.Equals("Content-Type")) {
                    SendFileToS3ContentType = pnFormField.Value;
                }
            }
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("SendFileToS3FileUploadRequestData.URL: {0}", SendFileToS3FileUploadRequestData.URL), PNLoggingMethod.LevelInfo);
            #endif

            byte[] boundary = UnityWebRequest.GenerateBoundary();
            string contentType = String.Concat("multipart/form-data; boundary=", Encoding.UTF8.GetString(boundary));

            byte[] formSections = UnityWebRequest.SerializeFormSections(formData, boundary);
            string formSectionString = Encoding.UTF8.GetString(formSections);
            formSectionString = formSectionString.Replace(xAmzSecurityTokenFillerKey, "");
            formSectionString = formSectionString.Replace(string.Format("--{0}--", Encoding.UTF8.GetString(boundary)), string.Format("--{0}", Encoding.UTF8.GetString(boundary)));
            formSections = Encoding.ASCII.GetBytes(formSectionString);

            // Do not use SerializeFormSections on the payload. 
            string formSectionFileHeader = string.Format("Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"\r\nContent-Type: {1}\r\n\r\n", SendFileToS3FileName, SendFileToS3ContentType);
            byte[] formSectionFileHeaderBytes = Encoding.ASCII.GetBytes(formSectionFileHeader);
            byte[] formSectionFileValue = System.IO.File.ReadAllBytes(SendFileToS3FilePath);
                          
            byte[] terminationString = Encoding.UTF8.GetBytes(String.Concat("\r\n--", Encoding.UTF8.GetString(boundary), "--"));
            byte[] boundaryWithHyphensAndNL = Encoding.UTF8.GetBytes(String.Format("\r\n--{0}--\r\n", Encoding.UTF8.GetString(boundary)));
            byte[] body = new byte[formSections.Length + formSectionFileHeaderBytes.Length + formSectionFileValue.Length + boundaryWithHyphensAndNL.Length + terminationString.Length];
            
            Buffer.BlockCopy(formSections, 0, body, 0, formSections.Length);            
            Buffer.BlockCopy(formSectionFileHeaderBytes, 0, body, formSections.Length, formSectionFileHeaderBytes.Length);
            Buffer.BlockCopy(formSectionFileValue, 0, body, formSections.Length + formSectionFileHeaderBytes.Length, formSectionFileValue.Length);
            Buffer.BlockCopy(boundaryWithHyphensAndNL, 0, body, formSections.Length + formSectionFileHeaderBytes.Length + formSectionFileValue.Length, boundaryWithHyphensAndNL.Length);
            Buffer.BlockCopy(terminationString, 0, body, formSections.Length + formSectionFileHeaderBytes.Length + formSectionFileValue.Length + boundaryWithHyphensAndNL.Length, terminationString.Length);

            uploadWWW = new UnityWebRequest(SendFileToS3FileUploadRequestData.URL);
            UploadHandler uploader = new UploadHandlerRaw(body);
            uploadWWW.downloadHandler = new DownloadHandlerBuffer();
            uploader.contentType = contentType;
            uploadWWW.uploadHandler = uploader;            
            uploadWWW.method = "POST";
                
            yield return uploadWWW.SendWebRequest();
            UploadCallback(new PNSendFileToS3Result{
                IsHttpError = uploadWWW.isHttpError,
                IsNetworkError = uploadWWW.isNetworkError,
                Text = uploadWWW.downloadHandler.text
            }, CreatePNStatus(uploadWWW));

        }

        public SendFileToS3RequestBuilder File(string filePath)
        {
            SendFileToS3FilePath = filePath;
            return this;
        }
        public SendFileToS3RequestBuilder FileUploadRequestData(PNFileUploadRequest data){
            SendFileToS3FileUploadRequestData = data;
            return this;
        }
        public SendFileToS3RequestBuilder FileName(string name){
            SendFileToS3FileName = name;
            return this;
        }

        public SendFileToS3RequestBuilder Channel(string channel)
        {
            DownloadFileChannel = channel;
            return this;
        }
        public SendFileToS3RequestBuilder ID(string id){
            DownloadFileID = id;
            return this;
        }
        public SendFileToS3RequestBuilder Name(string name){
            DownloadFileName = name;
            return this;
        }
        public SendFileToS3RequestBuilder SavePath(string path){
            DownloadFileSavePath = path;
            return this;
        }
        
    }
}