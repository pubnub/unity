using System;
using PubNubAPI;
using NUnit.Framework;
using System.Collections.Generic;

namespace PubNubAPI.Tests
{
    [TestFixture]
    public class UtilityUnitTests
    {
        #if DEBUG  

        #if(UNITY_IOS)
        [Test]
        public void TestCheckTimeoutValue(){
            int v = Utility.CheckTimeoutValue(20);
            Assert.True (v.Equals (20));
        }

        [Test]
        public void TestCheckTimeoutValueGreaterThan60(){
            int v = Utility.CheckTimeoutValue(60);
            Assert.True (v.Equals (59));
        }
        #endif

        [Test]
        public void TestCheckDictionaryForErrorTrue(){
            Assert.IsTrue(TestCheckDictionaryForErrorCommon("error", true));
        }
        
        [Test]
        public void TestCheckDictionaryForErrorFalse(){
            Assert.IsFalse(TestCheckDictionaryForErrorCommon("error", false));
        }

        [Test]
        public void TestCheckDictionaryForEFalse(){
            Assert.IsFalse(TestCheckDictionaryForErrorCommon("e", false));
        }

        [Test]
        public void TestCheckDictionaryForETrue(){
            Assert.IsTrue(TestCheckDictionaryForErrorCommon("e", true));
        }

        public bool TestCheckDictionaryForErrorCommon(string name, bool bCheck){
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add(name, bCheck);
            return Utility.CheckDictionaryForError(dict, name);
        }

        [Test]
        public void TestReadMessageFromResponseDictionaryTestTrue(){
            object test = "test";
            Assert.IsTrue(test.ToString().Equals(TestReadMessageFromResponseDictionaryCommon("e", test)));
        }

        [Test]
        public void TestReadMessageFromResponseDictionaryTestFalse(){
            object test = null;
            Assert.IsTrue("".Equals(TestReadMessageFromResponseDictionaryCommon("e", test)));
        }


        public string TestReadMessageFromResponseDictionaryCommon(string name, object obj){
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add(name, obj);
            return Utility.ReadMessageFromResponseDictionary(dict, name);
        }

        [Test]
        public void TestCheckKeyAndParseLong (){
            var dict = new Dictionary<string, object>(); 
            dict.Add("s", 2);
            string log;
            long o;
            Utility.TryCheckKeyAndParseLong(dict, "seq", "s", out log, out o);
            Assert.True(o.Equals(2));
        }

        [Test]
        public void TestCheckKeyAndParseLongFalse (){
            var dict = new Dictionary<string, object>(); 
            dict.Add("s", "l");
            string log;
            long o;
            Utility.TryCheckKeyAndParseLong(dict, "seq", "s", out log, out o);
            Assert.True(o.Equals(0));
            UnityEngine.Debug.Log(log);
            Assert.True(log.Contains("seq, s conversion failed: "));
        }

        [Test]
        public void TestCheckKeyAndParseLongWrongKey (){
            var dict = new Dictionary<string, object>(); 
            dict.Add("s", "l");
            string log;
            long o;
            Utility.TryCheckKeyAndParseLong(dict, "seq", "sa", out log, out o);
            Assert.True(o.Equals(0));
        }

        [Test]
        public void TestCheckKeyAndParseInt (){
            var dict = new Dictionary<string, object>(); 
            dict.Add("s", 2);
            string log;
            int val;
            Assert.True(Utility.TryCheckKeyAndParseInt(dict, "seq", "s", out log, out val));
            Assert.True(val.Equals(2));
        }

        [Test]
        public void TestCheckKeyAndParseIntFalse (){
            var dict = new Dictionary<string, object>(); 
            dict.Add("s", "l");
            string log;
            int val;
            Assert.False(Utility.TryCheckKeyAndParseInt(dict, "seq", "s", out log, out val));
            Assert.True(val.Equals(0));
            UnityEngine.Debug.Log(log);
            Assert.True(log.Contains("seq, s conversion failed: "));
        }

