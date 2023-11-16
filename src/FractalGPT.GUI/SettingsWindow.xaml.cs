using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace FractalGPT.GUI
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private string _path = "settings-api.json";
        private ApiSettings _apiSettings;

        public SettingsWindow()
        {
            InitializeComponent();
            LoadSettings();
        }

        /// <summary>
        /// Loads the API settings from a file if it exists, otherwise initializes new settings.
        /// </summary>
        private void LoadSettings()
        {
            if (File.Exists(_path))
            {
                string json = File.ReadAllText(_path);
                _apiSettings = JsonSerializer.Deserialize<ApiSettings>(json) ?? new ApiSettings();
            }
            else
            {
                _apiSettings = new ApiSettings();
                ShowDialog(); // Consider moving the ShowDialog() call out of the constructor
            }
        }

        /// <summary>
        /// Saves the current API settings to a file.
        /// </summary>
        private void SaveSettings()
        {
            string json = JsonSerializer.Serialize(_apiSettings);
            File.WriteAllText(_path, json);
        }

        /// <summary>
        /// Handles the click event of the save settings button.
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _apiSettings.KeyFractal = fractalKey.Password;
            _apiSettings.KeyOpenAI = openAIKey.Password;
            _apiSettings.KeyGigaChat = sberKey.Password;

            SaveSettings();
            Close();
        }

        /// <summary>
        /// Retrieves the stored Fractal API key, or requests it if it is missing.
        /// </summary>
        public string GetFractalKey() => GetApiKey(_apiSettings.KeyFractal, nameof(_apiSettings.KeyFractal));

        /// <summary>
        /// Retrieves the stored OpenAI key, or requests it if it is missing.
        /// </summary>
        public string GetOpenAIKey() => GetApiKey(_apiSettings.KeyOpenAI, nameof(_apiSettings.KeyOpenAI));

        /// <summary>
        /// Retrieves the stored GigaChat API key, or requests it if it is missing.
        /// </summary>
        public string GetGigaChatKey() => GetApiKey(_apiSettings.KeyGigaChat, nameof(_apiSettings.KeyGigaChat));

        /// <summary>
        /// A helper method to retrieve an API key from settings or prompt for it if it's missing.
        /// </summary>
        /// <param name="key">The API key from the settings.</param>
        /// <param name="keyName">The name of the key property for logging or error messages.</param>
        private string GetApiKey(string key, string keyName)
        {
            if (string.IsNullOrEmpty(key))
            {
                ShowDialog(); // A more refined logic may be required to prompt the user for the key
                              // After the dialog is closed, we need to get the updated key value
                key = keyName switch
                {
                    nameof(_apiSettings.KeyFractal) => _apiSettings.KeyFractal,
                    nameof(_apiSettings.KeyOpenAI) => _apiSettings.KeyOpenAI,
                    nameof(_apiSettings.KeyGigaChat) => _apiSettings.KeyGigaChat,
                    _ => key
                };
            }

            return key;
        }
    }


    /// <summary>
    /// Represents the API settings for the application.
    /// This class holds the keys necessary to interact with various APIs.
    /// </summary>
    public class ApiSettings
    {
        /// <summary>
        /// Gets or sets the API key for the Fractal GPT service.
        /// </summary>
        [JsonPropertyName("fractalGptKey")]
        public string KeyFractal { get; set; }

        /// <summary>
        /// Gets or sets the API key for the OpenAI service.
        /// </summary>
        [JsonPropertyName("openAiApiKey")]
        public string KeyOpenAI { get; set; }

        /// <summary>
        /// Gets or sets the API key for the GigaChat service.
        /// </summary>
        [JsonPropertyName("gigaChatKey")]
        public string KeyGigaChat { get; set; }
    }
}
