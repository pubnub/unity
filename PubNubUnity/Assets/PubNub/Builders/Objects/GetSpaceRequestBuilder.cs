using System;
using System.Collections.Generic;
using System.Linq;

namespace PubNubAPI
{    public class GetSpaceRequestBuilder : PubNubNonSubBuilder<GetSpaceRequestBuilder, PNSpaceResult>, IPubNubNonSubscribeBuilder<GetSpaceRequestBuilder, PNSpaceResult>
    {
        private string GetSpaceID { get; set; }
        private PNUserSpaceInclude[] CreateSpaceInclude { get; set; }
        private Dictionary<string, object> GetSpaceCustom { get; set; }

        public GetSpaceRequestBuilder(PubNubUnity pn) : base(pn, PNOperationType.PNGetSpaceOperation)
        {
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNSpaceResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public GetSpaceRequestBuilder Include(PNUserSpaceInclude[] include)
        {
            CreateSpaceInclude = include;
            return this;
        }

        public GetSpaceRequestBuilder ID(string id)
        {
            GetSpaceID = id;
            return this;
        }

        protected override void RunWebRequest(QueueManager qm)
        {
            RequestState requestState = new RequestState();
            requestState.OperationType = OperationType;

            string[] includeString = Enum.GetValues(typeof(PNUserSpaceInclude))
                .Cast<int>()
                .Select(x => x.ToString())
                .ToArray();
            //TODO: Need to refactor
            if (includeString != null && includeString.Length == 1 && includeString[0] == "0")
            {
                includeString[0] = "custom";
            }

            Uri request = BuildRequests.BuildObjectsGetSpaceRequest(
                    GetSpaceID,
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams
                );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this);
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState)
        {
            object[] c = deSerializedResult as object[];
            //  {"status":200,"data":{"id":"id777","name":"name 777","externalId":"externalID 777","profileUrl":"profileURL 777","email":"email 777","created":"2019-10-29T12:46:23.464847Z","updated":"2019-10-29T12:46:23.464847Z","eTag":"Ac/04uPhkaiiuwE"}}
            PNSpaceResult pnSpaceResult = new PNSpaceResult();
            PNStatus pnStatus = new PNStatus();

            try
            {
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;

                if (dictionary != null)
                {
                    object objData;
                    if (dictionary.TryGetValue("data", out objData))
                    {
                        Dictionary<string, object> objDataDict = objData as Dictionary<string, object>;
                        if (objDataDict != null)
                        {
                            pnSpaceResult = ObjectsHelpers.ExtractSpace(objDataDict);
                            // pnUserResult.ID = Utility.ReadMessageFromResponseDictionary(objDataDict, "id");
                            // pnUserResult.Name = Utility.ReadMessageFromResponseDictionary(objDataDict, "name");
                            // pnUserResult.ExternalID = Utility.ReadMessageFromResponseDictionary(objDataDict, "externalId");
                            // pnUserResult.ProfileURL = Utility.ReadMessageFromResponseDictionary(objDataDict, "profileUrl");
                            // pnUserResult.Email = Utility.ReadMessageFromResponseDictionary(objDataDict, "email");
                            // pnUserResult.Created = Utility.ReadMessageFromResponseDictionary(objDataDict, "created");
                            // pnUserResult.Updated = Utility.ReadMessageFromResponseDictionary(objDataDict, "updated");
                            // pnUserResult.ETag = Utility.ReadMessageFromResponseDictionary(objDataDict, "eTag");
                            // pnUserResult.Custom = Utility.ReadDictionaryFromResponseDictionary(objDataDict, "custom");
                        }
                        else
                        {
                            pnSpaceResult = null;
                            pnStatus = base.CreateErrorResponseFromException(new PubNubException("objDataDict not present"), requestState, PNStatusCategory.PNUnknownCategory);
                        }
                    }
                    else
                    {
                        pnSpaceResult = null;
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("Data not present"), requestState, PNStatusCategory.PNUnknownCategory);
                    }
                }
            }
            catch (Exception ex)
            {
                pnSpaceResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnSpaceResult, pnStatus);

        }

    }

}