        [Test]
        public void TestCheckKeyAndParseIntWrongKey (){
            var dict = new Dictionary<string, object>(); 
            dict.Add("s", "l");
            string log;
            int val;
            Assert.False(Utility.TryCheckKeyAndParseInt(dict, "seq", "sa", out log, out val));
            Assert.True(val.Equals(0));
            UnityEngine.Debug.Log(log);
            Assert.True(log.Contains("seq, sa key not found."));
        }
        
        [Test]
        public void TestCheckAndAddNameSpaceEmpty(){
            List<string> ls = Utility.CheckAndAddNameSpace("");
            Assert.True(ls==null);
        }

        [Test]
        public void TestCheckAndAddNameSpace(){
            List<string> ls = Utility.CheckAndAddNameSpace("ns");
            Assert.True(ls.Contains("ns") && ls.Contains("namespace"));
        }

        [Test]
        public void TestCheckChannelGroupConvertToPres(){
            string s = Utility.CheckChannelGroup("cg", true);
            Assert.True(s.Contains("cg-pnpres"));
        }

        [Test]
        public void TestCheckChannelGroup(){
            string s = Utility.CheckChannelGroup("cg", false);
            Assert.True(s.Contains("cg") && !s.Contains("cg-pnpres"));
        }

        [Test]
        public void TestCheckChannelGroupConvertToPresMulti(){
            string s = Utility.CheckChannelGroup("cg, cg2", true);
            Assert.True(
                s.Contains("cg-pnpres")
                && s.Contains("cg2-pnpres")
            );
        }

        [Test]
        public void TestCheckChannelGroupMulti(){
            string s = Utility.CheckChannelGroup("cg, cg2", false);
            Assert.True(
                s.Contains("cg") 
                && !s.Contains("cg-pnpres")
                && s.Contains("cg2") 
                && !s.Contains("cg2-pnpres"));
        }

        [Test]
        public void TestCheckChannelGroupMissingMemberException(){

            var ex = Assert.Throws<MissingMemberException>(() => Utility.CheckChannelGroup(",", false)); 

            Assert.That(ex.Message.Contains("Invalid channel group"), ex.Message, null);
        }

        [Test]
        public void TestCheckChannelGroupMissingMembeExcep(){
            var ex = Assert.Throws<MissingMemberException>(() => Utility.CheckChannelGroup("ch, ", false)); 

            Assert.That(ex.Message.Contains("Invalid channel group"), ex.Message, null);
        }

        [Test]
        public void TestCheckChannelOrChannelGroupBothEmpty(){
            var ex = Assert.Throws<ArgumentException>(() => Utility.CheckChannelOrChannelGroup("", "")); 

            Assert.That(ex.Message.Contains("Both Channel and ChannelGroup are empty"), ex.Message, null);
        }

        [Test]
        public void TestCheckChannelOrChannelGroupCG(){
            Utility.CheckChannelOrChannelGroup("", "cg");
            //Assert.True(true);
        }

        [Test]
        public void TestCheckChannelOrChannelGroupCH(){
            Utility.CheckChannelOrChannelGroup("ch", "");
            Assert.True(true);
        }

        [Test]
        public void TestCheckChannelOrChannelGroupCHnCG(){
            Utility.CheckChannelOrChannelGroup("ch", "cg");
            Assert.True(true);
        }

        [Test]
        public void TestValidateTimetoken (){
            long o = Utility.ValidateTimetoken("14685037252884276", false);
            Assert.True(o.Equals(14685037252884276));
        }

        [Test]
        public void TestValidateTimetokenRaiseError (){
            var ex = Assert.Throws<ArgumentException>(() => Utility.ValidateTimetoken("a", true)); 

            Assert.That(ex.Message.Contains("Invalid timetoken"), ex.Message, null);
        }

        [Test]
        public void TestValidateTimetokenNoError (){
            long o = Utility.ValidateTimetoken("", false);
            Assert.True(o.Equals(0));
        }

