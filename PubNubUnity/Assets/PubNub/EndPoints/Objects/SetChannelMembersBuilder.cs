using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class SetChannelMembersBuilder
    {
        private readonly ManageChannelMembersRequestBuilder manageChannelMembersRequestBuilder;

        public SetChannelMembersBuilder(PubNubUnity pn){
            manageChannelMembersRequestBuilder = new ManageChannelMembersRequestBuilder(pn);
        }
        public SetChannelMembersBuilder Include(PNChannelMembersInclude[] include){
            manageChannelMembersRequestBuilder.Include(include);
            return this;
        }

        public SetChannelMembersBuilder Channel(string id){
            manageChannelMembersRequestBuilder.Channel(id);
            return this;
        }

        public SetChannelMembersBuilder Limit(int limit){
            manageChannelMembersRequestBuilder.Limit(limit);
            return this;
        }

        public SetChannelMembersBuilder Start(string start){
            manageChannelMembersRequestBuilder.Start(start);
            return this;
        }
        public SetChannelMembersBuilder End(string end){
            manageChannelMembersRequestBuilder.End(end);
            return this;
        }
        public SetChannelMembersBuilder Count(bool count){
            manageChannelMembersRequestBuilder.Count(count);
            return this;
        }
        public SetChannelMembersBuilder Set(List<PNChannelMembersSet> add){
            manageChannelMembersRequestBuilder.Set(add);
            return this;
        }

        public SetChannelMembersBuilder Sort(List<string> sortBy){
            manageChannelMembersRequestBuilder.Sort(sortBy);
            return this;
        }
        public SetChannelMembersBuilder QueryParam(Dictionary<string, string> queryParam){
            manageChannelMembersRequestBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNManageChannelMembersResult, PNStatus> callback)
        {
            manageChannelMembersRequestBuilder.Async(callback);
        }
    }
}
