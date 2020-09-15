using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class SetUUIDMetadataRequestBuilder: PubNubNonSubBuilder<SetUUIDMetadataRequestBuilder, PNUUIDMetadataResult>, IPubNubNonSubscribeBuilder<SetUUIDMetadataRequestBuilder, PNUUIDMetadataResult>
    {        
        private PNUUIDMetadataInclude[] UUIDInclude { get; set;}
        private string UUIDToUse { get; set;}
        private string UUIDName { get; set;}
        private string UUIDExternalID { get; set;}
        private string UUIDProfileURL { get; set;}
        private string UUIDEmail { get; set;}
        private Dictionary<string, object> UUIDCustom { get; set;}
        
        public SetUUIDMetadataRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNSetUUIDMetadataOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNUUIDMetadataResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public SetUUIDMetadataRequestBuilder Include(PNUUIDMetadataInclude[] include){
            UUIDInclude = include;
            return this;
        }

        public SetUUIDMetadataRequestBuilder UUID(string id){
            UUIDToUse = id;
            return this;
        }

        public SetUUIDMetadataRequestBuilder Name(string name){
            UUIDName = name;
            return this;
        }

        public SetUUIDMetadataRequestBuilder ExternalID(string externalID){
            UUIDExternalID = externalID;
            return this;
        }

        public SetUUIDMetadataRequestBuilder ProfileURL(string profileURL){
            UUIDProfileURL = profileURL;
            return this;
        }

        public SetUUIDMetadataRequestBuilder Email(string email){
            UUIDEmail = email;
            return this;
        }

        public SetUUIDMetadataRequestBuilder Custom(Dictionary<string, object> custom){
            UUIDCustom = custom;
            return this;
        }   

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;
            requestState.httpMethod = HTTPMethod.Patch;

            var cub = new { 
                id = UUIDToUse, 
                email = UUIDEmail,
                name = UUIDName,
                profileUrl = UUIDProfileURL,
                externalId = UUIDExternalID,
                custom = UUIDCustom,
            };

            string jsonUUIDBody = Helpers.JsonEncodePublishMsg (cub, "", this.PubNubInstance.JsonLibrary, this.PubNubInstance.PNLog);
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("jsonUUIDBody: {0}", jsonUUIDBody), PNLoggingMethod.LevelInfo);
            #endif
            requestState.POSTData = jsonUUIDBody;

            string[] includeString = (UUIDInclude==null) ? new string[]{} : UUIDInclude.Select(a=>a.GetDescription().ToString()).ToArray();          

            Uri request = BuildRequests.BuildObjectsSetUUIDMetadataRequest(
                    UUIDToUse,
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams
                );
            request = this.PubNubInstance.TokenMgr.AppendTokenToURL( request.OriginalString, UUIDToUse, PNResourceType.PNUUIDMetadata, OperationType);                
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNUUIDMetadataResult pnUUIDResult = new PNUUIDMetadataResult();
            PNStatus pnStatus = new PNStatus();

            try{
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
                
                if(dictionary != null) {
                    object objData;
                    dictionary.TryGetValue("data", out objData);
                    if(objData!=null){
                        Dictionary<string, object> objDataDict = objData as Dictionary<string, object>;
                        if(objDataDict != null){
                            pnUUIDResult = ObjectsHelpers.ExtractUUIDMetadata(objDataDict);
                        }  else {
                            pnUUIDResult = null;
                            pnStatus = base.CreateErrorResponseFromException(new PubNubException("objDataDict null"), requestState, PNStatusCategory.PNUnknownCategory);
                        }  
                    }  else {
                        pnUUIDResult = null;
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("objData null"), requestState, PNStatusCategory.PNUnknownCategory);
                    }                      
                }
            } catch (Exception ex){
                pnUUIDResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnUUIDResult, pnStatus);

        }

    }
}
