/*
 * Description:             BasePostCheckJson.cs
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
    /// BasePostCheckJson.cs
    /// Asset后检查器Json抽象类(所有后检查都应该继承此类)
    /// </summary>
    [Serializable]
    public abstract class BasePostCheckJson : BaseCheckJson
    {
    }
}
