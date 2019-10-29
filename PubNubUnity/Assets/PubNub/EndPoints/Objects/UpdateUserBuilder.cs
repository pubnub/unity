using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class UpdateUserBuilder
    {     
        private readonly UpdateUserRequestBuilder updateUserBuilder;
        
        public UpdateUserBuilder(PubNubUnity pn){
            updateUserBuilder = new UpdateUserRequestBuilder(pn);
        }
        public UpdateUserBuilder Include(PNUserSpaceInclude[] include){
            updateUserBuilder.Include(include);
            return this;
        }

        public UpdateUserBuilder ID(string id){
            updateUserBuilder.ID(id);
            return this;
        }

        public UpdateUserBuilder Name(string name){
            updateUserBuilder.Name(name);
            return this;
        }

        public UpdateUserBuilder ExternalID(string externalID){
            updateUserBuilder.ExternalID(externalID);
            return this;
        }

        public UpdateUserBuilder ProfileURL(string profileURL){
            updateUserBuilder.ProfileURL(profileURL);
            return this;
        }

        public UpdateUserBuilder Email(string email){
            updateUserBuilder.Email(email);
            return this;
        }

        public UpdateUserBuilder Custom(Dictionary<string, object> custom){
            updateUserBuilder.Custom(custom);
            return this;
        }

        public UpdateUserBuilder QueryParam(Dictionary<string, string> queryParam){
            updateUserBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNUserResult, PNStatus> callback)
        {
            updateUserBuilder.Async(callback);
        }
    }
}