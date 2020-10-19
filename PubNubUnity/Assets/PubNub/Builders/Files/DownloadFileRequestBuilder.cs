using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace PubNubAPI
{
    public class DownloadFileRequestBuilder
    {        
        private string DownloadFileID { get; set;}
        private string DownloadFileChannel { get; set;}
        private string DownloadFileName { get; set;}
        private string DownloadFileCipherKey { get; set;}    
        internal PubNubUnity PubNubInstance;
        SendFileToS3RequestBuilder s3;
        private string DownloadFileSavePath { get; set;}  

        public DownloadFileRequestBuilder(PubNubUnity pn){
            PubNubInstance = pn;
            s3 = pn.GameObjectRef.AddComponent<SendFileToS3RequestBuilder> ();
            s3.PubNubInstance = pn;
        }

        public void AbortRequest(){
            s3.AbortDownloadRequest();
        }


        #region IPubNubBuilder implementation
        public void Async(Action<PNDownloadFileResult, PNStatus> callback)
        {
            PNDownloadFileResult pnDownloadFileResult = new PNDownloadFileResult();
            string cipherKeyToUse = "";
            string dlFilePath = DownloadFileSavePath;
            string tempFilePath = string.Format("{0}/{1}.dl.enc", Application.temporaryCachePath, DownloadFileName);
            if(!string.IsNullOrEmpty(DownloadFileCipherKey)){
                cipherKeyToUse = DownloadFileCipherKey;
                dlFilePath = tempFilePath;
            } else if(!string.IsNullOrEmpty(PubNubInstance.PNConfig.CipherKey)){
                cipherKeyToUse = PubNubInstance.PNConfig.CipherKey;
                dlFilePath = tempFilePath;
            } 
            s3.Channel(DownloadFileChannel).ID(DownloadFileID).Name(DownloadFileName).SavePath(dlFilePath);
            s3.ExecuteDownloadFile((status) => {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog(string.Format("request.GetResponseHeader {0}", status.StatusCode), PNLoggingMethod.LevelInfo);
                #endif
                if(status.StatusCode != 200){
                    pnDownloadFileResult = null;
                    callback(pnDownloadFileResult, status);
                } else{
                    // Decrypt
                    // save file to temp, decrypt, save file to desired location
                    if(!string.IsNullOrEmpty(cipherKeyToUse)){                        
                        PubnubCrypto pubnubCrypto = new PubnubCrypto (cipherKeyToUse, this.PubNubInstance.PNLog);
                        pubnubCrypto.DecryptFile(dlFilePath, DownloadFileSavePath);
                    }
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog(string.Format("File successfully downloaded and saved to  {0}", DownloadFileSavePath), PNLoggingMethod.LevelInfo);
                    #endif
                    callback(pnDownloadFileResult, status);
                }                
            });
        }
        #endregion

       
        public DownloadFileRequestBuilder Channel(string channel)
        {
            DownloadFileChannel = channel;
            return this;
        }
        public DownloadFileRequestBuilder CipherKey(string cipherKey)
        {
            DownloadFileCipherKey = cipherKey;
            return this;
        }
        public DownloadFileRequestBuilder ID(string id){
            DownloadFileID = id;
            return this;
        }
        public DownloadFileRequestBuilder Name(string name){
            DownloadFileName = name;
            return this;
        }
        public DownloadFileRequestBuilder SavePath(string path){
            DownloadFileSavePath = path;
            return this;
        }

    }
}