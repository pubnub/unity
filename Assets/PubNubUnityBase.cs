using System;
using UnityEngine;
using System.Linq;

namespace PubNubAPI
{
    public class PubNubUnityBase
    {
        private const string build = "4.0";
        private string pnsdkVersion = string.Format ("PubNub-CSharp-Unity/{0}", build);

        public string Version {
            get {
                return pnsdkVersion;
            }
            private set{
                pnsdkVersion = value;
            }
        }
        
        internal PNLoggingMethod PNLog {get; set;}
        public PNConfiguration PNConfig { get; set;}
        internal QueueManager QManager { get; set;}
        private IJsonLibrary jsonLibrary = null;
        public IJsonLibrary JsonLibrary {
            get {
                if (jsonLibrary == null)
                {
                    jsonLibrary = JSONSerializer.JsonLibrary(this);
                }
                return jsonLibrary;
            }

            set {
                if (value is IJsonLibrary) {
                    jsonLibrary = value;
                } else {
                    jsonLibrary = JSONSerializer.JsonLibrary(this);
                    this.PNLog.WriteToLog ("Missing or Incorrect JsonLibrary value, using default", PNLoggingMethod.LevelWarning);
                }
            }
        }
        public GameObject GameObjectRef { get; set;}
        internal Subscription SubscriptionInstance { get; set;}
        internal SubscriptionWorker<SubscribeRequestBuilder> SubWorker { get; set;}
        internal bool localGobj;

        public PubNubUnityBase(PNConfiguration pnConfiguration, GameObject gameObjectRef, IJsonLibrary jsonLibrary){
            PNConfig = pnConfiguration;
            PNLog = new PNLoggingMethod(PNConfig.LogVerbosity);
            /*if (PNConfig.LogVerbosity.Equals (PNLogVerbosity.BODY)) {
				//Debug.logger.logEnabled = true;
			} else {
				//Debug.logger.logEnabled = false;
			}*/

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

            if (GameObjectRef == null) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog ("Initilizing new GameObject", PNLoggingMethod.LevelInfo);
                #endif
                GameObjectRef = new GameObject ("PubnubGameObject");
                localGobj = true;
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog ("Reusing already initialized GameObject", PNLoggingMethod.LevelInfo);
                #endif
                localGobj = false;
            }

            QManager = GameObjectRef.AddComponent<QueueManager> ();
            QManager.NoOfConcurrentRequests = PNConfig.ConcurrentNonSubscribeWorkers;
        }
    }
}