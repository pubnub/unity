using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetMembersBuilder
    {     
        private readonly GetMembersRequestBuilder getMembersBuilder;
        
        public GetMembersBuilder(PubNubUnity pn){
            getMembersBuilder = new GetMembersRequestBuilder(pn);
        }
        public GetMembersBuilder Include(PNMembersInclude[] include){
            getMembersBuilder.Include(include);
            return this;
        }

        public GetMembersBuilder SpaceID(string id){
            getMembersBuilder.SpaceID(id);
            return this;
        }
        public GetMembersBuilder Limit(int limit){
            getMembersBuilder.Limit(limit);
            return this;
        }

        public GetMembersBuilder Start(string start){
            getMembersBuilder.Start(start);
            return this;
        }
        public GetMembersBuilder End(string end){
            getMembersBuilder.End(end);
            return this;
        }
        public GetMembersBuilder Filter(string filter)
        {
            getMembersBuilder.Filter(filter);
            return this;
        }
        public GetMembersBuilder Sort(List<string> sortBy){
            getMembersBuilder.Sort(sortBy);
            return this;
        }
        public GetMembersBuilder Count(bool count){
            getMembersBuilder.Count(count);
            return this;
        }
        public GetMembersBuilder QueryParam(Dictionary<string, string> queryParam){
            getMembersBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNMembersResult, PNStatus> callback)
        {
            getMembersBuilder.Async(callback);
        }
    }
}