using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class SignalBuilder
    {     
        private readonly SignalRequestBuilder sigBuilder;
        
        public SignalBuilder(PubNubUnity pn){
            sigBuilder = new SignalRequestBuilder(pn);
        }
        public SignalBuilder Message(object messageToPublish){
            sigBuilder.Message(messageToPublish);
            return this;
        }

        public SignalBuilder Channel(string channelName){
            sigBuilder.Channel(channelName);
            return this;
        }

        public SignalBuilder QueryParam(Dictionary<string, string> queryParam){
            sigBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNSignalResult, PNStatus> callback)
        {
            sigBuilder.Async(callback);
        }
    }
}