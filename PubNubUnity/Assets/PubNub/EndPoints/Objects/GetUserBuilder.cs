using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetUserBuilder
    {     
        private readonly GetUserRequestBuilder getUserBuilder;
        
        public GetUserBuilder(PubNubUnity pn){
            getUserBuilder = new GetUserRequestBuilder(pn);
        }
        public GetUserBuilder Include(PNUserSpaceInclude[] include){
            getUserBuilder.Include(include);
            return this;
        }

        public GetUserBuilder ID(string id){
            getUserBuilder.ID(id);
            return this;
        }

        public GetUserBuilder QueryParam(Dictionary<string, string> queryParam){
            getUserBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNUserResult, PNStatus> callback)
        {
            getUserBuilder.Async(callback);
        }
    }
}