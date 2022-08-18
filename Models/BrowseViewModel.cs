using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PhaseOneRecords.RestApi.ShopifyApi.Responses;

namespace PhaseOneRecords.Models
{
    public class BrowseViewModel
    {
        public List<ArtistModel> Artists { get; set; }
        public string Message { get; set; }
        public string AppliedFilter { get; set; }
        public string PageTitle { get; set; }
        public string Cursor { get; set; }
        public string SearchTerm { get; set; }
        public string Genres { get; set; }
        public string SortValue { get; set; }
        public string SearchArtist { get; set; }
        public string SearchAlbum { get; set; }
        public int PageFrom { get; set; } = 1;

        public SpotifyAPI.Web.SimpleAlbum SpotifyAlbum = null;
        public List<ArtistModel> RelatedArtists { get; set; } = null;

        public BrowseViewModel(RestApi.ShopifyApi.Requests.DTOs.GetProductsRequest request, List<ArtistModel> artists, string cursor, List<ArtistModel> relatedArtists = null, SpotifyAPI.Web.SimpleAlbum spotifyAlbum = null, List<string> genres = null)
        {
            SearchArtist = request.SearchArtist;
            SearchAlbum = request.SearchAlbum;
            Artists = artists;
            Cursor = cursor;      
            PageTitle = request.Filter.GetAppliedFilterLabel();
            SearchTerm = request.SearchTerm != null ? request.SearchTerm : null;
            Genres = request.Filter.GenreValues != null ? string.Join(",", request.Filter.GenreValues) : genres != null  ? string.Join(",", genres) : null;
            SortValue = request.Filter.SortValue != null ? request.Filter.SortValue : null;
            SpotifyAlbum = spotifyAlbum;
            RelatedArtists = relatedArtists;

            if (SearchArtist != null)
                PageTitle = "ARTIST // " + SearchArtist;

            if (SearchAlbum != null)
                PageTitle = "ALBUM // " + SearchAlbum;
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
    }
}
