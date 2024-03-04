using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using FractalGPT.GUI.Models;
using FractalGPT.GUI.Bots;
using FractalGPT.SharpGPTLib.Prompts.PersonaChat;

namespace FractalGPT.GUI
{
    /// <summary>
    /// Логика взаимодействия для основной формы приложения.
    /// </summary>
    public partial class MainForm : Window
    {
        
        private readonly OpenFileDialog _openFileDialog; // Диалог для выбора файлов
        private string _filePath; // Путь к выбранному файлу
        private IBotInterface _bot; 


        /// <summary>
        /// Конструктор, инициализирующий компоненты формы и API клиента.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            _openFileDialog = new OpenFileDialog();

            string[] facts =
            {
                "Я люблю читать книги",
                "Я очень дружелюбный",
                "Меня зовут Сергей",
                "Мне 30 лет",
                "У меня есть машина",
                "Моего собеседника зовут User"
            };

            PersonaChat personaChat = new PersonaChat(facts);
            personaChat.Context = new PersonaContext(3);
            personaChat.StartConversation = "Недавно, у меня был следующий диалог: ";

            _bot = new PersonaBotInterface(this, personaChat, "http://192.168.0.101:8080/");
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
            _bot.Send(question);

            // Создаем новое сообщение и добавляем его в список сообщений
            MessagesList.Items.Add(new Message(question, Sender.User, filePath: _filePath));
            // Прокручиваем список сообщений до последнего элемента
            MessagesList.ScrollIntoView(MessagesList.Items[^1]);
            // Очищаем путь к файлу, если он был использован
            _filePath = string.Empty;
            // Очищаем текстовое поле после отправки сообщения
            TextMessage.Clear();
        }


       

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //_chatGptApi.Dispose();
        }
    }
}