using System;
using PubNubMessaging.Core;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace PubNubMessaging.Tests
{
    [TestFixture]
    public class HelpersUnitTestsNew
    {
        #if DEBUG
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

        [Test]
        public void TestCreateTimetokenMetadata(){
            var dict = new Dictionary<string, object>(); 
            dict.Add("t", 14685037252884276);
            dict.Add("r", "east");
            TimetokenMetadata ttm= Helpers.CreateTimetokenMetadata(dict, "orig");
            Assert.True(
                ttm.Region.Equals("east")
                && ttm.Timetoken.Equals(14685037252884276));
        }

        [Test]
        public void TestAddToSubscribeMessageList(){
            List<SubscribeMessage> lsm = new List<SubscribeMessage>();

            Helpers.AddToSubscribeMessageList(Common.CreateSubscribeDictionary(), ref lsm);

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
        public void TestCreateListOfSubscribeMessage(){
            object[] obj = {Common.CreateSubscribeDictionary()}; 
            List<SubscribeMessage> lsm = Helpers.CreateListOfSubscribeMessage(obj);

            if(lsm!=null){
                ParseSubscribeMessageList(lsm);
            }
            else {
                Assert.True(false, "Lsm null");
            }

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
        public void TestBuildJsonUserStateCEObj(){
            TestBuildJsonUserStateCommon<object>(false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestBuildJsonUserStateCECGObj(){
            TestBuildJsonUserStateCommon<object>(true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestBuildJsonUserStateCECGnCHObj(){
            TestBuildJsonUserStateCommon<object>(true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestBuildJsonUserStateCE(){
            TestBuildJsonUserStateCommon<string>(false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestBuildJsonUserStateCECG(){
            TestBuildJsonUserStateCommon<string>(true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestBuildJsonUserStateCECGnCH(){
            TestBuildJsonUserStateCommon<string>(true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        public void TestBuildJsonUserStateCommon<T>(bool channelGroup, bool channel,
            Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback
        ){
            List<ChannelEntity> lstCE= Common.CreateListOfChannelEntities<T>(channelGroup, channel,
                false, false,
                userCallback, connectCallback,  
                wildcardPresenceCallback,
                disconnectCallback);
            string ret = Helpers.BuildJsonUserState(lstCE);
            if(channel && channelGroup){
                Assert.AreEqual(ret, "{\"ch1\":{\"k\":\"v\",\"k2\":\"v2\"},\"ch2\":{\"k3\":\"v3\",\"k4\":\"v4\"},\"cg1\":{\"k5\":\"v5\",\"k6\":\"v6\"},\"cg2\":{\"k7\":\"v7\",\"k8\":\"v8\"}}", ret);
            } else if (channelGroup){
                Assert.AreEqual(ret, "{\"cg1\":{\"k5\":\"v5\",\"k6\":\"v6\"},\"cg2\":{\"k7\":\"v7\",\"k8\":\"v8\"}}", ret);
            } else {
                Assert.AreEqual(ret, "{\"ch1\":{\"k\":\"v\",\"k2\":\"v2\"},\"ch2\":{\"k3\":\"v3\",\"k4\":\"v4\"}}", ret);
            }
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesObj(){
            TestGetNamesFromChannelEntitiesCommon<object>(true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCGObj(){
            TestGetNamesFromChannelEntitiesCommon<object>(true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCNObj(){
            TestGetNamesFromChannelEntitiesCommon<object>(false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestGetNamesFromChannelEntities(){
            TestGetNamesFromChannelEntitiesCommon<string>(true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCG(){
            TestGetNamesFromChannelEntitiesCommon<string>(true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCN(){
            TestGetNamesFromChannelEntitiesCommon<string>(false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }


        public void TestGetNamesFromChannelEntitiesCommon<T>(bool channelGroup, bool channel,
            Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback){
            List<ChannelEntity> lstCE= Common.CreateListOfChannelEntities<T>(channelGroup, channel,
                false, false,
                userCallback, connectCallback, 
                wildcardPresenceCallback,
                disconnectCallback
            );    
            string ces = Helpers.GetNamesFromChannelEntities(lstCE);

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
        public void TestGetNamesFromChannelEntitiesCG2Obj(){
            TestGetNamesFromChannelEntitiesCommon2<object>(true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCN2Obj(){
            TestGetNamesFromChannelEntitiesCommon2<object>(false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCG2(){
            TestGetNamesFromChannelEntitiesCommon2<string>(true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCN2(){
            TestGetNamesFromChannelEntitiesCommon2<string>(false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        public void TestGetNamesFromChannelEntitiesCommon2<T>(bool channelGroup, bool channel,
            Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback){
            List<ChannelEntity> lstCE= Common.CreateListOfChannelEntities<T>(channelGroup, channel,
                false, false,
                userCallback, connectCallback,  
                wildcardPresenceCallback,
                disconnectCallback
            );    
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
        public void TestUpdateOrAddUserStateOfEntityErrorCallbackObj(){
            TestUpdateOrAddUserStateOfEntityCommon<object>(true, false, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityObj(){
            TestUpdateOrAddUserStateOfEntityCommon<object>(false, false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityErrorCallbackEditObj(){
            TestUpdateOrAddUserStateOfEntityCommon<object>(true, true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityEditObj(){
            TestUpdateOrAddUserStateOfEntityCommon<object>(false, true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityErrorCallback(){
            TestUpdateOrAddUserStateOfEntityCommon<string>(true, false, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntity(){
            TestUpdateOrAddUserStateOfEntityCommon<string>(false, false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityErrorCallbackEdit(){
            TestUpdateOrAddUserStateOfEntityCommon<string>(true, true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityEdit(){
            TestUpdateOrAddUserStateOfEntityCommon<string>(false, true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        public void TestUpdateOrAddUserStateOfEntityCommon<T>(bool isChannelGroup, bool edit, 
            bool checkErrorCallback, bool ssl, Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback){

            var dictSM = new Dictionary<string, object>();
            dictSM.Add("k","v");
            dictSM.Add("k2","v2");

            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                ssl
            );
            string state = pubnub.JsonPluggableLibrary.SerializeToJsonString(dictSM);

            ChannelEntity ce1 = Helpers.CreateChannelEntity<T>("ch1", false, isChannelGroup, dictSM, 
                userCallback, connectCallback,
                ErrorCallbackUserState, disconnectCallback, 
                wildcardPresenceCallback);

            List<ChannelEntity> lstCe = new List<ChannelEntity>();
            lstCe.Add(ce1);
            string channelName = "ch1";

            if(checkErrorCallback || edit){
                var dictSM2 = new Dictionary<string, object>();
                dictSM2.Add("k2","v3");

                List<ChannelEntity> lstCe2 = new List<ChannelEntity>();
                lstCe2.Add(ce1);

                Helpers.UpdateOrAddUserStateOfEntity<T>(channelName, isChannelGroup, dictSM2, edit,
                    userCallback, ErrorCallbackUserState, PubnubErrorFilter.Level.Info
                    , ref lstCe2);
                string ustate = pubnub.JsonPluggableLibrary.SerializeToJsonString(lstCe2[0].ChannelParams.UserState);
                string state2 = pubnub.JsonPluggableLibrary.SerializeToJsonString(dictSM2);
                UnityEngine.Debug.Log(string.Format("{0}\n{1}", state2, ustate));
            }

            if(Helpers.UpdateOrAddUserStateOfEntity<T>(channelName, isChannelGroup, dictSM, edit,
                userCallback, ErrorCallbackUserState, PubnubErrorFilter.Level.Info
                , ref lstCe)){
                string ustate = pubnub.JsonPluggableLibrary.SerializeToJsonString(lstCe[0].ChannelParams.UserState);
                UnityEngine.Debug.Log(string.Format("{0}\n{1}", state, ustate));
                Assert.AreEqual(ustate, state, 
                    string.Format ("{0}\n{1}", ustate, state));
            } else {
                UnityEngine.Debug.Log(state);
                if(!checkErrorCallback){
                    Assert.True(false, "UpdateOrAddUserStateOfEntity returned false");
                }
            }
        }

        void ErrorCallbackUserState (PubnubClientError result)
        {
            UnityEngine.Debug.Log (string.Format("{0}", result.StatusCode));
            Assert.True(result.StatusCode.Equals(PubnubErrorCode.UserStateUnchanged), result.StatusCode.ToString());
        }


        [Test]
        public void TestCheckAndAddExistingUserStateEdit(){
            TestCheckAndAddExistingUserStateCommon<string>(false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCheckAndAddExistingUserStateEditObj(){
            TestCheckAndAddExistingUserStateCommon<object>(false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        public void TestCheckAndAddExistingUserStateCommon<T>( bool edit, Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback
        ){
            var dictSM = new Dictionary<string, object>();
            dictSM.Add("k","v");
            dictSM.Add("k2","v2");
            List<ChannelEntity> lstCE;

            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                true
            );
            string state = "{\"ch1\":{\"k\":\"v\",\"k2\":\"v2\"},\"ch2\":{\"k\":\"v\",\"k2\":\"v2\"},\"cg1\":{\"k\":\"v\",\"k2\":\"v2\"},\"cg2\":{\"k\":\"v\",\"k2\":\"v2\"}}";

            string userstate;

            string[] ch = {"ch1", "ch2"};
            string[] cg = {"cg1", "cg2"};
            bool stateChanged = Helpers.CheckAndAddExistingUserState<T>(string.Join(",",ch), 
                string.Join(",",cg), dictSM,  userCallback, 
                ErrorCallbackUserState,PubnubErrorFilter.Level.Info
                , edit, out userstate, out lstCE);

            bool ceFound = true;
            foreach(ChannelEntity ch2 in lstCE){
                bool chFound = false;
                bool cgFound = false;
                if(!ch2.ChannelID.IsChannelGroup){
                    foreach(string channel in ch){
                        if(channel.Equals(ch2.ChannelID.ChannelOrChannelGroupName)
                        )
                        {
                            UnityEngine.Debug.Log (string.Format("{0} found", channel));
                            chFound = true;
                            break;
                        }
                    }
                } else {
                    foreach(string channel in cg){
                        if(channel.Equals(ch2.ChannelID.ChannelOrChannelGroupName)
                        )
                        {
                            UnityEngine.Debug.Log (string.Format("{0} found", channel));
                            cgFound = true;
                            break;
                        }
                    }
                }

                if(!chFound && !cgFound){
                    ceFound = false;
                    break;
                }
            }
            bool userStateMatch = userstate.Equals(state);
            string resp = string.Format("{0} {1} {2} {3} {4}", ceFound, userStateMatch, 
                stateChanged, userstate, state);
            UnityEngine.Debug.Log(resp);
            Assert.True(ceFound & userStateMatch & stateChanged , resp);

        }

        [Test]
        public void TestCreateChannelEntity(){
            TestCreateChannelEntityCommon<string>(false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityObj(){
            TestCreateChannelEntityCommon<object>(false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityCG(){
            TestCreateChannelEntityCommon<string>(true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityObjCG(){
            TestCreateChannelEntityCommon<object>(true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityPres(){
            TestCreateChannelEntityCommon<string>(false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityObjPres(){
            TestCreateChannelEntityCommon<object>(false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityCGPres(){
            TestCreateChannelEntityCommon<string>(true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityObjCGPres(){
            TestCreateChannelEntityCommon<object>(true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        public void TestCreateChannelEntityCommon<T>( bool isChannelGroup, bool isAwaitingConnectCallback,
            bool isPresence, Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback
        ){
            var dictSM = new Dictionary<string, object>();
            dictSM.Add("k","v");
            dictSM.Add("k2","v2");

            string channelName = "ch1";
            if(isPresence){
                channelName = "ch1-pnpres";
            }

            ChannelEntity ce1 = Helpers.CreateChannelEntity<T>(channelName, isAwaitingConnectCallback, 
                isChannelGroup, dictSM, 
                userCallback, connectCallback,
                ErrorCallbackUserState, disconnectCallback, 
                wildcardPresenceCallback);
            CreateChannelEntityMatch<T>(ce1, isChannelGroup, isAwaitingConnectCallback, isPresence, channelName,
                userCallback,
                connectCallback,
                disconnectCallback, 
                wildcardPresenceCallback);

        }

        [Test]
        public void TestCreateChannelEntityCGPresMulti(){
            TestCreateChannelEntityMultiCommon<string>(true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityObjCGPresMulti(){
            TestCreateChannelEntityMultiCommon<object>(true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityMulti(){
            TestCreateChannelEntityMultiCommon<string>(false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityObjMulti(){
            TestCreateChannelEntityMultiCommon<object>(false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityPresMulti(){
            TestCreateChannelEntityMultiCommon<string>(false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityObjPresMulti(){
            TestCreateChannelEntityMultiCommon<object>(false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityPresMultiCb(){
            TestCreateChannelEntityMultiCommon<string>(false, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityObjPresMultiCb(){
            TestCreateChannelEntityMultiCommon<object>(false, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityCGPresMultiCb(){
            TestCreateChannelEntityMultiCommon<string>(true, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityObjCGPresMultiCb(){
            TestCreateChannelEntityMultiCommon<object>(true, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        public void TestCreateChannelEntityMultiCommon<T>( bool isChannelGroup, bool isAwaitingConnectCallback,
            bool isPresence, Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback
        ){
            var dictSM = new Dictionary<string, object>();
            dictSM.Add("k","v");
            dictSM.Add("k2","v2");

            string channelName = "ch1";

            if(isPresence){
                channelName = "ch1-pnpres";
            }
            string[] channelArr = {channelName};


            List<ChannelEntity> ce1 = Helpers.CreateChannelEntity<T>(channelArr, isAwaitingConnectCallback, 
                isChannelGroup, dictSM, 
                userCallback, connectCallback,
                ErrorCallbackUserState, disconnectCallback, 
                wildcardPresenceCallback);
            CreateChannelEntityMatch<T>(ce1[0], isChannelGroup, isAwaitingConnectCallback, isPresence, channelName,
                userCallback,
                connectCallback,
                disconnectCallback, 
                wildcardPresenceCallback);

        }

        void CreateChannelEntityMatch<T>(ChannelEntity ce1, bool isChannelGroup, bool isAwaitingConnectCallback,
            bool isPresence, string channelName, Action<T> userCallback, Action<T> connectCallback,
            Action<T> disconnectCallback, Action<T> wildcardPresenceCallback){

            bool chMatch = ce1.ChannelID.ChannelOrChannelGroupName.Equals(channelName);
            bool isAwaitingConnectCallbackMatch = ce1.ChannelParams.IsAwaitingConnectCallback.Equals(isAwaitingConnectCallback);
            bool isPresenceMatch = ce1.ChannelID.IsPresenceChannel.Equals(isPresence);
            bool isChannelGroupMatch = ce1.ChannelID.IsChannelGroup.Equals(isChannelGroup);
            bool typeMatch = ce1.ChannelParams.TypeParameterType.Equals(typeof(T));

            PubnubChannelCallback<T> channelCallbacks = ce1.ChannelParams.Callbacks as PubnubChannelCallback<T>;
            bool UserCallbackMatch = channelCallbacks.SuccessCallback == userCallback;
            bool ConnectCallbackMatch = channelCallbacks.ConnectCallback == connectCallback;
            bool ErrorCallbackUserStateMatch = channelCallbacks.ErrorCallback == ErrorCallbackUserState;
            bool DisconnectCallbackMatch = channelCallbacks.DisconnectCallback == disconnectCallback;
            bool WildcardPresenceCallbackMatch = channelCallbacks.WildcardPresenceCallback == wildcardPresenceCallback;

            var userState = ce1.ChannelParams.UserState as Dictionary<string, object>;
            bool userStateMatch = false;
            if(userState["k"].Equals("v") && userState["k2"].Equals("v2")){
                userStateMatch = true;
            }

            string resp = string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}", chMatch, 
                isAwaitingConnectCallbackMatch, 
                isPresenceMatch, isChannelGroupMatch, typeMatch,
                UserCallbackMatch, ConnectCallbackMatch, ErrorCallbackUserStateMatch,
                DisconnectCallbackMatch, WildcardPresenceCallbackMatch
            );
            UnityEngine.Debug.Log(resp);
            Assert.True(chMatch &
                isAwaitingConnectCallbackMatch &
                isPresenceMatch & isChannelGroupMatch & typeMatch &
                UserCallbackMatch & ConnectCallbackMatch & ErrorCallbackUserStateMatch &
                DisconnectCallbackMatch & WildcardPresenceCallbackMatch, resp);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribe(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(false, false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObj(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(false, false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeCG(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(true, false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjCG(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(true, false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribePres(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(false, true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjPres(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(false, true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeCGPres(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(true, true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjCGPres(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(true, true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeUnsub(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(false, false, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjUnsub(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(false, false, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeCGUnsub(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(true, false, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjCGUnsub(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(true, false, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribePresUnsub(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(false, true, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjPresUnsub(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(false, true, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeCGPresUnsub(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(true, true, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjCGPresUnsub(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(true, true, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(false, false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(false, false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeCGErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(true, false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjCGErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(true, false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribePresErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(false, true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjPresErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(false, true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeCGPresErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(true, true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjCGPresErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(true, true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeUnsubErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(false, false, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjUnsubErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(false, false, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeCGUnsubErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(true, false, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjCGUnsubErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(true, false, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribePresUnsubErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(false, true, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjPresUnsubErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(false, true, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeCGPresUnsubErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(true, true, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjCGPresUnsubErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(true, true, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        public void TestCreateChannelEntityAndAddToSubscribeCommon<T>(bool isChannelGroup,
            bool isPresence, bool isUnsubscribe, bool testErrorCB, Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback
        ){
            //subscribe
            //resubscribe -> Already sub

            //Subscribe

            //unsubscribe -> not subscribed

            //subscribe
            //unsubscrube

            //presence
            Subscription.Instance.CleanUp();
            ResponseType resp = ResponseType.SubscribeV2;
            if(isPresence && isUnsubscribe){
                resp = ResponseType.PresenceUnsubscribe;
            } else if(isPresence) {
                resp = ResponseType.PresenceV2;
            } else if(isUnsubscribe) {
                resp = ResponseType.Unsubscribe;
            }

            Action<PubnubClientError> errcb = Common.ErrorCallback;;

            string[] ch = {"ch1", "ch2"};
            string[] chpres = new string[ch.Length];
            for(int i=0; i<ch.Length; i++){
                chpres[i] = string.Format("{0}{1}", ch[i], Utility.PresenceChannelSuffix);
            }
            List<ChannelEntity> lstCE = new List<ChannelEntity>();
            if(testErrorCB && isUnsubscribe){
                errcb = ErrorCallbackCENotSub;
            } else if(testErrorCB){
                Helpers.CreateChannelEntityAndAddToSubscribe<T>(resp, ch, isChannelGroup, 
                    userCallback, connectCallback, 
                    Common.ErrorCallback, 
                    wildcardPresenceCallback,
                    disconnectCallback, PubnubErrorFilter.Level.Info, false, ref lstCE);
                errcb = ErrorCallbackCEAlreadySub;
                Subscription.Instance.Add (lstCE);
                lstCE.Clear();
            } else if(isUnsubscribe){
                Helpers.CreateChannelEntityAndAddToSubscribe<T>(resp, ch, isChannelGroup, 
                    userCallback, connectCallback, 
                    Common.ErrorCallback, 
                    wildcardPresenceCallback,
                    disconnectCallback, PubnubErrorFilter.Level.Info, false, ref lstCE);
                errcb = Common.ErrorCallback;
                Subscription.Instance.Add (lstCE);
                lstCE.Clear();
            }

            bool retBool = Helpers.CreateChannelEntityAndAddToSubscribe<T>(resp, ch, isChannelGroup, 
                userCallback, connectCallback, 
                errcb, 
                wildcardPresenceCallback,
                disconnectCallback, PubnubErrorFilter.Level.Info, isUnsubscribe, ref lstCE);

            bool ceFound = ParseListCE(lstCE, ch, chpres, isPresence, isChannelGroup, testErrorCB);
            string logStr = string.Format("{0} {1}", ceFound, retBool);
            UnityEngine.Debug.Log(logStr);
            if(!testErrorCB){
                Assert.True(ceFound & retBool, logStr);
            } else {
                Assert.True(ceFound & !retBool, logStr);
            }
        }

        public static bool ParseListCE(List<ChannelEntity> lstCE, string[] ch, string[] chpres,
            bool isPresence, bool isChannelGroup, bool testErrorCB
        ){
            bool ceFound = true;
            foreach(ChannelEntity ch2 in lstCE){
                if(isChannelGroup && !ch2.ChannelID.IsChannelGroup){
                    continue;
                }
                if(!isChannelGroup && ch2.ChannelID.IsChannelGroup){
                    continue;
                }

                bool chFound = false;
                for(int i=0; i<ch.Length; i++){
                    if(((!isPresence) && ch[i].Equals(ch2.ChannelID.ChannelOrChannelGroupName)) 
                        || ((isPresence) && chpres[i].Equals(ch2.ChannelID.ChannelOrChannelGroupName)))
                    {
                        bool presenceMatch = (isPresence)?ch2.ChannelID.IsPresenceChannel:true;
                        bool cgMatch = (isChannelGroup)?ch2.ChannelID.IsChannelGroup:true;
                        chFound = cgMatch & presenceMatch;
                        UnityEngine.Debug.Log (string.Format("{0} found, {1}, {2}, {3}", ch[i],
                            chFound, presenceMatch, cgMatch
                        ));
                        break;
                    }
                }
                if(!chFound){
                    UnityEngine.Debug.Log (string.Format("{0} not found", ch2.ChannelID.ChannelOrChannelGroupName

                    ));
                    ceFound = false;
                    break;
                }
            }
            return ceFound;
        }

        void ErrorCallbackCENotSub (PubnubClientError result)
        {
            PubnubErrorCode errorType =  PubnubErrorCode.NotSubscribed;
            if(result.Channel.Contains(Utility.PresenceChannelSuffix) 
                || result.ChannelGroup.Contains(Utility.PresenceChannelSuffix) ){
                errorType = PubnubErrorCode.NotPresenceSubscribed;
            }
            string logstr = string.Format("{0} {1}", result.StatusCode, (int)errorType);
            UnityEngine.Debug.Log (logstr);
            Assert.True(result.StatusCode.Equals((int)errorType), logstr);
        }

        void ErrorCallbackCEAlreadySub (PubnubClientError result)
        {
            
            PubnubErrorCode errorType =  PubnubErrorCode.AlreadySubscribed;
            if(result.Channel.Contains(Utility.PresenceChannelSuffix) 
                || result.ChannelGroup.Contains(Utility.PresenceChannelSuffix) ){
                errorType = PubnubErrorCode.AlreadyPresenceSubscribed;
            }
            string logstr = string.Format("{0} {1}", result.StatusCode, (int)errorType);
            UnityEngine.Debug.Log (logstr);
            Assert.True(result.StatusCode.Equals((int)errorType), logstr);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannels(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<string>(true, false, false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsObj(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<object>(true, false, false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCG(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<string>(false, true, false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsObjCG(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<object>(false, true, false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCGnCH(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<string>(true, true, false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsObjCGnCH(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<object>(true, true, false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsPres(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<string>(true, false, true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsObjPres(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<object>(true, false, true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCGPres(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<string>(false, true, true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsObjCGPres(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<object>(false, true, true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCGnCHPres(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<string>(true, true, true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsObjCGnCHPres(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<object>(true, true, true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<T>(bool testChannelGroup,
            bool testChannel,
            bool isPresence, bool isUnsubscribe, bool testErrorCB, Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback
        ){
            string[] ch = {"ch1", "ch2", "ch1", "ch2"};
            string[] chpres = new string[ch.Length];
            for(int i=0; i<ch.Length; i++){
                chpres[i] = string.Format("{0}{1}", ch[i], Utility.PresenceChannelSuffix);
            }
            string[] cg = {"cg1", "cg2", "cg1", "cg2"};
            string[] cgpres = new string[cg.Length];
            for(int i=0; i<cg.Length; i++){
                cgpres[i] = string.Format("{0}{1}", cg[i], Utility.PresenceChannelSuffix);
            }

            if(!testChannel){
                ch = null;
            }

            if(!testChannelGroup){
                cg = null;
            }

            List<ChannelEntity> lstCE;
            ResponseType resp = ResponseType.SubscribeV2;
            if(isPresence && isUnsubscribe){
                resp = ResponseType.PresenceUnsubscribe;
            } else if(isPresence) {
                resp = ResponseType.PresenceV2;
            } else if(isUnsubscribe) {
                resp = ResponseType.Unsubscribe;
            }

            bool retBool = Helpers.RemoveDuplicatesCheckAlreadySubscribedAndGetChannels(resp, userCallback,
                connectCallback, ErrorCallbackDup, wildcardPresenceCallback, disconnectCallback, ch, cg,
                PubnubErrorFilter.Level.Info, isUnsubscribe, out lstCE);

            bool ceFound = true;
            if(testChannel){
                ceFound = ParseListCE(lstCE, ch, chpres, isPresence, false, false);
            }
            bool cgFound = true;
            if(testChannelGroup){
                cgFound = ParseListCE(lstCE, cg, cgpres, isPresence, true, false);
            }

            string logStr = string.Format("{0} {1} {2}", ceFound, cgFound, retBool);
            UnityEngine.Debug.Log(logStr);
            //if(!testErrorCB){
                Assert.True(ceFound & cgFound & retBool, logStr);
            //}


        } 

        void ErrorCallbackDup (PubnubClientError result)
        {

            PubnubErrorCode errorType =  PubnubErrorCode.DuplicateChannel;
            if(!string.IsNullOrEmpty(result.ChannelGroup)){
                errorType = PubnubErrorCode.DuplicateChannelGroup;
            }
            string logstr = string.Format("{0} {1}", result.StatusCode, (int)errorType);
            UnityEngine.Debug.Log (logstr);
            Assert.True(result.StatusCode.Equals((int)errorType), logstr);

        }

        [Test]
        public void TestProcessResponseCallbacksV2(){
            TestProcessResponseCallbacksV2Common<string>(false, true, false, true, false, true,
                UserCallbackProcessReponse, ConnectCallbackProcessReponse, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestProcessResponseCallbacksV2WC(){
            TestProcessResponseCallbacksV2Common<string>(false, false, true, false, false, true,
                UserCallbackProcessReponseWC, ConnectCallbackProcessReponseWC, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestProcessResponseCallbacksV2WCPres(){
            TestProcessResponseCallbacksV2Common<string>(false, false, true, true, false, true,
                UserCallbackProcessReponseWC, ConnectCallbackProcessReponseWC, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestProcessResponseCallbacksV2CG(){
            TestProcessResponseCallbacksV2Common<string>(true, false, false, true, false, true,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        public void TestProcessResponseCallbacksV2Common<T>(bool testChannelGroup,
            bool testChannel, bool testwc,
            bool isPresence, bool isUnsubscribe, bool connectcallback, Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback
        ){
            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                true
            );

            List<ChannelEntity> channelEntities = Common.CreateListOfChannelEntities(true, true, 
                true, true, userCallback, connectCallback, wildcardPresenceCallback, disconnectCallback);
            
            string uuid = "CustomUUID";
            bool testUUID = false;

            ChannelEntity ce8 = Helpers.CreateChannelEntity<T>("ch8-pnpres", true, true, null, 
                userCallback, 
                connectCallback, 
                Common.ErrorCallback, 
                disconnectCallback, 
                wildcardPresenceCallback
                );
            channelEntities.Add(ce8);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelEntities, 
                ResponseType.SubscribeV2, false, 0, false, 0, typeof(T), uuid,
                userCallback, Common.ErrorCallback
            );
              
            TimetokenMetadata ott = new TimetokenMetadata(14691896187882984, "");
            TimetokenMetadata ptt = new TimetokenMetadata(14691896542063327, "");
            SubscribeMessage sm = new SubscribeMessage("0", "cg2", "ch2", "test-cg", "", "", "", 2, ott, ptt, null);
            SubscribeMessage sm2 = new SubscribeMessage("0", "ch2", "ch2", "test", "", "", "", 2, ott, ptt, null);
            SubscribeMessage sm3 = new SubscribeMessage("0", "ch2.*", "ch2", "test-wc", "", "", "", 2, ott, ptt, null);
            SubscribeMessage sm4 = new SubscribeMessage("0", "ch2.*", "ch2", "test-pnpres", "", "", "", 2, ott, ptt, null);
            List<SubscribeMessage> smLst = new List<SubscribeMessage>();
            if(testwc && isPresence){
                smLst.Add(sm4);
            }else if(testwc){    
                smLst.Add(sm3);
            }else if(testChannelGroup){
                smLst.Add(sm);
            } else if (testChannel){
                smLst.Add(sm2);
            }

            SubscribeEnvelope subscribeEnvelope = new SubscribeEnvelope ();

            subscribeEnvelope.Messages = smLst;
            subscribeEnvelope.TimetokenMeta = new TimetokenMetadata(14691897960994791,"");

            Helpers.ProcessResponseCallbacksV2(ref subscribeEnvelope, requestState, "", pubnub.JsonPluggableLibrary);
        }

        public  void UserCallbackProcessReponse (string result)
        {
            TestAssertions(result, false, false, false);

        }

        public  void UserCallbackProcessReponse (object result)
        {
            TestAssertions(result.ToString(), false, false, false);
        }

        public  void ConnectCallbackProcessReponse (string result)
        {
            TestAssertions(result, true, false, false);
        }

        public  void ConnectCallbackProcessReponse (object result)
        {
            TestAssertions(result.ToString(), true, false, false);
        }

        public  void UserCallbackProcessReponseWC (string result)
        {
            TestAssertions(result, false, true, false);

        }

        public  void UserCallbackProcessReponseWC (object result)
        {
            TestAssertions(result.ToString(), false, true, false);
        }

        public  void ConnectCallbackProcessReponseWC (string result)
        {
            TestAssertions(result, true, true, false);
        }

        public  void ConnectCallbackProcessReponseWC (object result)
        {
            TestAssertions(result.ToString(), true, true, false);
        }

        public  void UserCallbackProcessReponseCG (string result)
        {
            TestAssertions(result, false, false, true);

        }

        public  void UserCallbackProcessReponseCG (object result)
        {
            TestAssertions(result.ToString(), false, false, true);
        }

        public  void ConnectCallbackProcessReponseCG (string result)
        {
            TestAssertions(result, true, false, true);
        }

        public  void ConnectCallbackProcessReponseCG (object result)
        {
            TestAssertions(result.ToString(), true, false, true);
        }

        void TestAssertions(string result, bool connect, bool wc, bool cg){
            if(wc){
                if(connect){
                    UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result));
                    if(result.Contains(Utility.PresenceChannelSuffix)){
                        Assert.True(result.Contains("Presence Connected"));
                        Assert.True(result.Contains("ch2.*"));
                        Assert.True(result.Contains("ch2"));
                        Assert.True(result.Contains("test-pnpres"));

                    } else {
                        Assert.True(result.Contains("Connected"));
                        Assert.True(result.Contains("ch2.*"));
                        Assert.True(result.Contains("ch2"));
                        Assert.True(result.Contains("test-wc"));
                    }
                } else {
                    UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result));
                    if(result.Contains(Utility.PresenceChannelSuffix)){
                    } else {
                        Assert.True(result.Contains("ch2"));
                        Assert.True(result.Contains("test"));
                        Assert.True(result.Contains("14691897960994791"));

                    }
                }
            } else if (cg){
                if(connect){
                    UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result));
                    if(result.Contains(Utility.PresenceChannelSuffix)){
                        Assert.True(result.Contains("Presence Connected"));
                        Assert.True(result.Contains("ch8-pnpres"));
                    } else {
                        Assert.True(result.Contains("Connected"));
                        Assert.True(result.Contains("cg2"));
                        Assert.True(result.Contains("ch2"));
                    }
                } else {
                    UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result));
                    if(result.Contains(Utility.PresenceChannelSuffix)){
                    } else {
                        Assert.True(result.Contains("cg2"));
                        Assert.True(result.Contains("ch2"));
                        Assert.True(result.Contains("test-cg"));
                        Assert.True(result.Contains("14691897960994791"));

                    }
                }
            } else{
                if(connect){
                    UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result));
                    if(result.Contains(Utility.PresenceChannelSuffix)){
                        Assert.True(result.Contains("Presence Connected"));
                        Assert.True(result.Contains("ch8-pnpres"));
                    } else {
                        Assert.True(result.Contains("Connected"));
                        Assert.True(result.Contains("ch2"));
                    }
                } else {
                    UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result));
                    if(result.Contains(Utility.PresenceChannelSuffix)){
                    } else {
                        Assert.True(result.Contains("ch2"));
                        Assert.True(result.Contains("test"));
                        Assert.True(result.Contains("14691897960994791"));

                    }
                }
            }
        }

        #endif
    }
}

