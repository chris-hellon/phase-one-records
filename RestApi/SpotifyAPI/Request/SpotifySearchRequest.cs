using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhaseOneRecords.RestApi.SpotifyAPI.Request
{
    public class SpotifySearchRequest
    {
        public string SearchTerm { get; set; }

        public SpotifySearchRequest(string searchTerm)
        {
            SearchTerm = searchTerm;
        }
    }
}
