using PhaseOneRecords.RestApi.Response;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhaseOneRecords.RestApi.SpotifyAPI.Response
{
    public class SpotifyProcessLoginResponse : BaseResponse
    {
        public AuthorizationCodeTokenResponse AuthToken { get; set; }
    }
}
