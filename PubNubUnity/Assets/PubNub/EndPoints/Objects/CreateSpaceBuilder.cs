using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class CreateSpaceBuilder
    {     
        private readonly CreateSpaceRequestBuilder createSpaceBuilder;
        
        public CreateSpaceBuilder(PubNubUnity pn){
            createSpaceBuilder = new CreateSpaceRequestBuilder(pn);
        }
        public CreateSpaceBuilder Include(PNUserSpaceInclude[] include){
            createSpaceBuilder.Include(include);
            return this;
        }

        public CreateSpaceBuilder ID(string id){
            createSpaceBuilder.ID(id);
            return this;
        }

        public CreateSpaceBuilder Name(string name){
            createSpaceBuilder.Name(name);
            return this;
        }

        public CreateSpaceBuilder Description(string description){
            createSpaceBuilder.Description(description);
            return this;
        }

        public CreateSpaceBuilder Custom(Dictionary<string, object> custom){
            createSpaceBuilder.Custom(custom);
            return this;
        }

        public CreateSpaceBuilder QueryParam(Dictionary<string, string> queryParam){
            createSpaceBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNSpaceResult, PNStatus> callback)
        {
            createSpaceBuilder.Async(callback);
        }
    }
}