using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PeterO.Cbor;
using System.Collections;

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

    public class ChannelPermissions : ResourcePermission{
        //public bool Read;
        public bool Write;
        public bool Delete;
    }

    public class GroupPermissions : ResourcePermission{
        //public bool Read;
        public bool Manage;
    }

    public class UserSpacePermissions : ResourcePermission{
        //public bool Read;
        public bool Write;
        public bool Manage;
        public bool Delete;
        public bool Create;
    }

    public class ResourcePermission{
        public bool Read;
        // public bool Write;
        // public bool Manage;
        // public bool Delete;
        // public bool Create;
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

    public class ChannelPermissionsWithToken : ResourcePermissionsWithTokenBase{
	  public ChannelPermissions Permissions {get; set;}

    }

    public class GroupPermissionsWithToken: ResourcePermissionsWithTokenBase{
	  public GroupPermissions Permissions {get; set;}
    }

    public class UserSpacePermissionsWithToken: ResourcePermissionsWithTokenBase{
	  public UserSpacePermissions Permissions {get; set;}
    }

    public class ResourcePermissionsWithTokenBase{
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
        private GrantResourcesWithPermissions Tokens;
        private PubNubUnity PubNubInstance { get; set;}
        public TokenManager(PubNubUnity pn){
            Tokens = InitGrantResourcesWithPermissions();
            PubNubInstance = pn;
        }

        public void CleanUp() {
            Tokens = null; 
        }
        
        public string SetAuthParan(string resourceID, PNResourceType resourceType){
            string authParam = "auth";
            string token = GetToken(resourceID, resourceType);
            return string.Format("{0}={1}", authParam, token);

        }

        public GrantResourcesWithPermissions GetAllTokens(){
            return Tokens;
        }

        public GrantResourcesWithPermissions InitGrantResourcesWithPermissions(){
            return new GrantResourcesWithPermissions(){
                Channels = new Dictionary<string, ChannelPermissionsWithToken>(),
                Groups = new Dictionary<string, GroupPermissionsWithToken>(),
                Users = new Dictionary<string, UserSpacePermissionsWithToken>(),
                Spaces = new Dictionary<string, UserSpacePermissionsWithToken>(),
                ChannelsPattern = new Dictionary<string, ChannelPermissionsWithToken>(),
                GroupsPattern = new Dictionary<string, GroupPermissionsWithToken>(),
                UsersPattern = new Dictionary<string, UserSpacePermissionsWithToken>(),
                SpacesPattern = new Dictionary<string, UserSpacePermissionsWithToken>(),
            };
        }
        
        public GrantResourcesWithPermissions GetTokensByResource(PNResourceType resourceType){
            GrantResourcesWithPermissions grantResourcesWithPermissions = InitGrantResourcesWithPermissions();

            switch(resourceType) {
                case PNResourceType.PNChannels:
                grantResourcesWithPermissions.Channels = Tokens.Channels;
                grantResourcesWithPermissions.ChannelsPattern = Tokens.ChannelsPattern;
                break;
                case PNResourceType.PNGroups:
                grantResourcesWithPermissions.Groups = Tokens.Groups;
                grantResourcesWithPermissions.GroupsPattern = Tokens.GroupsPattern;
                break;
                case PNResourceType.PNSpaces:
                grantResourcesWithPermissions.Spaces = Tokens.Spaces;
                grantResourcesWithPermissions.SpacesPattern = Tokens.SpacesPattern;
                break;
                case PNResourceType.PNUsers:
                grantResourcesWithPermissions.Users = Tokens.Users;
                grantResourcesWithPermissions.UsersPattern = Tokens.UsersPattern;
                break;
            }

            return grantResourcesWithPermissions;

        }

        // GetToken first match for direct ids, if no match found use the first token from pattern match ignoring the regex (by design).
        public string GetToken(string resourceID, PNResourceType resourceType){
            switch(resourceType) {
                case PNResourceType.PNChannels:
                ChannelPermissionsWithToken channelPermissionsWithToken;
                if(Tokens.Channels.TryGetValue(resourceID, out channelPermissionsWithToken)){
                    return channelPermissionsWithToken.Token;
                }
                if ((Tokens.ChannelsPattern != null) && (Tokens.ChannelsPattern.Count > 0)){
                    return Tokens.ChannelsPattern.First().Value.Token;
                }
                return "";
                case PNResourceType.PNGroups:
                GroupPermissionsWithToken groupPermissionsWithToken;
                if(Tokens.Groups.TryGetValue(resourceID, out groupPermissionsWithToken)){
                    return groupPermissionsWithToken.Token;
                }
                if ((Tokens.GroupsPattern != null) && (Tokens.GroupsPattern.Count > 0)){
                    return Tokens.GroupsPattern.First().Value.Token;
                }
                return "";
                case PNResourceType.PNSpaces:
                UserSpacePermissionsWithToken spacePermissionsWithToken;
                if(Tokens.Spaces.TryGetValue(resourceID, out spacePermissionsWithToken)){
                    return spacePermissionsWithToken.Token;
                }
                if ((Tokens.SpacesPattern != null) && (Tokens.SpacesPattern.Count > 0)){
                    return Tokens.SpacesPattern.First().Value.Token;
                }
                return "";
                case PNResourceType.PNUsers:
                UserSpacePermissionsWithToken userPermissionsWithToken;
                if(Tokens.Users.TryGetValue(resourceID, out userPermissionsWithToken)){
                    return userPermissionsWithToken.Token;
                }
                if ((Tokens.UsersPattern != null) && (Tokens.UsersPattern.Count > 0)){
                    return Tokens.UsersPattern.First().Value.Token;
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
                try
                {
                    PNGrantTokenDecoded pnGrantTokenDecoded = GetPermissions(token);
                    ParseGrantResources(pnGrantTokenDecoded.Resources, token, pnGrantTokenDecoded.Timestamp, pnGrantTokenDecoded.TTL, false);
                    //clear all Users/Spaces pattern maps (by design, store last token only for patterns)
                    Tokens.SpacesPattern = new Dictionary<string, UserSpacePermissionsWithToken>();
                    Tokens.UsersPattern  = new Dictionary<string, UserSpacePermissionsWithToken>();
                    ParseGrantResources(pnGrantTokenDecoded.Patterns, token, pnGrantTokenDecoded.Timestamp, pnGrantTokenDecoded.TTL, true);
                } catch (Exception ex) {
                    Debug.Log(ex.ToString()); 
                }
            }
        }

        public ResourcePermission ParseGrantPrems(int b, PNResourceType pnResourceType){
            UserSpacePermissions rp = new UserSpacePermissions(){
                Read = false,
                Write = false,
                Manage = false,
                Delete = false,
                Create = false,                
            };
            string bits = Convert.ToString(b, 2);
            Debug.Log("binary==>"+bits);

            for (int i = 0; i < bits.Length; i++)
            {
                Debug.Log(string.Format("{0}-{1}", i, bits[i]));
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
                }
            }
            Debug.Log("ResourcePermissions Read ==> "  + rp.Read);
            Debug.Log("ResourcePermissions Write ==> "  + rp.Write);
            Debug.Log("ResourcePermissions Manage ==> "  + rp.Manage);
            Debug.Log("ResourcePermissions Delete ==> "  + rp.Delete);
            Debug.Log("ResourcePermissions Create ==> "  + rp.Create);
            
            
            return rp;

        }

        public void FillGrantResourcesWithPermissions(Dictionary<string, int> resDict, string token, long timetoken, int ttl, bool isPattern, PNResourceType pnResourceType){
            if((resDict != null) && (resDict.Count > 0)){
                foreach(KeyValuePair<string, int> kvp in resDict){
                    switch(pnResourceType){
                        case PNResourceType.PNChannels:
                        ChannelPermissionsWithToken channelPermissionsWithToken = new ChannelPermissionsWithToken(){
                                BitMaskPerms = kvp.Value,
                                Token = token,
                                Timestamp = timetoken,
                                TTL = ttl,
                                Permissions = ParseGrantPrems(kvp.Value, pnResourceType) as ChannelPermissions,
                            };
                        if(isPattern){
                            Tokens.ChannelsPattern[kvp.Key] = channelPermissionsWithToken; 
                        } else {
                            Tokens.Channels[kvp.Key] = channelPermissionsWithToken;
                        }

                        break;
                        case PNResourceType.PNGroups:
                        GroupPermissionsWithToken groupPermissionsWithToken = new GroupPermissionsWithToken(){
                                BitMaskPerms = kvp.Value,
                                Token = token,
                                Timestamp = timetoken,
                                TTL = ttl,
                                Permissions = ParseGrantPrems(kvp.Value, pnResourceType) as GroupPermissions,
                            };
                        if(isPattern){
                            Tokens.GroupsPattern[kvp.Key] = groupPermissionsWithToken; 
                        } else {
                            Tokens.Groups[kvp.Key] = groupPermissionsWithToken;
                        }                    
                        break;
                        case PNResourceType.PNSpaces:
                        UserSpacePermissionsWithToken spacePermissionsWithToken = new UserSpacePermissionsWithToken(){
                                BitMaskPerms = kvp.Value,
                                Token = token,
                                Timestamp = timetoken,
                                TTL = ttl,
                                Permissions = ParseGrantPrems(kvp.Value, pnResourceType) as UserSpacePermissions,
                            };
                        if(isPattern){
                            Tokens.SpacesPattern[kvp.Key] = spacePermissionsWithToken; 
                        } else {
                            Tokens.Spaces[kvp.Key] = spacePermissionsWithToken;
                        }                    
                        break;
                        case PNResourceType.PNUsers:
                        UserSpacePermissionsWithToken userPermissionsWithToken = new UserSpacePermissionsWithToken(){
                                BitMaskPerms = kvp.Value,
                                Token = token,
                                Timestamp = timetoken,
                                TTL = ttl,
                                Permissions = ParseGrantPrems(kvp.Value, pnResourceType) as UserSpacePermissions,
                            };
                        if(isPattern){
                            Tokens.UsersPattern[kvp.Key] = userPermissionsWithToken; 
                        } else {
                            Tokens.Users[kvp.Key] = userPermissionsWithToken;
                        }                    
                        break;
                    }
                }
            }
        }

        public void ParseGrantResources(GrantResources res, string token, long timetoken, int ttl, bool isPattern){
            FillGrantResourcesWithPermissions(res.Channels, token, timetoken, ttl, isPattern, PNResourceType.PNChannels);
            FillGrantResourcesWithPermissions(res.Groups, token, timetoken, ttl, isPattern, PNResourceType.PNGroups);
            FillGrantResourcesWithPermissions(res.Users, token, timetoken, ttl, isPattern, PNResourceType.PNUsers);
            FillGrantResourcesWithPermissions(res.Spaces, token, timetoken, ttl, isPattern, PNResourceType.PNSpaces);
            // GrantResourcesWithPermissions g = InitGrantResourcesWithPermissions();
            
            // Debug.Log("ParseGrantResources");
            
            // //if(!isPattern){                
                
            //     foreach(KeyValuePair<string, int> kvp in res.Channels){
            //         Debug.Log("ParseGrantResources Channels");
            //         //ChannelPermissions rp = ParseGrantPrems(kvp.Value, PNResourceType.PNChannels) as ChannelPermissions;
            //         // ChannelPermissions cp = new ChannelPermissions(){
            //         //          Read = rp.Read,
            //         //          Write = rp.Write,
            //         //          Delete = rp.Delete, 
            //         //      };
            //         g.Channels[kvp.Key] = new ChannelPermissionsWithToken(){
            //                 BitMaskPerms = kvp.Value,
            //                 Token = token,
            //                 Timestamp = timetoken,
            //                 TTL = ttl,
            //                 Permissions = ParseGrantPrems(kvp.Value, PNResourceType.PNChannels) as ChannelPermissions,
            //             };
            //         Debug.Log(g.Channels[kvp.Key].Permissions.Delete);    
            //         Debug.Log(g.Channels[kvp.Key].Permissions.Read);    
            //         Debug.Log(g.Channels[kvp.Key].Permissions.Write);    
            //     }

            //     g.Users = new Dictionary<string, UserSpacePermissionsWithToken>();
            //     foreach(KeyValuePair<string, int> kvp in res.Users){
            //         Debug.Log("ParseGrantResources Users");
            //         //ResourcePermissionsBase rp = ParseGrantPrems(kvp.Value, PNResourceType.PNUsers) as ResourcePermissions;
            //         // UserSpacePermissions cp = new UserSpacePermissions(){
            //         //          Read = rp.Read,
            //         //          Write = rp.Write,
            //         //          Manage = rp.Manage,
            //         //          Delete = rp.Delete,
            //         //          Create = rp.Create,
            //         //      };
            //         g.Users[kvp.Key] = new UserSpacePermissionsWithToken(){
            //                 BitMaskPerms = kvp.Value,
            //                 Token = token,
            //                 Timestamp = timetoken,
            //                 TTL = ttl,
            //                 Permissions = ParseGrantPrems(kvp.Value, PNResourceType.PNUsers) as UserSpacePermissions,
            //             };
            //         Debug.Log(g.Users[kvp.Key].Permissions.Create);    
            //         Debug.Log(g.Users[kvp.Key].Permissions.Read);    
            //         Debug.Log(g.Users[kvp.Key].Permissions.Write);    
            //         Debug.Log(g.Users[kvp.Key].Permissions.Manage);    
            //         Debug.Log(g.Users[kvp.Key].Permissions.Delete);    

            //     }
            //     return g;
            //}
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
                //Debug.Log(cbor.ToJSONString());
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
                pnGrantTokenDecoded.Timestamp = i;
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
                    Debug.Log("No match on parent: " + parent);
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