using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhaseOneRecords.Models
{
    public class ArtistModel
    {
        public string ArtistName { get; set; }
        public string AlbumName { get; set; }
        public string Description { get; set; }
        public List<string> Images { get; set; }
        public List<decimal> Prices { get; set; }
        public string StoreUrl { get; set; }
        public List<string> Tags { get; set; }

        public ArtistModel()
        {

        }

        public ArtistModel(string artistName, string albumName, string description, string storeUrl, List<string> images = null, List<decimal> prices = null, List<string> tags = null)
        {
            ArtistName = artistName;
            AlbumName = albumName;
            Prices = prices;
            Images = images;
            StoreUrl = storeUrl;
            Description = description;
            Tags = tags;
        }
    }
}
