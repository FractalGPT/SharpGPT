using System;
using System.Windows.Forms;
using FractalGPT.SharpGPTLib.API.ChatGPT;
using FractalGPT.SharpGPTLib.API.LocalServer;
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
        BaseAPI baseAPI = new BaseAPI();
        string prompt = "Ответь на вопрос \"{q}\" максимально точно.\nОтвет: ";


        private async void sendBtn_Click(object sender, EventArgs e)
        {
            //a_txt.Text = await textDialog.GenerateAsync(q_txt.Text);
            string input = prompt.Replace("{q}", q_txt.Text);
            a_txt.Text = await baseAPI.TextGeneration(input, 30);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ChatGptApi chatGptApi = new ChatGptApi(apiKey.Text);
            //textDialog = new TextDialog(chatGptApi);
            //textDialog.LoadText(text_richtxt.Text);
        }
    }
}
