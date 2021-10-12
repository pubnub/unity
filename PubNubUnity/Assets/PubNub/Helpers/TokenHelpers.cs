using System;
using System.Collections.Generic;
using System.Linq;
using PeterO.Cbor;
using System.Text;


namespace PubNubAPI
{
    public enum PNGrantBitMask
    {
        PNRead = 1,
        PNWrite = 2,
        PNManage = 4,
        PNDelete = 8,
        PNCreate =16,
        PNGet = 32,
        PNUpdate = 64,
        PNJoin = 128
    }

    public enum PNGrantType
    {
        PNReadEnabled = 0,
        PNWriteEnabled,
        PNManageEnabled,
        PNDeleteEnabled,
        PNCreateEnabled,
        PNGetEnabled,
        PNUpdateEnabled,
        PNJoinEnabled
    }

    public enum PNResourceType{
        PNChannels = 0,
        PNGroups,
        PNUUIDMetadata,
        PNChannelMetadata,
        PNUUIDs
    }

    public class TokenAuthValues {
        public bool Read {get; set;}
        public bool Write {get; set;}
        public bool Manage {get; set;}
        public bool Delete {get; set;}
        public bool Create {get; set;}
        public bool Get {get; set;}
        public bool Update {get; set;}
        public bool Join {get; set;}

    }

    public class TokenPermissionMappingBase{
        //chan
        public Dictionary<string, TokenAuthValues> Channels {get; set;}
        //grp
        public Dictionary<string, TokenAuthValues> Groups {get; set;}
        //usr
        public Dictionary<string, TokenAuthValues> Users {get; set;}
        //spc
        public Dictionary<string, TokenAuthValues> Spaces {get; set;}
        //uuid
        public Dictionary<string, TokenAuthValues> UUIDs {get; set;}

    }

    public class TokenPatterns: TokenPermissionMappingBase{
    }

    public class TokenResources: TokenPermissionMappingBase{
    }

    public class TokenContents{
        //res
        public TokenResources Resources {get; set;}
        //pat
        public TokenPatterns Patterns {get; set;}
        //meta
        public Dictionary<string, object> Meta {get; set;}
        //sig
        public string Signature {get; set;}
        //v
        public int Version {get; set;}
        //t
        public long Timestamp {get; set;}
        //ttl
        public int TTL {get; set;} 
        //uuid
        public string AuthorizedUUID {get; set;}
    }

    public static class TokenHelpers
    {
        public static TokenContents GetPermissions(PubNubUnity pnInstance, string token){
            token = token.Replace("-", "+").Replace("_", "/");
            int i = token.Length % 4;
            if (i != 0) {
                token += new String('=', 4-i);
            }
            #if (ENABLE_PUBNUB_LOGGING)
            pnInstance.PNLog.WriteToLog (token, PNLoggingMethod.LevelInfo);
            #endif
            TokenContents pnGrantTokenDecoded = new TokenContents();
            pnGrantTokenDecoded.Patterns = new TokenPatterns {
                Channels = new Dictionary<string, TokenAuthValues>(),
                Groups = new Dictionary<string, TokenAuthValues>(),
                Users = new Dictionary<string, TokenAuthValues>(),
                Spaces = new Dictionary<string, TokenAuthValues>(),
                UUIDs = new Dictionary<string, TokenAuthValues>()
            };
            pnGrantTokenDecoded.Resources = new TokenResources {
                Channels = new Dictionary<string, TokenAuthValues>(),
                Groups = new Dictionary<string, TokenAuthValues>(),
                Users = new Dictionary<string, TokenAuthValues>(),
                Spaces = new Dictionary<string, TokenAuthValues>(),
                UUIDs = new Dictionary<string, TokenAuthValues>()
            };
            pnGrantTokenDecoded.Meta = new Dictionary<string, object>();

            byte[] decryptedBytes = Convert.FromBase64CharArray (token.ToCharArray (), 0, token.Length);
            var cbor = CBORObject.DecodeFromBytes(decryptedBytes);

            ParseCBOR(pnInstance, cbor, "", ref pnGrantTokenDecoded); 

            return pnGrantTokenDecoded;
        }

