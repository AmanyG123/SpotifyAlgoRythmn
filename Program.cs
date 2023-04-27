using System;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using System.Collections.Generic;
using System.Diagnostics;

namespace SpotifyAPIExample
{
    class Program
    {
        // Merge Sort algorithm 
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

        // Merge portion of Merge Sort
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


        //  Quick Sort algorithm method
        public static void QuickSort(List<KeyValuePair<Tuple<string, string>, int>> list, int left, int right)
        {
            if (left < right)
            {
                int pivotIndex = Partition(list, left, right);
                QuickSort(list, left, pivotIndex - 1);
                QuickSort(list, pivotIndex + 1, right);
            }
        }

        // Partion Function
        public static int Partition(List<KeyValuePair<Tuple<string, string>, int>> list, int left, int right)
        {
            int pivotValue = list[right].Value;
            int i = left - 1;

            for (int j = left; j <= right - 1; j++)
            {
                if (list[j].Value > pivotValue)
                {
                    i++;
                    Swap(list, i, j);
                }
            }
            Swap(list, i + 1, right);
            return i + 1;
        }

        // Swap Function
        public static void Swap(List<KeyValuePair<Tuple<string, string>, int>> list, int i, int j)
        {
            KeyValuePair<Tuple<string, string>, int> temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
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


            if (searchResult.Artists.Items.Count > 0)
            {
                var artist = searchResult.Artists.Items[0];
                Console.WriteLine($"Found artist: {artist.Name}\n");

                // Search for the top 50 tracks with the artist's name in the title
                var featuringSearchRequest = new SearchRequest(SearchRequest.Types.Track, $"track:{artist.Name}")
                {
                    Limit = 50
                };

                var featuringSearchResult = await spotify.Search.Item(featuringSearchRequest);

                // Create a dictionary to store the artist, tracks, and their popularity scores
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

                // Convert the dictionary to a list of KeyValuePair
                var trackPopularityList = trackPopularityMap.ToList();


                // Measure time taken by Merge Sort
                Stopwatch mergeSortTimer = new Stopwatch();
                mergeSortTimer.Start();
                var mergeSortedTracks = MergeSort(trackPopularityList);
                mergeSortTimer.Stop();
                TimeSpan mergeSortTime = mergeSortTimer.Elapsed;

                // Take the top 5 most popular featured tracks for Merge Sort
                var top5FeaturedTracksMergeSort = mergeSortedTracks.GetRange(0, Math.Min(5, mergeSortedTracks.Count));

                // Display the top 5 most popular featured tracks sorted by Merge Sort
                Console.WriteLine("Merge Sort: Top 5 most popular featured tracks:");
                foreach (var entry in top5FeaturedTracksMergeSort)
                {
                    Console.WriteLine($"Track: {entry.Key.Item1} by {entry.Key.Item2} (featuring {artist.Name}), Popularity score: {entry.Value}");
                }

                // Measure time taken by Quick Sort
                Stopwatch quickSortTimer = new Stopwatch();
                quickSortTimer.Start();
                QuickSort(trackPopularityList, 0, trackPopularityList.Count - 1);
                quickSortTimer.Stop();
                TimeSpan quickSortTime = quickSortTimer.Elapsed;

                // Take the top 5 most popular featured tracks for Quick Sort
                var top5FeaturedTracksQuickSort = trackPopularityList.GetRange(0, Math.Min(5, trackPopularityList.Count));

                // Display the top 5 most popular featured tracks sorted by Quick Sort
                Console.WriteLine("\nQuick Sort: Top 5 most popular featured tracks:");
                foreach (var entry in top5FeaturedTracksQuickSort)
                {
                    Console.WriteLine($"Track: {entry.Key.Item1} by {entry.Key.Item2} (featuring {artist.Name}), Popularity score: {entry.Value}");
                }

                // Compare Merge Sort and Quick Sort time
                Console.WriteLine($"\nTime taken by Merge Sort: {mergeSortTime}");
                Console.WriteLine($"Time taken by Quick Sort: {quickSortTime}");
            }
            else
            {
                Console.WriteLine("Artist not found.");
            }
        }
    }
}
