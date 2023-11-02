using FractalGPT.GUI.Models;
using System;
using System.Windows;
using System.Windows.Media;

namespace FractalGPT.GUI
{
    /// <summary>
    /// Представляет сообщение в чате, содержащее текст, информацию об отправителе, времени отправки и другие свойства.
    /// </summary>
    [Serializable]
    public class Message
    {
        // Генератор случайных чисел, может использоваться, если ID сообщения генерируется случайным образом.
        private readonly Random _rnd = new();

        // Время создания сообщения.
        private readonly DateTime _date;

        // Уникальный идентификатор сообщения.
        public int ID { get; set; }

        // Текст сообщения.
        public string Text { get; set; }

        // Время создания сообщения в формате "HH:mm".
        public string Date => _date.ToString("HH:mm");

        // Определяет выравнивание сообщения в пользовательском интерфейсе (справа для пользователя, слева для GPT).
        public HorizontalAlignment Alignment { get; }

        // Цвет фона сообщения.
        public SolidColorBrush Brush { get; }

        // Путь к прикрепленному изображению (если есть).
        public string Img { get; set; }

        /// <summary>
        /// Создает новый экземпляр сообщения.
        /// </summary>
        /// <param name="text">Текст сообщения.</param>
        /// <param name="sender">Отправитель сообщения (пользователь или GPT).</param>
        /// <param name="id">Уникальный идентификатор сообщения (необязательно).</param>
        /// <param name="filePath">Путь к прикрепленному файлу (необязательно).</param>
        public Message(string text, Sender sender = Sender.User, int id = 0, string filePath = "")
        {
            _date = DateTime.Now; // Устанавливаем текущее время как время создания сообщения.
            Text = text;
            ID = id; // Можно модифицировать для создания уникального ID, например, используя _rnd.
            Img = filePath; // Прикрепленный файл (если есть).

            // Установка выравнивания сообщения в зависимости от отправителя.
            Alignment = sender == Sender.User ? HorizontalAlignment.Right : HorizontalAlignment.Left;

            // Выбор цвета сообщения в зависимости от отправителя.
            Brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(sender == Sender.User ? "#213857" : "#4D4FBC"));
        }
    }

}