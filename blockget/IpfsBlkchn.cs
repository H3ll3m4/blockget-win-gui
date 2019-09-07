using Ipfs.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

namespace blockget
{
    class IpfsBlkchn
    {
        Filesys fs;
//        Form1 form;

        static string ipfsServer = "http://localhost:5001";
        static string servertestnet = "https://testnet-api.dcore.io";
        //CancellationTokenSource source = new CancellationTokenSource();
        //CancellationToken token;
        //REPLACED by CancellationToken cancel = default(CancellationToken)
        private const string INDENT_STRING = "    ";
        static Encoding e = Encoding.UTF8;
        static string testJPG = "QmbwaRc6qptDqjVHHSwZ6oqTHX1WpdMYhRHwFddHr24QAJ";
        static string testText = "Qma1PbECNH6DsoTps9pmtqTAZQ3s17jzQEgEVQ97X5BDHN";

        public IpfsBlkchn(Filesys filesys)
        //public IpfsBlkchn(Filesys filesys, Form1 f)
        {
//            form = f;
            fs = filesys;
        }

        #region IPFS
        static async private void Dl_String(string cid)
        {
            var ipfs = new IpfsClient();
            string text = await ipfs.FileSystem.ReadAllTextAsync(cid);
            Console.WriteLine("Received on IPFS : {0}", text);
            System.IO.File.WriteAllText(@"C:\Blockget\tesfile.txt", text, e);
            Console.WriteLine("String written on a textfile");
        }

        //TODO
        async public void Remove_File(string cid)
        {
            var ipfs = new IpfsClient();
            var id = await ipfs.Block.RemoveAsync(cid);
            Console.WriteLine("Remove Block on IPFS : {0}", cid);
        }

        static async private void Dl_File(string cid)
        {
            var ipfs = new IpfsClient();
            Stream SourceStream = await ipfs.FileSystem.ReadFileAsync(cid);
            //https://docs.microsoft.com/fr-fr/dotnet/api/system.io.stream?view=netframework-4.8
            using (FileStream DestinationStream = File.Create(@"C:\Blockget\testfile2.txt"))
            {
                await SourceStream.CopyToAsync(DestinationStream);
            }
        }

        static async private void Dl_JPG(string cid)
        {
            var ipfs = new IpfsClient();
            Stream SourceStream = await ipfs.FileSystem.ReadFileAsync(cid);
            using (FileStream DestinationStream = File.Create(@"C:\Blockget\testfile.jpg"))
            {
                await SourceStream.CopyToAsync(DestinationStream);
            }
        }

        static async private void Dl_tar(string cid)
        {
            var ipfs = new IpfsClient();
            Stream SourceStream = await ipfs.FileSystem.GetAsync(cid);
            using (FileStream DestinationStream = File.Create(@"C:\Blockget\testfile.tar"))
            {
                await SourceStream.CopyToAsync(DestinationStream);
            }
        }

        static async private void Dl_Result(string cid)
        {
            var ipfs = new IpfsClient();
            Stream SourceStream = ipfs.FileSystem.ReadFileAsync(cid).Result;
            using (FileStream DestinationStream = File.Create(@"C:\Blockget\testfile2.jpg"))
            {
                await SourceStream.CopyToAsync(DestinationStream);
            }
        }

