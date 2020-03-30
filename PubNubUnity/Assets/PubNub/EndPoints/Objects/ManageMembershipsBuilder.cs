using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class ManageMembershipsBuilder
    {
        private readonly ManageMembershipsRequestBuilder manageMembershipsBuilder;
        
        public ManageMembershipsBuilder(PubNubUnity pn){
            manageMembershipsBuilder = new ManageMembershipsRequestBuilder(pn);
        }
        public ManageMembershipsBuilder Include(PNMembershipsInclude[] include){
            manageMembershipsBuilder.Include(include);
            return this;
        }

        public ManageMembershipsBuilder UserID(string id){
            manageMembershipsBuilder.UserID(id);
            return this;
        }

        public ManageMembershipsBuilder Limit(int limit){
            manageMembershipsBuilder.Limit(limit);
            return this;
        }

        public ManageMembershipsBuilder Start(string start){
            manageMembershipsBuilder.Start(start);
            return this;
        }
        public ManageMembershipsBuilder End(string end){
            manageMembershipsBuilder.End(end);
            return this;
        }
        public ManageMembershipsBuilder Count(bool count){
            manageMembershipsBuilder.Count(count);
            return this;
        }
        public ManageMembershipsBuilder Add(List<PNMembersInput> add){
            manageMembershipsBuilder.Add(add);
            return this;
        }
        public ManageMembershipsBuilder Update(List<PNMembersInput> update){
            manageMembershipsBuilder.Update(update);
            return this;
        }
        public ManageMembershipsBuilder Remove(List<PNMembersRemove> remove){
            manageMembershipsBuilder.Remove(remove);
            return this;
        }
        public ManageMembershipsBuilder QueryParam(Dictionary<string, string> queryParam){
            manageMembershipsBuilder.QueryParam(queryParam);
            return this;
        }
        public ManageMembershipsBuilder Sort(List<string> sortBy){
            manageMembershipsBuilder.Sort(sortBy);
            return this;
        }
        public void Async(Action<PNMembershipsResult, PNStatus> callback)
        {
            manageMembershipsBuilder.Async(callback);
        }
    }
}