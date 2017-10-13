using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class FetchBuilder
    {     
        private FetchMessagesRequestBuilder pubBuilder;
        
        public FetchBuilder IncludeTimetoken(bool includeTimetoken){
            pubBuilder.IncludeTimetoken(includeTimetoken);
            return this;
        }

        public FetchBuilder Reverse(bool reverse){
            pubBuilder.Reverse(reverse);
            return this;
        }

        public FetchBuilder Start(long start){
            pubBuilder.Start(start);
            return this;
        }

        public FetchBuilder End(long end){
            pubBuilder.End(end);
            return this;
        }

        public FetchBuilder Channels(List<string> channel){
            pubBuilder.Channel(channel);
            return this;
        }

        public FetchBuilder Count(ushort count){
            pubBuilder.Count(count);
            return this;
        }
        public FetchBuilder(PubNubUnity pn){
            pubBuilder = new FetchMessagesRequestBuilder(pn);

            Debug.Log ("FetchMessagesRequestBuilder Construct");
        }
        public void Async(Action<PNFetchMessagesResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}
