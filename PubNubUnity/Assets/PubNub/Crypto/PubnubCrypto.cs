using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace PubNubAPI
{
    #region "PubnubCrypto"
    public class PubnubCrypto: PubnubCryptoBase
    {
        private static int blockSize = 16;
        private static int bufferSize = 16;
        PNLoggingMethod pnLog {get; set;}
        private bool UseRandomInitializationVector = false;
        public PubnubCrypto (string cipher_key, PNLoggingMethod pnLog)
            : base (cipher_key)
        {
            this.pnLog = pnLog;
        }

        public PubnubCrypto (string cipher_key, PNLoggingMethod pnLog, bool useRandomInitializationVector)
            : base (cipher_key)
        {
            this.pnLog = pnLog;
            UseRandomInitializationVector = useRandomInitializationVector;
        }

        public bool EncryptFile(byte[] iv, string filePath, string saveFilePath){
            return EncryptOrDecryptFile(true, iv, filePath, saveFilePath);
        }

        public bool DecryptFile(string tempFilePath, string saveFilePath){
            return EncryptOrDecryptFile(false, null, tempFilePath, saveFilePath);
        }

        public bool EncryptOrDecryptFile(bool encrypt, byte[] iv, string filePath, string saveFilePath){
            byte[] key  = System.Text.Encoding.ASCII.GetBytes (GetEncryptionKey ());
            FileStream fin = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            FileStream fout = new FileStream(saveFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            byte[] bin = new byte[100]; //This is intermediate storage for the encryption.
            long rdlen = 0;              //This is the total number of bytes written.
            long totlen = fin.Length;    //This is the total length of the input file.
            int len;                     //This is the number of bytes to be written at a time.

            Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            CryptoStream cryptoStream;
 
            if(encrypt){
                if(iv == null){
                    aes.GenerateIV();
                    iv = aes.IV;
                }  
                // Prefix IV not working here as CryptoStream checks the padding of the whole file.
                cryptoStream = new CryptoStream(fout, aes.CreateEncryptor(key, iv), CryptoStreamMode.Write);
            } else {
                //Extract IV
                iv = new byte[16];
                len = fin.Read(iv, 0, 16);
                rdlen = rdlen + len;
                cryptoStream = new CryptoStream(fout, aes.CreateDecryptor(key, iv), CryptoStreamMode.Write);
            }

            while(rdlen < totlen)
            {
                len = fin.Read(bin, 0, 100);
                cryptoStream.Write(bin, 0, len);
                rdlen = rdlen + len;
            }

            cryptoStream.Close();
            fout.Close();
            fin.Close(); 

            if(encrypt){
                // Seek doesnt work as its overwrites the existing bytes
                // Reading file again 
                int bytesRead, totalBytesRead = 0;
               
                byte[] buffer = new byte[1024];
                string tempFilePath = string.Format("{0}.iv.txt", saveFilePath);
                using (Stream sr = new FileStream(saveFilePath, FileMode.Open, FileAccess.Read)){
                    using (Stream sw = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write)){
                        sw.Write(iv, 0, iv.Length);
                        while ((bytesRead = sr.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            sw.Write(buffer, 0, bytesRead);
                            totalBytesRead += bytesRead;
                        }
                        sw.Close();
                        sr.Close();
                    }
                } 
                File.Delete(saveFilePath);
                File.Move(tempFilePath, saveFilePath);
            }

            return true;
        }

        public override string ComputeHashRaw (string input)
        {
        #if (SILVERLIGHT || WINDOWS_PHONE || MONOTOUCH || __IOS__ || MONODROID || __ANDROID__ || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_IOS || UNITY_ANDROID || UNITY_5 || UNITY_WEBGL)
            HashAlgorithm algorithm = new System.Security.Cryptography.SHA256Managed ();
        #else
        HashAlgorithm algorithm = new SHA256CryptoServiceProvider ();
        #endif

            Byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes (input);
            Byte[] hashedBytes = algorithm.ComputeHash (inputBytes);
            return BitConverter.ToString (hashedBytes);
        }

        protected override string EncryptOrDecrypt (bool type, string plainStr)
        {
            {
                RijndaelManaged aesEncryption = new RijndaelManaged ();
                aesEncryption.KeySize = 256;
                aesEncryption.BlockSize = 128;
                //Mode CBC
                aesEncryption.Mode = CipherMode.CBC;
                //padding
                aesEncryption.Padding = PaddingMode.PKCS7;
                aesEncryption.Key = System.Text.Encoding.ASCII.GetBytes (GetEncryptionKey ());
                
                if (type) {
                    //get ASCII bytes of the string
                    if(UseRandomInitializationVector){
                        // Generate IV here
                        aesEncryption.GenerateIV();
                    } else {
                        aesEncryption.IV = System.Text.Encoding.ASCII.GetBytes ("0123456789012345");
                    }

                    ICryptoTransform crypto = aesEncryption.CreateEncryptor ();
                    plainStr = EncodeNonAsciiCharacters (plainStr);
                    byte[] plainText = Encoding.UTF8.GetBytes (plainStr);

                    //encrypt
                    byte[] cipherText = crypto.TransformFinalBlock (plainText, 0, plainText.Length);
                    
                    if(UseRandomInitializationVector){
                        // Add IV
                        byte[] message = new byte[cipherText.Length + aesEncryption.IV.Length];
                        System.Buffer.BlockCopy(aesEncryption.IV, 0, message, 0, aesEncryption.IV.Length);
                        System.Buffer.BlockCopy(cipherText, 0, message, aesEncryption.IV.Length, cipherText.Length);
                        cipherText = new byte[message.Length];
                        System.Buffer.BlockCopy(message, 0, cipherText, 0, message.Length);
                    }
                    return Convert.ToBase64String (cipherText);
                } else {
                    try {
                        //decode
                        byte[] decryptedBytes = Convert.FromBase64CharArray (plainStr.ToCharArray (), 0, plainStr.Length);

                        if(UseRandomInitializationVector){
                            // Extract IV here
                            byte[] iv = new byte[16];
                            System.Buffer.BlockCopy(decryptedBytes, 0, iv, 0, 16);
                            aesEncryption.IV = iv;
                            byte[] message = new byte[decryptedBytes.Length - 16];
                            System.Buffer.BlockCopy(decryptedBytes, 16, message, 0, decryptedBytes.Length-16);
                            decryptedBytes = new byte[message.Length];
                            System.Buffer.BlockCopy(message, 0, decryptedBytes, 0, message.Length);

                        } else {
                            aesEncryption.IV = System.Text.Encoding.ASCII.GetBytes ("0123456789012345");
                        }
                        
                        ICryptoTransform decrypto = aesEncryption.CreateDecryptor ();

                        //decrypt
                        string decrypted = System.Text.Encoding.UTF8.GetString (decrypto.TransformFinalBlock (decryptedBytes, 0, decryptedBytes.Length));

                        return decrypted;
                    } catch (Exception ex) {
                        #if (ENABLE_PUBNUB_LOGGING)
                        pnLog.WriteToLog (string.Format ("Decrypt Error. {0}",  ex.ToString ()), PNLoggingMethod.LevelInfo);
                        #endif
                        throw new PubNubException("Decrypt Error", ex);
                    }
                }
            }
        }
    }
    #endregion
}

