/*
 * Description:             BasePostProcessorJson.cs
 * Author:                  TONYTANG
 * Create Date:             2023/10/19
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAssetPipeline
{
    /// <summary>
    /// BasePostProcessorJson.cs
    /// Asset后处理器Json抽象类(所有预处理器都应该继承此类)
    /// </summary>
    [Serializable]
    public abstract class BasePostProcessorJson : BasePostProcessorJson
    {
    }
}
