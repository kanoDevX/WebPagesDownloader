using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebPagesDownloader
{
    internal class Program
    {
        private static readonly HttpClient httpClient = new();
        static async Task Main(string[] args)
        {
            List<string> urls = new() // List of URLs to download (you can put any websites here)
            {
                "https://www.google.com",
                "https://www.youtube.com",
                "https://www.facebook.com",
                "https://www.twitter.com",
                "https://www.instagram.com",
                "https://www.wikipedia.org",
                "https://www.amazon.com",
                "https://www.netflix.com",
                "https://www.microsoft.com",
                "https://www.apple.com"
            };  // List of URLs to download (you can put any websites here)

            Console.WriteLine("Starting async web page loading...\n");

            try
            {
                List<Task<WebPageResponse>> tasks = new();
                foreach (var url in urls)
                {
                    tasks.Add(GettingInformationAsync(url));
                }

                var results = await Task.WhenAll(tasks);

                Console.WriteLine("\nLoading results:\n");
                Console.WriteLine($"{"URL",-40} {"Characters",-15} {"Status",-15}");
                Console.WriteLine(new string('-', 70));

                foreach (var result in results)
                {
                    if (result.Success)
                        Console.WriteLine($"{result.Url,-40} " +
                            $"{result.CharCount,-15} {result.Success,-15}");
                    else
                    {
                        Console.WriteLine($"{result.Url,-40} {"N/A",-15} {result.Success,-15}");
                        Console.WriteLine($"    Error: [{result.ErrorMessage}]");
                    }
                }

                int successfulCount = results.Count(x => x.Success);
                int totalChars = results.Where(x => x.Success).Sum(u => u.CharCount);

                Console.WriteLine(new string('-', 70));
                Console.WriteLine($"Successfully loaded: {successfulCount} of {results.Length}");
                Console.WriteLine($"Total characters: {totalChars}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical error! {ex.Message}");
            }
            finally
            {
                httpClient.Dispose();
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            Console.Clear();
        }
        private static async Task<WebPageResponse> GettingInformationAsync(string url)
        {
            try
            {
                Console.WriteLine($"Loading: {url}");

                if (!httpClient.DefaultRequestHeaders.Contains("User-Agent"))
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0.0.0 Safari/537.36");

                var response = await httpClient.GetAsync(url); response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                int contentCount = content.Length;

                Console.WriteLine($"Loaded: {url} - [{contentCount} characters]");

                return new WebPageResponse
                {
                    Url = url,
                    CharCount = contentCount,
                    Success = true,
                };
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP error for {url}: {ex.Message}");
                return new WebPageResponse
                {
                    Url = url,
                    Success = false,
                    ErrorMessage = $"HTTP Error: {ex.Message}"
                };
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"Timeout for {url}: {ex.Message}");
                return new WebPageResponse
                {
                    Url = url,
                    Success = false,
                    ErrorMessage = $"Timeout: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error for {url}: {ex.Message}");
                return new WebPageResponse
                {
                    Url = url,
                    Success = false,
                    ErrorMessage = $"Error: {ex.Message}"
                };
            }
        }
    }
    public class WebPageResponse
    {
        public string Url { get; set; } = string.Empty;
        public int CharCount { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
