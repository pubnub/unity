using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class HistoryBuilder
    {     
        private readonly HistoryRequestBuilder pubBuilder;
        
        public HistoryBuilder(PubNubUnity pn){
            pubBuilder = new HistoryRequestBuilder(pn);

        }
        public HistoryBuilder IncludeTimetoken(bool includeTimetokenForHistory){
            pubBuilder.IncludeTimetoken(includeTimetokenForHistory);
            return this;
        }

        public HistoryBuilder Reverse(bool reverseHistory){
            pubBuilder.Reverse(reverseHistory);
            return this;
        }

        public HistoryBuilder Start(long startTime){
            pubBuilder.Start(startTime);
            return this;
        }

        public HistoryBuilder End(long endTime){
            pubBuilder.End(endTime);
            return this;
        }

        public HistoryBuilder Channel(string channelName){
            pubBuilder.Channel(channelName);
            return this;
        }

        public HistoryBuilder Count(ushort historyCount){
            pubBuilder.Count(historyCount);
            return this;
        }

        public void Async(Action<PNHistoryResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}