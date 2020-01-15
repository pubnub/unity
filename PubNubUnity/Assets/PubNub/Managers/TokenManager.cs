using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PeterO.Cbor;
using System.IO;

namespace PubNubAPI
{       
    public enum PNGrantBitMask
    {
        PNRead = 1,
        PNWrite = 2,
        PNManage = 4,
        PNDelete = 8,
        PNCreate =16
    }

    public enum PNGrantType
    {
        PNReadEnabled = 0,
        PNWriteEnabled,
        PNManageEnabled,
        PNDeleteEnabled,
        PNCreateEnabled
    }

    public enum PNResourceType{
        PNChannels = 0,
        PNGroups,
        PNUsers,
        PNSpaces
    }

    public struct ChannelPermissions{
        bool Read;
        bool Write;
        bool Delete;
    }

    public struct GroupPermissions{
        bool Read;
        bool Manage;
    }

    public struct UserSpacePermissions{
        bool Read;
        bool Write;
        bool Manage;
        bool Delete;
        bool Create;
    }

    public struct ResourcePermissions{
        bool Read;
        bool Write;
        bool Manage;
        bool Delete;
        bool Create;
    }

    public class GrantResources{
        public Dictionary<string, int> Channels; // chan; 
        public Dictionary<string, int> Groups; //grp; 
        public Dictionary<string, int> Users; //usr;
        public Dictionary<string, int> Spaces; //spc
    }

    public class PNGrantTokenDecoded{
        public GrantResources Resources; //res
        public GrantResources Patterns; //pat;
        public Dictionary<string, object> Meta; //meta;
        public byte[] Signature; //sig;
        public int Version; //v;
        public long Timestamp; //t;
        public int TTL; //ttl
    }

    public class ChannelPermissionsWithToken{
	  public ChannelPermissions Permissions {get; set;}
      public Int64 BitMaskPerms {get; set;}
      public string Token {get; set;}
      public Int64 Timestamp {get; set;}
      public int TTL {get; set;}
    }

    public class GroupPermissionsWithToken{
	  public GroupPermissions Permissions {get; set;}
      public Int64 BitMaskPerms {get; set;}
      public string Token {get; set;}
      public Int64 Timestamp {get; set;}
      public int TTL {get; set;}

    }

    public class UserSpacePermissionsWithToken{
	  public UserSpacePermissions Permissions {get; set;}
      public Int64 BitMaskPerms {get; set;}
      public string Token {get; set;}
      public Int64 Timestamp {get; set;}
      public int TTL {get; set;}

    }

    public class GrantResourcesWithPermissions{
        public Dictionary<string, ChannelPermissionsWithToken> Channels { get; set; }
        public Dictionary<string, GroupPermissionsWithToken> Groups { get; set; }
        public Dictionary<string, UserSpacePermissionsWithToken> Users { get; set; }
        public Dictionary<string, UserSpacePermissionsWithToken> Spaces { get; set; }
        public Dictionary<string, ChannelPermissionsWithToken> ChannelsPattern { get; set; }
        public Dictionary<string, GroupPermissionsWithToken> GroupsPattern { get; set; }
        public Dictionary<string, UserSpacePermissionsWithToken> UsersPattern { get; set; }
        public Dictionary<string, UserSpacePermissionsWithToken> SpacesPattern { get; set; }
    }

    public class TokenManager
    {
        private GrantResourcesWithPermissions tokens;
        private PubNubUnity PubNubInstance { get; set;}
        public TokenManager(PubNubUnity pn){
            tokens = new GrantResourcesWithPermissions(){
                Channels = new Dictionary<string, ChannelPermissionsWithToken>(),
                Groups = new Dictionary<string, GroupPermissionsWithToken>(),
                Users = new Dictionary<string, UserSpacePermissionsWithToken>(),
                Spaces = new Dictionary<string, UserSpacePermissionsWithToken>(),
                ChannelsPattern = new Dictionary<string, ChannelPermissionsWithToken>(),
                GroupsPattern = new Dictionary<string, GroupPermissionsWithToken>(),
                UsersPattern = new Dictionary<string, UserSpacePermissionsWithToken>(),
                SpacesPattern = new Dictionary<string, UserSpacePermissionsWithToken>(),
            };
            PubNubInstance = pn;
        }

