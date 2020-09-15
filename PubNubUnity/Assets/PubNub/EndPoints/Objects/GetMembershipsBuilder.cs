using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetMembershipsBuilder
    {     
        private readonly GetMembershipsRequestBuilder getMembershipsBuilder;
        
        public GetMembershipsBuilder(PubNubUnity pn){
            getMembershipsBuilder = new GetMembershipsRequestBuilder(pn);
        }
        public GetMembershipsBuilder Include(PNMembershipsInclude[] include){
            getMembershipsBuilder.Include(include);
            return this;
        }

        public GetMembershipsBuilder UUID(string id){
            getMembershipsBuilder.UUID(id);
            return this;
        }

        public GetMembershipsBuilder Limit(int limit){
            getMembershipsBuilder.Limit(limit);
            return this;
        }

        public GetMembershipsBuilder Start(string start){
            getMembershipsBuilder.Start(start);
            return this;
        }
        public GetMembershipsBuilder End(string end){
            getMembershipsBuilder.End(end);
            return this;
        }
        public GetMembershipsBuilder Filter(string filter)
        {
            getMembershipsBuilder.Filter(filter);
            return this;
        }
        public GetMembershipsBuilder Sort(List<string> sortBy){
            getMembershipsBuilder.Sort(sortBy);
            return this;
        }
        public GetMembershipsBuilder Count(bool count){
            getMembershipsBuilder.Count(count);
            return this;
        }
        public GetMembershipsBuilder QueryParam(Dictionary<string, string> queryParam){
            getMembershipsBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNGetMembershipsResult, PNStatus> callback)
        {
            getMembershipsBuilder.Async(callback);
        }
    }
}