        [Test]
        public void TestValidateTimetokenEmpty (){
            long o = Utility.ValidateTimetoken("", true);
            Assert.True(o.Equals(0));
        }

        [Test]
        public void TestCheckChannelOrChannelGroupFalse ()
        {
            var ex = Assert.Throws<ArgumentException>(() =>  Utility.CheckChannelOrChannelGroup ("", "")); 

            Assert.That(ex.Message.Contains("Both Channel and ChannelGroup are empty"), ex.Message, null);
        }

        [Test]
        public void TestCheckChannelFalse ()
        {
            var ex = Assert.Throws<ArgumentException>(() =>  Utility.CheckChannel ("")); 

            Assert.That(ex.Message.Contains("Missing Channel"), ex.Message, null);
        }

        [Test]
        public void TestCheckChannelOrChannelGroupTrueCHCG ()
        {
            Utility.CheckChannelOrChannelGroup ("ch", "cg");
            Assert.True (true);
        }

        [Test]
        public void TestCheckChannelOrChannelGroupTrueCH ()
        {
            Utility.CheckChannelOrChannelGroup ("ch", "");
            Assert.True (true);
        }

        [Test]
        public void TestCheckChannelOrChannelGroupTrueCG ()
        {
            Utility.CheckChannelOrChannelGroup ("", "cg");
            Assert.True (true);
        }

        [Test]
        public void TestCheckChannelsTrue ()
        {
            Utility.CheckChannels (new string[]{"ch"});
            Assert.True (true);
        }

        [Test]
        public void TestCheckChannelsFalse ()
        {
            var ex = Assert.Throws<ArgumentException>(() =>  Utility.CheckChannels (new string[]{})); 

            Assert.That(ex.Message.Contains("Missing channel(s)"), ex.Message, null);
        }

        [Test]
        public void TestCheckChannelTrue ()
        {
            Utility.CheckChannel ("ch");
            Assert.True (true);
        }

        [Test]
        public void TestCheckMessage ()
        {
            var ex = Assert.Throws<ArgumentException>(() =>  Utility.CheckMessage (null)); 

            Assert.That(ex.Message.Contains("Message is null"), ex.Message, null);
        }

        [Test]
        public void TestCheckPublishKey ()
        {
            var ex = Assert.Throws<MissingMemberException>(() =>  Utility.CheckPublishKey (null)); 

            Assert.That(ex.Message.Contains("Invalid publish key"), ex.Message, null);
        }

        [Test]
        public void TestGenerateGuid ()
        {
            Assert.IsTrue(Utility.GenerateGuid ().ToString() != "");
        }

        [Test]
        public void TestIsPresenceChannelTrue ()
        {

            Assert.IsTrue(Utility.IsPresenceChannel ("my_channel-pnpres"));
        }

        [Test]
        public void TestIsPresenceChannelFalse ()
        {

            Assert.IsFalse(Utility.IsPresenceChannel ("my_channel"));
        }

        [Test]
        public void TestIsUnsafeWithComma ()
        {
            RunUnsafeTests (false);
        }

        [Test]
        public void TestIsUnsafe ()
        {
            RunUnsafeTests (true);
        }

        void RunUnsafeTests(bool ignoreComma)
        {
            char[] ch = {',', ' ','~','`','!','@','#','$','%','^','&','*','(',')','+','=','[',']','\\','{','}','|',';','\'',':','\"','/','<','>','?'};

            bool bPass = true;
            char currentChar = ' ';
            foreach (char c in ch) {
                currentChar = c;
                if (ignoreComma && c.Equals (',')) {
                    continue;
                }
                if (!Utility.IsUnsafe (c, ignoreComma)) {
                    bPass = false;
                    break;
                }
            }
            if (bPass) {
                Assert.True(bPass);
            } else {
                Assert.Fail(string.Format("failed for {0}", currentChar));
            }
        }

