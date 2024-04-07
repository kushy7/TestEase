using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEase.Helpers;
using TestEase.Models;
using TestEase.ViewModels;
using System.Net.Http;
using System.IO;
using System.Text.Json;
using System.Reflection;


namespace TestEase.Services
{
    internal class UpdateService
    {
        private const String gitHubEndpoint = "https://github.ncsu.edu/api/v3";
        private const String org = "engr-csc-sdc";
        private const String repo = "2024SpringTeam31-Hitachi-2";
        private string gitHubReleaseUrl = $"{gitHubEndpoint}/repos/{org}/{repo}/releases/latest";
        private string gitHubAssetUrl = $"{gitHubEndpoint}/repos/{org}/{repo}/releases/assets/";
        private readonly HttpClient _httpClient;
        private bool updateAvailable = false;
        private String assetId = "";
        
        public UpdateService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer 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");
            _httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");     

        }

        public async Task DownloadGitHubReleaseAsset(string assetUrl, string outputPath)
        {
            var response = await _httpClient.GetAsync(assetUrl); 

            if (response.IsSuccessStatusCode)
            {
                using (var fileStream = File.Create(outputPath)) 
                using (var httpStream = await response.Content.ReadAsStreamAsync())
                {
                    await httpStream.CopyToAsync(fileStream);
                }
            }
            else
            {
               throw new Exception($"Failed to download file. HTTP response code: {response.StatusCode}");
            }
        }

        public async void getLatestVersion()
        {
            var response = await _httpClient.GetAsync(gitHubReleaseUrl);

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                // Parse JSON string into a JsonDocument
                using (JsonDocument doc = JsonDocument.Parse(jsonString))
                {
                    // Get the root element of the JSON
                    JsonElement root = doc.RootElement;

                    // Navigate to the "tag_name" property
                    if (root.TryGetProperty("tag_name", out JsonElement tagNameElement))
                    {
                        string tagName = tagNameElement.GetString();
                        if (tagName.StartsWith("v"))
                        {
                            tagName = tagName.Substring(1);
                        }
                        String current = Assembly.GetExecutingAssembly().GetName().Version.Major.ToString();

                        Trace.WriteLine("RELEASE TAG " + tagName);
                        Trace.WriteLine("CURRENT TAG " + current);

                        int releaseVersion = Convert.ToInt32(tagName);
                        int currentVersion = Convert.ToInt32(current);
                        if(releaseVersion > currentVersion)
                        {
                            updateAvailable = true;
                        }
                    }
                    else
                    {
                        // Handle the case when "tag_name" property is not found
                        Trace.WriteLine("Tag Name not found in JSON");
                    }

                    // Navigate to the "assets" array
                    if (root.TryGetProperty("assets", out JsonElement assetsElement) && assetsElement.ValueKind == JsonValueKind.Array)
                    {
                        // Get the first element in the "assets" array
                        JsonElement firstAssetElement = assetsElement.EnumerateArray().FirstOrDefault();

                        // Get the "id" property from the first asset
                        if (firstAssetElement.TryGetProperty("id", out JsonElement idElement) && idElement.ValueKind == JsonValueKind.Number)
                        {
                            // Return the "id" of the first asset
                            Trace.WriteLine(gitHubAssetUrl + idElement);
                        }
                    } else
                    {
                        Trace.WriteLine("No asset id found");
                    }
                    Trace.WriteLine(jsonString);
                }
            }
        }

        public bool checkForUpdate(String currentVersion)
        {
            Trace.WriteLine("Is update available: " + updateAvailable);
            getLatestVersion();
            return updateAvailable;
        }
    }
}
