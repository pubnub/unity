using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetChannelMembersBuilder
    {     
        private readonly GetChannelMembersRequestBuilder getChannelMembersRequestBuilder;
        
        public GetChannelMembersBuilder(PubNubUnity pn){
            getChannelMembersRequestBuilder = new GetChannelMembersRequestBuilder(pn);
        }
        public GetChannelMembersBuilder Include(PNChannelMembersInclude[] include){
            getChannelMembersRequestBuilder.Include(include);
            return this;
        }

        public GetChannelMembersBuilder Channel(string id){
            getChannelMembersRequestBuilder.Channel(id);
            return this;
        }
        public GetChannelMembersBuilder Limit(int limit){
            getChannelMembersRequestBuilder.Limit(limit);
            return this;
        }

        public GetChannelMembersBuilder Start(string start){
            getChannelMembersRequestBuilder.Start(start);
            return this;
        }
        public GetChannelMembersBuilder End(string end){
            getChannelMembersRequestBuilder.End(end);
            return this;
        }
        public GetChannelMembersBuilder Filter(string filter)
        {
            getChannelMembersRequestBuilder.Filter(filter);
            return this;
        }
        public GetChannelMembersBuilder Sort(List<string> sortBy){
            getChannelMembersRequestBuilder.Sort(sortBy);
            return this;
        }
        public GetChannelMembersBuilder Count(bool count){
            getChannelMembersRequestBuilder.Count(count);
            return this;
        }
        public GetChannelMembersBuilder QueryParam(Dictionary<string, string> queryParam){
            getChannelMembersRequestBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNGetChannelMembersResult, PNStatus> callback)
        {
            getChannelMembersRequestBuilder.Async(callback);
        }
    }
}