        [Test]
        public void TestEncodeUricomponent ()
        {
            //test unsafe surrogate and normal test
            string expected = "Text%20with%20\ud83d\ude1c%20emoji%20\ud83c\udf89.%20testencode%20%7E%60%21%40%23%24%25%5E%26%2A%28%29%2B%3D%5B%5D%5C%7B%7D%7C%3B%27%3A%22%2F%3C%3E%3F";
            string received = Utility.EncodeUricomponent("Text with 😜 emoji 🎉. testencode ~`!@#$%^&*()+=[]\\{}|;':\"/<>?", PNOperationType.PNSubscribeOperation, true, true);
            UnityEngine.Debug.Log (received);
            Assert.IsTrue(expected.Equals(received));
        }

        [Test]
        public void TestEncodeUricomponentDetailedHistoryIgnorePercentFalse ()
        {
            //test unsafe surrogate and normal test
            string expected = "Text%20with%20\ud83d\ude1c%20emoji%20\ud83c\udf89.%20testencode%20%7E%60%21%40%23%24%25%5E%26%2A%28%29%2B%3D%5B%5D%5C%7B%7D%7C%3B%27%3A%22%252F%3C%3E%3F";
            string received = Utility.EncodeUricomponent("Text with 😜 emoji 🎉. testencode ~`!@#$%^&*()+=[]\\{}|;':\"/<>?", PNOperationType.PNHistoryOperation, true, false);
            UnityEngine.Debug.Log (received);
            Assert.IsTrue(expected.Equals(received));
        }

        [Test]
        public void TestEncodeUricomponentDetailedHistoryIgnorePercentTrue ()
        {
            //test unsafe surrogate and normal test
            string expected = "Text%20with%20\ud83d\ude1c%20emoji%20\ud83c\udf89.%20testencode%20%7E%60%21%40%23%24%25%5E%26%2A%28%29%2B%3D%5B%5D%5C%7B%7D%7C%3B%27%3A%22%2F%3C%3E%3F";
            string received = Utility.EncodeUricomponent("Text with 😜 emoji 🎉. testencode ~`!@#$%^&*()+=[]\\{}|;':\"/<>?", PNOperationType.PNHistoryOperation, true, true);
            UnityEngine.Debug.Log (received);
            Assert.IsTrue(expected.Equals(received));
        }

        [Test]
        public void TestEncodeUricomponentPushGetIgnorePercentFalse ()
        {
            //test unsafe surrogate and normal test
            string expected = "Text%20with%20\ud83d\ude1c%20emoji%20\ud83c\udf89.%20testencode%20%7E%60%21%40%23%24%25%5E%26%2A%28%29%2B%3D%5B%5D%5C%7B%7D%7C%3B%27%3A%22%252F%3C%3E%3F";
            string received = Utility.EncodeUricomponent("Text with 😜 emoji 🎉. testencode ~`!@#$%^&*()+=[]\\{}|;':\"/<>?", PNOperationType.PNPushNotificationEnabledChannelsOperation, true, false);
            UnityEngine.Debug.Log (received);
            Assert.IsTrue(expected.Equals(received));
        }

        [Test]
        public void TestEncodeUricomponentPushGetIgnorePercentTrue ()
        {
            //test unsafe surrogate and normal test
            string expected = "Text%20with%20\ud83d\ude1c%20emoji%20\ud83c\udf89.%20testencode%20%7E%60%21%40%23%24%25%5E%26%2A%28%29%2B%3D%5B%5D%5C%7B%7D%7C%3B%27%3A%22%2F%3C%3E%3F";
            string received = Utility.EncodeUricomponent("Text with 😜 emoji 🎉. testencode ~`!@#$%^&*()+=[]\\{}|;':\"/<>?", PNOperationType.PNPushNotificationEnabledChannelsOperation, true, true);
            UnityEngine.Debug.Log (received);
            Assert.IsTrue(expected.Equals(received));
        }

