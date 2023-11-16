using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using FractalGPT.GUI.Models;
using FractalGPT.SharpGPTLib.API.ChatGPT;
using FractalGPT.SharpGPTLib.MLWithLLM.Speech;
using Microsoft.Win32;

namespace FractalGPT.GUI
{
    /// <summary>
    /// Логика взаимодействия для основной формы приложения.
    /// </summary>
    public partial class MainForm : Window
    {
        
        private readonly OpenFileDialog _openFileDialog; // Диалог для выбора файлов
        private string _filePath; // Путь к выбранному файлу

        private readonly ChatGptApi _chatGptApi; // Клиент API для взаимодействия с ChatGpt
        SettingsWindow _settingsWindow;
        SpeechSynthesizerWrapper speechSynthesizer = new SpeechSynthesizerWrapper();


        /// <summary>
        /// Конструктор, инициализирующий компоненты формы и API клиента.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            _openFileDialog = new OpenFileDialog();
            _settingsWindow = new SettingsWindow();
            
            string apiKey = _settingsWindow.GetOpenAIKey();
            _chatGptApi = new ChatGptApi(apiKey);
            _chatGptApi.Answer += _chatGptApi_Answer;
            speechSynthesizer.SetVoice(0);
        }

       

        // Обработчик события клика по кнопке отправки сообщения
        private void SendButton_Click(object sender, RoutedEventArgs e) => Send();

        // Обработчик события для перемещения окна приложения
        private void MovingForm(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        // Обработчик события нажатия клавиши в текстовом поле сообщения
        private void MessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Send();
        }

        // Обработчик события выбора файла
        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            if (_openFileDialog.ShowDialog() == true)
                _filePath = _openFileDialog.FileName;
        }

        // Обработчик события закрытия приложения
        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        // Обработчик события сворачивания окна приложения
        private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        // Обработчик события для возможности перемещения окна
        private void MoveWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }





        /// <summary>
        /// Отправляет текстовое сообщение в ChatGpt и выводит ответ.
        /// </summary>
        private async void Send()
        {
            if (string.IsNullOrEmpty(TextMessage.Text))
                return;

            string question = TextMessage.Text;
            _chatGptApi.SendAsyncText(question); // Запрос

            // Добавление сообщений в список сообщений
            MessagesList.Items.Add(new Message(question, Sender.User, filePath: _filePath));
            MessagesList.ScrollIntoView(MessagesList.Items[^1]); // Прокрутка до последнего сообщения

            _filePath = string.Empty; // Очистка пути файла после отправки
            TextMessage.Clear(); // Очистка текстового поля
        }

        /// <summary>
        /// Получает ответ от чат gpt
        /// </summary>
        private void _chatGptApi_Answer(string obj)
        {
            MessagesList.Items.Add(new Message(obj, Sender.GPT));
            MessagesList.ScrollIntoView(MessagesList.Items[^1]); // Прокрутка до последнего сообщения
            //speechSynthesizer.Speak(obj);
        }
    }
}