        public void CleanUp() {
            tokens = null; 
        }
        
        public string SetAuthParan(string resourceID, PNResourceType resourceType){
            string authParam = "auth";
            string token = GetToken(resourceID, resourceType);
            return string.Format("{0}={1}", authParam, token);

        }

        public GrantResourcesWithPermissions GetAllTokens(){
            return tokens;
        }
        
        public GrantResourcesWithPermissions GetTokensByResource(PNResourceType resourceType){
            GrantResourcesWithPermissions grantResourcesWithPermissions = new GrantResourcesWithPermissions(){
                Channels = new Dictionary<string, ChannelPermissionsWithToken>(),
                Groups = new Dictionary<string, GroupPermissionsWithToken>(),
                Users = new Dictionary<string, UserSpacePermissionsWithToken>(),
                Spaces = new Dictionary<string, UserSpacePermissionsWithToken>(),
                ChannelsPattern = new Dictionary<string, ChannelPermissionsWithToken>(),
                GroupsPattern = new Dictionary<string, GroupPermissionsWithToken>(),
                UsersPattern = new Dictionary<string, UserSpacePermissionsWithToken>(),
                SpacesPattern = new Dictionary<string, UserSpacePermissionsWithToken>(),
            };

            switch(resourceType) {
                case PNResourceType.PNChannels:
                grantResourcesWithPermissions.Channels = tokens.Channels;
                grantResourcesWithPermissions.ChannelsPattern = tokens.ChannelsPattern;
                break;
                case PNResourceType.PNGroups:
                grantResourcesWithPermissions.Groups = tokens.Groups;
                grantResourcesWithPermissions.GroupsPattern = tokens.GroupsPattern;
                break;
                case PNResourceType.PNSpaces:
                grantResourcesWithPermissions.Spaces = tokens.Spaces;
                grantResourcesWithPermissions.SpacesPattern = tokens.SpacesPattern;
                break;
                case PNResourceType.PNUsers:
                grantResourcesWithPermissions.Users = tokens.Users;
                grantResourcesWithPermissions.UsersPattern = tokens.UsersPattern;
                break;
            }

            return grantResourcesWithPermissions;

        }

        // GetToken first match for direct ids, if no match found use the first token from pattern match ignoring the regex (by design).
        public string GetToken(string resourceID, PNResourceType resourceType){
            switch(resourceType) {
                case PNResourceType.PNChannels:
                ChannelPermissionsWithToken channelPermissionsWithToken;
                if(tokens.Channels.TryGetValue(resourceID, out channelPermissionsWithToken)){
                    return channelPermissionsWithToken.Token;
                }
                if ((tokens.ChannelsPattern != null) && (tokens.ChannelsPattern.Count > 0)){
                    return tokens.ChannelsPattern.First().Value.Token;
                }
                return "";
                case PNResourceType.PNGroups:
                GroupPermissionsWithToken groupPermissionsWithToken;
                if(tokens.Groups.TryGetValue(resourceID, out groupPermissionsWithToken)){
                    return groupPermissionsWithToken.Token;
                }
                if ((tokens.GroupsPattern != null) && (tokens.GroupsPattern.Count > 0)){
                    return tokens.GroupsPattern.First().Value.Token;
                }
                return "";
                case PNResourceType.PNSpaces:
                UserSpacePermissionsWithToken spacePermissionsWithToken;
                if(tokens.Spaces.TryGetValue(resourceID, out spacePermissionsWithToken)){
                    return spacePermissionsWithToken.Token;
                }
                if ((tokens.SpacesPattern != null) && (tokens.SpacesPattern.Count > 0)){
                    return tokens.SpacesPattern.First().Value.Token;
                }
                return "";
                case PNResourceType.PNUsers:
                UserSpacePermissionsWithToken userPermissionsWithToken;
                if(tokens.Users.TryGetValue(resourceID, out userPermissionsWithToken)){
                    return userPermissionsWithToken.Token;
                }
                if ((tokens.UsersPattern != null) && (tokens.UsersPattern.Count > 0)){
                    return tokens.UsersPattern.First().Value.Token;
                }
                return "";
            }
            return "";
        }

