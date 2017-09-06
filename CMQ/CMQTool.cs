using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TencentCloud.CMQ {
    /// <summary>
    /// CMQ ǩ������
    /// </summary>
    public class CMQTool {
        /// <summary>
        /// Url ǩ������
        /// </summary>
        /// <param name="src"></param>
        /// <param name="key"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string Sign(string src, string key, string method = "sha1") {
            if (method == "sha1") {
                byte[] signByteArrary = HmacSha1Sign(src, key);
                return Convert.ToBase64String(signByteArrary);
            } else {
                byte[] signByteArrary = HmacSHA256Sign(src, key);
                return Convert.ToBase64String(signByteArrary);
            }
        }

        #region ���ú���
        /// <summary>
        /// UnixTimeʱ���
        /// </summary>
        /// <param name="expired">��Ч�ڣ���λ���룩</param>
        /// <returns></returns>
        public static string UnixTime(double expired = 0) {
            var time = (DateTime.Now.AddSeconds(expired).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            return time.ToString();
        }
        /// <summary>
        /// �ֽ�����ϲ�
        /// </summary>
        /// <param name="byte1"></param>
        /// <param name="byte2"></param>
        /// <returns></returns>
        public static byte[] JoinByteArr(byte[] byte1, byte[] byte2) {
            byte[] full = new byte[byte1.Length + byte2.Length];
            Stream s = new MemoryStream();
            s.Write(byte1, 0, byte1.Length);
            s.Write(byte2, 0, byte2.Length);
            s.Position = 0;
            int r = s.Read(full, 0, full.Length);
            if (r > 0) {
                return full;
            }
            throw new Exception("��ȡ����!");
        }
        /// <summary>
        /// HMAC-SHA1 �㷨ǩ��
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static byte[] HmacSha1Sign(string str, string key) {
            byte[] keyBytes = StrToByteArr(key);
            HMACSHA1 hmac = new HMACSHA1(keyBytes);
            byte[] inputBytes = StrToByteArr(str);
            return hmac.ComputeHash(inputBytes);
        }
        /// <summary>
        /// HmacSHA256 �㷨ǩ��
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static byte[] HmacSHA256Sign(string str, string key) {
            byte[] keyBytes = StrToByteArr(key);
            HMACSHA256 hmac = new HMACSHA256(keyBytes);            
            byte[] inputBytes = StrToByteArr(str);
            return hmac.ComputeHash(inputBytes);
        }
        /// <summary>
        /// �ַ���ת�ֽ�����
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] StrToByteArr(string str) {
            return Encoding.UTF8.GetBytes(str);
        }
        /// <summary>
        /// �ֽ�����ת�ַ���
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static string ByteArrToStr(byte[] byteArray) {
            return Encoding.UTF8.GetString(byteArray);
        }
        #endregion

    }

}