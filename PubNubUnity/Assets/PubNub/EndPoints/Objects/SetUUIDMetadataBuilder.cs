using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class SetUUIDMetadataBuilder
    {     
        private readonly SetUUIDMetadataRequestBuilder setUUIDMetadataRequestBuilder;
        
        public SetUUIDMetadataBuilder(PubNubUnity pn){
            setUUIDMetadataRequestBuilder = new SetUUIDMetadataRequestBuilder(pn);
        }
        public SetUUIDMetadataBuilder Include(PNUUIDMetadataInclude[] include){
            setUUIDMetadataRequestBuilder.Include(include);
            return this;
        }

        public SetUUIDMetadataBuilder UUID(string id){
            setUUIDMetadataRequestBuilder.UUID(id);
            return this;
        }

        public SetUUIDMetadataBuilder Name(string name){
            setUUIDMetadataRequestBuilder.Name(name);
            return this;
        }

        public SetUUIDMetadataBuilder ExternalID(string externalID){
            setUUIDMetadataRequestBuilder.ExternalID(externalID);
            return this;
        }

        public SetUUIDMetadataBuilder ProfileURL(string profileURL){
            setUUIDMetadataRequestBuilder.ProfileURL(profileURL);
            return this;
        }

        public SetUUIDMetadataBuilder Email(string email){
            setUUIDMetadataRequestBuilder.Email(email);
            return this;
        }

        public SetUUIDMetadataBuilder Custom(Dictionary<string, object> custom){
            setUUIDMetadataRequestBuilder.Custom(custom);
            return this;
        }

        public SetUUIDMetadataBuilder QueryParam(Dictionary<string, string> queryParam){
            setUUIDMetadataRequestBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNUUIDMetadataResult, PNStatus> callback)
        {
            setUUIDMetadataRequestBuilder.Async(callback);
        }
    }
}