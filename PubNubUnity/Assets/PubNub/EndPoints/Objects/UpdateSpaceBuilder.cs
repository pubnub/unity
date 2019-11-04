using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class UpdateSpaceBuilder
    {
        private readonly UpdateSpaceRequestBuilder updateSpaceBuilder;

        public UpdateSpaceBuilder(PubNubUnity pn)
        {
            updateSpaceBuilder = new UpdateSpaceRequestBuilder(pn);
        }
        public UpdateSpaceBuilder Include(PNUserSpaceInclude[] include)
        {
            updateSpaceBuilder.Include(include);
            return this;
        }

        public UpdateSpaceBuilder ID(string id)
        {
            updateSpaceBuilder.ID(id);
            return this;
        }

        public UpdateSpaceBuilder Name(string name)
        {
            updateSpaceBuilder.Name(name);
            return this;
        }

        public UpdateSpaceBuilder Description(string description)
        {
            updateSpaceBuilder.Description(description);
            return this;
        }

        public UpdateSpaceBuilder Custom(Dictionary<string, object> custom)
        {
            updateSpaceBuilder.Custom(custom);
            return this;
        }

        public UpdateSpaceBuilder QueryParam(Dictionary<string, string> queryParam)
        {
            updateSpaceBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNSpaceResult, PNStatus> callback)
        {
            updateSpaceBuilder.Async(callback);
        }
    }
}