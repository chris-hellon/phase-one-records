using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PhaseOneRecords.RestApi.ShopifyApi;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace PhaseOneRecords.Controllers
{
    public class ImportController : Controller
    {
        private readonly IShopifyApiClient _storeClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ImportController(IShopifyApiClient shopifyApiClient, IHttpContextAccessor httpContextAccessor)
        {
            _storeClient = shopifyApiClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> GetProductsRecursive(List<RestApi.ShopifyApi.Responses.DTOs.Node> nodes = null, string cursor = null)
        {
            var request = new RestApi.ShopifyApi.Requests.DTOs.GetProductsRequest("", 250, cursor);
            var response = await _storeClient.GetProducts(request);

            if (response.Success)
            {
                nodes.AddRange(response.Body.Products.Edges.Select(n => n.Node).ToList());

                if (response.Body.Products.Edges.Count == request.ItemsPerPage)
                    return await GetProductsRecursive(nodes, response.Body.Products.Edges.Last().Cursor);
            }

            nodes = nodes.Where(w => w.ArtistName != null && w.ArtistName.Value != null).Select(w => { w.ArtistName.Value = w.ArtistName.Value.TrimStart(); return w; }).ToList();

            if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Session != null)
                _httpContextAccessor.HttpContext.Session.SetString("AllProducts", JsonConvert.SerializeObject(nodes));

            return Ok(nodes);
        }
    }
}

