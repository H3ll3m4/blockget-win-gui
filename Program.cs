using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using System.Windows.Forms;

namespace blockget
{
    public static class Globals
    {
        public const Int32 SPLIT_NB = 3;//Convert.ToInt32(3); // Unmodifiable
        public static String BLOCKGET_FILE_FOLDER = "C:\\Blockget\\"; // Unmodifiable
        public const String ENC_PWD = "ThePasswordToDecryptAndEncryptTheFile";
        public const String KEY = "f5a2916401ff69a95a0b8c7d575586e4";
    }


    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //int SplitNb = Convert.ToInt32(5)
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            //tests:
            //ClientIpfs.testIpfsDownload(); OK
            //testSplitMergeEncryptedText(); OK
            //testSplitMergeEncryptedFile(); OK
            //Encryption.testEncryptionString(); OK
            //Split.testSplitMergeFile(); OK
            Application.Run(new Form1());
            //Background: FileSystemWatcher: https://docs.microsoft.com/en-us/dotnet/api/system.io.filesystemwatcher?view=netframework-4.8
            Watcher folderWatcher = new Watcher();
            Watcher.Run();
        }

        //Split and Merge Encrypted File
        public static void testSplitMergeEncryptedFile()
        {
            string encryptedFile = Encryption.EncryptFile("C:\\Blockget\\test.txt");
            Console.WriteLine("TEST: Encrypted file: {0}", encryptedFile);
            string[] splittedFiles = Split.SplitFile("C:\\Blockget\\test.txt.aes");
            foreach (string s in splittedFiles)
            {
                Console.WriteLine("TEST: Splitted Files paths are {0}", s);
            }
            string outputMerge = Split.MergeFile("C:\\Blockget\\test-txt\\");
            Console.WriteLine("TEST: Output of merged file is {0}", outputMerge);
            string resultDecrypt = Encryption.DecryptFile("C:\\Blockget\\test-txt\\test.txt.aes");
            Console.WriteLine("TEST: Decryped file is {0}", resultDecrypt);
        }

        //Split and Merge Encrypted Text
        public static void testSplitMergeEncryptedText()
        {
            string encryptedFile = Encryption.EncryptTextFile("C:\\Blockget\\test.txt");
            Console.WriteLine("TEST: Encrypted file: {0}", encryptedFile);
            string[] splittedFiles = Split.SplitFile("C:\\Blockget\\test.txt.aes");
            foreach (string s in splittedFiles)
            {
                Console.WriteLine("TEST: Splitted Files paths are {0}", s);
            }
            string outputMerge = Split.MergeFile("C:\\Blockget\\test-txt\\");
            Console.WriteLine("TEST: Output of merged file is {0}", outputMerge);
            string resultDecrypt = Encryption.DecryptTextFile("C:\\Blockget\\test-txt\\test.txt.aes");
            Console.WriteLine("TEST: Decryped file is {0}", resultDecrypt);
        }
    }
}
