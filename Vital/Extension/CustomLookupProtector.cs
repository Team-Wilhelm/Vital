using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Vital.Extension;

// Define a public class named CustomLookupProtector
public class CustomLookupProtector : ILookupProtector
{
    // Define an initialization vector for encryption and decryption
    byte[] iv = { 208, 148, 29, 187, 168, 51, 181, 178, 137, 83, 40, 13, 28, 177, 131, 248 };

    public string Protect(string keyId, string? data)
    {
        // Convert the input data to bytes
        var plainTextBytes = Encoding.UTF8.GetBytes(data ?? "");

        // Creating an Aes object for encryption
        using SymmetricAlgorithm algorithm = Aes.Create();
        // Create an encryptor from the provided key and IV.
        using var encryptor = algorithm.CreateEncryptor(Encoding.UTF8.GetBytes(keyId), iv);
        using var ms = new MemoryStream();
        using var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        // Perform the encryption
        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
        cryptoStream.Close();

        // Convert the encrypted bytes back into a string
        var chiperTextByte = ms.ToArray();
        var cipherText = Convert.ToBase64String(chiperTextByte);

        // Return the encrypted string
        return cipherText;
    }


    public string Unprotect(string keyId, string? data)
    {
        // Convert the input data back into bytes
        var cipherTextBytes = Convert.FromBase64String(data ?? "");

        // Create a Aes object for decryption.
        using SymmetricAlgorithm algorithm = Aes.Create();
        // Create a decryptor from the provided key and IV.
        using var decrypter = algorithm.CreateDecryptor(Encoding.UTF8.GetBytes(keyId), iv);
        using var ms = new MemoryStream(cipherTextBytes);
        using var cryptoStream = new CryptoStream(ms, decrypter, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);
        // Decrypt the data and read it into a string
        var plainText = streamReader.ReadToEnd();

        // Return the decrypted string.
        return plainText;
    }
}
