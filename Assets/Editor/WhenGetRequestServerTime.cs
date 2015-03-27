using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.ComponentModel;
using System.Threading;
using System.Collections;
using PubNubMessaging.Core;
using NUnit.Framework;

namespace PubNubMessaging.Tests
{
    public class WhenGetRequestServerTime
    {
        [Test]
        public void TranslateDateTimeToUnixTime ()
        {
            Debug.Log ("Running TranslateDateTimeToUnixTime()");
            //Test for 26th June 2012 GMT
            DateTime dt = new DateTime (2012, 6, 26, 0, 0, 0, DateTimeKind.Utc);
            long nanoSecondTime = Pubnub.TranslateDateTimeToPubnubUnixNanoSeconds (dt);
            Assert.True ((13406688000000000).Equals (nanoSecondTime));
        }

        [Test]
        public void TranslateUnixTimeToDateTime ()
        {
            Debug.Log ("Running TranslateUnixTimeToDateTime()");
            //Test for 26th June 2012 GMT
            DateTime expectedDate = new DateTime (2012, 6, 26, 0, 0, 0, DateTimeKind.Utc);
            DateTime actualDate = Pubnub.TranslatePubnubUnixNanoSecondsToDateTime (13406688000000000);
            Assert.True (expectedDate.ToString ().Equals (actualDate.ToString ()));
        }
    }
}
