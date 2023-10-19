/*
 * Description:             AssetCheckLocalDataJson.cs
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
    /// AssetCheckConfigData.cs
    /// Asset检查局部Json数据
    /// </summary>
    [Serializable]
    public class AssetCheckLocalDataJson
    {
        /// <summary>
        /// 局部预检查器数据
        /// </summary>
        public List<CheckLocalData> PreCheckDataList = new List<CheckLocalData>();

        /// <summary>
        /// 局部后检查器数据
        /// </summary>
        public List<CheckLocalData> PostCheckDataList = new List<CheckLocalData>();
    }
}
