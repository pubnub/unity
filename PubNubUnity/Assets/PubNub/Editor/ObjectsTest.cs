using System;
using PubNubAPI;
using NUnit.Framework;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace PubNubAPI.Tests
{
     public class ObjectsCreateUserReq : SetUUIDMetadataRequestBuilder{

         public ObjectsCreateUserReq(PubNubUnity pn, Action<PNUUIDMetadataResult, PNStatus> callback): base(pn){
             base.Callback = callback;
         }

         public void CreatePubNubResp(object deSerializedResult, RequestState requestState){
             base.CreatePubNubResponse(deSerializedResult, requestState);
         }

     }

    [TestFixture]
    public class ObjectsTest
    {
        #if DEBUG   
        [Test]
        public void TestObjectsUserRequest ()
        {
            TestObjectsUserCommon (true, false, true);
        }

        [Test]
        public void TestObjectsUserRequestQueryParam ()
        {
            TestObjectsUserCommon (true, true, true);
        }
        
        [Test]
        public void TestObjectsUserRequestWithoutIncl ()
        {
            TestObjectsUserCommon (true, false, false);
        }

        [Test]
        public void TestObjectsUserRequestWithoutInclQueryParam ()
        {
            TestObjectsUserCommon (true, true, false);
        }

        public void TestObjectsUserCommon(bool ssl, bool sendQueryParams, bool withIncl){
            string uuid = "customuuid";

            Dictionary<string,string> queryParams = new Dictionary<string, string>();
            string queryParamString = "";
            if(sendQueryParams){
                queryParams.Add("d","f");
                queryParamString="&d=f";
            } else {
                queryParams = null;
            }

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.Secure = ssl;
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            PNUUIDMetadataInclude[] CreateUserInclude = new PNUUIDMetadataInclude[]{PNUUIDMetadataInclude.PNUUIDMetadataIncludeCustom};
            var include = (CreateUserInclude==null) ? new string[]{} : CreateUserInclude.Select(a=>a.GetDescription().ToString()).ToArray();
            string incl = string.Join(",", include);
            if(!withIncl){
                incl = "";
            }
            

            Uri uri = BuildRequests.BuildObjectsSetUUIDMetadataRequest(uuid, incl, pnUnity, queryParams);

            //https://ps.pndsn.com/v2/objects/demo/spaces?uuid=customuuid&include=custom&pnsdk=PubNub-CSharp-UnityOSX%2F4.3.0 
            string expected = string.Format ("http{0}://{1}/v2/objects/{2}/uuids/{7}?uuid={3}{4}&pnsdk={5}{6}",
                ssl?"s":"", 
                pnConfiguration.Origin, 
                EditorCommon.SubscribeKey, 
                uuid, 
                (string.IsNullOrEmpty(incl))?"":string.Format("&include={0}",incl),
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNPublishOperation, false, false),
                queryParamString,
                uuid
            );

            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);

            System.Random r = new System.Random ();
			int ran = r.Next (10000);
			string userid = "userid"  + ran;
			string name = string.Format("user name {0}", ran);
			string email = string.Format("user email {0}", ran);
            string description = string.Format("description {0}", ran);
            
			string externalID = string.Format("user externalID {0}", ran);
			string profileURL = string.Format("user profileURL {0}", ran);

			bool tresult = false;

			Dictionary<string, object> userCustom = new Dictionary<string, object>();
			userCustom.Add("usercustomkey1", "ucv2");
			userCustom.Add("usercustomkey2", "ucv2");

            PNUUIDMetadataResult pnUserResult = new PNUUIDMetadataResult();
            PNStatus pnStatus = new PNStatus();

            ObjectsCreateUserReq cb = new ObjectsCreateUserReq(pnUnity, (result, status) => {
                Debug.Log("1");
                if(status != null){
                    pnStatus = status;
                    Debug.Log(pnStatus.Error);
                    Assert.True(pnStatus.Error.Equals(false));
                    Assert.True(pnStatus.StatusCode.Equals(0), pnStatus.StatusCode.ToString());

                }
 
                if(result != null){
                    pnUserResult = result;
                    Assert.AreEqual(name, pnUserResult.Name);
                    Assert.AreEqual(email, pnUserResult.Email);
                    Assert.AreEqual(externalID, pnUserResult.ExternalID);
                    Assert.AreEqual(profileURL, pnUserResult.ProfileURL);
                    Assert.AreEqual(userid, pnUserResult.ID);
                    Assert.True(!string.IsNullOrEmpty(pnUserResult.ETag), pnUserResult.ETag);
                    // Assert.True("ucv2" == pnUserResult.Custom["usercustomkey1"].ToString());
                    // Assert.True("ucv2" == pnUserResult.Custom["usercustomkey2"].ToString());
                    tresult = true;
                }

            });

            //{"status":200,"data":{"id":"{0}","name":"{1}}","externalId":"{2}","profileUrl":"{3}","email":"{4}","created":"2019-10-25T10:52:58.366074Z","updated":"2019-10-25T10:52:58.366074Z","eTag":"AdnSjuyx7KmDngE"}}      
            //"{\"status\":200,\"data\":{\"id\":{0},\"name\":{1},\"description\":\"description 935\",\"created\":\"2019-10-28T09:44:53.003174Z\",\"updated\":\"2019-10-28T09:44:53.003174Z\",\"eTag\":\"Ab/nhqOsxJr2PQ\"}}"
            string jsonString = string.Format("{{\"status\":200,\"data\":{{\"id\":\"{0}\",\"name\":\"{1}\",\"externalId\":\"{2}\",\"profileUrl\":\"{3}\",\"email\":\"{4}\",\"created\":\"2019-10-25T10:52:58.366074Z\",\"updated\":\"2019-10-25T10:52:58.366074Z\",\"eTag\":\"AdnSjuyx7KmDngE\"}}}}", userid, name, externalID, profileURL, email);
            object deSerializedResult = pnUnity.JsonLibrary.DeserializeToObject (jsonString);
            cb.CreatePubNubResp(deSerializedResult, null);            
            Debug.Log("2");
            Assert.That(() => tresult, Is.True.After(100), "SetUUIDMetadata didn't return");            

        }

        [Test]
        public void TestObjectsSpaceRequest ()
        {
            TestObjectsSpaceCommon (true, false, true);
        }

        [Test]
        public void TestObjectsSpaceRequestQueryParam ()
        {
            TestObjectsSpaceCommon (true, true, true);
        }

        [Test]
        public void TestObjectsSpaceRequestWithoutIncl ()
        {
            TestObjectsSpaceCommon (true, false, false);
        }

        [Test]
        public void TestObjectsSpaceRequestWithoutInclQueryParam ()
        {
            TestObjectsSpaceCommon (true, true, false);
        }

        public void TestObjectsSpaceCommon(bool ssl, bool sendQueryParams, bool withIncl){
            string uuid = "customuuid";

            Dictionary<string,string> queryParams = new Dictionary<string, string>();
            string queryParamString = "";
            if(sendQueryParams){
                queryParams.Add("d","f");
                queryParamString="&d=f";
            } else {
                queryParams = null;
            }

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.Secure = ssl;
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);


            PNUUIDMetadataInclude[] CreateUserInclude = new PNUUIDMetadataInclude[]{PNUUIDMetadataInclude.PNUUIDMetadataIncludeCustom};
            var include = (CreateUserInclude==null) ? new string[]{} : CreateUserInclude.Select(a=>a.GetDescription().ToString()).ToArray();
            string incl = string.Join(",", include);
            if(!withIncl){
                incl = "";
            }
            string ch = "ch";


            Uri uri = BuildRequests.BuildObjectsSetChannelMetadataRequest (ch, incl, pnUnity, queryParams);

            //https://ps.pndsn.com/v2/objects/demo/spaces?uuid=customuuid&include=custom&pnsdk=PubNub-CSharp-UnityOSX%2F4.3.0 
            string expected = string.Format ("http{0}://{1}/v2/objects/{2}/channels/{7}?uuid={3}{4}&pnsdk={5}{6}",
                ssl?"s":"", 
                pnConfiguration.Origin, 
                EditorCommon.SubscribeKey, 
                uuid, 
                (string.IsNullOrEmpty(incl))?"":string.Format("&include={0}",incl),
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNPublishOperation, false, false),
                queryParamString,
                ch
            );

            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }

        [Test]
        public void TestObjectsDelSpaceRequest ()
        {
            TestObjectsDelSpaceCommon (true, false);
        }

        [Test]
        public void TestObjectsDelSpaceRequestQueryParam ()
        {
            TestObjectsDelSpaceCommon (true, true);
        }

        public void TestObjectsDelSpaceCommon(bool ssl, bool sendQueryParams){
            string channel = EditorCommon.GetRandomChannelName();
            string uuid = "customuuid";

            Dictionary<string,string> queryParams = new Dictionary<string, string>();
            string queryParamString = "";
            if(sendQueryParams){
                queryParams.Add("d","f");
                queryParamString="&d=f";
            } else {
                queryParams = null;
            }

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.Secure = ssl;
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            Uri uri = BuildRequests.BuildObjectsDeleteChannelMetadataRequest (channel, pnUnity, queryParams);

            //https://ps.pndsn.com/v2/objects/demo/spaces/UnityUnitTests_86?uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX%2F4.3.0 
            string expected = string.Format ("http{0}://{1}/v2/objects/{2}/channels/{4}?uuid={3}&pnsdk={5}{6}",
                ssl?"s":"", 
                pnConfiguration.Origin, 
                EditorCommon.SubscribeKey, 
                uuid, 
                channel,
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNPublishOperation, false, false),
                queryParamString
            );

            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }

        [Test]
        public void TestObjectsDelUserRequest ()
        {
            TestObjectsDelUserCommon (true, false);
        }

        [Test]
        public void TestObjectsDelUsereRequestQueryParam ()
        {
            TestObjectsDelUserCommon (true, true);
        }

        public void TestObjectsDelUserCommon(bool ssl, bool sendQueryParams){
            string channel = EditorCommon.GetRandomChannelName();
            string uuid = "customuuid";

            Dictionary<string,string> queryParams = new Dictionary<string, string>();
            string queryParamString = "";
            if(sendQueryParams){
                queryParams.Add("d","f");
                queryParamString="&d=f";
            } else {
                queryParams = null;
            }

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.Secure = ssl;
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            Uri uri = BuildRequests.BuildObjectsDeleteUUIDMetadataRequest (channel, pnUnity, queryParams);

            //https://ps.pndsn.com/v2/objects/demo/spaces/UnityUnitTests_86?uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX%2F4.3.0 
            string expected = string.Format ("http{0}://{1}/v2/objects/{2}/uuids/{4}?uuid={3}&pnsdk={5}{6}",
                ssl?"s":"", 
                pnConfiguration.Origin, 
                EditorCommon.SubscribeKey, 
                uuid, 
                channel,
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNPublishOperation, false, false),
                queryParamString
            );

            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }

        [Test]
        public void TestObjectsUpdUserRequest ()
        {
            TestObjectsUpdUserCommon (true, false, true);
        }

        [Test]
        public void TestObjectsUpdUserRequestQueryParam ()
        {
            TestObjectsUpdUserCommon (true, true, true);
        }
        
        [Test]
        public void TestObjectsUpdUserRequestWithoutIncl ()
        {
            TestObjectsUpdUserCommon (true, false, false);
        }

        [Test]
        public void TestObjectsUpdUserRequestWithoutInclQueryParam ()
        {
            TestObjectsUpdUserCommon (true, true, false);
        }        

        public void TestObjectsUpdUserCommon(bool ssl, bool sendQueryParams, bool withIncl){
            string channel = EditorCommon.GetRandomChannelName();
            string uuid = "customuuid";

            Dictionary<string,string> queryParams = new Dictionary<string, string>();
            string queryParamString = "";
            if(sendQueryParams){
                queryParams.Add("d","f");
                queryParamString="&d=f";
            } else {
                queryParams = null;
            }

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.Secure = ssl;
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            PNUUIDMetadataInclude[] CreateUserInclude = new PNUUIDMetadataInclude[]{PNUUIDMetadataInclude.PNUUIDMetadataIncludeCustom};
            var include = (CreateUserInclude==null) ? new string[]{} : CreateUserInclude.Select(a=>a.GetDescription().ToString()).ToArray();
            string incl = string.Join(",", include);
            if(!withIncl){
                incl = "";
            }

            Uri uri = BuildRequests.BuildObjectsSetUUIDMetadataRequest (channel, incl, pnUnity, queryParams);

            //https://ps.pndsn.com/v2/objects/demo/spaces/UnityUnitTests_86?uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX%2F4.3.0 
            string expected = string.Format ("http{0}://{1}/v2/objects/{2}/uuids/{4}?uuid={3}{7}&pnsdk={5}{6}",
                ssl?"s":"", 
                pnConfiguration.Origin, 
                EditorCommon.SubscribeKey, 
                uuid, 
                channel,                
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNPublishOperation, false, false),
                queryParamString,
                (string.IsNullOrEmpty(incl))?"":string.Format("&include={0}",incl)
            );

            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }

        [Test]
        public void TestObjectsUpdSpaceRequest ()
        {
            TestObjectsUpdSpaceCommon (true, false, true);
        }

        [Test]
        public void TestObjectsUpdSpaceRequestQueryParam ()
        {
            TestObjectsUpdSpaceCommon (true, true, true);
        }

        [Test]
        public void TestObjectsUpdSpaceRequestWithoutIncl ()
        {
            TestObjectsUpdSpaceCommon (true, false, false);
        }

        [Test]
        public void TestObjectsUpdSpaceRequestWithoutInclQueryParam ()
        {
            TestObjectsUpdSpaceCommon (true, true, false);
        }

        public void TestObjectsUpdSpaceCommon(bool ssl, bool sendQueryParams, bool withIncl){
            string channel = EditorCommon.GetRandomChannelName();
            string uuid = "customuuid";

            Dictionary<string,string> queryParams = new Dictionary<string, string>();
            string queryParamString = "";
            if(sendQueryParams){
                queryParams.Add("d","f");
                queryParamString="&d=f";
            } else {
                queryParams = null;
            }

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.Secure = ssl;
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            PNUUIDMetadataInclude[] CreateUserInclude = new PNUUIDMetadataInclude[]{PNUUIDMetadataInclude.PNUUIDMetadataIncludeCustom};
            var include = (CreateUserInclude==null) ? new string[]{} : CreateUserInclude.Select(a=>a.GetDescription().ToString()).ToArray();
            string incl = string.Join(",", include);
            if(!withIncl){
                incl = "";
            }

            Uri uri = BuildRequests.BuildObjectsSetChannelMetadataRequest (channel, incl, pnUnity, queryParams);

            //https://ps.pndsn.com/v2/objects/demo/spaces/UnityUnitTests_86?uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX%2F4.3.0 
            string expected = string.Format ("http{0}://{1}/v2/objects/{2}/channels/{4}?uuid={3}{7}&pnsdk={5}{6}",
                ssl?"s":"", 
                pnConfiguration.Origin, 
                EditorCommon.SubscribeKey, 
                uuid, 
                channel,                
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNPublishOperation, false, false),
                queryParamString,
                (string.IsNullOrEmpty(incl))?"":string.Format("&include={0}",incl)
            );

            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }

        [Test]
        public void TestObjectsGetSpaceRequest ()
        {
            TestObjectsGetSpaceCommon (true, false, true);
        }

        [Test]
        public void TestObjectsGetSpaceRequestQueryParam ()
        {
            TestObjectsGetSpaceCommon (true, true, true);
        }
        [Test]
        public void TestObjectsGetSpaceRequestWithoutIncl ()
        {
            TestObjectsGetSpaceCommon (true, false, false);
        }

        [Test]
        public void TestObjectsGetSpaceRequestWithoutInclQueryParam ()
        {
            TestObjectsGetSpaceCommon (true, true, false);
        }        

        public void TestObjectsGetSpaceCommon(bool ssl, bool sendQueryParams, bool withIncl){
            string channel = EditorCommon.GetRandomChannelName();
            string uuid = "customuuid";

            Dictionary<string,string> queryParams = new Dictionary<string, string>();
            string queryParamString = "";
            if(sendQueryParams){
                queryParams.Add("d","f");
                queryParamString="&d=f";
            } else {
                queryParams = null;
            }

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.Secure = ssl;
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            PNUUIDMetadataInclude[] CreateUserInclude = new PNUUIDMetadataInclude[]{PNUUIDMetadataInclude.PNUUIDMetadataIncludeCustom};
            var include = (CreateUserInclude==null) ? new string[]{} : CreateUserInclude.Select(a=>a.GetDescription().ToString()).ToArray();
            string incl = string.Join(",", include);
            if(!withIncl){
                incl = "";
            }

            Uri uri = BuildRequests.BuildObjectsGetChannelMetadataRequest (channel, incl, pnUnity, queryParams);

            //https://ps.pndsn.com/v2/objects/demo/spaces/UnityUnitTests_86?uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX%2F4.3.0 
            string expected = string.Format ("http{0}://{1}/v2/objects/{2}/channels/{4}?uuid={3}{7}&pnsdk={5}{6}",
                ssl?"s":"", 
                pnConfiguration.Origin, 
                EditorCommon.SubscribeKey, 
                uuid, 
                channel,                
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNPublishOperation, false, false),
                queryParamString,
                (string.IsNullOrEmpty(incl))?"":string.Format("&include={0}",incl)
            );

            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }

        [Test]
        public void TestObjectsGetUserRequest ()
        {
            TestObjectsGetUserCommon (true, false, true);
        }

        [Test]
        public void TestObjectsGetUserRequestQueryParam ()
        {
            TestObjectsGetUserCommon (true, true, true);
        }
        [Test]
        public void TestObjectsGetUserRequestWithoutIncl ()
        {
            TestObjectsGetUserCommon (true, false, false);
        }

        [Test]
        public void TestObjectsGetUserRequestWithoutInclQueryParam ()
        {
            TestObjectsGetUserCommon (true, true, false);
        }

        public void TestObjectsGetUserCommon(bool ssl, bool sendQueryParams, bool withIncl){
            string channel = EditorCommon.GetRandomChannelName();
            string uuid = "customuuid";

            Dictionary<string,string> queryParams = new Dictionary<string, string>();
            string queryParamString = "";
            if(sendQueryParams){
                queryParams.Add("d","f");
                queryParamString="&d=f";
            } else {
                queryParams = null;
            }

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.Secure = ssl;
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            PNUUIDMetadataInclude[] CreateUserInclude = new PNUUIDMetadataInclude[]{PNUUIDMetadataInclude.PNUUIDMetadataIncludeCustom};
            var include = (CreateUserInclude==null) ? new string[]{} : CreateUserInclude.Select(a=>a.GetDescription().ToString()).ToArray();
            string incl = string.Join(",", include);
            if(!withIncl){
                incl = "";
            }

            Uri uri = BuildRequests.BuildObjectsGetUUIDMetadataRequest (channel, incl, pnUnity, queryParams);

            //https://ps.pndsn.com/v2/objects/demo/spaces/UnityUnitTests_86?uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX%2F4.3.0 
            string expected = string.Format ("http{0}://{1}/v2/objects/{2}/uuids/{4}?uuid={3}{7}&pnsdk={5}{6}",
                ssl?"s":"", 
                pnConfiguration.Origin, 
                EditorCommon.SubscribeKey, 
                uuid, 
                channel,                
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNPublishOperation, false, false),
                queryParamString,
                (string.IsNullOrEmpty(incl))?"":string.Format("&include={0}",incl)
            );

            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }

        [Test]
        public void TestObjectsGetUsersRequest1 ()
        {
            TestObjectsGetUsersCommon (true, false, true, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam2 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetUsersRequestWOIncl3 ()
        {
            TestObjectsGetUsersCommon (true, false, false, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetUsersRequestWOInclQueryParam4 ()
        {
            TestObjectsGetUsersCommon (true, true, false, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetUsersRequest5 ()
        {
            TestObjectsGetUsersCommon (true, false, true, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam6 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetUsersWOInclRequest7 ()
        {
            TestObjectsGetUsersCommon (true, false, false, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetUsersRequestWOInclQueryParam8 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetUsersRequest9 ()
        {
            TestObjectsGetUsersCommon (true, false, true, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam10 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsGetUsersRequest11 ()
        {
            TestObjectsGetUsersCommon (true, false, false, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam12 ()
        {
            TestObjectsGetUsersCommon (true, true, false, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsGetUsersRequest13 ()
        {
            TestObjectsGetUsersCommon (true, false, true, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam14 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsGetUsersRequest15 ()
        {
            TestObjectsGetUsersCommon (true, false, false, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam16 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsGetUsersRequest17 ()
        {
            TestObjectsGetUsersCommon (true, false, true, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam18 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsGetUsersRequest19 ()
        {
            TestObjectsGetUsersCommon (true, false, false, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam20 ()
        {
            TestObjectsGetUsersCommon (true, true, false, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsGetUsersRequest21 ()
        {
            TestObjectsGetUsersCommon (true, false, true, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam22 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsGetUsersRequest23 ()
        {
            TestObjectsGetUsersCommon (true, false, false, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam24 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsGetUsersRequest25 ()
        {
            TestObjectsGetUsersCommon (true, false, true, 100, "", "", true);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam26 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 100, "", "", true);
        }

        [Test]
        public void TestObjectsGetUsersRequest27 ()
        {
            TestObjectsGetUsersCommon (true, false, false, 100, "", "", true);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam28 ()
        {
            TestObjectsGetUsersCommon (true, true, false, 100, "", "", true);
        }

        [Test]
        public void TestObjectsGetUsersRequest29 ()
        {
            TestObjectsGetUsersCommon (true, false, true, 0, "", "", true);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam30 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 0, "", "", true);
        }

        [Test]
        public void TestObjectsGetUsersRequest31 ()
        {
            TestObjectsGetUsersCommon (true, false, false, 0, "", "", true);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam32 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 0, "", "", true);
        }

        [Test]
        public void TestObjectsGetUsersRequest33 ()
        {
            TestObjectsGetUsersCommon (true, false, true, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam34 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetUsersRequest35 ()
        {
            TestObjectsGetUsersCommon (true, false, false, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam36 ()
        {
            TestObjectsGetUsersCommon (true, true, false, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetUsersRequest37 ()
        {
            TestObjectsGetUsersCommon (true, false, true, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam38 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetUsersRequest39 ()
        {
            TestObjectsGetUsersCommon (true, false, false, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam40 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetUsersRequest41 ()
        {
            TestObjectsGetUsersCommon (true, false, true, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam42 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsGetUsersRequest43 ()
        {
            TestObjectsGetUsersCommon (true, false, false, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam44 ()
        {
            TestObjectsGetUsersCommon (true, true, false, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsGetUsersRequest45 ()
        {
            TestObjectsGetUsersCommon (true, false, true, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam46 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsGetUsersRequest47 ()
        {
            TestObjectsGetUsersCommon (true, false, false, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam48 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsGetUsersRequest49 ()
        {
            TestObjectsGetUsersCommon (true, false, true, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam50()
        {
            TestObjectsGetUsersCommon (true, true, true, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsGetUsersRequest51 ()
        {
            TestObjectsGetUsersCommon (true, false, false, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam52 ()
        {
            TestObjectsGetUsersCommon (true, true, false, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsGetUsersRequest53 ()
        {
            TestObjectsGetUsersCommon (true, false, true, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam54 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsGetUsersRequest55 ()
        {
            TestObjectsGetUsersCommon (true, false, false, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam56 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsGetUsersRequest57 ()
        {
            TestObjectsGetUsersCommon (true, false, true, 100, "", "", false);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam58 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 100, "", "", false);
        }

        [Test]
        public void TestObjectsGetUsersRequest59 ()
        {
            TestObjectsGetUsersCommon (true, false, false, 100, "", "", false);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam60 ()
        {
            TestObjectsGetUsersCommon (true, true, false, 100, "", "", false);
        }

        [Test]
        public void TestObjectsGetUsersRequest61 ()
        {
            TestObjectsGetUsersCommon (true, false, true, 0, "", "", false);
        }

        [Test]
        public void TestObjectsGetUsersRequestQueryParam62 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 0, "", "", false);
        }

        [Test]
        public void TestObjectsGetUsersRequest63 ()
        {
            TestObjectsGetUsersCommon (true, false, false, 0, "", "", false);
        }

         [Test]
        public void TestObjectsGetUsersRequestQueryParam64 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 0, "", "", false);
        }
        
        public void TestObjectsGetUsersCommon(bool ssl, bool sendQueryParams, bool withIncl,  int limit, string start, string end, bool count){
            TestObjectsGetUsersCommon(ssl, sendQueryParams, withIncl, limit, start, end, count, false, false);
        }
        
        [Test]
        public void TestObjectsGetUsersRequestWithFilterQueryParam64 ()
        {
            TestObjectsGetUsersCommon (true, true, true, 0, "", "", false, true, false);
        }

        [Test]
        public void TestObjectsGetUsersRequestWithSortQueryParam65()
        {
            TestObjectsGetUsersCommon(true, true, true, 0, "", "", false, false, true);
        }

        public void TestObjectsGetUsersCommon(bool ssl, bool sendQueryParams, bool withIncl,  int limit, string start, string end, bool count, bool withFilter, bool withSort){
            string uuid = "customuuid";

            Dictionary<string,string> queryParams = new Dictionary<string, string>();
            string queryParamString = "";
            if(sendQueryParams){
                queryParams.Add("d","f");
                queryParamString="&d=f";
            } else {
                queryParams = null;
            }

            string filter = "";
            if(withFilter){
                filter = "name like 'abc - / s*'";
            }

            string sortBy = "";
            if (withSort){
                sortBy = "name:desc";
            }

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.Secure = ssl;
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            PNUUIDMetadataInclude[] CreateUserInclude = new PNUUIDMetadataInclude[]{PNUUIDMetadataInclude.PNUUIDMetadataIncludeCustom};
            var include = (CreateUserInclude==null) ? new string[]{} : CreateUserInclude.Select(a=>a.GetDescription().ToString()).ToArray();
            string incl = string.Join(",", include);            
            if(!withIncl){
                incl = "";
            }

            Uri uri = BuildRequests.BuildObjectsGetAllUUIDMetadataRequest (limit, start, end, count, incl, pnUnity, queryParams, filter, sortBy);

            //https://ps.pndsn.com/v2/objects/demo/spaces/UnityUnitTests_86?uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX%2F4.3.0 
            string expected = string.Format ("http{0}://{1}/v2/objects/{2}/uuids?uuid={3}{7}{10}{11}{4}{8}{9}{12}&pnsdk={5}{6}",
                ssl?"s":"", 
                pnConfiguration.Origin, 
                EditorCommon.SubscribeKey, 
                uuid, 
                (limit>0)?string.Format("&limit={0}", limit.ToString()):"",
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNPublishOperation, false, false),
                queryParamString,
                (string.IsNullOrEmpty(incl))?"":string.Format("&include={0}",incl),
                (string.IsNullOrEmpty(start))?"":string.Format("&start={0}",start),
                (string.IsNullOrEmpty(end))?"":string.Format("&end={0}",end),
                (count) ? "&count=1" : "&count=0",
                (withFilter)?"&filter=name%20like%20%27abc%20-%20%2F%20s%2A%27":"",
                (withSort)?"&sort=name%3Adesc":""
            );

            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }

        [Test]
        public void TestObjectsGetSpacesRequest1 ()
        {
            TestObjectsGetSpacesCommon (true, false, true, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam2 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequestWOIncl3 ()
        {
            TestObjectsGetSpacesCommon (true, false, false, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequestWOInclQueryParam4 ()
        {
            TestObjectsGetSpacesCommon (true, true, false, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequest5 ()
        {
            TestObjectsGetSpacesCommon (true, false, true, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam6 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetSpacesWOInclRequest7 ()
        {
            TestObjectsGetSpacesCommon (true, false, false, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequestWOInclQueryParam8 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequest9 ()
        {
            TestObjectsGetSpacesCommon (true, false, true, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam10 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequest11 ()
        {
            TestObjectsGetSpacesCommon (true, false, false, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam12 ()
        {
            TestObjectsGetSpacesCommon (true, true, false, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequest13 ()
        {
            TestObjectsGetSpacesCommon (true, false, true, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam14 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequest15 ()
        {
            TestObjectsGetSpacesCommon (true, false, false, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam16 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequest17 ()
        {
            TestObjectsGetSpacesCommon (true, false, true, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam18 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequest19 ()
        {
            TestObjectsGetSpacesCommon (true, false, false, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam20 ()
        {
            TestObjectsGetSpacesCommon (true, true, false, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequest21 ()
        {
            TestObjectsGetSpacesCommon (true, false, true, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam22 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequest23 ()
        {
            TestObjectsGetSpacesCommon (true, false, false, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam24 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequest25 ()
        {
            TestObjectsGetSpacesCommon (true, false, true, 100, "", "", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam26 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 100, "", "", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequest27 ()
        {
            TestObjectsGetSpacesCommon (true, false, false, 100, "", "", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam28 ()
        {
            TestObjectsGetSpacesCommon (true, true, false, 100, "", "", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequest29 ()
        {
            TestObjectsGetSpacesCommon (true, false, true, 0, "", "", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam30 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 0, "", "", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequest31 ()
        {
            TestObjectsGetSpacesCommon (true, false, false, 0, "", "", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam32 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 0, "", "", true);
        }

        [Test]
        public void TestObjectsGetSpacesRequest33 ()
        {
            TestObjectsGetSpacesCommon (true, false, true, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam34 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequest35 ()
        {
            TestObjectsGetSpacesCommon (true, false, false, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam36 ()
        {
            TestObjectsGetSpacesCommon (true, true, false, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequest37 ()
        {
            TestObjectsGetSpacesCommon (true, false, true, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam38 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequest39 ()
        {
            TestObjectsGetSpacesCommon (true, false, false, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam40 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequest41 ()
        {
            TestObjectsGetSpacesCommon (true, false, true, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam42 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequest43 ()
        {
            TestObjectsGetSpacesCommon (true, false, false, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam44 ()
        {
            TestObjectsGetSpacesCommon (true, true, false, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequest45 ()
        {
            TestObjectsGetSpacesCommon (true, false, true, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam46 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequest47 ()
        {
            TestObjectsGetSpacesCommon (true, false, false, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam48 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequest49 ()
        {
            TestObjectsGetSpacesCommon (true, false, true, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam50()
        {
            TestObjectsGetSpacesCommon (true, true, true, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequest51 ()
        {
            TestObjectsGetSpacesCommon (true, false, false, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam52 ()
        {
            TestObjectsGetSpacesCommon (true, true, false, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequest53 ()
        {
            TestObjectsGetSpacesCommon (true, false, true, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam54 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequest55 ()
        {
            TestObjectsGetSpacesCommon (true, false, false, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam56 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequest57 ()
        {
            TestObjectsGetSpacesCommon (true, false, true, 100, "", "", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam58 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 100, "", "", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequest59 ()
        {
            TestObjectsGetSpacesCommon (true, false, false, 100, "", "", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam60 ()
        {
            TestObjectsGetSpacesCommon (true, true, false, 100, "", "", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequest61 ()
        {
            TestObjectsGetSpacesCommon (true, false, true, 0, "", "", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam62 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 0, "", "", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequest63 ()
        {
            TestObjectsGetSpacesCommon (true, false, false, 0, "", "", false);
        }

        [Test]
        public void TestObjectsGetSpacesRequestQueryParam64 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 0, "", "", false);
        }
        
        [Test]
        public void TestObjectsGetSpacesRequestWithFilterQueryParam64 ()
        {
            TestObjectsGetSpacesCommon (true, true, true, 0, "", "", false, true, false);
        }
        
        [Test]
        public void TestObjectsGetSpacesRequestWithSortQueryParam65()
        {
            TestObjectsGetSpacesCommon(true, true, true, 0, "", "", false, false, true);
        }

        public void TestObjectsGetSpacesCommon(bool ssl, bool sendQueryParams, bool withIncl,  int limit, string start, string end, bool count){
            TestObjectsGetSpacesCommon(ssl, sendQueryParams, withIncl, limit, start, end, count, false, false);
        }

        public void TestObjectsGetSpacesCommon(bool ssl, bool sendQueryParams, bool withIncl, int limit, string start, string end, bool count, bool withFilter, bool withSort)
        {            
            string uuid = "customuuid";

            Dictionary<string,string> queryParams = new Dictionary<string, string>();
            string queryParamString = "";
            if(sendQueryParams){
                queryParams.Add("d","f");
                queryParamString="&d=f";
            } else {
                queryParams = null;
            }

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.Secure = ssl;
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            PNUUIDMetadataInclude[] CreateUserInclude = new PNUUIDMetadataInclude[]{PNUUIDMetadataInclude.PNUUIDMetadataIncludeCustom};
            var include = (CreateUserInclude==null) ? new string[]{} : CreateUserInclude.Select(a=>a.GetDescription().ToString()).ToArray();
            string incl = string.Join(",", include);            
            if(!withIncl){
                incl = "";
            }

            string filter = "";
            if(withFilter){
                filter = "name == 'abc - / s*'";
            }
            string sortBy = "";
            if (withSort){
                sortBy = "name:desc";
            }

            Uri uri = BuildRequests.BuildObjectsGetAllChannelMetadataRequest (limit, start, end, count, incl, pnUnity, queryParams, filter, sortBy);

            //https://ps.pndsn.com/v2/objects/demo/spaces/UnityUnitTests_86?uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX%2F4.3.0 
            string expected = string.Format ("http{0}://{1}/v2/objects/{2}/channels?uuid={3}{7}{10}{11}{4}{8}{9}{12}&pnsdk={5}{6}",
                ssl?"s":"", 
                pnConfiguration.Origin, 
                EditorCommon.SubscribeKey, 
                uuid, 
                (limit>0)?string.Format("&limit={0}", limit.ToString()):"",
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNPublishOperation, false, false),
                queryParamString,
                (string.IsNullOrEmpty(incl))?"":string.Format("&include={0}",incl),
                (string.IsNullOrEmpty(start))?"":string.Format("&start={0}",start),
                (string.IsNullOrEmpty(end))?"":string.Format("&end={0}",end),
                (count) ? "&count=1" : "&count=0",
                (withFilter)?"&filter=name%20%3D%3D%20%27abc%20-%20%2F%20s%2A%27":"",
                (withSort) ? "&sort=name%3Adesc" : ""
            );

            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }

       [Test]
        public void TestObjectsGetMembersRequest1 ()
        {
            TestObjectsGetMembersCommon (true, false, true, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam2 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetMembersRequestWOIncl3 ()
        {
            TestObjectsGetMembersCommon (true, false, false, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetMembersRequestWOInclQueryParam4 ()
        {
            TestObjectsGetMembersCommon (true, true, false, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetMembersRequest5 ()
        {
            TestObjectsGetMembersCommon (true, false, true, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam6 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetMembersWOInclRequest7 ()
        {
            TestObjectsGetMembersCommon (true, false, false, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetMembersRequestWOInclQueryParam8 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetMembersRequest9 ()
        {
            TestObjectsGetMembersCommon (true, false, true, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam10 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsGetMembersRequest11 ()
        {
            TestObjectsGetMembersCommon (true, false, false, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam12 ()
        {
            TestObjectsGetMembersCommon (true, true, false, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsGetMembersRequest13 ()
        {
            TestObjectsGetMembersCommon (true, false, true, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam14 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsGetMembersRequest15 ()
        {
            TestObjectsGetMembersCommon (true, false, false, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam16 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsGetMembersRequest17 ()
        {
            TestObjectsGetMembersCommon (true, false, true, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam18 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsGetMembersRequest19 ()
        {
            TestObjectsGetMembersCommon (true, false, false, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam20 ()
        {
            TestObjectsGetMembersCommon (true, true, false, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsGetMembersRequest21 ()
        {
            TestObjectsGetMembersCommon (true, false, true, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam22 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsGetMembersRequest23 ()
        {
            TestObjectsGetMembersCommon (true, false, false, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam24 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsGetMembersRequest25 ()
        {
            TestObjectsGetMembersCommon (true, false, true, 100, "", "", true);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam26 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 100, "", "", true);
        }

        [Test]
        public void TestObjectsGetMembersRequest27 ()
        {
            TestObjectsGetMembersCommon (true, false, false, 100, "", "", true);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam28 ()
        {
            TestObjectsGetMembersCommon (true, true, false, 100, "", "", true);
        }

        [Test]
        public void TestObjectsGetMembersRequest29 ()
        {
            TestObjectsGetMembersCommon (true, false, true, 0, "", "", true);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam30 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 0, "", "", true);
        }

        [Test]
        public void TestObjectsGetMembersRequest31 ()
        {
            TestObjectsGetMembersCommon (true, false, false, 0, "", "", true);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam32 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 0, "", "", true);
        }

        [Test]
        public void TestObjectsGetMembersRequest33 ()
        {
            TestObjectsGetMembersCommon (true, false, true, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam34 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetMembersRequest35 ()
        {
            TestObjectsGetMembersCommon (true, false, false, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam36 ()
        {
            TestObjectsGetMembersCommon (true, true, false, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetMembersRequest37 ()
        {
            TestObjectsGetMembersCommon (true, false, true, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam38 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetMembersRequest39 ()
        {
            TestObjectsGetMembersCommon (true, false, false, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam40 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetMembersRequest41 ()
        {
            TestObjectsGetMembersCommon (true, false, true, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam42 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsGetMembersRequest43 ()
        {
            TestObjectsGetMembersCommon (true, false, false, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam44 ()
        {
            TestObjectsGetMembersCommon (true, true, false, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsGetMembersRequest45 ()
        {
            TestObjectsGetMembersCommon (true, false, true, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam46 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsGetMembersRequest47 ()
        {
            TestObjectsGetMembersCommon (true, false, false, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam48 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsGetMembersRequest49 ()
        {
            TestObjectsGetMembersCommon (true, false, true, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam50()
        {
            TestObjectsGetMembersCommon (true, true, true, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsGetMembersRequest51 ()
        {
            TestObjectsGetMembersCommon (true, false, false, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam52 ()
        {
            TestObjectsGetMembersCommon (true, true, false, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsGetMembersRequest53 ()
        {
            TestObjectsGetMembersCommon (true, false, true, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam54 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsGetMembersRequest55 ()
        {
            TestObjectsGetMembersCommon (true, false, false, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam56 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsGetMembersRequest57 ()
        {
            TestObjectsGetMembersCommon (true, false, true, 100, "", "", false);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam58 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 100, "", "", false);
        }

        [Test]
        public void TestObjectsGetMembersRequest59 ()
        {
            TestObjectsGetMembersCommon (true, false, false, 100, "", "", false);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam60 ()
        {
            TestObjectsGetMembersCommon (true, true, false, 100, "", "", false);
        }

        [Test]
        public void TestObjectsGetMembersRequest61 ()
        {
            TestObjectsGetMembersCommon (true, false, true, 0, "", "", false);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam62 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 0, "", "", false);
        }

        [Test]
        public void TestObjectsGetMembersRequest63 ()
        {
            TestObjectsGetMembersCommon (true, false, false, 0, "", "", false);
        }

        [Test]
        public void TestObjectsGetMembersRequestQueryParam64 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 0, "", "", false);
        }
        

        public void TestObjectsGetMembersCommon(bool ssl, bool sendQueryParams, bool withIncl,  int limit, string start, string end, bool count){
            TestObjectsGetMembersCommon(ssl, sendQueryParams, withIncl, limit, start, end, count, false,false);
        }         

        [Test]
        public void TestObjectsGetMembersRequestWithFilterQueryParam64 ()
        {
            TestObjectsGetMembersCommon (true, true, true, 0, "", "", false, true,false);
        }

        [Test]
        public void TestObjectsGetMembersRequestWithSortQueryParam65()
        {
            TestObjectsGetMembersCommon(true, true, true, 0, "", "", false, true, true);
        }


        public void TestObjectsGetMembersCommon(bool ssl, bool sendQueryParams, bool withIncl, int limit, string start, string end, bool count, bool withFilter, bool withSort){            
            string uuid = "customuuid";

            Dictionary<string,string> queryParams = new Dictionary<string, string>();
            string queryParamString = "";
            if(sendQueryParams){
                queryParams.Add("d","f");
                queryParamString="&d=f";
            } else {
                queryParams = null;
            }

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.Secure = ssl;
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            PNUUIDMetadataInclude[] CreateUserInclude = new PNUUIDMetadataInclude[]{PNUUIDMetadataInclude.PNUUIDMetadataIncludeCustom};
            var include = (CreateUserInclude==null) ? new string[]{} : CreateUserInclude.Select(a=>a.GetDescription().ToString()).ToArray();
            string incl = string.Join(",", include);            
            if(!withIncl){
                incl = "";
            }
            string ch = "ch";

            string filter = "";
            if(withFilter){
                filter = "name == 'abc - / s'*";
            }

            string sortBy = "";
            if (withSort){
                sortBy = "name:desc";
            }

            Uri uri = BuildRequests.BuildObjectsGetChannelMembersRequest (ch, limit, start, end, count, incl, pnUnity, queryParams, filter, sortBy);

            //https://ps.pndsn.com/v2/objects/demo/spaces/UnityUnitTests_86?uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX%2F4.3.0 
            string expected = string.Format ("http{0}://{1}/v2/objects/{2}/channels/{11}/uuids?uuid={3}{7}{10}{12}{4}{8}{9}{13}&pnsdk={5}{6}",
                ssl?"s":"", 
                pnConfiguration.Origin, 
                EditorCommon.SubscribeKey, 
                uuid, 
                (limit>0)?string.Format("&limit={0}", limit.ToString()):"",
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNPublishOperation, false, false),
                queryParamString,
                (string.IsNullOrEmpty(incl))?"":string.Format("&include={0}",incl),
                (string.IsNullOrEmpty(start))?"":string.Format("&start={0}",start),
                (string.IsNullOrEmpty(end))?"":string.Format("&end={0}",end),
                (count) ? "&count=1" : "&count=0",
                ch,
                (withFilter)?"&filter=name%20%3D%3D%20%27abc%20-%20%2F%20s%27%2A":"",
                (withSort) ? "&sort=name%3Adesc" : ""
            );

            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest1 ()
        {
            TestObjectsGetMembershipsCommon (true, false, true, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam2 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestWOIncl3 ()
        {
            TestObjectsGetMembershipsCommon (true, false, false, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestWOInclQueryParam4 ()
        {
            TestObjectsGetMembershipsCommon (true, true, false, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest5 ()
        {
            TestObjectsGetMembershipsCommon (true, false, true, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam6 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetMembershipsWOInclRequest7 ()
        {
            TestObjectsGetMembershipsCommon (true, false, false, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestWOInclQueryParam8 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest9 ()
        {
            TestObjectsGetMembershipsCommon (true, false, true, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam10 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest11 ()
        {
            TestObjectsGetMembershipsCommon (true, false, false, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam12 ()
        {
            TestObjectsGetMembershipsCommon (true, true, false, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest13 ()
        {
            TestObjectsGetMembershipsCommon (true, false, true, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam14 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest15 ()
        {
            TestObjectsGetMembershipsCommon (true, false, false, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam16 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest17 ()
        {
            TestObjectsGetMembershipsCommon (true, false, true, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam18 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest19 ()
        {
            TestObjectsGetMembershipsCommon (true, false, false, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam20 ()
        {
            TestObjectsGetMembershipsCommon (true, true, false, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest21 ()
        {
            TestObjectsGetMembershipsCommon (true, false, true, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam22 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest23 ()
        {
            TestObjectsGetMembershipsCommon (true, false, false, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam24 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest25 ()
        {
            TestObjectsGetMembershipsCommon (true, false, true, 100, "", "", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam26 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 100, "", "", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest27 ()
        {
            TestObjectsGetMembershipsCommon (true, false, false, 100, "", "", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam28 ()
        {
            TestObjectsGetMembershipsCommon (true, true, false, 100, "", "", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest29 ()
        {
            TestObjectsGetMembershipsCommon (true, false, true, 0, "", "", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam30 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 0, "", "", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest31 ()
        {
            TestObjectsGetMembershipsCommon (true, false, false, 0, "", "", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam32 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 0, "", "", true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest33 ()
        {
            TestObjectsGetMembershipsCommon (true, false, true, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam34 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest35 ()
        {
            TestObjectsGetMembershipsCommon (true, false, false, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam36 ()
        {
            TestObjectsGetMembershipsCommon (true, true, false, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest37 ()
        {
            TestObjectsGetMembershipsCommon (true, false, true, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam38 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest39 ()
        {
            TestObjectsGetMembershipsCommon (true, false, false, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam40 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest41 ()
        {
            TestObjectsGetMembershipsCommon (true, false, true, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam42 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest43 ()
        {
            TestObjectsGetMembershipsCommon (true, false, false, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam44 ()
        {
            TestObjectsGetMembershipsCommon (true, true, false, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest45 ()
        {
            TestObjectsGetMembershipsCommon (true, false, true, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam46 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest47 ()
        {
            TestObjectsGetMembershipsCommon (true, false, false, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam48 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest49 ()
        {
            TestObjectsGetMembershipsCommon (true, false, true, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam50()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest51 ()
        {
            TestObjectsGetMembershipsCommon (true, false, false, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam52 ()
        {
            TestObjectsGetMembershipsCommon (true, true, false, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest53 ()
        {
            TestObjectsGetMembershipsCommon (true, false, true, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam54 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest55 ()
        {
            TestObjectsGetMembershipsCommon (true, false, false, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam56 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest57 ()
        {
            TestObjectsGetMembershipsCommon (true, false, true, 100, "", "", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam58 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 100, "", "", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest59 ()
        {
            TestObjectsGetMembershipsCommon (true, false, false, 100, "", "", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam60 ()
        {
            TestObjectsGetMembershipsCommon (true, true, false, 100, "", "", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest61 ()
        {
            TestObjectsGetMembershipsCommon (true, false, true, 0, "", "", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam62 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 0, "", "", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequest63 ()
        {
            TestObjectsGetMembershipsCommon (true, false, false, 0, "", "", false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestQueryParam64 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 0, "", "", false);
        }

        public void TestObjectsGetMembershipsCommon(bool ssl, bool sendQueryParams, bool withIncl, int limit, string start, string end, bool count){
            TestObjectsGetMembershipsCommon(ssl, sendQueryParams, withIncl, limit, start, end, count, false);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestWithFilterQueryParam64 ()
        {
            TestObjectsGetMembershipsCommon (true, true, true, 0, "", "", false, true);
        }

        [Test]
        public void TestObjectsGetMembershipsRequestWithSortQueryParam65()
        {
            TestObjectsGetMembershipsCommon(true, true, true, 0, "", "", false, true,true);
        }

        public void TestObjectsGetMembershipsCommon(bool ssl, bool sendQueryParams, bool withIncl, int limit, string start, string end, bool count, bool withFilter){
            TestObjectsGetMembershipsCommon(ssl,sendQueryParams,withIncl,limit,start,end,count,withFilter,false);
        }
        public void TestObjectsGetMembershipsCommon(bool ssl, bool sendQueryParams, bool withIncl, int limit, string start, string end, bool count, bool withFilter, bool withSort){            
            string uuid = "customuuid";

            Dictionary<string,string> queryParams = new Dictionary<string, string>();
            string queryParamString = "";
            if(sendQueryParams){
                queryParams.Add("d","f");
                queryParamString="&d=f";
            } else {
                queryParams = null;
            }

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.Secure = ssl;
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            PNUUIDMetadataInclude[] CreateUserInclude = new PNUUIDMetadataInclude[]{PNUUIDMetadataInclude.PNUUIDMetadataIncludeCustom};
            var include = (CreateUserInclude==null) ? new string[]{} : CreateUserInclude.Select(a=>a.GetDescription().ToString()).ToArray();
            string incl = string.Join(",", include);            
            if(!withIncl){
                incl = "";
            }
            string ch = "ch";
            
            string filter = "name == 's1236 9'";
            if(!withFilter){
                filter = "";
            }
            string sortBy = "";
            if (withSort){
                sortBy = "name:desc";
            }

            Uri uri = BuildRequests.BuildObjectsGetMembershipsRequest (ch, limit, start, end, count, incl, pnUnity, queryParams, filter, sortBy);

            //https://ps.pndsn.com/v2/objects/demo/spaces/UnityUnitTests_86?uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX%2F4.3.0 
            string expected = string.Format ("http{0}://{1}/v2/objects/{2}/uuids/{11}/channels?uuid={3}{7}{10}{12}{4}{8}{9}{13}&pnsdk={5}{6}",
                ssl?"s":"",
                pnConfiguration.Origin,
                EditorCommon.SubscribeKey,
                uuid, 
                (limit>0)?string.Format("&limit={0}", limit.ToString()):"",
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNPublishOperation, false, false),
                queryParamString,
                (string.IsNullOrEmpty(incl))?"":string.Format("&include={0}",incl),
                (string.IsNullOrEmpty(start))?"":string.Format("&start={0}",start),
                (string.IsNullOrEmpty(end))?"":string.Format("&end={0}",end),
                (count) ? "&count=1" : "&count=0",
                ch,
                (withFilter)?"&filter=name%20%3D%3D%20%27s1236%209%27":"",
                (withSort) ? "&sort=name%3Adesc" : ""
            );

            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest1 ()
        {
            TestObjectsManageMembershipsCommon (true, false, true, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam2 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestWOIncl3 ()
        {
            TestObjectsManageMembershipsCommon (true, false, false, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestWOInclQueryParam4 ()
        {
            TestObjectsManageMembershipsCommon (true, true, false, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest5 ()
        {
            TestObjectsManageMembershipsCommon (true, false, true, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam6 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsManageMembershipsWOInclRequest7 ()
        {
            TestObjectsManageMembershipsCommon (true, false, false, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestWOInclQueryParam8 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest9 ()
        {
            TestObjectsManageMembershipsCommon (true, false, true, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam10 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest11 ()
        {
            TestObjectsManageMembershipsCommon (true, false, false, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam12 ()
        {
            TestObjectsManageMembershipsCommon (true, true, false, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest13 ()
        {
            TestObjectsManageMembershipsCommon (true, false, true, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam14 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest15 ()
        {
            TestObjectsManageMembershipsCommon (true, false, false, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam16 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest17 ()
        {
            TestObjectsManageMembershipsCommon (true, false, true, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam18 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest19 ()
        {
            TestObjectsManageMembershipsCommon (true, false, false, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam20 ()
        {
            TestObjectsManageMembershipsCommon (true, true, false, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest21 ()
        {
            TestObjectsManageMembershipsCommon (true, false, true, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam22 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest23 ()
        {
            TestObjectsManageMembershipsCommon (true, false, false, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam24 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest25 ()
        {
            TestObjectsManageMembershipsCommon (true, false, true, 100, "", "", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam26 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 100, "", "", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest27 ()
        {
            TestObjectsManageMembershipsCommon (true, false, false, 100, "", "", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam28 ()
        {
            TestObjectsManageMembershipsCommon (true, true, false, 100, "", "", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest29 ()
        {
            TestObjectsManageMembershipsCommon (true, false, true, 0, "", "", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam30 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 0, "", "", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest31 ()
        {
            TestObjectsManageMembershipsCommon (true, false, false, 0, "", "", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam32 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 0, "", "", true);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest33 ()
        {
            TestObjectsManageMembershipsCommon (true, false, true, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam34 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest35 ()
        {
            TestObjectsManageMembershipsCommon (true, false, false, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam36 ()
        {
            TestObjectsManageMembershipsCommon (true, true, false, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest37 ()
        {
            TestObjectsManageMembershipsCommon (true, false, true, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam38 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest39 ()
        {
            TestObjectsManageMembershipsCommon (true, false, false, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam40 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest41 ()
        {
            TestObjectsManageMembershipsCommon (true, false, true, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam42 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest43 ()
        {
            TestObjectsManageMembershipsCommon (true, false, false, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam44 ()
        {
            TestObjectsManageMembershipsCommon (true, true, false, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest45 ()
        {
            TestObjectsManageMembershipsCommon (true, false, true, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam46 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest47 ()
        {
            TestObjectsManageMembershipsCommon (true, false, false, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam48 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest49 ()
        {
            TestObjectsManageMembershipsCommon (true, false, true, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam50()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest51 ()
        {
            TestObjectsManageMembershipsCommon (true, false, false, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam52 ()
        {
            TestObjectsManageMembershipsCommon (true, true, false, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest53 ()
        {
            TestObjectsManageMembershipsCommon (true, false, true, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam54 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest55 ()
        {
            TestObjectsManageMembershipsCommon (true, false, false, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam56 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest57 ()
        {
            TestObjectsManageMembershipsCommon (true, false, true, 100, "", "", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam58 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 100, "", "", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest59 ()
        {
            TestObjectsManageMembershipsCommon (true, false, false, 100, "", "", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam60 ()
        {
            TestObjectsManageMembershipsCommon (true, true, false, 100, "", "", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest61 ()
        {
            TestObjectsManageMembershipsCommon (true, false, true, 0, "", "", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam62 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 0, "", "", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequest63 ()
        {
            TestObjectsManageMembershipsCommon (true, false, false, 0, "", "", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestQueryParam64 ()
        {
            TestObjectsManageMembershipsCommon (true, true, true, 0, "", "", false);
        }

        [Test]
        public void TestObjectsManageMembershipsRequestSortQueryParam65()
        {
            TestObjectsManageMembershipsCommon(true, true, true, 0, "", "", false, true);
        }

        public void TestObjectsManageMembershipsCommon(bool ssl, bool sendQueryParams, bool withIncl, int limit, string start, string end, bool count){
            TestObjectsManageMembershipsCommon(ssl,sendQueryParams,withIncl,limit,start,end,count,false);
        }

        public void TestObjectsManageMembershipsCommon(bool ssl, bool sendQueryParams, bool withIncl,  int limit, string start, string end, bool count, bool withSort){
            string uuid = "customuuid";

            Dictionary<string,string> queryParams = new Dictionary<string, string>();
            string queryParamString = "";
            if(sendQueryParams){
                queryParams.Add("d","f");
                queryParamString="&d=f";
            } else {
                queryParams = null;
            }

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.Secure = ssl;
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            PNUUIDMetadataInclude[] CreateUserInclude = new PNUUIDMetadataInclude[]{PNUUIDMetadataInclude.PNUUIDMetadataIncludeCustom};
            var include = (CreateUserInclude==null) ? new string[]{} : CreateUserInclude.Select(a=>a.GetDescription().ToString()).ToArray();
            string incl = string.Join(",", include);            
            if(!withIncl){
                incl = "";
            }
            string sortBy = "";
            if (withSort){
                sortBy = "name:desc";
            }

            string ch = "ch";

            Uri uri = BuildRequests.BuildObjectsManageMembershipsRequest (ch, limit, start, end, count, incl, pnUnity, queryParams,sortBy);

            //https://ps.pndsn.com/v2/objects/demo/spaces/UnityUnitTests_86?uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX%2F4.3.0 
            string expected = string.Format ("http{0}://{1}/v2/objects/{2}/uuids/{11}/channels?uuid={3}{7}{10}{4}{8}{9}{12}&pnsdk={5}{6}",
                ssl?"s":"", 
                pnConfiguration.Origin, 
                EditorCommon.SubscribeKey, 
                uuid, 
                (limit>0)?string.Format("&limit={0}", limit.ToString()):"",
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNPublishOperation, false, false),
                queryParamString,
                (string.IsNullOrEmpty(incl))?"":string.Format("&include={0}",incl),
                (string.IsNullOrEmpty(start))?"":string.Format("&start={0}",start),
                (string.IsNullOrEmpty(end))?"":string.Format("&end={0}",end),
                (count) ? "&count=1" : "&count=0",
                ch,
                (withSort) ? "&sort=name%3Adesc" : ""
            );

            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }

        [Test]
        public void TestObjectsManageMembersRequest1 ()
        {
            TestObjectsManageMembersCommon (true, false, true, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam2 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsManageMembersRequestWOIncl3 ()
        {
            TestObjectsManageMembersCommon (true, false, false, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsManageMembersRequestWOInclQueryParam4 ()
        {
            TestObjectsManageMembersCommon (true, true, false, 100, "s", "e", true);
        }

        [Test]
        public void TestObjectsManageMembersRequest5 ()
        {
            TestObjectsManageMembersCommon (true, false, true, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam6 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsManageMembersWOInclRequest7 ()
        {
            TestObjectsManageMembersCommon (true, false, false, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsManageMembersRequestWOInclQueryParam8 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 0, "s", "e", true);
        }

        [Test]
        public void TestObjectsManageMembersRequest9 ()
        {
            TestObjectsManageMembersCommon (true, false, true, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam10 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsManageMembersRequest11 ()
        {
            TestObjectsManageMembersCommon (true, false, false, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam12 ()
        {
            TestObjectsManageMembersCommon (true, true, false, 100, "", "e", true);
        }

        [Test]
        public void TestObjectsManageMembersRequest13 ()
        {
            TestObjectsManageMembersCommon (true, false, true, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam14 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsManageMembersRequest15 ()
        {
            TestObjectsManageMembersCommon (true, false, false, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam16 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 0, "", "e", true);
        }

        [Test]
        public void TestObjectsManageMembersRequest17 ()
        {
            TestObjectsManageMembersCommon (true, false, true, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam18 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsManageMembersRequest19 ()
        {
            TestObjectsManageMembersCommon (true, false, false, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam20 ()
        {
            TestObjectsManageMembersCommon (true, true, false, 100, "s", "", true);
        }

        [Test]
        public void TestObjectsManageMembersRequest21 ()
        {
            TestObjectsManageMembersCommon (true, false, true, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam22 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsManageMembersRequest23 ()
        {
            TestObjectsManageMembersCommon (true, false, false, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam24 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 0, "s", "", true);
        }

        [Test]
        public void TestObjectsManageMembersRequest25 ()
        {
            TestObjectsManageMembersCommon (true, false, true, 100, "", "", true);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam26 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 100, "", "", true);
        }

        [Test]
        public void TestObjectsManageMembersRequest27 ()
        {
            TestObjectsManageMembersCommon (true, false, false, 100, "", "", true);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam28 ()
        {
            TestObjectsManageMembersCommon (true, true, false, 100, "", "", true);
        }

        [Test]
        public void TestObjectsManageMembersRequest29 ()
        {
            TestObjectsManageMembersCommon (true, false, true, 0, "", "", true);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam30 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 0, "", "", true);
        }

        [Test]
        public void TestObjectsManageMembersRequest31 ()
        {
            TestObjectsManageMembersCommon (true, false, false, 0, "", "", true);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam32 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 0, "", "", true);
        }

        [Test]
        public void TestObjectsManageMembersRequest33 ()
        {
            TestObjectsManageMembersCommon (true, false, true, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam34 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsManageMembersRequest35 ()
        {
            TestObjectsManageMembersCommon (true, false, false, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam36 ()
        {
            TestObjectsManageMembersCommon (true, true, false, 100, "s", "e", false);
        }

        [Test]
        public void TestObjectsManageMembersRequest37 ()
        {
            TestObjectsManageMembersCommon (true, false, true, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam38 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsManageMembersRequest39 ()
        {
            TestObjectsManageMembersCommon (true, false, false, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam40 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 0, "s", "e", false);
        }

        [Test]
        public void TestObjectsManageMembersRequest41 ()
        {
            TestObjectsManageMembersCommon (true, false, true, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam42 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsManageMembersRequest43 ()
        {
            TestObjectsManageMembersCommon (true, false, false, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam44 ()
        {
            TestObjectsManageMembersCommon (true, true, false, 100, "", "e", false);
        }

        [Test]
        public void TestObjectsManageMembersRequest45 ()
        {
            TestObjectsManageMembersCommon (true, false, true, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam46 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsManageMembersRequest47 ()
        {
            TestObjectsManageMembersCommon (true, false, false, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam48 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 0, "", "e", false);
        }

        [Test]
        public void TestObjectsManageMembersRequest49 ()
        {
            TestObjectsManageMembersCommon (true, false, true, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam50()
        {
            TestObjectsManageMembersCommon (true, true, true, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsManageMembersRequest51 ()
        {
            TestObjectsManageMembersCommon (true, false, false, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam52 ()
        {
            TestObjectsManageMembersCommon (true, true, false, 100, "s", "", false);
        }

        [Test]
        public void TestObjectsManageMembersRequest53 ()
        {
            TestObjectsManageMembersCommon (true, false, true, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam54 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsManageMembersRequest55 ()
        {
            TestObjectsManageMembersCommon (true, false, false, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam56 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 0, "s", "", false);
        }

        [Test]
        public void TestObjectsManageMembersRequest57 ()
        {
            TestObjectsManageMembersCommon (true, false, true, 100, "", "", false);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam58 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 100, "", "", false);
        }

        [Test]
        public void TestObjectsManageMembersRequest59 ()
        {
            TestObjectsManageMembersCommon (true, false, false, 100, "", "", false);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam60 ()
        {
            TestObjectsManageMembersCommon (true, true, false, 100, "", "", false);
        }

        [Test]
        public void TestObjectsManageMembersRequest61 ()
        {
            TestObjectsManageMembersCommon (true, false, true, 0, "", "", false);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam62 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 0, "", "", false);
        }

        [Test]
        public void TestObjectsManageMembersRequest63 ()
        {
            TestObjectsManageMembersCommon (true, false, false, 0, "", "", false);
        }

        [Test]
        public void TestObjectsManageMembersRequestQueryParam64 ()
        {
            TestObjectsManageMembersCommon (true, true, true, 0, "", "", false);
        }
        [Test]
        public void TestObjectsManageMembersRequestwithSortQueryParam65(){
            TestObjectsManageMembersCommon(true, true, true, 0, "", "", false,true);
        }

        public void TestObjectsManageMembersCommon(bool ssl, bool sendQueryParams, bool withIncl, int limit, string start, string end, bool count){
            TestObjectsManageMembersCommon(ssl, sendQueryParams, withIncl, limit, start, end, count, false);
        }

        public void TestObjectsManageMembersCommon(bool ssl, bool sendQueryParams, bool withIncl,  int limit, string start, string end, bool count, bool withSort){
            string uuid = "customuuid";

            Dictionary<string,string> queryParams = new Dictionary<string, string>();
            string queryParamString = "";
            if(sendQueryParams){
                queryParams.Add("d","f");
                queryParamString="&d=f";
            } else {
                queryParams = null;
            }

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.Secure = ssl;
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            PNUUIDMetadataInclude[] CreateUserInclude = new PNUUIDMetadataInclude[]{PNUUIDMetadataInclude.PNUUIDMetadataIncludeCustom};
            var include = (CreateUserInclude==null) ? new string[]{} : CreateUserInclude.Select(a=>a.GetDescription().ToString()).ToArray();
            string incl = string.Join(",", include);            
            if(!withIncl){
                incl = "";
            }
            string ch = "ch";

            string sortBy = "";
            if (withSort){
                sortBy = "name:desc";
            }

            Uri uri = BuildRequests.BuildObjectsManageChannelMembersRequest (ch, limit, start, end, count, incl, pnUnity, queryParams, sortBy);

            //https://ps.pndsn.com/v2/objects/demo/spaces/UnityUnitTests_86?uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX%2F4.3.0 
            string expected = string.Format ("http{0}://{1}/v2/objects/{2}/channels/{11}/uuids?uuid={3}{7}{10}{4}{8}{9}{12}&pnsdk={5}{6}",
                ssl?"s":"", 
                pnConfiguration.Origin, 
                EditorCommon.SubscribeKey, 
                uuid, 
                (limit>0)?string.Format("&limit={0}", limit.ToString()):"",
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNPublishOperation, false, false),
                queryParamString,
                (string.IsNullOrEmpty(incl))?"":string.Format("&include={0}",incl),
                (string.IsNullOrEmpty(start))?"":string.Format("&start={0}",start),
                (string.IsNullOrEmpty(end))?"":string.Format("&end={0}",end),
                (count) ? "&count=1" : "&count=0",
                ch,
                (withSort) ? "&sort=name%3Adesc" : ""
            );

            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }
        #endif
    }
}
