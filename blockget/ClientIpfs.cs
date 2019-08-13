using Ipfs.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace blockget
{
    class ClientIpfs
    {
        static Encoding e = Encoding.UTF8;
        static string testJPG = "QmbwaRc6qptDqjVHHSwZ6oqTHX1WpdMYhRHwFddHr24QAJ";
        static string testText = "Qma1PbECNH6DsoTps9pmtqTAZQ3s17jzQEgEVQ97X5BDHN";

        static async private void Dl_String(string cid)
        {
            var ipfs = new IpfsClient();
            string text = await ipfs.FileSystem.ReadAllTextAsync(cid);
            Console.WriteLine("Received on IPFS : {0}", text);
            System.IO.File.WriteAllText(@"C:\Blockget\tesfile.txt", text, e);
            Console.WriteLine("String written on a textfile");
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
            var ipfs = new IpfsClient("http://localhost:5001");
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
    }
}
