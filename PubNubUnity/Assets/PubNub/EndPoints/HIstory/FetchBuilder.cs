using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class FetchBuilder
    {     
        private readonly FetchMessagesRequestBuilder pubBuilder;
        
        public FetchBuilder IncludeTimetoken(bool includeTimetokenForFetch){
            pubBuilder.IncludeTimetoken(includeTimetokenForFetch);
            return this;
        }

        public FetchBuilder Reverse(bool reverseHistory){
            pubBuilder.Reverse(reverseHistory);
            return this;
        }

        public FetchBuilder Start(long startTime){
            pubBuilder.Start(startTime);
            return this;
        }

        public FetchBuilder End(long endTime){
            pubBuilder.End(endTime);
            return this;
        }

        public FetchBuilder Channels(List<string> channelNames){
            pubBuilder.Channel(channelNames);
            return this;
        }

        public FetchBuilder Count(ushort historyCount){
            pubBuilder.Count(historyCount);
            return this;
        }
        public FetchBuilder(PubNubUnity pn){
            pubBuilder = new FetchMessagesRequestBuilder(pn);

        }
        public void Async(Action<PNFetchMessagesResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}
