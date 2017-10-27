using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using PubNubAPI;

namespace PubNubAPI.Tests
{
    [TestFixture]
    public class SubscriptionTests
    {
        #if DEBUG
        /*[Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresRemove (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, true, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresAlreadySubscribed (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, true, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNew (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel3";
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, false, true, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresRemoveObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, true, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresAlreadySubscribedObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, true, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNewObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel3";
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, false, true, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNoNetRemove (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, false, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNoNetAlreadySubscribed (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, false, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNoNetNew (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel3";
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, false, false, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNoNetRemoveObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, false, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNoNetAlreadySubscribedObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, false, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNoNetNewObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel3";
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, false, false, ResponseType.PresenceV2);
        }
            
        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsRemove (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, true, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsAlreadySubscribed (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, true, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNew (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel3";
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, false, true, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsRemoveObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, true, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsAlreadySubscribedObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, true, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNewObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel3";
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, false, true, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNoNetRemove (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, false, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNoNetAlreadySubscribed (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, false, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNoNetNew (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel3";
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, false, false, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNoNetRemoveObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, false, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNoNetAlreadySubscribedObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, false, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNoNetNewObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel3";
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, false, false, ResponseType.SubscribeV2);
        }

        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<T> (string channel,
            string[] multiChannel, bool fireCallback, bool networkConnection, ResponseType responseType
        ){ 

            //Remove duplicate channels
            //Already subscribed
            //New channel
            readCallback = fireCallback;
            List<string> validChannels = new List<string> ();
            validChannels.Add (channel);

            PubnubErrorFilter.Level errorLevel = PubnubErrorFilter.Level.Info;

            Action<PubnubClientError> errorcb = ErrorCallbackCommonExceptionHandler;
            if (!readCallback) {
                errorcb = null;
            }
            List<ChannelEntity> channelEntities;

            Helpers.RemoveDuplicatesCheckAlreadySubscribedAndGetChannels<T> (responseType, null, null, 
                errorcb, null, null, multiChannel, null, errorLevel, false, out channelEntities
            );

            List<ChannelEntity> channelEntities2;

            Helpers.RemoveDuplicatesCheckAlreadySubscribedAndGetChannels<T> (responseType, null, null, 
                errorcb, null, null, new string[] {channel}, null, errorLevel, false, out channelEntities2
            );

            if (!readCallback) {
                string channels2 = Helpers.GetNamesFromChannelEntities(channelEntities2, false);

                bool channelMatch = false;
                if (channelEntities != null) {
                    foreach (ChannelEntity c in channelEntities2) {
                        string ch2= c.ChannelID.ChannelOrChannelGroupName;
                        if(c.ChannelID.IsPresenceChannel){
                            channel = channel + Utility.PresenceChannelSuffix;
                        }
                        channelMatch = channel.Equals(ch2);
                        if(channelMatch)
                            break;
                    }
                }
                UnityEngine.Debug.Log ("not fireCallback:" +channelMatch + channels2 + channel);
                Assert.IsTrue (channelMatch);

            }
        }
        [Test]
        public void TestSubscription(){
            TestSubscriptionCommon<string>(Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestSubscriptionObj(){
            TestSubscriptionCommon<object>(Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        public void TestSubscriptionCommon<T>(Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback){
            Subscription.Instance.CleanUp();
            //Add CE with ch cg, ch-pres, cgpres, 2 awaiting connect callback with userstate
            List<ChannelEntity> channelEntities = Common.CreateListOfChannelEntities<T>(true, true, true, true,
                userCallback, connectCallback, 
                wildcardPresenceCallback,
                disconnectCallback
                );
            Subscription.Instance.Add(channelEntities);

            // Test All
            RunAssertions("{\"ch1\":{\"k\":\"v\",\"k2\":\"v2\"},\"ch2\":{\"k3\":\"v3\",\"k4\":\"v4\"},\"ch2-pnpres\":{\"k7\":\"v7\",\"k8\":\"v8\"},\"ch7\":{\"k7\":\"v7\",\"k8\":\"v8\"},\"cg1\":{\"k5\":\"v5\",\"k6\":\"v6\"},\"cg2\":{\"k7\":\"v7\",\"k8\":\"v8\"},\"cg2-pnpres\":{\"k7\":\"v7\",\"k8\":\"v8\"},\"cg8\":{\"k7\":\"v7\",\"k8\":\"v8\"}}",
                false, true, true, true, true, 4, 4,
                new string[] {"cg1", "cg2", "cg2-pnpres", "cg8" },
                new string[] {"ch1", "ch2", "ch2-pnpres", "ch7" },
                new string[] {"ch1", "ch2", "ch7", "cg1", "cg2", "cg8" },
                new string[] { "cg2-pnpres", "ch2-pnpres"},
                new string[] {"ch7", "cg8" },
                new string[] {"ch1", "ch2", "ch2-pnpres", "ch7", "cg1", "cg2", "cg2-pnpres", "cg8" });


            // Delete 2
            ChannelEntity ce = Helpers.CreateChannelEntity("ch2", false, false, null, userCallback,
                connectCallback, Common.ErrorCallback, disconnectCallback, wildcardPresenceCallback
            );
            Subscription.Instance.Delete(ce);
            ChannelEntity ce2 = Helpers.CreateChannelEntity("cg2", false, true, null, userCallback,
                connectCallback, Common.ErrorCallback, disconnectCallback, wildcardPresenceCallback
            );
            Subscription.Instance.Delete(ce2);

            // Test All
            RunAssertions("{\"ch1\":{\"k\":\"v\",\"k2\":\"v2\"},\"ch2-pnpres\":{\"k7\":\"v7\",\"k8\":\"v8\"},\"ch7\":{\"k7\":\"v7\",\"k8\":\"v8\"},\"cg1\":{\"k5\":\"v5\",\"k6\":\"v6\"},\"cg2-pnpres\":{\"k7\":\"v7\",\"k8\":\"v8\"},\"cg8\":{\"k7\":\"v7\",\"k8\":\"v8\"}}",
                false, true, true, true, true, 3, 3,
                new string[] {"cg1", "cg2-pnpres", "cg8" },
                new string[] {"ch1", "ch2-pnpres", "ch7" },
                new string[] {"ch1", "ch7", "cg1", "cg8" },
                new string[] { "cg2-pnpres", "ch2-pnpres"},
                new string[] {"ch7", "cg8" },
                new string[] {"ch1", "ch2-pnpres", "ch7", "cg1", "cg2-pnpres", "cg8" });
            

            // UpdateOrAddUserStateOfEntity
            ChannelEntity ce3 = Helpers.CreateChannelEntity("ch1", false, false, null, userCallback,
                connectCallback, Common.ErrorCallback, disconnectCallback, wildcardPresenceCallback
            );
            var dictSMN = new Dictionary<string, object>();
            dictSMN.Add("k","v9");

            Subscription.Instance.UpdateOrAddUserStateOfEntity(ref ce3, dictSMN, true);

            // Test All
            RunAssertions("{\"ch1\":{\"k\":\"v9\",\"k2\":\"v2\"},\"ch2-pnpres\":{\"k7\":\"v7\",\"k8\":\"v8\"},\"ch7\":{\"k7\":\"v7\",\"k8\":\"v8\"},\"cg1\":{\"k5\":\"v5\",\"k6\":\"v6\"},\"cg2-pnpres\":{\"k7\":\"v7\",\"k8\":\"v8\"},\"cg8\":{\"k7\":\"v7\",\"k8\":\"v8\"}}",
                false, true, true, true, true, 3, 3,
                new string[] {"cg1", "cg2-pnpres", "cg8" },
                new string[] {"ch1", "ch2-pnpres", "ch7" },
                new string[] {"ch1", "ch7", "cg1", "cg8" },
                new string[] { "cg2-pnpres", "ch2-pnpres"},
                new string[] {"ch7", "cg8" },
                new string[] {"ch1", "ch2-pnpres", "ch7", "cg1", "cg2-pnpres", "cg8" });

            // UpdateIsAwaitingConnectCallbacksOfEntity
            ChannelEntity ce4 = Helpers.CreateChannelEntity("ch7", false, false, null, userCallback,
                connectCallback, Common.ErrorCallback, disconnectCallback, wildcardPresenceCallback
            );
            ChannelEntity ce5 = Helpers.CreateChannelEntity("cg8", false, true, null, userCallback,
                connectCallback, Common.ErrorCallback, disconnectCallback, wildcardPresenceCallback
            );

            List<ChannelEntity> lstCE = new List<ChannelEntity>();
            lstCE.Add(ce4);
            lstCE.Add(ce5);

            Subscription.Instance.UpdateIsAwaitingConnectCallbacksOfEntity(lstCE, false);

            // Test All
            RunAssertions("{\"ch1\":{\"k\":\"v9\",\"k2\":\"v2\"},\"ch2-pnpres\":{\"k7\":\"v7\",\"k8\":\"v8\"},\"ch7\":{\"k7\":\"v7\",\"k8\":\"v8\"},\"cg1\":{\"k5\":\"v5\",\"k6\":\"v6\"},\"cg2-pnpres\":{\"k7\":\"v7\",\"k8\":\"v8\"},\"cg8\":{\"k7\":\"v7\",\"k8\":\"v8\"}}",
                true, true, true, true, true, 3, 3,
                new string[] {"cg1", "cg2-pnpres", "cg8" },
                new string[] {"ch1", "ch2-pnpres", "ch7" },
                new string[] {"ch1", "ch7", "cg1", "cg8" },
                new string[] { "cg2-pnpres", "ch2-pnpres"},
                new string[] {"ch7", "cg8" },
                new string[] {"ch1", "ch2-pnpres", "ch7", "cg1", "cg2-pnpres", "cg8" });

            // CleanUp
            Subscription.Instance.CleanUp();

            // Test All

            RunAssertions(String.Empty,
                false, false, false, false, false, 0, 0,
                new string[] {},
                new string[] {},
                new string[] {},
                new string[] {},
                new string[] {},
                new string[] {});

        }

        public void RunAssertions(string compiledUserStateA,
            bool connectCallbackSentA, bool hasChannelGroupsA, bool hasChannelA,
            bool hasChannelsOrChannelGroupsA, bool hasPresenceChannelsA, int currentSubscribedChannelGroupsCountA,
            int currentSubscribedChannelsCountA, string[] allChannelGroupsA, string[] allChannelsA, 
            string[] allNonPresenceChannelsOrChannelGroupsA, string[] allPresenceChannelsOrChannelGroupsA,
            string[] channelsAndChannelGroupsAwaitingConnectCallbackA, string[] allSubscribedChannelsAndChannelGroupsA

        ){
            List<ChannelEntity> allChannelGroups = Subscription.Instance.AllChannelGroups;
            List<ChannelEntity> allChannels = Subscription.Instance.AllChannels;
            List<ChannelEntity> allNonPresenceChannelsOrChannelGroups = Subscription.Instance.AllNonPresenceChannelsOrChannelGroups;
            List<ChannelEntity> allPresenceChannelsOrChannelGroups = Subscription.Instance.AllPresenceChannelsOrChannelGroups;
            List<ChannelEntity> allSubscribedChannelsAndChannelGroups = Subscription.Instance.AllSubscribedChannelsAndChannelGroups;
            List<ChannelEntity> channelsAndChannelGroupsAwaitingConnectCallback = Subscription.Instance.ChannelsAndChannelGroupsAwaitingConnectCallback;
            string compiledUserState = Subscription.Instance.CompiledUserState;
            bool connectCallbackSent = Subscription.Instance.ConnectCallbackSent;
            int currentSubscribedChannelGroupsCount = Subscription.Instance.CurrentSubscribedChannelGroupsCount;
            int currentSubscribedChannelsCount = Subscription.Instance.CurrentSubscribedChannelsCount;
            bool hasChannelGroups = Subscription.Instance.HasChannelGroups;
            bool hasChannel = Subscription.Instance.HasChannels;
            bool hasChannelsOrChannelGroups = Subscription.Instance.HasChannelsOrChannelGroups;
            bool hasPresenceChannels = Subscription.Instance.HasPresenceChannels;

            Assert.True(compiledUserState.Equals(compiledUserStateA), compiledUserState);
            Assert.True(connectCallbackSent.Equals(connectCallbackSentA), connectCallbackSent.ToString());
            Assert.True(hasChannelGroups.Equals(hasChannelGroupsA), hasChannelGroups.ToString());
            Assert.True(hasChannel.Equals(hasChannelA), hasChannel.ToString());
            Assert.True(hasChannelsOrChannelGroups.Equals(hasChannelsOrChannelGroupsA), hasChannelsOrChannelGroups.ToString());
            Assert.True(hasPresenceChannels.Equals(hasPresenceChannelsA), hasPresenceChannels.ToString());
            Assert.True(currentSubscribedChannelGroupsCount.Equals(currentSubscribedChannelGroupsCountA), currentSubscribedChannelGroupsCount.ToString());
            Assert.True(currentSubscribedChannelsCount.Equals(currentSubscribedChannelsCountA), currentSubscribedChannelsCount.ToString());

            bool allChannelGroupsFound = ParseListCE(allChannelGroups, 
                allChannelGroupsA);

            Assert.True(allChannelGroupsFound);

            bool allChannelsFound = ParseListCE(allChannels, 
                allChannelsA);

            Assert.True(allChannelsFound);

            bool allNonPresenceChannelsOrChannelGroupsFound = ParseListCE(allNonPresenceChannelsOrChannelGroups, 
                allNonPresenceChannelsOrChannelGroupsA);

            Assert.True(allNonPresenceChannelsOrChannelGroupsFound);

            bool allPresenceChannelsOrChannelGroupsFound = ParseListCE(allPresenceChannelsOrChannelGroups, 
                allPresenceChannelsOrChannelGroupsA);

            Assert.True(allPresenceChannelsOrChannelGroupsFound);

            bool channelsAndChannelGroupsAwaitingConnectCallbackFound = ParseListCE(channelsAndChannelGroupsAwaitingConnectCallback, 
                channelsAndChannelGroupsAwaitingConnectCallbackA);

            Assert.True(channelsAndChannelGroupsAwaitingConnectCallbackFound);

            bool allSubscribedChannelsAndChannelGroupsFound = ParseListCE(allSubscribedChannelsAndChannelGroups, 
                allSubscribedChannelsAndChannelGroupsA);

            Assert.True(allSubscribedChannelsAndChannelGroupsFound);
        }

        public static bool ParseListCE(List<ChannelEntity> lstCE, string[] ch
            
        ){
            bool ceFound = true;
            foreach(ChannelEntity ch2 in lstCE){
                
                bool chFound = false;
                for(int i=0; i<ch.Length; i++){
                    bool isPresence = ch.Contains(Utility.PresenceChannelSuffix);
                    bool isChannelGroup = ch.Contains("cg");
                    if(ch[i].Equals(ch2.ChannelID.ChannelOrChannelGroupName)) 
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
        }*/


        #endif
    }
}