        [Test]
        public void TestEncodeUricomponentPushRemoveIgnorePercentFalse ()
        {
            //test unsafe surrogate and normal test
            string expected = "Text%20with%20\ud83d\ude1c%20emoji%20\ud83c\udf89.%20testencode%20%7E%60%21%40%23%24%25%5E%26%2A%28%29%2B%3D%5B%5D%5C%7B%7D%7C%3B%27%3A%22%252F%3C%3E%3F";
            string received = Utility.EncodeUricomponent("Text with 😜 emoji 🎉. testencode ~`!@#$%^&*()+=[]\\{}|;':\"/<>?", PNOperationType.PNRemoveAllPushNotificationsOperation, true, false);
            UnityEngine.Debug.Log (received);
            Assert.IsTrue(expected.Equals(received));
        }

        [Test]
        public void TestEncodeUricomponentPushRemoveIgnorePercentTrue ()
        {
            //test unsafe surrogate and normal test
            string expected = "Text%20with%20\ud83d\ude1c%20emoji%20\ud83c\udf89.%20testencode%20%7E%60%21%40%23%24%25%5E%26%2A%28%29%2B%3D%5B%5D%5C%7B%7D%7C%3B%27%3A%22%2F%3C%3E%3F";
            string received = Utility.EncodeUricomponent("Text with 😜 emoji 🎉. testencode ~`!@#$%^&*()+=[]\\{}|;':\"/<>?", PNOperationType.PNRemoveAllPushNotificationsOperation, true, true);
            UnityEngine.Debug.Log (received);
            Assert.IsTrue(expected.Equals(received));
        }

        [Test]
        public void TestEncodeUricomponentPushRegisterIgnorePercentFalse ()
        {
            //test unsafe surrogate and normal test
            string expected = "Text%20with%20\ud83d\ude1c%20emoji%20\ud83c\udf89.%20testencode%20%7E%60%21%40%23%24%25%5E%26%2A%28%29%2B%3D%5B%5D%5C%7B%7D%7C%3B%27%3A%22%252F%3C%3E%3F";
            string received = Utility.EncodeUricomponent("Text with 😜 emoji 🎉. testencode ~`!@#$%^&*()+=[]\\{}|;':\"/<>?", PNOperationType.PNAddPushNotificationsOnChannelsOperation, true, false);
            UnityEngine.Debug.Log (received);
            Assert.IsTrue(expected.Equals(received));
        }

        [Test]
        public void TestEncodeUricomponentPushRegisterIgnorePercentTrue ()
        {
            //test unsafe surrogate and normal test
            string expected = "Text%20with%20\ud83d\ude1c%20emoji%20\ud83c\udf89.%20testencode%20%7E%60%21%40%23%24%25%5E%26%2A%28%29%2B%3D%5B%5D%5C%7B%7D%7C%3B%27%3A%22%2F%3C%3E%3F";
            string received = Utility.EncodeUricomponent("Text with 😜 emoji 🎉. testencode ~`!@#$%^&*()+=[]\\{}|;':\"/<>?", PNOperationType.PNRemovePushNotificationsFromChannelsOperation, true, true);
            UnityEngine.Debug.Log (received);
            Assert.IsTrue(expected.Equals(received));
        }

        [Test]
        public void TestEncodeUricomponentPushUnregisterIgnorePercentFalse ()
        {
            //test unsafe surrogate and normal test
            string expected = "Text%20with%20\ud83d\ude1c%20emoji%20\ud83c\udf89.%20testencode%20%7E%60%21%40%23%24%25%5E%26%2A%28%29%2B%3D%5B%5D%5C%7B%7D%7C%3B%27%3A%22%252F%3C%3E%3F";
            string received = Utility.EncodeUricomponent("Text with 😜 emoji 🎉. testencode ~`!@#$%^&*()+=[]\\{}|;':\"/<>?", PNOperationType.PNRemovePushNotificationsFromChannelsOperation, true, false);
            UnityEngine.Debug.Log (received);
            Assert.IsTrue(expected.Equals(received));
        }

