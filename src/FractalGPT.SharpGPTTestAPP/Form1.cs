using System;
using System.Windows.Forms;
using FractalGPT.SharpGPTLib.API.ChatGPT;
using FractalGPT.SharpGPTLib.API.LocalServer;
using FractalGPT.SharpGPTLib.Prompts.FewShot;
using FractalGPT.SharpGPTLib.Prompts.PersonaChat;
using FractalGPT.SharpGPTLib.Task.DialogTasks;
using FractalGPT.SharpGPTLib.Task.PromptGeneration;
using FractalGPT.SharpGPTLib.Task.Summarizing;
using FractalGPT.SharpGPTLib.Task.TextQA;

namespace FractalGPT.SharpGPTTestAPP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            string[] facts =
            {
                "Я консультант по произведениям Лермонтова",
                "Меня зовут Сергей",
                "Мне 30 лет",
                "У меня есть машина",
                "Я знаю что Лермонтов не умер, а был похищен инопланетянами",
                "Моего собеседника зовут User"
            };

            personaChat = new PersonaChat(facts);
            personaChat.Context = new PersonaContext(5);
            personaChat.StartConversation = "Недавно, у меня был следующий диалог";
        }

        PersonaChat personaChat;

        //TextDialog textDialog;
        BaseLLMServerAPI baseAPI = new BaseLLMServerAPI();
        string prompt = "Задание: Ответь на вопрос максимально точно.\nВопрос: \"{q}\"\nОтвет: ";
        string promptFred = "<SC6>Я парень, консультант по произведения Лермонтова. Мне 31 год. Меня зовут Сережа. Я люблю помогать собеседнику. Недавно, у меня был следующий диалог:\nТы: {q}\nЯ: <extra_id_0>";


        private async void sendBtn_Click(object sender, EventArgs e)
        {
            if(apiKeyOrHost.Text != string.Empty)
                baseAPI = new BaseLLMServerAPI(apiKeyOrHost.Text);

            //LocalServerWithFewShot localServerWithFewShot = new LocalServerWithFewShot(baseAPI, GetFewShotManager());

            personaChat.AddUserMessage(q_txt.Text);
           
            string input = personaChat.ToString()+personaChat.SepReplics+personaChat.BotTag+ " <extra_id_0>";
            string answer = await baseAPI.TextGeneration(input, maxLen: 100);
            answer = answer.Replace("<extra_id_0>", "");
            personaChat.AddAssistantMessage(answer);
            a_txt.Text = answer;

            //a_txt.Text = await textDialog.GenerateAsync(q_txt.Text);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (apiKeyOrHost.Text != string.Empty)
                baseAPI = new BaseLLMServerAPI(apiKeyOrHost.Text);

            await baseAPI.SetLLM("SiberiaSoft/SiberianPersonaFredLarge-2", "t5");
            //ChatGptApi chatGptApi = new ChatGptApi(apiKey.Text);
            //textDialog = new TextDialog(chatGptApi);
            //textDialog.LoadText(text_richtxt.Text);
        }


        private FewShotManager GetFewShotManager()
        {
            FewShotElement[] fewShotElements = 
            {
                new FewShotElement("Задание: Ответь на вопрос максимально точно.\nВопрос: \"Кто ты?\"\nОтвет: ", "Я дружелюбный ассистент)"),
                new FewShotElement("Задание: Ответь на вопрос максимально точно.\nВопрос: \"Кто такой Пушкин?\"\nОтвет: ", "Пушкин — русский поэт!"),
                new FewShotElement("Задание: Ответь на вопрос максимально точно.\nВопрос: \"Что такое транзистор?\"\nОтвет: ", "Это полупроводниковый твердотельный элемент с выводами, коллектор, эмиттер и база"),
            };

            return new FewShotManager(fewShotElements) { Sep = ""};
        }
    }
}
