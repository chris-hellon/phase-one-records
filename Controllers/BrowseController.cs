using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PhaseOneRecords.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PhaseOneRecords.RestApi.SpotifyAPI;
using PhaseOneRecords.RestApi.ShopifyApi;
using PhaseOneRecords.RestApi.ShopifyApi.Responses;
using PhaseOneRecords.RestApi.ShopifyApi.Responses.DTOs;
using PhaseOneRecords.RestApi.ShopifyApi.Requests.DTOs;
using SpotifyAPI.Web;
using Microsoft.AspNetCore.Http;

namespace PhaseOneRecords.Controllers
{
    public class BrowseController : Controller
    {
        private readonly ILogger<BrowseController> _logger;
        private readonly ISpotifyApiClient _spotifyClient;
        private readonly IShopifyApiClient _storeClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BrowseController(ILogger<BrowseController> logger, ISpotifyApiClient spotifyClient, IShopifyApiClient shopifyApiClient, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _spotifyClient = spotifyClient;
            _storeClient = shopifyApiClient;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Views
        public async Task<IActionResult> Index(string searchTerm = "", int itemsPerPage = 10, string cursor = "", string sortValue = "sortKey: VENDOR", string genres = null, string searchArtist = null, string searchAlbum = null)
        {
            var request = new GetProductsRequest(FormatSearchValue(searchTerm), itemsPerPage, cursor, new BrowseFilterModel(sortValue, genres), FormatSearchValue(searchArtist), FormatSearchValue(searchAlbum));
            GetProductsResponse response = null;
            var sessionNodes = GetNodesFromSession();

            List<ArtistModel> artists = null;
            SimpleAlbum spotifyAlbum = null;
            List<ArtistModel> relatedArtists = null;
            List<string> responseGenres = null;

            bool usingSession = sessionNodes != null;

            // Check if session is available, else call Graph QL directly to retrieve artists
            if (usingSession)
                artists = ParseSessionNodesToArtists(sessionNodes, searchTerm, itemsPerPage, sortValue, genres, searchArtist, searchAlbum);
            else
            {
                response = await _storeClient.GetProducts(request);

                if (response.Success && response.Body != null)
                {
                    artists = response.Body.Products.Edges.Select(w => new ArtistModel(w.Node.ArtistName.Value, w.Node.Title, w.Node.Description, w.Node.OnlineStoreUrl, GetNodeImages(w.Node), GetNodePrices(w.Node))).ToList();

                    if (response.Body.Products.PageInfo.HasNextPage)
                        cursor = response.Body.Products.Edges.Last().Cursor;
                    else
                        cursor = null;
                }
            }

            if (artists != null && artists.Count > 0)
            {
                responseGenres = usingSession ? sessionNodes.SelectMany(w => w.Tags).Distinct().ToList() : response.Body.Products.Edges.SelectMany(w => w.Node.Tags).Distinct().ToList();

                bool artistAlbumPage = searchArtist != null || searchAlbum != null;

                if (artistAlbumPage)
                {
                    // If displaying album on page, get it from Spotify API
                    if (searchAlbum != null)
                    {
                        var spotifyRequest = new RestApi.SpotifyAPI.Request.SpotifySearchRequest(searchArtist + " " + searchAlbum);
                        var spotifyResponse = await _spotifyClient.TextSearch(spotifyRequest);

                        if (spotifyResponse.Success && spotifyResponse != null)
                            spotifyAlbum = spotifyResponse.Response.Albums.Items.Where(w => w.Name.ToLower() == searchAlbum.ToLower() && w.Artists.Select(a => a.Name.ToLower()).Contains(searchArtist.ToLower())).FirstOrDefault();
                    }

                    // Get related artists
                    if (usingSession)
                    {
                        relatedArtists = GetRelatedArtistsFromSession(sessionNodes, artists, artists.First().Tags);
                        responseGenres = artists.First().Tags;
                    }
                    else
                    {
                        List<string> artistGenres = response.Body.Products.Edges.First().Node.Tags;

                        var relatedArtistsRequest = new RestApi.ShopifyApi.Requests.DTOs.GetProductsRequest("", 10, "", new BrowseFilterModel("sortKey: VENDOR", string.Join(",", artistGenres)));
                        var releatedArtitstResponse = await _storeClient.GetProducts(relatedArtistsRequest);

                        if (releatedArtitstResponse.Success && releatedArtitstResponse.Body != null)
                        {
                            relatedArtists = releatedArtitstResponse.Body.Products.Edges.Select(w => new ArtistModel(w.Node.ArtistName.Value, w.Node.Title, w.Node.Description, w.Node.OnlineStoreUrl, GetNodeImages(w.Node), GetNodePrices(w.Node))).ToList();
                            relatedArtists = relatedArtists.Where(w => w.ArtistName != artists.First().ArtistName).ToList();

                            if (releatedArtitstResponse.Body.Products.PageInfo.HasNextPage)
                                cursor = releatedArtitstResponse.Body.Products.Edges.Last().Cursor;
                            else
                                cursor = null;
                        }
                    }
                }
            }

            return View("Index", new BrowseViewModel(request, artists, cursor, relatedArtists, spotifyAlbum, responseGenres));
        }
        public IActionResult Search()
        {
            return View(new BrowseFilterViewModel());
        }
        public async Task<IActionResult> Landing()
        {
            var nodesFromSession = GetNodesFromSession();

            if (nodesFromSession == null)
                await GetProductsRecursive(new List<RestApi.ShopifyApi.Responses.DTOs.Node>());

            return View();
        }
        public IActionResult Filter()
        {
            return View("Filter", new BrowseFilterViewModel(GetProductTags()));
        }
        public IActionResult ApplyFilter(BrowseFilterViewModel filter)
        {
            string genres = null;
            if (filter.GenreValues != null && filter.GenreValues.Count > 0)
                genres = string.Join(",", filter.GenreValues);

            return RedirectToAction("Index", new { @sortValue = filter.SortValue, @genres = genres, @searchArtist = filter.SearchArtist, @searchAlbum = filter.SearchAlbum, @searchTerm = filter.SearchTerm });
        }
        #endregion

        #region Helpers
        public async Task<JsonResult> LoadMoreArtists(string cursor = "", string searchTerm = "", int itemsPerPage = 10, string sortValue = "sortKey: VENDOR", string genres = null, string searchArtist = null, string searchAlbum = null, int pageFrom = 1, bool relatedArtists = false)
        {
            var request = new RestApi.ShopifyApi.Requests.DTOs.GetProductsRequest(searchTerm, itemsPerPage, cursor, new BrowseFilterModel(sortValue, genres), searchArtist, searchAlbum);
            var sessionNodes = GetNodesFromSession();

            List<ArtistModel> artists = null;
            if (sessionNodes != null)
            {
                if (relatedArtists)
                {
                    artists = ParseSessionNodesToArtists(sessionNodes, searchTerm, itemsPerPage, sortValue, genres, null, searchAlbum, pageFrom);
                    artists = GetRelatedArtistsFromSession(sessionNodes, artists, artists.First().Tags, pageFrom, itemsPerPage);
                }
                else 
                    artists = ParseSessionNodesToArtists(sessionNodes, searchTerm, itemsPerPage, sortValue, genres, searchArtist, searchAlbum, pageFrom);
            }
            else
            {
                var response = await _storeClient.GetProducts(request);

                artists = null;

                if (response.Success && response.Body != null)
                    artists = response.Body.Products.Edges.Select(w => new ArtistModel(w.Node.ArtistName.Value, w.Node.Title, w.Node.Description, w.Node.OnlineStoreUrl, GetNodeImages(w.Node), GetNodePrices(w.Node))).ToList();

                cursor = null;
                if (response.Success)
                    cursor = response.Body.Products.Edges.Last().Cursor;
            }

            var model = new BrowseViewModel(request, artists, cursor);

            return new JsonResult(new ArtistsWithCursorModel()
            {
                Html = this.RenderViewToStringAsync("~/Views/Browse/_Artists.cshtml", model.Artists, true).Result,
                Cursor = cursor,
                PageFrom = pageFrom
            });
        }
        private List<string> GetProductTags()
        {
            var sessionNodes = GetNodesFromSession();

            if (sessionNodes != null)
                return sessionNodes.Where(w => w.Tags.Count > 0).SelectMany(w => w.Tags).Distinct().ToList();

            return null;
        }
        public List<string> GetNodeImages(RestApi.ShopifyApi.Responses.DTOs.Node node)
        {
            if (node.Images.Edges != null && node.Images.Edges.Count > 0)
                return node.Images.Edges.Select(w => w.Node.OriginalSrc).ToList();

            return new List<string>();
        }
        public List<decimal> GetNodePrices(RestApi.ShopifyApi.Responses.DTOs.Node node)
        {
            if (node.Variants.Edges != null && node.Variants.Edges.Count > 0)
                return node.Variants.Edges.Select(w => w.Node.Price).ToList();

            return new List<decimal>();
        }

        private string FormatSearchValue(string searchTerm)
        {
            if (searchTerm != null)
                return searchTerm.Replace("\"", "");
            else
                return searchTerm;
        }
        #endregion

        #region Session Nodes
        private List<RestApi.ShopifyApi.Responses.DTOs.Node> GetNodesFromSession()
        {
            if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Session != null && _httpContextAccessor.HttpContext.Session.Keys.Contains("AllProducts"))
                return JsonConvert.DeserializeObject<List<RestApi.ShopifyApi.Responses.DTOs.Node>>(_httpContextAccessor.HttpContext.Session.GetString("AllProducts"));

            return null;
        }
        private List<ArtistModel> ParseSessionNodesToArtists(List<Node> sessionNodes, string searchTerm = "", int itemsPerPage = 10, string sortValue = "sortKey: VENDOR", string genres = null, string searchArtist = null, string searchAlbum = null, int pageFrom = 1)
        {
            if (searchTerm != null && searchTerm.Length > 0)
            {
                searchTerm = searchTerm.ToLower().Replace(" ", "");

                sessionNodes = sessionNodes.Where(w =>
                (w.ArtistName != null && w.ArtistName.Value != null &&
                (w.ArtistName.Value.ToLower().Replace(" ", "").Contains(searchTerm) ||
                w.ArtistName.Value.ToLower().Replace(" ", "").StartsWith(searchTerm) ||
                w.ArtistName.Value.ToLower().Replace(" ", "").EndsWith(searchTerm))) ||
                w.Title.ToLower().Replace(" ", "").Contains(searchTerm) ||
                w.Description.ToLower().Replace(" ", "").Contains(searchTerm)).ToList();
            }
            else if (searchArtist != null && searchArtist.Length > 0 || searchAlbum != null && searchAlbum.Length > 0)
            {
                searchArtist = searchArtist?.ToLower().Replace(" ", "");
                searchAlbum = searchAlbum?.ToLower().Replace(" ", "");

                if (searchArtist != null && searchArtist.Length > 0 && searchAlbum != null && searchAlbum.Length > 0)
                    sessionNodes = sessionNodes.Where(w => w.ArtistName != null && w.ArtistName.Value != null && w.ArtistName.Value.ToLower().Replace(" ", "") == searchArtist && w.Title.ToLower().Replace(" ", "") == searchAlbum).ToList();
                else if (searchArtist != null && searchArtist.Length > 0)
                    sessionNodes = sessionNodes.Where(w => w.ArtistName != null && w.ArtistName.Value != null && w.ArtistName.Value.ToLower().Replace(" ", "") == searchArtist).ToList();
                else
                    sessionNodes = sessionNodes.Where(w => w.Title.ToLower().Replace(" ", "") == searchAlbum).ToList();
            }

            var browseFilterModel = new BrowseFilterModel(sortValue, genres);

            if (browseFilterModel.GenreValues != null && browseFilterModel.GenreValues.Count > 0)
            {
                var genreNodes = new List<RestApi.ShopifyApi.Responses.DTOs.Node>();
                browseFilterModel.GenreValues.ForEach(genre =>
                {
                    var matchedNodes = sessionNodes.Where(w => w.Tags.Contains(genre)).ToList();

                    if (matchedNodes.Count > 0)
                        genreNodes.AddRange(matchedNodes);
                });

                sessionNodes = genreNodes;
            }

            switch (browseFilterModel.SortValue)
            {
                case "sortKey: VENDOR":
                    sessionNodes = sessionNodes.Where(w => w.ArtistName != null && w.ArtistName.Value != null).OrderBy(w => w.ArtistName.Value).ToList();
                    break;
                case "sortKey: VENDOR, reverse: true":
                    sessionNodes = sessionNodes.Where(w => w.ArtistName != null && w.ArtistName.Value != null).OrderByDescending(w => w.ArtistName.Value).ToList();
                    break;
                case "sortKey: TITLE":
                    sessionNodes = sessionNodes.OrderBy(w => w.Title).ToList();
                    break;
                case "sortKey: TITLE, reverse: true":
                    sessionNodes = sessionNodes.OrderByDescending(w => w.Title).ToList();
                    break;
                case "sortKey: CREATED_AT":
                    sessionNodes = sessionNodes.Where(w => w.CreateDate != null && w.CreateDate.Value != null).OrderByDescending(w => w.CreateDate.Value).ToList();
                    break;
                case "sortKey: CREATED_AT, reverse: true":
                    sessionNodes = sessionNodes.Where(w => w.CreateDate != null && w.CreateDate.Value != null).OrderBy(w => w.CreateDate.Value).ToList();
                    break;
            }

            if (pageFrom > 1)
                sessionNodes = sessionNodes.Skip(itemsPerPage * pageFrom).Take(itemsPerPage).ToList();
            else
                sessionNodes = sessionNodes.Take(itemsPerPage).ToList();

            return sessionNodes.Select(w => new ArtistModel(w.ArtistName.Value.TrimStart(), w.Title, w.Description, w.OnlineStoreUrl, GetNodeImages(w), GetNodePrices(w), w.Tags)).ToList();
        }
        private List<ArtistModel> GetRelatedArtistsFromSession(List<Node> sessionNodes, List<ArtistModel> artists, List<string> tags, int pageFrom = 1, int itemsPerPage = 10)
        {
            // GET RELATED ARTISTS
            List<string> artistGenres = tags;
            var relatedArtists = new List<ArtistModel>();

            var relatedArtistsNodes = sessionNodes.Where(t => t.ArtistName != null && t.ArtistName.Value != null && t.Tags.All(t2 => artistGenres.Contains(t2))).ToList();

            if (relatedArtistsNodes != null && relatedArtistsNodes.Count > 0)
            {
                relatedArtists = relatedArtistsNodes.Select(w => new ArtistModel(w.ArtistName.Value, w.Title, w.Description, w.OnlineStoreUrl, GetNodeImages(w), GetNodePrices(w))).ToList();
                relatedArtists = relatedArtists.Where(w => w.ArtistName != artists.First().ArtistName).ToList();
                relatedArtists = relatedArtists.OrderBy(w => w.ArtistName).ToList();

                if (pageFrom > 1)
                    relatedArtists = relatedArtists.Skip(itemsPerPage * pageFrom).Take(itemsPerPage).ToList();
                else
                    relatedArtists = relatedArtists.Take(itemsPerPage).ToList();
            }

            return relatedArtists;
        }
        private async Task<List<Node>> GetProductsRecursive(List<Node> nodes = null, string cursor = null)
        {
            var request = new GetProductsRequest("", 250, cursor);
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

            return nodes;
        }
        #endregion
    }
}
