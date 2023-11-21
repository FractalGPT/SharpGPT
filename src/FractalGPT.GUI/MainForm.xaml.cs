using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using FractalGPT.GUI.Models;
using FractalGPT.SharpGPTLib.API.ChatGPT;
using FractalGPT.SharpGPTLib.MLWithLLM.Speech;

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
            _chatGptApi = new ChatGptApi(apiKey, false);
            _chatGptApi.Answer += ChatGptApi_Answer;
            _chatGptApi.ProxyInfo += _chatGptApi_ProxyInfo;
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
            // Проверяем, не пуст ли текст сообщения
            if (string.IsNullOrEmpty(TextMessage.Text))
                return;

            // Получаем текст вопроса из текстового поля
            string question = TextMessage.Text;
            _chatGptApi.SendAsyncText(question);
            // Создаем новое сообщение и добавляем его в список сообщений
            MessagesList.Items.Add(new Message(question, Sender.User, filePath: _filePath));
            // Прокручиваем список сообщений до последнего элемента
            MessagesList.ScrollIntoView(MessagesList.Items[^1]);
            // Очищаем путь к файлу, если он был использован
            _filePath = string.Empty;
            // Очищаем текстовое поле после отправки сообщения
            TextMessage.Clear();
        }


        /// <summary>
        /// Получает ответ от чат gpt
        /// </summary>
        private void ChatGptApi_Answer(string obj)
        {
            MessagesList.Items.Add(new Message(obj, Sender.GPT));
            MessagesList.ScrollIntoView(MessagesList.Items[^1]); // Прокрутка до последнего сообщения
            //speechSynthesizer.Speak(obj);
        }

        private void _chatGptApi_ProxyInfo(string obj)
        {
            MessagesList.Items.Add(new Message(obj, Sender.GPT));
            MessagesList.ScrollIntoView(MessagesList.Items[^1]);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //_chatGptApi.Dispose();
        }
    }
}