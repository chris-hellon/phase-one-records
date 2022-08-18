using PhaseOneRecords.RestApi.Response;
using PhaseOneRecords.RestApi.SpotifyAPI.Request;
using PhaseOneRecords.RestApi.SpotifyAPI.Response;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace PhaseOneRecords.RestApi.SpotifyAPI
{
    public class SpotifyApiClient : ISpotifyApiClient
    {
        private readonly SpotifyClient _client;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly ISearchClient _searchClient;

        public SpotifyApiClient(SpotifyClientConfig config, string clientId, string clientSecret)
        {
            _client = new SpotifyClient(config);
            _clientId = clientId;
            _clientSecret = clientSecret;
            _searchClient = _client.Search;
        }

        public Uri Login(SpotifyLoginRequest request)
        {
            var loginRequest = new LoginRequest(
              request.CallbackUrl,
              _clientId,
              LoginRequest.ResponseType.Code
            )
            {
                Scope = new[] { Scopes.PlaylistReadPrivate, Scopes.PlaylistReadCollaborative }
            };
            var uri = loginRequest.ToUri();

            return uri;
        }


        public async Task<SpotifyProcessLoginResponse> ProcessLogin(SpotifyProcessLoginRequest request)
        {
            var response = new SpotifyProcessLoginResponse();

            try
            {
                var OAuthResponse = await new OAuthClient().RequestToken(
                       new AuthorizationCodeTokenRequest(_clientId, _clientSecret, request.Code, new Uri("https://localhost:44330/home/callback/"))
                       );

                response.AuthToken = OAuthResponse;
                response.Success = true;
                
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.ToString();
            }

            return response;
        }

        public async Task<SpotifySearchResponse> TextSearch(SpotifySearchRequest request)
        {
            var response = new  SpotifySearchResponse();

            try
            {
                var searchResponse = await _searchClient.Item(new SearchRequest(SearchRequest.Types.Album | SearchRequest.Types.Artist | SearchRequest.Types.Track, request.SearchTerm) { Market = "GB"});
                
                response.Response = searchResponse;
                response.Success = true;

            }
            catch (Exception ex){

                response.Success = false;
                response.Message = ex.ToString();
            }

            return response;

        }

    }
}
