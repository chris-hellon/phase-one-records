using GetProductsDtos;
using GraphQL.Client.Http;
using Newtonsoft.Json;
using PhaseOneRecords.RestApi.ShopifyApi.Requests.DTOs;
using PhaseOneRecords.RestApi.ShopifyApi.Responses;
using PhaseOneRecords.RestApi.ShopifyApi.Responses.DTOs;
using PhaseOneRecords.RestApi.ShopifyApi.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GetProductsDtos = PhaseOneRecords.RestApi.ShopifyApi.Responses.DTOs;
using Rollbar.DTOs;

namespace PhaseOneRecords.RestApi.ShopifyApi
{
    public class ShopifyStorefrontApiClient : IShopifyApiClient
    {
        private readonly GraphQLHttpClient _client;

        private readonly ShopifyStorefrontApiOptions _options;

        public ShopifyStorefrontApiClient(GraphQLHttpClient client, ShopifyStorefrontApiOptions options)
        {
            client.HttpClient.DefaultRequestHeaders.Add("X-Shopify-Storefront-Access-Token", options.AccessToken);
            _client = client;
            _options = options;
        }

        public async Task<GetProductsResponse> GetProducts(GetProductsRequest request)
        {
            var response = new GetProductsResponse();

            request.ItemsPerPage = request.ItemsPerPage == 0 ? 10 : request.ItemsPerPage;

            try
            {
                var queryParams = "first:" + request.ItemsPerPage + (String.IsNullOrWhiteSpace(request.Cursor) ? "" : ", after:" + "\"" + request.Cursor + "\"");

                bool filterExists = request.Filter != null;
                
                if (filterExists)
                    queryParams = $"{queryParams},{request.Filter.SortValue}";
          
                bool searchExists = !string.IsNullOrWhiteSpace(request.SearchTerm) || !string.IsNullOrWhiteSpace(request.SearchArtist) || !string.IsNullOrWhiteSpace(request.SearchAlbum);
                if (searchExists)
                {
                    if (request.SearchArtist != null && request.SearchAlbum != null)
                        queryParams = queryParams + ",query:" + "\"(artistName.value:" + request.SearchArtist + ") AND (title:" + request.SearchAlbum + ")\"";
                    else if (request.SearchArtist != null)
                        queryParams = queryParams + ",query:" + "\"artistName.value:" + request.SearchArtist + "\"";
                    else if (request.SearchAlbum != null)
                        queryParams = queryParams + ",query:" + "\"title:" + request.SearchAlbum + "\"";
                    else
                        queryParams = queryParams + ",query:" + "\"" + request.SearchTerm + "\"";
                }

                if (filterExists && request.Filter.GenreValues != null)
                {
                    string genresQuery = !searchExists ? ",query:" : " AND ";

                    if (request.Filter.GenreValues.Count == 1) 
                        genresQuery = genresQuery + "\"tag:" + request.Filter.GenreValues[0] + "" + "\"";
                    else
                    {
                        int genresCount = request.Filter.GenreValues.Count;
                        for (int i = 0; i < request.Filter.GenreValues.Count; i++)
                        {
                            string genreValue = request.Filter.GenreValues[i];

                            if (i == 0)
                                genresQuery = genresQuery + "\"tag:" + genreValue;
                            else
                                genresQuery = genresQuery + " OR tag:" + genreValue;

                            if (i + 1 == genresCount)
                                genresQuery += "\"";
                        }
                    }

                    queryParams = $"{queryParams}{genresQuery}";
                }

              

                var graphRequest = new GraphQLHttpRequest
                {
                    Query = @"{
                                products({query}) {
                                    pageInfo{
                                        hasNextPage
                                        hasPreviousPage
                                    }
                                    edges {
                                        cursor
                                        node {
                                            id,
                                            description,
                                            handle,
                                            onlineStoreUrl,
                                            productType,
                                            tags,
                                            title,
                                            vendor,
                                            totalInventory,
	                                        artistName: metafield(namespace: ""oscar"", key: ""artist-name"") {
                                              value
                                            },
                                            createDate: metafield(namespace: ""oscar"", key: ""release-date"") {
                                              value
                                            },
                                            variants(first: 250) {
                                                  edges {
                                                    node {
                                                      price,
                                                      id
                                                    }
                                                  }
                                            },
                                            images(first: 250) {
                                                edges{
                                                    node{
                                                        altText,
                                                        id,
                                                        originalSrc
                                                        }
                                                    }
                                            }
                                        }
                                    }
                                }
                        }".Replace("{query}", queryParams)
                };


                var data = await _client.SendQueryAsync<Responses.DTOs.Data>(graphRequest);

                response.Body = data.Data;
                response.Success = true;
            }
            catch(System.Exception ex)
            {
                Rollbar.RollbarLocator.RollbarInstance.Error(ex);
                response.Message = ex.Message;
                response.Success = false;
            }

            return response;
        }

        public async Task<GetProductsResponse> GetProductTags(GetProductTagsRequest request)
        {
            var response = new GetProductsResponse();

            request.ItemsPerPage = request.ItemsPerPage == 0 ? 250 : request.ItemsPerPage;

            try
            {
                var products = await GetProducts(new GetProductsRequest("", request.ItemsPerPage, request.Cursor, null, null, null, request.ExistingTags));

                response.Body = products.Body;
                response.Success = true;
            }
            catch (System.Exception ex)
            {
                Rollbar.RollbarLocator.RollbarInstance.Error(ex);
                response.Message = ex.Message;
                response.Success = false;
            }

            return response;
        }
    }
}
