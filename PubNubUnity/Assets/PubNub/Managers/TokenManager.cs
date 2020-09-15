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
        PNUUIDMetadata,
        PNChannelMetadata
    }

    public class ChannelPermissions : ResourcePermission{
        public bool Write {get; set;}
        public bool Delete {get; set;}
    }

    public class GroupPermissions : ResourcePermission{
        public bool Manage {get; set;}
    }

    public class UserSpacePermissions : ResourcePermission{
        public bool Write {get; set;}
        public bool Manage {get; set;}
        public bool Delete {get; set;}
        public bool Create {get; set;}
    }

    public class ResourcePermission{
        public bool Read {get; set;}
    }

    public class GrantResources{
        //chan
        public Dictionary<string, int> Channels {get; set;}
        //grp
        public Dictionary<string, int> Groups {get; set;}
        //usr
        public Dictionary<string, int> Users {get; set;}
        //spc
        public Dictionary<string, int> Spaces {get; set;}
    }

    public class PNGrantTokenDecoded{
        //res
        public GrantResources Resources {get; set;}
        //pat
        public GrantResources Patterns {get; set;}
        //meta
        public Dictionary<string, object> Meta {get; set;}
        //sig
        public byte[] Signature {get; set;}
        //v
        public int Version {get; set;}
        //t
        public long Timestamp {get; set;}
        //ttl
        public int TTL {get; set;} 
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
        public SafeDictionary<string, ChannelPermissionsWithToken> Channels { get; set; }
        public SafeDictionary<string, GroupPermissionsWithToken> Groups { get; set; }
        public SafeDictionary<string, UserSpacePermissionsWithToken> Users { get; set; }
        public SafeDictionary<string, UserSpacePermissionsWithToken> Spaces { get; set; }
        public SafeDictionary<string, ChannelPermissionsWithToken> ChannelsPattern { get; set; }
        public SafeDictionary<string, GroupPermissionsWithToken> GroupsPattern { get; set; }
        public SafeDictionary<string, UserSpacePermissionsWithToken> UsersPattern { get; set; }
        public SafeDictionary<string, UserSpacePermissionsWithToken> SpacesPattern { get; set; }
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
        
        internal Uri AppendTokenToURL(string request, string resourceID, PNResourceType resourceType, PNOperationType type){
            string token = GetToken(resourceID, resourceType);
            StringBuilder uriBuilder = new StringBuilder(request);
            if(!string.IsNullOrEmpty(token)){                
                uriBuilder.AppendFormat ("&auth={0}", Utility.EncodeUricomponent (token, type, false, false));
            } else {
                if (!string.IsNullOrEmpty (this.PubNubInstance.PNConfig.AuthKey)) {
                    uriBuilder.AppendFormat ("&auth={0}", Utility.EncodeUricomponent (this.PubNubInstance.PNConfig.AuthKey, type, false, false));
                }
            }
            
            return new Uri (uriBuilder.ToString ());
        }

        public GrantResourcesWithPermissions GetAllTokens(){
            #if (ENABLE_PUBNUB_LOGGING)
            foreach(KeyValuePair<string, ChannelPermissionsWithToken> kvp in Tokens.Channels){
                this.PubNubInstance.PNLog.WriteToLog (string.Format("Channels: key {0}, val {1}", kvp.Key, kvp.Value), PNLoggingMethod.LevelInfo);
            }
            foreach(KeyValuePair<string, GroupPermissionsWithToken> kvp in Tokens.Groups){
                this.PubNubInstance.PNLog.WriteToLog  (string.Format("Groups: key {0}, val {1}", kvp.Key, kvp.Value), PNLoggingMethod.LevelInfo);
            }
            foreach(KeyValuePair<string, UserSpacePermissionsWithToken> kvp in Tokens.Spaces){
                this.PubNubInstance.PNLog.WriteToLog  (string.Format("Spaces: key {0}, val {1}", kvp.Key, kvp.Value), PNLoggingMethod.LevelInfo);
            }
            foreach(KeyValuePair<string, UserSpacePermissionsWithToken> kvp in Tokens.Users){
                this.PubNubInstance.PNLog.WriteToLog  (string.Format("Users: key {0}, val {1}", kvp.Key, kvp.Value), PNLoggingMethod.LevelInfo);
            }
            foreach(KeyValuePair<string, ChannelPermissionsWithToken> kvp in Tokens.ChannelsPattern){
                this.PubNubInstance.PNLog.WriteToLog  (string.Format("ChannelsPattern: key {0}, val {1}", kvp.Key, kvp.Value), PNLoggingMethod.LevelInfo);
            }
            foreach(KeyValuePair<string, GroupPermissionsWithToken> kvp in Tokens.GroupsPattern){
                this.PubNubInstance.PNLog.WriteToLog  (string.Format("GroupsPattern: key {0}, val {1}", kvp.Key, kvp.Value), PNLoggingMethod.LevelInfo);
            }
            foreach(KeyValuePair<string, UserSpacePermissionsWithToken> kvp in Tokens.SpacesPattern){
                this.PubNubInstance.PNLog.WriteToLog  (string.Format("SpacesPattern: key {0}, val {1}", kvp.Key, kvp.Value), PNLoggingMethod.LevelInfo);
            }
            foreach(KeyValuePair<string, UserSpacePermissionsWithToken> kvp in Tokens.UsersPattern){
                this.PubNubInstance.PNLog.WriteToLog  (string.Format("UsersPattern: key {0}, val {1}", kvp.Key, kvp.Value), PNLoggingMethod.LevelInfo);
            }
            #endif
  
            return Tokens;
        }

        public GrantResourcesWithPermissions InitGrantResourcesWithPermissions(){
            return new GrantResourcesWithPermissions {
                Channels = new SafeDictionary<string, ChannelPermissionsWithToken>(),
                Groups = new SafeDictionary<string, GroupPermissionsWithToken>(),
                Users = new SafeDictionary<string, UserSpacePermissionsWithToken>(),
                Spaces = new SafeDictionary<string, UserSpacePermissionsWithToken>(),
                ChannelsPattern = new SafeDictionary<string, ChannelPermissionsWithToken>(),
                GroupsPattern = new SafeDictionary<string, GroupPermissionsWithToken>(),
                UsersPattern = new SafeDictionary<string, UserSpacePermissionsWithToken>(),
                SpacesPattern = new SafeDictionary<string, UserSpacePermissionsWithToken>(),
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
                case PNResourceType.PNChannelMetadata:
                grantResourcesWithPermissions.Spaces = Tokens.Spaces;
                grantResourcesWithPermissions.SpacesPattern = Tokens.SpacesPattern;
                break;
                case PNResourceType.PNUUIDMetadata:
                grantResourcesWithPermissions.Users = Tokens.Users;
                grantResourcesWithPermissions.UsersPattern = Tokens.UsersPattern;
                break;
                default:
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format("No match on GetTokensByResource: {0}", resourceType), PNLoggingMethod.LevelInfo);
                #endif

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
                case PNResourceType.PNChannelMetadata:
                UserSpacePermissionsWithToken spacePermissionsWithToken;
                if(Tokens.Spaces.TryGetValue(resourceID, out spacePermissionsWithToken)){                    
                    return spacePermissionsWithToken.Token;
                }
                if ((Tokens.SpacesPattern != null) && (Tokens.SpacesPattern.Count > 0)){
                    return Tokens.SpacesPattern.First().Value.Token;
                }
                return "";
                case PNResourceType.PNUUIDMetadata:
                UserSpacePermissionsWithToken userPermissionsWithToken;
                if(Tokens.Users.TryGetValue(resourceID, out userPermissionsWithToken)){
                    return userPermissionsWithToken.Token;
                }
                if ((Tokens.UsersPattern != null) && (Tokens.UsersPattern.Count > 0)){
                    return Tokens.UsersPattern.First().Value.Token;
                }
                return "";
                default:
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format("No match on GetToken: {0}", resourceType), PNLoggingMethod.LevelInfo);
                #endif

                break;

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
            if (PubNubInstance.PNConfig.StoreTokensOnGrant) {
                try
                {
                    PNGrantTokenDecoded pnGrantTokenDecoded = GetPermissions(token);
                    ParseGrantResources(pnGrantTokenDecoded.Resources, token, pnGrantTokenDecoded.Timestamp, pnGrantTokenDecoded.TTL, false);
                    //clear all Users/Spaces pattern maps (by design, store last token only for patterns)
                    Tokens.SpacesPattern = new SafeDictionary<string, UserSpacePermissionsWithToken>();
                    Tokens.UsersPattern  = new SafeDictionary<string, UserSpacePermissionsWithToken>();
                    ParseGrantResources(pnGrantTokenDecoded.Patterns, token, pnGrantTokenDecoded.Timestamp, pnGrantTokenDecoded.TTL, true);
                } catch (Exception ex) {
                    //Logging the exception when the debug symbol is set.
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog (ex.ToString(), PNLoggingMethod.LevelError); 
                    #endif
                }
            }
        }

        public ResourcePermission ParseGrantPrems(int b, PNResourceType pnResourceType){
            UserSpacePermissions rp = new UserSpacePermissions {
                Read = false,
                Write = false,
                Manage = false,
                Delete = false,
                Create = false,                
            };
            string bits = Convert.ToString(b, 2);
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog ("binary==>"+bits, PNLoggingMethod.LevelInfo);
            #endif

            for (int i = 0; i < bits.Length; i++)
            {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format("{0}-{1}", i, bits[i]), PNLoggingMethod.LevelInfo);
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
                    default:
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog (string.Format("No match on ParseGrantPrems: {0}", i), PNLoggingMethod.LevelInfo);
                    #endif

                    break;

                }
            }
            #if (ENABLE_PUBNUB_LOGGING)
            StringBuilder sbLog = new StringBuilder();
            sbLog.AppendFormat("ResourcePermissions Read ==> {0}", rp.Read);
            sbLog.AppendFormat("ResourcePermissions Write ==> {0}", rp.Write);
            sbLog.AppendFormat("ResourcePermissions Manage ==> {0}", rp.Manage);
            sbLog.AppendFormat("ResourcePermissions Delete ==> {0}", rp.Delete);
            sbLog.AppendFormat("ResourcePermissions Create ==> {0}", rp.Create);
            this.PubNubInstance.PNLog.WriteToLog(sbLog.ToString(), PNLoggingMethod.LevelInfo);
            #endif
            return rp;

        }

        public void FillGrantResourcesWithPermissions(Dictionary<string, int> resDict, string token, long timetoken, int ttl, bool isPattern, PNResourceType pnResourceType){
            if((resDict != null) && (resDict.Count > 0)){
                foreach(KeyValuePair<string, int> kvp in resDict){
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog (string.Format("FillGrantResourcesWithPermissions ==> {0}", kvp.Key), PNLoggingMethod.LevelInfo);
                    #endif

                    switch(pnResourceType){
                        case PNResourceType.PNChannels:
                        ChannelPermissionsWithToken channelPermissionsWithToken = new ChannelPermissionsWithToken {
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
                        GroupPermissionsWithToken groupPermissionsWithToken = new GroupPermissionsWithToken {
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
                        case PNResourceType.PNChannelMetadata:
                        UserSpacePermissionsWithToken spacePermissionsWithToken = new UserSpacePermissionsWithToken {
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
                        case PNResourceType.PNUUIDMetadata:
                        UserSpacePermissionsWithToken userPermissionsWithToken = new UserSpacePermissionsWithToken {
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
                        default:
                        #if (ENABLE_PUBNUB_LOGGING)
                        this.PubNubInstance.PNLog.WriteToLog (string.Format("No match on FillGrantResourcesWithPermissions: {0}", pnResourceType), PNLoggingMethod.LevelInfo);
                        #endif

                        break;

                    }
                }
            }
        }

        public void ParseGrantResources(GrantResources res, string token, long timetoken, int ttl, bool isPattern){
            FillGrantResourcesWithPermissions(res.Channels, token, timetoken, ttl, isPattern, PNResourceType.PNChannels);
            FillGrantResourcesWithPermissions(res.Groups, token, timetoken, ttl, isPattern, PNResourceType.PNGroups);
            FillGrantResourcesWithPermissions(res.Users, token, timetoken, ttl, isPattern, PNResourceType.PNUUIDMetadata);
            FillGrantResourcesWithPermissions(res.Spaces, token, timetoken, ttl, isPattern, PNResourceType.PNChannelMetadata);
        }

        public PNGrantTokenDecoded GetPermissions(string token){
            token = token.Replace("-", "+").Replace("_", "/");
            int i = token.Length % 4;
            if (i != 0) {
                token += new String('=', 4-i);
            }
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (token, PNLoggingMethod.LevelInfo);
            #endif
            PNGrantTokenDecoded pnGrantTokenDecoded = new PNGrantTokenDecoded();
            pnGrantTokenDecoded.Patterns = new GrantResources {
                Channels = new Dictionary<string, int>(),
                Groups = new Dictionary<string, int>(),
                Users = new Dictionary<string, int>(),
                Spaces = new Dictionary<string, int>()
            };
            pnGrantTokenDecoded.Resources = new GrantResources {
                Channels = new Dictionary<string, int>(),
                Groups = new Dictionary<string, int>(),
                Users = new Dictionary<string, int>(),
                Spaces = new Dictionary<string, int>()
            };
            pnGrantTokenDecoded.Meta = new Dictionary<string, object>();

            byte[] decryptedBytes = Convert.FromBase64CharArray (token.ToCharArray (), 0, token.Length);
            var cbor = CBORObject.DecodeFromBytes(decryptedBytes);

            ParseCBOR(cbor, "", ref pnGrantTokenDecoded); 

            return pnGrantTokenDecoded;
        }

        public void ParseCBOR(CBORObject cbor, string parent, ref PNGrantTokenDecoded pnGrantTokenDecoded){
            foreach (KeyValuePair<CBORObject, CBORObject> kvp in cbor.Entries){
                if(kvp.Key.Type.ToString().Equals("ByteString")){
                    string key = System.Text.Encoding.ASCII.GetString(kvp.Key.GetByteString());
                    ParseCBORValue(key, parent, kvp, ref pnGrantTokenDecoded);
                } else if(kvp.Key.Type.ToString().Equals("TextString")) {
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog (string.Format("TextString Key {0}-{1}-{2}", kvp.Key.ToString(), kvp.Value.ToString(), kvp.Value.Type), PNLoggingMethod.LevelInfo);
                    #endif
                    ParseCBORValue(kvp.Key.ToString(), parent, kvp, ref pnGrantTokenDecoded);
                } 
                #if (ENABLE_PUBNUB_LOGGING)
                else {
                    this.PubNubInstance.PNLog.WriteToLog (string.Format("Others Key {0}-{1}-{2}-{3}", kvp.Key, kvp.Key.Type, kvp.Value, kvp.Value.Type), PNLoggingMethod.LevelError);
                }
                #endif
                
            }
        }

        public void ParseCBORValue(string key, string parent, KeyValuePair<CBORObject, CBORObject> kvp, ref PNGrantTokenDecoded pnGrantTokenDecoded){
            if(kvp.Value.Type.ToString().Equals("Map")){
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format("Map Key {0}", key), PNLoggingMethod.LevelInfo);
                #endif
                var p = string.Format("{0}{1}{2}", parent, string.IsNullOrEmpty(parent)?"":":", key);
                ParseCBOR(kvp.Value, p, ref pnGrantTokenDecoded);
            } else if(kvp.Value.Type.ToString().Equals("ByteString")){                 
                #if (ENABLE_PUBNUB_LOGGING)
                string val = System.Text.Encoding.ASCII.GetString(kvp.Value.GetByteString());
                this.PubNubInstance.PNLog.WriteToLog  (string.Format("ByteString Value {0}-{1}", key, val), PNLoggingMethod.LevelInfo);
                #endif
                FillGrantToken(parent, key, kvp.Value, typeof(string), ref pnGrantTokenDecoded);
            } else if(kvp.Value.Type.ToString().Equals("Integer")){      
                #if (ENABLE_PUBNUB_LOGGING)                      
                this.PubNubInstance.PNLog.WriteToLog  (string.Format("Integer Value {0}-{1}", key, kvp.Value), PNLoggingMethod.LevelInfo);
                #endif
                FillGrantToken(parent, key, kvp.Value, typeof(int), ref pnGrantTokenDecoded);
            } 
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                this.PubNubInstance.PNLog.WriteToLog  (string.Format("Others Key Value {0}-{1}-{2}-{3}", kvp.Key.Type, kvp.Value.Type, key, kvp.Value), PNLoggingMethod.LevelError);                
            }
            #endif
        }

        public string ReplaceBoundaryQuotes(string key){
            if(key.ElementAt(0).Equals('"') && key.ElementAt(key.Length-1).Equals('"')){
                key = key.Remove(key.Length-1, 1).Remove(0, 1);
            }
            return key;
        }

        public void FillGrantToken(string parent, string key, object val, Type type, ref PNGrantTokenDecoded pnGrantTokenDecoded){            
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format("FillGrantToken: {0}", key), PNLoggingMethod.LevelInfo);
            #endif
            key = ReplaceBoundaryQuotes(key);
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format("FillGrantToken after: {0}", key), PNLoggingMethod.LevelInfo);
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
                this.PubNubInstance.PNLog.WriteToLog (string.Format("typeName: {0}", type.Name), PNLoggingMethod.LevelInfo);
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
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog (string.Format("No match on parent: {0}", parent), PNLoggingMethod.LevelInfo);
                    #endif
                    break;
                }
                break;
            }
        }

    }
}