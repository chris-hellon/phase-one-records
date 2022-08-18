using ShopifySharp;
using ShopifySharp.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PhaseOneRecords.RestApi.ShopifyApi
{
    public class ShopifyOptions
    {
        private readonly IConfiguration _configuration;

        private readonly string _apiKey;
        private readonly string _appPassword;
        private readonly string _clientSecret;
        private readonly string _storeUrl;
        private readonly string _graphQLEndpoint;

        public ShopifyOptions(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiKey = _configuration["ShopifyApi:ApiKey"];
            _appPassword = _configuration["ShopifyApi:AppPassword"];
            _clientSecret = _configuration["ShopifyApi:ClientSecret"];
            _storeUrl = _configuration["ShopifyApi:StoreUrl"];
            _graphQLEndpoint = _configuration["ShopifyApi:GraphQLEndpoint"];
        }

        public async Task<string> GetAuthUrl(string callbackUrl)
        {
            bool isValidDomain = await AuthorizationService.IsValidShopDomainAsync(_storeUrl);

            if (isValidDomain)
            {
                var scopes = new List<AuthorizationScope>() {
                    AuthorizationScope.ReadProducts,
                    AuthorizationScope.UnauthenticatedReadProductListings,
                    AuthorizationScope.UnauthenticatedReadCollectionListings,
                    AuthorizationScope.ReadInventory
                };

                Uri authUri = AuthorizationService.BuildAuthorizationUrl(scopes, _storeUrl, _apiKey, callbackUrl);

                return authUri.ToString();

            }
            else return "";
        }

        public async Task<string> GetAccessToken(string code, string myShopifyUrl)
        {
            string accessToken = await AuthorizationService.Authorize(code, myShopifyUrl, _apiKey, _clientSecret);

            return accessToken;
        }
    }

}
