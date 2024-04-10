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
using System.Net.Http.Headers;


namespace TestEase.Services
{
    internal class UpdateService
    {
        private const string gitHubEndpoint = "https://github.ncsu.edu/api/v3";
        private const string org = "engr-csc-sdc";
        private const string repo = "2024SpringTeam31-Hitachi-2";
        private string gitHubReleaseUrl = $"{gitHubEndpoint}/repos/{org}/{repo}/releases/latest";
        private string gitHubAssetUrl = $"{gitHubEndpoint}/repos/{org}/{repo}/releases/assets/";
        private string gitHubUpdaterUrl = $"{gitHubEndpoint}/repos/{org}/{repo}/releases/assets/270";
        private readonly HttpClient _httpClient;
        private bool updateAvailable = false;
        private static string parentPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName;
        private readonly string outputPathAsset = Path.Combine(parentPath, "publish.zip");
        private readonly string outputPathUpdater = Path.Combine(parentPath, "updater.zip");

        private readonly string executablePath = Path.Combine(parentPath, "publish\\TestEase.exe");
        private readonly string executableUpdater = Path.Combine(parentPath, "updater\\updater\\TestEaseUpdater.exe");


        private string gitHubReleaseVersion = "";
        
        public UpdateService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer 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");
            _httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            updateAvailable = checkForUpdate();
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
                        gitHubReleaseVersion = tagName;
                        if (tagName.StartsWith("v"))
                        {
                            tagName = tagName.Substring(1);
                        }
                        String current = Assembly.GetExecutingAssembly().GetName().Version.Major.ToString();

                        Trace.WriteLine("RELEASE TAG " + tagName);
                        Trace.WriteLine("CURRENT TAG " + current);

                        int releaseVersion = Convert.ToInt32(tagName);
                        int currentVersion = Convert.ToInt32(current);
                        if (releaseVersion > currentVersion)
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
                            // append the "id" of the asset
                            gitHubAssetUrl += idElement;
                        }
                    }
                    else
                    {
                        Trace.WriteLine("No asset id found");
                    }
                    Trace.WriteLine(jsonString);
                }
            }
        }

        public bool checkForUpdate()
        {
            Trace.WriteLine("Is update available: " + updateAvailable);
            getLatestVersion();
            return updateAvailable;
        }
        public async Task DownloadUpdater()
        {
            var currentAcceptHeader = _httpClient.DefaultRequestHeaders.Accept.FirstOrDefault();


            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
            var response = await _httpClient.GetAsync(gitHubUpdaterUrl);

            if (response.IsSuccessStatusCode)
            {
                using (var fileStream = File.Create(outputPathUpdater))
                using (var httpStream = await response.Content.ReadAsStreamAsync())
                {
                    await httpStream.CopyToAsync(fileStream);
                }
                Trace.WriteLine("Download Success", outputPathUpdater);
            }
            else
            {
                throw new Exception($"Failed to download file. HTTP response code: {response.StatusCode}");
            }
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            if (currentAcceptHeader != null)
            {
                _httpClient.DefaultRequestHeaders.Accept.Add(currentAcceptHeader);
            }
        }
        public async Task DownloadGitHubReleaseAsset(string assetUrl)
        {

            var currentAcceptHeader = _httpClient.DefaultRequestHeaders.Accept.FirstOrDefault();


            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
            var response = await _httpClient.GetAsync(assetUrl);

            if (response.IsSuccessStatusCode)
            {
                using (var fileStream = File.Create(outputPathAsset))
                using (var httpStream = await response.Content.ReadAsStreamAsync())
                {
                    await httpStream.CopyToAsync(fileStream);
                }
                Trace.WriteLine("Download Success", outputPathAsset);
            }
            else
            {
                throw new Exception($"Failed to download file. HTTP response code: {response.StatusCode}");
            }
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            if (currentAcceptHeader != null)
            {
                _httpClient.DefaultRequestHeaders.Accept.Add(currentAcceptHeader);
            }

        }

        public void ExtractZipFile()
        {
            if(!Directory.Exists(Path.Combine(parentPath, "publish")))
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(outputPathAsset, Path.Combine(parentPath, "publish"));
            } else
            {
                Trace.WriteLine("Publish already exists");
            }

            if (!Directory.Exists(Path.Combine(parentPath, "updater")))
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(outputPathUpdater, Path.Combine(parentPath, "updater"));
            }
            else
            {
                Trace.WriteLine("Updater already exists");
            }


        }



        public void performUpdate()
        {
            var startInfo = new ProcessStartInfo
            {
               FileName = executableUpdater,
                Arguments = executablePath,

            };
        
        
            Process.Start(startInfo);

            Application.Current.Quit();


        }

        public bool isUpdateAvailable()
        {
            return updateAvailable;
        }

        public string getAssetUrl()
        {
            return gitHubAssetUrl;
        }

        public string getGitHubReleaseVersion()
        {
            return gitHubReleaseVersion;
        }









    }
}
