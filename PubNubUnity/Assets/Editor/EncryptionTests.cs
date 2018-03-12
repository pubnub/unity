//#define USE_MiniJSON
using System;
using NUnit.Framework;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using PubNubAPI;

namespace PubNubAPI.Tests
{
    //[TestFixture]
    public class EncryptionTests
    {
        PNLoggingMethod PNLog = new PNLoggingMethod(PNLogVerbosity.BODY);
        PNConfiguration pnConfig = new PNConfiguration();
        PubNub pn {get; set;}
        
        /// <summary>
        /// Tests the yay decryption.
        /// Assumes that the input message is deserialized  
        /// Decrypted string should match yay!
        /// </summary>
        [Test]
        public void TestYayDecryptionBasic ()
        {            
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            string message = "q/xJqqN6qbiZMXYmiQC1Fw==";
            //decrypt
            string decrypted = pubnubCrypto.Decrypt (message);

            Assert.True (("yay!").Equals (decrypted));
        }

        /// <summary>
        /// Tests the yay encryption.
        /// The output is not serialized
        /// Encrypted string should match q/xJqqN6qbiZMXYmiQC1Fw==
        /// </summary>
        [Test]
        public void TestYayEncryptionBasic ()
        {
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //deserialized string
            string message = "yay!";
            //Encrypt
            string encrypted = pubnubCrypto.Encrypt (message);
            Assert.True (("q/xJqqN6qbiZMXYmiQC1Fw==").Equals (encrypted));
        }

        /// <summary>
        /// Tests the yay decryption.
        /// Assumes that the input message is not deserialized  
        /// Decrypted and Deserialized string should match yay!
        /// </summary>
        [Test]
        public void TestYayDecryption ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //Non deserialized string
            string message = "\"Wi24KS4pcTzvyuGOHubiXg==\"";

            //Deserialize 
            message = EditorCommon.Deserialize<string> (message);

            //decrypt
            string decrypted = pubnubCrypto.Decrypt (message);
            //deserialize again
            message = EditorCommon.Deserialize<string> (decrypted);
            Assert.True (("yay!").Equals (message));
        }

        #if (USE_MiniJSON)
        [Test]
        public void TestYayDecryptionMiniJson ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //Non deserialized string
            string message = "\"Wi24KS4pcTzvyuGOHubiXg==\"";

            //Deserialize 
            message = Common.DeserializeMiniJson<string> (message);

