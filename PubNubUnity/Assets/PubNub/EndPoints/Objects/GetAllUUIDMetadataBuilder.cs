using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetAllUUIDMetadataBuilder
    {     
        private readonly GetAllUUIDMetadataRequestBuilder getAllUUIDMetadataRequestBuilder;
        
        public GetAllUUIDMetadataBuilder(PubNubUnity pn){
            getAllUUIDMetadataRequestBuilder = new GetAllUUIDMetadataRequestBuilder(pn);
        }
        public GetAllUUIDMetadataBuilder Include(PNUUIDMetadataInclude[] include){
            getAllUUIDMetadataRequestBuilder.Include(include);
            return this;
        }

        public GetAllUUIDMetadataBuilder Limit(int limit){
            getAllUUIDMetadataRequestBuilder.Limit(limit);
            return this;
        }

        public GetAllUUIDMetadataBuilder Start(string start){
            getAllUUIDMetadataRequestBuilder.Start(start);
            return this;
        }
        public GetAllUUIDMetadataBuilder End(string end){
            getAllUUIDMetadataRequestBuilder.End(end);
            return this;
        }
        public GetAllUUIDMetadataBuilder Filter(string filter)
        {
            getAllUUIDMetadataRequestBuilder.Filter(filter);
            return this;
        }
        public GetAllUUIDMetadataBuilder Sort(List<string> sortBy){
            getAllUUIDMetadataRequestBuilder.Sort(sortBy);
            return this;
        }
        public GetAllUUIDMetadataBuilder Count(bool count){
            getAllUUIDMetadataRequestBuilder.Count(count);
            return this;
        }
        public GetAllUUIDMetadataBuilder QueryParam(Dictionary<string, string> queryParam){
            getAllUUIDMetadataRequestBuilder.QueryParam(queryParam);
            return this;
        }

        public void Async(Action<PNGetAllUUIDMetadataResult, PNStatus> callback)
        {
            getAllUUIDMetadataRequestBuilder.Async(callback);
        }
    }
}