using System;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using System.Collections.Generic;

namespace SpotifyAPIExample
{
    class Program
    {
        // Add Merge Sort algorithm method
        public static List<KeyValuePair<Tuple<string, string>, int>> MergeSort(List<KeyValuePair<Tuple<string, string>, int>> list)
        {
            if (list.Count <= 1)
                return list;

            List<KeyValuePair<Tuple<string, string>, int>> left = new List<KeyValuePair<Tuple<string, string>, int>>();
            List<KeyValuePair<Tuple<string, string>, int>> right = new List<KeyValuePair<Tuple<string, string>, int>>();

            int middle = list.Count / 2;
            for (int i = 0; i < middle; i++)
            {
                left.Add(list[i]);
            }
            for (int i = middle; i < list.Count; i++)
            {
                right.Add(list[i]);
            }

            left = MergeSort(left);
            right = MergeSort(right);
            return Merge(left, right);
        }

        public static List<KeyValuePair<Tuple<string, string>, int>> Merge(List<KeyValuePair<Tuple<string, string>, int>> left, List<KeyValuePair<Tuple<string, string>, int>> right)
        {
            List<KeyValuePair<Tuple<string, string>, int>> result = new List<KeyValuePair<Tuple<string, string>, int>>();

            while (left.Count > 0 || right.Count > 0)
            {
                if (left.Count > 0 && right.Count > 0)
                {
                    if (left[0].Value > right[0].Value)
                    {
                        result.Add(left[0]);
                        left.RemoveAt(0);
                    }
                    else
                    {
                        result.Add(right[0]);
                        right.RemoveAt(0);
                    }
                }
                else if (left.Count > 0)
                {
                    result.Add(left[0]);
                    left.RemoveAt(0);
                }
                else if (right.Count > 0)
                {
                    result.Add(right[0]);
                    right.RemoveAt(0);
                }
            }

            return result;
        }

        // ...\

        // Convert the dictionary to a list of KeyValuePair
     //   var trackPopularityList = trackPopularityMap.ToList();

        // Sort the tracks by popularity in descending order using Merge Sort
   //     var sortedTracks = MergeSort(trackPopularityList);

        // Take the top 5 most popular featured tracks
     //   var top5FeaturedTracks = sortedTracks.Take(5);

        // ...

        static async Task Main(string[] args)
        {
            var clientId = "2989616a929a4e0795fa42497949870c";
            var clientSecret = "80a70ea4bb4f4ff0947b1acf738b6af3";

            var config = SpotifyClientConfig.CreateDefault();
            var request = new ClientCredentialsRequest(clientId, clientSecret);
            var response = await new OAuthClient(config).RequestToken(request);

            var spotify = new SpotifyClient(config.WithToken(response.AccessToken));

            Console.Write("Enter an artist's name: ");
            string artistName = Console.ReadLine();

            // Search for the artist
            var searchRequest = new SearchRequest(SearchRequest.Types.Artist, artistName);
            var searchResult = await spotify.Search.Item(searchRequest);

            // ...

            if (searchResult.Artists.Items.Count > 0)
            {
                var artist = searchResult.Artists.Items[0];
                Console.WriteLine($"Found artist: {artist.Name}");

                // Search for the top 50 tracks with the artist's name in the title
                var featuringSearchRequest = new SearchRequest(SearchRequest.Types.Track, $"track:{artist.Name}")
                {
                    Limit = 50
                };
                var featuringSearchResult = await spotify.Search.Item(featuringSearchRequest);

                // Create a dictionary to store the tracks and their popularity scores
                var trackPopularityMap = new Dictionary<Tuple<string, string>, int>();

                // Filter tracks to keep only the ones where the searched artist is not the main artist
                foreach (var track in featuringSearchResult.Tracks.Items)
                {
                    if (track.Artists[0].Id != artist.Id && track.Artists.Any(a => a.Id == artist.Id))
                    {
                        var trackName = track.Name;
                        var mainArtist = track.Artists[0].Name;
                        var trackPopularity = track.Popularity;

                        var key = new Tuple<string, string>(trackName, mainArtist);
                        trackPopularityMap[key] = trackPopularity;
                    }
                }

                // Sort the tracks by popularity in descending order and take the top 5
                // Convert the dictionary to a list of KeyValuePair
                var trackPopularityList = trackPopularityMap.ToList();

                // Sort the tracks by popularity in descending order using Merge Sort
                var sortedTracks = MergeSort(trackPopularityList);

                // Take the top 5 most popular featured tracks
                var top5FeaturedTracks = sortedTracks.GetRange(0, Math.Min(5, sortedTracks.Count));

                // Display the top 5 most popular featured tracks
                Console.WriteLine("Top 5 most popular featured tracks:");
                foreach (var entry in top5FeaturedTracks)
                {
                    Console.WriteLine($"Track: {entry.Key.Item1} by {entry.Key.Item2} (featuring {artist.Name}), Popularity score: {entry.Value}");
                }
            }
            else
            {
                Console.WriteLine("Artist not found.");
            }

            // ...

        }
    }
}