        [Test]
        public void TestEncodeUricomponentPushUnregisterIgnorePercentTrue ()
        {
            //test unsafe surrogate and normal test
            string expected = "Text%20with%20\ud83d\ude1c%20emoji%20\ud83c\udf89.%20testencode%20%7E%60%21%40%23%24%25%5E%26%2A%28%29%2B%3D%5B%5D%5C%7B%7D%7C%3B%27%3A%22%2F%3C%3E%3F";
            string received = Utility.EncodeUricomponent("Text with 😜 emoji 🎉. testencode ~`!@#$%^&*()+=[]\\{}|;':\"/<>?", PNOperationType.PNRemovePushNotificationsFromChannelsOperation, true, true);
            UnityEngine.Debug.Log (received);
            Assert.IsTrue(expected.Equals(received));
        }

        [Test]
        public void TestEncodeUricomponentHereNowIgnorePercentFalse ()
        {
            //test unsafe surrogate and normal test
            string expected = "Text%20with%20\ud83d\ude1c%20emoji%20\ud83c\udf89.%20testencode%20%7E%60%21%40%23%24%25%5E%26%2A%28%29%2B%3D%5B%5D%5C%7B%7D%7C%3B%27%3A%22%252F%3C%3E%3F";

            string received = Utility.EncodeUricomponent("Text with 😜 emoji 🎉. testencode ~`!@#$%^&*()+=[]\\{}|;':\"/<>?", PNOperationType.PNHereNowOperation, true, false);
            UnityEngine.Debug.Log (received);
            Assert.IsTrue(expected.Equals(received));
        }

        [Test]
        public void TestEncodeUricomponentHereNowIgnorePercentTrue ()
        {
            //test unsafe surrogate and normal test
            string expected = "Text%20with%20\ud83d\ude1c%20emoji%20\ud83c\udf89.%20testencode%20%7E%60%21%40%23%24%25%5E%26%2A%28%29%2B%3D%5B%5D%5C%7B%7D%7C%3B%27%3A%22%2F%3C%3E%3F";
            string received = Utility.EncodeUricomponent("Text with 😜 emoji 🎉. testencode ~`!@#$%^&*()+=[]\\{}|;':\"/<>?", PNOperationType.PNHereNowOperation, true, true);
            UnityEngine.Debug.Log (received);
            Assert.IsTrue(expected.Equals(received));
        }

        [Test]
        public void TestEncodeUricomponentLeaveIgnorePercentFalse ()
        {
            //test unsafe surrogate and normal test
            string expected = "Text%20with%20\ud83d\ude1c%20emoji%20\ud83c\udf89.%20testencode%20%7E%60%21%40%23%24%25%5E%26%2A%28%29%2B%3D%5B%5D%5C%7B%7D%7C%3B%27%3A%22%252F%3C%3E%3F";
            string received = Utility.EncodeUricomponent("Text with 😜 emoji 🎉. testencode ~`!@#$%^&*()+=[]\\{}|;':\"/<>?", PNOperationType.PNLeaveOperation, true, false);
            UnityEngine.Debug.Log (received);
            Assert.IsTrue(expected.Equals(received));
        }

        [Test]
        public void TestEncodeUricomponentLeaveIgnorePercentTrue ()
        {
            //test unsafe surrogate and normal test
            string expected = "Text%20with%20\ud83d\ude1c%20emoji%20\ud83c\udf89.%20testencode%20%7E%60%21%40%23%24%25%5E%26%2A%28%29%2B%3D%5B%5D%5C%7B%7D%7C%3B%27%3A%22%2F%3C%3E%3F";
            string received = Utility.EncodeUricomponent("Text with 😜 emoji 🎉. testencode ~`!@#$%^&*()+=[]\\{}|;':\"/<>?", PNOperationType.PNLeaveOperation, true, true);
            UnityEngine.Debug.Log (received);
            Assert.IsTrue(expected.Equals(received));
        }

