using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PubNubAPI
{
    public class PNGrantTokenResult{
        public string Token {get; set;}
    }

    public class GrantTokenBuilder: PubNubNonSubBuilder<GrantTokenBuilder, PNGrantTokenResult>, IPubNubNonSubscribeBuilder<GrantTokenBuilder, PNGrantTokenResult>{

        private Dictionary<string, int> ResUsers {get; set;}
        private Dictionary<string, int> ResSpaces {get; set;}
        private Dictionary<string, int> ResUUIDs {get; set;}
        private Dictionary<string, int> ResChannels {get; set;}
        private Dictionary<string, int> ResGroups {get; set;}
        private Dictionary<string, int> PatUsers {get; set;}
        private Dictionary<string, int> PatSpaces {get; set;}
        private Dictionary<string, int> PatUUIDs {get; set;}
        private Dictionary<string, int> PatChannels {get; set;}
        private Dictionary<string, int> PatGroups {get; set;}
        private Dictionary<string, object> Meta {get; set;}
        private int TTL {get; set;}
        private string AuthorizedUUID {get; set;}

        public GrantTokenBuilder(PubNubUnity pn): base(pn, PNOperationType.PNGrantTokenOperation){
        }

        public GrantTokenBuilder SetParams(Dictionary<string, int> resChannels, Dictionary<string, int> resGroups, Dictionary<string, int> resUsers, Dictionary<string, int> resSpaces, Dictionary<string, int> resUUIDs, Dictionary<string, int> patUsers, Dictionary<string, int> patSpaces, Dictionary<string, int> patChannels, Dictionary<string, int> patGroups, Dictionary<string, int> patUUIDs, int ttl, string authorizedUUID, Dictionary<string, object> meta){
            this.ResUsers = resUsers;
            this.ResSpaces = resSpaces;
            this.ResUUIDs = resUUIDs;
            this.ResChannels = resChannels;
            this.ResGroups = resGroups;
            this.PatUsers = patUsers;
            this.PatSpaces = patSpaces;
            this.PatUUIDs = patUUIDs;
            this.PatChannels = patChannels;
            this.PatGroups = patGroups;
            this.AuthorizedUUID = authorizedUUID;
            this.TTL = ttl;
            this.Meta = meta;
            return this;
        }

        public void Async(Action<PNGrantTokenResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }

        protected override void RunWebRequest(QueueManager qm){
            var permissions = new Dictionary<string, object>(){
                {"resources", new Dictionary<string, object>(){
                    {"channels", ResChannels },
                    {"groups", ResGroups },
                    {"users", ResUsers },
                    {"spaces", ResSpaces },
                    {"uuids", ResUUIDs },
                }},
                {"patterns",  new Dictionary<string, object>(){
                    {"channels", PatChannels },
                    {"groups", PatGroups },
                    {"users", PatUsers },
                    {"spaces", PatSpaces },
                    {"uuids", PatUUIDs },

                }},
                {"meta", Meta},
                {"uuid", AuthorizedUUID}
            };

            Dictionary<string, object> objBody = new Dictionary<string, object>(){
                {"ttl", TTL.ToString()},
                {"permissions", permissions}
            };
            string body = this.PubNubInstance.JsonLibrary.SerializeToJsonString(objBody);

            Uri request = BuildGrantAccessRequest(
                base.PubNubInstance.PNConfig.PublishKey,
                base.PubNubInstance.PNConfig.SubscribeKey,
                "",
                base.PubNubInstance.PNConfig.SecretKey,
                this.PubNubInstance,
                body
            );
            RequestState requestState = new RequestState ();
            requestState.OperationType = PNOperationType.PNGrantTokenOperation;
            requestState.URL = request.OriginalString; 
            requestState.Timeout = base.PubNubInstance.PNConfig.NonSubscribeTimeout;
            requestState.Pause = 0;
            requestState.Reconnect = false;
            requestState.httpMethod = HTTPMethod.Post;
            requestState.POSTData = body;

            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this);             
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNGrantTokenResult pnGrantTokenResult = new PNGrantTokenResult();
            PNStatus pnStatus = new PNStatus();            

            try{
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
                
                if(dictionary != null) {
                    object objData;
                    dictionary.TryGetValue("data", out objData);
                    if(objData!=null){
                        Dictionary<string, object> dataDictionary = objData as Dictionary<string, object>;
                        if(dataDictionary!=null){
                            dataDictionary.TryGetValue("token", out objData);
                            pnGrantTokenResult.Token = objData.ToString();
                            this.PubNubInstance.Token = pnGrantTokenResult.Token;
                        } else {
                            pnGrantTokenResult = null;
                            pnStatus = base.CreateErrorResponseFromException(new PubNubException("dataDictionary null"), requestState, PNStatusCategory.PNUnknownCategory);

                        }
                    }  else {
                        pnGrantTokenResult = null;
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("objData null"), requestState, PNStatusCategory.PNUnknownCategory);
                    }                      
                } 
                #if (ENABLE_PUBNUB_LOGGING)
                else {
                    this.PubNubInstance.PNLog.WriteToLog ("dictionary null", PNLoggingMethod.LevelInfo);
                }
                #endif
            } catch (Exception ex){
                pnGrantTokenResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("ex: {0}", ex.ToString()), PNLoggingMethod.LevelError);
                #endif
            }          
            Callback(pnGrantTokenResult, pnStatus);
        }

        internal static Uri BuildGrantAccessRequest (string publishKey, string subscribeKey, string cipherKey, string secretKey, PubNubUnity pn, string body)
        {
            string signature = "0";
            string httpMethod = "POST";
            string protocol = "https://";
            long timeStamp = Utility.TranslateDateTimeToSeconds (DateTime.UtcNow);
            string queryString = "";

            string path = string.Format("/v3/pam/{0}/grant", subscribeKey);
            string query = string.Format("timestamp={0}", timeStamp.ToString());

            Debug.Log("path ===== >" + path + secretKey);

            if (secretKey.Length > 0) {
                signature = CreateV2Signature(httpMethod, path, query, publishKey, cipherKey, secretKey, body);
                queryString = string.Format ("&signature={0}", signature); 
            }

            StringBuilder grantQ = new StringBuilder(protocol); 
            grantQ.Append(pn.PNConfig.Origin);
            grantQ.Append(path);
            grantQ.Append("?");
            grantQ.Append(query);
            grantQ.Append(queryString);
            Debug.Log("grantQ ===== >" + grantQ.ToString());

            Uri request = new Uri(grantQ.ToString());
            Debug.Log("request ===== >" + request.ToString());
            return request;
        }

        public static string CreateV2Signature(string httpMethod, string path, string query, string publishKey, string cipherKey, string secretKey, string body){
            StringBuilder stringToSign = new StringBuilder ();
            stringToSign.Append (httpMethod)
                .Append ("\n")
                .Append (publishKey)
                .Append ("\n")
                .Append (path)
                .Append ("\n")
                .Append (query)
                .Append ("\n")
                .Append (body);

            PNLoggingMethod PNLog = new PNLoggingMethod(PNLogVerbosity.BODY);    

            PubnubCrypto pubnubCrypto = new PubnubCrypto (cipherKey, PNLog);
            string signature = pubnubCrypto.PubnubAccessManagerSign (secretKey, stringToSign.ToString ());
            signature = signature.TrimEnd('=');
            signature = string.Format("v2.{0}", signature);
            Debug.Log("signature ===== >" + signature);
            return signature;
        }
    }
}