using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TestEase.Models;
using TestEase.ViewModels;


namespace TestEase.Services
{
    public class ConfigurationService
    {

        private static JsonSerializerOptions _options;

        static ConfigurationService()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
        }

        public string GetFolderPath()
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Test Ease");

            // Ensure directory exists
            Directory.CreateDirectory(folderPath);

            return folderPath;
        }

        public async Task SaveConfigurationAsync<T>(T configuration, string filename)
        {
            string fullPath = Path.Combine(GetFolderPath(), filename);
            string json = JsonSerializer.Serialize(configuration, _options);
            await File.WriteAllTextAsync(fullPath, json);
        }

        public async Task<T> LoadConfigurationAsync<T>(string filename)
        {
            string fullPath = Path.Combine(GetFolderPath(), filename);
            if (File.Exists(fullPath))
            {
                string json = await File.ReadAllTextAsync(fullPath);
                return JsonSerializer.Deserialize<T>(json);
            }

            return default;

            //var service = new ConfigurationService();

            //string folderPath = service.GetFolderPath();
            //string[] files = Directory.GetFiles(folderPath, "*.json");

            //foreach (string file in files)
            //{
            //    try
            //    {
            //        string jsonContent = await File.ReadAllTextAsync(file);
            //        ConfigurationModel config = JsonSerializer.Deserialize<ConfigurationModel>(jsonContent);
            //        if (config != null)
            //        {
            //            appViewModel.Configurations.Add(config);
            //        }
            //    }
            //    catch (JsonException jsonEx)
            //    {
            //        // Handle JSON-specific exceptions, e.g., malformed JSON
            //        await Current.MainPage.DisplayAlert("Error", $"JSON Error from file {file}: {jsonEx.Message}", "OK");
            //    }
            //    catch (Exception ex)
            //    {
            //        // Handle other exceptions, e.g., file read errors
            //        await Current.MainPage.DisplayAlert("Error", $"Error loading configuration from file {file}: {ex.Message}", "OK");
            //    }
            //}
        }

        public void OpenConfigurationFolderInExplorer()
        {
            string folderPath = GetFolderPath();
            Process.Start("explorer.exe", folderPath);
        }

    }
}
