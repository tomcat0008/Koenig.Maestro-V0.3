using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace Koenig.Maestro.Operation.Utility
{
    public static class StringUtils
    {

        public static string EncodeFileName(string fileName)
        {
            return fileName.Replace(":", "[").Replace("\\", "]").Replace(".", "[dot]");
        }

        public static string DecodeFileName(string fileName)
        {
            return fileName.Replace("[dot]", ".").Replace("[", ":").Replace("]", "\\");
        }

        public static string EncodeCacheId(string cacheId)
        {
            return cacheId.Replace("+", "[plus]").Replace(".", "[dot]");
        }

        public static string DecodeCacheId(string cacheId)
        {
            return cacheId.Replace("[plus]", "+").Replace("[dot]", ".");
        }

        public static string ConcatExceptionMessages(Exception e)
        {
            StringBuilder sb = new StringBuilder();
            while (e != null)
            {
                sb.Append(e.Message);
                sb.Append(Environment.NewLine);
                e = e.InnerException;
            }

            return sb.ToString();
        }

        public static string ConvertToPascalCase(string str, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(char.ToUpper(str[0], cultureInfo));
            for (int i = 1; i < str.Length; i++)
            {
                sb.Append(char.ToLower(str[i], cultureInfo));
            }

            return sb.ToString();
        }

        public static string ConvertToPascalCase(string str)
        {
            return ConvertToPascalCase(str, CultureInfo.InvariantCulture);
        }

        public static string ConvertByteArrayToHex(byte[] arr)
        {
            StringBuilder sb = new StringBuilder();
            if (arr != null)
            {
                foreach (var b in arr)
                {
                    sb.AppendFormat("{0:X2}", b);
                }
            }
            return sb.ToString();
        }

        public static string ConvertByteArrayToString(byte[] arr)
        {
            StringBuilder sb = new StringBuilder();
            if (arr != null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    sb.Append((char)arr[i]);
                }
            }
            return sb.ToString();
        }

        public static string WriteByteArrayToString(byte[] arr)
        {
            StringBuilder sb = new StringBuilder();
            if (arr != null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    sb.AppendFormat(" {0}", arr[i]);
                }
            }
            return sb.ToString();
        }

        public static byte[] ConvertHexToByteArray(string hex)
        {
            if (String.IsNullOrEmpty(hex))
            {
                return null;
            }

            int len = hex.Length;

            if (len % 2 == 1)
            {
                return null;
            }

            int halfOfLen = len / 2;

            byte[] res = new byte[halfOfLen];
            for (int i = 0; i < halfOfLen; i++)
            {
                res[i] = Byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            return res;
        }

        public static string ToString(object obj)
        {
            return obj == null ? "null" : obj.ToString();
        }

        public static string ApplyFormat(object obj, int formatLength)
        {
            if (obj == null)
            {
                return new string(' ', formatLength);
            }

            string str = obj.ToString();
            if (formatLength == 0)
            {
                return str;
            }

            if (str.Length > formatLength)
            {
                return str.Substring(0, formatLength);
            }

            StringBuilder fmtsb = new StringBuilder("{0,");
            if (formatLength > 0)
            {
                fmtsb.Append('-');
            }
            fmtsb.Append(formatLength);
            fmtsb.Append('}');

            return string.Format(fmtsb.ToString(), str);
        }

        public static string ConvertToHexaDecimal(string asciiString)
        {
            string hex = String.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (char c in asciiString)
            {
                int tmp = c;
                sb.AppendFormat("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return sb.ToString();
        }

        public static string ConvertHexToAscii(string hexValue)
        {
            string strValue = "";
            while (hexValue.Length > 0)
            {
                strValue += Convert.ToChar(Convert.ToUInt32(hexValue.Substring(0, 2), 16)).ToString();
                hexValue = hexValue.Substring(2, hexValue.Length - 2);
            }
            return hexValue;
        }

        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        /// <summary>
        /// Compresses given string using GZipStream
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        /// <summary>
        /// Decompresses the given byte array, compressed using GZipStream
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    //gs.CopyTo(mso);
                    CopyTo(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        /// <summary>
        /// http://stackoverflow.com/questions/15919598/serialize-datetime-as-binary
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string SerializeDateTimeToString(DateTime date)
        {
            ulong dataToSerialise = (ulong)(date.Ticks | ((long)date.Kind) << 62);
            return dataToSerialise.ToString();
        }

        /// <summary>
        /// http://stackoverflow.com/questions/15919598/serialize-datetime-as-binary
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DateTime DeserializeDateTimeFromString(string data)
        {
            ulong deserialisedData = ulong.Parse(data);
            long ticks = (long)(deserialisedData & 0x3FFFFFFFFFFFFFFF);
            DateTimeKind kind = (DateTimeKind)(deserialisedData >> 62);
            DateTime date = new DateTime(ticks, kind);
            return date;
        }

        public static string SanitizeFileName(string fileName)
        {
            if (fileName == null)
            {
                return null;
            }
            return fileName.Replace("..", "").Replace(@"\", "");
        }

        public static void CheckSecureFilePath(string fileName)
        {
            if (fileName == null)
            {
                return;
            }
            if (fileName.ToLowerInvariant().IndexOf("windows") > 0)
            {
                throw new Exception(string.Format("insecure file path {0}", fileName));
            }
            return;
        }

        public static void CheckSecureCommand(string command)
        {
            if (command == null)
            {
                return;
            }
            if (command.IndexOf("&") > 0)
            {
                throw new Exception(string.Format("insecure command {0}", command));
            }
        }

        /// <summary>
        /// Returns md5hashcode. Default encoding is Turkish
        /// </summary>
        /// <param name="input"></param>
        /// <param name="codePage">1254=>Turkish (Windows)https://msdn.microsoft.com/en-us/library/system.text.encodinginfo.getencoding(v=vs.110).aspx</param></param>
        /// <returns></returns>
        public static string GetMd5Hash(string input, int codePage = 1254)
        {

            //https://msdn.microsoft.com/en-us/library/system.security.cryptography.md5(v=vs.110).aspx
            // Convert the input string to a byte array and compute the hash.
            //byte[] data = md5Hash.ComputeHash(Encoding.ASCII.GetBytes(input));
            string output;
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.GetEncoding(codePage).GetBytes(input));
                output = GetStringFromByteArray(data);
            }
            // Return the hexadecimal string.
            return output;
        }

        public static bool VerifyMd5Hash(string input, string hash, int codePage = 1254)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(input, codePage);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GetSha256Hash(string input, int codePage = 1254)
        {
            string output;
            using (SHA256 mySHA256 = SHA256Managed.Create())
            {
                byte[] data = mySHA256.ComputeHash(Encoding.GetEncoding(codePage).GetBytes(input));
                output = GetStringFromByteArray(data);
            }
            return output;
        }

        static string GetStringFromByteArray(byte[] data)
        {
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public static string MaskCardNumber(string cardNumber)
        {
            try
            {
                string maskedCardNumber = string.Empty;
                if (!string.IsNullOrEmpty(cardNumber))
                {
                    if (cardNumber.Length >= 6 /*6 digitden büyük tüm kartları maskelemek için*/)
                        maskedCardNumber = cardNumber.Substring(0, 6) + "".PadLeft(cardNumber.Length - 4 - 6, '*') + cardNumber.Substring(cardNumber.Length - 4);
                    else throw new Exception("Invalid card number length.");
                }
                return maskedCardNumber;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to mask card number. Internal: " + ex.Message);
            }
        }
    }
}
