using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace EncryptStringSample
{
    public static class StringCipher
    {
        // This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        //      private const string initVector = "tu89geji340t89u2";
        //private const string initVector = "2lk77nbazplowhda";
        private const string initVector = "limmquusmmd1luhd";

        // This constant is used to determine the keysize of the encryption algorithm.
        private const int keysize = 256;

        private const int DEFAULT_BYTES_SIZE = 128 * 1024;

        private static int currentBufferSize = DEFAULT_BYTES_SIZE;
        private static byte[] initVectorBytes;
        private static byte[] plainTextBytes = new byte[DEFAULT_BYTES_SIZE];

        private static string passwordPhraseEcryptor = string.Empty;

        private static byte[] memoryBuffer = new byte[DEFAULT_BYTES_SIZE];

        static StringCipher()
        {
            initVectorBytes = Encoding.UTF8.GetBytes(initVector);
        }

        public static string Encrypt(string plainText, string passPhrase)
        {
#if UNITY_WP8
            throw new NotSupportedException("StringCipher doesn't support current platform");
#else

            int charCount = plainText.Length;
            int maxbytesCount = Encoding.UTF8.GetMaxByteCount(charCount);
            if (maxbytesCount > currentBufferSize)
            {
                int difference = (int)Math.Ceiling((float)maxbytesCount / currentBufferSize);
                currentBufferSize *= difference;
                plainTextBytes = new byte[currentBufferSize];
                memoryBuffer = new byte[currentBufferSize];
            }

            int bytesCount = Encoding.UTF8.GetBytes(plainText, 0, charCount, plainTextBytes, 0);

            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = new MemoryStream(memoryBuffer, 0, memoryBuffer.Length);
            //MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, bytesCount);
            cryptoStream.FlushFinalBlock();
            int memoryStreamPosition = (int)memoryStream.Position;
            memoryStream.Close();
            cryptoStream.Close();

            return Convert.ToBase64String(memoryBuffer, 0, memoryStreamPosition);


            /*
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
            */

#endif
        }

        public static byte[] EncryptToBytes(string plainText, string passPhrase)
        {
#if UNITY_WP8
            throw new NotSupportedException("StringCipher doesn't support current platform");
#else

            //var initVectorBytes = Encoding.UTF8.GetBytes(initVector);

            int charCount = plainText.Length;
            //int bytesCount = Encoding.UTF8.GetMaxByteCount(charCount);
            int bytesCount = Encoding.UTF8.GetBytes(plainText, 0, charCount, plainTextBytes, 0);

            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, bytesCount);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return cipherTextBytes;
#endif
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
#if UNITY_WP8
            throw new NotSupportedException("StringCipher doesn't support current platform");
#else
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
#endif
        }

        public static string ComputeHmac(byte[] input, byte[] key)
        {
            using (var hmac = new HMACSHA1(key))
            {
                return Convert.ToBase64String(hmac.ComputeHash(input));
            }
        }

    }
}