        public static TokenContents ParseToken(PubNubUnity pnInstance, string token){
            
            return GetPermissions(pnInstance, token);
        }

        public static void ParseCBOR(PubNubUnity pnInstance, CBORObject cbor, string parent, ref TokenContents pnGrantTokenDecoded){
            foreach (KeyValuePair<CBORObject, CBORObject> kvp in cbor.Entries){
                if(kvp.Key.Type.ToString().Equals("ByteString")){
                    string key = System.Text.Encoding.ASCII.GetString(kvp.Key.GetByteString());
                    ParseCBORValue(pnInstance, key, parent, kvp, ref pnGrantTokenDecoded);
                } else if(kvp.Key.Type.ToString().Equals("TextString")) {
                    #if (ENABLE_PUBNUB_LOGGING)
                    pnInstance.PNLog.WriteToLog (string.Format("TextString Key {0}-{1}-{2}", kvp.Key.ToString(), kvp.Value.ToString(), kvp.Value.Type), PNLoggingMethod.LevelInfo);
                    #endif
                    ParseCBORValue(pnInstance, kvp.Key.ToString(), parent, kvp, ref pnGrantTokenDecoded);
                } 
                #if (ENABLE_PUBNUB_LOGGING)
                else {
                    pnInstance.PNLog.WriteToLog (string.Format("Others Key {0}-{1}-{2}-{3}", kvp.Key, kvp.Key.Type, kvp.Value, kvp.Value.Type), PNLoggingMethod.LevelError);
                }
                #endif
                
            }
        }

        public static void ParseCBORValue(PubNubUnity pnInstance, string key, string parent, KeyValuePair<CBORObject, CBORObject> kvp, ref TokenContents pnGrantTokenDecoded){
            if(kvp.Value.Type.ToString().Equals("Map")){
                #if (ENABLE_PUBNUB_LOGGING)
                pnInstance.PNLog.WriteToLog (string.Format("Map Key {0}", key), PNLoggingMethod.LevelInfo);
                #endif
                var p = string.Format("{0}{1}{2}", parent, string.IsNullOrEmpty(parent)?"":":", key);
                ParseCBOR(pnInstance, kvp.Value, p, ref pnGrantTokenDecoded);
            } else if(kvp.Value.Type.ToString().Equals("ByteString")){                 
                #if (ENABLE_PUBNUB_LOGGING)
                string val = System.Text.Encoding.ASCII.GetString(kvp.Value.GetByteString());
                pnInstance.PNLog.WriteToLog  (string.Format("ByteString Value {0}-{1}", key, val), PNLoggingMethod.LevelInfo);
                #endif
                FillGrantToken(pnInstance, parent, key, kvp.Value, typeof(string), ref pnGrantTokenDecoded);
            } else if(kvp.Value.Type.ToString().Equals("TextString")){                 
                #if (ENABLE_PUBNUB_LOGGING)
                string val = kvp.Value.ToString();
                pnInstance.PNLog.WriteToLog  (string.Format("TextString Value {0}-{1}", key, val), PNLoggingMethod.LevelInfo);
                #endif
                FillGrantToken(pnInstance, parent, key, kvp.Value, typeof(string), ref pnGrantTokenDecoded);
            } else if(kvp.Value.Type.ToString().Equals("Integer")){      
                #if (ENABLE_PUBNUB_LOGGING)                      
                pnInstance.PNLog.WriteToLog  (string.Format("Integer Value {0}-{1}", key, kvp.Value), PNLoggingMethod.LevelInfo);
                #endif
                FillGrantToken(pnInstance, parent, key, kvp.Value, typeof(int), ref pnGrantTokenDecoded);
            } 
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                pnInstance.PNLog.WriteToLog  (string.Format("Others Key Value {0}-{1}-{2}-{3}", kvp.Key.Type, kvp.Value.Type, key, kvp.Value), PNLoggingMethod.LevelError);                
            }
            #endif
        }

        public static string ReplaceBoundaryQuotes(string key){
            if(key.ElementAt(0).Equals('"') && key.ElementAt(key.Length-1).Equals('"')){
                key = key.Remove(key.Length-1, 1).Remove(0, 1);
            }
            return key;
        }

