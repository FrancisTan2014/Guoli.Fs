using System;
using System.Runtime.Remoting.Messaging;
using Guoli.Fs.Api.Utils;
using Guoli.Fs.Bll;
using Guoli.Fs.Model;
using Guoli.Utilities.Extensions;

namespace Guoli.Fs.WebApi.Utils
{
    /// <summary>
    /// 提供验证、管理登录状态的方法
    /// </summary>
    /// <author>FrancisTan</author>
    /// <since>2017-03-20</since>
    public static class LoginStatus
    {
        private static readonly SystemUserBll SysUserBll = new SystemUserBll();
        private static readonly ViewSystemUserBll ViewSysUserBll = new ViewSystemUserBll();

        /// <summary>
        /// 生成表示登录成功的令牌
        /// </summary>
        /// <param name="userId">登录用户Id</param>
        /// <param name="password">登录口令</param>
        /// <returns>表示登录令牌的字符串</returns>
        public static string GenerateLoginToken(int userId, string password)
        {
            return Token.Generate(userId, password);
        }

        /// <summary>
        /// 判断当前请求的用户是否已登录
        /// </summary>
        public static bool HasLogin(string token)
        {
            int userId;
            return HasLogin(token, out userId);
        }

        private static bool HasLogin(string tokenStr, out int userId)
        {
            userId = 0;

            var token = Token.Create(tokenStr);
            if (token == null)
            {
                return false;
            }

            // 验证时间戳
            var currTimestamp = DateTime.Now.Timestamp();
            if (currTimestamp - token.Timestamp > WebConfig.TokenExpireSeconds)
            {
                return false;
            }

            // 验证密码
            var user = SysUserBll.QuerySingle(token.UserId);
            if (user == null)
            {
                return false;
            }
            if (!token.IfPasswordEquals(user.Password))
            {
                return false;
            }

            userId = token.UserId;
            return true;
        }

        public static ViewSystemUser GetLoginUser()
        {
            var user = CallContext.GetData(Literals.LoginUserKey) as ViewSystemUser;
            if (user == null)
            {
                int userId;
                var token = CallContext.GetData(Literals.AppTokenName).ToString();
                if (!HasLogin(token, out userId))
                {
                    // do something
                }

                user = ViewSysUserBll.QuerySingle(userId);
                CallContext.SetData(Literals.LoginUserKey, user);
            }

            return user;
        }
        
        public static bool IsSuperAdminLogin()
        {
            return GetLoginUser().UserType == 0;
        }

        /// <summary>
        /// 提供对登录令牌的操作方法
        /// </summary>
        class Token
        {
            public int UserId { get; private set; }
            public string EncryptString { get; private set; }
            public long Timestamp { get; private set; }
            public string TokenString { get; private set; }

            /// <summary>
            /// 创建一个Token对象，若失败则返回null
            /// </summary>
            public static Token Create(string token)
            {
                if (string.IsNullOrEmpty(token))
                {
                    return null;
                }

                return Resolve(token);
            }

            /// <summary>
            /// 创建一个Token对象，若失败则返回null
            /// </summary>
            public static Token Create(int userId, string password)
            {
                var timestamp = DateTime.Now.Timestamp();
                var encryptStr = Generate(userId, password, timestamp);

                return new Token
                {
                    UserId = userId,
                    Timestamp = timestamp,
                    EncryptString = encryptStr,
                    TokenString = string.Join(":", userId, timestamp, encryptStr)
                };

            }

            /// <summary>
            /// 判断当前token对象中的密码与指定密码是否相同
            /// </summary>
            public bool IfPasswordEquals(string password)
            {
                var encryptPwd = EncryptPassword(password);
                return encryptPwd == EncryptString;
            }

            private Token()
            {

            }

            private Token(int userId, long timeStamp, string encryptStr, string token)
            {
                UserId = userId;
                Timestamp = timeStamp;
                EncryptString = encryptStr;
                TokenString = token;
            }

            private static Token Resolve(string token)
            {
                try
                {
                    var strs = token.Split(':');
                    if (strs.Length != 3)
                    {
                        return null;
                    }

                    var userId = strs[0].ToInt32();
                    if (userId <= 0)
                    {
                        return null;
                    }

                    var timestamp = Convert.ToInt64(strs[1]);
                    if (timestamp <= 0)
                    {
                        return null;
                    }

                    var encryptString = strs[2];

                    return new Token(userId, timestamp, encryptString, token);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public static string Generate(int userId, string password, long? timestamp = null)
            {
                var ts = timestamp ?? DateTime.Now.Timestamp();
                var encryptStr = EncryptPassword(password);

                return string.Join(":", userId, ts, encryptStr);
            }

            private static string EncryptPassword(string password)
            {
                var tokenRandomStr = WebConfig.TokenRandomStr;
                return (password + tokenRandomStr).GetMd5();
            }
        }
    }
}