        public static bool testDownloadStringTextFileAsync() 
        {
            //Arrange
            //Act
            // Assert
            bool result = false;
            try
            {
                Dl_String(testText); 
                string text = System.IO.File.ReadAllText(@"C:\Blockget\testfile.txt");
                Console.WriteLine("Contents of testfile.txt = {0}", text);
                result = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return result;
        }

        public static bool testDownloadFile()
        {
            //Arrange
            //Act
            // Assert
            bool result = false;
            try
            {
                Dl_File(testText);
                string text = System.IO.File.ReadAllText(@"C:\Blockget\testfile2.txt");
                Console.WriteLine("Contents of testfile.txt = {0}", text);
                result = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return result;
        }

        public static bool testDownloadJPG()
        {
            //Arrange
            //Act
            // Assert
            bool result = false;
            try
            {
                Dl_JPG(testJPG);
                Dl_Result(testJPG);
                Console.WriteLine("JPG downloaded => Check if it worked");
                result = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return result;
        }

        public async static void UploadFile() {
            var ipfs = new IpfsClient(ipfsServer);
            Ipfs.CoreApi.AddFileOptions options = default(Ipfs.CoreApi.AddFileOptions);
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            string path = "C:\\Blockget\\ifmalesuperfherodressedlikewomen.jpg";
            Ipfs.IFileSystemNode node = await ipfs.FileSystem.AddFileAsync(path, options, token);
            Console.WriteLine("Cid = {0}", node.Id);
            Console.WriteLine("Size = {0}", node.Size);
        }

        public static void testReadText()
        {
            var ipfs = new IpfsClient();
            var text = ipfs.FileSystem.ReadAllTextAsync(testText).Result;
            Console.WriteLine("Text received is {0}", text);
        }

        public static void testIpfsDownload()
        {
           //testDownloadStringTextFileAsync();
            //Thread.Wait(1000);
            //testDownloadFile();
            //UploadFile();
            //testDownloadJPG();
            //testReadText();
            //Need to add test for tar archive
        }

        /// <summary>
        /// Creation IPFS Task: upload file to IPFS and add the CID on DCore. Drop file on IPFS using dotnet library: https://github.com/richardschneider/net-ipfs-http-client
        /// </summary>
        /// <param name="FilePath"></param>
        private async void creationIpfsTask(string FilePath)
        {
            var ipfs = new IpfsClient(ipfsServer);
            Ipfs.CoreApi.AddFileOptions options = default(Ipfs.CoreApi.AddFileOptions);
            CancellationToken token = default(CancellationToken);
            //token = source.Token;
//            form.buttonSend.Text = "Sending";
            Ipfs.IFileSystemNode node = await ipfs.FileSystem.AddFileAsync(FilePath, options, token);

//            form.buttonSend.Text = "Send";
            //We want to display a few properties:
            Console.WriteLine("Cid = {0}", node.Id); // Qma1PbECNH6DsoTps9pmtqTAZQ3s17jzQEgEVQ97X5BDHN
            Console.WriteLine("Size = {0}", node.Size); //20
            Console.WriteLine("Links = {0}", node.Links);
            Console.WriteLine("Directory? = {0}", node.IsDirectory);
            Console.WriteLine("DataBytes = {0}", node.DataBytes);
            Console.WriteLine("DataStream = {0}", node.DataStream);
            //We want to return the hash
            //return text; can't it's async
//            form.richTextBox1.Text = "File on IPFS under the hash:" + node.Id;
            AddCidDCore(node.Id);
        }


        /// <summary>
        /// Download a File form IPFS with the CID to the system filepath
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="filepath"></param>
        //Only download Text:
        public static async void DownloadFile(string cid, string filepath)
        {
            var ipfs = new IpfsClient(ipfsServer);
            string content = await ipfs.FileSystem.ReadAllTextAsync(cid);
            Console.WriteLine("Read file from Cid {0} =  {1}", cid, content);
            //form.richTextBox1.Text = "Read file from Cid " + cid + " = " + content;
            if (!String.IsNullOrEmpty(filepath))
            {
                System.IO.File.WriteAllText(Globals.BLOCKGET_FILE_FOLDER + "donwload.txt", content);
            }
            else System.IO.File.WriteAllText(filepath, content);
        }




        /// <summary>
        /// Upload to Ipfs and add cid to DCore Blockchain
        /// </summary>
        /// <param name="SplittedFile"></param>
        /// <param name="EntryFileCid"></param>
        /// <exception cref = "member" > HttpRequestException </ exception >
        public async void IpfsUploadDCore(string filepath, int orderFile, List<string> EntryFileCid)
        { //async void cannot have ref or out
            var ipfs = new IpfsClient(ipfsServer);
            Ipfs.CoreApi.AddFileOptions options = default(Ipfs.CoreApi.AddFileOptions);
            CancellationToken token = default(CancellationToken);
            //token = source.Token;
//            form.buttonSend.Text = "Sending";
            try
            {
                Ipfs.IFileSystemNode node = await ipfs.FileSystem.AddFileAsync(filepath, options, token);
//                form.buttonSend.Text = "Send";
//                form.textBoxFilePath.Text = "";
                Console.WriteLine("Cid = {0}", node.Id);
                Console.WriteLine("Size = {0}", node.Size);
                //We want to return the cid
//                form.richTextBox1.Text = "File on IPFS under the cid:" + node.Id;

                EntryFileCid[orderFile] += node.Id;
                Console.WriteLine("Adding the node.Id {0} at position {1} to the entry", node.Id, orderFile);
                AddCidDCore(node.Id);
                File.Delete(filepath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (e.InnerException is TaskCanceledException)
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
        void uploadFile(string file)
        {
            Console.WriteLine("Upload the file : {0}", file);
            IpfsUploadDCore(file, 1);
        }

        /// <summary>
        /// Upload part #orderFile to IPFS and send a message on DCore
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="orderFile"></param>
        /// <exception cref = "member" > HttpRequestException </ exception >
        async void IpfsUploadDCore(string FilePath, int orderFile)
        {
            var ipfs = new IpfsClient(ipfsServer);
            Ipfs.CoreApi.AddFileOptions options = default(Ipfs.CoreApi.AddFileOptions);
            //token = source.Token;
            CancellationToken token = default(CancellationToken);
//            form.buttonSend.Text = "Sending";
            try
            {
                Ipfs.IFileSystemNode node = await ipfs.FileSystem.AddFileAsync(FilePath, options, token);
//                form.buttonSend.Text = "Send";
//                form.textBoxFilePath.Text = "";
                Console.WriteLine("Cid = {0}", node.Id);
                Console.WriteLine("Size = {0}", node.Size);
                //We want to return the cid
//                form.richTextBox1.Text = "File on IPFS under the cid:" + node.Id;
                // S1 - global var
                fs.AddFileNameCID(FilePath, node.Id, orderFile);
                //AddFileNameCID(FilePath, node.Id, orderFile);
                AddCidDCore(node.Id);
                File.Delete(FilePath);
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("FAIL! Don't forget to run IPFS on your computer");
            }
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
            bool FileExist = fs.SearchFileByPathBlockgetFS(file);

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
                fs.UpdateEntryListFileNamesCids(file, EntryFileCid);
            }
            else
            {
                Console.WriteLine("Creating a new file in Blockget File System");
                fs.AddEntryListFileNamesCids(EntryFileCid);
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
            var ipfs = new IpfsClient(ipfsServer);
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
        public async void RecoverFilesMergeDecrypt(List<string> cidArray)
        {
            //We connect
            var ipfs = new IpfsClient(ipfsServer);
            if (cidArray.Count >= 0)
            {
                // First need to get the right folder. For ease of test, the part of the file are downloaded 
                // on a folder filename-extension, i.e. test.txt to blockget/test-txt
                string fileName = Path.GetFileNameWithoutExtension(cidArray[0]);
                string extensionPt = Path.GetExtension(cidArray[0]);
                //string extension = extensionPt.Skip(1).ToArray();
                string extension = extensionPt.Remove(0, 1);
                //string inputFolder = Globals.BLOCKGET_FILE_FOLDER + fileName + "-" + extension + "\\";
                string inputFolder = Globals.OPERATION_FOLDER + fileName + "-" + extension + "\\";
                string[] SplittedFilesArray = new string[Globals.SPLIT_NB];
                string baseFileName = Path.GetFileNameWithoutExtension(cidArray[0]);
                int i = 0;
                //Create input forlder for download and merge
                Directory.CreateDirectory(inputFolder);
                foreach (string s in cidArray.Skip(1)) // We want to skip the first (contre intuitive, not 0)
                {
                    try
                    {
                        //SplittedFilesArray[i] = inputFolder + baseFileName + "." + 
                        //        i.ToString().PadLeft(5, Convert.ToChar("0")) + extensionPt + ".tmp";
                        SplittedFilesArray[i] = inputFolder + baseFileName + extensionPt + ".aes." +
                                i.ToString().PadLeft(5, Convert.ToChar("0")) + ".tmp";
                        //We download bytes => File
                        Stream result = await ipfs.FileSystem.ReadFileAsync(s);
                        //We want to save them in the SaveFolder + cidArray folder
                        //Could also try: Stream result = await ipfs.FileSystem.ReadFileAsync(s).Result;
                        using (FileStream DestinationStream = File.Create(SplittedFilesArray[i])) //TODO ERROR: System.IO.DirectoryNotFoundException: 'Could not find a part of the path 'C:\Blockget\ifmalesuperfherodresedlikewomen-jpg\ifmalesuperfherodresedlikewomen.jpg.aes.00000.tmp'.'
                        {
                            await result.CopyToAsync(DestinationStream);
                        }
                        i++;
                    } //System.Net.Http.HttpRequestException: 'invalid path "": path does not begin with '/''
                    catch (HttpRequestException e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine("FAIL! Don't forget to run IPFS on your computer");
                        break;
                    }

                }
                // We merge
                string pathEncryptedFile = Split.MergeFile(inputFolder);
                //We decrypt
                //string pathEncryptedFile = inputFolder + fileName + ".aes"; //Globals.BLOCKGET_FILE_FOLDER + "\\" + cidArray[0];
                //string pathEncryptedFile = inputFolder + baseFileName + extensionPt + ".aes";
                string outputPath = Encryption.DecryptFile(pathEncryptedFile);
                // Test if the file downloaded is the same as the one already present
                if (File.Exists(cidArray[0]) && Filesys.compare2FilesByBlocks(outputPath, cidArray[0]))
                {
                    Console.WriteLine("{0} and {1} have been compared bit by bit and are the same", outputPath, cidArray[0]);
                }
                else {
                    Console.WriteLine("{0} and {1} have been compared bit by bit and are different, we download and store the file in the Blockget Folder", outputPath, cidArray[0]);
                    // Could use the Path.Combine method to safely append the file name to the path.
                    // Will overwrite if the destination file already exists.
                    File.Copy(outputPath, cidArray[0], true);
                }
                //We delete folder and file downloaded
                File.Delete(outputPath);
                Directory.Delete(inputFolder);
                
            }
            else Console.WriteLine("Problem this entry is empty");
        }

        public async void RecoverFilesMergeDecryptSeparateFolder(List<string> cidArray)
        {
            //We connect
            var ipfs = new IpfsClient(ipfsServer);
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
                    try
                    {
                        //SplittedFilesArray[i] = inputFolder + baseFileName + "." + 
                        //        i.ToString().PadLeft(5, Convert.ToChar("0")) + extensionPt + ".tmp";
                        SplittedFilesArray[i] = inputFolder + baseFileName + extensionPt + ".aes." +
                                i.ToString().PadLeft(5, Convert.ToChar("0")) + ".tmp";
                        //We download bytes => File
                        Stream result = await ipfs.FileSystem.ReadFileAsync(s);
                        //We want to save them in the SaveFolder + cidArray folder
                        //Could also try: Stream result = await ipfs.FileSystem.ReadFileAsync(s).Result;
                        using (FileStream DestinationStream = File.Create(SplittedFilesArray[i])) //TODO ERROR: System.IO.DirectoryNotFoundException: 'Could not find a part of the path 'C:\Blockget\ifmalesuperfherodresedlikewomen-jpg\ifmalesuperfherodresedlikewomen.jpg.aes.00000.tmp'.'
                        {
                            await result.CopyToAsync(DestinationStream);
                        }
                        i++;
                    } //System.Net.Http.HttpRequestException: 'invalid path "": path does not begin with '/''
                    catch (HttpRequestException e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine("FAIL! Don't forget to run IPFS on your computer");
                        break;
                    }

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


        private async void RecoverTextMergeDecrypt(List<string> cidArray)
        {
            //We connect
            var ipfs = new IpfsClient(ipfsServer);
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
                Encryption.DecryptFileSeparateFolder(pathEncryptedFile);
            }
            else Console.WriteLine("Problem this entry is empty");
        }


        #endregion Ipfs

        #region DCore

        /// <summary>
        /// Add Cid on DCore using CallAPI func
        /// </summary>
        /// <param name="cid"></param>
        static void AddCidDCore(string cid)
        {
            string parameterString = ",\"params\":[" + Globals.UserName + ",dw-blockget," + cid + "," + "false]";
            Console.WriteLine("Add Cid DCore = {0}", parameterString);
            callAPI("send_message", parameterString);
        }

        //TODO
        //connect to DCore
        //https://docs.decent.ch/developer/group___database_a_p_i.html
        public static void connectDCore()
        {   //we want to send ,"params":["dw-hellema"]
            string parameterString = ",\"params\":[\"" + Globals.UserName + "\"]";
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
        private static async void callAPI(string method, string parameterString)
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
                        //richTextBox1.Text = FormatJson(responseBody);
                        Console.WriteLine("DCore blockchain returned:", responseBody);
                    }
                }
                catch
                {
                    //richTextBox1.AppendText("An unknown error occured");
                }
            }
        }



        #endregion DCore


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

    }
}
