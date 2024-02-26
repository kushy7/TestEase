using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Dispatching;
using System.Threading.Tasks;

namespace TestEase.ViewModels
{
    public class SavedConfigurationsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<string> _configurationFiles = new ObservableCollection<string>();
        private FileSystemWatcher _fileSystemWatcher;

        public ObservableCollection<string> ConfigurationFiles
        {
            get => _configurationFiles;
            set
            {
                _configurationFiles = value;
                OnPropertyChanged(nameof(ConfigurationFiles));
            }
        }

        public SavedConfigurationsViewModel()
        {
            // Initialize and start the file system watcher
            InitializeFileSystemWatcher();
            // Load initial configurations
            LoadConfigurationsAsync();
        }

        private void InitializeFileSystemWatcher()
        {
            // Get the directory path where the configurations are saved
            string configFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            _fileSystemWatcher = new FileSystemWatcher(configFolderPath)
            {
                Filter = "*.json",
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName
            };

            // Subscribe to change events
            _fileSystemWatcher.Changed += OnConfigurationChanged;
            _fileSystemWatcher.Created += OnConfigurationChanged;
            _fileSystemWatcher.Deleted += OnConfigurationChanged;
            _fileSystemWatcher.Renamed += OnConfigurationChanged;
        }

        private void OnConfigurationChanged(object sender, FileSystemEventArgs e)
        {
           
            MainThread.BeginInvokeOnMainThread(async () => await LoadConfigurationsAsync());
        }

        public async Task LoadConfigurationsAsync()
        {
            // Clear existing files to refresh the list
            var files = Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "*.json");

            // Dispatch the update to the ObservableCollection to the UI thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ConfigurationFiles.Clear();
                foreach (var file in files)
                {
                    ConfigurationFiles.Add(Path.GetFileName(file));
                }
            });
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
