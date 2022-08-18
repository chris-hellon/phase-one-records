using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PhaseOneRecords.Models;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PhaseOneRecords.RestApi.SpotifyAPI;
using PhaseOneRecords.RestApi.SpotifyAPI.Request;
using PhaseOneRecords.RestApi.ShopifyApi;

namespace PhaseOneRecords.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISpotifyApiClient _spotifyClient;
        private readonly IShopifyApiClient _storeClient;

        public HomeController(ILogger<HomeController> logger, ISpotifyApiClient spotifyClient, IShopifyApiClient shopifyApiClient)
        {
            _logger = logger;
            _spotifyClient = spotifyClient;
            _storeClient = shopifyApiClient;
        }

        public IActionResult Index()
        {
            var uri = Request.Scheme + "://" +  Request.Host;
            var loginUri = _spotifyClient.Login(new SpotifyLoginRequest() { 
                CallbackUrl =  new Uri(uri + "/home/callback/")
            });
            
            return Redirect(loginUri.ToString());
        }

        public async Task<IActionResult> Callback(string code)
        {
            var response = await _spotifyClient.ProcessLogin(new SpotifyProcessLoginRequest() { Code = code });

            return RedirectToAction("Landing", "Browse");
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            var response = await _spotifyClient.TextSearch(new SpotifySearchRequest(searchTerm));

            return View("Response", JsonConvert.SerializeObject(response));
        }

        public async Task<IActionResult> ShopifySearch(string searchTerm, int itemCount, string cursor)
        {
            var response = await _storeClient.GetProducts(new RestApi.ShopifyApi.Requests.DTOs.GetProductsRequest() { 
                SearchTerm = searchTerm,
                ItemsPerPage = itemCount,
                Cursor = cursor
            });

            if (response.Success)
            {
                return View("Response", JsonConvert.SerializeObject(response.Body));
            }
            else
            {
                return View("Response", response.Message);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
