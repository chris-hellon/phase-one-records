using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PhaseOneRecords.RestApi.ShopifyApi.Responses.DTOs
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);

    //public class ResponseBody
    //{
    //    [JsonPropertyName("data")]
    //    public Data Data { get; set; }
    //}

    public class Data
    {
        [JsonPropertyName("products")]
        public Products Products { get; set; }
    }


    public class Products
    {
        [JsonPropertyName("pageInfo")]
        public PageInfo PageInfo { get; set; }

        [JsonPropertyName("edges")]
        public List<Edge> Edges { get; set; }
    }


    public class Edge
    {
        [JsonPropertyName("node")]
        public Node Node { get; set; }

        [JsonPropertyName("cursor")]
        public string Cursor { get; set; }
    }
    public class MetafieldEdge
    {
        [JsonPropertyName("node")]
        public Node Node { get; set; }

        [JsonPropertyName("cursor")]
        public string Cursor { get; set; }
    }


    public class Node
    {
        [JsonPropertyName("altText")]
        public object AltText { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("originalSrc")]
        public string OriginalSrc { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("handle")]
        public string Handle { get; set; }

        [JsonPropertyName("onlineStoreUrl")]
        public string OnlineStoreUrl { get; set; }


        [JsonProperty("vendor")]
        public string Artist { get; set; }

        [JsonPropertyName("productType")]
        public string ProductType { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("totalInventory")]
        public int TotalInventory { get; set; }

        [JsonPropertyName("images")]
        public Images Images { get; set; }

        [JsonPropertyName("variants")]
        public Variants Variants { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("metaFields")]
        public MetaFields MetaFields { get; set; }

        [JsonPropertyName("artistName")]

        public ArtistName ArtistName { get; set; }

        [JsonPropertyName("createDate")]

        public CreateDate CreateDate { get; set; }
    }

    public class ArtistName
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
    public class CreateDate
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class PageInfo
    {
        [JsonPropertyName("hasNextPage")]
        public bool HasNextPage { get; set; }

        [JsonPropertyName("hasPreviousPage")]
        public bool HasPreviousPage { get; set; }
    }


    public class Variants
    {
        [JsonPropertyName("edges")]
        public List<Edge> Edges { get; set; }
    }


    public class Images
    {
        [JsonPropertyName("edges")]
        public List<Edge> Edges { get; set; }
    }

    public class MetaFields
    {
        [JsonPropertyName("edges")]
        public List<Edge> Edges { get; set; }
    }

    public class PriceRange
    {
        [JsonPropertyName("maxVariantPrice")]
        public decimal MaxPrice { get; set; }
        [JsonPropertyName("minVariantPrice")]
        public decimal MinPrice { get; set; }
    }
}
