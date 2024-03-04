using FractalGPT.GUI.Models;
using FractalGPT.SharpGPTLib.API.ChatGPT;
using System;

namespace FractalGPT.GUI.Bots
{
    [Serializable]
    public class ChatGPTBotInterface : IBotInterface
    {
        private readonly ChatGptApi _chatGptApi; // Клиент API для взаимодействия с ChatGpt
        SettingsWindow _settingsWindow;
        MainForm _window;

        public ChatGPTBotInterface(MainForm mainForm) 
        {
            _window = mainForm;
            _settingsWindow = new SettingsWindow();
            string apiKey = _settingsWindow.GetOpenAIKey();
            _chatGptApi = new ChatGptApi(apiKey, false);
            _chatGptApi.Answer += ChatGptApi_Answer;
            _chatGptApi.ProxyInfo += _chatGptApi_ProxyInfo;
        }

        public async void Send(string question) 
        {
            _chatGptApi.SendAsyncText(question);
        }


        /// <summary>
        /// Получает ответ от чат gpt
        /// </summary>
        private void ChatGptApi_Answer(string obj)
        {
            _window.MessagesList.Items.Add(new Message(obj, Sender.Bot));
            _window.MessagesList.ScrollIntoView(_window.MessagesList.Items[^1]); // Прокрутка до последнего сообщения
        }

        private void _chatGptApi_ProxyInfo(string obj)
        {
            _window.MessagesList.Items.Add(new Message(obj, Sender.Bot));
            _window.MessagesList.ScrollIntoView(_window.MessagesList.Items[^1]);
        }
    }
}
