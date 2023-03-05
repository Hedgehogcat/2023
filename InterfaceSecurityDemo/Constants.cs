using Microsoft.Extensions.Configuration.Json;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;



namespace InterfaceSecurityDemo
{
    public static class Constants
    {
        public static readonly string CurrentUserOwinEnvironmentKey = "JustFun.user";

        public static readonly string CurrentUserTokenOwinEnvironmentKey = "JustFun.usertoken";

        public const string Sha512HashAlgorithm = "SHA512";

        public const string DefalutUniqueSeoCode = "EN";

        /// <summary>
        /// 加密字符串1
        /// </summary>
        public const string SECRET1 = "ds*^tgg)B5445";

        /// <summary>
        /// 加密字符串2
        /// </summary>
        public const string SECRET2 = "988Fddgb@^78956$&";

        /// <summary>
        /// 新粉丝消息提醒资源名称
        /// </summary>
        public const string ResourceFans = "resourcefans";

        /// <summary>
        /// 关注人新动态消息提醒资源名称
        /// </summary>
        public const string ResourceFollow = "resourcefollow";

        /// <summary>
        /// 评论发布内容资源名称
        /// </summary>
        public const string ResourceComment = "resourcecomment";

        /// <summary>
        /// 内容点赞资源名称
        /// </summary>
        public const string ResourcePraise = "resourcepraise";

        /// <summary>
        /// @资源名称
        /// </summary>
        public const string ResourceRemaind = "resourceremaind";

