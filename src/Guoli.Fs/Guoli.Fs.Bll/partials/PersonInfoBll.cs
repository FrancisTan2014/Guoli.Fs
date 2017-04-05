/*
 * 本文件使用代码生成器生成，随时有可能被更改
 * 若要添加新逻辑请在其他地方新建partial类
 */

using Guoli.Fs.Model;

namespace Guoli.Fs.Bll 
{
	/// <summary>
    /// PersonInfo表的业务逻辑层实现
    /// </summary>
    /// <remarks>动软生成于2017-03-22</remarks>
	public partial class PersonInfoBll : BaseBll<PersonInfo>
	{
		public override PersonInfo QuerySingle(int id)
	    {
	        return QuerySingle(f => f.Id == id);
	    }
	}
}