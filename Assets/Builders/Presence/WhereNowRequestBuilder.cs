using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class WhereNowRequestBuilder: PubNubNonSubBuilder<WhereNowRequestBuilder, PNWhereNowResult>, IPubNubNonSubscribeBuilder<WhereNowRequestBuilder, PNWhereNowResult>
    {
        private string UuidForWhereNow { get; set;}

        public WhereNowRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNWhereNowOperation){
        }

        public WhereNowRequestBuilder Uuid(string uuid){
            UuidForWhereNow = uuid;
            return this;
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNWhereNowResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;

            Debug.Log ("WhereNowBuilder UuidForWhereNow: " + this.UuidForWhereNow);

            //TODO verify is this uuid is passed
            string uuidForWhereNow = this.PubNubInstance.PNConfig.UUID;
            if(!string.IsNullOrEmpty(this.UuidForWhereNow)){
                uuidForWhereNow = this.UuidForWhereNow;
            }

            /* Uri request = BuildRequests.BuildWhereNowRequest(
                uuidForWhereNow,
                this.PubNubInstance.PNConfig.UUID,
                this.PubNubInstance.PNConfig.Secure,
                this.PubNubInstance.PNConfig.Origin,
                this.PubNubInstance.PNConfig.AuthKey,
                this.PubNubInstance.PNConfig.SubscribeKey,
                this.PubNubInstance.Version
            ); */
            Uri request = BuildRequests.BuildWhereNowRequest(
                uuidForWhereNow,
                ref this.PubNubInstance
            );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        // protected override void CreateErrorResponse(Exception exception, bool showInCallback, bool level){
            
        // }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //{"status": 200, "message": "OK", "payload": {"channels": ["channel2", "channel1"]}, "service": "Presence"}
            //TODO read other values.
            
            PNWhereNowResult pnWhereNowResult = new PNWhereNowResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if(dictionary != null) {
                string message = Utility.ReadMessageFromResponseDictionary(dictionary, "message");
                if(Utility.CheckDictionaryForError(dictionary, "error")){
                    pnWhereNowResult = null;
                    pnStatus = base.CreateErrorResponseFromMessage(message, requestState, PNStatusCategory.PNUnknownCategory);
                } else {
                    object objPayload;
                    dictionary.TryGetValue("payload", out objPayload);
                    Dictionary<string, object> payload = objPayload as Dictionary<string, object>;
                    
                    if(payload!=null){ 
                        object objChannels;
                        payload.TryGetValue("channels", out objChannels);
                        if(objChannels != null){
                            string[] ch = objChannels as string[];
                            //TODO check channels null
                            if(ch != null){
                                List<string> channels = ch.ToList<string>();//new List<string> ();
                                /*foreach(KeyValuePair<string, object> key in dictionary["payload"] as Dictionary<string, object>){
                                    Debug.Log(key.Key + key.Value);
                                    result1.Add (key.Value as string);
                                }
                                foreach(string key in channels){
                                    Debug.Log(key);
                                }*/

                                //result1.Add (multiChannel);
                                //List<string> result1 = ((IEnumerable)deSerializedResult).Cast<string> ().ToList ();
                                pnWhereNowResult.Channels = channels;
                            } else {
                                pnWhereNowResult = null;
                                pnStatus = base.CreateErrorResponseFromMessage("channels are null", requestState, PNStatusCategory.PNMalformedResponseCategory);
                            }
                        } else {
                            pnWhereNowResult = null;
                            pnStatus = base.CreateErrorResponseFromMessage("channels object is null", requestState, PNStatusCategory.PNMalformedResponseCategory);
                        }
                    } else {
                        pnWhereNowResult = null;
                        pnStatus = base.CreateErrorResponseFromMessage("payload is null", requestState, PNStatusCategory.PNMalformedResponseCategory);
                    }
                }
            } else {
                pnWhereNowResult = null;
                pnStatus = base.CreateErrorResponseFromMessage("Response dictionary is null", requestState, PNStatusCategory.PNMalformedResponseCategory);
            }

            Callback(pnWhereNowResult, pnStatus);
        }
       
    }
}