        /// <summary>
        /// 内容回复名称
        /// </summary>
        public const string ResourceReply = "resourcereply";

    }
    public class RandomHelper
    {
        /// <summary>
        ///生成制定位数的随机码（数字）
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandomCode(int length)
        {
            var result = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                var r = new Random(Guid.NewGuid().GetHashCode());
                result.Append(r.Next(0, 10));
            }
            return result.ToString();
        }
    }
    public class AppSettingsHelper
    {
        public static IConfiguration Configuration { get; set; }
        static AppSettingsHelper()
        {
            //ReloadOnChange = true 当appsettings.json被修改时重新加载            
            Configuration = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
            .Build();
        }

    }

    public static class Commons
    {


        private static int _timeStampExpires = 5;
        /// <summary>
        /// 时间戳过期默认3分钟
        /// </summary>
        public static int TimeStampExpires
        {
            get
            {
                int.TryParse(AppSettings("TimeStampExpires"), out _timeStampExpires);
                return _timeStampExpires;
            }
        }

        /// <summary>
        /// 读取AppSettings
        /// </summary>
        public static string AppSettings(string key)
        {
            var result = AppSettingsHelper.Configuration[key];
            return result;
        }
        #region 签名算法
        private static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        /// <summary>
        /// 获取当前实例的日期和时间的计时周期数(Ticks表示一百纳秒的一千万分之一)
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentTimeStepNumber()
        {
            var delta = DateTime.UtcNow - _unixEpoch;
            return (int)delta.TotalSeconds;
        }

        /// <summary>
        /// 日期格式转换为时间戳(UTC)格式
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int ConvertToTimeStep(DateTime time)
        {
            if (time.Kind == DateTimeKind.Unspecified || time.Kind == DateTimeKind.Local)
            { time = time.ToUniversalTime(); }
            var delta = time - _unixEpoch;
            return (int)delta.TotalSeconds;
        }
        /// <summary>
        /// 日期格式转换为时间戳(UTC)格式
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int ConvertToTimeStepOutOfCheck(DateTime time)
        {
            var delta = time - _unixEpoch;
            return (int)delta.TotalSeconds;
        }
        /// <summary>
        /// Ticks转换为Utc时间
        /// </summary>
        /// <param name="ticks">日期和时间的计时周期数</param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(double ticks)
        {
            return _unixEpoch.AddSeconds(ticks);
        }

        /// <summary>
        /// 检查sign签名字符串是否有效
        /// </summary>
        /// <param name="sign">签名</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="parameters">时间戳+硬代码字符串1+随机字符串+硬代码字符串2+apptype</param>
        /// <returns></returns>
        public static bool CheckSignature(string sign, long timestamp, IDictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(sign))
                return false;
            if (timestamp == 0)
                return false;
            System.DateTime dtTime = ConvertToDateTime(timestamp);
            double minutes = DateTime.UtcNow.Subtract(dtTime).Minutes;
            if (minutes > Commons.TimeStampExpires)
                return false;

            var token = GetSignature(parameters);
            return token == sign;
        }

        /// <summary>
        /// 验证时间戳
        /// </summary>
        /// <param name="timeStamp">时间戳</param>
        /// <param name="expiresMinutes">分钟数</param>
        /// <returns>True:过期</returns>
        public static bool CheckTimeStamp(long timeStamp, int expiresMinutes)
        {
            System.DateTime dtTime = ConvertToDateTime(timeStamp);
            double minutes = DateTime.UtcNow.Subtract(dtTime).Minutes;
            if (minutes > expiresMinutes)
                return true;
            return false;
        }

        /// <summary>
        /// 计算参数签名
        /// </summary>
        /// <param name="parameters">请求参数集，所有参数必须已转换为字符串类型</param>
        /// <returns>签名</returns>
        public static string GetSignature(IDictionary<string, string> parameters)
        {
            // 先将参数以其参数名的字典序升序进行排序
            IDictionary<string, string> sortedParams;

            if (parameters == null || parameters.Count == 0)
                sortedParams = new SortedDictionary<string, string>();
            else
                sortedParams = new SortedDictionary<string, string>(parameters);

            IEnumerator<KeyValuePair<string, string>> iterator = sortedParams.GetEnumerator();

            // 遍历排序后的字典，将所有参数按"key=value"格式拼接在一起
            List<string> arguList = new List<string>();
            while (iterator.MoveNext())
            {
                string key = (iterator.Current.Key ?? "").Trim();
                string value = (iterator.Current.Value ?? "").Trim();
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    arguList.Add(key + "=" + value);
                }
            }
            string baseString = string.Join("&", arguList.ToArray());

            //// 使用MD5对待签名串求签，基于UTF-8格式
            //MD5 md5 = MD5.Create();
            //byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(baseString));

            //// 将MD5输出的二进制结果转换为小写的十六进制
            //StringBuilder result = new StringBuilder();
            //for (int i = 0; i < bytes.Length; i++)
            //{
            //    string hex = bytes[i].ToString("x");
            //    if (hex.Length == 1)
            //    {
            //        result.Append("0");
            //    }
            //    result.Append(hex);
            //}

            //return result.ToString();
            return GetStrSignature(baseString).ToLower();
        }

        /// <summary>
        /// 使用MD5对待签名串求签
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetStrSignature(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "");
        }
        /// <summary>
        /// 封装加密参数字典
        /// </summary>
        /// <param name="token">客户端签名</param>
        /// <returns>sortedParams,sign,timeStamp,accountSource</returns>
        public static Tuple<IDictionary<string, string>, string, long, string> GetSecretParams(string token)
        {
            var secret = EncryptUtils.Base64Decode(token);
            string sign = string.Empty;
            string source = string.Empty;
            long timeStamp = 0;
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>();
            if (secret.IndexOf("timestamp=", StringComparison.Ordinal) >= 0 &&
                secret.IndexOf("nonce=", StringComparison.Ordinal) >= 0 &&
                secret.IndexOf("apptype=", StringComparison.Ordinal) >= 0)
            {
                var secretParams = secret.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in secretParams)
                {
                    var param = item.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    switch (param[0])
                    {
                        case "signature":
                            sign = param[1];
                            break;
                        case "timestamp":
                            sortedParams.Add(param[0], param[1]);
                            long.TryParse(param[1], out timeStamp);
                            break;
                        case "source":
                            source = param[1];
                            break;
                        default:
                            sortedParams.Add(param[0], param[1]);
                            break;
                    }
                }
                sortedParams.Add("secret1", Constants.SECRET1);
                sortedParams.Add("secret2", Constants.SECRET2);
            }
            return Tuple.Create(sortedParams, sign, timeStamp, source);
        }


        // <summary>
        /// 封装加密参数字典
        /// </summary>
        /// <param name="secretModel"></param>
        /// <returns></returns>
        public static IDictionary<string, string> GetSecretParams(SecretModels secretModel)
        {
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>();
            sortedParams.Add("timestamp", secretModel.timestamp.ToString());
            sortedParams.Add("secret1", Constants.SECRET1);
            sortedParams.Add("nonce", secretModel.nonce);
            sortedParams.Add("secret2", Constants.SECRET2);
            sortedParams.Add("apptype", secretModel.apptype);
            return sortedParams;

        }


        #endregion

        #region 计算文件的Hash值

        /// <summary>
        /// 计算文件MD5 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ComputeMd5(string fileName)
        {
            string hashMd5;
            using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                //计算文件的MD5值
                System.Security.Cryptography.MD5 calculator = System.Security.Cryptography.MD5.Create();
                Byte[] buffer = calculator.ComputeHash(fs);
                calculator.Clear();
                //将字节数组转换成十六进制的字符串形式
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var t in buffer)
                {
                    stringBuilder.Append(t.ToString("x2"));
                }
                hashMd5 = stringBuilder.ToString();
            }
            return hashMd5;
        }

        #endregion

    }
    /// <summary>
    /// 提供一组加密处理的公共函数。
    /// </summary>
    public static class EncryptUtils
    {
        //随机数 
        private static char[] constant =
        {
        '0','1','2','3','4','5','6','7','8','9',
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
        'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
        };

        /// <summary>
        /// 将指定的字符串进行MD5加密。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MD5Crypto(string str)
        {
            byte[] bytes = Encoding.Default.GetBytes(str);
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "");
        }
        #region 产生随机码
        /// <summary>
        /// 产生随机码
        /// </summary>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static string GenerateRandom(int Length)
        {
            StringBuilder newRandom = new StringBuilder(62);
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(62)]);
            }
            return newRandom.ToString();
        }
        #endregion


        #region UrlBase64转码
        /// <summary>
        /// urlbase64转码
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string Base64Code(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// base64解码
        /// </summary>
        /// <param name="codestr"></param>
        /// <returns></returns>
        public static string Base64Decode(string codestr)
        {
            byte[] outputb = Convert.FromBase64String(codestr);
            string orgStr = Encoding.UTF8.GetString(outputb);
            return orgStr;
        }

        #endregion

        private static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 获取当前实例的日期和时间的计时周期数(Ticks表示一百纳秒的一千万分之一)
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentTimeStepNumber()
        {
            var delta = DateTime.UtcNow - _unixEpoch;
            return (int)delta.TotalSeconds;
        }
        #region UTF8编码解码
        /// <summary>
        /// UTF8解码
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string UTF8Decode(string message)
        {
            if (string.IsNullOrEmpty(message))
                return message;
            byte[] encodebytes = Encoding.UTF8.GetBytes(message.Replace("%", ""));
            string utf8String = Encoding.UTF8.GetString(encodebytes);
            return utf8String;


        }

        /// <summary>
        /// 字符编码
        /// </summary>
        /// <param name="srcEndcoding">原编码格式</param>
        /// <param name="dstEndcoding">解码格式</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string StringEncode(string srcEndcoding, string dstEndcoding, string message)
        {
            if (string.IsNullOrEmpty(message))
                return message;
            Encoding srcEndcode = Encoding.GetEncoding(srcEndcoding);
            Encoding dstEndcode = Encoding.GetEncoding(dstEndcoding);
            byte[] srcBytes = srcEndcode.GetBytes(srcEndcoding);
            byte[] dscBytes = Encoding.Convert(srcEndcode, dstEndcode, srcBytes);
            return dstEndcode.GetString(dscBytes);
        }
        #endregion

        #region 判断字符串数据中是否存在某一字符
        public static bool CheckStringArray(string isexistsr, string str)
        {
            bool falg = false;
            if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(isexistsr))
            {
                List<string> stringList = new List<string>();
                stringList = str.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var item in stringList)
                {
                    if (item == isexistsr)
                    {
                        falg = true;
                        break;
                    }

                }
            }
            return falg;

        }
        #endregion
    }

    public enum AppPlatform
    {
        /// <summary>
        /// 安卓
        /// </summary>
        [Description("Android")]
        Android,

        /// <summary>
        /// iOS
        /// </summary>
        [Description("iOS")]
        iOS
    }
}
public class ServerTimeStampModels
{
    /// <summary>
    /// 时间戳
    /// </summary>
    public double TimeStamp { get; set; }
}

public class SecretModels
{
    /// <summary>
    /// 签名
    /// </summary>
    public string signature { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public string timestamp { get; set; }

    /// <summary>
    /// 随机字符串
    /// </summary>
    public string nonce { get; set; }

    /// <summary>
    /// 客户端来源
    /// </summary>
    public string apptype { get; set; }
}
