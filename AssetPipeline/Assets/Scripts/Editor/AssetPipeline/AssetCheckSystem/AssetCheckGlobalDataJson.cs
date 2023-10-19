/*
 * Description:             AssetCheckGlobalDataJson.cs
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
    /// AssetCheckGlobalData.cs
    /// Asset检查全局Json数据
    /// </summary>
    [Serializable]
    public class AssetCheckGlobalDataJson
    {
        /// <summary>
        /// 预检查器数据
        /// </summary>
        public CheckGlobalData PreCheckData = new CheckGlobalData();

        /// <summary>
        /// 后检查器数据
        /// </summary>
        public CheckGlobalData PostCheckData = new CheckGlobalData();
    }
}
