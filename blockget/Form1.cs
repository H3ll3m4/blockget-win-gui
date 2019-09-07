using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Collections;


//need : dotnet add package Ipfs.Http.Client
namespace blockget
{
    public partial class Form1 : Form
    {
        //Variables:

        //public string statusFromTextBox;

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem executeMenuItem, pauseMenuItem, quitMenuItem;
        public readonly Object LockQ = new Object();
        public Queue displayQ;
        Filesys fs;

        //public Form1(Queue myQ, Filesys filesys)
        public Form1()
        {
            InitializeComponent();
            //string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            notifyIconInit();
            //notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            //displayQ = myQ;
            //fs = new Filesys(this);
            fs = new Filesys();
            Globals.UserName = textBoxUserName.Text;
        }

        //TO DO :
        // render or delegate
        //https://stackoverflow.com/questions/661561/how-do-i-update-the-gui-from-another-thread
        private void ActualiseDisplay() {
            if (displayQ.Count > 0 )
                richTextBox1.Text = displayQ.Dequeue().ToString();
        }

 


        #region Buttons Forms for testing GUI

        /// <summary>
        /// Send button => Upload a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (String.Compare(buttonSend.Text, "Sending", false) == 0)
            {
                notifyIcon1.ShowBalloonTip(1000, "Important notice", "You interrupted the upload to IPFS", ToolTipIcon.Info);
                buttonSend.Text = "Send";
                //source.Cancel();
                //Console.WriteLine("Cancelling IPFS upload task");
            }

