using GetProductsDtos;
using PhaseOneRecords.RestApi.Response;
using PhaseOneRecords.RestApi.ShopifyApi.Responses.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhaseOneRecords.RestApi.ShopifyApi.Responses
{
    public class GetProductTagsResponse : BaseResponse
    {
        public ProductTagsResponseBody Body { get; set; }
    }
}
