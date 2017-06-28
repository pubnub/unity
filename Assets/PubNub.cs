using System;
using UnityEngine;
using System.Linq;

namespace PubNubAPI
{
    public class PubNub{
        public PNConfiguration PNConfig { get; set;}
        private PubNubUnity pnUnity;

        public GameObject GameObjectRef {
            get {
                return pnUnity.GameObjectRef;
            }
        }

        public event EventHandler<EventArgs> SusbcribeCallback; 
        public void RaiseEvent(EventArgs ea){
            if (SusbcribeCallback != null) {
                SusbcribeCallback.Raise (typeof(PubNub), ea);
            }
        }

        public string Version {
            get {
                return pnUnity.Version;
            }
        }
        
        public PubNub (PNConfiguration pnConfiguration): this(pnConfiguration, null, null)
        {

        }

        public PubNub (PNConfiguration pnConfiguration, GameObject gameObjectRef) : this(pnConfiguration, gameObjectRef, null)
        {
        }

        public PubNub (PNConfiguration pnConfiguration, GameObject gameObjectRef, IJsonLibrary jsonLibrary)
        {
            pnUnity = new PubNubUnity(pnConfiguration, gameObjectRef, jsonLibrary);
            
            pnUnity.SusbcribeCallback += (sender, e) => { //; //+= (pnStatus, pnMessageResut, pnPresenceEventResult) => {
                SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;

                Debug.Log ("SusbcribeCallback PN");
                RaiseEvent(mea);
            };
        }

         public SubscribeBuilder Subscribe(){
            return pnUnity.Subscribe();
        }

        public TimeBuilder Time(){
            Debug.Log ("TimeBuilder");
            return pnUnity.Time();
        }

        public WhereNowBuilder WhereNow(){
            Debug.Log ("WhereNowBuilder");
            return pnUnity.WhereNow();
        }

        public HistoryBuilder History(){
            Debug.Log ("HistoryBuilder");
            return pnUnity.History();
        }

        public void AddListener(Action<PNStatus> callback, Action<PNMessageResult> callback2, Action<PNPresenceEventResult> callback3)
        {
            pnUnity.AddListener(callback, callback2, callback3);
        }


    }
}