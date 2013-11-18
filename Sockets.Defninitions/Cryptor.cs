using System.Security.Cryptography;
using System.Text;

namespace Apocalypse.Sockets.Definitions
{
    public class Cryptor
    {
        private readonly byte[] _key;

        public Cryptor(string sessionPassword)
        {
            _key = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(sessionPassword));
        }

        public byte[] Decrypt(byte[] take)
        {
            if (take == null || take.Length == 0)
            {
                return new byte[] {};
            }
            var des = new TripleDESCryptoServiceProvider { Key = _key, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
            var result = des.CreateDecryptor();
            var ret = result.TransformFinalBlock(take, 0, take.Length);
            des.Clear();
            return ret;
        }

        public byte[] Crypt(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return new byte[] {};
            }
            var des = new TripleDESCryptoServiceProvider { Key = _key, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
            var enc = des.CreateEncryptor();
            var arr = enc.TransformFinalBlock(bytes, 0, bytes.Length);
            des.Clear();
            return arr;
        }
    }
}
