using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNHereNowResult: PNResult
    {
        public int TotalChannels  { get; set;}
        public int TotalOccupancy  { get; set;}
        public Dictionary<string, PNHereNowChannelData> Channels { get; set;}
    }

    public class PNHereNowChannelData {
        public string ChannelName {get; set;}
        public int Occupancy {get; set;}
        public List<PNHereNowOccupantData> Occupants {get; set;}
    }

    public class PNHereNowOccupantData {
        public string UUID {get; set;}
        public object State {get; set;}

    }

}