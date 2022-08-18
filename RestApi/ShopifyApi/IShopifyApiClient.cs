using PhaseOneRecords.RestApi.ShopifyApi.Requests.DTOs;
using PhaseOneRecords.RestApi.ShopifyApi.Requests;
using PhaseOneRecords.RestApi.ShopifyApi.Responses;
using ShopifySharp;
using ShopifySharp.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhaseOneRecords.RestApi.ShopifyApi
{
    public interface IShopifyApiClient
    {
        public Task<GetProductsResponse> GetProducts(GetProductsRequest request);
        Task<GetProductsResponse> GetProductTags(GetProductTagsRequest request);
    }
}
