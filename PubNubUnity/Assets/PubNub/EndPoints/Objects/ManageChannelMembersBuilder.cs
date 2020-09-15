using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class ManageChannelMembersBuilder
    {
        private readonly ManageChannelMembersRequestBuilder manageChannelMembersRequestBuilder;

        public ManageChannelMembersBuilder(PubNubUnity pn){
            manageChannelMembersRequestBuilder = new ManageChannelMembersRequestBuilder(pn);
        }
        public ManageChannelMembersBuilder Include(PNChannelMembersInclude[] include){
            manageChannelMembersRequestBuilder.Include(include);
            return this;
        }

        public ManageChannelMembersBuilder Channel(string id){
            manageChannelMembersRequestBuilder.Channel(id);
            return this;
        }

        public ManageChannelMembersBuilder Limit(int limit){
            manageChannelMembersRequestBuilder.Limit(limit);
            return this;
        }

        public ManageChannelMembersBuilder Start(string start){
            manageChannelMembersRequestBuilder.Start(start);
            return this;
        }
        public ManageChannelMembersBuilder End(string end){
            manageChannelMembersRequestBuilder.End(end);
            return this;
        }
        public ManageChannelMembersBuilder Count(bool count){
            manageChannelMembersRequestBuilder.Count(count);
            return this;
        }
        public ManageChannelMembersBuilder Set(List<PNChannelMembersSet> set){
            manageChannelMembersRequestBuilder.Set(set);
            return this;
        }
        public ManageChannelMembersBuilder Remove(List<PNChannelMembersRemove> remove){
            manageChannelMembersRequestBuilder.Remove(remove);
            return this;
        }
        public ManageChannelMembersBuilder Sort(List<string> sortBy){
            manageChannelMembersRequestBuilder.Sort(sortBy);
            return this;
        }
        public ManageChannelMembersBuilder QueryParam(Dictionary<string, string> queryParam){
            manageChannelMembersRequestBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNManageChannelMembersResult, PNStatus> callback)
        {
            manageChannelMembersRequestBuilder.Async(callback);
        }
    }
}
