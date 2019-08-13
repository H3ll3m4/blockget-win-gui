using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Ipfs.Http;
using System.IO;
using System.Threading;


//need : dotnet add package Ipfs.Http.Client
namespace blockget
{
    public partial class Form1 : Form
    {
        //Variables:
        string servertestnet = "https://testnet-api.dcore.io";
        CancellationTokenSource source = new CancellationTokenSource();
        CancellationToken token;
        //public string statusFromTextBox;

        //Multithreading semaphores
        private readonly Object LockCIDArray = new Object();
        //Queue<string> MessageQueue = new Queue<string>(); // queue.Enqueue("1");  =  queue.Dequeue();
        private readonly Object LockAllFileList = new Object();

        //File System
        //private Dictionary FileNameCID;
        //Dictionary<string, List<string, string>> FileNameCID = new Dictionary<string, List<string, string>>();
        //private int[] FileNameCID = new int[SplitNb];
        //private string[] FileNameCID = new string[3];
        List<string> FileNameCID = new List<string>();
        //We choose HashSet because filepath cannot be duplicated
        HashSet<List<string>> listFileNamesCids = new HashSet<List<string>>();

        /// <summary>
        /// Search File By Name in the BlockGet File System.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>string with the list of filepath found. It can be displayed by a RichTextBox</returns>
        //For the HashSet<List<string>>:
        //bool SearchFileByNameBlockgetFS(string name) {
        string SearchFileByNameBlockgetFS(string name) {
            //bool result = false;
            string result = "File : " + name + " found in: \n";
            int counter = 1;
            lock (LockAllFileList)
            {
                foreach (List<string> l in listFileNamesCids) {
                    string filePath = GetPathFileEntry(l);
                    if (filePath.Contains(name)){
                        result += counter.ToString() + " : " + filePath + "\n";
                        counter++;
                        Console.WriteLine("The item {0} has been found in the Blockget File System.", name);
                    }                      
                }           
            }
            return result;
        }
        /// <summary>
        /// Search File By File Path in the BlockGet File System.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns>Boolean if found or not</returns>
        bool SearchFileByPathBlockgetFS(string RequiredFile) {
            bool result = false;
            lock (LockAllFileList)
            {
                using (HashSet<List<string>>.Enumerator lEnumerator = listFileNamesCids.GetEnumerator())
                {
                    while (lEnumerator.MoveNext() && !result)
                    {
                        result = lEnumerator.Current.Contains(RequiredFile);
                        if (result)
                        {
                            Console.WriteLine("The item {0} has been found in the Blockget File System.", RequiredFile);
                            string filepath = GetPathFileEntry(lEnumerator.Current);
                            if (!String.IsNullOrEmpty(filepath))
                            {
                                pictureBox1.Image = WindowsThumbnailProvider.GetThumbnail(
                                   filepath, pictureBox1.Width, pictureBox1.Height, ThumbnailOptions.None);
                            }
                            else Console.WriteLine("Couldn't get the filepath of the file for thumbnail update");

                        }
                    }
                }     
            }
            return result;
        }
        /// <summary>
        /// Search a file with pathfile in blockget Folder and return the entry in Hash Table File System
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        List<string> SearchGetEntryByPath(string file)
        {
            List <string> result = null;
            lock (LockAllFileList)
            {
                using (HashSet<List<string>>.Enumerator lEnumerator = listFileNamesCids.GetEnumerator())
                {
                    while (lEnumerator.MoveNext())
                    {
                        if (lEnumerator.Current.Contains(file))
                        {
                            Console.WriteLine("The item {0} has been found in the Blockget File System.", file);
                            result = lEnumerator.Current;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get the File Path of an Entry of type List < string > .
        /// </summary>
        /// <param name="FileEntry"></param>
        /// <returns>string with the file path from the first character / byte of the list</returns>
        string GetPathFileEntry(List<string> FileEntry) {
            string result = "";
            if ((FileEntry != null) && (FileEntry.Any()))
            {//if ((FileEntry != null) && (!FileEntry.Any())) {
                Console.WriteLine("Filepath is {0}", FileEntry[0]);
                result += FileEntry[0];
            }
            return result;
        }
        /// <summary>
        /// Add an entry to the Hash Table File-Name-Cids.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool AddEntryListFileNamesCids(List<string> entry) {
            //Test first if the filepath is present has been done before choosing add entry or update entry
            bool result = false;
            lock (LockAllFileList) {
                result = listFileNamesCids.Add(entry);
                if (result)
                    Console.WriteLine("The entry has been added to the file system");
                else {
                    Console.WriteLine("The entry exist already in the file system");
                }
            }
            return result;
        }
        /// <summary>
        /// Remove an entry from the blockget file system
        /// </summary>
        /// <param name="entry"></param>
        void RemoveEntryListFileNameCids(List<string> entry) {
            lock (LockAllFileList)
            {
                listFileNamesCids.Remove(entry);
            }
        }
        /// <summary>
        /// Update and File Entry in the FileName-CID File System
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="NewEntry"></param>
        void UpdateEntryListFileNamesCids(string filepath, List<string> NewEntry) {
            List<string> previousEntry = SearchGetEntryByPath(filepath);
            if (previousEntry.Count != 0) {
                lock (LockAllFileList)
                {
                    RemoveEntryListFileNameCids(previousEntry);
                    AddEntryListFileNamesCids(NewEntry);
                }
            }
        }
        /// <summary>
        /// Return the current FileNameCID - //S1 - Global variable
        /// </summary>
        /// <returns>List < string > FileNameCID </returns>
        public List<string> GetFileNameCID() {
            lock (LockCIDArray)
            {
                for(int i = 0; i < FileNameCID.Count; i++)
                {
                    MessageBox.Show(FileNameCID[i]);
                }
                return FileNameCID;
            }
        }

        /// <summary>
        /// Add entry to current FileNameCID - TO UPDATE IF SPLIT_NB change
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="cid1"></param>
        /// <param name="cid2"></param>
        /// <param name="cid3"></param>
        public void AddFileNameCID(string filename, string cid1, string cid2, string cid3) {
            lock (LockCIDArray)
            {
                //for (int i = 0; i < SplitNb - 1; i++) {
                //    FileNameCID[i] = Liste[i];
                //}
                FileNameCID[0] = filename;
                FileNameCID[1] = cid1;
                FileNameCID[2] = cid2;
                FileNameCID[3] = cid3;
            }
        }
        /// <summary>
        /// Add entry to current FileNameCID - //S1 - Global variable
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="cid"></param>
        /// <param name="orderFile"></param>
        public void AddFileNameCID(string FilePath, string cid, int orderFile) {
            lock (LockCIDArray)
            {
                if (orderFile <= FileNameCID.Count) {                  
                    FileNameCID.Add(cid);
                    //FileNameCID[0] = FilePath; 
                    //FileNameCID[orderFile] = cid;
                    Console.WriteLine("We add {0} to {1} .", cid, FilePath);
                } else Console.WriteLine("Size of FileNameCID is {0}", FileNameCID.Count);
            }
        }

        /// <summary>
        /// Add current FileNameCID Array to the HashSet - //S1 - Global variable
        /// </summary>
        /// <param name="filepath"></param>
        void AddCurrentArrayListFileCids() {
            lock (LockCIDArray) {
                //First check if entry already exist?
                string filepath = GetPathFileEntry(GetFileNameCID()); //NOT GOING TO WORK BECAUSE GetFileNameCID() want LockCIDArray
                bool fileExist = SearchFileByPathBlockgetFS(filepath);
                if (fileExist) {
                    Console.WriteLine("The file already exist, we update {0} in Blockget", filepath);
                    UpdateEntryListFileNamesCids(filepath, GetFileNameCID());
                    } else {
                    AddEntryListFileNamesCids(FileNameCID);
                    Console.WriteLine("Adding current Entry to HashTable");
                }
            }
        }

        /// <summary>
        /// Creation IPFS Task: upload file to IPFS and add the CID on DCore. Drop file on IPFS using dotnet library: https://github.com/richardschneider/net-ipfs-http-client
        /// </summary>
        /// <param name="FilePath"></param>
        private async void creationIpfsTask(string FilePath) {
            var ipfs = new IpfsClient("http://localhost:5001");
            Ipfs.CoreApi.AddFileOptions options = default(Ipfs.CoreApi.AddFileOptions);
            //CancellationToken token = default(CancellationToken);
            token = source.Token;
            buttonSend.Text = "Sending";
            Ipfs.IFileSystemNode node = await ipfs.FileSystem.AddFileAsync(FilePath, options, token);

            buttonSend.Text = "Send";
            //We want to display a few properties:
            Console.WriteLine("Cid = {0}", node.Id); // Qma1PbECNH6DsoTps9pmtqTAZQ3s17jzQEgEVQ97X5BDHN
            Console.WriteLine("Size = {0}", node.Size); //20
            Console.WriteLine("Links = {0}", node.Links);
            Console.WriteLine("Directory? = {0}", node.IsDirectory);
            Console.WriteLine("DataBytes = {0}", node.DataBytes);
            Console.WriteLine("DataStream = {0}", node.DataStream);
            //We want to return the hash
            //return text; can't it's async
            richTextBox1.Text = "File on IPFS under the hash:" + node.Id;
            AddCidDCore(node.Id);
        }

        /// <summary>
        /// Add Cid on DCore using CallAPI func
        /// </summary>
        /// <param name="cid"></param>
        void AddCidDCore(string cid) {
            string parameterString = ",\"params\":[" + textBoxUserName.Text + ",dw-blockget," + cid + "," + "false]";
            Console.WriteLine("Add Cid DCore = {0}", parameterString);
            callAPI("send_message", parameterString);
        }

        /// <summary>
        /// Download a File form IPFS with the CID to the system filepath
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="filepath"></param>
        //Only download Text:
        private async void DownloadFile(string cid, string filepath) {
            var ipfs = new IpfsClient("http://localhost:5001");
            string content = await ipfs.FileSystem.ReadAllTextAsync(cid);
            Console.WriteLine("Read file from Cid {0} =  {1}", cid, content);
            richTextBox1.Text = "Read file from Cid " + cid + " = " + content;
            if (!String.IsNullOrEmpty(filepath)) {
                System.IO.File.WriteAllText(Globals.BLOCKGET_FILE_FOLDER + "donwload.txt", content);
            } else System.IO.File.WriteAllText(filepath, content);
        }

        /*
        //private string DownloadFile(string cid) { 
        private Task<string> DownloadFile(string cid) {
            var ipfs = new IpfsClient("http://localhost:5001");

            //string filePath = await Task.Run((string cid) => {
            //string filePath = await Task.Run(() => {
            string filePath;
            await Task.Run(() => {
                     //return ipfs.FileSystem.ReadAllTextAsync(cid);
                     filePath = ipfs.FileSystem.ReadAllTextAsync(cid);
            } );

            //await Task.Run(() =>
            //{
            //    //stream archivetar = ipfs.FileSystem.GetAsync(cid);
            //    //stream file = ipfs.FileSystem.ReadAllTextAsync(cid);
            //    ipfs.FileSystem.ReadAllTextAsync(cid);
            //});

            // Read in the specified file.
            // ... Use async StreamReader method.
            //using (StreamReader reader = new StreamReader(file))
            //{
            //    //string v = await reader.ReadToEndAsync();
            //    Stream file = await ipfs.FileSystem.ReadAllTextAsync(cid);
            //}

            //return Task.Run(() => ExpensiveTask());
            return filePath;
            //return Task.Run((string cid) => ipfs.FileSystem.ReadAllTextAsync(cid));
        }
        */

        /// <summary>
        /// Upload part #orderFile to IPFS and send a message on DCore
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="orderFile"></param>
        /// <exception cref = "member" > HttpRequestException </ exception >
        async void IpfsUploadDCore(string FilePath, int orderFile)
        {
            var ipfs = new IpfsClient("http://localhost:5001");
            Ipfs.CoreApi.AddFileOptions options = default(Ipfs.CoreApi.AddFileOptions);
            token = source.Token;
            buttonSend.Text = "Sending";
            try {
                Ipfs.IFileSystemNode node = await ipfs.FileSystem.AddFileAsync(FilePath, options, token);
                buttonSend.Text = "Send";
                textBoxFilePath.Text = "";
                Console.WriteLine("Cid = {0}", node.Id);
                Console.WriteLine("Size = {0}", node.Size);
                //We want to return the cid
                richTextBox1.Text = "File on IPFS under the cid:" + node.Id;
                // S1 - global var
                AddFileNameCID(FilePath, node.Id, orderFile);
                AddCidDCore(node.Id);
                File.Delete(FilePath);
            } catch (HttpRequestException) {
                Console.WriteLine("FAIL! Don't forget to run IPFS on your computer");
            }
        }

        /// <summary>
        /// Upload to Ipfs and add cid to DCore Blockchain
        /// </summary>
        /// <param name="SplittedFile"></param>
        /// <param name="EntryFileCid"></param>
        /// <exception cref = "member" > HttpRequestException </ exception >
        async void IpfsUploadDCore(string filepath, int orderFile, List<string> EntryFileCid) { //async void cannot have ref or out
            var ipfs = new IpfsClient("http://localhost:5001");
            Ipfs.CoreApi.AddFileOptions options = default(Ipfs.CoreApi.AddFileOptions);
            token = source.Token;
            buttonSend.Text = "Sending";
            try
            {
                Ipfs.IFileSystemNode node = await ipfs.FileSystem.AddFileAsync(filepath, options, token);
                buttonSend.Text = "Send";
                textBoxFilePath.Text = "";
                Console.WriteLine("Cid = {0}", node.Id);
                Console.WriteLine("Size = {0}", node.Size);
                //We want to return the cid
                richTextBox1.Text = "File on IPFS under the cid:" + node.Id;

                EntryFileCid[orderFile] += node.Id;
                Console.WriteLine("Adding the node.Id {0} at position {1} to the entry", node.Id, orderFile);
                AddCidDCore(node.Id);
                File.Delete(filepath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (e.InnerException is TaskCanceledException )
                        Console.WriteLine("=> IPFS Upload DCore failed. The task was alredy run before and now canceled");

                if (e.InnerException is HttpRequestException)
                    Console.WriteLine("=> IPFS Upload DCore failed. Don't forget to run IPFS on your computer");
                /*
                switch e.InnerException
            {
                case TaskCanceledException:
                    Console.WriteLine("IPFS Upload DCore failed. The task was alredy run before and now canceled");
                    break;
                case HttpRequestException:
                    Console.WriteLine("FAIL! Don't forget to run IPFS on your computer");
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }                
            */
            }

        }
        /// <summary>
        /// Upload File to IPFS and send a message to DCore
        /// </summary>
        /// <param name="file"></param>
        void uploadFile(string file) {
            Console.WriteLine("Upload the file : {0}", file);
            IpfsUploadDCore(file, 1);
        }

        /// <summary>
        /// Encrypt, Split, Upload the file on IPFS and Timestamp on DCore
        /// </summary>
        /// <param name="file">filepath</param>
        //string[] 
        void EncryptSplitUploadFile(string file)
        {
            //We encrypt
            string encryptedFile = Encryption.EncryptFile(file);

            //We split 
            string[] splittedFile = Split.SplitFile(encryptedFile);

            //Update or Create new Entry?
            bool FileExist = SearchFileByPathBlockgetFS(file);

            List<string> EntryFileCid = new List<string> { file , "", "", ""};
            // We can send
            for (int i = 0; i <= Globals.SPLIT_NB - 1; i++)
            {
                Console.WriteLine("Adding {0} at position {1} for {2} entry", splittedFile[i], i+1 , file);
                IpfsUploadDCore(splittedFile[i], i+1 , EntryFileCid);
            }

            //Add or Update? => Check if it exist or not
            if (FileExist)
            {
                Console.WriteLine("The file {0} already exist in Blockget, we update");
                UpdateEntryListFileNamesCids(file, EntryFileCid);
            }
            else {
                Console.WriteLine("Creating a new file in Blockget File System");
                AddEntryListFileNamesCids(EntryFileCid);
            }
            //We delete the temp files:
            //foreach (string tempFile in splittedFile) {
            //    File.Delete(tempFile);
            //}
        }
        /// <summary>
        /// Encrypt, Split, Upload the text on IPFS and Timestamp on DCore
        /// </summary>
        /// <param name="file">filepath</param>
        //string[] 
        void EncryptSplitUploadText(string file)
        {
            //We encrypt
            string encryptedFile = Encryption.EncryptTextFile(file);

            //We split 
            string[] splittedFile = Split.SplitFile(encryptedFile);

            //Update or Create new Entry?
            bool FileExist = SearchFileByPathBlockgetFS(file);

            List<string> EntryFileCid = new List<string> { file, "", "", "" };

            // We can send
            for (int i = 0; i <= Globals.SPLIT_NB - 1; i++)
            {
                Console.WriteLine("Adding {0} at position {1} for {2} entry", splittedFile[i], i + 1, file);
                //S2 - reference 
                IpfsUploadDCore(splittedFile[i], i + 1, EntryFileCid);
            }

            //Add or Update? => Check if it exist or not
            if (FileExist)
            {
                Console.WriteLine("The file {0} already exist in Blockget, we update");
                UpdateEntryListFileNamesCids(file, EntryFileCid);
            }
            else
            {
                Console.WriteLine("Creating a new file in Blockget File System");
                AddEntryListFileNamesCids(EntryFileCid);
            }
            //We delete the temp files:
            //foreach (string tempFile in splittedFile) {
            //    File.Delete(tempFile);
            //}
        }

        /// <summary>
        /// Recover the files from the array of CID and filename: download, merge and decrypt
        /// </summary>
        /// <param name="cidArray"></param>
        private async void RecoverFilesMergeDecrypt(string[] cidArray)
        {
            //We connect
            var ipfs = new IpfsClient("http://localhost:5001");
            foreach (string s in cidArray.Skip(0))
            {
                //We download
                string result = await ipfs.FileSystem.ReadAllTextAsync(s);
                //We want to save them in the SaveFolder + cidArray folder
            }
            // We merge
            string inputFolder = Globals.BLOCKGET_FILE_FOLDER + "\\" + cidArray[0] + "\\";
            Split.MergeFile(inputFolder);
            //We decrypt
            string pathFile = Globals.BLOCKGET_FILE_FOLDER + "\\" + cidArray[0];
            Encryption.DecryptFile(pathFile + ".aes");
        }

        /// <summary>
        /// Recover the files from the list of string: filename and CIDs: download, merge and decrypt
        /// </summary>
        /// <param name="cidArray"></param>
        private async void RecoverFilesMergeDecrypt(List<string> cidArray)
        {
            //We connect
            var ipfs = new IpfsClient("http://localhost:5001");
            if (cidArray.Count >= 0)
            {
                // First need to get the right folder. For ease of test, the part of the file are downloaded 
                // on a folder filename-extension, i.e. test.txt to blockget/test-txt
                string fileName = Path.GetFileNameWithoutExtension(cidArray[0]);
                string extensionPt = Path.GetExtension(cidArray[0]);
                //string extension = extensionPt.Skip(1).ToArray();
                string extension = extensionPt.Remove(0, 1);
                string inputFolder = Globals.BLOCKGET_FILE_FOLDER + fileName + "-" + extension + "\\";
                string[] SplittedFilesArray = new string[Globals.SPLIT_NB];
                string baseFileName = Path.GetFileNameWithoutExtension(cidArray[0]);
                int i = 0;

                foreach (string s in cidArray.Skip(1)) // We want to skip the first (contre intuitive, not 0)
                {
                    //SplittedFilesArray[i] = inputFolder + baseFileName + "." + 
                    //        i.ToString().PadLeft(5, Convert.ToChar("0")) + extensionPt + ".tmp";
                    SplittedFilesArray[i] = inputFolder + baseFileName + extensionPt + ".aes." +
                            i.ToString().PadLeft(5, Convert.ToChar("0")) + ".tmp";
                    //We download bytes => File
                    Stream result = await ipfs.FileSystem.ReadFileAsync(s);
                    //We want to save them in the SaveFolder + cidArray folder
                    //Could also try: Stream result = await ipfs.FileSystem.ReadFileAsync(s).Result;
                    using (FileStream DestinationStream = File.Create(SplittedFilesArray[i]))
                    {
                        await result.CopyToAsync(DestinationStream);
                    }
                    i++;
                }
                // We merge
                Split.MergeFile(inputFolder);
                //We decrypt
                //string pathEncryptedFile = inputFolder + fileName + ".aes"; //Globals.BLOCKGET_FILE_FOLDER + "\\" + cidArray[0];
                string pathEncryptedFile = inputFolder + baseFileName + extensionPt + ".aes";
                Encryption.DecryptFile(pathEncryptedFile);
            }
            else Console.WriteLine("Problem this entry is empty");
        }

        /// <summary>
        /// Recover the texts from the list of string: filename and CIDs: download, merge and decrypt
        /// </summary>
        /// <param name="cidArray"></param>
        private async void RecoverTextMergeDecrypt(List<string> cidArray)
        {
            //We connect
            var ipfs = new IpfsClient("http://localhost:5001");
            if (cidArray.Count >= 0)
            {
                // First need to get the right folder. For ease of test, the part of the file are downloaded 
                // on a folder filename-extension, i.e. test.txt to blockget/test-txt
                string fileName = Path.GetFileNameWithoutExtension(cidArray[0]);
                string extensionPt = Path.GetExtension(cidArray[0]);
                string extension = extensionPt.Remove(0, 1);
                string inputFolder = Globals.BLOCKGET_FILE_FOLDER + fileName + "-" + extension + "\\";
                string[] SplittedFilesArray = new string[Globals.SPLIT_NB];
                string baseFileName = Path.GetFileNameWithoutExtension(cidArray[0]);
                int i = 0;

                foreach (string s in cidArray.Skip(1)) // We want to skip the first (contre intuitive, not 0)
                {
                    SplittedFilesArray[i] = inputFolder + baseFileName + extensionPt + ".aes." +
                            i.ToString().PadLeft(5, Convert.ToChar("0")) + ".tmp";
                    //We download text => string
                    string result = await ipfs.FileSystem.ReadAllTextAsync(s);
                    System.IO.File.WriteAllText(SplittedFilesArray[i], result);
                    i++;
                }
                // We merge
                Split.MergeFile(inputFolder);
                //We decrypt
                string pathEncryptedFile = inputFolder + baseFileName + extensionPt + ".aes";
                Encryption.DecryptTextFile(pathEncryptedFile);
            }
            else Console.WriteLine("Problem this entry is empty");
        }

        //TODO
        //connect to DCore
        //https://docs.decent.ch/developer/group___database_a_p_i.html
        void connectDCore()
        {   //we want to send ,"params":["dw-hellema"]
            string parameterString = ",\"params\":[\"" + textBoxUserName.Text + "\"]";
            Console.WriteLine("Get account by name = {0}", parameterString);
            callAPI("get_account_by_name", parameterString);
        }

        /// <summary>
        /// Call the Dcore BlockChain API
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameterString"></param>
        ///<exception cref="member">Exception</exception>  
        //TODO: check how it sends several parameters:
        //C:\Documents\Blockchain\HackXLR8\DCore-Test-App-c#\DCore-Test-App-master\DCore API Test\DCore API Test
        //Async void not great, check: https://docs.microsoft.com/en-us/dotnet/api/system.threading.synchronizationcontext?view=netframework-4.8
        //and https://docs.microsoft.com/en-us/dotnet/api/system.appdomain.unhandledexception?view=netframework-4.8
        private async void callAPI(string method, string parameterString)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), servertestnet))
                    {
                        string content = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"" + method + "\"" + parameterString + "}";
                        request.Content = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
                        var response = await httpClient.SendAsync(request);
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        richTextBox1.Text = FormatJson(responseBody);
                        Console.WriteLine("DCore blockchain returned:", responseBody);
                    }
                }
                catch
                {
                    richTextBox1.AppendText("An unknown error occured");
                }
            }
        }

