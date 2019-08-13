using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace blockget
{
    class Encryption
    {
        const int BYTES_TO_READ_FILE_COMP = sizeof(Int64);

        public static string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static string EncryptTextFile(string pathFile)
        {
            byte[] iv = new byte[16];
            byte[] array;
            string plainText;
            using (var streamReader = new StreamReader(pathFile, Encoding.UTF8))
            {
                plainText = streamReader.ReadToEnd();
            }
            //byte[] buffer = Convert.FromBase64String(plainText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Globals.KEY);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }
            string PathResult = pathFile + ".aes"; 
            //System.IO.File.WriteAllText(PathResult, Convert.ToBase64String(array));
            System.IO.File.WriteAllText(PathResult, System.Text.Encoding.UTF8.GetString(array, 0, array.Length));
            return PathResult;
        }

        public static string DecryptTextFile(string pathFile)
        {
            byte[] iv = new byte[16];
            string cipherText;
            using (var streamReader = new StreamReader(pathFile, Encoding.UTF8))
            {
                cipherText = streamReader.ReadToEnd();
            }
            //byte[] buffer = Convert.FromBase64String(cipherText);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(cipherText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Globals.KEY);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            string PathResult = pathFile.Remove(pathFile.Length - 4, 4) ;
                            System.IO.File.WriteAllText(PathResult, streamReader.ReadToEnd());
                            return PathResult;
                        }
                    }
                }
            }
        }

        public static string EncryptFile(string path) {
            // For additional security Pin the password of your files
            GCHandle gch = GCHandle.Alloc(Globals.ENC_PWD, GCHandleType.Pinned);
            // Encrypt the file
            string newFilePath = FileEncrypt(@path, Globals.ENC_PWD);
            // To increase the security of the encryption, delete the given password from the memory !
            ZeroMemory(gch.AddrOfPinnedObject(), Globals.ENC_PWD.Length * 2);
            gch.Free();
            // You can verify it by displaying its value later on the console (the password won't appear)
            //Console.WriteLine("The given password is surely nothing: " + password);
            return newFilePath;
        }

        public static string DecryptFile(string path) {
            //We remove .aes:
            string newFilePath = path.Remove(path.Length - 4, 4); 
            //We change if the file already exist in the folder:
            string fileName = Path.GetFileNameWithoutExtension(newFilePath);
            string extension = Path.GetExtension(newFilePath);
            newFilePath = Path.GetDirectoryName(path) + "\\" + fileName + "-decrypted" + extension;
            // For additional security Pin the password of your files
            GCHandle gch = GCHandle.Alloc(Globals.ENC_PWD, GCHandleType.Pinned);
            // Decrypt the file
            FileDecrypt(@path, @newFilePath, Globals.ENC_PWD);
            // To increase the security of the decryption, delete the used password from the memory !
            ZeroMemory(gch.AddrOfPinnedObject(), Globals.ENC_PWD.Length * 2);
            gch.Free();
            // We can verify it by displaying its value later on the console (the password won't appear)
            //Console.WriteLine("The given password is surely nothing: " + password);
            //We can delete the aes file now
            File.Delete(path);
            return newFilePath;
        }

        //https://ourcodeworld.com/articles/read/471/how-to-encrypt-and-decrypt-files-using-the-aes-encryption-algorithm-in-c-sharp
        //  Call this function to remove the key from memory after use for security
        [DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
        public static extern bool ZeroMemory(IntPtr Destination, int Length);

        /// <summary>
        /// Creates a random salt that will be used to encrypt your file. This method is required on FileEncrypt.
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateRandomSalt()
        {
            byte[] data = new byte[32];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < 10; i++)
                {
                    // Fille the buffer with the generated data
                    rng.GetBytes(data);
                }
            }

            return data;
        }

        /// <summary>
        /// Encrypts a file from its path and a plain password.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="password"></param>
        static private string FileEncrypt(string inputFile, string password)
        {
            //http://stackoverflow.com/questions/27645527/aes-encryption-on-large-files

            //generate random salt
            byte[] salt = GenerateRandomSalt();

            //create output file name
            FileStream fsCrypt = new FileStream(inputFile + ".aes", FileMode.Create);
            string newFilePath = inputFile + ".aes";

            //convert password string to byte arrray
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            //Set Rijndael symmetric encryption algorithm
            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Padding = PaddingMode.PKCS7;

            //http://stackoverflow.com/questions/2659214/why-do-i-need-to-use-the-rfc2898derivebytes-class-in-net-instead-of-directly
            //"What it does is repeatedly hash the user password along with the salt." High iteration counts.
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);

            //Cipher modes: http://security.stackexchange.com/questions/52665/which-is-the-best-cipher-mode-and-padding-mode-for-aes-encryption
            AES.Mode = CipherMode.CFB;

            // write salt to the begining of the output file, so in this case can be random every time
            fsCrypt.Write(salt, 0, salt.Length);

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);

            FileStream fsIn = new FileStream(inputFile, FileMode.Open);

            //create a buffer (1mb) so only this amount will allocate in the memory and not the whole file
            byte[] buffer = new byte[1048576];
            int read;

            try
            {
                while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Application.DoEvents(); // -> for responsive GUI, using Task will be better!
                    cs.Write(buffer, 0, read);
                }

                // Close up
                fsIn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                cs.Close();
                fsCrypt.Close();
            }
            return newFilePath; 
        }

        /// <summary>
        /// Decrypts an encrypted file with the FileEncrypt method through its path and the plain password.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        /// <param name="password"></param>
        static private void FileDecrypt(string inputFile, string outputFile, string password)
        {
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] salt = new byte[32];

            FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);
            fsCrypt.Read(salt, 0, salt.Length);

            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.PKCS7;
            AES.Mode = CipherMode.CFB;

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);

            FileStream fsOut = new FileStream(outputFile, FileMode.Create);

            int read;
            byte[] buffer = new byte[1048576];
            //byte[] buffer = new byte[1024];
            //byte[] buffer = new byte[48];
            try
            {
                while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Application.DoEvents();
                    fsOut.Write(buffer, 0, read);
                }
            }
            catch (CryptographicException ex_CryptographicException)
            {
                Console.WriteLine("CryptographicException error: " + ex_CryptographicException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            try
            {
                cs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error by closing CryptoStream: " + ex.Message);
            }
            finally
            {
                fsOut.Close();
                fsCrypt.Close();
            }
        }

        

        static bool compare2FilesByBlocks(string file1 , string file2)
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

        //See Unit Test
        public static void testEncryptionString()
        {
            //Arrange
            string pathFile = "C:\\Blockget\\test.txt";
            EncryptFile(pathFile);

            //Act
            //We check decryption : 
            string decrypted = DecryptFile(pathFile+".aes");

            //Assert
            if (compare2FilesByBlocks(pathFile,decrypted)) Console.WriteLine("The 2 files are the same");
            else Console.WriteLine("Oh oh the file hasn't been decrypted correctly.");

        }
        public void TestMethod1()
        {
            //Arrange
            var key = "b14ca5898a4e4133bbce2ea2315a1916";

            //Console.WriteLine("Please enter a secret key for the symmetric algorithm.");  
            //var key = Console.ReadLine();  

            Console.WriteLine("Please enter a string for encryption");
            var str = Console.ReadLine();
            var encryptedString = Encryption.EncryptString(key, str);
            Console.WriteLine($"encrypted string = {encryptedString}");

            //Act
            //We check decryption
            var decryptedString = Encryption.DecryptString(key, encryptedString);
            Console.WriteLine($"decrypted string = {decryptedString}");

            //Assert
            Console.ReadKey();
            //Assert.AreNotEqual(encryptedString, decryptedString);
        }
    }
}
