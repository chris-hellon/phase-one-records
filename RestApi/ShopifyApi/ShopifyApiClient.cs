using PhaseOneRecords.RestApi.ShopifyApi.Requests.DTOs;
using PhaseOneRecords.RestApi.ShopifyApi.Responses;
using ShopifySharp;
using ShopifySharp.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhaseOneRecords.RestApi.ShopifyApi
{
    //[Obsolete("Not required for GraphQL Storefront Api")]
    //public class ShopifyApiClient : IShopifyApiClient
    //{
    //    private readonly string _apiKey;

    //    private readonly string _storeUrl;

    //    private ProductService _productService;

    //    private string _shopAccessToken { get; set;  }

    //    public ShopifyApiClient(ProductService productService)
    //    {
    //        _productService = productService;
    //    }

    //    //public ShopifyApiClient(string apiKey, string storeUrl, string shopAccessToken)
    //    //{
    //    //    _apiKey = apiKey;
    //    //    _storeUrl = storeUrl;
    //    //    _storeUrl = shopAccessToken;
    //    //    //_productService = 
    //    //}

    //    public async Task<ListResult<Product>> GetAllProducts()
    //    {
    //        var products = await _productService.ListAsync();

    //        return products;
    //    }

    //    public void Authenticate()
    //    {

    //    }

    //    public Task<GetProductsResponse> GetProducts(GetProductsRequest request)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