        public static void FillGrantToken(PubNubUnity pnInstance, string parent, string key, object val, Type type, ref TokenContents pnGrantTokenDecoded){            
            #if (ENABLE_PUBNUB_LOGGING)
            pnInstance.PNLog.WriteToLog (string.Format("FillGrantToken: {0}", key), PNLoggingMethod.LevelInfo);
            #endif
            key = ReplaceBoundaryQuotes(key);
            #if (ENABLE_PUBNUB_LOGGING)
            pnInstance.PNLog.WriteToLog (string.Format("FillGrantToken after: {0}", key), PNLoggingMethod.LevelInfo);
            #endif
            int i = 0;
            long l = 0;
            switch(type.Name){
                case "Int32":
                if (!int.TryParse (val.ToString(), out i)) {
                    //log
                }                
                break;
                case "Int64":
                if (!long.TryParse (val.ToString(), out l)) {
                    //log
                }                
                break;
                case "String":
                // do nothing 
                break;
                default:
                #if (ENABLE_PUBNUB_LOGGING)
                pnInstance.PNLog.WriteToLog (string.Format("typeName: {0}", type.Name), PNLoggingMethod.LevelInfo);
                #endif
                break;
            }
            switch(key){
                case "v":
                pnGrantTokenDecoded.Version = i;
                break;
                case "t":
                pnGrantTokenDecoded.Timestamp = i;
                break;
                case "ttl":
                pnGrantTokenDecoded.TTL = i;
                break;
                case "uuid":
                pnGrantTokenDecoded.AuthorizedUUID = ((CBORObject)val).AsString();
                break;
                case "sig":
                pnGrantTokenDecoded.Signature = val.ToString();
                break;
                default:
                switch(parent){
                    case "meta":
                    if ((pnGrantTokenDecoded.Meta != null) && !pnGrantTokenDecoded.Meta.ContainsKey(key))
                    {
                        switch (type.Name)
                        {
                            case "Int32":
                                pnGrantTokenDecoded.Meta.Add(key, i);
                                break;
                            case "Int64":
                                pnGrantTokenDecoded.Meta.Add(key, l);
                                break;
                            case "String":
                                pnGrantTokenDecoded.Meta.Add(key, ((CBORObject)val).AsString());
                                break;
                            default:
                                #if (ENABLE_PUBNUB_LOGGING)
                                pnInstance.PNLog.WriteToLog (string.Format("typeName: {0}", type.Name), PNLoggingMethod.LevelInfo);
                                #endif                            
                                break;
                        }
                    }
                    break;                    
                    case "res:chan": 
                    pnGrantTokenDecoded.Resources.Channels[key] = ParseGrantPrems(pnInstance, i);
                    break;
                    case "res:grp": 
                    pnGrantTokenDecoded.Resources.Groups[key] = ParseGrantPrems(pnInstance, i);
                    break;
                    case "res:uuid": 
                    pnGrantTokenDecoded.Resources.UUIDs[key] = ParseGrantPrems(pnInstance, i);
                    break;
                    case "pat:chan": 
                    pnGrantTokenDecoded.Patterns.Channels[key] = ParseGrantPrems(pnInstance, i);
                    break;
                    case "pat:grp": 
                    pnGrantTokenDecoded.Patterns.Groups[key] = ParseGrantPrems(pnInstance, i);
                    break;
                    case "pat:uuid": 
                    pnGrantTokenDecoded.Patterns.UUIDs[key] = ParseGrantPrems(pnInstance, i);
                    break;
                    default:
                    #if (ENABLE_PUBNUB_LOGGING)
                    pnInstance.PNLog.WriteToLog (string.Format("No match on parent: {0}", parent), PNLoggingMethod.LevelInfo);
                    #endif
                    break;
                }
                break;
            }
        }

