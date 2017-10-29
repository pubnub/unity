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

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe400CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "test message", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, false, 0, false);
        }

        public void TestResponseCallbackErrorOrTimeoutHandler<T>(string[] channelGroups, string message, string[] channels, bool resumeOnReconnect, PNOperationType responseType, PNCurrentRequestType crt, bool isTimeout, bool isError, long timetoken, bool ssl){
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
            //pnConfiguration.AuthKey = authKey;

            //PubNub pn = new PubNub(pnConfiguration);
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

            List<ChannelEntity> channelEntities = Helpers.CreateChannelEntity(channels, true, false, null, ref pnLog);
            List<ChannelEntity> channelGroupEntities = Helpers.CreateChannelEntity(channelGroups, true, true, null, ref pnLog);  

            if((channelEntities != null) && (channelGroupEntities != null)){
                channelEntities.AddRange(channelGroupEntities);
            } else if(channelEntities == null) {
                channelEntities = channelGroupEntities;
            }

            RequestState requestState = new RequestState();
            requestState.Reconnect = resumeOnReconnect;
            requestState.OperationType = responseType;

            CustomEventArgs cea = new CustomEventArgs ();
            cea.PubNubRequestState = requestState;
            cea.Message = message;
            cea.IsError = isError;
            cea.IsTimeout = isTimeout;
            cea.CurrRequestType = crt;
            PNStatus pnStatus;
            Helpers.CheckErrorTypeAndCallback<T> (cea, pnUnity, out pnStatus);

            /*if (responseType == ResponseType.PresenceV2 || responseType == ResponseType.SubscribeV2) {
                DateTime dt = DateTime.Now;
                while (dt.AddSeconds(2) > DateTime.Now) {
                    UnityEngine.Debug.Log ("waiting");
                }
            }*/
            
            /*////*bool channelMatch = false;
            bool channelGroupMatch = false;
            if (pnStatus != null) {
                if(pnStatus.AffectedChannelGroups != null){
                    channelGroupMatch = pnStatus.AffectedChannelGroups.Contains(c.ChannelID.ChannelOrChannelGroupName);
                }
                foreach (ChannelEntity c in mea.channelEntities) {
                    channelMatch = ExceptionChannel.Contains(c.ChannelID.ChannelOrChannelGroupName);
                    channelGroupMatch = ExceptionChannelGroups.Contains(c.ChannelID.ChannelOrChannelGroupName);
                    if(channelMatch || channelGroupMatch)
                        continue;
                }
            }
            string channels1 = Helpers.GetNamesFromChannelEntities(mea.channelEntities, false);

            UnityEngine.Debug.Log (string.Format("mea.responseType.Equals (CRequestType) {0}\n" +
                "channelMatch {1}\n" +
                "mea.resumeOnReconnect.Equals(ResumeOnReconnect) {2}\n"
                , mea.responseType.Equals (responseType),
            channelMatch,
                mea.resumeOnReconnect.Equals(ResumeOnReconnect)));

            UnityEngine.Debug.Log (string.Format ("HandleMultiplexException LOG: \n" +
                "mea.responseType.Equals (CRequestType): {0} \n" +
                "channelMatch: {1} \n" +
                "mea.resumeOnReconnect.Equals(ResumeOnReconnect): {2} \n" +
                "CRequestType:{3} \n" +
                "ExceptionChannel: {4} \n" +
                "ResumeOnReconnect: {5} \n" +
                "mea.responseType: {6} \n" +
                "channels: {7} \n" +
                "mea.resumeOnReconnect: {8} \n" +
                "resultPart1: {9} \n" +
                "channelGroupMatch: {10}\n",
                "channelGroups: {11}\n",
                mea.responseType.Equals (responseType),
                channelMatch,
                mea.resumeOnReconnect.Equals(ResumeOnReconnect), responseType.ToString(), 
                ExceptionChannel, ResumeOnReconnect, mea.responseType,
                channels1, mea.resumeOnReconnect, resultPart1,
                channelGroupMatch,
                ExceptionChannelGroups
                ));
            bool resultPart2 = false;
            if (mea.responseType.Equals (responseType)
                && (string.IsNullOrEmpty(ExceptionChannel))?true:channelMatch
                && (string.IsNullOrEmpty(ExceptionChannelGroups))?true:channelGroupMatch
                && mea.resumeOnReconnect.Equals (ResumeOnReconnect)) {
                resultPart2 = true;
            }
            Assert.IsTrue (resultPart1 && resultPart2);
        }    */

        
        #endif
    }
}
