using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PhaseOneRecords.RestApi.ShopifyApi
{
    public class ShopifyStorefrontApiOptions
    {
        private readonly IConfiguration _configuration;

        public readonly string AccessToken;
        private readonly string _apiKey;
        private readonly string _storeId;
        private readonly string _authEndpoint;
        private readonly string _apiEndpoint;

        public ShopifyStorefrontApiOptions(IConfiguration configuration)
        {
            _configuration = configuration;

            AccessToken = _configuration["ShopifyApi:AccessToken"];
            _apiKey = _configuration["ShopifyApi:ApiKey"];
            _storeId = _configuration["ShopifyApi:StoreId"];
            _authEndpoint = _configuration["ShopifyApi:AuthEndpoint"];
            _apiEndpoint = _configuration["ShopifyApi:ApiEndpoint"];
        }

        public string BuildUrl()
        {
            return _authEndpoint.Replace("{shop_id}", _storeId).Replace("{client_Id}", _apiKey);
        }

        public string BuildGraphUrl()
        {
            return _apiEndpoint;
        }

    }
}
