using System;
using PubNubAPI;
using NUnit.Framework;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace PubNubAPI.Tests
{
    [TestFixture]
    public class FilesTest
    {
        PNLoggingMethod PNLog = new PNLoggingMethod(PNLogVerbosity.BODY);
        #if DEBUG   
        [Test]
        public void TestListFiles ()
        {
            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);            
            ListFilesRequestBuilder ls = new ListFilesRequestBuilder(pnUnity);
            // ls.CreatePubNubResponse();
            // TestObjectsUserCommon (true, false, true);
        }
        #endif
        [Test]
        public void TestFileEncryption ()
        {
            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null); 

            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            string filePath = "Assets/PubNub/PlayModeTests/file_upload_test.txt";
            string fileEncPath = "Assets/PubNub/PlayModeTests/file_upload_sample_encrypted.txt";
            string savePath = string.Format("{0}/test.up.enc.txt", Application.temporaryCachePath);
            byte[] iv = new byte[]{133, 126, 158, 123, 43, 95, 96, 90, 215, 178, 17, 73, 166, 130, 79, 156};
            pubnubCrypto.EncryptFile(iv, filePath, savePath);

            string read = System.IO.File.ReadAllText(fileEncPath);
			string save = System.IO.File.ReadAllText(savePath);
			Assert.True(read.Equals(save));
        }       

        [Test]
        public void TestFileDecryption ()
        {
            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null); 

            PubnubCrypto pubnubCrypto = new PubnubCrypto ("enigma", PNLog);
            string filePath = "Assets/PubNub/PlayModeTests/file_upload_test.txt";
            string fileEncPath = "Assets/PubNub/PlayModeTests/file_upload_sample_encrypted.txt";
            string savePath = string.Format("{0}/test.dl.enc.txt", Application.temporaryCachePath);
            pubnubCrypto.DecryptFile(fileEncPath, savePath);

            string read = System.IO.File.ReadAllText(filePath);
			string save = System.IO.File.ReadAllText(savePath);
			Assert.True(read.Equals(save));
        }        
       
    }
}