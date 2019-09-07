using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace blockget
{
    class Split
    {
        //https://www.c-sharpcorner.com/uploadfile/a72401/split-and-merge-files-in-C-Sharp/
        //Merge file is stored in drive
        static public string[] SplitFile(string SourceFile)
        {
            string[] Split = new string[Globals.SPLIT_NB];
            try
            {
                FileStream fs = new FileStream(SourceFile, FileMode.Open, FileAccess.Read);
                int SizeofEachFile = (int)Math.Ceiling((double)fs.Length / Globals.SPLIT_NB);

                //string baseFileName = Path.GetFileNameWithoutExtension(SourceFile);
                //string Extension = Path.GetExtension(SourceFile);

                //To save the *tmp in a new folder:
                //String pathString = Path.GetDirectoryName(SourceFile) + "\\" + baseFileName + "\\";
                //String pathString = Path.GetDirectoryName(SourceFile) + "\\" + SourceFile;
                //pathString = pathString.Replace('.', '-');
                //Console.WriteLine(pathString);
                //System.IO.Directory.CreateDirectory(pathString);

                for (int i = 0; i < Globals.SPLIT_NB; i++)
                {
                    //Split[i] = pathString + baseFileName + Extension + "." +
                    //i.ToString().PadLeft(5, Convert.ToChar("0")) + ".tmp";
                    //This save the *tmp where the initial file is stored
                    //Split[i] = Path.GetDirectoryName(SourceFile) + "\\" + baseFileName + Extension + "." +
                    //i.ToString().PadLeft(5, Convert.ToChar("0")) + ".tmp";
                    Split[i] = SourceFile +"." + i.ToString().PadLeft(5, Convert.ToChar("0")) + ".tmp";
                    //Split[i] = Path.GetDirectoryName(SourceFile) + "\\" + baseFileName + "." +
                    //i.ToString().PadLeft(5, Convert.ToChar("0")) + Extension + ".tmp";
                    FileStream outputFile = new FileStream(Split[i], FileMode.Create, FileAccess.Write);

                    //FileStream outputFile = new FileStream(Path.GetDirectoryName(SourceFile) + "\\" + baseFileName + "." +
                    //  i.ToString().PadLeft(5, Convert.ToChar("0")) + Extension + ".tmp", FileMode.Create, FileAccess.Write);

                    int bytesRead = 0;
                    byte[] buffer = new byte[SizeofEachFile];

                    if ((bytesRead = fs.Read(buffer, 0, SizeofEachFile)) > 0)
                    {
                        outputFile.Write(buffer, 0, bytesRead);
                    }
                    outputFile.Close();
                }
                fs.Close();
            }
            catch (Exception Ex)
            {
                throw new ArgumentException(Ex.Message);
            }

            return Split;
        }
        //static public string MergeFile(string [] Split)
        static public string MergeFile(string inputfoldername)
        {
            string OutputPath = "";
            try
            {
                Console.WriteLine("Looking for *tmp files in the folder: {0}", inputfoldername);
                string[] tmpfiles = Directory.GetFiles(inputfoldername, "*.tmp");

                FileStream outPutFile = null;
                string PrevFileName = "";

                foreach (string tempFile in tmpfiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(tempFile);
                    //string baseFileName = fileName.Substring(0, fileName.IndexOf(Convert.ToChar(".")));
                    string baseFileName = fileName.Substring(0, fileName.LastIndexOf(Convert.ToChar(".")));
                    string extension = Path.GetExtension(baseFileName);

                    if (!PrevFileName.Equals(baseFileName))
                    {
                        if (outPutFile != null)
                        {
                            outPutFile.Flush();
                            outPutFile.Close();
                        }
                        OutputPath = inputfoldername + baseFileName; //For test only
                        //If the file already exists: TOUPDATE
                        //int i = 0;
                        //while (File.Exists(Output)) {
                        //    //Output = inputfoldername + baseFileName + i.ToString();
                        //    Output = inputfoldername + baseFileName + i.ToString() + extension; // when merge the extension is .00000
                        //    i++;
                        //}
                        //Output = Globals.BLOCKGET_FILE_FOLDER + baseFileName + extension; //Will save in Blockget folder
                        //Output = inputfoldername + baseFileName;
                        outPutFile = new FileStream(OutputPath, FileMode.OpenOrCreate, FileAccess.Write);
                        Console.WriteLine("Output Merged file = {0}", OutputPath);
                    }

                    int bytesRead = 0;
                    byte[] buffer = new byte[1024];
                    FileStream inputTempFile = new FileStream(tempFile, FileMode.OpenOrCreate, FileAccess.Read);

                    while ((bytesRead = inputTempFile.Read(buffer, 0, 1024)) > 0)
                        outPutFile.Write(buffer, 0, bytesRead);

                    inputTempFile.Close();
                    File.Delete(tempFile);
                    PrevFileName = baseFileName;

                }

                outPutFile.Close();
                //Form1.statusFromTextBox = "Files have been merged and saved at location C:\\";
                //Form1.richTextBox1.Text = "Files have been merged and saved at location C:\\";
                //Application.OpenForms.GetEnumerator.Get(1).statusFromTextBox = "Files have been merged and saved at location C:\\";
                //statusFromTextBox = Output;
                Console.WriteLine("Files have been merged and saved at location : {0}", OutputPath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in merge file: {0}", e.Message);
            }
            return OutputPath;
        }

        static public string[] SplitFileSeparateFolder(string SourceFile)
        {
            string [] Split = new string[Globals.SPLIT_NB];
            try
            {
                FileStream fs = new FileStream(SourceFile, FileMode.Open, FileAccess.Read);
                int SizeofEachFile = (int)Math.Ceiling((double)fs.Length / Globals.SPLIT_NB);

                string baseFileName = Path.GetFileNameWithoutExtension(SourceFile);
                string Extension = Path.GetExtension(SourceFile);

                //To save the *tmp in a new folder:
                //String pathString = Path.GetDirectoryName(SourceFile)+ "\\"+ SourceFile + "\\";
                String pathString = Path.GetDirectoryName(SourceFile) + "\\" + baseFileName + "\\";
                //String pathString = Path.GetDirectoryName(SourceFile) + "\\" + SourceFile;
                pathString = pathString.Replace('.', '-');
                Console.WriteLine(pathString);
                System.IO.Directory.CreateDirectory(pathString);

                for (int i = 0; i < Globals.SPLIT_NB; i++)
                {
                    Split[i] = pathString + baseFileName + Extension + "." +
                    i.ToString().PadLeft(5, Convert.ToChar("0")) + ".tmp";
                    //This save the *tmp where the initial file is stored
                    //Split[i] = Path.GetDirectoryName(SourceFile) + "\\" + baseFileName + "." +
                    //i.ToString().PadLeft(5, Convert.ToChar("0")) + Extension + ".tmp";

                    FileStream outputFile = new FileStream(Split[i], FileMode.Create, FileAccess.Write);

                    //FileStream outputFile = new FileStream(Path.GetDirectoryName(SourceFile) + "\\" + baseFileName + "." +
                    //  i.ToString().PadLeft(5, Convert.ToChar("0")) + Extension + ".tmp", FileMode.Create, FileAccess.Write);

                    int bytesRead = 0;
                    byte[] buffer = new byte[SizeofEachFile];

                    if ((bytesRead = fs.Read(buffer, 0, SizeofEachFile)) > 0)
                    {
                        outputFile.Write(buffer, 0, bytesRead);
                        //outp.Write(buffer, 0, BytesRead);

                        //string packet = baseFileName + "." + i.ToString().PadLeft(3, Convert.ToChar("0")) + Extension.ToString();
                        //Console.WriteLine("Split packet = {0}", packet);
                    }
                    outputFile.Close();
                }
                fs.Close();
            }
            catch (Exception Ex)
            {
                throw new ArgumentException(Ex.Message);
            }

            return Split;
        }
        //static public string MergeFile(string [] Split)
        static public string MergeFileSeparateFolder(string inputfoldername)
        {
            string Output="";
            try
            {
                Console.WriteLine("Looking for *tmp files in the folder: {0}",inputfoldername);
                string[] tmpfiles = Directory.GetFiles(inputfoldername, "*.tmp");

                FileStream outPutFile = null;
                string PrevFileName = "";

                foreach (string tempFile in tmpfiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(tempFile);
                    //string baseFileName = fileName.Substring(0, fileName.IndexOf(Convert.ToChar(".")));
                    string baseFileName = fileName.Substring(0, fileName.LastIndexOf(Convert.ToChar(".")));
                    string extension = Path.GetExtension(baseFileName);

                    if (!PrevFileName.Equals(baseFileName))
                    {
                        if (outPutFile != null)
                        {
                            outPutFile.Flush();
                            outPutFile.Close();
                        }
                        Output = inputfoldername + baseFileName ; //For test only
                        //If the file already exists: TOUPDATE
                        //int i = 0;
                        //while (File.Exists(Output)) {
                        //    //Output = inputfoldername + baseFileName + i.ToString();
                        //    Output = inputfoldername + baseFileName + i.ToString() + extension; // when merge the extension is .00000
                        //    i++;
                        //}
                        //Output = Globals.BLOCKGET_FILE_FOLDER + baseFileName + extension; //Will save in Blockget folder
                        //Output = inputfoldername + baseFileName;
                        outPutFile = new FileStream(Output, FileMode.OpenOrCreate, FileAccess.Write);                      
                        Console.WriteLine("Output Merged file = {0}", Output);
                    }

                    int bytesRead = 0;
                    byte[] buffer = new byte[1024];
                    FileStream inputTempFile = new FileStream(tempFile, FileMode.OpenOrCreate, FileAccess.Read);

                    while ((bytesRead = inputTempFile.Read(buffer, 0, 1024)) > 0)
                        outPutFile.Write(buffer, 0, bytesRead);

                    inputTempFile.Close();
                    File.Delete(tempFile); 
                    PrevFileName = baseFileName;

                }

                outPutFile.Close();
                //Form1.statusFromTextBox = "Files have been merged and saved at location C:\\";
                //Form1.richTextBox1.Text = "Files have been merged and saved at location C:\\";
                //Application.OpenForms.GetEnumerator.Get(1).statusFromTextBox = "Files have been merged and saved at location C:\\";
                //statusFromTextBox = Output;
                Console.WriteLine("Files have been merged and saved at location : {0}", Output);
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception in merge file: {0}",e.Message);
            }
            return Output;
        }

        //Split test
        public static void testSplitMergeFile()
        {
            //Arrange
            SplitFileSeparateFolder("C:\\Blockget\\test.txt");
            MergeFileSeparateFolder("C:\\Blockget\\test\\");
            //Act
            //Assert
        }
    }
}
