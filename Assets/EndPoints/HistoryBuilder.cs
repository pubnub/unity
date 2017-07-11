using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class HistoryBuilder
    {     
        private HistoryRequestBuilder pubBuilder;
        
        public HistoryBuilder(PubNubUnity pn){
            pubBuilder = new HistoryRequestBuilder(pn);

            Debug.Log ("HistoryBuilder Construct");
        }
        public HistoryBuilder IncludeTimetoken(bool includeTimetoken){
            pubBuilder.IncludeTimetoken(includeTimetoken);
            return this;
        }

        public HistoryBuilder Reverse(bool reverse){
            pubBuilder.Reverse(reverse);
            return this;
        }

        public HistoryBuilder Start(long start){
            pubBuilder.Start(start);
            return this;
        }

        public HistoryBuilder End(long end){
            pubBuilder.End(end);
            return this;
        }

        public HistoryBuilder Channel(string channel){
            pubBuilder.Channel(channel);
            return this;
        }

        public HistoryBuilder Count(ushort count){
            pubBuilder.Count(count);
            return this;
        }

        public void Async(Action<PNHistoryResult, PNStatus> callback)
        {
            pubBuilder.Async(callback);
        }
    }
}