        public static TokenAuthValues ParseGrantPrems(PubNubUnity pnInstance, int b){
            TokenAuthValues rp = new TokenAuthValues {
                Read = false,
                Write = false,
                Manage = false,
                Delete = false,
                Create = false,
                Get = false,
                Update = false,
                Join = false              
            };
            char[] charArray = Convert.ToString(b, 2).ToCharArray();
            Array.Reverse( charArray );
            string bits = new string( charArray );
            #if (ENABLE_PUBNUB_LOGGING)
            pnInstance.PNLog.WriteToLog ("binary==>"+bits, PNLoggingMethod.LevelInfo);
            #endif

            for (int i = 0; i < bits.Length; i++)
            {
                #if (ENABLE_PUBNUB_LOGGING)
                pnInstance.PNLog.WriteToLog (string.Format("{0}-{1}", i, bits[i]), PNLoggingMethod.LevelInfo);
                #endif
                switch(i) {
                    case 0:
                    rp.Read = (bits[i] == '1');
                    break;
                    case 1:
                    rp.Write = (bits[i] == '1');
                    break;
                    case 2:
                    rp.Manage = (bits[i] == '1');
                    break;
                    case 3:
                    rp.Delete = (bits[i] == '1');
                    break;
                    case 4:
                    rp.Create = (bits[i] == '1');
                    break;
                    case 5:
                    rp.Get = (bits[i] == '1');
                    break;
                    case 6:
                    rp.Update = (bits[i] == '1');
                    break;
                    case 7:
                    rp.Join = (bits[i] == '1');
                    break;
                    default:
                    #if (ENABLE_PUBNUB_LOGGING)
                    pnInstance.PNLog.WriteToLog (string.Format("No match on ParseGrantPrems: {0}", i), PNLoggingMethod.LevelInfo);
                    #endif

                    break;

                }
            }
            #if (ENABLE_PUBNUB_LOGGING)
            StringBuilder sbLog = PrintTokenPermissions(rp);
            pnInstance.PNLog.WriteToLog(sbLog.ToString(), PNLoggingMethod.LevelInfo);
            #endif
            return rp;

        }

        public static StringBuilder PrintTokenPermissions(TokenAuthValues tokenAuthValues){
            StringBuilder sbLog = new StringBuilder();
            sbLog.AppendLine();
            sbLog.AppendFormat("ResourcePermissions Read ==> {0}", tokenAuthValues.Read);
            sbLog.AppendLine();
            sbLog.AppendFormat("ResourcePermissions Write ==> {0}", tokenAuthValues.Write);
            sbLog.AppendLine();
            sbLog.AppendFormat("ResourcePermissions Manage ==> {0}", tokenAuthValues.Manage);
            sbLog.AppendLine();
            sbLog.AppendFormat("ResourcePermissions Delete ==> {0}", tokenAuthValues.Delete);
            sbLog.AppendLine();
            sbLog.AppendFormat("ResourcePermissions Create ==> {0}", tokenAuthValues.Create);
            sbLog.AppendLine();
            sbLog.AppendFormat("ResourcePermissions Get ==> {0}", tokenAuthValues.Get);
            sbLog.AppendLine();
            sbLog.AppendFormat("ResourcePermissions Update ==> {0}", tokenAuthValues.Update);
            sbLog.AppendLine();
            sbLog.AppendFormat("ResourcePermissions Join ==> {0}", tokenAuthValues.Join);
            return sbLog;

        }

        public static int PermissionsMapping(TokenAuthValues tokenAuthValues){
            int count=0;
            if(tokenAuthValues.Read){
                count += ((int)PNGrantBitMask.PNRead);
            }
            if(tokenAuthValues.Write){
                count += ((int)PNGrantBitMask.PNWrite);
            }
            if(tokenAuthValues.Manage){
                count += ((int)PNGrantBitMask.PNManage);
            }
            if(tokenAuthValues.Delete){
                count += ((int)PNGrantBitMask.PNDelete);
            }
            if(tokenAuthValues.Create){
                count += ((int)PNGrantBitMask.PNCreate);
            }
            if(tokenAuthValues.Get){
                count += ((int)PNGrantBitMask.PNGet);
            }
            if(tokenAuthValues.Update){
                count += ((int)PNGrantBitMask.PNUpdate);
            }
            if(tokenAuthValues.Join){
                count += ((int)PNGrantBitMask.PNJoin);
            }

            return count;
        }
    
    }
}
