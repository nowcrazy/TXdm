using System;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Text;
using NETCore.Encrypt;
using NETCore.Encrypt.Shared;
using static NETCore.Encrypt.Shared.Check;

namespace Repository.Helper
{
    public class encryption
    {
        public encryption()
        {
        }
        #region MD5加密
        public static string Md5(string srcString, MD5Length length = MD5Length.L32)
        {
            Check.Argument.IsNotEmpty(srcString, nameof(srcString));
            string str_md5_out = string.Empty;
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes_md5_in = Encoding.UTF8.GetBytes(srcString);
                byte[] bytes_md5_out = md5.ComputeHash(bytes_md5_in);

                str_md5_out = length == MD5Length.L32
                    ? BitConverter.ToString(bytes_md5_out)
                    : BitConverter.ToString(bytes_md5_out, 4, 8);

                str_md5_out = str_md5_out.Replace("-", "");
                return str_md5_out;
            }
        }
        #endregion
        #region DES加密
        public static byte[] DESEncrypt(byte[] data, string key, string vector)
        {
            Check.Argument.IsNotEmpty(data, nameof(data));
            Check.Argument.IsNotEmpty(key, nameof(key));
            Check.Argument.IsEqualLength(key.Length, 24, nameof(key));
            Check.Argument.IsNotEmpty(vector, nameof(vector));
            Check.Argument.IsEqualLength(vector.Length, 8, nameof(vector));

            return DESEncrypt(data, key, CipherMode.CBC, vector);
        }
        #endregion
        #region Des加密
        private static byte[] DESEncrypt(byte[] data, string key, CipherMode cipherMode, string vector = "", PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            Check.Argument.IsNotEmpty(data, nameof(data));
            Check.Argument.IsNotEmpty(key, nameof(key));
            Check.Argument.IsEqualLength(key.Length, 24, nameof(key));

            using (MemoryStream Memory = new MemoryStream())
            {
                using (TripleDES des = TripleDES.Create())
                {
                    byte[] plainBytes = data;
                    byte[] bKey = new byte[24];
                    Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);

                    des.Mode = cipherMode;
                    des.Padding = paddingMode;
                    des.Key = bKey;

                    if (cipherMode == CipherMode.CBC)
                    {
                        byte[] bVector = new byte[8];
                        Array.Copy(Encoding.UTF8.GetBytes(vector.PadRight(bVector.Length)), bVector, bVector.Length);
                        des.IV = bVector;
                    }

                    using (CryptoStream cryptoStream = new CryptoStream(Memory, des.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        try
                        {
                            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            return Memory.ToArray();
                        }
                        catch (Exception ex)
                        {
                            return null;
                        }
                    }
                }
            }
        }
        #endregion
        #region Des解密
        public static byte[] DESDecrypt(byte[] data, string key, string vector)
        {
            Check.Argument.IsNotEmpty(data, nameof(data));
            Check.Argument.IsNotEmpty(key, nameof(key));
            Check.Argument.IsEqualLength(key.Length, 24, nameof(key));
            Check.Argument.IsNotEmpty(vector, nameof(vector));
            Check.Argument.IsEqualLength(vector.Length, 8, nameof(vector));

            return DESDecrypt(data, key, CipherMode.CBC, vector);
        }
        #endregion
        #region Des解密
        private static byte[] DESDecrypt(byte[] data, string key, CipherMode cipherMode, string vector = "", PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            Check.Argument.IsNotEmpty(data, nameof(data));
            Check.Argument.IsNotEmpty(key, nameof(key));
            Check.Argument.IsEqualLength(key.Length, 24, nameof(key));

            byte[] encryptedBytes = data;
            byte[] bKey = new byte[24];
            Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);

            using (MemoryStream Memory = new MemoryStream(encryptedBytes))
            {
                using (TripleDES des = TripleDES.Create())
                {
                    des.Mode = cipherMode;
                    des.Padding = paddingMode;
                    des.Key = bKey;

                    if (cipherMode == CipherMode.CBC)
                    {
                        byte[] bVector = new byte[8];
                        Array.Copy(Encoding.UTF8.GetBytes(vector.PadRight(bVector.Length)), bVector, bVector.Length);
                        des.IV = bVector;
                    }

                    using (CryptoStream cryptoStream = new CryptoStream(Memory, des.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        try
                        {
                            byte[] tmp = new byte[encryptedBytes.Length];
                            int len = cryptoStream.Read(tmp, 0, encryptedBytes.Length);
                            byte[] ret = new byte[len];
                            Array.Copy(tmp, 0, ret, 0, len);
                            return ret;
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
            }
        }
        #endregion
        #region Aes加密
        public static string AESEncrypt(string data, string key)
        {
            Check.Argument.IsNotEmpty(data, nameof(data));
            Check.Argument.IsNotEmpty(key, nameof(key));
            Check.Argument.IsEqualLength(key.Length, 32, nameof(key));

            using (MemoryStream memory = new MemoryStream())
            {
                using (Aes aes = Aes.Create())
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(data);
                    byte[] bKey = new byte[32];
                    Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);

                    aes.Mode = CipherMode.ECB;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.KeySize = 256;
                    aes.Key = bKey;

                    using (CryptoStream cryptoStream = new CryptoStream(memory, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        try
                        {
                            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            return Convert.ToBase64String(memory.ToArray());
                        }
                        catch (Exception ex)
                        {
                            return null;
                        }
                    }
                }
            }
        }
        #endregion
        #region Aes解密
        public static string AESDecrypt(string data, string key)
        {
            Check.Argument.IsNotEmpty(data, nameof(data));
            Check.Argument.IsNotEmpty(key, nameof(key));
            Check.Argument.IsEqualLength(key.Length, 32, nameof(key));

            byte[] encryptedBytes = Convert.FromBase64String(data);
            byte[] bKey = new byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);

            try
            {
                byte[] decryptedData = null; // decrypted data

                using (MemoryStream memory = new MemoryStream(encryptedBytes))
                {
                    using (Aes aes = Aes.Create())
                    {
                        aes.Mode = CipherMode.ECB;
                        aes.Padding = PaddingMode.PKCS7;
                        aes.KeySize = 256;
                        aes.Key = bKey;

                        using (CryptoStream decryptor = new CryptoStream(memory, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (MemoryStream tempMemory = new MemoryStream())
                            {
                                byte[] buffer = new byte[1024];
                                Int32 readBytes = 0;
                                while ((readBytes = decryptor.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    tempMemory.Write(buffer, 0, readBytes);
                                }

                                decryptedData = tempMemory.ToArray();
                                return Encoding.UTF8.GetString(decryptedData);
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

    }
}