        //mergeTokensByResource
        public void StoreTokens(List<string> tokens){  
            foreach (string token in tokens){
                StoreToken(token);
            }
        }

        public void StoreToken(string token){  
            if ((PubNubInstance.PNConfig.StoreTokensOnGrant) && (PubNubInstance.PNConfig.SecretKey == "")) {
                PNGrantTokenDecoded cborObject = GetPermissions(token);
            }
        }

        public PNGrantTokenDecoded GetPermissions(string token){
            token = token.Replace("-", "+").Replace("_", "/");
            int i = token.Length % 4;
            if (i != 0) {
                token += new String('=', 4-i);
            }
            Debug.Log(token);
            PNGrantTokenDecoded pnGrantTokenDecoded = new PNGrantTokenDecoded();
            pnGrantTokenDecoded.Patterns = new GrantResources(){
                Channels = new Dictionary<string, int>(),
                Groups = new Dictionary<string, int>(),
                Users = new Dictionary<string, int>(),
                Spaces = new Dictionary<string, int>()
            };
            pnGrantTokenDecoded.Resources = new GrantResources(){
                Channels = new Dictionary<string, int>(),
                Groups = new Dictionary<string, int>(),
                Users = new Dictionary<string, int>(),
                Spaces = new Dictionary<string, int>()
            };
            pnGrantTokenDecoded.Meta = new Dictionary<string, object>();

            byte[] decryptedBytes = Convert.FromBase64CharArray (token.ToCharArray (), 0, token.Length);
            //using (var stream = new MemoryStream(decryptedBytes)) {
                // Read the CBOR object from the stream
                //var cbor = CBORObject.Read(stream);
                var cbor = CBORObject.DecodeFromBytes(decryptedBytes);

                //Debug.Log(cbor.GetAllTags().ToString());
                // foreach (CBORObject obj in cbor.Values){
                //     Debug.Log(obj.ToString());
                // }
                Debug.Log(cbor.ToJSONString());
                //foreach (CBORObject obj in cbor.Values){
                ParseCBOR(cbor, "", ref pnGrantTokenDecoded); 
                
                // var d = cbor.ToObject<Dictionary<string, object>>();
                // foreach(KeyValuePair<string, object> kvp in d){
                //     Debug.Log(kvp.Key.ToString());
                //     Debug.Log(kvp.Value.ToString());
                // }
                
                //Debug.Log(s);
                // Debug.Log(cborObject.ttl);
                // Debug.Log(cborObject.sig);
                // Debug.Log(cborObject.v);
                // Debug.Log(cborObject.res.spc["s-1707983"].ToString());
                //cborObject = cbor.ToObject<PNGrantTokenDecoded>();
                
                return pnGrantTokenDecoded;
            //}
        }

        public void ParseCBOR(CBORObject cbor, string parent, ref PNGrantTokenDecoded pnGrantTokenDecoded){
            foreach (KeyValuePair<CBORObject, CBORObject> kvp in cbor.Entries){
                if(kvp.Key.Type.ToString().Equals("ByteString")){
                    //Debug.Log(string.Format("Key {0}-{1}", System.Text.Encoding.ASCII.GetString(kvp.Key.GetByteString()), kvp.Value));
                    string key = System.Text.Encoding.ASCII.GetString(kvp.Key.GetByteString());
                    ParseCBORValue(key, parent, kvp, ref pnGrantTokenDecoded);
                } else if(kvp.Key.Type.ToString().Equals("TextString")) {
                    Debug.Log(string.Format("TextString Key {0}-{1}-{2}", kvp.Key.ToString(), kvp.Value.ToString(), kvp.Value.Type));
                    ParseCBORValue(kvp.Key.ToString(), parent, kvp, ref pnGrantTokenDecoded);
                    //FillGrantToken(parent, kvp.Key.ToString(), kvp.Value, typeof(string), ref pnGrantTokenDecoded);
                } else {
                    Debug.Log(string.Format("Others Key {0}-{1}-{2}-{3}", kvp.Key, kvp.Key.Type, kvp.Value, kvp.Value.Type));
                }
                
                //byte[] dataV = FromHex(obj.Value.ToString());    
                //byte[] dataK = FromHex(obj.Key.ToString());    
                //Debug.Log(string.Format("{0}-{1}", obj.Key.ToString(), obj.Value));
                //Debug.Log(string.Format("{0}-{1}", "dataK", System.Text.Encoding.ASCII.GetString(dataV)));
            }
        }

