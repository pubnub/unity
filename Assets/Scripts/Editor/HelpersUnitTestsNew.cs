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
            TestBuildJsonUserStateCommon<object>(false, true);
        }

        [Test]
        public void TestBuildJsonUserStateCECGObj(){
            TestBuildJsonUserStateCommon<object>(true, false);
        }

        [Test]
        public void TestBuildJsonUserStateCECGnCHObj(){
            TestBuildJsonUserStateCommon<object>(true, true);
        }

        [Test]
        public void TestBuildJsonUserStateCE(){
            TestBuildJsonUserStateCommon<string>(false, true);
        }

        [Test]
        public void TestBuildJsonUserStateCECG(){
            TestBuildJsonUserStateCommon<string>(true, false);
        }

        [Test]
        public void TestBuildJsonUserStateCECGnCH(){
            TestBuildJsonUserStateCommon<string>(true, true);
        }

        public void TestBuildJsonUserStateCommon<T>(bool channelGroup, bool channel){
            List<ChannelEntity> lstCE= Common.CreateListOfChannelEntities<T>(channelGroup, channel);
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
            TestGetNamesFromChannelEntitiesCommon<object>(true, true);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCGObj(){
            TestGetNamesFromChannelEntitiesCommon<object>(true, false);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCNObj(){
            TestGetNamesFromChannelEntitiesCommon<object>(false, true);
        }

        [Test]
        public void TestGetNamesFromChannelEntities(){
            TestGetNamesFromChannelEntitiesCommon<string>(true, true);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCG(){
            TestGetNamesFromChannelEntitiesCommon<string>(true, false);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCN(){
            TestGetNamesFromChannelEntitiesCommon<string>(false, true);
        }

        public void TestGetNamesFromChannelEntitiesCommon<T>(bool channelGroup, bool channel){
            List<ChannelEntity> lstCE= Common.CreateListOfChannelEntities<T>(channelGroup, channel);    
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
            TestGetNamesFromChannelEntitiesCommon2<object>(true, false);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCN2Obj(){
            TestGetNamesFromChannelEntitiesCommon2<object>(false, true);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCG2(){
            TestGetNamesFromChannelEntitiesCommon2<string>(true, false);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCN2(){
            TestGetNamesFromChannelEntitiesCommon2<string>(false, true);
        }

        public void TestGetNamesFromChannelEntitiesCommon2<T>(bool channelGroup, bool channel){
            List<ChannelEntity> lstCE= Common.CreateListOfChannelEntities<T>(channelGroup, channel);    
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
            TestUpdateOrAddUserStateOfEntityCommon<object>(true, false, true, false);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityObj(){
            TestUpdateOrAddUserStateOfEntityCommon<object>(false, false, false, false);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityErrorCallbackEditObj(){
            TestUpdateOrAddUserStateOfEntityCommon<object>(true, true, false, false);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityEditObj(){
            TestUpdateOrAddUserStateOfEntityCommon<object>(false, true, false, false);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityErrorCallback(){
            TestUpdateOrAddUserStateOfEntityCommon<string>(true, false, true, false);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntity(){
            TestUpdateOrAddUserStateOfEntityCommon<string>(false, false, false, false);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityErrorCallbackEdit(){
            TestUpdateOrAddUserStateOfEntityCommon<string>(true, true, false, false);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityEdit(){
            TestUpdateOrAddUserStateOfEntityCommon<string>(false, true, false, false);
        }

        public void TestUpdateOrAddUserStateOfEntityCommon<T>(bool isChannelGroup, bool edit, 
            bool checkErrorCallback, bool ssl){

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
                Common.UserCallback, Common.ConnectCallback,
                ErrorCallbackUserState, Common.DisconnectCallback, 
                Common.WildcardPresenceCallback);

            List<ChannelEntity> lstCe = new List<ChannelEntity>();
            lstCe.Add(ce1);
            string channelName = "ch1";

            if(checkErrorCallback || edit){
                var dictSM2 = new Dictionary<string, object>();
                dictSM2.Add("k2","v3");

                List<ChannelEntity> lstCe2 = new List<ChannelEntity>();
                lstCe2.Add(ce1);

                Helpers.UpdateOrAddUserStateOfEntity<T>(channelName, isChannelGroup, dictSM2, edit,
                    Common.UserCallback, ErrorCallbackUserState, PubnubErrorFilter.Level.Info
                    , ref lstCe2);
                string ustate = pubnub.JsonPluggableLibrary.SerializeToJsonString(lstCe2[0].ChannelParams.UserState);
                string state2 = pubnub.JsonPluggableLibrary.SerializeToJsonString(dictSM2);
                UnityEngine.Debug.Log(string.Format("{0}\n{1}", state2, ustate));
            }

            if(Helpers.UpdateOrAddUserStateOfEntity<T>(channelName, isChannelGroup, dictSM, edit,
                Common.UserCallback, ErrorCallbackUserState, PubnubErrorFilter.Level.Info
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
            TestCheckAndAddExistingUserStateCommon<string>(false);
        }

        [Test]
        public void TestCheckAndAddExistingUserStateEditObj(){
            TestCheckAndAddExistingUserStateCommon<object>(false);
        }

        public void TestCheckAndAddExistingUserStateCommon<T>( bool edit 
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
                string.Join(",",cg), dictSM,  Common.UserCallback, 
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
            TestCreateChannelEntityCommon<string>(false, false, false);
        }

        [Test]
        public void TestCreateChannelEntityObj(){
            TestCreateChannelEntityCommon<object>(false, false, false);
        }

        [Test]
        public void TestCreateChannelEntityCG(){
            TestCreateChannelEntityCommon<string>(true, false, false);
        }

        [Test]
        public void TestCreateChannelEntityObjCG(){
            TestCreateChannelEntityCommon<object>(true, false, false);
        }

        [Test]
        public void TestCreateChannelEntityPres(){
            TestCreateChannelEntityCommon<string>(false, false, true);
        }

        [Test]
        public void TestCreateChannelEntityObjPres(){
            TestCreateChannelEntityCommon<object>(false, false, true);
        }

        [Test]
        public void TestCreateChannelEntityCGPres(){
            TestCreateChannelEntityCommon<string>(true, false, true);
        }

        [Test]
        public void TestCreateChannelEntityObjCGPres(){
            TestCreateChannelEntityCommon<object>(true, false, true);
        }


        public void TestCreateChannelEntityCommon<T>( bool isChannelGroup, bool isAwaitingConnectCallback,
            bool isPresence
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
                Common.UserCallback, Common.ConnectCallback,
                ErrorCallbackUserState, Common.DisconnectCallback, 
                Common.WildcardPresenceCallback);
            CreateChannelEntityMatch<T>(ce1, isChannelGroup, isAwaitingConnectCallback, isPresence, channelName);

        }

        [Test]
        public void TestCreateChannelEntityCGPresMulti(){
            TestCreateChannelEntityMultiCommon<string>(true, false, true);
        }

        [Test]
        public void TestCreateChannelEntityObjCGPresMulti(){
            TestCreateChannelEntityMultiCommon<object>(true, false, true);
        }

        [Test]
        public void TestCreateChannelEntityMulti(){
            TestCreateChannelEntityMultiCommon<string>(false, false, false);
        }

        [Test]
        public void TestCreateChannelEntityObjMulti(){
            TestCreateChannelEntityMultiCommon<object>(false, false, false);
        }

        [Test]
        public void TestCreateChannelEntityPresMulti(){
            TestCreateChannelEntityMultiCommon<string>(false, false, true);
        }

        [Test]
        public void TestCreateChannelEntityObjPresMulti(){
            TestCreateChannelEntityMultiCommon<object>(false, false, true);
        }

        [Test]
        public void TestCreateChannelEntityPresMultiCb(){
            TestCreateChannelEntityMultiCommon<string>(false, true, true);
        }

        [Test]
        public void TestCreateChannelEntityObjPresMultiCb(){
            TestCreateChannelEntityMultiCommon<object>(false, true, true);
        }

        [Test]
        public void TestCreateChannelEntityCGPresMultiCb(){
            TestCreateChannelEntityMultiCommon<string>(true, true, true);
        }

        [Test]
        public void TestCreateChannelEntityObjCGPresMultiCb(){
            TestCreateChannelEntityMultiCommon<object>(true, true, true);
        }

        public void TestCreateChannelEntityMultiCommon<T>( bool isChannelGroup, bool isAwaitingConnectCallback,
            bool isPresence
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
                Common.UserCallback, Common.ConnectCallback,
                ErrorCallbackUserState, Common.DisconnectCallback, 
                Common.WildcardPresenceCallback);
            CreateChannelEntityMatch<T>(ce1[0], isChannelGroup, isAwaitingConnectCallback, isPresence, channelName);

        }

        void CreateChannelEntityMatch<T>(ChannelEntity ce1, bool isChannelGroup, bool isAwaitingConnectCallback,
            bool isPresence, string channelName){
            bool chMatch = ce1.ChannelID.ChannelOrChannelGroupName.Equals(channelName);
            bool isAwaitingConnectCallbackMatch = ce1.ChannelParams.IsAwaitingConnectCallback.Equals(isAwaitingConnectCallback);
            bool isPresenceMatch = ce1.ChannelID.IsPresenceChannel.Equals(isPresence);
            bool isChannelGroupMatch = ce1.ChannelID.IsChannelGroup.Equals(isChannelGroup);
            bool typeMatch = ce1.ChannelParams.TypeParameterType.Equals(typeof(T));

            PubnubChannelCallback<T> channelCallbacks = ce1.ChannelParams.Callbacks as PubnubChannelCallback<T>;
            bool UserCallbackMatch = channelCallbacks.SuccessCallback == Common.UserCallback;
            bool ConnectCallbackMatch = channelCallbacks.ConnectCallback == Common.ConnectCallback;
            bool ErrorCallbackUserStateMatch = channelCallbacks.ErrorCallback == ErrorCallbackUserState;
            bool DisconnectCallbackMatch = channelCallbacks.DisconnectCallback == Common.DisconnectCallback;
            bool WildcardPresenceCallbackMatch = channelCallbacks.WildcardPresenceCallback == Common.WildcardPresenceCallback;

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

        #endif
    }
}

