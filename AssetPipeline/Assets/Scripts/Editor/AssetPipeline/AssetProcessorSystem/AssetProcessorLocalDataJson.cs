/*
 * Description:             AssetProcessorLocalDataJson.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/17
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetProcessorLocalDataJson.cs
    /// Asset处理器局部Json数据
    /// </summary>
    [Serializable]
    public class AssetProcessorLocalDataJson
    {
        /// <summary>
        /// 局部预处理器数据
        /// </summary>
        [Header("局部预处理器数据")]
        public List<ProcessorLocalData> PreProcessorDataList = new List<ProcessorLocalData>();

        /// <summary>
        /// 局部后处理器数据
        /// </summary>
        [Header("局部后处理器数据")]
        public List<ProcessorLocalData> PostProcessorDataList = new List<ProcessorLocalData>();

        /// <summary>
        /// 局部移动处理器数据
        /// </summary>
        [Header("局部移动处理器数据")]
        public List<ProcessorLocalData> MovedProcessorDataList = new List<ProcessorLocalData>();

        /// <summary>
        /// 局部删除处理器数据
        /// </summary>
        [Header("局部删除处理器数据")]
        public List<ProcessorLocalData> DeletedProcessorDataList = new List<ProcessorLocalData>();
    }
}
