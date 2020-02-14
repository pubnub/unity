using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class CreateSpaceRequestBuilder: PubNubNonSubBuilder<CreateSpaceRequestBuilder, PNSpaceResult>, IPubNubNonSubscribeBuilder<CreateSpaceRequestBuilder, PNSpaceResult>
    {        
        private PNUserSpaceInclude[] CreateSpaceInclude { get; set;}
        private string CreateSpaceID { get; set;}
        private string CreateSpaceName { get; set;}
        private string CreateSpaceDescription { get; set;}
        private Dictionary<string, object> CreateSpaceCustom { get; set;}
        
        public CreateSpaceRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNCreateSpaceOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNSpaceResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public CreateSpaceRequestBuilder Include(PNUserSpaceInclude[] include){
            CreateSpaceInclude = include;
            return this;
        }

        public CreateSpaceRequestBuilder ID(string id){
            CreateSpaceID = id;
            return this;
        }

        public CreateSpaceRequestBuilder Name(string name){
            CreateSpaceName = name;
            return this;
        }

        public CreateSpaceRequestBuilder Description(string description){
            CreateSpaceDescription = description;
            return this;
        }

        public CreateSpaceRequestBuilder Custom(Dictionary<string, object> custom){
            CreateSpaceCustom = custom;
            return this;
        }   

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;
            requestState.httpMethod = HTTPMethod.Post;

            var cub = new { 
                id = CreateSpaceID, 
                name = CreateSpaceName,
                description = CreateSpaceDescription,
                custom = CreateSpaceCustom,
            };

            string jsonUserBody = Helpers.JsonEncodePublishMsg (cub, "", this.PubNubInstance.JsonLibrary, this.PubNubInstance.PNLog);
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("jsonUserBody: {0}", jsonUserBody), PNLoggingMethod.LevelInfo);
            #endif
            requestState.POSTData = jsonUserBody;

            string[] includeString = (CreateSpaceInclude==null) ? new string[]{} : CreateSpaceInclude.Select(a=>a.GetDescription().ToString()).ToArray(); 

            Uri request = BuildRequests.BuildObjectsCreateSpaceRequest(
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams
                );
            request = this.PubNubInstance.TokenMgr.AppendTokenToURL(request.OriginalString, CreateSpaceID, PNResourceType.PNSpaces, OperationType);
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNSpaceResult pnSpaceResult = new PNSpaceResult();
            PNStatus pnStatus = new PNStatus();

            try{
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
                
                if(dictionary != null) {
                    object objData;
                    dictionary.TryGetValue("data", out objData);
                    if(objData!=null){
                        Dictionary<string, object> objDataDict = objData as Dictionary<string, object>;
                        if(objDataDict!=null){
                            pnSpaceResult = ObjectsHelpers.ExtractSpace(objDataDict);
                        }  else {
                            pnSpaceResult = null;
                            pnStatus = base.CreateErrorResponseFromException(new PubNubException("objDataDict null"), requestState, PNStatusCategory.PNUnknownCategory);
                        }  
                    }  else {
                        pnSpaceResult = null;
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("objData null"), requestState, PNStatusCategory.PNUnknownCategory);
                    }  
                }
            } catch (Exception ex){
                pnSpaceResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnSpaceResult, pnStatus);

        }

    }
}