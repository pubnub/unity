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

            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "test message", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, false, 0, false, ExceptionStatusCode);
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe414CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 414;
            TestResponseCallbackErrorOrTimeoutHandler<SubscribeEnvelope> (channelGroups, "414 response", channels, false, PNOperationType.PNSubscribeOperation, PNCurrentRequestType.Subscribe, false, false, 0, false, ExceptionStatusCode);
        }*/

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

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeFailedTOCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "timedout", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

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

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeFailedTOCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "timedout", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

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

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeFailedTO ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("timedout", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

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
            
            List<ChannelEntity> channelEntities1 = (channelEntities==null)? null : new List<ChannelEntity>(channelEntities);
            List<ChannelEntity> channelGroupEntities = Helpers.CreateChannelEntity(channelGroups, true, true, null, ref pnLog);  

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
            Helpers.CheckErrorTypeAndCallback<T> (cea, pnUnity, out pnStatus);

            /*if (responseType == ResponseType.PresenceV2 || responseType == ResponseType.SubscribeV2) {
                DateTime dt = DateTime.Now;
                while (dt.AddSeconds(2) > DateTime.Now) {
                    UnityEngine.Debug.Log ("waiting");
                }
            }*/
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
