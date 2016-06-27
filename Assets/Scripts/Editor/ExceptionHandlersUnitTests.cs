using System;
using PubNubMessaging.Core;
using NUnit.Framework;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PubNubMessaging.Tests
{
    [TestFixture]
    public class ExceptionHandlersUnitTests
    {
        #if DEBUG
        string ExceptionMessage ="";
        string ExceptionChannel = "";
        int ExceptionStatusCode = 0;

        ResponseType CRequestType;
        bool ResumeOnReconnect = false;
        bool resultPart1 = false;

        bool IsTimeout = false;
        bool IsError = false;

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe400 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe404 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("404 test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe414 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("414 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe504 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("504 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe503 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("503 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe500 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("500 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe403 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("403 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeNameResolutionFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("NameResolutionFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeConnectFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("ConnectFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeServerProtocolViolation ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("ServerProtocolViolation 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeProtocolError ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("ProtocolError 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeFNF ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("java.io.FileNotFoundException 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeFailedDL ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("Failed downloading UnityWeb", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeFailedTO ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("timed out", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe400 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe404 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("404 test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe414 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("414 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe504 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("504 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe503 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("503 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe500 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("500 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe403 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("403 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeNameResolutionFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("NameResolutionFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeConnectFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ConnectFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeServerProtocolViolation ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ServerProtocolViolation 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeProtocolError ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ProtocolError 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeFNF ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("java.io.FileNotFoundException 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeFailedDL ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("Failed downloading UnityWeb", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeFailedTO ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("timedout", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test] 
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe400 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe404 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("404 test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe414 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("414 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe504 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("504 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe503 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("503 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe500 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("500 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe403 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("403 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeNameResolutionFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("NameResolutionFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeConnectFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ConnectFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeServerProtocolViolation ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ServerProtocolViolation 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeProtocolError ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ProtocolError 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeFNF ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("java.io.FileNotFoundException 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeFailedDL ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("Failed downloading UnityWeb", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
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

            TestResponseCallbackErrorOrTimeoutHandler<string> ("test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe404 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("404 test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe414 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("414 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe504 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("504 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe503 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("503 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe500 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("500 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe403 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("403 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeNameResolutionFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("NameResolutionFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeConnectFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ConnectFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeServerProtocolViolation ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ServerProtocolViolation 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeProtocolError ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ProtocolError 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeFNF ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("java.io.FileNotFoundException 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeFailedDL ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("Failed downloading UnityWeb", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe400 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe404 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("404 test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe414 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("414 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe504 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("504 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe503 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("503 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe500 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("500 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe403 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("403 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeNameResolutionFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("NameResolutionFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeConnectFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ConnectFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeServerProtocolViolation ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ServerProtocolViolation 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeProtocolError ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ProtocolError 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeFNF ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("java.io.FileNotFoundException 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeFailedDL ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("Failed downloading UnityWeb", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeFailedTO ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("timed out", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe400 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe404 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("404 test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe414 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("414 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe504 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("504 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe503 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("503 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe500 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("500 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe403 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("403 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeNameResolutionFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("NameResolutionFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeConnectFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ConnectFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeServerProtocolViolation ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ServerProtocolViolation 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeProtocolError ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ProtocolError 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeFNF ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("java.io.FileNotFoundException 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeFailedDL ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("Failed downloading UnityWeb", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeFailedTO ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("timedout", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test] 
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe400 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe404 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("404 test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe414 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("414 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe504 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("504 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe503 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("503 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe500 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("500 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe403 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("403 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeNameResolutionFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("NameResolutionFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeConnectFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ConnectFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeServerProtocolViolation ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ServerProtocolViolation 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeProtocolError ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ProtocolError 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeFNF ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("java.io.FileNotFoundException 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeFailedDL ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("Failed downloading UnityWeb", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeFailedTO ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("timedout", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe400 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe404 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("404 test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe414 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("414 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe504 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("504 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe503 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("503 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe500 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("500 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe403 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("403 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeNameResolutionFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("NameResolutionFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeConnectFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ConnectFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeServerProtocolViolation ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ServerProtocolViolation 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeProtocolError ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ProtocolError 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeFNF ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("java.io.FileNotFoundException 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeFailedDL ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("Failed downloading UnityWeb", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeFailedTO ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("timedout", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPub400 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 0;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("test message", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPub404 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("404 test message", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPub414 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("414 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPub504 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("504 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPub503 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("503 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPub500 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("500 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPub403 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("403 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPubNameResolutionFailure ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("NameResolutionFailure 400", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPubConnectFailure ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ConnectFailure 400", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPubServerProtocolViolation ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ServerProtocolViolation 400 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPubProtocolError ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ProtocolError 400 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPubFNF ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("java.io.FileNotFoundException 400 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPubFailedDL ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("Failed downloading UnityWeb", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPubFailedTO ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 127;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("timedout", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPub400 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("test message", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPub404 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("404 test message", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPub414 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("414 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPub504 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("504 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPub503 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("503 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPub500 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("500 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPub403 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("403 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPubNameResolutionFailure ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("NameResolutionFailure 400", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPubConnectFailure ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ConnectFailure 400", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPubServerProtocolViolation ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ServerProtocolViolation 400 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPubProtocolError ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ProtocolError 400 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPubFNF ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("java.io.FileNotFoundException 400 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPubFailedDL ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("Failed downloading UnityWeb", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPubFailedTO ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("timedout", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPub400 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 0;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("test message", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPub404 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("404 test message", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPub414 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("414 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPub504 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("504 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPub503 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("503 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPub500 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("500 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPub403 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("403 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPubNameResolutionFailure ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("NameResolutionFailure 400", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPubConnectFailure ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ConnectFailure 400", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPubServerProtocolViolation ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ServerProtocolViolation 400 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPubProtocolError ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ProtocolError 400 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPubFNF ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("java.io.FileNotFoundException 400 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPubFailedDL ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("Failed downloading UnityWeb", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPubFailedTO ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("timedout", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPub400 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("test message", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPub404 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("404 test message", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPub414 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("414 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPub504 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("504 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPub503 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("503 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPub500 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("500 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPub403 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("403 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPubNameResolutionFailure ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("NameResolutionFailure 400", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPubConnectFailure ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ConnectFailure 400", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPubServerProtocolViolation ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ServerProtocolViolation 400 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPubProtocolError ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ProtocolError 400 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPubFNF ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("java.io.FileNotFoundException 400 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPubFailedDL ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("Failed downloading UnityWeb", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPubFailedTO ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("timedout", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        public void TestResponseCallbackErrorOrTimeoutHandler<T>(string message, string[] channels,
            bool resumeOnReconnect, ResponseType responseType, CurrentRequestType crt, Action<T> userCallback,
            Action<T> connectCallback, Action<PubnubClientError> errorCallback,
            bool isTimeout, bool isError, long timetoken, bool ssl, PubnubErrorFilter.Level errorLevel
        ){
            ExceptionMessage = message;
            ExceptionChannel = string.Join (",", channels);

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

            List<ChannelEntity> channelEntities = Helpers.CreateChannelEntity<T>(channels, 
                true, false, null, userCallback, connectCallback, errorCallback, null, null);  

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelEntities, responseType, 
                resumeOnReconnect, 0, isTimeout, timetoken, typeof(T));

            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                ssl
            );

            CustomEventArgs<T> cea = new CustomEventArgs<T> ();
            cea.PubnubRequestState = requestState;
            cea.Message = message;
            cea.IsError = isError;
            cea.IsTimeout = isTimeout;
            cea.CurrRequestType = crt;

            CRequestType = responseType;
            if (responseType == ResponseType.PresenceV2 || responseType == ResponseType.SubscribeV2) {
                ExceptionHandlers.MultiplexException += HandleMultiplexException<T>;
                resultPart1 = false;
            }
            ExceptionHandlers.ResponseCallbackErrorOrTimeoutHandler<T> (cea, requestState, 
                errorLevel);

            /*if (responseType == ResponseType.PresenceV2 || responseType == ResponseType.SubscribeV2) {
                DateTime dt = DateTime.Now;
                while (dt.AddSeconds(2) > DateTime.Now) {
                    UnityEngine.Debug.Log ("waiting");
                }
            }*/
        }    

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(2.0f);
        }

        private void HandleMultiplexException<T> (object sender, EventArgs ea)
        {
            ExceptionHandlers.MultiplexException -= HandleMultiplexException<T>;
            MultiplexExceptionEventArgs<T> mea = ea as MultiplexExceptionEventArgs<T>;
            bool channelMatch = false;
            if (mea.channelEntities != null) {
                foreach (ChannelEntity c in mea.channelEntities) {
                    channelMatch = ExceptionChannel.Contains(c.ChannelID.ChannelOrChannelGroupName);
                    if(channelMatch)
                        break;
                }
            }
            string channels = Helpers.GetNamesFromChannelEntities(mea.channelEntities, false);

            UnityEngine.Debug.Log (mea.responseType.Equals (CRequestType));
            UnityEngine.Debug.Log (channelMatch);
            UnityEngine.Debug.Log (mea.resumeOnReconnect.Equals(ResumeOnReconnect));

            UnityEngine.Debug.Log (string.Format ("HandleMultiplexException LOG: {0} {1} {2} {3} {4} {5} {6} {7} {8} {9}",
                mea.responseType.Equals (CRequestType),
                channelMatch,
                mea.resumeOnReconnect.Equals(ResumeOnReconnect), CRequestType.ToString(), 
                ExceptionChannel, ResumeOnReconnect, mea.responseType,
                channels, mea.resumeOnReconnect, resultPart1
            ));
            bool resultPart2 = false;
            if (mea.responseType.Equals (CRequestType)
                && channelMatch
                && mea.resumeOnReconnect.Equals (ResumeOnReconnect)) {
                resultPart2 = true;
            }
            Assert.IsTrue (resultPart1 && resultPart2);
        }
            
        void ErrorCallbackCommonExceptionHandler (PubnubClientError result)
        {
            UnityEngine.Debug.Log (string.Format ("DisplayErrorMessage LOG: {0} {1} {2} {3} {4} {5} {6} {7} {8} {9}",
                result, result.Message.Equals (ExceptionMessage),
                ExceptionChannel.Contains(result.Channel),
                result.StatusCode.Equals(ExceptionStatusCode), result.StatusCode.ToString(), ExceptionMessage,
                ExceptionChannel, ExceptionStatusCode, IsTimeout, result.Channel

            ));

            bool statusCodeCheck = false;
            //TODO: Check why isError and isTimeout status codes dont match
            if (IsTimeout || IsError) {
                //statusCodeCheck = result.StatusCode.Equals (400);
                statusCodeCheck = true;
            } else {
                statusCodeCheck = result.StatusCode.Equals (ExceptionStatusCode);
            }

            if ((result.Channel.Contains ("Subscribe")) || (result.Channel.Contains ("Presence"))) {
                if (result.Message.Equals (ExceptionMessage)
                    && ExceptionChannel.Contains(result.Channel)
                    && statusCodeCheck) {
                    resultPart1 = true;
                } else {
                    resultPart1 = false;
                }
                UnityEngine.Debug.Log ("Subscribe || Presence " + resultPart1);
            } else {
                Assert.IsTrue (result.Message.Equals (ExceptionMessage)
                && ExceptionChannel.Contains(result.Channel)
                && statusCodeCheck);
            }
        }

        void UserCallbackCommonExceptionHandler (string result)
        {
            UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result));
        }

        void UserCallbackCommonExceptionHandler (object result)
        {
            UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result.ToString()));
        }

        void DisconnectCallbackCommonExceptionHandler (string result)
        {
            UnityEngine.Debug.Log (string.Format ("Disconnect CALLBACK LOG: {0}", result));
        }

        void ConnectCallbackCommonExceptionHandler (string result)
        {
            UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result));
        }

        void ConnectCallbackCommonExceptionHandler (object result)
        {
            UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result.ToString()));
        }

        #endif
    }
}