            //decrypt
            string decrypted = pubnubCrypto.Decrypt (message);
            //deserialize again
            message = Common.DeserializeMiniJson<string> (decrypted);
            Assert.True (("yay!").Equals (message));
        }
        #endif

        /// <summary>
        /// Tests the yay encryption.
        /// The output is not serialized
        /// Encrypted string should match Wi24KS4pcTzvyuGOHubiXg==
        /// </summary>
        [Test]
        public void TestYayEncryption ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //deserialized string
            string message = "yay!";
            //serialize the string
            message = EditorCommon.Serialize (message);
            //Encrypt
            string encrypted = pubnubCrypto.Encrypt (message);
            Assert.True (("Wi24KS4pcTzvyuGOHubiXg==").Equals (encrypted));
        }

        #if (USE_MiniJSON)
        [Test]
        public void TestYayEncryptionMiniJson ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //deserialized string
            string message = "yay!";
            //serialize the string
            message = Common.SerializeMiniJson (message);
            Console.WriteLine (message);
            //Encrypt
            string encrypted = pubnubCrypto.Encrypt (message);
            Assert.True (("Wi24KS4pcTzvyuGOHubiXg==").Equals (encrypted));
        }
        #endif

        /// <summary>
        /// Tests the array encryption.
        /// The output is not serialized
        /// Encrypted string should match the serialized object
        /// </summary>
        [Test]
        public void TestArrayEncryption ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //create an empty array object
            object[] objArr = { };
            string strArr = EditorCommon.Serialize (objArr);
            //Encrypt
            string encrypted = pubnubCrypto.Encrypt (strArr);

            Assert.True (("Ns4TB41JjT2NCXaGLWSPAQ==").Equals (encrypted));
        }

        #if (USE_MiniJSON)
        [Test]
        public void TestArrayEncryptionMiniJson ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //create an empty array object
            object[] objArr = { };
            string strArr = Common.SerializeMiniJson (objArr);
            //Encrypt
            string encrypted = pubnubCrypto.Encrypt (strArr);

            Assert.True (("Ns4TB41JjT2NCXaGLWSPAQ==").Equals (encrypted));
        }
        #endif

        /// <summary>
        /// Tests the array decryption.
        /// Assumes that the input message is deserialized
        /// And the output message has to been deserialized.
        /// Decrypted string should match the serialized object
        /// </summary>
        [Test]
        public void TestArrayDecryption ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //Input the deserialized string
            string message = "Ns4TB41JjT2NCXaGLWSPAQ==";
            //decrypt
            string decrypted = pubnubCrypto.Decrypt (message);
            //create a serialized object
            object[] objArr = { };
            string result = EditorCommon.Serialize (objArr);
            //compare the serialized object and the return of the Decrypt method
            Assert.True ((result).Equals (decrypted));
        }

        #if (USE_MiniJSON)
        public void TestArrayDecryptionUsingMiniJson ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //Input the deserialized string
            string message = "Ns4TB41JjT2NCXaGLWSPAQ==";
            //decrypt
            string decrypted = pubnubCrypto.Decrypt (message);
            //create a serialized object
            object[] objArr = { };
            string result = Common.SerializeMiniJson (objArr);
            //compare the serialized object and the return of the Decrypt method
            Assert.True ((result).Equals (decrypted));
        }
        #endif

        /// <summary>
        /// Tests the object encryption.
        /// The output is not serialized
        /// Encrypted string should match the serialized object
        /// </summary>
        #if USE_MiniJSON
                [Ignore]
        #else
                [Test]
        #endif
                public void TestObjectEncryption ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //create an object
            Object obj = new Object ();
            //serialize
            string strObj = EditorCommon.Serialize (obj);
            //encrypt
            string encrypted = pubnubCrypto.Encrypt (strObj);

            Assert.True (("IDjZE9BHSjcX67RddfCYYg==").Equals (encrypted));
        }

        #if (USE_MiniJSON)
        //will fail with minijson
        //[Test]
        public void TestObjectEncryptionMiniJson ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //create an object
            Object obj = new Object ();
            //serialize
            string strObj = Common.SerializeMiniJson (obj);
            //encrypt
            string encrypted = pubnubCrypto.Encrypt (strObj);

            Assert.True (("IDjZE9BHSjcX67RddfCYYg==").Equals (encrypted));
        }
        #endif

        /// <summary>
        /// Tests the object decryption.
        /// Assumes that the input message is deserialized
        /// And the output message has to be deserialized.
        /// Decrypted string should match the serialized object
        /// </summary>
        #if USE_MiniJSON
                [Ignore]
        #else
                [Test]
        #endif
                public void TestObjectDecryption ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //Deserialized
            string message = "IDjZE9BHSjcX67RddfCYYg==";
            //Decrypt
            string decrypted = pubnubCrypto.Decrypt (message);
            //create an object
            Object obj = new Object ();
            //Serialize the object
            string result = EditorCommon.Serialize (obj);

            Assert.True ((decrypted).Equals (result));
        }

        #if (USE_MiniJSON)
        //will fail with minijson
        //[Test]
        public void TestObjectDecryptionMiniJson ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //Deserialized
            string message = "IDjZE9BHSjcX67RddfCYYg==";
            //Decrypt
            string decrypted = pubnubCrypto.Decrypt (message);
            //create an object
            Object obj = new Object ();
            //Serialize the object
            string result = Common.SerializeMiniJson (obj);

            Assert.True ((decrypted).Equals (result));
        }
        #endif

        /// <summary>
        /// Tests my object encryption.
        /// The output is not serialized 
        /// Encrypted string should match the serialized object
        /// </summary>
        #if USE_MiniJSON
                [Ignore]
        #else
                [Test]
        #endif
                public void TestMyObjectEncryption ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //create an object of the custom class
            CustomClass cc = new CustomClass ();
            //serialize it
            string result = EditorCommon.Serialize (cc);
            //encrypt it
            string encrypted = pubnubCrypto.Encrypt (result);

            UnityEngine.Debug.Log ("encrypted:" + encrypted);
            UnityEngine.Debug.Log ("result:" + result);
            Assert.True (("Zbr7pEF/GFGKj1rOstp0tWzA4nwJXEfj+ezLtAr8qqE=").Equals (encrypted));
        }

        #if (USE_MiniJSON)
        //will fail with minijson
        //[Test]
        public void TestMyObjectEncryptionMiniJson ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //create an object of the custom class
            CustomClass cc = new CustomClass ();
            //serialize it
            string result = Common.SerializeMiniJson (cc);
            //encrypt it
            string encrypted = pubnubCrypto.Encrypt (result);

            UnityEngine.Debug.Log ("encrypted:" + encrypted);
            UnityEngine.Debug.Log ("result:" + result);
            Assert.True (("Zbr7pEF/GFGKj1rOstp0tWzA4nwJXEfj+ezLtAr8qqE=").Equals (encrypted));
        }
        #endif


        /// <summary>
        /// Tests my object decryption.
        /// The output is not deserialized
        /// Decrypted string should match the serialized object
        /// </summary>
        #if USE_MiniJSON
                [Ignore]
        #else
                [Test]
        #endif
                public void TestMyObjectDecryption ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //Deserialized
            string message = "Zbr7pEF/GFGKj1rOstp0tWzA4nwJXEfj+ezLtAr8qqE=";
            //Decrypt
            string decrypted = pubnubCrypto.Decrypt (message);
            //create an object of the custom class
            CustomClass cc = new CustomClass ();
            //Serialize it
            string result = EditorCommon.Serialize (cc);

            UnityEngine.Debug.Log ("decrypted:" + decrypted);
            UnityEngine.Debug.Log ("result:" + result);
            Assert.True ((decrypted).Equals (result));
        }

        #if (USE_MiniJSON)
        //will fail with minijson
        //[Test]
        public void TestMyObjectDecryptionMiniJson ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //Deserialized
            string message = "Zbr7pEF/GFGKj1rOstp0tWzA4nwJXEfj+ezLtAr8qqE=";
            //Decrypt
            string decrypted = pubnubCrypto.Decrypt (message);
            //create an object of the custom class
            CustomClass cc = new CustomClass ();
            //Serialize it
            string result = Common.SerializeMiniJson (cc);

            UnityEngine.Debug.Log ("decrypted:" + decrypted);
            UnityEngine.Debug.Log ("result:" + result);
            Assert.True ((decrypted).Equals (result));
        }
        #endif

        /// <summary>
        /// Tests the pub nub encryption2.
        /// The output is not serialized
        /// Encrypted string should match f42pIQcWZ9zbTbH8cyLwB/tdvRxjFLOYcBNMVKeHS54=
        /// </summary>
        [Test]
        public void TestPubNubEncryption2 ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //Deserialized
            string message = "Pubnub Messaging API 2";
            //serialize the message
            message = EditorCommon.Serialize (message);
            //encrypt
            string encrypted = pubnubCrypto.Encrypt (message);

            Assert.True (("f42pIQcWZ9zbTbH8cyLwB/tdvRxjFLOYcBNMVKeHS54=").Equals (encrypted));
        }

        #if (USE_MiniJSON)
        [Test]
        public void TestPubNubEncryption2MiniJson ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //Deserialized
            string message = "Pubnub Messaging API 2";
            //serialize the message
            message = Common.SerializeMiniJson (message);
            //encrypt
            string encrypted = pubnubCrypto.Encrypt (message);

            Assert.True (("f42pIQcWZ9zbTbH8cyLwB/tdvRxjFLOYcBNMVKeHS54=").Equals (encrypted));
        }
        #endif

        /// <summary>
        /// Tests the pub nub decryption2.
        /// Assumes that the input message is deserialized  
        /// Decrypted and Deserialized string should match Pubnub Messaging API 2
        /// </summary>
        [Test]
        public void TestPubNubDecryption2 ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //Deserialized string    
            string message = "f42pIQcWZ9zbTbH8cyLwB/tdvRxjFLOYcBNMVKeHS54=";
            //Decrypt
            string decrypted = pubnubCrypto.Decrypt (message);
            //Deserialize
            message = EditorCommon.Deserialize<string> (decrypted);
            Assert.True (("Pubnub Messaging API 2").Equals (message));
        }

        #if (USE_MiniJSON)
        [Test]
        public void TestPubNubDecryption2MiniJson ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //Deserialized string    
            string message = "f42pIQcWZ9zbTbH8cyLwB/tdvRxjFLOYcBNMVKeHS54=";
            //Decrypt
            string decrypted = pubnubCrypto.Decrypt (message);
            //Deserialize
            message = Common.DeserializeMiniJson<string> (decrypted);
            Assert.True (("Pubnub Messaging API 2").Equals (message));
        }
        #endif

        /// <summary>
        /// Tests the pub nub encryption1.
        /// The input is not serialized
        /// Encrypted string should match f42pIQcWZ9zbTbH8cyLwByD/GsviOE0vcREIEVPARR0=
        /// </summary>
        [Test]
        public void TestPubNubEncryption1 ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //non serialized string
            string message = "Pubnub Messaging API 1";
            //serialize
            message = EditorCommon.Serialize (message);
            //encrypt
            string encrypted = pubnubCrypto.Encrypt (message);

            Assert.True (("f42pIQcWZ9zbTbH8cyLwByD/GsviOE0vcREIEVPARR0=").Equals (encrypted));
        }

        #if (USE_MiniJSON)
        [Test]
        public void TestPubNubEncryption1MiniJson ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //non serialized string
            string message = "Pubnub Messaging API 1";
            //serialize
            message = Common.SerializeMiniJson (message);
            //encrypt
            string encrypted = pubnubCrypto.Encrypt (message);

            Assert.True (("f42pIQcWZ9zbTbH8cyLwByD/GsviOE0vcREIEVPARR0=").Equals (encrypted));
        }
        #endif

        /// <summary>
        /// Tests the pub nub decryption1.
        /// Assumes that the input message is  deserialized  
        /// Decrypted and Deserialized string should match Pubnub Messaging API 1        
        /// </summary>
        [Test]
        public void TestPubNubDecryption1 ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //deserialized string
            string message = "f42pIQcWZ9zbTbH8cyLwByD/GsviOE0vcREIEVPARR0=";
            //decrypt
            string decrypted = pubnubCrypto.Decrypt (message);
            //deserialize
            message = EditorCommon.Deserialize<string> (decrypted);
            Assert.True (("Pubnub Messaging API 1").Equals (message));
        }

        #if (USE_MiniJSON)
        [Test]
        public void TestPubNubDecryption1MiniJson ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //deserialized string
            string message = "f42pIQcWZ9zbTbH8cyLwByD/GsviOE0vcREIEVPARR0=";
            //decrypt
            string decrypted = pubnubCrypto.Decrypt (message);
            //deserialize
            message = Common.DeserializeMiniJson<string> (decrypted);
            Assert.True (("Pubnub Messaging API 1").Equals (message));
        }
        #endif

        /// <summary>
        /// Tests the stuff can encryption.
        /// The input is serialized
        /// Encrypted string should match zMqH/RTPlC8yrAZ2UhpEgLKUVzkMI2cikiaVg30AyUu7B6J0FLqCazRzDOmrsFsF
        /// </summary>
        [Test]
        public void TestStuffCanEncryption ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //input serialized string
            string message = "{\"this stuff\":{\"can get\":\"complicated!\"}}";
            //encrypt
            string encrypted = pubnubCrypto.Encrypt (message);

            Assert.True (("zMqH/RTPlC8yrAZ2UhpEgLKUVzkMI2cikiaVg30AyUu7B6J0FLqCazRzDOmrsFsF").Equals (encrypted));
        }

        /// <summary>
        /// Tests the stuffcan decryption.
        /// Assumes that the input message is  deserialized  
        /// </summary>
        [Test]
        public void TestStuffcanDecryption ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //deserialized string
            string message = "zMqH/RTPlC8yrAZ2UhpEgLKUVzkMI2cikiaVg30AyUu7B6J0FLqCazRzDOmrsFsF";
            //decrypt
            string decrypted = pubnubCrypto.Decrypt (message);

            Assert.True (("{\"this stuff\":{\"can get\":\"complicated!\"}}").Equals (decrypted));
        }

        /// <summary>
        /// Tests the hash encryption.
        /// The input is serialized
        /// Encrypted string should match GsvkCYZoYylL5a7/DKhysDjNbwn+BtBtHj2CvzC4Y4g=
        /// </summary>
        [Test]
        public void TestHashEncryption ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //serialized string
            string message = "{\"foo\":{\"bar\":\"foobar\"}}";
            //encrypt
            string encrypted = pubnubCrypto.Encrypt (message);

            Assert.True (("GsvkCYZoYylL5a7/DKhysDjNbwn+BtBtHj2CvzC4Y4g=").Equals (encrypted));
        }

        /// <summary>
        /// Tests the hash decryption.
        /// Assumes that the input message is  deserialized  
        /// </summary>        
        [Test]
        public void TestHashDecryption ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //deserialized string
            string message = "GsvkCYZoYylL5a7/DKhysDjNbwn+BtBtHj2CvzC4Y4g=";
            //decrypt
            string decrypted = pubnubCrypto.Decrypt (message);

            Assert.True (("{\"foo\":{\"bar\":\"foobar\"}}").Equals (decrypted));
        }

        /// <summary>
        /// Tests the null encryption.
        /// The input is serialized
        /// </summary>
        [Test]
        //[ExpectedException (typeof(ArgumentNullException))]
        public void TestNullEncryption ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //serialized string
            string message = null;
            //encrypt

            var ex = Assert.Throws<ArgumentNullException>(() => pubnubCrypto.Encrypt (message)); // True (("").Equals (encrypted));
            Assert.That(ex.Message.Contains("Argument cannot be null."),ex.Message, null);
        }

        /// <summary>
        /// Tests the null decryption.
        /// Assumes that the input message is  deserialized  
        /// </summary>        
        [Test]
        //[ExpectedException (typeof(ArgumentNullException))]
        public void TestNullDecryption ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            //deserialized string
            string message = null;
            //decrypt
            var ex = Assert.Throws<ArgumentNullException>(() => pubnubCrypto.Decrypt (message)); 

            Assert.That(ex.Message.Contains("Argument cannot be null."), ex.Message, null);
        }

        /// <summary>
        /// Tests the unicode chars encryption.
        /// The input is not serialized
        /// </summary>
        [Test]
        public void TestUnicodeCharsEncryption ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            string message = "漢語";
            message = EditorCommon.Serialize (message);
            string encrypted = pubnubCrypto.Encrypt (message);
            Assert.That (("+BY5/miAA8aeuhVl4d13Kg==").Equals (encrypted));
        }

        #if (USE_MiniJSON)
        [Test]
        public void TestUnicodeCharsEncryptionMiniJson ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            string message = "漢語";
            message = Common.SerializeMiniJson (message);
            string encrypted = pubnubCrypto.Encrypt (message);
            Assert.That (("+BY5/miAA8aeuhVl4d13Kg==").Equals (encrypted));
        }
        #endif

        /// <summary>
        /// Tests the unicode decryption.
        /// Assumes that the input message is  deserialized  
        /// Decrypted and Deserialized string should match the unicode chars       
        /// </summary>
        [Test]
        public void TestUnicodeCharsDecryption ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            string message = "+BY5/miAA8aeuhVl4d13Kg==";
            //decrypt
            string decrypted = pubnubCrypto.Decrypt (message);
            //deserialize
            message = EditorCommon.Deserialize<string> (decrypted);

            Assert.True (("漢語").Equals (message));
        }

        #if (USE_MiniJSON)
        [Test]
        public void TestUnicodeCharsDecryptionMiniJson ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            string message = "+BY5/miAA8aeuhVl4d13Kg==";
            //decrypt
            string decrypted = pubnubCrypto.Decrypt (message);
            //deserialize
            message = Common.DeserializeMiniJson<string> (decrypted);

            Assert.True (("漢語").Equals (message));
        }
        #endif
        /// <summary>
        /// Tests the german chars decryption.
        /// Assumes that the input message is  deserialized  
        /// Decrypted and Deserialized string should match the unicode chars  
        /// </summary>
        [Test]
        public void TestGermanCharsDecryption ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            string message = "stpgsG1DZZxb44J7mFNSzg==";
            //decrypt
            string decrypted = pubnubCrypto.Decrypt (message);
            //deserialize
            message = EditorCommon.Deserialize<string> (decrypted);

            Assert.True (("ÜÖ").Equals (message));
        }

        #if (USE_MiniJSON)
        [Test]
        public void TestGermanCharsDecryptionMiniJson ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            string message = "stpgsG1DZZxb44J7mFNSzg==";
            //decrypt
            string decrypted = pubnubCrypto.Decrypt (message);
            //deserialize
            message = Common.DeserializeMiniJson<string> (decrypted);

            Assert.True (("ÜÖ").Equals (message));
        }
        #endif

        /// <summary>
        /// Tests the german encryption.
        /// The input is not serialized
        /// </summary>
        [Test]
        public void TestGermanCharsEncryption ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            string message = "ÜÖ";
            message = EditorCommon.Serialize (message);
            string encrypted = pubnubCrypto.Encrypt (message);

            Assert.True (("stpgsG1DZZxb44J7mFNSzg==").Equals (encrypted));
        }

        #if (USE_MiniJSON)
        [Test]
        public void TestGermanCharsEncryptionMiniJson ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            string message = "ÜÖ";
            message = Common.SerializeMiniJson (message);
            string encrypted = pubnubCrypto.Encrypt (message);

            Assert.True (("stpgsG1DZZxb44J7mFNSzg==").Equals (encrypted));
        }
        #endif

        [Test]
        public void TestPAMSignature ()
        {
            pn = EditorCommon.InitPN(pnConfig);
            PubnubCrypto crypto = new PubnubCrypto ("", PNLog);
            string secretKey = "secret";
            string message = "Pubnub Messaging 1";

            string signature = crypto.PubnubAccessManagerSign (secretKey, message);

            Assert.True (("mIoxTVM2WAM5j-M2vlp9bVblDLoZQI5XIoYyQ48U0as=").Equals (signature));
        }

        static string EncodeNonAsciiCharacters (string value)
        {
            StringBuilder encodedString = new StringBuilder ();
            foreach (char c in value) {
                if (c > 127) {
                    // This character is too big for ASCII
                    string encodedValue = "\\u" + ((int)c).ToString ("x4");
                    encodedString.Append (encodedValue);
                } else {
                    encodedString.Append (c);
                }
            }
            return encodedString.ToString ();
        }

        static string DecodeEncodedNonAsciiCharacters (string value)
        {
            return Regex.Replace (
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => {
                    return ((char)int.Parse (m.Groups ["Value"].Value, NumberStyles.HexNumber)).ToString ();
                });
        }
    }
}