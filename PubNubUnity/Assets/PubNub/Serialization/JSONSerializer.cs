#if((!USE_JSONFX_UNITY_IOS) && (!USE_MiniJSON) && (!USE_NEWTONSOFT_JSON))
// #define USE_JSONFX_UNITY_IOS
// #define USE_MiniJSON
#define USE_NEWTONSOFT_JSON
#endif

#if (USE_JSONFX_UNITY_IOS)
using Pathfinding.Serialization.JsonFx;
#elif (USE_MiniJSON)
using MiniJSON;
#elif (USE_NEWTONSOFT_JSON)
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace PubNubAPI
{
    public static class ListExtensions
    {
        // https://docs.unity3d.com/Manual/ScriptingRestrictions.html
        public static void UsedOnlyForAOTCodeGeneration() 
        {
            // IL2CPP workarounds
            List<System.Double> ld = new List<System.Double>();
            System.Double[] d = ConvertToAnArray<System.Double>(ld);
            List<System.Int32> li32 = new List<System.Int32>();
            System.Int32[] i32 = ConvertToAnArray<System.Int32>(li32);
            List<System.Int16> li16 = new List<System.Int16>();
            System.Int16[] i16 = ConvertToAnArray<System.Int16>(li16);
            List<System.Int64> li64 = new List<System.Int64>();
            System.Int64[] l = ConvertToAnArray<System.Int64>(li64);
            List<System.String> ls = new List<System.String>();
            System.String[] s = ConvertToAnArray<System.String>(ls);
            List<int> li = new List<int>();
            int[] i2 = ConvertToAnArray<int>(li);
            List<double> ld1 = new List<double>();
            double[] d2 = ConvertToAnArray<double>(ld1);
            List<long> ll = new List<long>();
            long[] l2 = ConvertToAnArray<long>(ll);
            List<string> ls1 = new List<string>();
            string[] s1 = ConvertToAnArray<string>(ls1);

            // Include an exception so we can be sure to know if this method is ever called.
            throw new InvalidOperationException("This method is used for AOT code generation only. Do not call it at runtime.");
        }
        public static T[] ConvertToAnArray<T>(IList list)
        {
            return list.Cast<T>().ToArray();
        }

        public static object ConvertToDynamicArray(IList list, Type whatType, PubNubUnityBase  pnUnityBase)
        {
            var method = typeof(ListExtensions).GetMethod("ConvertToAnArray", BindingFlags.Static | BindingFlags.Public, null, new [] { typeof(IList)}, null);
            var genericMethod = method.MakeGenericMethod(whatType);
            var inv = genericMethod.Invoke(null, new object[] {list});
            #if (ENABLE_PUBNUB_LOGGING)
            pnUnityBase.PNLog.WriteToLog (string.Format("{0} and {1}", whatType, inv.GetType()), PNLoggingMethod.LevelInfo);
            #endif
            return (object)inv;
        }
    }

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
            #elif (USE_SimpleJSON)
                #if (ENABLE_PUBNUB_LOGGING)
                pnUnityBase.PNLog.WriteToLog ("JSON LIB: USE_SimpleJSON", PNLoggingMethod.LevelInfo);
                #endif
                jsonLibrary = new SimpleJSONSerializer (pnUnityBase);
            #elif (USE_NEWTONSOFT_JSON)
                #if (ENABLE_PUBNUB_LOGGING)
                pnUnityBase.PNLog.WriteToLog ("JSON LIB: USE_NEWTONSOFT_JSON", PNLoggingMethod.LevelInfo);
                #endif
                jsonLibrary = new NewtonsoftJsonSerializer(pnUnityBase);
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
    #elif (USE_NEWTONSOFT_JSON)
    public class NewtonsoftJsonSerializer : IJsonLibrary {
        readonly PubNubUnityBase  pnUnityBase;

        private readonly JsonSerializerSettings defaultSettings;
        
        public NewtonsoftJsonSerializer(PubNubUnityBase pnUnityBase) {

            this.pnUnityBase = pnUnityBase;
            defaultSettings = new JsonSerializerSettings() {
                Context = new StreamingContext(),
                Culture = CultureInfo.InvariantCulture,
                ReferenceLoopHandling = ReferenceLoopHandling.Error,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include,
                ObjectCreationHandling = ObjectCreationHandling.Auto,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ConstructorHandling = ConstructorHandling.Default,
                TypeNameHandling = TypeNameHandling.None,
                MetadataPropertyHandling = MetadataPropertyHandling.Default,
                Formatting = Formatting.None,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
                DateParseHandling = DateParseHandling.DateTime,
                FloatParseHandling = FloatParseHandling.Double,
                FloatFormatHandling = FloatFormatHandling.String,
                StringEscapeHandling = StringEscapeHandling.Default,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                CheckAdditionalContent = false,
                DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK",
                MaxDepth = 64
            };
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
            string json = JsonConvert.SerializeObject(objectToSerialize, defaultSettings); 
            return EncodeNonAsciiCharacters(json);
        }

        public List<object> DeserializeToListOfObject (string jsonString)
        {
            
            #if (ENABLE_PUBNUB_LOGGING)
            pnUnityBase.PNLog.WriteToLog (string.Format ("DeserializeToListOfObject: jsonString: {0}", jsonString), PNLoggingMethod.LevelInfo);
            #endif

            var output = JsonConvert.DeserializeObject<List<object>>(jsonString, defaultSettings);
            return output;
        }

        public object DeserializeToObject (string jsonString)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            pnUnityBase.PNLog.WriteToLog (string.Format ("DeserializeToObject: jsonString: {0}", jsonString), PNLoggingMethod.LevelInfo);
            #endif
            jsonString = DecodeEncodedNonAsciiCharacters(jsonString);
            var output = JsonConvert.DeserializeObject<object> (jsonString, defaultSettings);
            
            #if (ENABLE_PUBNUB_LOGGING)
            pnUnityBase.PNLog.WriteToLog (string.Format("DeserializeToObject: type {0} decoded jsonString: {1}", output.GetType(), jsonString), PNLoggingMethod.LevelInfo);
            #endif

            if(output.GetType().ToString() == "Newtonsoft.Json.Linq.JArray"){
                JArray outputArr = output as JArray;
                bool isIntArr = true;
                bool isArr = false;
                #if (ENABLE_PUBNUB_LOGGING)
                pnUnityBase.PNLog.WriteToLog (string.Format("outputArr.Type {0}", outputArr.Type), PNLoggingMethod.LevelInfo);
                #endif

                foreach(object obj in outputArr){
                    if(obj.GetType().ToString() == "Newtonsoft.Json.Linq.JValue"){
                        JValue item = obj as JValue;
                        if(item.Type.ToString() != "Integer"){
                            isIntArr = false;
                            break;
                        }
                    } else if(obj.GetType().ToString() == "Newtonsoft.Json.Linq.JArray"){
                        isIntArr = false;
                        isArr = true;
                        break;
                    } else {
                        #if (ENABLE_PUBNUB_LOGGING)
                        pnUnityBase.PNLog.WriteToLog (string.Format("DeserializeToObject OBJ {0}", obj.GetType()), PNLoggingMethod.LevelInfo);
                        #endif
                        isIntArr = false;
                        break;
                    }
                }
                if(isIntArr){
                    Int64[] intArr = outputArr.ToObject<Int64[]>();
                    return (object)intArr;
                } else if(isArr){
                    return deserializeToDictionary(jsonString, true);;
                } else {
                    #if (ENABLE_PUBNUB_LOGGING)
                    pnUnityBase.PNLog.WriteToLog (string.Format("DeserializeToObject outputArr {0}", outputArr.GetType()), PNLoggingMethod.LevelInfo);
                    #endif
                    object[] objArr = outputArr.ToObject<object[]>();
                    return (object)objArr;
                }
            } else if(output.GetType().ToString() == "Newtonsoft.Json.Linq.JObject"){
                return deserializeToDictionary(jsonString, false);
            } 
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                pnUnityBase.PNLog.WriteToLog (string.Format("DeserializeToObject TYPE  {0}", output.GetType()), PNLoggingMethod.LevelInfo);
            }
            #endif

            return output;
        }
        static string EncodeNonAsciiCharacters( string value ) {
            StringBuilder sb = new StringBuilder();
            foreach( char c in value ) {
                if( c > 127 ) {
                    // This character is too big for ASCII
                    string encodedValue = "\\u" + ((int) c).ToString( "x4" );
                    sb.Append( encodedValue );
                }
                else {
                    sb.Append( c );
                }
            }
            return sb.ToString();
        }


        static string DecodeEncodedNonAsciiCharacters( string value ) {
            return Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => {
                    return ((char) int.Parse( m.Groups["Value"].Value, NumberStyles.HexNumber )).ToString();
                } );
        }

        private object deserializeToDictionary(string jo, bool isArray=false)
        {
            
            if (!isArray)
            {
                isArray = jo.Substring(0, 1) == "[";
            }
            if (!isArray)
            {
                
                var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(jo, defaultSettings);
                #if (ENABLE_PUBNUB_LOGGING)
                pnUnityBase.PNLog.WriteToLog (string.Format("JsonConvert.SerializeObject(values) {0}", JsonConvert.SerializeObject(values, defaultSettings)), PNLoggingMethod.LevelInfo);
                #endif
                
                var values2 = new Dictionary<string, object>();
                foreach (KeyValuePair<string, object> d in values)
                {
                    if (d.Value is JObject)
                    {
                        #if (ENABLE_PUBNUB_LOGGING)
                        pnUnityBase.PNLog.WriteToLog (string.Format("1: d.Key {0}, d.Value {1}", d.Key, d.Value), PNLoggingMethod.LevelInfo);
                        #endif
                        values2.Add(d.Key, deserializeToDictionary(d.Value.ToString()));
                    }
                    else if (d.Value is JArray)
                    {
                        #if (ENABLE_PUBNUB_LOGGING)
                        pnUnityBase.PNLog.WriteToLog (string.Format("2: d.Key {0}, d.Value {1}", d.Key, d.Value), PNLoggingMethod.LevelInfo);
                        #endif
                        values2.Add(d.Key, deserializeToDictionary(d.Value.ToString(), true));
                    }
                    else
                    {
                        #if (ENABLE_PUBNUB_LOGGING)
                        pnUnityBase.PNLog.WriteToLog (string.Format("3: d.Key {0}, d.Value {1}", d.Key, d.Value), PNLoggingMethod.LevelInfo);
                        #endif
                        values2.Add(d.Key, d.Value);
                    }
                }
                return values2;
            }
            else
            {
                
                var values = JsonConvert.DeserializeObject<List<object>>(jo, defaultSettings);
                #if (ENABLE_PUBNUB_LOGGING)
                pnUnityBase.PNLog.WriteToLog (string.Format("2: JsonConvert.SerializeObject(values) {0}", JsonConvert.SerializeObject(values, defaultSettings)), PNLoggingMethod.LevelInfo);
                #endif

                Type whatType = typeof(object);
                Type currType = whatType;
                int count = 0;
                foreach (var d in values)
                {
                    if ((d is JObject) || (d is JArray)){
                        break;
                    }
                    if(count == 0){
                        currType = d.GetType();
                    } else if(!currType.Equals(d.GetType())){
                        break;
                    } 
                    count++;
                    if (count == values.Count){
                        whatType = currType;
                    }
                    currType = d.GetType();
                }
                #if (ENABLE_PUBNUB_LOGGING)
                pnUnityBase.PNLog.WriteToLog (string.Format("whatType {0}", whatType), PNLoggingMethod.LevelInfo);
                #endif
                
                Type listType = typeof(List<>).MakeGenericType(new [] { whatType } );
                IList values2 = (IList)Activator.CreateInstance(listType);

                foreach (var d in values)
                {
                    #if (ENABLE_PUBNUB_LOGGING)
                    pnUnityBase.PNLog.WriteToLog (string.Format("d.GetType() {0}", d.GetType()), PNLoggingMethod.LevelInfo);
                    #endif
                    if (d is JObject)
                    {
                        #if (ENABLE_PUBNUB_LOGGING)
                        pnUnityBase.PNLog.WriteToLog (string.Format("1: d {0}", d), PNLoggingMethod.LevelInfo);
                        #endif
                        values2.Add(deserializeToDictionary(d.ToString()));
                    }
                    else if (d is JArray)
                    {
                        #if (ENABLE_PUBNUB_LOGGING)
                        pnUnityBase.PNLog.WriteToLog (string.Format("2: d {0}", d), PNLoggingMethod.LevelInfo);
                        #endif
                        values2.Add(deserializeToDictionary(d.ToString(), true));
                    }
                    else
                    {      
                        #if (ENABLE_PUBNUB_LOGGING)
                        pnUnityBase.PNLog.WriteToLog (string.Format("3: d {0}", d), PNLoggingMethod.LevelInfo);
                        #endif
                        values2.Add(d);
                    }
                }
                #if (ENABLE_PUBNUB_LOGGING)
                pnUnityBase.PNLog.WriteToLog (string.Format("values2.GetType() {0}", values2.GetType()), PNLoggingMethod.LevelInfo);
                #endif
                return ListExtensions.ConvertToDynamicArray(values2, whatType, pnUnityBase);
            }
        }

        public T Deserialize<T> (string jsonString)
        {
            var output = JsonConvert.DeserializeObject<T> (jsonString, defaultSettings);
            return output;
        }

        public Dictionary<string, object> DeserializeToDictionaryOfObject(string jsonString) {
            var output =
                JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString, defaultSettings);

            return output;
        }
    }      
    #elif (USE_SimpleJSON)
    public class SimpleJSONSerializer : IJsonLibrary
    {
        PubNubUnityBase pnUnityBase;
        public SimpleJSONSerializer(PubNubUnityBase pnUnityBase){
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
            string json = JsonUtility.ToJson (objectToSerialize); 
            return PubnubCryptoBase.ConvertHexToUnicodeChars (json);
        }

        public List<object> DeserializeToListOfObject (string jsonString)
        {
            
            #if (ENABLE_PUBNUB_LOGGING)
            pnUnityBase.PNLog.WriteToLog (string.Format ("DeserializeToListOfObject: jsonString: {0}", jsonString), PNLoggingMethod.LevelInfo);
            #endif
        
            var output = JsonUtility.FromJson<object[]> (jsonString) as object[];
            List<object> messageList = output.Cast<object> ().ToList ();
            return messageList;
        }

        public object DeserializeToObject (string jsonString)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            pnUnityBase.PNLog.WriteToLog (string.Format ("DeserializeToObject: jsonString: {0}", jsonString), PNLoggingMethod.LevelInfo);
            #endif
        
            var output = JsonUtility.FromJson<object> (jsonString);
            return output;
        }

        public T Deserialize<T> (string jsonString)
        {
            var output = JsonUtility.FromJson<T> (jsonString);
            return output;
        }

        public Dictionary<string, object> DeserializeToDictionaryOfObject (string jsonString)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            pnUnityBase.PNLog.WriteToLog (string.Format ("DeserializeToDictionaryOfObject: before"), PNLoggingMethod.LevelInfo);
            #endif

            object obj = DeserializeToObject (jsonString);
            #if (ENABLE_PUBNUB_LOGGING)
            pnUnityBase.PNLog.WriteToLog (string.Format ("DeserializeToDictionaryOfObject: after {0}", obj.ToString()), PNLoggingMethod.LevelInfo);
            #endif
            
            Dictionary<string, object> stateDictionary = new Dictionary<string, object> ();
            Dictionary<string, object> message = (Dictionary<string, object>)obj;
            if (message != null) {
                foreach (KeyValuePair<String, object> kvp in message) {
                    stateDictionary.Add (kvp.Key, kvp.Value);
                }
            } else {
            #if (ENABLE_PUBNUB_LOGGING)
            pnUnityBase.PNLog.WriteToLog (string.Format ("DeserializeToDictionaryOfObject: message null {0}", obj.ToString()), PNLoggingMethod.LevelInfo);
            #endif
            }
            return stateDictionary;
        }
    }       

    #endif
    #endregion
}

