using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class CreateUserBuilder
    {     
        private readonly CreateUserRequestBuilder createUserBuilder;
        
        public CreateUserBuilder(PubNubUnity pn){
            createUserBuilder = new CreateUserRequestBuilder(pn);
        }
        public CreateUserBuilder Include(PNUserSpaceInclude[] include){
            createUserBuilder.Include(include);
            return this;
        }

        public CreateUserBuilder ID(string id){
            createUserBuilder.ID(id);
            return this;
        }

        public CreateUserBuilder Name(string name){
            createUserBuilder.Name(name);
            return this;
        }

        public CreateUserBuilder ExternalID(string externalID){
            createUserBuilder.ExternalID(externalID);
            return this;
        }

        public CreateUserBuilder ProfileURL(string profileURL){
            createUserBuilder.ProfileURL(profileURL);
            return this;
        }

        public CreateUserBuilder Email(string email){
            createUserBuilder.Email(email);
            return this;
        }

        public CreateUserBuilder Custom(Dictionary<string, object> custom){
            createUserBuilder.Custom(custom);
            return this;
        }

        public CreateUserBuilder QueryParam(Dictionary<string, string> queryParam){
            createUserBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNUserResult, PNStatus> callback)
        {
            createUserBuilder.Async(callback);
        }
    }
}