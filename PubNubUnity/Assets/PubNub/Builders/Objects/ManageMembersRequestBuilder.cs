using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class PNMembersInput{
        public string ID {get; set;}
        public Dictionary<string, object> Custom {get; set;}
    }
    public class PNMembersRemove{
        public string ID {get; set;}
    }

    class PNMembersInputForJSON{
        public string id;
        public Dictionary<string, object> custom;
    }
    class PNMembersRemoveForJSON{
        public string id;
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
        private List<string> SortBy { get; set; } 
        
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
        public ManageMembersRequestBuilder Sort(List<string> sortBy){
            SortBy = sortBy;
            return this;
        }
        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;
            requestState.httpMethod = HTTPMethod.Patch;

            var cub = new { 
                add = ObjectsHelpers.ConvertPNMembersInputForJSON(ManageMembersAdd),
                update = ObjectsHelpers.ConvertPNMembersInputForJSON(ManageMembersUpdate),
                remove = ObjectsHelpers.ConvertPNMembersRemoveForJSON(ManageMembersRemove),
            };

            string jsonUserBody = Helpers.JsonEncodePublishMsg (cub, "", this.PubNubInstance.JsonLibrary, this.PubNubInstance.PNLog);
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("jsonUserBody: {0}", jsonUserBody), PNLoggingMethod.LevelInfo);
            #endif
            requestState.POSTData = jsonUserBody;

            string[] includeString = (ManageMembersInclude==null) ? new string[]{} : ManageMembersInclude.Select(a=>a.GetDescription().ToString()).ToArray();
            List<string> sortFields = SortBy ?? new List<string>();

            Uri request = BuildRequests.BuildObjectsManageMembersRequest(
                    ManageMembersSpaceID,
                    ManageMembersLimit,
                    ManageMembersStart,
                    ManageMembersEnd,
                    ManageMembersCount,
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams,
                    string.Join(",", sortFields)
                );
            request = this.PubNubInstance.TokenMgr.AppendTokenToURL( request.OriginalString, ManageMembersSpaceID, PNResourceType.PNSpaces, OperationType);    
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
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
                                pnStatus = base.CreateErrorResponseFromException(new PubNubException("objDataDict null"), requestState, PNStatusCategory.PNUnknownCategory);
                            }  
                        }
                    }  else {
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("objData null"), requestState, PNStatusCategory.PNUnknownCategory);
                    }
                    int totalCount;
                    string next;
                    string prev;
                    ObjectsHelpers.ExtractPagingParamsAndTotalCount(dictionary, "totalCount", "next", "prev", out totalCount, out next, out prev);
                    pnManageMembersResult.Next = next;
                    pnManageMembersResult.Prev = prev;
                    pnManageMembersResult.TotalCount = totalCount;    
                }
            } catch (Exception ex){
                pnManageMembersResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnManageMembersResult, pnStatus);

        }

    }
}
