using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace Guoli.Fs.WebApi.Models
{
    /// <summary>
    /// 分页获取数据的参数设置
    /// </summary>
    public class GetPageParams
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int page { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int size { get; set; }

        public JObject conditions { get; set; }
    }
}