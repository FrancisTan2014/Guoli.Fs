using System;
using System.Configuration;

namespace Guoli.Fs.Api.Utils
{
    /// <summary>
    /// 提供对WebConfig中字面值的获取
    /// </summary>
    /// <author>FrancisTan</author>
    /// <since>2017-03-17</since>
    public static class WebConfig
    {
        public static string GetAppSetting(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return ConfigurationManager.AppSettings?[name];
        }

        public static string GetConnString(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
        }

        public static string FileSystemDbConnStr => GetConnString("FileSystemDbConnStr");

        public static string TokenRandomStr => GetAppSetting("TokenRandomStr");

        public static long TokenExpireSeconds => Convert.ToInt64(GetAppSetting("TokenExpireSeconds"));
    }
}