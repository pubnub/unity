using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class RemoveChannelMembersBuilder
    {
        private readonly ManageChannelMembersRequestBuilder manageChannelMembersRequestBuilder;

        public RemoveChannelMembersBuilder(PubNubUnity pn){
            manageChannelMembersRequestBuilder = new ManageChannelMembersRequestBuilder(pn);
        }
        public RemoveChannelMembersBuilder Include(PNChannelMembersInclude[] include){
            manageChannelMembersRequestBuilder.Include(include);
            return this;
        }

        public RemoveChannelMembersBuilder Channel(string id){
            manageChannelMembersRequestBuilder.Channel(id);
            return this;
        }

        public RemoveChannelMembersBuilder Limit(int limit){
            manageChannelMembersRequestBuilder.Limit(limit);
            return this;
        }

        public RemoveChannelMembersBuilder Start(string start){
            manageChannelMembersRequestBuilder.Start(start);
            return this;
        }
        public RemoveChannelMembersBuilder End(string end){
            manageChannelMembersRequestBuilder.End(end);
            return this;
        }
        public RemoveChannelMembersBuilder Count(bool count){
            manageChannelMembersRequestBuilder.Count(count);
            return this;
        }
        public RemoveChannelMembersBuilder Remove(List<PNChannelMembersRemove> remove){
            manageChannelMembersRequestBuilder.Remove(remove);
            return this;
        }
        public RemoveChannelMembersBuilder Sort(List<string> sortBy){
            manageChannelMembersRequestBuilder.Sort(sortBy);
            return this;
        }
        public RemoveChannelMembersBuilder QueryParam(Dictionary<string, string> queryParam){
            manageChannelMembersRequestBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNManageChannelMembersResult, PNStatus> callback)
        {
            manageChannelMembersRequestBuilder.Async(callback);
        }
    }
}
