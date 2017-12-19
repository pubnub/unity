using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNGetStateResult: PNResult
    {
        public Dictionary<string, object> StateByChannels  { get; set;}
        public PNGetStateResult ()
        {

        }
    }

    /*public class PNHereNowChannelData {
        public string ChannelName;
        public int Occupancy;
        public List<PNHereNowOccupantData> Occupants;
    }

    public class PNHereNowOccupantData {
        public string UUID;
        public object State;

    }*/

}