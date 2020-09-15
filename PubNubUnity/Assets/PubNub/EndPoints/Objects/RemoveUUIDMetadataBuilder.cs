using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class RemoveUUIDMetadataBuilder
    {     
        private readonly RemoveUUIDMetadataRequestBuilder removeUUIDMetadataRequestBuilder;
        
        public RemoveUUIDMetadataBuilder(PubNubUnity pn){
            removeUUIDMetadataRequestBuilder = new RemoveUUIDMetadataRequestBuilder(pn);
        }

        public RemoveUUIDMetadataBuilder UUID(string id){
            removeUUIDMetadataRequestBuilder.UUID(id);
            return this;
        }

        public RemoveUUIDMetadataBuilder QueryParam(Dictionary<string, string> queryParam){
            removeUUIDMetadataRequestBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNRemoveUUIDMetadataResult, PNStatus> callback)
        {
            removeUUIDMetadataRequestBuilder.Async(callback);
        }
    }
}