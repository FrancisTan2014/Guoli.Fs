using System.Net;

namespace Guoli.Fs.Api.Models
{
    /// <summary>
    /// 封装接口返回信息，包括状态码、消息及返回数据等
    /// </summary>
    public class ApiReturns
    {
        public int code { get; set; }
        public string msg { get; set; }
        public object data { get; set; }

        /// <summary>
        /// 200，数据请求成功
        /// </summary>
        public static ApiReturns Ok(object data)
        {
            return new ApiReturns
            {
                code = (int)HttpStatusCode.OK,
                msg = "数据请求成功",
                data = data
            };
        }

        /// <summary>
        /// 201，数据添加或修改成功
        /// </summary>
        public static ApiReturns Created(object data = null)
        {
            return new ApiReturns
            {
                code = (int)HttpStatusCode.Created,
                msg = "数据添加或修改成功",
                data = data
            };
        }

        /// <summary>
        /// 400，数据添加或更新失败
        /// </summary>
        public static ApiReturns BadRequest(object data = null)
        {
            return new ApiReturns
            {
                code = (int)HttpStatusCode.BadRequest,
                msg = "数据添加或更新失败",
                data = data
            };
        }

        /// <summary>
        /// 204，数据删除成功
        /// </summary>
        public static ApiReturns NoContent()
        {
            return new ApiReturns
            {
                code = (int)HttpStatusCode.NoContent,
                msg = "数据删除成功"
            };
        }

        /// <summary>
        /// 401，用户名密码错误
        /// </summary>
        public static ApiReturns Unauthorized()
        {
            return new ApiReturns
            {
                code = (int)HttpStatusCode.Unauthorized,
                msg = "用户名密码错误"
            };
        }

        /// <summary>
        /// 403，禁止访问或者无操作权限
        /// </summary>
        public static ApiReturns Forbidden()
        {
            return new ApiReturns
            {
                code = (int)HttpStatusCode.Forbidden,
                msg = "禁止访问"
            };
        }

        /// <summary>
        /// 40317，未登录，或者登录令牌已到过期时间
        /// </summary>
        public static ApiReturns TokenExpired()
        {
            return new ApiReturns
            {
                code = 40317,
                msg = "未登录或者登录已过期"
            };
        }

        /// <summary>
        /// 404，要删除的数据不存在
        /// </summary>
        public static ApiReturns NotFound()
        {
            return new ApiReturns
            {
                code = (int)HttpStatusCode.NotFound,
                msg = "要删除的数据不存在"
            };
        }

        /// <summary>
        /// 60011，存在相同项
        /// </summary>
        public static ApiReturns Exists()
        {
            return new ApiReturns
            {
                code = 60011,
                msg = "存在相同项"
            };
        }

        /// <summary>
        /// 60012，数据库操作失败
        /// </summary>
        public static ApiReturns Failed()
        {
            return new ApiReturns
            {
                code = 60012,
                msg = "数据库操作失败"
            };
        }

        /// <summary>
        /// 415，不支持的文件类型
        /// </summary>
        public static ApiReturns UnsupportedMediaType()
        {
            return new ApiReturns
            {
                code = (int)HttpStatusCode.UnsupportedMediaType,
                msg = "不支持的文件类型"
            };
        }
    }
}