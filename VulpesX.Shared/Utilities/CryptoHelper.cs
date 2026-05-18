using GeneXus.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared.Utilities
{
    public static class CryptoHelper
    {
        public static string SHA512Hasher(string Info)
        {
            var hasher = SHA512.Create();
            byte[] data = hasher.ComputeHash(Encoding.Unicode.GetBytes(Info));
            return (Encoding.Unicode.GetString(data));
        }

        public static string? CSDecrypt(string ConnectionString, byte[] PK, string Domain)
        {
            try
            {
                SymmetricAlgorithm symmetricAlgorithm = DES.Create();
                symmetricAlgorithm.Key = PK;
                symmetricAlgorithm.IV = Encoding.UTF8.GetBytes(Domain, 0, 8);
                ICryptoTransform transform = symmetricAlgorithm.CreateDecryptor();
                byte[] inputbuffer = Convert.FromBase64String(ConnectionString);
                byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
                return Encoding.UTF8.GetString(outputBuffer, 0, outputBuffer.Length);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string CSEncrypt(string ConnectionString, byte[] PK, string Domain)
        {
            SymmetricAlgorithm symmetricAlgorithm = DES.Create();
            symmetricAlgorithm.Key = PK;
            symmetricAlgorithm.IV = Encoding.UTF8.GetBytes(Domain, 0, 8);
            ICryptoTransform transform = symmetricAlgorithm.CreateEncryptor();
            byte[] inputbuffer = Encoding.UTF8.GetBytes(ConnectionString);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Convert.ToBase64String(outputBuffer);
        }

        public static string? Encrypt64(string Decipher, string Value)
        {
            if (Value != null)
                return (Crypto.Encrypt64(Value, Decipher));
            else
                return (null);
        }

        private static string simplePK = "vulPpLuV";
        private static string simpleIV = "warnactu";
        public static string SimpleEncrypt(string Text)
        {
            SymmetricAlgorithm symmetricAlgorithm = DES.Create();
            symmetricAlgorithm.Key = Encoding.UTF8.GetBytes(simplePK);
            symmetricAlgorithm.IV = Encoding.UTF8.GetBytes(simpleIV, 0, 8);
            ICryptoTransform transform = symmetricAlgorithm.CreateEncryptor();
            byte[] inputbuffer = Encoding.UTF8.GetBytes(Text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Convert.ToBase64String(outputBuffer);
        }

        public static string? SimpleDecrypt(string Text)
        {
            try
            {
                SymmetricAlgorithm symmetricAlgorithm = DES.Create();
                symmetricAlgorithm.Key = Encoding.UTF8.GetBytes(simplePK);
                symmetricAlgorithm.IV = Encoding.UTF8.GetBytes(simpleIV, 0, 8);
                ICryptoTransform transform = symmetricAlgorithm.CreateDecryptor();
                byte[] inputbuffer = Convert.FromBase64String(Text);
                byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
                return Encoding.UTF8.GetString(outputBuffer, 0, outputBuffer.Length);
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
