/*
 * Description:             AssetProcessorGlobalDataJson.cs
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
    /// AssetProcessorGlobalDataJson.cs
    /// Asset处理器全局Json数据
    /// </summary>
    [Serializable]
    public class AssetProcessorGlobalDataJson
    {
        /// <summary>
        /// 预处理器数据
        /// </summary>
        public ProcessorGlobalData PreProcessorData = new ProcessorGlobalData();

        /// <summary>
        /// 后处理器数据
        /// </summary>
        public ProcessorGlobalData PostProcessorData = new ProcessorGlobalData();

        /// <summary>
        /// 移动处理器数据
        /// </summary>
        public ProcessorGlobalData MovedProcessorData = new ProcessorGlobalData();

        /// <summary>
        /// 删除处理器数据
        /// </summary>
        public ProcessorGlobalData DeletedProcessorData = new ProcessorGlobalData();
    }
}
