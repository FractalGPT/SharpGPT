using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using FractalGPT.GUI.Models;
using FractalGPT.SharpGPTLib.API.ChatGPT;
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
        private readonly string _apiKey = "api_key"; // Ключ API для доступа к ChatGpt

        private readonly ChatGptApi _chatGptApi; // Клиент API для взаимодействия с ChatGpt

        /// <summary>
        /// Конструктор, инициализирующий компоненты формы и API клиента.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            _openFileDialog = new OpenFileDialog();
            _chatGptApi = new ChatGptApi(_apiKey);
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
            string answer = await _chatGptApi.SendAsyncReturnText(question);

            // Добавление сообщений в список сообщений
            MessagesList.Items.Add(new Message(question, Sender.User, filePath: _filePath));
            MessagesList.Items.Add(new Message(answer, Sender.GPT));
            MessagesList.ScrollIntoView(MessagesList.Items[^1]); // Прокрутка до последнего сообщения

            _filePath = string.Empty; // Очистка пути файла после отправки
            TextMessage.Clear(); // Очистка текстового поля
        }
    }
}