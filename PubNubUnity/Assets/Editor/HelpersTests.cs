using System;
using PubNubAPI;
using NUnit.Framework;
using System.Text;
using System.Collections.Generic;

namespace PubNubAPI.Tests
{
    [TestFixture]
    public class HelpersTests
    {
        #if DEBUG  

        [Test]
        public void TestCreatePNStatusWithCGAndCh(){
            PNConfiguration pnConfiguration = EditorCommon.CreatePNConfig();
            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);
            List<ChannelEntity> channelEntities1 = EditorCommon.CreateListOfChannelEntities(true, false, false, false, pnLog);
            List<ChannelEntity> channelEntities2 = EditorCommon.CreateListOfChannelEntities(false, true, false, false, pnLog);
            pnUnity.SubscriptionInstance.Add(channelEntities1);
            pnUnity.SubscriptionInstance.Add(channelEntities2);

            PNStatus pnStatus = Helpers.CreatePNStatus(
                        PNStatusCategory.PNReconnectedCategory,
                        "",
                        null,
                        false,
                        PNOperationType.PNSubscribeOperation,
                        pnUnity.SubscriptionInstance.AllChannels,
                        pnUnity.SubscriptionInstance.AllChannelGroups,
                        null,
                        pnUnity
                    );

            Assert.True(pnStatus.Category.Equals(PNStatusCategory.PNReconnectedCategory));
            Assert.True(pnStatus.Error.Equals(false));
            Assert.True(pnStatus.ErrorData==null);
            Assert.True((pnStatus.AuthKey!=null)?pnStatus.AuthKey.Equals(pnConfiguration.AuthKey):true);
            Assert.True(pnStatus.Operation.Equals(PNOperationType.PNSubscribeOperation));
            Assert.True(pnStatus.Origin.Equals(pnConfiguration.Origin));
            Assert.True(pnStatus.StatusCode.Equals(0));
            Assert.True(pnStatus.TlsEnabled.Equals(pnConfiguration.Secure));
            Assert.True(pnStatus.UUID.Equals(pnConfiguration.UUID));
            Assert.True(EditorCommon.MatchChannelsEntities(channelEntities1, pnStatus.AffectedChannelGroups));
            Assert.True(EditorCommon.MatchChannelsEntities(channelEntities2, pnStatus.AffectedChannels));
        }

        [Test]
        public void TestCreatePNStatusWithoutCGAndCh(){
            PNConfiguration pnConfiguration = EditorCommon.CreatePNConfig();
            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);

            PNStatus pnStatus = Helpers.CreatePNStatus(
                        PNStatusCategory.PNReconnectedCategory,
                        "",
                        null,
                        false,
                        PNOperationType.PNSubscribeOperation,
                        pnUnity.SubscriptionInstance.AllChannels,
                        pnUnity.SubscriptionInstance.AllChannelGroups,
                        null,
                        pnUnity
                    );

