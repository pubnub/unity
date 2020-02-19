using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class UpdateUserRequestBuilder: PubNubNonSubBuilder<UpdateUserRequestBuilder, PNUserResult>, IPubNubNonSubscribeBuilder<UpdateUserRequestBuilder, PNUserResult>
    {        
        private PNUserSpaceInclude[] UpdateUserInclude { get; set;}
        private string UpdateUserID { get; set;}
        private string UpdateUserName { get; set;}
        private string UpdateUserExternalID { get; set;}
        private string UpdateUserProfileURL { get; set;}
        private string UpdateUserEmail { get; set;}
        private Dictionary<string, object> UpdateUserCustom { get; set;}
        
        public UpdateUserRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNUpdateUserOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNUserResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public UpdateUserRequestBuilder Include(PNUserSpaceInclude[] include){
            UpdateUserInclude = include;
            return this;
        }

        public UpdateUserRequestBuilder ID(string id){
            UpdateUserID = id;
            return this;
        }

        public UpdateUserRequestBuilder Name(string name){
            UpdateUserName = name;
            return this;
        }

        public UpdateUserRequestBuilder ExternalID(string externalID){
            UpdateUserExternalID = externalID;
            return this;
        }

        public UpdateUserRequestBuilder ProfileURL(string profileURL){
            UpdateUserProfileURL = profileURL;
            return this;
        }

        public UpdateUserRequestBuilder Email(string email){
            UpdateUserEmail = email;
            return this;
        }

        public UpdateUserRequestBuilder Custom(Dictionary<string, object> custom){
            UpdateUserCustom = custom;
            return this;
        }   

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;
            requestState.httpMethod = HTTPMethod.Patch;

            var cub = new { 
                id = UpdateUserID, 
                email = UpdateUserEmail,
                name = UpdateUserName,
                profileUrl = UpdateUserProfileURL,
                externalId = UpdateUserExternalID,
                custom = UpdateUserCustom,
            };

            string jsonUserBody = Helpers.JsonEncodePublishMsg (cub, "", this.PubNubInstance.JsonLibrary, this.PubNubInstance.PNLog);
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("jsonUserBody: {0}", jsonUserBody), PNLoggingMethod.LevelInfo);
            #endif
            requestState.POSTData = jsonUserBody;

            string[] includeString = (UpdateUserInclude==null) ? new string[]{} : UpdateUserInclude.Select(a=>a.GetDescription().ToString()).ToArray();          

            Uri request = BuildRequests.BuildObjectsUpdateUserRequest(
                    UpdateUserID,
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams
                );
            request = this.PubNubInstance.TokenMgr.AppendTokenToURL( request.OriginalString, UpdateUserID, PNResourceType.PNUsers, OperationType);    
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNUserResult pnUserResult = new PNUserResult();
            PNStatus pnStatus = new PNStatus();

            try{
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
                
                if(dictionary != null) {
                    object objData;
                    dictionary.TryGetValue("data", out objData);
                    if(objData!=null){
                        Dictionary<string, object> objDataDict = objData as Dictionary<string, object>;
                        if(objDataDict != null){
                            pnUserResult = ObjectsHelpers.ExtractUser(objDataDict);
                        }  else {
                            pnUserResult = null;
                            pnStatus = base.CreateErrorResponseFromException(new PubNubException("objDataDict null"), requestState, PNStatusCategory.PNUnknownCategory);
                        }  
                    }  else {
                        pnUserResult = null;
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("objData null"), requestState, PNStatusCategory.PNUnknownCategory);
                    }                      
                }
            } catch (Exception ex){
                pnUserResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnUserResult, pnStatus);

        }

    }
}
