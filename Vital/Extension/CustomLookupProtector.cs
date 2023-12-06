using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Vital.Extension
{
    // Define a public class named CustomLookupProtector
    public class CustomLookupProtector : ILookupProtector
    {
        // Define an initialization vector for encryption and decryption
        byte[] iv = { 208, 148, 29, 187, 168, 51, 181, 178, 137, 83, 40, 13, 28, 177, 131, 248 };

        // Define a method named Protect
        public string Protect(string keyId, string data)
        {
            // Convert the input data to bytes
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(data);

            string cipherText;
            // Creating an Aes object for encryption
            using (SymmetricAlgorithm algorithm = Aes.Create())
            {
                // Create an encryptor from the provided key and IV.
                using (ICryptoTransform encryptor = algorithm.CreateEncryptor(Encoding.UTF8.GetBytes(keyId), iv))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            // Perform the encryption
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.Close();

                            // Convert the encrypted bytes back into a string
                            byte[] chiperTextByte = ms.ToArray();
                            cipherText = Convert.ToBase64String(chiperTextByte);
                        }
                    }
                }
            }

            // Return the encrypted string
            return cipherText;
        }


        // Define a method named Unprotect
        public string Unprotect(string keyId, string data)
        {
            // Convert the input data back into bytes
            byte[] cipherTextBytes = Convert.FromBase64String(data);
            string plainText;

            // Create a Aes object for decryption.
            using (SymmetricAlgorithm algorithm = Aes.Create())
            {
                // Create a decryptor from the provided key and IV.
                using (ICryptoTransform decrypter = algorithm.CreateDecryptor(Encoding.UTF8.GetBytes(keyId), iv))
                {
                    using (MemoryStream ms = new MemoryStream(cipherTextBytes))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(ms, decrypter, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader(cryptoStream))
                            {
                                // Decrypt the data and read it into a string
                                plainText = streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }

            // Return the decrypted string.
            return plainText;
        }
    }
}