        [Test]
        public void TestEncodeUricomponentPresenceHeartbeatIgnorePercentFalse ()
        {
            //test unsafe surrogate and normal test
            string expected = "Text%20with%20\ud83d\ude1c%20emoji%20\ud83c\udf89.%20testencode%20%7E%60%21%40%23%24%25%5E%26%2A%28%29%2B%3D%5B%5D%5C%7B%7D%7C%3B%27%3A%22%252F%3C%3E%3F";
            string received = Utility.EncodeUricomponent("Text with 😜 emoji 🎉. testencode ~`!@#$%^&*()+=[]\\{}|;':\"/<>?", PNOperationType.PNPresenceHeartbeatOperation, true, false);
            UnityEngine.Debug.Log (received);
            Assert.IsTrue(expected.Equals(received));
        }

        [Test]
        public void TestEncodeUricomponentPresenceHeartbeatIgnorePercentTrue ()
        {
            //test unsafe surrogate and normal test
            string expected = "Text%20with%20\ud83d\ude1c%20emoji%20\ud83c\udf89.%20testencode%20%7E%60%21%40%23%24%25%5E%26%2A%28%29%2B%3D%5B%5D%5C%7B%7D%7C%3B%27%3A%22%2F%3C%3E%3F";
            string received = Utility.EncodeUricomponent("Text with 😜 emoji 🎉. testencode ~`!@#$%^&*()+=[]\\{}|;':\"/<>?", PNOperationType.PNPresenceHeartbeatOperation, true, true);
            UnityEngine.Debug.Log (received);
            Assert.IsTrue(expected.Equals(received));
        }

        [Test]
        public void TestMd5 ()
        {
            //test unsafe surrogate and normal test
            string expected = "83a644046796c6a0d76bc161f73b75b4";
            string received = Utility.Md5("test md5");
            UnityEngine.Debug.Log (received);
            Assert.IsTrue(expected.Equals(received));
        }

        [Test]
        public void TestTranslateDateTimeToSeconds ()
        {
            //test unsafe surrogate and normal test
            long expected = 1449792000;
            long received = Utility.TranslateDateTimeToSeconds(DateTime.Parse("11 Dec 2015"));
            UnityEngine.Debug.Log (received);
            Assert.IsTrue(expected.Equals(received));
        }

        [Test]
        public void TranslateDateTimeToUnixTime ()
        {
            UnityEngine.Debug.Log ("Running TranslateDateTimeToUnixTime()");
            //Test for 26th June 2012 GMT
            DateTime dt = new DateTime (2012, 6, 26, 0, 0, 0, DateTimeKind.Utc);
            long nanoSecondTime = PubNub.TranslateDateTimeToPubnubUnixNanoSeconds (dt);
            Assert.True ((13406688000000000).Equals (nanoSecondTime));
        }

        [Test]
        public void TranslateUnixTimeToDateTime ()
        {
            UnityEngine.Debug.Log ("Running TranslateUnixTimeToDateTime()");
            //Test for 26th June 2012 GMT
            DateTime expectedDate = new DateTime (2012, 6, 26, 0, 0, 0, DateTimeKind.Utc);
            DateTime actualDate = PubNub.TranslatePubnubUnixNanoSecondsToDateTime (13406688000000000);
            Assert.True (expectedDate.ToString ().Equals (actualDate.ToString ()));
        }

        [Test]
        public void TestCheckKeyAndConvertObjToStringArrNull ()
        {
            UnityEngine.Debug.Log ("Running TranslateUnixTimeToDateTime()");
            Assert.True (Utility.CheckKeyAndConvertObjToStringArr (null) == null);
        }

        [Test]
        public void TestCheckKeyAndConvertObjToStringArr ()
        {
            UnityEngine.Debug.Log ("Running TranslateUnixTimeToDateTime()");
            string[] strArr = {"Client-odx4y", "test"};
            object obj = (object)strArr;
            List<string> lst= Utility.CheckKeyAndConvertObjToStringArr (obj);
            Assert.True (lst.Count == 2);
            if (lst.Count > 1) {
                Assert.True (lst [0].Equals("Client-odx4y"));
                Assert.True (lst [1].Equals("test"));
            } else {
                Assert.Fail ("lst.Count <1");
            }
        }

        #endif
    }
}
