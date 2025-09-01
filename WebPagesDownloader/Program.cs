using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Threading;

namespace WebPagesDownloader
{
    internal class Program
    {
        private static readonly HttpClient httpClient = new();
        static async Task Main(string[] args)
        {
            using CancellationTokenSource token = new();
            List<string> urls = new()
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
            };
            Console.WriteLine("Starting async web page loading...");
            Console.WriteLine("Press SPACEBAR to cancel loading\n");

            var waitForEnterTask = WaitForEnterAsync(token.Token);
            var downloadTask = DownloadAllUrlsAsync(urls, token.Token);

            try
            {
                var completedTask = await Task.WhenAny(waitForEnterTask, downloadTask);

                if (completedTask == waitForEnterTask)
                {
                    Console.WriteLine("\nThe tasks were canceled!\n");
                    token.Cancel();

                    await Task.Delay(1000);
                }
                else
                {
                    Console.WriteLine("All tasks were successfully downloaded");
                }

                var results = await downloadTask;
                if (results.Count > 0) BuildAndShowUrl(results);
                else Console.WriteLine("No URLs were successfully loaded");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                httpClient.Dispose();
            }
            Console.WriteLine("\nPress any key to exit");
            Console.ReadKey();
            Console.Clear();

        }
        private static async Task<List<WebPageResponse>> DownloadAllUrlsAsync(List<string> listOfUrls, CancellationToken cts)
        {
            var results = new List<WebPageResponse>();

            foreach (var url in listOfUrls)
            {
                if (cts.IsCancellationRequested)
                    break;

                try
                {
                    await Task.Delay(500, cts);

                    var result = await DownloadUrlAsync(url, cts);

                    if (!result.Success)
                    {
                        Console.WriteLine($"[FAILED] {url} - {result.ErrorMessage}");
                        continue;
                    }

                    results.Add(result);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Operation was cancelled before starting next request");
                    break;
                }
            }

            return results;
        }
        private static async Task<WebPageResponse> DownloadUrlAsync(string url, CancellationToken cts)
        {
            try
            {
                Console.WriteLine($"Loading: {url}");

                if (!httpClient.DefaultRequestHeaders.Contains("User-Agent"))
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0.0.0 Safari/537.36");

                var response = await httpClient.GetStringAsync(url, cts);
                if (string.IsNullOrEmpty(response)) Console.WriteLine("[Error] URL contains nothing");

                return new WebPageResponse
                {
                    Url = url,
                    CharCount = response.Length,
                    Success = true
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
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                Console.WriteLine($"Timeout for {url}: {ex.Message}");
                return new WebPageResponse
                {
                    Url = url,
                    Success = false,
                    ErrorMessage = $"Timeout: {ex.Message}"
                };
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine($"Operation cancelled for {url}");
                return new WebPageResponse
                {
                    Url = url,
                    Success = false,
                    ErrorMessage = ex.Message
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
        private static async Task WaitForEnterAsync(CancellationToken cts)
        {
            await Task.Run(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true);
                        if (ConsoleKey.Spacebar == key.Key)
                        {
                            break;
                        }
                    }
                    Thread.Sleep(100);
                }
            }, cts);
        }
        private static void BuildAndShowUrl(List<WebPageResponse> results)
        {
            Console.WriteLine("\nLoading results...\n");
            Console.WriteLine($"{"URL",-40} {"CharCount",-15} {"Success",-15}");
            Console.WriteLine(new string('-', 70));

            var successFullLoaded = results.Where(r => r.Success).Select(x => $"{x.Url,-40} {x.CharCount,-15} {x.Success,-15}");
            Console.WriteLine(string.Join(Environment.NewLine, successFullLoaded));

            var notSuccessFullLoaded = results.Where(r => !r.Success).Select(x => $"{x.Url,-40} {"-",-15} {x.Success,-15}");
            Console.WriteLine(string.Join(Environment.NewLine, notSuccessFullLoaded));

            Console.WriteLine(new string('-', 70));
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
