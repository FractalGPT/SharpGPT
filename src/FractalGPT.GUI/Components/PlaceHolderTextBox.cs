using System;
using System.Windows.Controls;

namespace FractalGPT.GUI.Components
{
    /// <summary>
    /// Представляет элемент управления TextBox с текстом-заполнителем, который отображается, когда контрол пуст.
    /// </summary>
    public class PlaceHolderTextBox : TextBox
    {
        private bool _isPlaceHolder = true;
        private string _placeHolderText;

        /// <summary>
        /// Получает или устанавливает текст-заполнитель, отображаемый при отсутствии ввода.
        /// </summary>
        public string PlaceHolderText
        {
            get => _placeHolderText;
            set
            {
                _placeHolderText = value;
                if (_isPlaceHolder)
                {
                    SetPlaceholder();
                }
            }
        }

        /// <summary>
        /// Получает или устанавливает текст в текстовом поле, игнорируя текст-заполнитель.
        /// </summary>
        public new string Text
        {
            get => _isPlaceHolder ? string.Empty : base.Text;
            set
            {
                // Сброс состояния заполнителя, если устанавливается реальный текст.
                _isPlaceHolder = string.IsNullOrEmpty(value);
                base.Text = value ?? string.Empty; // Обеспечение того, что текст не установлен в null.
            }
        }

        public PlaceHolderTextBox()
        {
            // Инициализация контрола с установкой текста-заполнителя.
            this.LostFocus += SetPlaceholder;
            this.GotFocus += RemovePlaceHolder;
        }

        private void SetPlaceholder(object sender, EventArgs e)
        {
            // Проверка необходимости отображения заполнителя.
            if (string.IsNullOrEmpty(base.Text))
            {
                SetPlaceholder();
            }
        }

        private void RemovePlaceHolder(object sender, EventArgs e)
        {
            // Удаление заполнителя при получении фокуса.
            RemovePlaceHolder();
        }

        // Методы для управления текстом-заполнителем.
        private void SetPlaceholder()
        {
            base.Text = PlaceHolderText;
            _isPlaceHolder = true;
        }

        private void RemovePlaceHolder()
        {
            if (_isPlaceHolder)
            {
                base.Text = string.Empty;
                _isPlaceHolder = false;
            }
        }
    }
}

