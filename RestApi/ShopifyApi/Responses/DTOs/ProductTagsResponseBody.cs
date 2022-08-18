using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PhaseOneRecords.RestApi.ShopifyApi.Responses.DTOs
{
    public class ProductTagsResponseBody
    {
        public Data Body { get; set; }
    }

    public class ProductTags
    {
        [JsonPropertyName("pageInfo")]
        public PageInfo PageInfo { get; set; }

        [JsonPropertyName("edges")]
        public List<ProductTagEdge> Edges { get; set; }
    }

    public class ProductTagEdge
    {
        [JsonPropertyName("node")]
        public string Node { get; set; }

        [JsonPropertyName("cursor")]
        public string Cursor { get; set; }
    }
}
