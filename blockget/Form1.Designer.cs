namespace blockget
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFilePath = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxUserName = new System.Windows.Forms.TextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label3 = new System.Windows.Forms.Label();
            this.FilenameBlockgetSearchTextBox = new System.Windows.Forms.TextBox();
            this.SearchButton = new System.Windows.Forms.Button();
            this.DownloadFileButton = new System.Windows.Forms.Button();
            this.LookForCIDTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonRecover = new System.Windows.Forms.Button();
            this.textBoxRecover = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(283, 52);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowse.TabIndex = 0;
            this.buttonBrowse.Text = "Browse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(271, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select file to upload to Blockget and IPFS:";
            // 
            // textBoxFilePath
            // 
            this.textBoxFilePath.Location = new System.Drawing.Point(9, 82);
            this.textBoxFilePath.Name = "textBoxFilePath";
            this.textBoxFilePath.Size = new System.Drawing.Size(606, 22);
            this.textBoxFilePath.TabIndex = 2;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(16, 121);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(373, 330);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "Check the console => Delegate to other threads to implement";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(395, 121);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(319, 330);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Username";
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.Location = new System.Drawing.Point(85, 31);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Size = new System.Drawing.Size(104, 22);
            this.textBoxUserName.TabIndex = 6;
            this.textBoxUserName.Text = "dw-hellema";
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(621, 81);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(86, 23);
            this.buttonSend.TabIndex = 7;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.button2_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(195, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(176, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "Look for a file on Blockget:";
            // 
            // FilenameBlockgetSearchTextBox
            // 
            this.FilenameBlockgetSearchTextBox.Location = new System.Drawing.Point(377, 29);
            this.FilenameBlockgetSearchTextBox.Name = "FilenameBlockgetSearchTextBox";
            this.FilenameBlockgetSearchTextBox.Size = new System.Drawing.Size(238, 22);
            this.FilenameBlockgetSearchTextBox.TabIndex = 9;
            // 
            // SearchButton
            // 
            this.SearchButton.Location = new System.Drawing.Point(621, 28);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(86, 23);
            this.SearchButton.TabIndex = 10;
            this.SearchButton.Text = "Search";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // DownloadFileButton
            // 
            this.DownloadFileButton.Location = new System.Drawing.Point(621, 18);
            this.DownloadFileButton.Name = "DownloadFileButton";
            this.DownloadFileButton.Size = new System.Drawing.Size(86, 23);
            this.DownloadFileButton.TabIndex = 13;
            this.DownloadFileButton.Text = "Download";
            this.DownloadFileButton.UseVisualStyleBackColor = true;
            this.DownloadFileButton.Click += new System.EventHandler(this.DownloadFileButton_Click);
            // 
            // LookForCIDTextBox
            // 
            this.LookForCIDTextBox.Location = new System.Drawing.Point(188, 19);
            this.LookForCIDTextBox.Name = "LookForCIDTextBox";
            this.LookForCIDTextBox.Size = new System.Drawing.Size(427, 22);
            this.LookForCIDTextBox.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(149, 17);
            this.label4.TabIndex = 11;
            this.label4.Text = "Download a file by CID";
            // 
            // buttonRecover
            // 
            this.buttonRecover.Location = new System.Drawing.Point(621, 58);
            this.buttonRecover.Name = "buttonRecover";
            this.buttonRecover.Size = new System.Drawing.Size(86, 23);
            this.buttonRecover.TabIndex = 16;
            this.buttonRecover.Text = "Recover";
            this.buttonRecover.UseVisualStyleBackColor = true;
            this.buttonRecover.Click += new System.EventHandler(this.buttonRecover_Click);
            // 
            // textBoxRecover
            // 
            this.textBoxRecover.Location = new System.Drawing.Point(188, 59);
            this.textBoxRecover.Name = "textBoxRecover";
            this.textBoxRecover.Size = new System.Drawing.Size(427, 22);
            this.textBoxRecover.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 62);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(177, 17);
            this.label5.TabIndex = 14;
            this.label5.Text = "Recover File from Blockget";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(395, 95);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 17);
            this.label6.TabIndex = 17;
            this.label6.Text = "Thumbnail:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 95);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(121, 17);
            this.label7.TabIndex = 18;
            this.label7.Text = "Blockget Console:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.buttonBrowse);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBoxFilePath);
            this.groupBox1.Controls.Add(this.buttonSend);
            this.groupBox1.Controls.Add(this.LookForCIDTextBox);
            this.groupBox1.Controls.Add(this.DownloadFileButton);
            this.groupBox1.Location = new System.Drawing.Point(21, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(729, 122);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "IPFS";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.textBoxUserName);
            this.groupBox2.Controls.Add(this.pictureBox1);
            this.groupBox2.Controls.Add(this.FilenameBlockgetSearchTextBox);
            this.groupBox2.Controls.Add(this.richTextBox1);
            this.groupBox2.Controls.Add(this.buttonRecover);
            this.groupBox2.Controls.Add(this.SearchButton);
            this.groupBox2.Controls.Add(this.textBoxRecover);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(21, 144);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(729, 469);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "BlockGet";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(780, 614);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Blockget";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox textBoxFilePath;
        public System.Windows.Forms.RichTextBox richTextBox1;
        //public static System.Windows.Forms.RichTextBox richTextBox1; // It won't load => public static variable => need to invoke https://stackoverflow.com/questions/11995466/c-sharp-calling-form-show-from-another-thread
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxUserName;
        public System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox FilenameBlockgetSearchTextBox;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.Button DownloadFileButton;
        private System.Windows.Forms.TextBox LookForCIDTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonRecover;
        private System.Windows.Forms.TextBox textBoxRecover;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        //private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        //private System.Windows.Forms.ToolStripMenuItem executeToolStripMenuItem;
        //private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
        //private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
    }
}

