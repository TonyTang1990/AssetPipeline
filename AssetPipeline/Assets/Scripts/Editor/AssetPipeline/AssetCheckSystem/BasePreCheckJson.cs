/*
 * Description:             BasePreCheckJson.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/17
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAssetPipeline
{
    /// <summary>
    /// BasePreCheckJson.cs
    /// Asset预检查器Json抽象类(所有预检查都应该继承此类)
    /// </summary>
    [Serializable]
    public abstract class BasePreCheckJson : BaseCheckJson
    {
    }
}
