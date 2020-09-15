using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class SetMembershipsBuilder
    {
        private readonly ManageMembershipsRequestBuilder manageMembershipsBuilder;
        
        public SetMembershipsBuilder(PubNubUnity pn){
            manageMembershipsBuilder = new ManageMembershipsRequestBuilder(pn);
        }
        public SetMembershipsBuilder Include(PNMembershipsInclude[] include){
            manageMembershipsBuilder.Include(include);
            return this;
        }

        public SetMembershipsBuilder UUID(string id){
            manageMembershipsBuilder.UUID(id);
            return this;
        }

        public SetMembershipsBuilder Limit(int limit){
            manageMembershipsBuilder.Limit(limit);
            return this;
        }

        public SetMembershipsBuilder Start(string start){
            manageMembershipsBuilder.Start(start);
            return this;
        }
        public SetMembershipsBuilder End(string end){
            manageMembershipsBuilder.End(end);
            return this;
        }
        public SetMembershipsBuilder Count(bool count){
            manageMembershipsBuilder.Count(count);
            return this;
        }
        public SetMembershipsBuilder Set(List<PNMembershipsSet> add){
            manageMembershipsBuilder.Set(add);
            return this;
        }

        public SetMembershipsBuilder QueryParam(Dictionary<string, string> queryParam){
            manageMembershipsBuilder.QueryParam(queryParam);
            return this;
        }
        public SetMembershipsBuilder Sort(List<string> sortBy){
            manageMembershipsBuilder.Sort(sortBy);
            return this;
        }
        public void Async(Action<PNManageMembershipsResult, PNStatus> callback)
        {
            manageMembershipsBuilder.Async(callback);
        }
    }
}