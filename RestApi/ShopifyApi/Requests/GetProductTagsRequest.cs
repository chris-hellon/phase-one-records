using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhaseOneRecords.RestApi.ShopifyApi.Requests
{
    public class GetProductTagsRequest
    {
        public int ItemsPerPage { get; set; }
        public List<string> ExistingTags { get; set; }
        public string Cursor { get; set; }

        public GetProductTagsRequest(int itemsPerPage = 250, List<string> existingTags = null, string cursor = null)
        {
            ItemsPerPage = itemsPerPage;
            ExistingTags = existingTags;
            Cursor = cursor;
        }
    }
}
