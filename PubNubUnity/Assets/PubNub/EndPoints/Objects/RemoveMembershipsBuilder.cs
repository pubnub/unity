using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class RemoveMembershipsBuilder
    {
        private readonly ManageMembershipsRequestBuilder manageMembershipsBuilder;
        
        public RemoveMembershipsBuilder(PubNubUnity pn){
            manageMembershipsBuilder = new ManageMembershipsRequestBuilder(pn);
        }
        public RemoveMembershipsBuilder Include(PNMembershipsInclude[] include){
            manageMembershipsBuilder.Include(include);
            return this;
        }

        public RemoveMembershipsBuilder UUID(string id){
            manageMembershipsBuilder.UUID(id);
            return this;
        }

        public RemoveMembershipsBuilder Limit(int limit){
            manageMembershipsBuilder.Limit(limit);
            return this;
        }

        public RemoveMembershipsBuilder Start(string start){
            manageMembershipsBuilder.Start(start);
            return this;
        }
        public RemoveMembershipsBuilder End(string end){
            manageMembershipsBuilder.End(end);
            return this;
        }
        public RemoveMembershipsBuilder Count(bool count){
            manageMembershipsBuilder.Count(count);
            return this;
        }
        public RemoveMembershipsBuilder Remove(List<PNMembershipsRemove> remove){
            manageMembershipsBuilder.Remove(remove);
            return this;
        }
        public RemoveMembershipsBuilder QueryParam(Dictionary<string, string> queryParam){
            manageMembershipsBuilder.QueryParam(queryParam);
            return this;
        }
        public RemoveMembershipsBuilder Sort(List<string> sortBy){
            manageMembershipsBuilder.Sort(sortBy);
            return this;
        }
        public void Async(Action<PNManageMembershipsResult, PNStatus> callback)
        {
            manageMembershipsBuilder.Async(callback);
        }
    }
}