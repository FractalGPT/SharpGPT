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
        ApiSettings apiSettings;

        public SettingsWindow()
        {
            InitializeComponent();

            if (File.Exists(_path)) 
            {
                string json = File.ReadAllText(_path);
                apiSettings = JsonSerializer.Deserialize<ApiSettings>(json);
            }

            else 
            {
                apiSettings = new ApiSettings();
                ShowDialog();
            }

        }


        /// <summary>
        /// Save settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            apiSettings.KeyFractal = fractalKey.Password;
            apiSettings.KeyOpenAI = openAIKey.Password;
            apiSettings.KeyGigaChat = sberKey.Password;

            string json = JsonSerializer.Serialize(apiSettings);
            File.WriteAllText(_path, json);
            Close();
        }

        public string GetFractalKey()
        {
            var key = apiSettings.KeyFractal;
            if (string.IsNullOrEmpty(key)) ShowDialog();

            return apiSettings.KeyFractal;
        }

        public string GetOpenAIKey()
        {
            var key = apiSettings.KeyOpenAI;
            if (string.IsNullOrEmpty(key)) ShowDialog();

            return apiSettings.KeyOpenAI;
        }

        public string GetGigaChatKey()
        {
            var key = apiSettings.KeyGigaChat;
            if (string.IsNullOrEmpty(key)) ShowDialog();

            return apiSettings.KeyGigaChat;
        }

    }

    [Serializable]
    public class ApiSettings 
    {
        [JsonPropertyName("fractal-gpt key")]
        public string KeyFractal { get; set; }

        [JsonPropertyName("open-ai api key")]
        public string KeyOpenAI { get; set; }

        [JsonPropertyName("giga-chat key")]
        public string KeyGigaChat { get; set; }
    }
}
