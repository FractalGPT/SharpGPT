using System;
using System.Windows.Forms;
using FractalGPT.SharpGPTLib.API.ChatGPT;
using FractalGPT.SharpGPTLib.API.LocalServer;
using FractalGPT.SharpGPTLib.Prompts.FewShot;
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

        }

        //TextDialog textDialog;
        BaseLLMServerAPI baseAPI = new BaseLLMServerAPI();
        string prompt = "Задание: Ответь на вопрос максимально точно.\nВопрос: \"{q}\"\nОтвет: ";


        private async void sendBtn_Click(object sender, EventArgs e)
        {
            if(apiKeyOrHost.Text != string.Empty)
                baseAPI = new BaseLLMServerAPI(apiKeyOrHost.Text);

            LocalServerWithFewShot localServerWithFewShot = new LocalServerWithFewShot(baseAPI, GetFewShotManager());

           
            string input = prompt.Replace("{q}", q_txt.Text);
            a_txt.Text = await localServerWithFewShot.SendAsync(input, new GenerationParametrs() { MaxLen = 200, Temperature = 0.1});

            //a_txt.Text = await textDialog.GenerateAsync(q_txt.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
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
