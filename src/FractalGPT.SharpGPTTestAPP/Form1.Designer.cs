namespace FractalGPT.SharpGPTTestAPP
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.apiKeyOrHost = new System.Windows.Forms.TextBox();
            this.a_txt = new System.Windows.Forms.RichTextBox();
            this.q_txt = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.sendBtn = new System.Windows.Forms.Button();
            this.text_richtxt = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(202, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ключ OpenAI или FractalGPT, или хост:";
            // 
            // apiKeyOrHost
            // 
            this.apiKeyOrHost.Location = new System.Drawing.Point(13, 30);
            this.apiKeyOrHost.Name = "apiKeyOrHost";
            this.apiKeyOrHost.Size = new System.Drawing.Size(267, 20);
            this.apiKeyOrHost.TabIndex = 1;
            this.apiKeyOrHost.Text = "http://192.168.0.101:8080/";
            // 
            // a_txt
            // 
            this.a_txt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.a_txt.Location = new System.Drawing.Point(14, 88);
            this.a_txt.Name = "a_txt";
            this.a_txt.Size = new System.Drawing.Size(645, 367);
            this.a_txt.TabIndex = 2;
            this.a_txt.Text = "";
            // 
            // q_txt
            // 
            this.q_txt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.q_txt.Location = new System.Drawing.Point(12, 491);
            this.q_txt.Name = "q_txt";
            this.q_txt.Size = new System.Drawing.Size(648, 111);
            this.q_txt.TabIndex = 2;
            this.q_txt.Text = "Как пропалывать сорняки";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(10, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Ответ:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(11, 468);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "Вопрос";
            // 
            // sendBtn
            // 
            this.sendBtn.Location = new System.Drawing.Point(585, 608);
            this.sendBtn.Name = "sendBtn";
            this.sendBtn.Size = new System.Drawing.Size(75, 23);
            this.sendBtn.TabIndex = 4;
            this.sendBtn.Text = "Отправить";
            this.sendBtn.UseVisualStyleBackColor = true;
            this.sendBtn.Click += new System.EventHandler(this.sendBtn_Click);
            // 
            // text_richtxt
            // 
            this.text_richtxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.text_richtxt.Location = new System.Drawing.Point(678, 88);
            this.text_richtxt.Name = "text_richtxt";
            this.text_richtxt.Size = new System.Drawing.Size(500, 543);
            this.text_richtxt.TabIndex = 5;
            this.text_richtxt.Text = "";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(674, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 20);
            this.label4.TabIndex = 6;
            this.label4.Text = "Текст:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1063, 607);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(114, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Загрузка текста";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(1190, 643);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.text_richtxt);
            this.Controls.Add(this.sendBtn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.q_txt);
            this.Controls.Add(this.a_txt);
            this.Controls.Add(this.apiKeyOrHost);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.Text = "SharpGPT Test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox apiKeyOrHost;
        private System.Windows.Forms.RichTextBox a_txt;
        private System.Windows.Forms.RichTextBox q_txt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button sendBtn;
        private System.Windows.Forms.RichTextBox text_richtxt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
    }
}