        private const string INDENT_STRING = "    ";
        /// <summary>
        /// Format to JSON
        /// </summary>
        /// <param name="json"></param>
        /// <returns>Json string</returns>
        static string FormatJson(string json)
        {

            int indentation = 0;
            int quoteCount = 0;
            var result =
                from ch in json
                let quotes = ch == '"' ? quoteCount++ : quoteCount
                let lineBreak = ch == ',' && quotes % 2 == 0 ? ch + Environment.NewLine + String.Concat(Enumerable.Repeat(INDENT_STRING, indentation)) : null
                let openChar = ch == '{' || ch == '[' ? ch + Environment.NewLine + String.Concat(Enumerable.Repeat(INDENT_STRING, ++indentation)) : ch.ToString()
                let closeChar = ch == '}' || ch == ']' ? Environment.NewLine + String.Concat(Enumerable.Repeat(INDENT_STRING, --indentation)) + ch : ch.ToString()
                select lineBreak == null
                            ? openChar.Length > 1
                                ? openChar
                                : closeChar
                            : lineBreak;

            return String.Concat(result);
        }

        public Form1()
        {
            InitializeComponent();
            //string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

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
                source.Cancel();
                Console.WriteLine("Cancelling IPFS upload task");
            }

            if (String.Compare(buttonSend.Text, "Send", false) == 0)
            {
                connectDCore();
                if (!String.IsNullOrEmpty(textBoxFilePath.Text))
                {
                    Console.WriteLine("Upload File");
                    //First step:
                    //uploadFile(textBoxFilePath.Text);
                    //Second Step:
                    EncryptSplitUploadFile(textBoxFilePath.Text);
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
            DownloadFile(LookForCIDTextBox.Text, Globals.BLOCKGET_FILE_FOLDER + "\\dl.txt");
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
                if (!SearchFileByPathBlockgetFS(FilenameBlockgetSearchTextBox.Text)) {
                    Console.WriteLine("Couldn't find by path so we look for a filename");
                    richTextBox1.Text = SearchFileByNameBlockgetFS(FilenameBlockgetSearchTextBox.Text);
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
                List<string> entry = SearchGetEntryByPath(textBoxRecover.Text);
                if (entry != null)
                {
                    if (entry.Count > 0)
                    {
                        RecoverFilesMergeDecrypt(entry);
                        //RecoverTextMergeDecrypt(entry);
                    }
                    else Console.WriteLine("This file hasn't been found in Blockget HashSet");
                }
                else Console.WriteLine("The file cannot be recovered from Blockget, maybe the system just initialised");
            }
        }
    }

}
