using System;
using PubNubAPI;
using NUnit.Framework;
using System.Collections.Generic;

namespace PubNubAPI.Tests
{
    [TestFixture]
    public class PNConfigurationTests
    {
        #if DEBUG  
        [Test]
        public void TestPNinGenerateGuid ()
        {
            PNConfiguration pnConfig = new PNConfiguration();
            Assert.IsTrue(pnConfig.UserId.Contains("pn-"));
        }
       #endif
    }
}