            if (String.Compare(buttonSend.Text, "Send", false) == 0)
            {
                IpfsBlkchn.connectDCore();
                if (!String.IsNullOrEmpty(textBoxFilePath.Text))
                {
                    Console.WriteLine("Upload File");
                    //First step:
                    //uploadFile(textBoxFilePath.Text);
                    //Second Step:
                    fs.EncryptSplitUploadFile(textBoxFilePath.Text);
                    //EncryptSplitUploadText(textBoxFilePath.Text);
                }
            }
        }
        /// <summary>
        /// Browse button: Content of Blockget folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Globals.BLOCKGET_FILE_FOLDER;
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                    textBoxFilePath.Text = filePath;
                    pictureBox1.Image = WindowsThumbnailProvider.GetThumbnail(
textBoxFilePath.Text, pictureBox1.Width, pictureBox1.Height, ThumbnailOptions.None);
                }
            }
            //MessageBox.Show(fileContent, "File Content at path: " + filePath, MessageBoxButtons.OK);
        }

        private void Form1_FormClosed(Object sender, FormClosedEventArgs e)
        {
            System.Text.StringBuilder messageBoxCS = new System.Text.StringBuilder();
            messageBoxCS.AppendFormat("{0} = {1}", "CloseReason", e.CloseReason);
            messageBoxCS.AppendLine();
            MessageBox.Show(messageBoxCS.ToString(), "FormClosed Event");
        }
        /// <summary>
        /// Download Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadFileButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Download File");
            IpfsBlkchn.DownloadFile(LookForCIDTextBox.Text, Globals.BLOCKGET_FILE_FOLDER + "\\dl.txt");
            //List<string> cids = GetFileNameCID();
            //for( int i = 1 ; i <= cids.Count - 1 ; i++)
            //{
            //    string pathTemporaryFile = cids[0] + ".tmp" // NEED TO CHECK HOW IT IS STORED WHEN
            //    DownloadFile(cids[i], pathTemporaryFile);
            //}
        }

        /// <summary>
        /// Search button: in the Hash table or HashSet Blockget File System
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(FilenameBlockgetSearchTextBox.Text))
            {
                //if (listFileNamesCids.Contains(FilenameBlockgetSearchTextBox.Text)) 
                // Console.WriteLine("The item {0} has been found in the Blockget File System.", FilenameBlockgetSearchTextBox.Text);
                //Look for the filename in the blockget file system
                if (!fs.SearchFileByPathBlockgetFS(FilenameBlockgetSearchTextBox.Text))
                {
                    Console.WriteLine("Couldn't find by path so we look for a filename");
                    richTextBox1.Text = "File found there:" + fs.SearchFileByNameBlockgetFS(FilenameBlockgetSearchTextBox.Text);
                }
                else {
                    Console.WriteLine("Find the file by path");
                }    
            }
            else Console.WriteLine("There is nothing to search for");
        }

        /// <summary>
        /// Recover Button: For test recover the file in TextBoxRecover.Text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRecover_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxRecover.Text))
            {
                fs.RecoverFile(textBoxRecover.Text);              
            }
        }
        #endregion

        public void setImage(string filepath) {
            pictureBox1.Image = WindowsThumbnailProvider.GetThumbnail(
                filepath, pictureBox1.Width, pictureBox1.Height, ThumbnailOptions.None);
        }

        #region notifyIcon

        void notifyIconInit()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.executeMenuItem = new System.Windows.Forms.MenuItem();
            this.pauseMenuItem = new System.Windows.Forms.MenuItem();
            this.quitMenuItem = new System.Windows.Forms.MenuItem();
            //this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip();
            //this.executeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            //this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            //this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            // Initialize contextMenu1
            //this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripMenuItem[] { this.executeToolStripMenuItem,
            //                this.pauseToolStripMenuItem, this.quitToolStripMenuItem });
            this.contextMenu1.MenuItems.AddRange(
                        new System.Windows.Forms.MenuItem[] { this.executeMenuItem,
                            this.pauseMenuItem, this.quitMenuItem });

            // Initialize menuItem1
            this.executeMenuItem.Index = 0;
            this.executeMenuItem.Text = "E&xecute";
            this.executeMenuItem.Click += new System.EventHandler(this.executeToolStripMenuItem_Click);

            // Initialize menuItem1
            this.pauseMenuItem.Index = 0;
            this.pauseMenuItem.Text = "Pause";
            this.pauseMenuItem.Click += new System.EventHandler(this.pauseToolStripMenuItem_Click);

            // Initialize menuItem1
            this.quitMenuItem.Index = 0;
            this.quitMenuItem.Text = "E&xit";
            this.quitMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            /*
            // Initialize menuItem1
            this.executeToolStripMenuItem.Text = "E&xecute";
            this.executeToolStripMenuItem.Click += new System.EventHandler(this.executeToolStripMenuItem_Click);

            // Initialize menuItem1
            this.pauseToolStripMenuItem.Text = "Pause";
            this.pauseToolStripMenuItem.Click += new System.EventHandler(this.pauseToolStripMenuItem_Click);

            // Initialize menuItem1
            this.quitToolStripMenuItem.Text = "E&xit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            */
            // Set up how the form should be displayed.
            //this.ClientSize = new System.Drawing.Size(292, 266);
            //this.Text = "Notify Icon Example";

            // Create the NotifyIcon.
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);

            // The Icon property sets the icon that will appear
            // in the systray for this application.
            notifyIcon1.Icon = new Icon(@"C:\Documents\Blockchain\HackXLR8\blockget-win-gui\nodes.ico");

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            notifyIcon1.ContextMenu = this.contextMenu1;
            //notifyIcon1.ContextMenuStrip = contextMenuStrip1;

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            notifyIcon1.Text = "Form1 (NotifyIcon example)";
            notifyIcon1.Visible = true;

            // Handle the DoubleClick event to activate the form.
            //notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            //notifyIcon1.MouseDoubleClick += new EventHandler(this.notifyIcon1_MouseDoubleClick);
            //notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_MouseDoubleClick);
            //https://stackoverflow.com/questions/8067246/no-overload-for-method-matches-delegate-system-eventhandler
        }


        private void executeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Activate();
            fs.getCurrentState();
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Activate();
            fs.SaveCurrentState();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Console.WriteLine("DoubleMouseClick!!");
            if (this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        #endregion notifyIcon





    }

}
