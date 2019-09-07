using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace blockget
{
    /*
    public interface Ifs
    {
        void AddFileNameCID(string FilePath, string cid, int orderFile);
    }
    
        class Filesys : Ifs 
        */
    class Filesys {
//        Form1 form;
        IpfsBlkchn ipfsblock;
        myWatcher watcher;
        //FileSystemWatcher watcher;
        string pathfs = Globals.BLOCKGET_FILE_FOLDER + "\\filesystem.txt";
        const int BYTES_TO_READ_FILE_COMP = sizeof(Int64);
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
        public HashSet<List<string>> listFileNamesCids = new HashSet<List<string>>();
        private static readonly int CHUNK_SIZE = 1024;


        //public Filesys(Form1 f)
        public Filesys()
        {
//            form = f;
            watcher = new myWatcher(this);
            //initWatcher();
//            ipfsblock = new IpfsBlkchn(this, f );
            ipfsblock = new IpfsBlkchn(this);
            Console.WriteLine("New Object Filesys created");
            //Check if filesys file already exists
            if (fsExists())
            {
                Console.WriteLine("A copy of the file system has been found in the Blockget Folder and is put in memory");
                listFileNamesCids = getCurrentState();
            }
        }
        
        
        /*
        #region Watcher
        private void initWatcher()
        {
            watcher = new FileSystemWatcher();
            //Will have to recover this from installation params
            watcher.Path = Globals.BLOCKGET_FILE_FOLDER;
            // Watch for changes in LastAccess and LastWrite times, and
            // the renaming of files or directories.
            watcher.NotifyFilter = NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName;
            // Add event handlers.
            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }
        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
            //Filesys.UpdateEvent(e.FullPath);
            //Filesys fs = new Filesys();
            //fs.UpdateEvent(e.FullPath);
            //Form1.UpdateEvent(e.FullPath);
            UpdateEvent(e.FullPath);
        }
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
            //Filesys.UpdateEvent(e.FullPath);
            UpdateEvent(e.FullPath);
        }


        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
            DeleteEvent(e.FullPath);
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
            CreateEvent(e.FullPath);
        }

        #endregion Watcher
        */

        /// <summary>
        /// React to the watcher event - file deleted
        /// </summary>
        /// <param name="filepath"></param>
        public void DeleteEvent(string filepath) {
            Console.WriteLine("We will have to remove {0} from the File System", filepath);
            if (DeleteFileByPathBlockgetFS(filepath))
                Console.WriteLine("File successfully deleted");
            else
                Console.WriteLine("File couldn't be deleted from Blockget FileSystem");
        }
        /// <summary>
        /// React to the watcher event - file created
        /// </summary>
        /// <param name="filepath"></param>
        public void CreateEvent(string filepath) {
            Console.WriteLine("We will have to add {0} on the File System", filepath);
            EncryptSplitUploadFile(filepath);
        }

        /// <summary>
        /// React to the watcher event - file created
        /// </summary>
        /// <param name="filepath"></param>
        public void UpdateEvent(string filepath)
        {
            Console.WriteLine("We will have to update {0} on the File System", filepath);
            EncryptSplitUploadFile(filepath);
        }

        #region Storing Hash Table
        //https://stackoverflow.com/questions/1749044/c-sharp-object-binary-serialization
        private static TData DeserializeFromString<TData>(string settings)
        {
            byte[] b = Convert.FromBase64String(settings);
            using (var stream = new MemoryStream(b))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                return (TData)formatter.Deserialize(stream);
            }
        }

        private static string SerializeToString<TData>(TData settings)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, settings);
                stream.Flush();
                stream.Position = 0;
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        //Save as Text
        void storeFS0(HashSet<List<string>> BlockgetFS)
        {
            string toSave;
            byte[] myBytesToFile;
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter((Stream)memoryStream);
            streamWriter.Write(BlockgetFS);
            myBytesToFile = memoryStream.ToArray();
            toSave = Convert.ToBase64String(myBytesToFile);
            //File.WriteAllText(path, streamWriter.ReadToEnd());
            File.WriteAllText(pathfs, toSave);
        }

        HashSet<List<string>> getFS0()
        {
            //Request the FS from the cloud?
            //Saved on the Blockget Folder
            HashSet<List<string>> BlockgetFS = null;
            FileStream fs = new FileStream(pathfs, FileMode.Open, FileAccess.Read);
            //string[] lines = System.IO.File.ReadAllLines(pathfs);
            string text = System.IO.File.ReadAllText(pathfs);
            byte[] resultByte = Convert.FromBase64String(text);
            MemoryStream memoryStream = new MemoryStream(resultByte);
            StreamReader streamReader = new StreamReader((Stream)memoryStream);
            //https://docs.microsoft.com/fr-fr/dotnet/api/system.io.streamreader?view=netframework-4.8
            //TODO: BlockgetFS = streamReader.ReadLine() as HashSet<List<string>>;
            return BlockgetFS;
        }

        //Save as Byte
        void storeFS1(HashSet<List<string>> BlockgetFS)
        {
            byte[] myBytesToFile;
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter((Stream)memoryStream);
            streamWriter.Write(BlockgetFS);
            myBytesToFile = memoryStream.ToArray();
            File.WriteAllBytes(pathfs, myBytesToFile);
        }

        HashSet<List<string>> getFS1()
        {
            //Request the FS from the cloud?
            //Saved on the Blockget Folder
            HashSet<List<string>> BlockgetFS = null;
            FileStream fs = new FileStream(pathfs, FileMode.Open, FileAccess.Read);
            byte[] resultByte = System.IO.File.ReadAllBytes(pathfs);
            MemoryStream memoryStream = new MemoryStream(resultByte);
            StreamReader streamReader = new StreamReader((Stream)memoryStream);
            //TODO: BlockgetFS = streamReader.ReadToEnd() as HashSet<List<string>>;
            return BlockgetFS;
        }

        //https://stackoverflow.com/questions/1749044/c-sharp-object-binary-serialization
        //Binary Fotmatter and Serialise
        void storeFS2(HashSet<List<string>> BlockgetFS) {
            MemoryStream memorystream = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(memorystream, BlockgetFS);
            byte[] myBytesToFile = memorystream.ToArray();
            File.WriteAllBytes(pathfs,myBytesToFile);
        }

        HashSet<List<string>> getFS2()
        {
            //Request the FS from the cloud?
            //Saved on the Blockget Folder
            HashSet<List<string>> BlockgetFS;
            FileStream fs = new FileStream(pathfs, FileMode.Open, FileAccess.Read);
            byte[] resultByte = System.IO.File.ReadAllBytes(pathfs);
            MemoryStream memoryStream = new MemoryStream(resultByte);
            BinaryFormatter bfd = new BinaryFormatter();
            BlockgetFS = bfd.Deserialize(memoryStream) as HashSet<List<string>>;
            return BlockgetFS;
        }

        //https://www.tutorialspoint.com/csharp/csharp_binary_files.htm
        void storeFS3(HashSet<List<string>> BlockgetFS)
        {
            //int i = 25;
            //double d = 3.14157;
            //bool b = true;
            //string s = "I am happy";
            BinaryWriter bw;
            //create the file
            try
            {
                bw = new BinaryWriter(new FileStream(pathfs, FileMode.Create));
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message + "\n Cannot create file.");
                return;
            }

            //writing into the file
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(memoryStream, BlockgetFS);
                //bw.Write(BlockgetFS);
                byte[] myBytesToFile = memoryStream.ToArray();
                string toSave = Convert.ToBase64String(myBytesToFile);
                bw.Write(toSave);
                //bw.Write(i);
                //bw.Write(d);
                //bw.Write(b);
                //bw.Write(s);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message + "\n Cannot write to file.");
                return;
            }
            bw.Close();
        }


        HashSet<List<String>> getFS3() {
            BinaryReader br;
            byte[] myBytesFromFile = new byte [CHUNK_SIZE];
            HashSet<List<String>> BlockgetFS; 
            //reading from the file
            try
            {
                //br = new BinaryReader(new FileStream(path, FileMode.Open));
                FileStream fs = new FileStream(pathfs, FileMode.Open, FileAccess.Read);
                //BinaryReader 
                    br = new BinaryReader(fs, new ASCIIEncoding());
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message + "\n Cannot open file.");
                return null;
            }

            try
            {
                //https://docs.microsoft.com/en-us/dotnet/api/system.io.binaryreader.readbytes?view=netframework-4.8
                byte[] chunk;
                chunk = br.ReadBytes(CHUNK_SIZE);
                while (chunk.Length > 0)
                {
                    DumpBytes(chunk, chunk.Length);
                    chunk = br.ReadBytes(CHUNK_SIZE);
                }
                //TODO: WILL NEED TO MAKE SOMETHING SIZE DYNAMIC: https://stackoverflow.com/questions/415291/best-way-to-combine-two-or-more-byte-arrays-in-c-sharp
                System.Buffer.BlockCopy(chunk, 0, myBytesFromFile, 0, CHUNK_SIZE); 
                //How to do something like: memcopy(BlockgetFS, myBytesFromFile)
                MemoryStream memoryStream = new MemoryStream();
                BinaryFormatter bfd = new BinaryFormatter();
                BlockgetFS = bfd.Deserialize(memoryStream) as HashSet<List<string>>;
                //i = br.ReadInt32();
                //Console.WriteLine("Integer data: {0}", i);
                //d = br.ReadDouble();
                //Console.WriteLine("Double data: {0}", d);
                //b = br.ReadBoolean();
                //Console.WriteLine("Boolean data: {0}", b);
                //s = br.ReadString();
                //Console.WriteLine("String data: {0}", s);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message + "\n Cannot read from file.");
                return null;
            }
            br.Close();
            Console.ReadKey();
            return BlockgetFS;
        }

        public static void DumpBytes(byte[] bdata, int len)
        {
            int i;
            int j = 0;
            char dchar;
            // 3 * 16 chars for hex display, 16 chars for text and 8 chars
            // for the 'gutter' int the middle.
            StringBuilder dumptext = new StringBuilder("        ", 16 * 4 + 8);
            for (i = 0; i < len; i++)
            {
                dumptext.Insert(j * 3, String.Format("{0:X2} ", (int)bdata[i]));
                dchar = (char)bdata[i];
                //' replace 'non-printable' chars with a '.'.
                if (Char.IsWhiteSpace(dchar) || Char.IsControl(dchar))
                {
                    dchar = '.';
                }
                dumptext.Append(dchar);
                j++;
                if (j == 16)
                {
                    Console.WriteLine(dumptext);
                    dumptext.Length = 0;
                    dumptext.Append("        ");
                    j = 0;
                }
            }
            // display the remaining line
            if (j > 0)
            {
                for (i = j; i < 16; i++)
                {
                    dumptext.Insert(j * 3, "   ");
                }
                Console.WriteLine(dumptext);
            }
        }
        #endregion Storing Hash Table

        public void testFileSys() {
            storeFS2(listFileNamesCids);
            getFS2();
        }

        public void SaveCurrentState() {
            storeFS2(listFileNamesCids);
        }

        public HashSet<List<string>> getCurrentState() {
            return getFS2();
        }

        Boolean fsExists() {
            return File.Exists(pathfs);
        }

        //Check if the element of filename - CID List correspond to the filepath
        bool isRightFile(List<string> FileNameCid, string filepath) {
            //return FileNameCid[1].Equals(filepath);
            return String.Compare(FileNameCid.ElementAt(0), filepath, true) == 0;
        }

        public void RecoverFile(string path) {
            List<string> entry = SearchGetEntryByPath(path);
            if (entry != null)
            {
                if (entry.Count > 0)
                {
                    Console.WriteLine("The file " + path + " has been found in Blockget collection, we recover the file from IPFS");
//                    form.richTextBox1.Text = "The file " + path + " has been found in Blockget collection, we recover the file from IPFS";
                    ipfsblock.RecoverFilesMergeDecrypt(entry);
                    //RecoverTextMergeDecrypt(entry);
                }
                else Console.WriteLine("This file hasn't been found in Blockget HashSet");
            }
            else Console.WriteLine("The file cannot be recovered from Blockget, maybe the system just initialised");
        }

        /// <summary>
        /// Delete File By File Path in the BlockGet File System.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns>Boolean if found or not</returns>
        public bool DeleteFileByPathBlockgetFS(string RequiredFile)
        {
            bool result = false;
            lock (LockAllFileList)
            {
                //
                //int index = listFileNamesCids.FindIndex(listFileNamesCids => listFileNamesCids[index][1] = RequiredFile);
                //if (index >= 0)
                //    listFileNamesCids.RemoveAt(index);
                //int index = listFileNamesCids.FindIndex(isRightFile);
                //https://stackoverflow.com/questions/853526/using-linq-to-remove-elements-from-a-listt
                //listFileNamesCids.RemoveWhere(x => listFileNamesCids[x][1] == RequiredFile);
                //listFileNamesCids.Remove(x => listFileNamesCids[x][1] == RequiredFile);
                //listFileNamesCids.RemoveWhere(isRightFile);
                //listFileNamesCids.Remove(isRightFile);
                //listFileNamesCids[1][1] // There are no indexes HashSet => ToList
                //listFileNamesCids.RemoveWhere(listFileNamesCids.Contains(RequiredFile));
                //listFileNamesCids = listFileNamesCids.Where(x => listFileNamesCids[x][1] == RequiredFile).ToList();
                
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
                                Console.WriteLine("Here: " + filepath);
                                //remove file from Blockget
                                //lEnumerator.Current.Remove();
                                //lEnumerator.Current.Remove(1);
                                listFileNamesCids.Remove(lEnumerator.Current);
                                Console.WriteLine("{0} removed from Blockget (Memory)");
                                SaveCurrentState();
                                Console.WriteLine("{0} removed from Blockget (HD)");
                                //ipfs block rm
                                for (int i = 0; i < Globals.SPLIT_NB; i++)
                                    ipfsblock.Remove_File(lEnumerator.Current[i]);
                                Console.WriteLine("{0} removed from IPFS");
                                //remove file from the hard drive
                                if (File.Exists(RequiredFile)) File.Delete(RequiredFile);
                                Console.WriteLine("{0} removed from the hard drive");
                            }
                            else Console.WriteLine("Couldn't get the filepath of the file for thumbnail update");

                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Search File By Name in the BlockGet File System.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>string with the list of filepath found. It can be displayed by a RichTextBox</returns>
        //For the HashSet<List<string>>:
        //bool SearchFileByNameBlockgetFS(string name) {
        public string SearchFileByNameBlockgetFS(string name)
        {
            //bool result = false;
            string result = "File : " + name + " found in: \n";
            int counter = 1;
            lock (LockAllFileList)
            {
                foreach (List<string> l in listFileNamesCids)
                {
                    string filePath = GetPathFileEntry(l);
                    if (filePath.Contains(name))
                    {
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
        public bool SearchFileByPathBlockgetFS(string RequiredFile)
        {
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
                            //Check Current
                            string filepath = GetPathFileEntry(lEnumerator.Current);
                            if (!String.IsNullOrEmpty(filepath))
                            {
                                //Form1.setImage(filepath);
                                Console.WriteLine("Here: " + filepath);
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
        public List<string> SearchGetEntryByPath(string file)
        {
            List<string> result = null;
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
        public string GetPathFileEntry(List<string> FileEntry)
        {
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
        public bool AddEntryListFileNamesCids(List<string> entry)
        {
            //Test first if the filepath is present has been done before choosing add entry or update entry
            bool result = false;
            lock (LockAllFileList)
            {
                result = listFileNamesCids.Add(entry);
                if (result)
                    Console.WriteLine("The entry has been added to the file system");
                else
                {
                    Console.WriteLine("The entry exist already in the file system");
                }
            }
            return result;
        }
        /// <summary>
        /// Remove an entry from the blockget file system
        /// </summary>
        /// <param name="entry"></param>
        public void RemoveEntryListFileNameCids(List<string> entry)
        {
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
        public void UpdateEntryListFileNamesCids(string filepath, List<string> NewEntry)
        {
            List<string> previousEntry = SearchGetEntryByPath(filepath);
            if (previousEntry.Count != 0)
            {
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
        public List<string> GetFileNameCID()
        {
            lock (LockCIDArray)
            {
                for (int i = 0; i < FileNameCID.Count; i++)
                {
                    //MessageBox.Show(FileNameCID[i]);
                    Console.WriteLine(FileNameCID[i]);
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
        public void AddFileNameCID(string filename, string cid1, string cid2, string cid3)
        {
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
        public void AddFileNameCID(string FilePath, string cid, int orderFile)
        {
            lock (LockCIDArray)
            {
                if (orderFile <= FileNameCID.Count)
                {
                    FileNameCID.Add(cid);
                    //FileNameCID[0] = FilePath; 
                    //FileNameCID[orderFile] = cid;
                    Console.WriteLine("We add {0} to {1} .", cid, FilePath);
                }
                else Console.WriteLine("Size of FileNameCID is {0}", FileNameCID.Count);
            }
        }

        /// <summary>
        /// Add current FileNameCID Array to the HashSet - //S1 - Global variable
        /// </summary>
        /// <param name="filepath"></param>
        public void AddCurrentArrayListFileCids()
        {
            lock (LockCIDArray)
            {
                //First check if entry already exist?
                string filepath = GetPathFileEntry(GetFileNameCID()); //NOT GOING TO WORK BECAUSE GetFileNameCID() want LockCIDArray
                bool fileExist = SearchFileByPathBlockgetFS(filepath);
                if (fileExist)
                {
                    Console.WriteLine("The file already exist, we update {0} in Blockget", filepath);
                    UpdateEntryListFileNamesCids(filepath, GetFileNameCID());
                }
                else
                {
                    AddEntryListFileNamesCids(FileNameCID);
                    Console.WriteLine("Adding current Entry to HashTable");
                }
            }
        }

        #region Process

        /// <summary>
        /// Encrypt, Split, Upload the file on IPFS and Timestamp on DCore
        /// </summary>
        /// <param name="file">filepath</param>
        //string[] 
        public void EncryptSplitUploadFile(string file)
        {
            try
            {
                //We encrypt
                string encryptedFile = Encryption.EncryptFile(file);

                //We split 
                string[] splittedFile = Split.SplitFile(encryptedFile);

                //Update or Create new Entry?
                bool FileExist = SearchFileByPathBlockgetFS(file);

                List<string> EntryFileCid = new List<string> { file, "", "", "" };
                // We can send
                for (int i = 0; i <= Globals.SPLIT_NB - 1; i++)
                {
                    Console.WriteLine("Adding {0} at position {1} for {2} entry", splittedFile[i], i + 1, file);
                    ipfsblock.IpfsUploadDCore(splittedFile[i], i + 1, EntryFileCid);
                }

                //Add or Update? => Check if it exists or not
                if (FileExist)
                {
                    Console.WriteLine("The file {0} already exists in Blockget, we update", file);
 //                   form.richTextBox1.Text = "The file " + file + " already exist in Blockget, we update";
                    UpdateEntryListFileNamesCids(file, EntryFileCid);
                }
                else
                {
                    Console.WriteLine("Creating a new file in Blockget File System");
 //                   form.richTextBox1.Text = "Creating a new file in Blockget File System";
                    AddEntryListFileNamesCids(EntryFileCid);
                }
                //We delete the temp files:
                //foreach (string tempFile in splittedFile) {
                //    File.Delete(tempFile);
                //}
                File.Delete(encryptedFile);
            }
            catch (Exception e)
            { //System.UnauthorizedAccessException from Encryption Input FileStream
                Console.WriteLine(e.Message);
                //Cross-thread operation not valid: Control 'richTextBox1' accessed from a thread other than the thread it was created on.'
//                form.richTextBox1.Text = "Couldn't encrypt and upload the file into Blockget IPFS"; 
            }
        }


        public void EncryptSplitUploadFileSeparateFolder(string file)
        {
            try
            {
                //We encrypt
                string encryptedFile = Encryption.EncryptFile(file);

                //We split 
                string[] splittedFile = Split.SplitFileSeparateFolder(encryptedFile);

                //Update or Create new Entry?
                bool FileExist = SearchFileByPathBlockgetFS(file);

                List<string> EntryFileCid = new List<string> { file, "", "", "" };
                // We can send
                for (int i = 0; i <= Globals.SPLIT_NB - 1; i++)
                {
                    Console.WriteLine("Adding {0} at position {1} for {2} entry", splittedFile[i], i + 1, file);
                    ipfsblock.IpfsUploadDCore(splittedFile[i], i + 1, EntryFileCid);
                }

                //Add or Update? => Check if it exist or not
                if (FileExist)
                {
                    Console.WriteLine("The file {0} already exist in Blockget, we update", file);
                    //                   form.richTextBox1.Text = "The file " + file + " already exist in Blockget, we update";
                    UpdateEntryListFileNamesCids(file, EntryFileCid);
                }
                else
                {
                    Console.WriteLine("Creating a new file in Blockget File System");
                    //                   form.richTextBox1.Text = "Creating a new file in Blockget File System";
                    AddEntryListFileNamesCids(EntryFileCid);
                }
                //We delete the temp files:
                //foreach (string tempFile in splittedFile) {
                //    File.Delete(tempFile);
                //}
            }
            catch (Exception e)
            { //System.UnauthorizedAccessException from Encryption Input FileStream
                Console.WriteLine(e.Message);
                //Cross-thread operation not valid: Control 'richTextBox1' accessed from a thread other than the thread it was created on.'
                //                form.richTextBox1.Text = "Couldn't encrypt and upload the file into Blockget IPFS"; 
            }
        }
        #endregion

        #region file methods
        /// <summary>
        /// Compare byte block by block if the 2 files are the same
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns>Boolean</returns>
        public static bool compare2FilesByBlocks(string file1, string file2)
        {
            FileInfo first = new FileInfo(file1);
            FileInfo second = new FileInfo(file2);

            if (first.Length != second.Length)
                return false;

            if (string.Equals(first.FullName, second.FullName, StringComparison.OrdinalIgnoreCase))
                return true;

            int iterations = (int)Math.Ceiling((double)first.Length / BYTES_TO_READ_FILE_COMP);

            using (FileStream fs1 = first.OpenRead())
            using (FileStream fs2 = second.OpenRead())
            {
                byte[] one = new byte[BYTES_TO_READ_FILE_COMP];
                byte[] two = new byte[BYTES_TO_READ_FILE_COMP];

                for (int i = 0; i < iterations; i++)
                {
                    fs1.Read(one, 0, BYTES_TO_READ_FILE_COMP);
                    fs2.Read(two, 0, BYTES_TO_READ_FILE_COMP);

                    if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                        return false;
                }
            }
            return true;
        }

        #endregion file method
    }
}
