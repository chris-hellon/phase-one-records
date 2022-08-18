using GetProductsDtos;
using PhaseOneRecords.RestApi.Response;
using PhaseOneRecords.RestApi.ShopifyApi.Responses.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhaseOneRecords.RestApi.ShopifyApi.Responses
{
    public class GetProductsResponse : BaseResponse
    {
        public Data Body { get; set; } 
    }
}
