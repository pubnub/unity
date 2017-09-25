using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNSetStateResult: PNResult
    {
        public Dictionary<string, object> StateByChannels  { get; set;}
        //public Dictionary<string, PNHereNowChannelData> Channels { get; set;}
        public PNSetStateResult ()
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