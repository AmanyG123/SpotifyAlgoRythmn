using System;
using System.Threading.Tasks;
using SpotifyAPI.Web;

namespace SpotifyAPIExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var clientId = "2989616a929a4e0795fa42497949870c";
            var clientSecret = "80a70ea4bb4f4ff0947b1acf738b6af3";

            var config = SpotifyClientConfig.CreateDefault();
            var request = new ClientCredentialsRequest(clientId, clientSecret);
            var response = await new OAuthClient(config).RequestToken(request);

            var spotify = new SpotifyClient(config.WithToken(response.AccessToken));
            
            // Search for a track
            var searchRequest = new SearchRequest(SearchRequest.Types.Track, "Neon Guts");
            var searchResult = await spotify.Search.Item(searchRequest);

            if (searchResult.Tracks.Items.Count > 0)
            {
                var track = searchResult.Tracks.Items[0];
                Console.WriteLine($"Found track: {track.Name} by {track.Artists[0].Name}");
            }
            else
            {
                Console.WriteLine("Track not found.");
            }
        }
    }
}
