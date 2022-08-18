using PhaseOneRecords.RestApi.Response;
using PhaseOneRecords.RestApi.SpotifyAPI.Request;
using PhaseOneRecords.RestApi.SpotifyAPI.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhaseOneRecords.RestApi.SpotifyAPI
{
    public interface ISpotifyApiClient
    {
        public Uri Login(SpotifyLoginRequest request);

        public Task<SpotifyProcessLoginResponse> ProcessLogin(SpotifyProcessLoginRequest request);

        public Task<SpotifySearchResponse> TextSearch(SpotifySearchRequest request);

    }
}
