using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhaseOneRecords.RestApi.ShopifyApi.Requests.DTOs
{
    public class GetProductsRequest
    {
        public string SearchTerm { get; set; }

        public int ItemsPerPage { get; set; }

        public string Cursor { get; set; }

        public PhaseOneRecords.Models.BrowseFilterModel Filter { get; set; }

        public string SearchArtist { get; set; } = null;
        public string SearchAlbum { get; set; } = null;

        public List<string> ExistingTags { get; set; }

        public GetProductsRequest(string searchTerm = "", int itemsPerPage = 10, string cursor = "", Models.BrowseFilterModel filter = null, string searchArtist = null, string searchAlbum = null, List<string> existingTags = null)
        {
            SearchTerm = searchTerm;
            ItemsPerPage = itemsPerPage;
            Cursor = cursor;
            Filter = filter;
            SearchArtist = searchArtist;
            SearchAlbum = searchAlbum;
            ExistingTags = existingTags;
        }
    }
}
