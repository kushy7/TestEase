using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TestEase.Models;

namespace TestEase.Services
{
    public class ConfigurationService
    {
        private const string ConfigurationsDirectory = "Configurations";

        public ConfigurationService()
        {
            Directory.CreateDirectory(ConfigurationsDirectory);
        }

        public async Task SaveConfigurationAsync(string configName, ModbusModel configuration)
        {
            string filePath = Path.Combine(ConfigurationsDirectory, configName + ".json");
            string json = JsonSerializer.Serialize(configuration);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<ModbusModel> LoadConfigurationAsync(string configName)
        {
            string filePath = Path.Combine(ConfigurationsDirectory, configName + ".json");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Configuration file not found.");
            }

            string json = await File.ReadAllTextAsync(filePath);
            if (json != null)
            {
                return JsonSerializer.Deserialize<ModbusModel>(json); // danger!
            } else
            {
                throw new FileNotFoundException("Configuration file is empty.");
            }
        }

        public void DeleteConfiguration(string configName)
        {
            string filePath = Path.Combine(ConfigurationsDirectory, configName + ".json");
            File.Delete(filePath);
        }

        public IEnumerable<string> GetSavedConfigurations()
        {
            var files = Directory.GetFiles(ConfigurationsDirectory, "*.json");
            foreach (var file in files)
            {
                yield return Path.GetFileNameWithoutExtension(file);
            }
        }

        // Additional methods for export/import, synchronization, etc.

    }
}
