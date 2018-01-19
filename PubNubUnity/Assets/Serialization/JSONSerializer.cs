#if((!USE_JSONFX_UNITY_IOS) && (!USE_MiniJSON))
#define USE_JSONFX_UNITY_IOS
//#define USE_MiniJSON
#endif

#if (USE_JSONFX_UNITY_IOS)
using Pathfinding.Serialization.JsonFx;
#elif (USE_MiniJSON)
using MiniJSON;
#endif

using System;
using System.Collections.Generic;
using System.Linq;

namespace PubNubAPI
{
    #region "Json Pluggable Library"
    public interface IJsonLibrary
    {
        bool IsArrayCompatible (string jsonString);

        bool IsDictionaryCompatible (string jsonString);

        string SerializeToJsonString (object objectToSerialize);

        List<object> DeserializeToListOfObject (string jsonString);

        object DeserializeToObject (string jsonString);

        Dictionary<string, object> DeserializeToDictionaryOfObject (string jsonString);
    }

    public static class JSONSerializer{
        public static IJsonLibrary JsonLibrary(PubNubUnityBase pnUnityBase){
            IJsonLibrary jsonLibrary;
            #if (USE_MiniJSON)
                #if (ENABLE_PUBNUB_LOGGING)
                pnUnityBase.PNLog.WriteToLog("JSON LIB: USE_MiniJSON", PNLoggingMethod.LevelInfo);
                #endif
                jsonLibrary = new MiniJSONObjectSerializer(pnUnityBase);
            #elif (USE_JSONFX_UNITY_IOS)
                #if (ENABLE_PUBNUB_LOGGING)
                pnUnityBase.PNLog.WriteToLog ("JSON LIB: USE_JSONFX_UNITY_IOS", PNLoggingMethod.LevelInfo);
                #endif
                jsonLibrary = new JsonFxUnitySerializer (pnUnityBase);
            #endif
            return jsonLibrary;
        }

    }

    #if (USE_JSONFX_UNITY_IOS)
    public class JsonFxUnitySerializer : IJsonLibrary
    {
        readonly PubNubUnityBase  pnUnityBase;
        public JsonFxUnitySerializer(PubNubUnityBase pnUnityBase){
            this.pnUnityBase = pnUnityBase;
        }

        public bool IsArrayCompatible (string jsonString)
        {
            return false;
        }

        public bool IsDictionaryCompatible (string jsonString)
        {
            return true;
        }

        public string SerializeToJsonString (object objectToSerialize)
        {
            string json = JsonWriter.Serialize (objectToSerialize); 
            return PubnubCryptoBase.ConvertHexToUnicodeChars (json);
        }

        public List<object> DeserializeToListOfObject (string jsonString)
        {
            
            #if (ENABLE_PUBNUB_LOGGING)
            pnUnityBase.PNLog.WriteToLog (string.Format ("DeserializeToListOfObject: jsonString: {0}", jsonString), PNLoggingMethod.LevelInfo);
            #endif
        
            var output = JsonReader.Deserialize<object[]> (jsonString) as object[];
            List<object> messageList = output.Cast<object> ().ToList ();
            return messageList;
        }

        public object DeserializeToObject (string jsonString)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            pnUnityBase.PNLog.WriteToLog (string.Format ("DeserializeToObject: jsonString: {0}", jsonString), PNLoggingMethod.LevelInfo);
            #endif
        
            var output = JsonReader.Deserialize<object> (jsonString) as object;
            return output;
        }

        public T Deserialize<T> (string jsonString)
        {
            var output = JsonReader.Deserialize<T> (jsonString);
            return output;
        }

        public Dictionary<string, object> DeserializeToDictionaryOfObject (string jsonString)
        {
            object obj = DeserializeToObject (jsonString);
            Dictionary<string, object> stateDictionary = new Dictionary<string, object> ();
            Dictionary<string, object> message = (Dictionary<string, object>)obj;
            if (message != null) {
                foreach (KeyValuePair<String, object> kvp in message) {
                    stateDictionary.Add (kvp.Key, kvp.Value);
                }
            }
            return stateDictionary;
        }
    }
    #elif (USE_MiniJSON)
    public class MiniJSONObjectSerializer : IJsonLibrary
    {
        PubNubUnityBase pnUnityBase;
        public MiniJSONObjectSerializer(PubNubUnityBase pnUnityBase){
            this.pnUnityBase = pnUnityBase;
        }

        public bool IsArrayCompatible (string jsonString)
        {
            return jsonString.Trim().StartsWith("[");
        }

        public bool IsDictionaryCompatible (string jsonString)
        {
            return jsonString.Trim().StartsWith("{");
        }

        public string SerializeToJsonString (object objectToSerialize)
        {
            string json = Json.Serialize (objectToSerialize); 
            return PubnubCryptoBase.ConvertHexToUnicodeChars (json);
        }

        public List<object> DeserializeToListOfObject (string jsonString)
        {
            return Json.Deserialize (jsonString) as List<object>;
        }

        public object DeserializeToObject (string jsonString)
        {
            return Json.Deserialize (jsonString) as object;
        }

        public Dictionary<string, object> DeserializeToDictionaryOfObject (string jsonString)
        {
            return Json.Deserialize (jsonString) as Dictionary<string, object>;
        }
    }
    #endif
    #endregion
}