            Assert.True(pnStatus.Category.Equals(PNStatusCategory.PNReconnectedCategory));
            Assert.True(pnStatus.Error.Equals(false));
            Assert.True(pnStatus.ErrorData==null);
            Assert.True((pnStatus.AuthKey!=null)?pnStatus.AuthKey.Equals(pnConfiguration.AuthKey):true);
            Assert.True(pnStatus.Operation.Equals(PNOperationType.PNSubscribeOperation));
            Assert.True(pnStatus.Origin.Equals(pnConfiguration.Origin));
            Assert.True(pnStatus.StatusCode.Equals(0));
            Assert.True(pnStatus.TlsEnabled.Equals(pnConfiguration.Secure));
            Assert.True(pnStatus.UUID.Equals(pnConfiguration.UUID));
            Assert.True(pnStatus.AffectedChannelGroups.Count.Equals(0));
            Assert.True(pnStatus.AffectedChannels.Count.Equals(0));
        }

        [Test]
        public void TestCreatePNStatusExceptionWithoutCHAndCG(){
            PNConfiguration pnConfiguration = EditorCommon.CreatePNConfig();
            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);
                    
            string message = "Both Channels and ChannelGroups cannot be empty";
            PNStatus pnStatus = Helpers.CreatePNStatus(
                        PNStatusCategory.PNReconnectedCategory,
                        message,
                        new PubNubException(message),
                        true,
                        PNOperationType.PNSubscribeOperation,
                        pnUnity.SubscriptionInstance.AllChannels,
                        pnUnity.SubscriptionInstance.AllChannelGroups,
                        null,
                        pnUnity
                    );

            Assert.True(pnStatus.Category.Equals(PNStatusCategory.PNReconnectedCategory));
            Assert.True(pnStatus.Error.Equals(true));
            Assert.True(pnStatus.ErrorData.Info.Equals(message));
            Assert.True(pnStatus.ErrorData.Ex.Message.Equals(message));
            Assert.True((pnStatus.AuthKey!=null)?pnStatus.AuthKey.Equals(pnConfiguration.AuthKey):true);
            Assert.True(pnStatus.Operation.Equals(PNOperationType.PNSubscribeOperation));
            Assert.True(pnStatus.Origin.Equals(pnConfiguration.Origin));
            Assert.True(pnStatus.StatusCode.Equals(0));
            Assert.True(pnStatus.TlsEnabled.Equals(pnConfiguration.Secure));
            Assert.True(pnStatus.UUID.Equals(pnConfiguration.UUID));
            Assert.True(pnStatus.AffectedChannelGroups.Count.Equals(0));
            Assert.True(pnStatus.AffectedChannels.Count.Equals(0));

        }

        [Test]
        public void TestCreatePNStatusExceptionWithCHAndCG(){
            PNConfiguration pnConfiguration = EditorCommon.CreatePNConfig();
            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);
            List<ChannelEntity> channelEntities1 = EditorCommon.CreateListOfChannelEntities(true, false, false, false, pnLog);
            List<ChannelEntity> channelEntities2 = EditorCommon.CreateListOfChannelEntities(false, true, false, false, pnLog);
            pnUnity.SubscriptionInstance.Add(channelEntities1);
            pnUnity.SubscriptionInstance.Add(channelEntities2);

                    
            string message = "Both Channels and ChannelGroups cannot be empty";
            PNStatus pnStatus = Helpers.CreatePNStatus(
                        PNStatusCategory.PNReconnectedCategory,
                        message,
                        new PubNubException(message),
                        true,
                        PNOperationType.PNSubscribeOperation,
                        pnUnity.SubscriptionInstance.AllChannels,
                        pnUnity.SubscriptionInstance.AllChannelGroups,
                        null,
                        pnUnity
                    );

            Assert.True(pnStatus.Category.Equals(PNStatusCategory.PNReconnectedCategory));
            Assert.True(pnStatus.Error.Equals(true));
            Assert.True(pnStatus.ErrorData.Info.Equals(message));
            Assert.True(pnStatus.ErrorData.Ex.Message.Equals(message));
            Assert.True((pnStatus.AuthKey!=null)?pnStatus.AuthKey.Equals(pnConfiguration.AuthKey):true);
            Assert.True(pnStatus.Operation.Equals(PNOperationType.PNSubscribeOperation));
            Assert.True(pnStatus.Origin.Equals(pnConfiguration.Origin));
            Assert.True(pnStatus.StatusCode.Equals(0));
            Assert.True(pnStatus.TlsEnabled.Equals(pnConfiguration.Secure));
            Assert.True(pnStatus.UUID.Equals(pnConfiguration.UUID));
            Assert.True(EditorCommon.MatchChannelsEntities(channelEntities1, pnStatus.AffectedChannelGroups));
            Assert.True(EditorCommon.MatchChannelsEntities(channelEntities2, pnStatus.AffectedChannels));

        }

        [Test]
        public void TestCreatePNStatusExceptionWithCH(){
            PNConfiguration pnConfiguration = EditorCommon.CreatePNConfig();
            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);
            List<ChannelEntity> channelEntities1 = EditorCommon.CreateListOfChannelEntities(true, false, false, false, pnLog);
            
            RequestState pnRequestState = new RequestState();
            pnRequestState.URL = "https://testurl";

            string message = "Both Channels and ChannelGroups cannot be empty";
            PNStatus pnStatus = Helpers.CreatePNStatus(
                        PNStatusCategory.PNConnectedCategory,
                        message,
                        new PubNubException(message),
                        true,
                        PNOperationType.PNSubscribeOperation,
                        channelEntities1[0], //channel entity
                        pnRequestState,
                        pnUnity
                    );

            Assert.True(pnStatus.Category.Equals(PNStatusCategory.PNConnectedCategory));
            Assert.True(pnStatus.Error.Equals(true));
            Assert.True(pnStatus.ErrorData.Info.Equals(message));
            Assert.True(pnStatus.ErrorData.Ex.Message.Equals(message));
            Assert.True((pnStatus.AuthKey!=null)?pnStatus.AuthKey.Equals(pnConfiguration.AuthKey):true);
            Assert.True(pnStatus.Operation.Equals(PNOperationType.PNSubscribeOperation));
            Assert.True(pnStatus.Origin.Equals(pnConfiguration.Origin));
            Assert.True(pnStatus.StatusCode.Equals(0));
            Assert.True(pnStatus.TlsEnabled.Equals(pnConfiguration.Secure));
            Assert.True(pnStatus.UUID.Equals(pnConfiguration.UUID));
            Assert.True(channelEntities1[0].ChannelID.ChannelOrChannelGroupName.Equals(pnStatus.AffectedChannelGroups[0]));
            //Assert.True(pnStatus.AffectedChannels.Count.Equals(0));
            Assert.True(pnStatus.ClientRequest.Equals(pnRequestState.URL));
        }

        [Test]
        public void TestCreatePNStatusWithCH(){
            PNConfiguration pnConfiguration = EditorCommon.CreatePNConfig();
            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);
            List<ChannelEntity> channelEntities1 = EditorCommon.CreateListOfChannelEntities(true, false, false, false, pnLog);
            
            RequestState pnRequestState = new RequestState();
            pnRequestState.URL = "https://testurl";

            
            PNStatus pnStatus = Helpers.CreatePNStatus(
                        PNStatusCategory.PNConnectedCategory,
                        "",
                        null,
                        true,
                        PNOperationType.PNSubscribeOperation,
                        channelEntities1[0], //channel entity
                        pnRequestState,
                        pnUnity
                    );

            Assert.True(pnStatus.Category.Equals(PNStatusCategory.PNConnectedCategory));
            Assert.True(pnStatus.Error.Equals(true));
            Assert.True((pnStatus.AuthKey!=null)?pnStatus.AuthKey.Equals(pnConfiguration.AuthKey):true);
            Assert.True(pnStatus.Operation.Equals(PNOperationType.PNSubscribeOperation));
            Assert.True(pnStatus.Origin.Equals(pnConfiguration.Origin));
            Assert.True(pnStatus.StatusCode.Equals(0));
            Assert.True(pnStatus.TlsEnabled.Equals(pnConfiguration.Secure));
            Assert.True(pnStatus.UUID.Equals(pnConfiguration.UUID));
            Assert.True(channelEntities1[0].ChannelID.ChannelOrChannelGroupName.Equals(pnStatus.AffectedChannelGroups[0]));
            Assert.True(pnStatus.ClientRequest.Equals(pnRequestState.URL));
        }

        [Test]
        public void TestCreatePNStatusExceptionWithCG(){
            PNConfiguration pnConfiguration = EditorCommon.CreatePNConfig();
            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);
            List<ChannelEntity> channelEntities1 = EditorCommon.CreateListOfChannelEntities(false, true, false, false, pnLog);
            
            RequestState pnRequestState = new RequestState();
            pnRequestState.URL = "https://testurl";

            string message = "Both Channels and ChannelGroups cannot be empty";
            PNStatus pnStatus = Helpers.CreatePNStatus(
                        PNStatusCategory.PNConnectedCategory,
                        message,
                        new PubNubException(message),
                        true,
                        PNOperationType.PNSubscribeOperation,
                        channelEntities1[0], //channel entity
                        pnRequestState,
                        pnUnity
                    );

            Assert.True(pnStatus.Category.Equals(PNStatusCategory.PNConnectedCategory));
            Assert.True(pnStatus.Error.Equals(true));
            Assert.True(pnStatus.ErrorData.Info.Equals(message));
            Assert.True(pnStatus.ErrorData.Ex.Message.Equals(message));
            Assert.True((pnStatus.AuthKey!=null)?pnStatus.AuthKey.Equals(pnConfiguration.AuthKey):true);
            Assert.True(pnStatus.Operation.Equals(PNOperationType.PNSubscribeOperation));
            Assert.True(pnStatus.Origin.Equals(pnConfiguration.Origin));
            Assert.True(pnStatus.StatusCode.Equals(0));
            Assert.True(pnStatus.TlsEnabled.Equals(pnConfiguration.Secure));
            Assert.True(pnStatus.UUID.Equals(pnConfiguration.UUID));
            Assert.True(channelEntities1[0].ChannelID.ChannelOrChannelGroupName.Equals(pnStatus.AffectedChannels[0]));
            //Assert.True(pnStatus.AffectedChannels.Count.Equals(0));
            Assert.True(pnStatus.ClientRequest.Equals(pnRequestState.URL));
        }

        [Test]
        public void TestCreatePNStatusWithCG(){
            PNConfiguration pnConfiguration = EditorCommon.CreatePNConfig();
            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);
            List<ChannelEntity> channelEntities1 = EditorCommon.CreateListOfChannelEntities(false, true, false, false, pnLog);
            
            RequestState pnRequestState = new RequestState();
            pnRequestState.URL = "https://testurl";

            
            PNStatus pnStatus = Helpers.CreatePNStatus(
                        PNStatusCategory.PNConnectedCategory,
                        "",
                        null,
                        true,
                        PNOperationType.PNSubscribeOperation,
                        channelEntities1[0], //channel entity
                        pnRequestState,
                        pnUnity
                    );

            Assert.True(pnStatus.Category.Equals(PNStatusCategory.PNConnectedCategory));
            Assert.True(pnStatus.Error.Equals(true));
            Assert.True((pnStatus.AuthKey!=null)?pnStatus.AuthKey.Equals(pnConfiguration.AuthKey):true);
            Assert.True(pnStatus.Operation.Equals(PNOperationType.PNSubscribeOperation));
            Assert.True(pnStatus.Origin.Equals(pnConfiguration.Origin));
            Assert.True(pnStatus.StatusCode.Equals(0));
            Assert.True(pnStatus.TlsEnabled.Equals(pnConfiguration.Secure));
            Assert.True(pnStatus.UUID.Equals(pnConfiguration.UUID));
            Assert.True(channelEntities1[0].ChannelID.ChannelOrChannelGroupName.Equals(pnStatus.AffectedChannels[0]));
            //Assert.True(pnStatus.AffectedChannels.Count.Equals(0));
            Assert.True(pnStatus.ClientRequest.Equals(pnRequestState.URL));
        }

        public void TestCreatePNStatus3(){
            PNConfiguration pnConfiguration = EditorCommon.CreatePNConfig();
            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);
            RequestState pnRequestState = new RequestState();
            pnRequestState.URL = "https://testurl";
            List<string> ch = new List<string>();
            ch.Add("ch");
            List<string> cg = new List<string>();
            cg.Add("cg");
            
            string message = "Duplicate Channels or Channel Groups";
            PNStatus pnStatus = Helpers.CreatePNStatus(
                    PNStatusCategory.PNUnknownCategory,
                    message,
                    null,
                    false,                
                    PNOperationType.PNSubscribeOperation,
                    ch,
                    cg,
                    pnRequestState,
                    pnUnity
                );

            Assert.True(pnStatus.Category.Equals(PNStatusCategory.PNUnknownCategory));
            Assert.True(pnStatus.Error.Equals(false));
           Assert.True((pnStatus.AuthKey!=null)?pnStatus.AuthKey.Equals(pnConfiguration.AuthKey):true);
            Assert.True(pnStatus.Operation.Equals(PNOperationType.PNSubscribeOperation));
            Assert.True(pnStatus.Origin.Equals(pnConfiguration.Origin));
            Assert.True(pnStatus.StatusCode.Equals(0));
            Assert.True(pnStatus.TlsEnabled.Equals(pnConfiguration.Secure));
            Assert.True(pnStatus.UUID.Equals(pnConfiguration.UUID));
            Assert.True(ch[0].Equals(pnStatus.AffectedChannels[0]));
            Assert.True(cg[0].Equals(pnStatus.AffectedChannelGroups[0]));
            Assert.True(pnStatus.ClientRequest.Equals(pnRequestState.URL));

            
        }

        [Test]
        public void TestCreatePNStatus3ErrorNoException(){
            PNConfiguration pnConfiguration = EditorCommon.CreatePNConfig();
            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);
            RequestState pnRequestState = new RequestState();
            pnRequestState.URL = "https://testurl";
            List<string> ch = new List<string>();
            ch.Add("ch");
            List<string> cg = new List<string>();
            cg.Add("cg");
            
            string message = "Duplicate Channels or Channel Groups";
            PNStatus pnStatus = Helpers.CreatePNStatus(
                    PNStatusCategory.PNUnknownCategory,
                    message,
                    null,
                    true,                
                    PNOperationType.PNSubscribeOperation,
                    ch,
                    cg,
                    pnRequestState,
                    pnUnity
                );

            Assert.True(pnStatus.Category.Equals(PNStatusCategory.PNUnknownCategory));
            Assert.True(pnStatus.Error.Equals(true));
            Assert.True(pnStatus.ErrorData.Info.Equals(message));
            Assert.True((pnStatus.AuthKey!=null)?pnStatus.AuthKey.Equals(pnConfiguration.AuthKey):true);
            Assert.True(pnStatus.Operation.Equals(PNOperationType.PNSubscribeOperation));
            Assert.True(pnStatus.Origin.Equals(pnConfiguration.Origin));
            Assert.True(pnStatus.StatusCode.Equals(0));
            Assert.True(pnStatus.TlsEnabled.Equals(pnConfiguration.Secure));
            Assert.True(pnStatus.UUID.Equals(pnConfiguration.UUID));
            Assert.True(ch[0].Equals(pnStatus.AffectedChannels[0]));
            Assert.True(cg[0].Equals(pnStatus.AffectedChannelGroups[0]));
            Assert.True(pnStatus.ClientRequest.Equals(pnRequestState.URL));

            
        }

        [Test]
        public void TestCreatePNStatus3Exception(){
            PNConfiguration pnConfiguration = EditorCommon.CreatePNConfig();
            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);
            RequestState pnRequestState = new RequestState();
            pnRequestState.URL = "https://testurl";
            List<string> ch = new List<string>();
            ch.Add("ch");
            List<string> cg = new List<string>();
            cg.Add("cg");
            
            string message = "Duplicate Channels or Channel Groups";
            PNStatus pnStatus = Helpers.CreatePNStatus(
                    PNStatusCategory.PNUnknownCategory,
                    message,
                    new PubNubException(message),
                    true,                
                    PNOperationType.PNSubscribeOperation,
                    ch,
                    cg,
                    pnRequestState,
                    pnUnity
                );

            Assert.True(pnStatus.Category.Equals(PNStatusCategory.PNUnknownCategory));
            Assert.True(pnStatus.Error.Equals(true));
            Assert.True(pnStatus.ErrorData.Info.Equals(message));
            Assert.True(pnStatus.ErrorData.Ex.Message.Equals(message));
            Assert.True((pnStatus.AuthKey!=null)?pnStatus.AuthKey.Equals(pnConfiguration.AuthKey):true);
            Assert.True(pnStatus.Operation.Equals(PNOperationType.PNSubscribeOperation));
            Assert.True(pnStatus.Origin.Equals(pnConfiguration.Origin));
            Assert.True(pnStatus.StatusCode.Equals(0));
            Assert.True(pnStatus.TlsEnabled.Equals(pnConfiguration.Secure));
            Assert.True(pnStatus.UUID.Equals(pnConfiguration.UUID));
            Assert.True(ch[0].Equals(pnStatus.AffectedChannels[0]));
            Assert.True(cg[0].Equals(pnStatus.AffectedChannelGroups[0]));
            Assert.True(pnStatus.ClientRequest.Equals(pnRequestState.URL));

            
        }

        [Test]
        public void TestCreateListOfSubscribeMessage(){
            object[] obj = {EditorCommon.CreateSubscribeDictionary()}; 
            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);
            List<SubscribeMessage> lsm = Helpers.CreateListOfSubscribeMessage(obj, pnLog);

            if(lsm!=null){
                ParseSubscribeMessageList(lsm);
            }
            else {
                Assert.True(false, "Lsm null");
            }

        }

        [Test]
        public void TestAddToSubscribeMessageList(){
            List<SubscribeMessage> lsm = new List<SubscribeMessage>();
            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);
            Helpers.TryAddToSubscribeMessageList(EditorCommon.CreateSubscribeDictionary(), ref lsm, pnLog);

            if(lsm!=null){
                ParseSubscribeMessageList(lsm);
            }
            else {
                Assert.True(false, "Lsm null");
            }
        }

        internal void ParseSubscribeMessageList(List<SubscribeMessage> lsm){
            var dictUR = lsm[0].UserMetadata as Dictionary<string, object>;
            string log=
                String.Format(" " 
                    +"\n lsm[0].Channel.Equals('Channel') {0} "
                    +"\n lsm[0].Flags.Equals('flags') {1} "
                    +"\n lsm[0].IssuingClientId.Equals('issuingClientId') {2} "
                    +"\n lsm[0].OriginatingTimetoken.Region.Equals('west') {3} "
                    +"\n lsm[0].OriginatingTimetoken.Timetoken.Equals(14685037252884276) {4} "
                    +"\n lsm[0].Payload.Equals('Message') {5} "
                    +"\n lsm[0].PublishTimetokenMetadata.Region.Equals('east') {6} "
                    +"\n lsm[0].PublishTimetokenMetadata.Timetoken.Equals(14685037252884348) {7} "
                    +"\n lsm[0].SequenceNumber.Equals('10') {8} "
                    +"\n lsm[0].Shard.Equals('1') {9} "
                    +"\n lsm[0].SubscribeKey.Equals('subscribeKey') {10} "
                    +"\n lsm[0].SubscriptionMatch.Equals('SM') {11} "
                    +"\n dictUR.ContainsKey('region')  {12} "
                    +"\n dictUR.ContainsValue('north')  {13} ",
                    lsm[0].Channel.Equals("Channel")
                    , lsm[0].Flags.Equals("flags")
                    , lsm[0].IssuingClientId.Equals("issuingClientId")
                    , lsm[0].OriginatingTimetoken.Region.Equals("west")
                    , lsm[0].OriginatingTimetoken.Timetoken.Equals(14685037252884276)
                    , lsm[0].Payload.Equals("Message")
                    , lsm[0].PublishTimetokenMetadata.Region.Equals("east")
                    , lsm[0].PublishTimetokenMetadata.Timetoken.Equals(14685037252884348)
                    , lsm[0].SequenceNumber.Equals(10)
                    , lsm[0].Shard.Equals("1")
                    , lsm[0].SubscribeKey.Equals("subscribeKey")
                    , lsm[0].SubscriptionMatch.Equals("SM")
                    , dictUR.ContainsKey("region")
                    , dictUR.ContainsValue("north") 
                );
            UnityEngine.Debug.Log(log);
            Assert.True(
                lsm[0].Channel.Equals("Channel")
                && lsm[0].Flags.Equals("flags")
                && lsm[0].IssuingClientId.Equals("issuingClientId")
                && lsm[0].OriginatingTimetoken.Region.Equals("west")
                && lsm[0].OriginatingTimetoken.Timetoken.Equals(14685037252884276)
                && lsm[0].Payload.Equals("Message")
                && lsm[0].PublishTimetokenMetadata.Region.Equals("east")
                && lsm[0].PublishTimetokenMetadata.Timetoken.Equals(14685037252884348)
                && lsm[0].SequenceNumber.Equals(10)
                && lsm[0].Shard.Equals("1")
                && lsm[0].SubscribeKey.Equals("subscribeKey")
                && lsm[0].SubscriptionMatch.Equals("SM")
                && dictUR.ContainsKey("region")
                && dictUR.ContainsValue("north"), log);

        }

        [Test]
        public void TestCreateTimetokenMetadata(){
            var dict = new Dictionary<string, object>(); 
            dict.Add("t", 14685037252884276);
            dict.Add("r", "east");
            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);
            TimetokenMetadata ttm= Helpers.CreateTimetokenMetadata(dict, "orig", pnLog);
            Assert.True(
                ttm.Region.Equals("east")
                && ttm.Timetoken.Equals(14685037252884276));
        }

        [Test]
        public void TestCreateTimetokenMetadataWithoutRegion(){
            var dict = new Dictionary<string, object>(); 
            dict.Add("t", 14685037252884276);
            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);
            TimetokenMetadata ttm= Helpers.CreateTimetokenMetadata(dict, "orig", pnLog);
            Assert.True(
                ttm.Region.Equals("")
                && ttm.Timetoken.Equals(14685037252884276));
        }

        [Test]
        public void TestDecodeMessage(){ 
            string str = "UXgV6VPqJ7WI04csguMrqw==";
            TestDecodeMessageCommon (str, "test message");
        }

        [Test]
        public void TestDecodeMessageError(){ 
            string str = "UXgV6VPqJ7WI04csguMrqw=";
            TestDecodeMessageCommon (str, "**DECRYPT ERROR**");
        }

        void TestDecodeMessageCommon(object inputMessage, string resultExpected){
            string[] multiChannel = {"testChannel3"};
            string cipherKey = "enigma";
            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);
            List<ChannelEntity> channelEntities = Helpers.CreateChannelEntity(multiChannel, false, false, null, pnLog);

            object resp= Helpers.DecodeMessage(cipherKey, inputMessage, PNOperationType.PNSubscribeOperation, pnUnity);

            UnityEngine.Debug.Log ("ser2:" + resultExpected.ToString());
            UnityEngine.Debug.Log ("ser1:" + resp.ToString());
            Assert.IsTrue (resp.Equals(resultExpected));
        }

        [Test]
        public void TestDeserializeAndAddToResult(){
            string str = "{\"status\":200,\"message\":\"OK\",\"service\":\"Presence\",\"occupancy\":0}";
            TestDeserializeAndAddToResultCommon (false, str, "status");
        }

        [Test]
        public void TestDeserializeAndAddToResultAddChannel(){
            string str = "{\"status\":200,\"message\":\"OK\",\"service\":\"Presence\",\"occupancy\":0}";
            TestDeserializeAndAddToResultCommon (true, str, "status");
        }

        public void TestDeserializeAndAddToResultCommon(bool addChannel, string str, string expectedResult){ 
            string channel = "testChannel3";

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);

            List<object> lstObj = Helpers.DeserializeAndAddToResult (str, channel, pnUnity.JsonLibrary, addChannel);

            bool bRes = false;
            foreach (object obj in lstObj) {
                if (obj.GetType ().IsGenericType) {
                    UnityEngine.Debug.Log ("generic:" + obj.ToString ());
                    Dictionary<string, object> dictionary = (Dictionary<string, object>)obj;
                    bRes = dictionary.ContainsKey(expectedResult);
                    break;
                }
            }

            UnityEngine.Debug.Log ("ser1:" + bRes );
            Assert.IsTrue(bRes);
            if (addChannel) {
                bRes = lstObj.Contains (channel);
                UnityEngine.Debug.Log ("ser2:" + bRes);
                Assert.IsTrue (bRes);

            } else {
                bRes = lstObj.Contains (channel);
                UnityEngine.Debug.Log ("ser2:" + bRes);
                Assert.IsFalse (bRes);
            }
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityErrorCallback(){
            TestUpdateOrAddUserStateOfEntityCommon(true, false, true, false,
                 false);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntity(){
            TestUpdateOrAddUserStateOfEntityCommon(false, false, false, false,
                 false);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityErrorCallbackEdit(){
            TestUpdateOrAddUserStateOfEntityCommon(true, true, false, false,
                 false);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityEdit(){
            TestUpdateOrAddUserStateOfEntityCommon(false, true, false, false,
                 false);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityErrorCallbackOther(){
            TestUpdateOrAddUserStateOfEntityCommon(true, false, true, false,
                 true);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityOther(){
            TestUpdateOrAddUserStateOfEntityCommon(false, false, false, false,
                 true);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityErrorCallbackEditOther(){
            TestUpdateOrAddUserStateOfEntityCommon(true, true, false, false,
                 true);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityEditOther(){
            TestUpdateOrAddUserStateOfEntityCommon(false, true, false, false,
                 true);
        }

        public void TestUpdateOrAddUserStateOfEntityCommon(bool isChannelGroup, bool edit, 
            bool checkErrorCallback, bool ssl, bool isForOtherUUID){

            var dictSM = new Dictionary<string, object>();
            dictSM.Add("k2","v2");
            dictSM.Add("k","v");

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);
            pnUnity.SubscriptionInstance.CleanUp();

            string state = pnUnity.JsonLibrary.SerializeToJsonString(dictSM);
            ChannelEntity ce1 = Helpers.CreateChannelEntity("ch1", false, isChannelGroup, dictSM, pnLog);

            List<ChannelEntity> lstCe = new List<ChannelEntity>();
            lstCe.Add(ce1);
            string channelName = "ch1";

            if(checkErrorCallback || edit){
                var dictSM2 = new Dictionary<string, object>();
                dictSM2.Add("k2","v3");

                List<ChannelEntity> lstCe2 = new List<ChannelEntity>();
                lstCe2.Add(ce1);
                ChannelEntity ce3 = Helpers.CreateChannelEntity("ch1", false, false, null, pnLog);

                pnUnity.SubscriptionInstance.TryUpdateOrAddUserStateOfEntity(ref ce3, dictSM2, edit);

                string ustate = pnUnity.JsonLibrary.SerializeToJsonString(lstCe2[0].ChannelParams.UserState);
                string state2 = pnUnity.JsonLibrary.SerializeToJsonString(dictSM2);
                UnityEngine.Debug.Log(string.Format("{0}\n{1}", state2, ustate));
            }

            if(pnUnity.SubscriptionInstance.TryUpdateOrAddUserStateOfEntity(ref ce1, dictSM, edit)){
                string ustate = pnUnity.JsonLibrary.SerializeToJsonString(lstCe[0].ChannelParams.UserState);
                UnityEngine.Debug.Log(string.Format("{0}\n{1}", state, ustate));
                Assert.AreEqual(ustate, state, string.Format ("{0}\n{1}", ustate, state));
            } else {
                UnityEngine.Debug.Log(state);
                if(!checkErrorCallback){
                    Assert.True(false, "UpdateOrAddUserStateOfEntity returned false");
                }
            }
        }

        [Test]
        public void TestGetNamesFromChannelEntities(){
            TestGetNamesFromChannelEntitiesCommon(true, true);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCG(){
            TestGetNamesFromChannelEntitiesCommon(true, false);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCN(){
            TestGetNamesFromChannelEntitiesCommon(false, true);
        }

        public void TestGetNamesFromChannelEntitiesCommon(bool channelGroup, bool channel){
            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);
            pnUnity.SubscriptionInstance.CleanUp();            
            List<ChannelEntity> lstCE= EditorCommon.CreateListOfChannelEntities(channelGroup, channel, false, false, pnLog);  
            string ces = Helpers.GetAllNamesFromChannelEntities(lstCE, false);

            bool ceFound = true;
            foreach(ChannelEntity ch in lstCE){
                if(!ces.Contains(ch.ChannelID.ChannelOrChannelGroupName)){
                    ceFound = false;
                }
            }
            UnityEngine.Debug.Log(ces);
            Assert.True(ceFound, ces);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCG2(){
            TestGetNamesFromChannelEntitiesCommon2(true, false);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCN2(){
            TestGetNamesFromChannelEntitiesCommon2(false, true);
        }

        public void TestGetNamesFromChannelEntitiesCommon2(bool channelGroup, bool channel){
            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);
            pnUnity.SubscriptionInstance.CleanUp();            
            List<ChannelEntity> lstCE= EditorCommon.CreateListOfChannelEntities(channelGroup, channel, false, false, pnLog);  
             
            string ces = Helpers.GetNamesFromChannelEntities(lstCE, channelGroup);

            bool ceFound = true;
            foreach(ChannelEntity ch in lstCE){
                if(channelGroup){
                    if(!ces.Contains(ch.ChannelID.ChannelOrChannelGroupName)
                        && ch.ChannelID.IsChannelGroup)
                    {
                        ceFound = false;
                    }
                } else {
                    if(!ces.Contains(ch.ChannelID.ChannelOrChannelGroupName)
                        && !ch.ChannelID.IsChannelGroup)
                    {
                        ceFound = false;
                    }
                }
            }
            UnityEngine.Debug.Log(ces);
            Assert.True(ceFound, ces);
        }

        [Test]
        public void TestBuildJsonUserState(){
            var dictSM = new Dictionary<string, object>();
            dictSM.Add("k","v");
            dictSM.Add("k2","v2");
            string ret = Helpers.BuildJsonUserState(dictSM);
            Assert.AreEqual(ret, "\"k\":\"v\",\"k2\":\"v2\"", ret);
        }

        [Test]
        public void TestBuildJsonUserStateCE(){
            TestBuildJsonUserStateCommon(false, true);
        }

        [Test]
        public void TestBuildJsonUserStateCECG(){
            TestBuildJsonUserStateCommon(true, false);
        }

        [Test]
        public void TestBuildJsonUserStateCECGnCH(){
            TestBuildJsonUserStateCommon(true, true);
        }

        public void TestBuildJsonUserStateCommon(bool channelGroup, bool channel){
            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);
            pnUnity.SubscriptionInstance.CleanUp();
            List<ChannelEntity> lstCE= EditorCommon.CreateListOfChannelEntities(channelGroup, channel, false, false, pnLog);
            string ret = Helpers.BuildJsonUserState(lstCE);
            UnityEngine.Debug.Log("ret:" + ret);
            if(channel && channelGroup){
                UnityEngine.Debug.Log("expected:" + "{\"ch1\":{\"k\":\"v\",\"k2\":\"v2\"},\"ch2\":{\"k3\":\"v3\",\"k4\":\"v4\"},\"cg1\":{\"k5\":\"v5\",\"k6\":\"v6\"},\"cg2\":{\"k7\":\"v7\",\"k8\":\"v8\"}}");
                Assert.AreEqual(ret, "{\"ch1\":{\"k\":\"v\",\"k2\":\"v2\"},\"ch2\":{\"k3\":\"v3\",\"k4\":\"v4\"},\"cg1\":{\"k5\":\"v5\",\"k6\":\"v6\"},\"cg2\":{\"k7\":\"v7\",\"k8\":\"v8\"}}", ret);
            } else if (channelGroup){
                UnityEngine.Debug.Log("expected:" + "{\"cg1\":{\"k5\":\"v5\",\"k6\":\"v6\"},\"cg2\":{\"k7\":\"v7\",\"k8\":\"v8\"}}");
                Assert.AreEqual(ret, "{\"cg1\":{\"k5\":\"v5\",\"k6\":\"v6\"},\"cg2\":{\"k7\":\"v7\",\"k8\":\"v8\"}}", ret);
            } else {
                UnityEngine.Debug.Log("expected:" + "{\"ch1\":{\"k\":\"v\",\"k2\":\"v2\"},\"ch2\":{\"k3\":\"v3\",\"k4\":\"v4\"}}");
                Assert.AreEqual(ret, "{\"ch1\":{\"k\":\"v\",\"k2\":\"v2\"},\"ch2\":{\"k3\":\"v3\",\"k4\":\"v4\"}}", ret);
            }
        }

        [Test]
        public void TestCounterClassNextValue(){
            Counter publishMessageCounter = new Counter ();
            publishMessageCounter.NextValue();
            Assert.True(publishMessageCounter.NextValue().Equals(2));
        }

        [Test]
        public void TestCounterClassReset(){
            Counter publishMessageCounter = new Counter ();
            publishMessageCounter.NextValue();
            publishMessageCounter.NextValue();
            publishMessageCounter.Reset();
            Assert.True(publishMessageCounter.NextValue().Equals(1));
        }

        string ExceptionMessage ="";
        string ExceptionChannel = "";
        string ExceptionChannelGroups = "";
        int ExceptionStatusCode = 0;

        //ResponseType CRequestType;
        bool ResumeOnReconnect = false;
        bool resultPart1 = false;

        bool IsTimeout = false;
        bool IsError = false;
        [Test]
        public void CheckRequestTimeoutMessageInError ()
        {
            CustomEventArgs cea = new CustomEventArgs ();
            cea.CurrRequestType = PNCurrentRequestType.Subscribe;
            cea.IsError = true;
            cea.IsTimeout = false;
            cea.Message = "The request timed out.";
            cea.PubNubRequestState = null;
            Assert.IsTrue(Helpers.CheckRequestTimeoutMessageInError(cea));
        }

        [Test] 
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe400CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;
            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "test message", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe404CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "404 test message", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe414CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "414 response", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe504CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "504 response", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe503CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "503 response", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe500CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 500;
            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "500 response", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe403CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 403;
            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "403 response", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeNameResolutionFailureCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;
            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "NameResolutionFailure 400", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeConnectFailureCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "ConnectFailure 400", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeServerProtocolViolationCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "ServerProtocolViolation 400 response", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeProtocolErrorCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "ProtocolError 400 response", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeFNFCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "java.io.FileNotFoundException 400 response", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeFailedDLCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "Failed downloading UnityWeb", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe400CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "test message", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe404CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "404 test message", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe414CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "414 response", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe504CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "504 response", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe503CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "503 response", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe500CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "500 response", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe403CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "403 response", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeNameResolutionFailureCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;
            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "NameResolutionFailure 400", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeConnectFailureCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "ConnectFailure 400", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeServerProtocolViolationCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "ServerProtocolViolation 400 response", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeProtocolErrorCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "ProtocolError 400 response", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeFNFCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "java.io.FileNotFoundException 400 response", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeFailedDLCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "Failed downloading UnityWeb", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        } 

        [Test] 
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe400CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "test message", null, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe404CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "404 test message", null, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe414CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "414 response", null, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe504CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;
            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "504 response", null, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe503CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "503 response", null, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe500CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "500 response", null, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe403CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "403 response", null, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeNameResolutionFailureCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "NameResolutionFailure 400", null, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeConnectFailureCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;
            
            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "ConnectFailure 400", null, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeServerProtocolViolationCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "ServerProtocolViolation 400 response", null, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeProtocolErrorCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "ProtocolError 400 response", null, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeFNFCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "java.io.FileNotFoundException 400 response", null, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeFailedDLCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "Failed downloading UnityWeb", null, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe400CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "test message", null, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe404CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "404 test message", null, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe414CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "414 response", null, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe504CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;
            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "504 response", null, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe503CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "503 response", null, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe500CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "500 response", null, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe403CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "403 response", null, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeNameResolutionFailureCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "NameResolutionFailure 400", null, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeConnectFailureCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "ConnectFailure 400", null, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeServerProtocolViolationCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "ServerProtocolViolation 400 response", null, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeProtocolErrorCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "ProtocolError 400 response", null, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeFNFCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "java.io.FileNotFoundException 400 response", null, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeFailedDLCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "Failed downloading UnityWeb", null, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test] 
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe400 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "test message", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe404 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "404 test message", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe414 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "404 response", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe504 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "504 response", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe503 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "503 response", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe500 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "500 response", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe403 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "403 response", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeNameResolutionFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "NameResolutionFailure 400", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeConnectFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "ConnectFailure 400", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeServerProtocolViolation ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "ServerProtocolViolation 400 response", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeProtocolError ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "ProtocolError 400 response", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeFNF ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "java.io.FileNotFoundException 400 response", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeFailedDL ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "Failed downloading UnityWeb", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe400 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;
            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "test message", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe404 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "404 test message", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe414 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;
            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "414 response", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe504 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "504 response", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe503 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "503 response", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe500 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "500 response", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe403 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "403 response", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeNameResolutionFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "NameResolutionFailure 400", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeConnectFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "ConnectFailure 400", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeServerProtocolViolation ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;
            
            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "ServerProtocolViolation 400 response", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeProtocolError ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "ProtocolError 400 response", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeFNF ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "java.io.FileNotFoundException 400 response", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeFailedDL ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (null, "Failed downloading UnityWeb", channels, true, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, true, 0, false, ExceptionStatusCode);
        }

        public void TestResponseCallbackErrorOrTimeoutHandler<T>(string[] channelGroups, string message, string[] channels, bool resumeOnReconnect, PNOperationType responseType, PNCurrentRequestType crt, bool isTimeout, bool isError, long timetoken, bool ssl, int exceptionStatusCode){
            ExceptionMessage = message;
            ExceptionChannel = (channels!=null)?string.Join (",", channels):"";
            ExceptionChannelGroups = (channelGroups!=null)?string.Join (",", channelGroups):"";

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.Secure = ssl;
            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            
            PNLoggingMethod pnLog = new PNLoggingMethod(pnConfiguration.LogVerbosity);

            if (isTimeout) { 
                ExceptionMessage = "Operation Timeout";
                IsTimeout = true;
            } else {
                IsTimeout = false;
            }

            if (isError) {
                IsError = true;
            } else {
                IsError = false;
            }

            List<ChannelEntity> channelEntities = Helpers.CreateChannelEntity(channels, true, false, null, pnLog);
            
            List<ChannelEntity> channelEntities1 = (channelEntities==null)? null : new List<ChannelEntity>(channelEntities);
            List<ChannelEntity> channelGroupEntities = Helpers.CreateChannelEntity(channelGroups, true, true, null, pnLog);  

            if((channelEntities != null) && (channelGroupEntities != null)){
                channelEntities.AddRange(channelGroupEntities);
            } else if(channelEntities == null) {
                channelEntities = channelGroupEntities;
            }
            pnUnity.SubscriptionInstance.Add(channelEntities);

            RequestState requestState = new RequestState();
            requestState.Reconnect = resumeOnReconnect;
            requestState.OperationType = responseType;

            CustomEventArgs cea = new CustomEventArgs ();
            cea.PubNubRequestState = requestState;
            cea.PubNubRequestState.ResponseCode = ExceptionStatusCode;
            
            cea.Message = message;
            cea.IsError = isError;
            cea.IsTimeout = isTimeout;
            cea.CurrRequestType = crt;
            PNStatus pnStatus;
            Helpers.TryCheckErrorTypeAndCallback<T> (cea, pnUnity, out pnStatus);

            if(pnStatus!=null){
                if(channelEntities1!= null)
                    Assert.True(EditorCommon.MatchChannelsEntities(channelEntities1, pnStatus.AffectedChannels));
                if(channelGroupEntities!= null)
                    Assert.True(EditorCommon.MatchChannelsEntities(channelGroupEntities, pnStatus.AffectedChannelGroups));
                UnityEngine.Debug.Log(pnStatus.StatusCode);
                Assert.True(pnStatus.StatusCode.ToString().Equals(exceptionStatusCode.ToString()));
                Assert.True(pnStatus.Operation.Equals(responseType));
                Assert.True(pnStatus.Error);
                
                Assert.True(pnStatus.ErrorData.Info.Contains(ExceptionMessage));
            } else {
                Assert.Fail("pnStatus null");
            }
        }    

        
        #endif
    }
}
