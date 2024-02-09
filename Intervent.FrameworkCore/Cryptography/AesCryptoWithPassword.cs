using System.Security.Cryptography;
using System.Text;

namespace Intervent.Framework.Cryptography
{
    public sealed class AesCryptoWithPassword
    {
        private byte[] _key;

        private byte[] _iv;

        private int _keySize = 128;

        private int _blockSize = 128;

        private CipherMode _cipherMode = CipherMode.CBC;

        private PaddingMode _paddingMode = PaddingMode.PKCS7;


        public AesCryptoWithPassword(string password, byte[] salt)
        {
            Rfc2898DeriveBytes keyGenerator = new Rfc2898DeriveBytes(password, salt);
            _key = keyGenerator.GetBytes(16);
            _iv = keyGenerator.GetBytes(16);
        }

        public string Encrypt(string clearText)
        {
            RijndaelManaged aesEncryption = new RijndaelManaged();
            aesEncryption.KeySize = _keySize;
            aesEncryption.BlockSize = _blockSize;
            aesEncryption.Mode = _cipherMode;
            aesEncryption.Padding = _paddingMode;


            ICryptoTransform cryptoEncryptor = aesEncryption.CreateEncryptor(_key, _iv);

            MemoryStream mStream = new MemoryStream();

            CryptoStream writerStream = new CryptoStream(mStream, cryptoEncryptor, CryptoStreamMode.Write);

            byte[] buffer = Encoding.UTF8.GetBytes(clearText);

            writerStream.Write(buffer, 0, buffer.Length);
            writerStream.FlushFinalBlock();

            byte[] cipherTextinBytes = mStream.ToArray();

            mStream.Close();
            writerStream.Close();

            string cipherText = Convert.ToBase64String(cipherTextinBytes, 0, cipherTextinBytes.Length);

            return cipherText;
        }

        public string Decrypt(string cipherText)
        {

            RijndaelManaged aesDecryption = new RijndaelManaged();
            aesDecryption.KeySize = _keySize;
            aesDecryption.BlockSize = _blockSize;
            aesDecryption.Mode = _cipherMode;
            aesDecryption.Padding = _paddingMode;
            byte[] cipherTextBytesForDecrypt = Convert.FromBase64String(cipherText);

            ICryptoTransform cryptoDecryptor = aesDecryption.CreateDecryptor(_key, _iv);

            MemoryStream memStreamEncryptData = new MemoryStream(cipherTextBytesForDecrypt);

            CryptoStream rStream
                = new CryptoStream(memStreamEncryptData, cryptoDecryptor, CryptoStreamMode.Read);

            byte[] plainTextBytes = new byte[cipherTextBytesForDecrypt.Length];

            int decryptedByteCount = rStream.Read(plainTextBytes, 0, plainTextBytes.Length);

            memStreamEncryptData.Close();
            rStream.Close();

            string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

            return plainText;
        }
    }
}
