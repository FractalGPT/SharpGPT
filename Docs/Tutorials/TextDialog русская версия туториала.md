# Создание приложения Windows Forms с ChatGPT с использованием FractalGPT.SharpGPTLib

## Требования:
- Visual Studio 2019 или новее.
- Подписка на API openai (требуется ключ API).

## Шаг 1: Создание проекта
1. Откройте Visual Studio.
2. Выберите "Создать новый проект".
3. Выберите "Приложение Windows Forms (.NET Framework)" и нажмите "Далее".
4. Назовите проект, например, "SharpGPTTestApp", и нажмите "Создать".

## Шаг 2: Добавление ссылок на библиотеку
1. Клонируйте репозиторий FractalGPT SharpGPT с GitHub по [адресу](https://github.com/FractalGPT/SharpGPT/tree/main/src)
2. Добавьте ссылки на необходимые библиотеки в ваш проект в Visual Studio.

## Шаг 3: Дизайн формы
1. Откройте Form1 в режиме дизайнера.
2. Добавьте следующие элементы управления:
- TextBox для ввода вопроса (назовите его `q_txt`).
- Button для отправки запроса (назовите его `sendBtn`).
- RichTextBox для отображения ответа (назовите его `a_txt`).
- TextBox для ввода ключа API (назовите его `apiKey`).
- Button для инициализации соединения с API (назовите его `button1`).
- RichTextBox для загрузки текста (назовите его `text_richtxt`).

## Шаг 4: Написание кода
1. Дважды щелкните по кнопке `sendBtn` и `button1`, чтобы создать события нажатия.
2. Вставьте предоставленный код в файл `Form1.cs`.

- **Код на C#**

```csharp
using System;
using System.Windows.Forms;
using FractalGPT.SharpGPTLib.API.ChatGPT;
using FractalGPT.SharpGPTLib.Task.DialogTasks;

namespace FractalGPT.SharpGPTTestAPP
{
    public partial class Form1 : Form
    {
        // Инициализируем объект TextDialog для работы с диалогами.
        TextDialog textDialog;

        public Form1()
        {
            // Инициализация компонентов формы.
            InitializeComponent();
        }

        // Асинхронный обработчик события нажатия на кнопку отправки.
        private async void sendBtn_Click(object sender, EventArgs e)
        {
            // Генерация ответа на основе введенного текста и отображение его в текстовом поле.
            a_txt.Text = await textDialog.GenerateAsync(q_txt.Text);  
        }

        // Обработчик события нажатия на кнопку инициализации соединения с API.
        private void button1_Click(object sender, EventArgs e)
        {
            // Создаем экземпляр API, используя введенный ключ.
            ChatGptApi chatGptApi = new ChatGptApi(apiKey.Text);
            // Инициализируем объект TextDialog с созданным API.
            textDialog = new TextDialog(chatGptApi);
            // Загружаем текст для обработки в TextDialog.
            textDialog.LoadText(text_richtxt.Text);
        }
    }
}
```


## Шаг 5: Проверка и запуск
1. Введите ваш ключ API в соответствующее текстовое поле.
2. Загрузите текст, с которым должен работать ChatGPT, в `text_richtxt`.
3. Введите вопрос в `q_txt` и нажмите кнопку отправки.
4. Проверьте ответ в `a_txt`.

## Завершение:
Теперь у вас есть работающее приложение Windows Forms, которое использует библиотеку FractalGPT's SharpGPTLib для генерации ответов на основе введенного текста и вопросов с помощью ChatGPT. Экспериментируйте с различными запросами и посмотрите, какие интересные ответы вы можете получить от вашего AI-ассистента!