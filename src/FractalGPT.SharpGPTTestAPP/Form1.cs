using System;
using System.Windows.Forms;
using FractalGPT.SharpGPTLib.API.ChatGPT;
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

        TextDialog textDialog;


        private async void sendBtn_Click(object sender, EventArgs e)
        {
            a_txt.Text = await textDialog.GenerateAsync(q_txt.Text);  
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChatGptApi chatGptApi = new ChatGptApi(apiKey.Text);
            textDialog = new TextDialog(chatGptApi);
            textDialog.LoadText(text_richtxt.Text);
        }
    }
}
