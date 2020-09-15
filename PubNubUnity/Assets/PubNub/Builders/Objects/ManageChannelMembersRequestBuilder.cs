using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class PNChannelMembersUUID{
        public string ID {get; set;}
    }
    public class PNChannelMembersSet{
        public PNChannelMembersUUID UUID {get; set;}
        public Dictionary<string, object> Custom {get; set;}
    }
    public class PNChannelMembersRemove{
        public PNChannelMembersUUID UUID {get; set;}
    }
    class PNChannelMembersUUIDForJSON{
        public string id {get; set;}
    }
    class PNMembersInputForJSON{
        public PNChannelMembersUUIDForJSON uuid;
        public Dictionary<string, object> custom;
    }
    class PNMembersRemoveForJSON{
        public PNChannelMembersUUIDForJSON uuid;
    }
    public class ManageChannelMembersRequestBuilder: PubNubNonSubBuilder<ManageChannelMembersRequestBuilder, PNManageChannelMembersResult>, IPubNubNonSubscribeBuilder<ManageChannelMembersRequestBuilder, PNManageChannelMembersResult>
    {        
        private string ManageMembersChannel { get; set;}
        private int ManageMembersLimit { get; set;}
        private string ManageMembersEnd { get; set;}
        private string ManageMembersStart { get; set;}
        private bool ManageMembersCount { get; set;}
        private PNChannelMembersInclude[] ManageMembersInclude { get; set;}
        private List<PNChannelMembersSet> ManageMembersSet { get; set;}
        private List<PNChannelMembersRemove> ManageMembersRemove { get; set;}
        private List<string> SortBy { get; set; } 
        
        public ManageChannelMembersRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNManageChannelMembersOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNManageChannelMembersResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public ManageChannelMembersRequestBuilder Channel(string id){
            ManageMembersChannel = id;
            return this;
        }

        public ManageChannelMembersRequestBuilder Include(PNChannelMembersInclude[] include){
            ManageMembersInclude = include;
            return this;
        }
        public ManageChannelMembersRequestBuilder Limit(int limit){
            ManageMembersLimit = limit;
            return this;
        }

        public ManageChannelMembersRequestBuilder Start(string start){
            ManageMembersStart = start;
            return this;
        }
        public ManageChannelMembersRequestBuilder End(string end){
            ManageMembersEnd = end;
            return this;
        }
        public ManageChannelMembersRequestBuilder Count(bool count){
            ManageMembersCount = count;
            return this;
        }
        public ManageChannelMembersRequestBuilder Set(List<PNChannelMembersSet> set){
            ManageMembersSet = set;
            return this;
        }
        public ManageChannelMembersRequestBuilder Remove(List<PNChannelMembersRemove> remove){
            ManageMembersRemove = remove;
            return this;
        }
        public ManageChannelMembersRequestBuilder Sort(List<string> sortBy){
            SortBy = sortBy;
            return this;
        }
        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;
            requestState.httpMethod = HTTPMethod.Patch;

            var cub = new { 
                set = ObjectsHelpers.ConvertPNMembersInputForJSON(ManageMembersSet),
                delete = ObjectsHelpers.ConvertPNMembersRemoveForJSON(ManageMembersRemove),
            };

            string jsonUserBody = Helpers.JsonEncodePublishMsg (cub, "", this.PubNubInstance.JsonLibrary, this.PubNubInstance.PNLog);
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("jsonUserBody: {0}", jsonUserBody), PNLoggingMethod.LevelInfo);
            #endif
            requestState.POSTData = jsonUserBody;

            string[] includeString = (ManageMembersInclude==null) ? new string[]{} : ManageMembersInclude.Select(a=>a.GetDescription().ToString()).ToArray();
            List<string> sortFields = SortBy ?? new List<string>();

            Uri request = BuildRequests.BuildObjectsManageChannelMembersRequest(
                    ManageMembersChannel,
                    ManageMembersLimit,
                    ManageMembersStart,
                    ManageMembersEnd,
                    ManageMembersCount,
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams,
                    string.Join(",", sortFields)
                );
            request = this.PubNubInstance.TokenMgr.AppendTokenToURL( request.OriginalString, ManageMembersChannel, PNResourceType.PNChannelMetadata, OperationType);    
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNManageChannelMembersResult pnManageMembersResult = new PNManageChannelMembersResult();
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
