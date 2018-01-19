using System;
using UnityEngine;
using System.Linq;

namespace PubNubAPI
{
    public class PubNubUnityBase
    {
        protected Counter publishMessageCounter;
        private const string build = "4.0-beta5";
        private string pnsdkVersion = string.Format ("PubNub-CSharp-Unity/{0}", build);

        public string Version {
            get {
                return pnsdkVersion;
            }
            private set{
                pnsdkVersion = value;
            }
        }
        
        internal PNLoggingMethod PNLog;
        public PNConfiguration PNConfig { get; set;}
        internal QueueManager QManager { get; set;}
        private IJsonLibrary jsonLibrary;
        public IJsonLibrary JsonLibrary {
            get {
                if (jsonLibrary == null)
                {
                    jsonLibrary = JSONSerializer.JsonLibrary(this);
                }
                return jsonLibrary;
            }

            set {
                if ((value != null) && (value is IJsonLibrary)) {
                    jsonLibrary = value;
                } else {
                    jsonLibrary = JSONSerializer.JsonLibrary(this);
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PNLog.WriteToLog ("Missing or Incorrect JsonLibrary value, using default", PNLoggingMethod.LevelWarning);
                    #endif
                }
            }
        }
        internal GameObject GameObjectRef { get; set;}
        public Subscription SubscriptionInstance { get; set;}
        internal SubscriptionWorker<SubscribeEnvelope> SubWorker { get; set;}
        internal bool localGobj;
        public readonly PNLatencyManager latency;
        public PNLatencyManager Latency{
            get {return latency;}
        }


        public PubNubUnityBase(PNConfiguration pnConfiguration, GameObject gameObjectRef, IJsonLibrary jsonLibrary){
            PNConfig = pnConfiguration;
            PNLog = new PNLoggingMethod(PNConfig.LogVerbosity);
            #if(UNITY_IOS)
            Version = string.Format("PubNub-CSharp-UnityIOS/{0}", build);
            #elif(UNITY_STANDALONE_WIN)
            Version = string.Format("PubNub-CSharp-UnityWin/{0}", build);
            #elif(UNITY_STANDALONE_OSX)
            Version = string.Format("PubNub-CSharp-UnityOSX/{0}", build);
            #elif(UNITY_ANDROID)
            Version = string.Format("PubNub-CSharp-UnityAndroid/{0}", build);
            #elif(UNITY_STANDALONE_LINUX)
            Version = string.Format("PubNub-CSharp-UnityLinux/{0}", build);
            #elif(UNITY_WEBPLAYER)
            Version = string.Format("PubNub-CSharp-UnityWeb/{0}", build);
            #elif(UNITY_WEBGL)
            Version = string.Format("PubNub-CSharp-UnityWebGL/{0}", build);
            #else
            Version = string.Format("PubNub-CSharp-Unity/{0}", build);
            #endif
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog (Version, PNLoggingMethod.LevelInfo);
            #endif

            if (gameObjectRef == null) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog ("Initilizing new GameObject", PNLoggingMethod.LevelInfo);
                #endif
                GameObjectRef = new GameObject ("PubnubGameObject");
                localGobj = true;
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog ("Reusing already initialized GameObject", PNLoggingMethod.LevelInfo);
                #endif
                GameObjectRef = gameObjectRef;
                localGobj = false;
            }
            publishMessageCounter = new Counter ();
            
            QManager = GameObjectRef.AddComponent<QueueManager> ();
            latency = GameObjectRef.AddComponent<PNLatencyManager> ();
            QManager.NoOfConcurrentRequests = PNConfig.ConcurrentNonSubscribeWorkers;
        }

         public void CleanUp (){
            publishMessageCounter.Reset ();
            if (QManager != null) {
                UnityEngine.Object.Destroy (QManager);
            }
            if (Latency != null) {
                Latency.CleanUp();
                UnityEngine.Object.Destroy (Latency);
            }
            
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog ("CleanUp: Destructing GameObject", PNLoggingMethod.LevelInfo);
            #endif
            
            try{
                if(localGobj && (GameObjectRef != null))
                {
                    UnityEngine.Object.Destroy (GameObjectRef);
                } 
            }catch (Exception ex){
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format("CleanUp: Destructing GameObject exception {0}", ex.ToString()), PNLoggingMethod.LevelInfo);
                #endif
            }
         }
    }
}