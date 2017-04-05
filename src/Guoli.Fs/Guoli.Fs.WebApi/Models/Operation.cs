using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Guoli.Fs.WebApi.Models
{
    /// <summary>
    /// 描述对数据操作类型（如增加或者删除等）的枚举
    /// </summary>
    /// <author>FrancisTan</author>
    /// <since>2017-03-27</since>
    public enum Operation
    {
        Insert = 1,

        Update = 2,

        Delete = 3
    }
}