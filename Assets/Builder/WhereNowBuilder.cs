using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class WhereNowBuilder: PubNubNonSubBuilder<WhereNowBuilder>, IPubNubNonSubscribeBuilder<WhereNowBuilder, PNWhereNowResult>
    {
        protected Action<PNWhereNowResult, PNStatus> Callback;  
        public string UuidForWhereNow { get; private set;}

        public WhereNowBuilder(PubNubUnity pn): base(pn){
            Debug.Log ("WhereNowBuilder Construct");
        }

        public WhereNowBuilder Uuid(string uuid){
            UuidForWhereNow = uuid;
            return this;
        }

        #region IPubNubBuilder implementation

        public void Async(Action<PNWhereNowResult, PNStatus> callback)
        {
            this.Callback = callback;
            Debug.Log ("PNWherNowResult Async");
            base.Async<PNWhereNowResult>(callback, PNOperationType.PNWhereNowOperation, CurrentRequestType.NonSubscribe, this);
        }

        protected override void RunWebRequest(QueueManager qm){
            RequestState<PNWhereNowResult> requestState = new RequestState<PNWhereNowResult> ();
            requestState.RespType = PNOperationType.PNWhereNowOperation;

            Debug.Log ("WhereNowBuilder UuidForWhereNow: " + this.UuidForWhereNow);

            //TODO verify is this uuid is passed
            string uuidForWhereNow = this.PubNubInstance.PNConfig.UUID;
            if(!string.IsNullOrEmpty(this.UuidForWhereNow)){
                uuidForWhereNow = this.UuidForWhereNow;
            }

            Uri request = BuildRequests.BuildWhereNowRequest(
                uuidForWhereNow,
                this.PubNubInstance.PNConfig.UUID,
                this.PubNubInstance.PNConfig.Secure,
                this.PubNubInstance.PNConfig.Origin,
                this.PubNubInstance.PNConfig.AuthKey,
                this.PubNubInstance.PNConfig.SubscribeKey,
                this.PubNubInstance.Version
            );
            this.PubNubInstance.PNLog.WriteToLog(string.Format("RunWhereNowRequest {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            base.RunWebRequest<PNWhereNowResult>(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult){
            PNWhereNowResult pnWhereNowResult = new PNWhereNowResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();

            if (dictionary!=null && dictionary.ContainsKey("error") && dictionary["error"].Equals(true)){
                pnWhereNowResult = null;
                pnStatus.Error = true;
                //TODO create error data
            } else if(dictionary!=null && dictionary.ContainsKey("payload")){
                Dictionary<string, object> payload = dictionary["payload"] as Dictionary<string, object>;
                //TODO check channels null
                
                string[] ch = payload["channels"] as string[];
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
                pnStatus.Error = false;
            } else{
                pnWhereNowResult = null;
                pnStatus.Error = true;
            }
            Callback(pnWhereNowResult, pnStatus);
        }
        #endregion
    }
}

