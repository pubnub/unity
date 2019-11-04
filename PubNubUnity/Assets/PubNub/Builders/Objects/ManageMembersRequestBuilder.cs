using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class PNMembersInput{
        string ID;
        Dictionary<string, object> Custom;
    }
    public class PNMembersRemove{
        string ID;
    }
    public class ManageMembersRequestBuilder: PubNubNonSubBuilder<ManageMembersRequestBuilder, PNMembersResult>, IPubNubNonSubscribeBuilder<ManageMembersRequestBuilder, PNMembersResult>
    {        
        private string ManageMembersSpaceID { get; set;}
        private int ManageMembersLimit { get; set;}
        private string ManageMembersEnd { get; set;}
        private string ManageMembersStart { get; set;}
        private bool ManageMembersCount { get; set;}
        private PNMembersInclude[] ManageMembersInclude { get; set;}
        private List<PNMembersInput> ManageMembersAdd { get; set;}
        private List<PNMembersInput> ManageMembersUpdate { get; set;}
        private List<PNMembersRemove> ManageMembersRemove { get; set;}
 
        
        public ManageMembersRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNManageMembersOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNMembersResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public ManageMembersRequestBuilder SpaceID(string id){
            ManageMembersSpaceID = id;
            return this;
        }

        public ManageMembersRequestBuilder Include(PNMembersInclude[] include){
            ManageMembersInclude = include;
            return this;
        }
        public ManageMembersRequestBuilder Limit(int limit){
            ManageMembersLimit = limit;
            return this;
        }

        public ManageMembersRequestBuilder Start(string start){
            ManageMembersStart = start;
            return this;
        }
        public ManageMembersRequestBuilder End(string end){
            ManageMembersEnd = end;
            return this;
        }
        public ManageMembersRequestBuilder Count(bool count){
            ManageMembersCount = count;
            return this;
        }
        public ManageMembersRequestBuilder Add(List<PNMembersInput> add){
            ManageMembersAdd = add;
            return this;
        }
        public ManageMembersRequestBuilder Update(List<PNMembersInput> update){
            ManageMembersUpdate = update;
            return this;
        }
        public ManageMembersRequestBuilder Remove(List<PNMembersRemove> remove){
            ManageMembersRemove = remove;
            return this;
        }

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;
            requestState.httpMethod = HTTPMethod.Post;

            var cub = new { 
                add = ManageMembersAdd.ToArray(), 
                update = ManageMembersUpdate.ToArray(),
                remove = ManageMembersRemove.ToArray(),
            };

            string jsonUserBody = Helpers.JsonEncodePublishMsg (cub, "", this.PubNubInstance.JsonLibrary, this.PubNubInstance.PNLog);
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("jsonUserBody: {0}", jsonUserBody), PNLoggingMethod.LevelInfo);
            #endif
            requestState.POSTData = jsonUserBody;

            string[] includeString = (ManageMembersInclude==null) ? new string[]{} : ManageMembersInclude.Select(a=>a.GetDescription().ToString()).ToArray();    

            Uri request = BuildRequests.BuildObjectsManageMembersRequest(
                    ManageMembersSpaceID,
                    ManageMembersLimit,
                    ManageMembersStart,
                    ManageMembersEnd,
                    ManageMembersCount,
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams
                );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            object[] c = deSerializedResult as object[];
            
            PNMembersResult pnManageMembersResult = new PNMembersResult();
            pnManageMembersResult.Data = new List<PNMembers>();
            PNStatus pnStatus = new PNStatus();

            try{
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
                
                if(dictionary != null) {
                    object objData;
                    dictionary.TryGetValue("data", out objData);
                    if(objData!=null){
                        object[] objArr = objData as object[];
                        foreach (object data in objArr){
                            Dictionary<string, object> objDataDict = data as Dictionary<string, object>;
                            if(objDataDict!=null){
                                PNMembers pnMembers = ObjectsHelpers.ExtractMembers(objDataDict);
                                pnManageMembersResult.Data.Add(pnMembers);
                            }  else {
                                pnManageMembersResult = null;
                                pnStatus = base.CreateErrorResponseFromException(new PubNubException("objDataDict null"), requestState, PNStatusCategory.PNUnknownCategory);
                            }  
                        }
                    }  else {
                        pnManageMembersResult = null;
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("objData null"), requestState, PNStatusCategory.PNUnknownCategory);
                    }  
                }
            } catch (Exception ex){
                pnManageMembersResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnManageMembersResult, pnStatus);

        }

    }
}
