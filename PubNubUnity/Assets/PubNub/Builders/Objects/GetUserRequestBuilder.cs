using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetUserRequestBuilder: PubNubNonSubBuilder<GetUserRequestBuilder, PNUserResult>, IPubNubNonSubscribeBuilder<GetUserRequestBuilder, PNUserResult>
    {        
        private string GetUserID { get; set;}
        private PNUserSpaceInclude[] GetUserInclude { get; set;}
        
        public GetUserRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNGetUserOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNUserResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public GetUserRequestBuilder Include(PNUserSpaceInclude[] include){
            GetUserInclude = include;
            return this;
        }

        public GetUserRequestBuilder ID(string id){
            GetUserID = id;
            return this;
        }

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;

            string[] includeString = (GetUserInclude==null) ? new string[]{} : GetUserInclude.Select(a=>a.GetDescription().ToString()).ToArray();

            Uri request = BuildRequests.BuildObjectsGetUserRequest(
                    GetUserID,
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams
                );
            request = this.PubNubInstance.TokenMgr.AppendTokenToURL( request.OriginalString, GetUserID, PNResourceType.PNUsers, OperationType);    
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNUserResult pnUserResult = new PNUserResult();
            PNStatus pnStatus = new PNStatus();

            try{
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
                
                if(dictionary != null) {
                    object objData;
                    if(dictionary.TryGetValue("data", out objData)){
                        Dictionary<string, object> objDataDict = objData as Dictionary<string, object>;
                        if(objDataDict!=null){
                            pnUserResult = ObjectsHelpers.ExtractUser(objDataDict);
                        } else {
                            pnStatus = base.CreateErrorResponseFromException(new PubNubException("objDataDict not present"), requestState, PNStatusCategory.PNUnknownCategory);
                        }
                    } else {
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("Data not present"), requestState, PNStatusCategory.PNUnknownCategory);
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