using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetUUIDMetadataBuilder
    {     
        private readonly GetUUIDMetadataRequestBuilder getUUIDMetadataResultBuilder;
        
        public GetUUIDMetadataBuilder(PubNubUnity pn){
            getUUIDMetadataResultBuilder = new GetUUIDMetadataRequestBuilder(pn);
        }
        public GetUUIDMetadataBuilder Include(PNUUIDMetadataInclude[] include){
            getUUIDMetadataResultBuilder.Include(include);
            return this;
        }

        public GetUUIDMetadataBuilder UUID(string id){
            getUUIDMetadataResultBuilder.UUID(id);
            return this;
        }

        public GetUUIDMetadataBuilder QueryParam(Dictionary<string, string> queryParam){
            getUUIDMetadataResultBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNUUIDMetadataResult, PNStatus> callback)
        {
            getUUIDMetadataResultBuilder.Async(callback);
        }
    }
}