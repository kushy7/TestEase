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


//file that allows for the saving and opening of the configuration files
namespace TestEase.Services
{
    public class ConfigurationService
    {

        private static JsonSerializerOptions _options;

        //overall setting for the json serialization
        static ConfigurationService()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
        }

        //ensure or create the folder path for saving the config files exists
        public string GetFolderPath()
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Test Ease");

            //Ensure directory exists
            Directory.CreateDirectory(folderPath);

            return folderPath;
        }

        //saves the config in json
        public async Task SaveConfigurationAsync<T>(T configuration, string filename)
        {
            string fullPath = Path.Combine(GetFolderPath(), filename);
            string json = JsonSerializer.Serialize(configuration, _options);
            await File.WriteAllTextAsync(fullPath, json);
        }

        //desearlizes the json back into register objects
        public async Task<T> LoadConfigurationAsync<T>(string filename)
        {
            string fullPath = Path.Combine(GetFolderPath(), filename);
            if (File.Exists(fullPath))
            {
                string json = await File.ReadAllTextAsync(fullPath);
                return JsonSerializer.Deserialize<T>(json);
            }

            return default;

            
        }

        //the "folder" button in the top right of the saved configurations list
        public void OpenConfigurationFolderInExplorer()
        {
            string folderPath = GetFolderPath();
            Process.Start("explorer.exe", folderPath);
        }

    }
}