        public void ParseCBORValue(string key, string parent, KeyValuePair<CBORObject, CBORObject> kvp, ref PNGrantTokenDecoded pnGrantTokenDecoded){
            if(kvp.Value.Type.ToString().Equals("Map")){
                Debug.Log(string.Format("Map Key {0}", key));
                var p = string.Format("{0}{1}{2}", parent, string.IsNullOrEmpty(parent)?"":":", key);
                ParseCBOR(kvp.Value, p, ref pnGrantTokenDecoded);
            // } else if(kvp.Key.Type.ToString().Equals("TextString")){
            //     Debug.Log(string.Format("TextString Key1 {0}-{1}", key, kvp.Value.ToString()));
            //     FillGrantToken(parent, key, kvp.Value, typeof(string), ref pnGrantTokenDecoded);
            } else if(kvp.Value.Type.ToString().Equals("ByteString")){ 
                string val = System.Text.Encoding.ASCII.GetString(kvp.Value.GetByteString());
                Debug.Log(string.Format("ByteString Value {0}-{1}", key, val));
                FillGrantToken(parent, key, kvp.Value, typeof(string), ref pnGrantTokenDecoded);
            } else if(kvp.Value.Type.ToString().Equals("Integer")){                            
                Debug.Log(string.Format("Integer Value {0}-{1}", key, kvp.Value));
                FillGrantToken(parent, key, kvp.Value, typeof(int), ref pnGrantTokenDecoded);
            } else {
                Debug.Log(string.Format("Others Key Value {0}-{1}-{2}-{3}", kvp.Key.Type, kvp.Value.Type, key, kvp.Value));
            }
        }

        public void FillGrantToken(string parent, string key, object val, Type type, ref PNGrantTokenDecoded pnGrantTokenDecoded){            
            Debug.Log("parent-p:" + parent);
            int i = 0;
            long l = 0;
            string s = "";
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
                s = val.ToString();
                break;
                default:
                Debug.Log("typeName:" + type.Name);
                break;
            }
            switch(key){
                case "v":
                pnGrantTokenDecoded.Version = i;
                break;
                case "t":
                pnGrantTokenDecoded.Timestamp = l;
                break;
                case "ttl":
                pnGrantTokenDecoded.TTL = i;
                break;
                case "meta":
                pnGrantTokenDecoded.Meta = val as Dictionary<string, object>;
                break;
                case "sig":
                pnGrantTokenDecoded.Signature = System.Text.Encoding.ASCII.GetBytes(val.ToString());
                break;
                default:
                switch(parent){
                    case "res:spc": 
                    pnGrantTokenDecoded.Resources.Spaces[key] = i;
                    break;
                    case "res:usr": 
                    pnGrantTokenDecoded.Resources.Users[key] = i;
                    break;
                    case "res:chan": 
                    pnGrantTokenDecoded.Resources.Channels[key] = i;
                    break;
                    case "res:grp": 
                    pnGrantTokenDecoded.Resources.Groups[key] = i;
                    break;
                    case "pat:spc": 
                    pnGrantTokenDecoded.Patterns.Spaces[key] = i;
                    break;
                    case "pat:usr": 
                    pnGrantTokenDecoded.Patterns.Users[key] = i;
                    break;
                    case "pat:chan": 
                    pnGrantTokenDecoded.Patterns.Channels[key] = i;
                    break;
                    case "pat:grp": 
                    pnGrantTokenDecoded.Patterns.Groups[key] = i;
                    break;
                    default:
                    Debug.Log("No match parent: " + parent);
                    break;
                }
                break;
            }
        }


        // public static byte[] FromHex(string hex)
        // {
        //     hex = hex.Replace("-", "");
        //     byte[] raw = new byte[hex.Length / 2];
        //     for (int i = 0; i < raw.Length; i++)
        //     {
        //         raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        //     }
        //     return raw;
        // }
    }
}