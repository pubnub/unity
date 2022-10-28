using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace PubNubAPI
{

    public static class BuildRequests
    {

        #region "Build Requests"
        public static Uri BuildRegisterDevicePushRequest(string channel, PNPushType pushType, string pushToken, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            return BuildRegisterDevicePushRequest(channel, pushType, pushToken, pnInstance, queryParams, "", PNPushEnvironment.None);
        }

        public static Uri BuildRegisterDevicePushRequest(string channel, PNPushType pushType, string pushToken, PubNubUnity pnInstance, Dictionary<string, string> queryParams, string topic, PNPushEnvironment env)
        {
            StringBuilder parameterBuilder = new StringBuilder();

            parameterBuilder.AppendFormat("?add={0}", Utility.EncodeUricomponent(channel, PNOperationType.PNAddPushNotificationsOnChannelsOperation, true, false));
            parameterBuilder.AppendFormat("&type={0}", pushType.ToString().ToLowerInvariant());

            // Build URL
            List<string> url = new List<string>();
            string version = "v1";
            string devices = "devices";
            if(pushType.Equals(PNPushType.APNS2)){
                version = "v2";
                devices = "devices-apns2";
                PushHelpers.SetTopic(topic, ref parameterBuilder);            
                PushHelpers.SetEnvironment(env, ref parameterBuilder);    
            } 
            url.Add(version);           
            url.Add("push");
            url.Add("sub-key");
            url.Add(pnInstance.PNConfig.SubscribeKey);
            url.Add(devices);
            url.Add(pushToken);

            return BuildRestApiRequest<Uri>(url, PNOperationType.PNAddPushNotificationsOnChannelsOperation, parameterBuilder.ToString(), pnInstance, queryParams);
        }
        public static Uri BuildRemoveChannelPushRequest(string channel, PNPushType pushType, string pushToken, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            return BuildRemoveChannelPushRequest(channel, pushType, pushToken, pnInstance, queryParams, "", PNPushEnvironment.None);
        }

        public static Uri BuildRemoveChannelPushRequest(string channel, PNPushType pushType, string pushToken, PubNubUnity pnInstance, Dictionary<string, string> queryParams, string topic, PNPushEnvironment env)
        {
            StringBuilder parameterBuilder = new StringBuilder();

            parameterBuilder.AppendFormat("?remove={0}", Utility.EncodeUricomponent(channel, PNOperationType.PNRemoveChannelsFromGroupOperation, true, false));
            parameterBuilder.AppendFormat("&type={0}", pushType.ToString().ToLowerInvariant());

            // Build URL
            List<string> url = new List<string>();
            string version = "v1";
            string devices = "devices";
            if(pushType.Equals(PNPushType.APNS2)){
                version = "v2";
                devices = "devices-apns2";
                PushHelpers.SetTopic(topic, ref parameterBuilder);            
                PushHelpers.SetEnvironment(env, ref parameterBuilder);    
            } 

            url.Add(version);
            url.Add("push");
            url.Add("sub-key");
            url.Add(pnInstance.PNConfig.SubscribeKey);
            url.Add(devices);
            url.Add(pushToken);

            return BuildRestApiRequest<Uri>(url, PNOperationType.PNRemoveChannelsFromGroupOperation, parameterBuilder.ToString(), pnInstance, queryParams);
        }
        public static Uri BuildRemoveAllDevicePushRequest(PNPushType pushType, string pushToken, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            return BuildRemoveAllDevicePushRequest(pushType, pushToken, pnInstance, queryParams, "", PNPushEnvironment.None);
        }

        public static Uri BuildRemoveAllDevicePushRequest(PNPushType pushType, string pushToken, PubNubUnity pnInstance, Dictionary<string, string> queryParams, string topic, PNPushEnvironment env)
        {
            StringBuilder parameterBuilder = new StringBuilder();

            parameterBuilder.AppendFormat("?type={0}", pushType.ToString().ToLowerInvariant());

            // Build URL
            List<string> url = new List<string>();
            string version = "v1";
            string devices = "devices";
            if(pushType.Equals(PNPushType.APNS2)){
                version = "v2";
                devices = "devices-apns2";
                PushHelpers.SetTopic(topic, ref parameterBuilder);            
                PushHelpers.SetEnvironment(env, ref parameterBuilder);    
            } 
 
            url.Add(version);
            url.Add("push");
            url.Add("sub-key");
            url.Add(pnInstance.PNConfig.SubscribeKey);
            url.Add(devices);
            url.Add(pushToken);
            url.Add("remove");

            return BuildRestApiRequest<Uri>(url, PNOperationType.PNRemoveChannelsFromGroupOperation, parameterBuilder.ToString(), pnInstance, queryParams);
        }
        public static Uri BuildGetChannelsPushRequest(PNPushType pushType, string pushToken, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            return BuildGetChannelsPushRequest(pushType, pushToken, pnInstance, queryParams, "", PNPushEnvironment.None);
        }

        public static Uri BuildGetChannelsPushRequest(PNPushType pushType, string pushToken, PubNubUnity pnInstance, Dictionary<string, string> queryParams, string topic, PNPushEnvironment env)
        {
            StringBuilder parameterBuilder = new StringBuilder();

            parameterBuilder.AppendFormat("?type={0}", pushType.ToString().ToLowerInvariant());

            // Build URL
            List<string> url = new List<string>();
            string version = "v1";
            string devices = "devices";
            if(pushType.Equals(PNPushType.APNS2)){
                version = "v2";
                devices = "devices-apns2";
                PushHelpers.SetTopic(topic, ref parameterBuilder);            
                PushHelpers.SetEnvironment(env, ref parameterBuilder);    
            } 

            url.Add(version);
            url.Add("push");
            url.Add("sub-key");
            url.Add(pnInstance.PNConfig.SubscribeKey);
            url.Add(devices);
            url.Add(pushToken);

            return BuildRestApiRequest<Uri>(url, PNOperationType.PNPushNotificationEnabledChannelsOperation, parameterBuilder.ToString(), pnInstance, queryParams);
        }

        public static Uri BuildUnregisterDevicePushRequest(PNPushType pushType, string pushToken, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            StringBuilder parameterBuilder = new StringBuilder();

            parameterBuilder.AppendFormat("?type={0}", pushType.ToString().ToLowerInvariant());

            // Build URL
            List<string> url = new List<string>();
            url.Add("v1");
            url.Add("push");
            url.Add("sub-key");
            url.Add(pnInstance.PNConfig.SubscribeKey);
            url.Add("devices");
            url.Add (pushToken);
            url.Add("remove");

            return BuildRestApiRequest<Uri>(url, PNOperationType.PNRemoveAllPushNotificationsOperation, parameterBuilder.ToString (), pnInstance, queryParams);
        }

        public static Uri BuildPublishFileMessageRequest (string channel, string message, bool storeInHistory, string metadata, uint messageCounter, int ttl, bool usePost, bool repilicate, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            return BuildPublishRequestCommon (channel, message, storeInHistory, metadata, messageCounter, ttl, usePost, repilicate, pnInstance, queryParams, PNOperationType.PNPublishFileMessageOperation);
        }

        public static Uri BuildPublishRequest (string channel, string message, bool storeInHistory, string metadata, uint messageCounter, int ttl, bool usePost, bool repilicate, PubNubUnity pnInstance, Dictionary<string, string> queryParams){
            return BuildPublishRequestCommon (channel, message, storeInHistory, metadata, messageCounter, ttl, usePost, repilicate, pnInstance, queryParams, PNOperationType.PNPublishOperation);
        }

        public static Uri BuildPublishRequestCommon (string channel, string message, bool storeInHistory, string metadata, uint messageCounter, int ttl, bool usePost, bool repilicate, PubNubUnity pnInstance, Dictionary<string, string> queryParams, PNOperationType pnOperationType)
        {
            StringBuilder parameterBuilder = new StringBuilder ();
            parameterBuilder.AppendFormat ("&seqn={0}", messageCounter.ToString ());
            parameterBuilder.Append ((storeInHistory) ? "" : "&store=0");
            if (ttl >= 0) {
                parameterBuilder.AppendFormat ("&ttl={0}", ttl.ToString());
            }
            if(!repilicate){
                parameterBuilder.Append("&norep=true");
            }

            if (!string.IsNullOrEmpty (metadata) || metadata.Equals("\"\"")) {
                parameterBuilder.AppendFormat ("&meta={0}", Utility.EncodeUricomponent (metadata, PNOperationType.PNPublishOperation, false, false));
            }

            // Generate String to Sign
            string signature = "0";
            if (!string.IsNullOrEmpty(pnInstance.PNConfig.SecretKey) && (pnInstance.PNConfig.SecretKey.Length > 0)) {
                StringBuilder stringToSign = new StringBuilder ();
                stringToSign
                    .Append (pnInstance.PNConfig.PublishKey)
                    .Append ('/')
                    .Append (pnInstance.PNConfig.SubscribeKey)
                    .Append ('/')
                    .Append (pnInstance.PNConfig.SecretKey)
                    .Append ('/')
                    .Append (channel);
                    if(!usePost){
                        stringToSign.Append ('/');
                        stringToSign.Append (message); // 1
                    }

                // Sign Message
                PubnubCrypto pnCrypto = new PubnubCrypto (pnInstance.PNConfig.CipherKey, pnInstance.PNLog);
                signature = pnCrypto.ComputeHashRaw(stringToSign.ToString ());
            }

            // Build URL
            List<string> url = new List<string> ();
            if(pnOperationType.Equals(PNOperationType.PNPublishFileMessageOperation)){
                url.Add ("v1");
                url.Add ("files");
                url.Add ("publish-file");
            } else {
                url.Add ("publish");
            }
            url.Add (pnInstance.PNConfig.PublishKey);
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add (signature);
            url.Add (channel);
            url.Add ("0");
            if(!usePost){
                url.Add (message);
            }

            return BuildRestApiRequest<Uri> (url, pnOperationType, parameterBuilder.ToString (), pnInstance, queryParams);
        }
        public static Uri BuildDeleteFileRequest (string channel, string id, string name, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            ///v1/files/%s/channels/%s/files/%s/%s
            List<string> url = new List<string> ();
            url.Add ("v1");
            url.Add ("files");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("channels");
            url.Add (channel);
            url.Add ("files");
            url.Add (id);
            url.Add (name);

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNGetMessageActionsOperation, "", pnInstance, queryParams);
        }  

        public static Uri BuildDownloadFileRequest (string channel, string id, string name, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            ///v1/files/%s/channels/%s/files/%s/%s
            List<string> url = new List<string> ();
            url.Add ("v1");
            url.Add ("files");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("channels");
            url.Add (channel);
            url.Add ("files");
            url.Add (id);
            url.Add (name);

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNGetMessageActionsOperation, "", pnInstance, queryParams);
        } 

        public static Uri BuildSendFileRequest (string channel, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            ///v1/files/%s/channels/%s/generate-upload-url
            List<string> url = new List<string> ();
            url.Add ("v1");
            url.Add ("files");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("channels");
            url.Add (channel);
            url.Add ("generate-upload-url");

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNGetMessageActionsOperation, "", pnInstance, queryParams);
        }  
        public static Uri BuildListFilesRequest (string channel, int limit, string next, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            ///v1/files/%s/channels/%s/files
            StringBuilder parameterBuilder = new StringBuilder ();
            if (limit > 0) {
                parameterBuilder.AppendFormat ("&limit={0}", limit.ToString());
            }
            if (!string.IsNullOrEmpty(next) ) {
                parameterBuilder.AppendFormat ("&next={0}", next);
            }

            List<string> url = new List<string> ();
            url.Add ("v1");
            url.Add ("files");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("channels");
            url.Add (channel);
            url.Add ("files");

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNGetMessageActionsOperation, parameterBuilder.ToString (), pnInstance, queryParams);
        }                                                

        public static Uri BuildSignalRequest (string channel, string message, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            List<string> url = new List<string> ();
            url.Add ("signal");
            url.Add (pnInstance.PNConfig.PublishKey);
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("0");
            url.Add (channel);
            url.Add ("0");
            url.Add (message);

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNSignalOperation, "", pnInstance, queryParams);
        }

        public static Uri BuildAddMessageActionsRequest (string channel, string messageTimetoken, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            ///v1/message-actions/%s/channel/%s/message/%s
            List<string> url = new List<string> ();
            url.Add ("v1");
            url.Add ("message-actions");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("channel");
            url.Add (channel);
            url.Add ("message");
            url.Add (messageTimetoken);

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNAddMessageActionsOperation, "", pnInstance, queryParams);
        }

        public static Uri BuildRemoveMessageActionsRequest (string channel, string messageTimetoken, string actionTimetoken, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            ///v1/message-actions/%s/channel/%s/message/%s/action/%s

            List<string> url = new List<string> ();
            url.Add ("v1");
            url.Add ("message-actions");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("channel");
            url.Add (channel);
            url.Add ("message");
            url.Add (messageTimetoken);
            url.Add ("action");
            url.Add (actionTimetoken);

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNRemoveMessageActionsOperation, "", pnInstance, queryParams);
        }

        public static Uri BuildGetMessageActionsRequest (string channel, long start, long end, int limit, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            ///v1/message-actions/%s/channel/%s
            StringBuilder parameterBuilder = new StringBuilder ();
            if (limit > 0) {
                parameterBuilder.AppendFormat ("&limit={0}", limit.ToString());
            }
            if (start > 0 ) {
                parameterBuilder.AppendFormat ("&start={0}", start.ToString());
            }
            if (end > 0) {
                parameterBuilder.AppendFormat ("&end={0}", end.ToString());
            }

            List<string> url = new List<string> ();
            url.Add ("v1");
            url.Add ("message-actions");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("channel");
            url.Add (channel);

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNGetMessageActionsOperation, parameterBuilder.ToString (), pnInstance, queryParams);
        }

        public static Uri BuildObjectsSetUUIDMetadataRequest (string id, string include, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            ///v1/objects/%s/users/%s
            StringBuilder parameterBuilder = new StringBuilder ();
            if(!string.IsNullOrEmpty(include)){
                parameterBuilder.AppendFormat ("&include={0}", include);
            }

            List<string> url = new List<string> ();
            url.Add ("v2");
            url.Add ("objects");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("uuids");
            url.Add (id);

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNSetUUIDMetadataOperation, parameterBuilder.ToString (), pnInstance, queryParams);
        }

        public static Uri BuildObjectsSetChannelMetadataRequest(string id, string include, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            ///v1/objects/%s/spaces/%s
            StringBuilder parameterBuilder = new StringBuilder();
            if(!string.IsNullOrEmpty(include)){
                parameterBuilder.AppendFormat("&include={0}", include);
            }

            List<string> url = new List<string>();
            url.Add("v2");
            url.Add("objects");
            url.Add(pnInstance.PNConfig.SubscribeKey);
            url.Add("channels");
            url.Add(id);

            return BuildRestApiRequest<Uri>(url, PNOperationType.PNSetChannelMetadataOperation, parameterBuilder.ToString (), pnInstance, queryParams);
        }

        public static Uri BuildObjectsDeleteUUIDMetadataRequest (string id, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            ///v1/objects/%s/users/%s
            StringBuilder parameterBuilder = new StringBuilder ();

            List<string> url = new List<string> ();
            url.Add ("v2");
            url.Add ("objects");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("uuids");
            url.Add (id);

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNRemoveUUIDMetadataOperation, parameterBuilder.ToString (), pnInstance, queryParams);
        }

        public static Uri BuildObjectsDeleteChannelMetadataRequest(string id, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            ///v1/objects/%s/spaces/%s
            StringBuilder parameterBuilder = new StringBuilder();

            List<string> url = new List<string>();
            url.Add("v2");
            url.Add("objects");
            url.Add(pnInstance.PNConfig.SubscribeKey);
            url.Add("channels");
            url.Add(id);

            return BuildRestApiRequest<Uri>(url, PNOperationType.PNRemoveChannelMetadataOperation, parameterBuilder.ToString (), pnInstance, queryParams);
        }

        public static Uri BuildObjectsGetUUIDMetadataRequest (string id, string include, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            ///v1/objects/%s/users/%s
            StringBuilder parameterBuilder = new StringBuilder ();
            if(!string.IsNullOrEmpty(include)){
                parameterBuilder.AppendFormat ("&include={0}", include);
            }

            List<string> url = new List<string> ();
            url.Add ("v2");
            url.Add ("objects");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("uuids");
            url.Add (id);

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNGetUUIDMetadataOperation, parameterBuilder.ToString (), pnInstance, queryParams);
        }

        public static Uri BuildObjectsGetChannelMetadataRequest(string id, string include, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            ///v1/objects/%s/spaces/%s
            StringBuilder parameterBuilder = new StringBuilder();
            if(!string.IsNullOrEmpty(include)){
                parameterBuilder.AppendFormat("&include={0}", include);
            }

            List<string> url = new List<string>();
            url.Add("v2");
            url.Add("objects");
            url.Add(pnInstance.PNConfig.SubscribeKey);
            url.Add("channels");
            url.Add(id);

            return BuildRestApiRequest<Uri>(url, PNOperationType.PNGetChannelMetadataOperation, parameterBuilder.ToString (), pnInstance, queryParams);
        }

        public static Uri BuildObjectsGetAllUUIDMetadataRequest (int limit, string start, string end, bool count, string include, PubNubUnity pnInstance, Dictionary<string, string> queryParams, string filter, string sortBy)
        {
            ///v1/objects/%s/users
            StringBuilder parameterBuilder = new StringBuilder ();
            if(!string.IsNullOrEmpty(include)){
                parameterBuilder.AppendFormat ("&include={0}", include);
            }
            parameterBuilder.Append ((count) ? "&count=1" : "&count=0");
            if (limit > 0) {
                parameterBuilder.AppendFormat ("&limit={0}", limit.ToString());
            }
            if (!string.IsNullOrEmpty(start)) {
                parameterBuilder.AppendFormat ("&start={0}", start);
            }
            if (!string.IsNullOrEmpty(end)) {
                parameterBuilder.AppendFormat ("&end={0}", end);
            }
            if (!string.IsNullOrEmpty(filter)) {
                parameterBuilder.AppendFormat ("&filter={0}", Utility.EncodeUricomponent(filter, PNOperationType.PNGetAllUUIDMetadataOperation, false, false));
            }
            if (!string.IsNullOrEmpty(sortBy))
            {
                parameterBuilder.AppendFormat("&sort={0}", Utility.EncodeUricomponent(sortBy, PNOperationType.PNGetAllUUIDMetadataOperation, false, false));
            }

            List<string> url = new List<string> ();
            url.Add ("v2");
            url.Add ("objects");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("uuids");

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNGetAllUUIDMetadataOperation, parameterBuilder.ToString (), pnInstance, queryParams);
        }

        public static Uri BuildObjectsGetAllChannelMetadataRequest(int limit, string start, string end, bool count, string include, PubNubUnity pnInstance, Dictionary<string, string> queryParams, string filter, string sortBy)
        {
            ///v1/objects/%s/spaces
            StringBuilder parameterBuilder = new StringBuilder();
            if(!string.IsNullOrEmpty(include)){
                parameterBuilder.AppendFormat("&include={0}", include);
            }
            parameterBuilder.Append((count) ? "&count=1" : "&count=0");
            if (limit > 0)
            {
                parameterBuilder.AppendFormat("&limit={0}", limit.ToString());
            }
            if (!string.IsNullOrEmpty(start))
            {
                parameterBuilder.AppendFormat("&start={0}", start);
            }
            if (!string.IsNullOrEmpty(end))
            {
                parameterBuilder.AppendFormat("&end={0}", end);
            }
            if (!string.IsNullOrEmpty(filter)) {
                parameterBuilder.AppendFormat ("&filter={0}", Utility.EncodeUricomponent(filter, PNOperationType.PNGetAllChannelMetadataOperation, false, false));
            }
            if (!string.IsNullOrEmpty(sortBy))
            {
                parameterBuilder.AppendFormat("&sort={0}", Utility.EncodeUricomponent(sortBy, PNOperationType.PNGetAllChannelMetadataOperation, false, false));
            } 

            List<string> url = new List<string>();
            url.Add("v2");
            url.Add("objects");
            url.Add(pnInstance.PNConfig.SubscribeKey);
            url.Add("channels");

            return BuildRestApiRequest<Uri>(url, PNOperationType.PNGetAllChannelMetadataOperation, parameterBuilder.ToString (), pnInstance, queryParams);
        }

        public static Uri BuildObjectsGetChannelMembersRequest (string channelID, int limit, string start, string end, bool count, string include, PubNubUnity pnInstance, Dictionary<string, string> queryParams, string filter, string sortBy)
        {
            ///v1/objects/%s/spaces/%s/users
            StringBuilder parameterBuilder = new StringBuilder ();
            if(!string.IsNullOrEmpty(include)){
                parameterBuilder.AppendFormat ("&include={0}", include);
            }
            parameterBuilder.Append ((count) ? "&count=1" : "&count=0");
            if (limit > 0) {
                parameterBuilder.AppendFormat ("&limit={0}", limit.ToString());
            }
            if (!string.IsNullOrEmpty(start)) {
                parameterBuilder.AppendFormat ("&start={0}", start);
            }
            if (!string.IsNullOrEmpty(end)) {
                parameterBuilder.AppendFormat ("&end={0}", end);
            }
            if (!string.IsNullOrEmpty(filter)) {
                parameterBuilder.AppendFormat ("&filter={0}", Utility.EncodeUricomponent(filter, PNOperationType.PNGetChannelMembersOperation, false, false));
            }
            if (!string.IsNullOrEmpty(sortBy)){
                parameterBuilder.AppendFormat("&sort={0}", Utility.EncodeUricomponent(sortBy, PNOperationType.PNGetChannelMembersOperation, false, false));
            }

            List<string> url = new List<string> ();
            url.Add ("v2");
            url.Add ("objects");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("channels");
            url.Add (channelID);
            url.Add ("uuids");

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNGetChannelMembersOperation, parameterBuilder.ToString (), pnInstance, queryParams);
        } 

        public static Uri BuildObjectsManageChannelMembersRequest (string channelMetatdataID, int limit, string start, string end, bool count, string include, PubNubUnity pnInstance, Dictionary<string, string> queryParams, string sortBy)
        {
            ///v1/objects/%s/spaces/%s/users
            StringBuilder parameterBuilder = new StringBuilder ();
            if(!string.IsNullOrEmpty(include)){
                parameterBuilder.AppendFormat ("&include={0}", include);
            }
            parameterBuilder.Append ((count) ? "&count=1" : "&count=0");
            if (limit > 0) {
                parameterBuilder.AppendFormat ("&limit={0}", limit.ToString());
            }
            if (!string.IsNullOrEmpty(start)) {
                parameterBuilder.AppendFormat ("&start={0}", start);
            }
            if (!string.IsNullOrEmpty(end)) {
                parameterBuilder.AppendFormat ("&end={0}", end);
            }
            if (!string.IsNullOrEmpty(sortBy)){
                parameterBuilder.AppendFormat("&sort={0}", Utility.EncodeUricomponent(sortBy, PNOperationType.PNManageChannelMembersOperation, false, false));
            }

            List<string> url = new List<string> ();
            url.Add ("v2");
            url.Add ("objects");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("channels");
            url.Add (channelMetatdataID);
            url.Add ("uuids");

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNManageChannelMembersOperation, parameterBuilder.ToString (), pnInstance, queryParams);
        } 

        public static Uri BuildObjectsManageMembershipsRequest(string userID, int limit, string start, string end, bool count, string include, PubNubUnity pnInstance, Dictionary<string, string> queryParams, string sortBy)
        {
            ///v1/objects/%s/users/%s/spaces
            StringBuilder parameterBuilder = new StringBuilder ();
            if(!string.IsNullOrEmpty(include)){
                parameterBuilder.AppendFormat ("&include={0}", include);
            }
            parameterBuilder.Append ((count) ? "&count=1" : "&count=0");
            if (limit > 0) {
                parameterBuilder.AppendFormat ("&limit={0}", limit.ToString());
            }
            if (!string.IsNullOrEmpty(start)) {
                parameterBuilder.AppendFormat ("&start={0}", start);
            }
            if (!string.IsNullOrEmpty(end)) {
                parameterBuilder.AppendFormat ("&end={0}", end);
            }
            if (!string.IsNullOrEmpty(sortBy)){
                parameterBuilder.AppendFormat("&sort={0}", Utility.EncodeUricomponent(sortBy, PNOperationType.PNManageMembershipsOperation, false, false));
            }

            List<string> url = new List<string> ();
            url.Add ("v2");
            url.Add ("objects");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("uuids");
            url.Add (userID);
            url.Add ("channels");

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNManageMembershipsOperation, parameterBuilder.ToString (), pnInstance, queryParams);
        }  
        public static Uri BuildObjectsGetMembershipsRequest(string userID, int limit, string start, string end, bool count, string include, PubNubUnity pnInstance, Dictionary<string, string> queryParams, string filter, string sortBy)
        {
            ///v1/objects/%s/users/%s/spaces
            StringBuilder parameterBuilder = new StringBuilder ();
            if(!string.IsNullOrEmpty(include)){
                parameterBuilder.AppendFormat ("&include={0}", include);
            }
            parameterBuilder.Append ((count) ? "&count=1" : "&count=0");
            if (limit > 0) {
                parameterBuilder.AppendFormat ("&limit={0}", limit.ToString());
            }
            if (!string.IsNullOrEmpty(start)) {
                parameterBuilder.AppendFormat ("&start={0}", start);
            }
            if (!string.IsNullOrEmpty(end)) {
                parameterBuilder.AppendFormat ("&end={0}", end);
            }
            if (!string.IsNullOrEmpty(filter)) {
                parameterBuilder.AppendFormat ("&filter={0}", Utility.EncodeUricomponent(filter, PNOperationType.PNGetMembershipsOperation, false, false));
            }
            if (!string.IsNullOrEmpty(sortBy)){
                parameterBuilder.AppendFormat("&sort={0}", Utility.EncodeUricomponent(sortBy, PNOperationType.PNGetMembershipsOperation, false, false));
            }

            List<string> url = new List<string> ();
            url.Add ("v2");
            url.Add ("objects");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("uuids");
            url.Add (userID);
            url.Add ("channels");

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNGetMembershipsOperation, parameterBuilder.ToString (), pnInstance, queryParams);
        } 

        public static Uri BuildDeleteMessagesRequest (string channel, long start, long end, PubNubUnity pnInstance, Dictionary<string, string> queryParams){
            StringBuilder parameterBuilder = new StringBuilder ();
            if (start != -1) {
                parameterBuilder.AppendFormat ("&start={0}", start.ToString ().ToLowerInvariant ());
            }
            if (end != -1) {
                parameterBuilder.AppendFormat ("&end={0}", end.ToString ().ToLowerInvariant ());
            }
            long timeStamp = Utility.TranslateDateTimeToSeconds (DateTime.UtcNow);
            parameterBuilder.AppendFormat ("&timestamp={0}", timeStamp.ToString());

            List<string> url = new List<string> ();

            url.Add ("v3");
            url.Add ("history");
            url.Add ("sub-key");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("channel");
            url.Add (channel);

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNDeleteMessagesOperation, parameterBuilder.ToString(), pnInstance, queryParams);
        }

        public static Uri BuildMessageCountsRequest (string[] channels, string channelsTimetoken, string timetoken, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            StringBuilder parameterBuilder = new StringBuilder ();

            if (!string.IsNullOrEmpty(timetoken)) {
                parameterBuilder.AppendFormat("?timetoken={0}&", timetoken);
            } else {
                parameterBuilder.Append("?");
            }
            parameterBuilder.AppendFormat("channelsTimetoken={0}", channelsTimetoken);

            List<string> url = new List<string> ();

            url.Add ("v3");
            url.Add ("history");
            url.Add ("sub-key");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("message-counts");
            url.Add (string.Join(",", channels));

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNMessageCountsOperation, parameterBuilder.ToString(), pnInstance, queryParams);
        }

        public static Uri BuildFetchRequest (string[] channels, long start, long end, uint count, bool reverse, bool includeToken, PubNubUnity pnInstance, Dictionary<string, string> queryParams, bool withMeta, bool withMessageActions, bool withMessageType, bool withUUID)
        {
            StringBuilder parameterBuilder = new StringBuilder ();

            parameterBuilder.AppendFormat ("?max={0}", count);
            if (includeToken) {
                parameterBuilder.AppendFormat ("&include_token={0}", includeToken.ToString ().ToLowerInvariant ());
            }
            if (reverse) {
                parameterBuilder.AppendFormat ("&reverse={0}", reverse.ToString ().ToLowerInvariant ());
            }
            if (start != -1) {
                parameterBuilder.AppendFormat ("&start={0}", start.ToString ().ToLowerInvariant ());
            }
            if (end != -1) {
                parameterBuilder.AppendFormat ("&end={0}", end.ToString ().ToLowerInvariant ());
            }
            if (withMeta) {
                parameterBuilder.AppendFormat ("&include_meta={0}", withMeta.ToString ().ToLowerInvariant ());
            }
            if (withMessageType) {
                parameterBuilder.AppendFormat ("&include_message_type={0}", withMessageType.ToString ().ToLowerInvariant ());
            }
            if (withUUID) {
                parameterBuilder.AppendFormat ("&include_uuid={0}", withUUID.ToString ().ToLowerInvariant ());
            }
            List<string> url = new List<string> ();
            
            url.Add ("v3");
            if(withMessageActions){
                url.Add ("history-with-actions");
            } else {
                url.Add ("history");
            }
            
            url.Add ("sub-key");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("channel");
            url.Add (string.Join(",", channels));
            
            return BuildRestApiRequest<Uri> (url, PNOperationType.PNFetchMessagesOperation, parameterBuilder.ToString(), pnInstance, queryParams);
        }

        public static Uri BuildHistoryRequest (string channel, long start, long end, uint count, bool reverse, bool includeToken, PubNubUnity pnInstance, Dictionary<string, string> queryParams, bool withMeta)
        {
            StringBuilder parameterBuilder = new StringBuilder ();

            parameterBuilder.AppendFormat ("?count={0}", count);
            if (includeToken) {
                parameterBuilder.AppendFormat ("&include_token={0}", includeToken.ToString ().ToLowerInvariant ());
            }
            if (reverse) {
                parameterBuilder.AppendFormat ("&reverse={0}", reverse.ToString ().ToLowerInvariant ());
            }
            if (start != -1) {
                parameterBuilder.AppendFormat ("&start={0}", start.ToString ().ToLowerInvariant ());
            }
            if (end != -1) {
                parameterBuilder.AppendFormat ("&end={0}", end.ToString ().ToLowerInvariant ());
            }
            if (withMeta) {
                parameterBuilder.AppendFormat ("&include_meta={0}", withMeta.ToString ().ToLowerInvariant ());
            }

            List<string> url = new List<string> ();

            url.Add ("v2");
            url.Add ("history");
            url.Add ("sub-key");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("channel");
            url.Add (channel);

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNHistoryOperation, parameterBuilder.ToString(), pnInstance, queryParams);
        }

        public static Uri BuildHereNowRequest (string channel, string channelGroups, bool showUUIDList, bool includeUserState, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            int disableUUID = (showUUIDList) ? 0 : 1;
            int userState = (includeUserState) ? 1 : 0;
            StringBuilder parameterBuilder = new StringBuilder ();
            parameterBuilder.AppendFormat ("?disable_uuids={0}&state={1}", disableUUID, userState);
            if (!string.IsNullOrEmpty(channelGroups))
            {
                parameterBuilder.AppendFormat("&channel-group={0}",  Utility.EncodeUricomponent(channelGroups, PNOperationType.PNHereNowOperation, true, false));
            }

            List<string> url = new List<string> ();

            url.Add ("v2");
            url.Add ("presence");
            url.Add ("sub_key");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            if(!string.IsNullOrEmpty(channel))
            {
                url.Add ("channel");
                url.Add (channel);
            } else if (string.IsNullOrEmpty(channel) && (!string.IsNullOrEmpty(channelGroups))){
                url.Add ("channel");
                url.Add (",");
            }

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNHereNowOperation, parameterBuilder.ToString(), pnInstance, queryParams);
        }


        public static Uri BuildWhereNowRequest (string uuid, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            List<string> url = new List<string> ();

            url.Add ("v2");
            url.Add ("presence");
            url.Add ("sub_key");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("uuid");
            url.Add (uuid);

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNWhereNowOperation, "", pnInstance, queryParams);
        }

        public static Uri BuildTimeRequest (PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            List<string> url = new List<string> ();

            url.Add ("time");
            url.Add ("0");

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNTimeOperation, "", pnInstance, queryParams);
        }

        public static Uri BuildSetStateRequest (string channel, string channelGroup, string jsonUserState, string uuid, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            StringBuilder paramBuilder = new StringBuilder ();
            paramBuilder.AppendFormat ("?state={0}", Utility.EncodeUricomponent (jsonUserState, PNOperationType.PNSetStateOperation, false, false));
            if (!string.IsNullOrEmpty(channelGroup) && channelGroup.Trim().Length > 0)
            {
                paramBuilder.AppendFormat("&channel-group={0}", Utility.EncodeUricomponent(channelGroup, PNOperationType.PNSetStateOperation, true, false));
            }

            List<string> url = new List<string> ();

            url.Add ("v2");
            url.Add ("presence");
            url.Add ("sub_key");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("channel");
            url.Add (string.IsNullOrEmpty(channel) ? "," : channel);
            url.Add ("uuid");
            url.Add (uuid);
            url.Add ("data");

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNSetStateOperation, paramBuilder.ToString (), pnInstance, queryParams);
        }

        public static Uri BuildGetStateRequest (string channel, string channelGroup, string uuid, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            string parameters = "";
            if (!string.IsNullOrEmpty(channelGroup) && channelGroup.Trim().Length > 0)
            {
                parameters = string.Format("&channel-group={0}", Utility.EncodeUricomponent(channelGroup, PNOperationType.PNGetStateOperation, true, false));
            }

            List<string> url = new List<string> ();

            url.Add ("v2");
            url.Add ("presence");
            url.Add ("sub_key");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("channel");
            url.Add (string.IsNullOrEmpty(channel) ? "," : channel);
            url.Add ("uuid");
            url.Add (uuid);

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNGetStateOperation, parameters, pnInstance, queryParams);
        }

        public static Uri BuildPresenceHeartbeatRequest (string channels, string channelGroups, string channelsJsonState, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            StringBuilder presenceParamBuilder = new StringBuilder ();
            if (channelsJsonState != "{}" && channelsJsonState != "") {
                presenceParamBuilder.AppendFormat("&state={0}", Utility.EncodeUricomponent (channelsJsonState, PNOperationType.PNPresenceHeartbeatOperation, false, false));
            }
            if (channelGroups != null && channelGroups.Length > 0)
            {
                presenceParamBuilder.AppendFormat("&channel-group={0}", Utility.EncodeUricomponent(channelGroups, PNOperationType.PNPresenceHeartbeatOperation, true, false));
            }

            List<string> url = new List<string> ();

            url.Add ("v2");
            url.Add ("presence");
            url.Add ("sub_key");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("channel");
            url.Add (string.IsNullOrEmpty(channels) ? "," : channels);
            url.Add ("heartbeat");

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNPresenceHeartbeatOperation, presenceParamBuilder.ToString(), pnInstance, queryParams);
        }

        public static Uri BuildLeaveRequest (string channels, string channelGroups, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            string compiledUserState = pnInstance.SubscriptionInstance.CompiledUserState;
            
            StringBuilder unsubscribeParamBuilder = new StringBuilder ();
            if(!string.IsNullOrEmpty(compiledUserState)){
                unsubscribeParamBuilder.AppendFormat("&state={0}", Utility.EncodeUricomponent(compiledUserState, PNOperationType.PNLeaveOperation, false, false));
            }
            if (channelGroups != null && channelGroups.Length > 0)
            {
                unsubscribeParamBuilder.AppendFormat("&channel-group={0}",  Utility.EncodeUricomponent(channelGroups, PNOperationType.PNLeaveOperation, true, false));
            }

            List<string> url = new List<string> ();

            url.Add ("v2");
            url.Add ("presence");
            url.Add ("sub_key");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add ("channel");
            url.Add (string.IsNullOrEmpty(channels) ? "," : channels);
            url.Add ("leave");

            return BuildRestApiRequest<Uri> (url, PNOperationType.PNLeaveOperation, unsubscribeParamBuilder.ToString(), pnInstance, queryParams);
        }

        public static Uri BuildSubscribeRequest (string channels, string channelGroups, string timetoken, string channelsJsonState, string region, string filterExpr, PubNubUnity pnInstance, Dictionary<string, string> queryParams){
            StringBuilder subscribeParamBuilder = new StringBuilder ();
            subscribeParamBuilder.AppendFormat ("&tt={0}", timetoken);

            if (!string.IsNullOrEmpty (filterExpr)) {
                subscribeParamBuilder.AppendFormat ("&filter-expr=({0})",  Utility.EncodeUricomponent(filterExpr, PNOperationType.PNSubscribeOperation, false, false));
            }

            if (!string.IsNullOrEmpty (region)) {
                subscribeParamBuilder.AppendFormat ("&tr={0}", Utility.EncodeUricomponent(region, PNOperationType.PNSubscribeOperation, false, false));
            }

            if (channelsJsonState != "{}" && channelsJsonState != "") {
                subscribeParamBuilder.AppendFormat ("&state={0}", Utility.EncodeUricomponent (channelsJsonState, PNOperationType.PNSubscribeOperation, false, false));
            }

            if (!string.IsNullOrEmpty(channelGroups))
            {
                subscribeParamBuilder.AppendFormat ("&channel-group={0}", Utility.EncodeUricomponent (channelGroups, PNOperationType.PNSubscribeOperation, true, false));
            }                   

            List<string> url = new List<string> ();
            url.Add ("v2");
            url.Add ("subscribe");
            url.Add (pnInstance.PNConfig.SubscribeKey);
            url.Add (string.IsNullOrEmpty(channels) ? "," : channels);
            url.Add ("0");

            return BuildRestApiRequest<Uri> (
                url, 
                PNOperationType.PNSubscribeOperation, 
                subscribeParamBuilder.ToString (),
                pnInstance,
                queryParams
            );
        }

        public static Uri BuildAddChannelsToChannelGroupRequest(string[] channels, string nameSpace, string groupName, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            string parameters = string.Format("?add={0}", Utility.EncodeUricomponent(string.Join(",", channels), PNOperationType.PNAddChannelsToGroupOperation, true, false));

            // Build URL
            List<string> url = new List<string>();
            url.Add("v1");
            url.Add("channel-registration");
            url.Add("sub-key");
            url.Add(pnInstance.PNConfig.SubscribeKey);
            List<string> ns = Utility.CheckAndAddNameSpace (nameSpace);
            if (ns != null) {
                url.AddRange (ns);    
            }

            url.Add("channel-group");
            url.Add(groupName);

            return BuildRestApiRequest<Uri>(url, PNOperationType.PNAddChannelsToGroupOperation, parameters, pnInstance, queryParams);
        }

        public static Uri BuildRemoveChannelsFromChannelGroupRequest(string[] channels, string nameSpace, string groupName, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            bool channelsAvailable = false;
            string parameters = "";
            if (channels != null && channels.Length > 0) {
                parameters = string.Format ("?remove={0}", Utility.EncodeUricomponent(string.Join(",", channels), PNOperationType.PNAddChannelsToGroupOperation, true, false));
                channelsAvailable = true;
            }

            // Build URL
            List<string> url = new List<string>();
            url.Add("v1");
            url.Add("channel-registration");
            url.Add("sub-key");
            url.Add(pnInstance.PNConfig.SubscribeKey);
            List<string> ns = Utility.CheckAndAddNameSpace (nameSpace);
            if (ns != null) {
                url.AddRange (ns);    
            }
            url.Add("channel-group");
            url.Add(groupName);

            PNOperationType respType = PNOperationType.PNAddChannelsToGroupOperation;
            if (!channelsAvailable) {
                url.Add ("remove");
                respType = PNOperationType.PNRemoveGroupOperation;
            }

            return BuildRestApiRequest<Uri> (url, respType, parameters, pnInstance, queryParams);
        }

        public static Uri BuildGetChannelsForChannelGroupRequest(string nameSpace, string groupName, bool limitToChannelGroupScopeOnly, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            bool groupNameAvailable = false;
            bool nameSpaceAvailable = false;

            // Build URL
            List<string> url = new List<string>();
            url.Add("v1");
            url.Add("channel-registration");
            url.Add("sub-key");
            url.Add(pnInstance.PNConfig.SubscribeKey);
            List<string> ns = Utility.CheckAndAddNameSpace (nameSpace);
            if (ns != null) {
                nameSpaceAvailable = true;
                url.AddRange (ns);    
            }

            if (limitToChannelGroupScopeOnly)
            {
                url.Add("channel-group");
            }
            else
            {
                if (!string.IsNullOrEmpty(groupName) && groupName.Trim().Length > 0)
                {
                    groupNameAvailable = true;
                    url.Add("channel-group");
                    url.Add(groupName);
                }

                if (!nameSpaceAvailable && !groupNameAvailable)
                {
                    url.Add("namespace");
                }
                else if (nameSpaceAvailable && !groupNameAvailable)
                {
                    url.Add("channel-group");
                }
            }

            return BuildRestApiRequest<Uri>(url, PNOperationType.PNChannelsForGroupOperation, "", pnInstance, queryParams);
        }

        private static StringBuilder AppendAuthKeyToURL(StringBuilder url, string authenticationKey, PNOperationType type){
            if (!string.IsNullOrEmpty (authenticationKey)) {
                url.AppendFormat ("&auth={0}", Utility.EncodeUricomponent (authenticationKey, type, false, false));
            }
            return url;
        }

        private static StringBuilder AppendUUIDToURL(StringBuilder url, string uuid, bool firstInQS){
            if (firstInQS)
            {
                url.AppendFormat("?uuid={0}", uuid);
            }
            else
            {
                url.AppendFormat("&uuid={0}", uuid);
            }
            return url;
        }

        private static StringBuilder AppendPresenceHeartbeatToURL(StringBuilder url, int pubnubPresenceHeartbeatInSeconds){
            if (pubnubPresenceHeartbeatInSeconds != 0) {
                url.AppendFormat ("&heartbeat={0}", pubnubPresenceHeartbeatInSeconds);
            }
            return url;
        }

        private static StringBuilder AppendPNSDKVersionToURL(StringBuilder url, string pnsdkVersion, PNOperationType type){
            url.AppendFormat ("&pnsdk={0}", Utility.EncodeUricomponent (pnsdkVersion, type, false, true));
            return url;
        }

        private static StringBuilder AppendLatencyToURL(StringBuilder url, PNOperationType operationType, PNLatencyManager latency){
            //TODO Add delete history 
            switch(operationType){
                case PNOperationType.PNTimeOperation:
                    if(latency.Time > 0){
                        url.AppendFormat("&l_time={0}", latency.Time);
                    }
                    break;
                case PNOperationType.PNPublishOperation:
                    if(latency.Publish > 0){
                        url.AppendFormat("&l_pub={0}", latency.Publish);
                    }
                    break;
                case PNOperationType.PNWhereNowOperation:
                case PNOperationType.PNHereNowOperation:
                case PNOperationType.PNLeaveOperation:
                case PNOperationType.PNSetStateOperation:
                case PNOperationType.PNGetStateOperation:
                    if(latency.Presence > 0){
                        url.AppendFormat("&l_pres={0}", latency.Presence);
                    }
                    break;
                case PNOperationType.PNRemoveAllPushNotificationsOperation:
                case PNOperationType.PNAddPushNotificationsOnChannelsOperation:
                case PNOperationType.PNPushNotificationEnabledChannelsOperation:
                case PNOperationType.PNRemovePushNotificationsFromChannelsOperation:
                    if(latency.MobilePush > 0){
                        url.AppendFormat("&l_push={0}", latency.MobilePush);
                    }
                    break;
                case PNOperationType.PNMessageCountsOperation:
                    if(latency.MessageCounts > 0){
                        url.AppendFormat("&l_mc={0}", latency.MessageCounts);
                    }
                    break;
                case PNOperationType.PNFetchMessagesOperation:
                case PNOperationType.PNHistoryOperation:
                case PNOperationType.PNHistoryWithActionsOperation:
                    if(latency.History > 0){
                        url.AppendFormat("&l_hist={0}", latency.History);
                    }
                    break;
                case PNOperationType.PNAddChannelsToGroupOperation:
                case PNOperationType.PNChannelGroupsOperation:
                case PNOperationType.PNChannelsForGroupOperation:
                case PNOperationType.PNRemoveChannelsFromGroupOperation:
                case PNOperationType.PNRemoveGroupOperation:
                    if(latency.ChannelGroups > 0){
                        url.AppendFormat("&l_cg={0}", latency.ChannelGroups);
                    }
                    break;
                case PNOperationType.PNSignalOperation:
                    if(latency.Signal > 0){
                        url.AppendFormat("&l_sig={0}", latency.Signal);
                    }
                    break;
                case PNOperationType.PNGetMessageActionsOperation:
                case PNOperationType.PNAddMessageActionsOperation:
                case PNOperationType.PNRemoveMessageActionsOperation:
                    if(latency.MessageActions > 0){
                        url.AppendFormat("&l_msga={0}", latency.Signal);
                    }
                    break;    
                case PNOperationType.PNSetUUIDMetadataOperation:
                case PNOperationType.PNGetChannelMetadataOperation:
                case PNOperationType.PNGetAllUUIDMetadataOperation:
                case PNOperationType.PNGetUUIDMetadataOperation:
                case PNOperationType.PNRemoveUUIDMetadataOperation:
                case PNOperationType.PNGetAllChannelMetadataOperation:
                case PNOperationType.PNRemoveChannelMetadataOperation:
                case PNOperationType.PNSetChannelMetadataOperation:
                case PNOperationType.PNGetMembershipsOperation:
                case PNOperationType.PNGetChannelMembersOperation:
                case PNOperationType.PNManageMembershipsOperation:
                case PNOperationType.PNManageChannelMembersOperation:
                    if(latency.Objects > 0){
                        url.AppendFormat("&l_obj={0}", latency.Signal);
                    }
                    break; 
                case PNOperationType.PNDeleteFileOperation:
                case PNOperationType.PNDownloadFileOperation:
                case PNOperationType.PNGetFileURLOperation:
                case PNOperationType.PNListFilesOperation:
                case PNOperationType.PNSendFileOperation:
                    if(latency.Files > 0){
                        url.AppendFormat("&l_file={0}", latency.Files);
                    }
                    break; 

                default:
                    break;    
                    
            }
            return url;
        }

        static StringBuilder EncodeURL (List<string> urlComponents, PNOperationType type){
            StringBuilder url = new StringBuilder();
             // Generate URL with UTF-8 Encoding
            for (int componentIndex = 0; componentIndex < urlComponents.Count; componentIndex++)
            {
	            if (urlComponents[componentIndex] is null) {
		            UnityEngine.Debug.LogError("Null component passed into URL encoder");
	            }
                url.Append("/");
                if (type == PNOperationType.PNPublishOperation && componentIndex == urlComponents.Count - 1)
                {
                    url.Append(Utility.EncodeUricomponent(urlComponents[componentIndex].ToString(), type, false, false));
                }
                else
                {
                    url.Append(Utility.EncodeUricomponent(urlComponents[componentIndex].ToString(), type, true, false));
                }
            }
            return url;
        }

        static StringBuilder AddSSLAndOrigin(bool ssl, string origin, StringBuilder url)
        {
            // Add http or https based on SSL flag
            if (ssl)
            {
                url.Append("https://");
            }
            else
            {
                url.Append("http://");
            }
            // Add Origin To The Request
            url.Append(origin);
           
            return url;
        }

        public static string SetParametersInOrder(Uri uri){
            string query = uri.Query;
                #if (ENABLE_PUBNUB_LOGGING)
                UnityEngine.Debug.Log(string.Format("query: {0}", query));
                #endif
            if(query.Contains("?"))
            {
                query = query.Substring(query.IndexOf('?') + 1);
            }
            List<string> lstQuery = new List<string>();
            foreach (string qp in Regex.Split(query, "&")){
                lstQuery.Add(qp);
                #if (ENABLE_PUBNUB_LOGGING)
                UnityEngine.Debug.Log(string.Format("qp {0}", qp));
                #endif
                
            }
            lstQuery.Sort();

            return string.Join("&", lstQuery.ToArray());
        }

        public static string GenerateSignatureAndAddToURL(PubNubUnity pnInstance, Uri uri, string url){
            if (!string.IsNullOrEmpty(pnInstance.PNConfig.SecretKey) && (pnInstance.PNConfig.SecretKey.Length > 0)) {
                string signature = "";
                string parameters = SetParametersInOrder(uri);
                #if (ENABLE_PUBNUB_LOGGING)
                UnityEngine.Debug.Log(string.Format("ordered parameters:  {0}", parameters));
                #endif

                StringBuilder stringToSign = new StringBuilder ();
                stringToSign
                    .Append (pnInstance.PNConfig.SubscribeKey)
                    .Append ('\n')
                    .Append (pnInstance.PNConfig.PublishKey)
                    .Append ('\n')
                    .Append (url)
                    .Append ('\n')
                    .Append (parameters);

                // Sign Message
                PubnubCrypto pubnubCrypto = new PubnubCrypto (pnInstance.PNConfig.CipherKey, pnInstance.PNLog);
                signature = pubnubCrypto.PubnubAccessManagerSign (pnInstance.PNConfig.SecretKey, stringToSign.ToString ());
                return string.Format("&signature={0}", signature);
            }
            return "";
        }

        private static StringBuilder AddQueryParams (StringBuilder url, Dictionary<string, string> queryParams){
            if(queryParams!=null){
                foreach (KeyValuePair<string, string> kvp in queryParams){
                    url.Append("&");
                    url.Append(kvp.Key);
                    url.Append("=");
                    url.Append(kvp.Value);
                }
            }
            return url;
        }

        private static string TokenOrAuthKey(PubNubUnity pnInstance){
            if(string.IsNullOrEmpty(pnInstance.Token)){
                return pnInstance.PNConfig.AuthKey;
            } else {
                return pnInstance.Token;
            }
        }
        
        private static Uri BuildRestApiRequest<T> (List<string> urlComponents, PNOperationType type, string parameters, PubNubUnity pnInstance, Dictionary<string, string> queryParams)
        {
            string uuid = pnInstance.PNConfig.UserId;
            bool ssl = pnInstance.PNConfig.Secure;
            string origin = pnInstance.PNConfig.Origin;
            int pubnubPresenceHeartbeatInSeconds = pnInstance.PNConfig.PresenceTimeout;
            string authenticationKey = TokenOrAuthKey(pnInstance);
            string pnsdkVersion = pnInstance.Version;

            StringBuilder url = new StringBuilder ();
            uuid = Utility.EncodeUricomponent (uuid, type, false, false);

            url = AddSSLAndOrigin(ssl, origin, url);

            string urlComponentsEncoded = EncodeURL(urlComponents, type).ToString();

            url.Append(urlComponentsEncoded);            

            switch (type) {
            case PNOperationType.PNLeaveOperation:
            case PNOperationType.PNSubscribeOperation:
            case PNOperationType.PNPresenceOperation:
            case PNOperationType.PNPresenceHeartbeatOperation:

                url = AppendUUIDToURL(url, uuid, true);
                url.Append(parameters);
                url = AppendAuthKeyToURL(url, authenticationKey, type);

                url = AppendPresenceHeartbeatToURL(url, pubnubPresenceHeartbeatInSeconds);
                url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
                break;
            case PNOperationType.PNGetStateOperation:
            case PNOperationType.PNPublishOperation:
            case PNOperationType.PNSignalOperation:
            case PNOperationType.PNAddMessageActionsOperation:
            case PNOperationType.PNGetMessageActionsOperation:
            case PNOperationType.PNRemoveMessageActionsOperation:

                url = AppendUUIDToURL(url, uuid, true);
                url.Append (parameters);
                url = AppendAuthKeyToURL(url, authenticationKey, type);
                url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
                break;

            case PNOperationType.PNGetUUIDMetadataOperation:
            case PNOperationType.PNGetChannelMetadataOperation:
            case PNOperationType.PNGetAllChannelMetadataOperation:
            case PNOperationType.PNGetAllUUIDMetadataOperation:
            case PNOperationType.PNManageMembershipsOperation:
            case PNOperationType.PNManageChannelMembersOperation:
            case PNOperationType.PNGetMembershipsOperation:
            case PNOperationType.PNGetChannelMembersOperation:
            case PNOperationType.PNRemoveChannelMetadataOperation:
            case PNOperationType.PNRemoveUUIDMetadataOperation:
            case PNOperationType.PNSetChannelMetadataOperation:
            case PNOperationType.PNSetUUIDMetadataOperation:
                url = AppendUUIDToURL(url, uuid, true); 
                url.Append (parameters);
                url = AppendAuthKeyToURL(url, authenticationKey, type);
                url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
                break;

            case PNOperationType.PNWhereNowOperation:

                url = AppendUUIDToURL(url, uuid, true);
                url = AppendAuthKeyToURL(url, authenticationKey, type);
                url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
                break;
            case PNOperationType.PNPushNotificationEnabledChannelsOperation:
            case PNOperationType.PNAddPushNotificationsOnChannelsOperation:
            case PNOperationType.PNRemoveAllPushNotificationsOperation:
            case PNOperationType.PNRemovePushNotificationsFromChannelsOperation:
            case PNOperationType.PNAddChannelsToGroupOperation:
            case PNOperationType.PNRemoveChannelsFromGroupOperation:
            case PNOperationType.PNSetStateOperation:
            case PNOperationType.PNHereNowOperation:
            
                url.Append (parameters);
                url = AppendUUIDToURL (url, uuid, false);
                url = AppendAuthKeyToURL (url, authenticationKey, type);
                url = AppendPNSDKVersionToURL (url, pnsdkVersion, type);
                break;
            
            case PNOperationType.PNChannelGroupsOperation:
            case PNOperationType.PNRemoveGroupOperation:
            case PNOperationType.PNChannelsForGroupOperation:
                url.Append (parameters);
                url = AppendUUIDToURL(url, uuid, true);
                url = AppendAuthKeyToURL(url, authenticationKey, type);
                url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
                break;
            case PNOperationType.PNDeleteMessagesOperation:
                url = AppendUUIDToURL (url, uuid, true);
                url.Append (parameters);
                url = AppendAuthKeyToURL(url, authenticationKey, type);
                url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
                string urlPath = string.Format("/{0}", string.Join( "/", urlComponents.ToArray()));

                string sig = GenerateSignatureAndAddToURL(pnInstance, new Uri (url.ToString ()), urlComponentsEncoded);
                #if (ENABLE_PUBNUB_LOGGING)                
                UnityEngine.Debug.Log(string.Format("urlComponentsString : {0}\nsig: {1}\nurlPath: {2},\nurlComponentsEncoded: {3}", url, sig, urlPath, urlComponentsEncoded));
                #endif
                url = url.Append(sig);
                break;

            case PNOperationType.PNHistoryOperation:
            case PNOperationType.PNMessageCountsOperation:
            case PNOperationType.PNFetchMessagesOperation:
                url.Append (parameters);
                url = AppendAuthKeyToURL(url, authenticationKey, type);
                url = AppendUUIDToURL (url, uuid, false);
                url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
                
                break;
            case PNOperationType.PNDeleteFileOperation:
            case PNOperationType.PNDownloadFileOperation:
            case PNOperationType.PNGetFileURLOperation:
            case PNOperationType.PNListFilesOperation:
            case PNOperationType.PNSendFileOperation:
                url.Append (parameters);
                url = AppendAuthKeyToURL(url, authenticationKey, type);
                url = AppendUUIDToURL (url, uuid, false);
                url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
                break; 
                
            default:
                url = AppendUUIDToURL(url, uuid, true);
                url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
                break;
            }

            url = AppendLatencyToURL(url, type, pnInstance.Latency);
            url = AddQueryParams(url, queryParams);

            Uri requestUri = new Uri (url.ToString ());

            return requestUri;

        }
        #endregion
    }
}

