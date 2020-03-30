using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class ManageMembersBuilder
    {
        private readonly ManageMembersRequestBuilder manageMembersBuilder;
        
        public ManageMembersBuilder(PubNubUnity pn){
            manageMembersBuilder = new ManageMembersRequestBuilder(pn);
        }
        public ManageMembersBuilder Include(PNMembersInclude[] include){
            manageMembersBuilder.Include(include);
            return this;
        }

        public ManageMembersBuilder SpaceID(string id){
            manageMembersBuilder.SpaceID(id);
            return this;
        }
        public ManageMembersBuilder Limit(int limit){
            manageMembersBuilder.Limit(limit);
            return this;
        }

        public ManageMembersBuilder Start(string start){
            manageMembersBuilder.Start(start);
            return this;
        }
        public ManageMembersBuilder End(string end){
            manageMembersBuilder.End(end);
            return this;
        }
        public ManageMembersBuilder Count(bool count){
            manageMembersBuilder.Count(count);
            return this;
        }
        public ManageMembersBuilder Add(List<PNMembersInput> add){
            manageMembersBuilder.Add(add);
            return this;
        }
        public ManageMembersBuilder Update(List<PNMembersInput> update){
            manageMembersBuilder.Update(update);
            return this;
        }
        public ManageMembersBuilder Remove(List<PNMembersRemove> remove){
            manageMembersBuilder.Remove(remove);
            return this;
        }
        public ManageMembersBuilder Sort(List<string> sortBy){
            manageMembersBuilder.Sort(sortBy);
            return this;
        }
        public ManageMembersBuilder QueryParam(Dictionary<string, string> queryParam){
            manageMembersBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNMembersResult, PNStatus> callback)
        {
            manageMembersBuilder.Async(callback);
        }
    }
}

