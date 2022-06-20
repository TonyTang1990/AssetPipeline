/*
 * Description:             AssetProcessorLocalData.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/17
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetProcessorLocalData.cs
    /// Asset处理器局部数据
    /// </summary>
    public class AssetProcessorLocalData : ScriptableObject
    {
        /// <summary>
        /// Asset处理器局部配置数据
        /// </summary>
        public class ProcessorLocalConfigData
        {
            /// <summary>
            /// 目录设置
            /// </summary>
            [Header("目录设置")]
            public string FolderPath;

            /// <summary>
            /// 处理器设置列表
            /// </summary>
            public List<BaseProcessor> ProcessorList;
        }

        /// <summary>
        /// 局部预处理器列表
        /// </summary>
        [Header("局部预处理器列表")]
        public List<ProcessorLocalConfigData> PreProcessorLocalList;

        /// <summary>
        /// 局部后处理器列表
        /// </summary>
        [Header("局部后处理器列表")]
        public List<ProcessorLocalConfigData> PostProcessorLocalList;

        /// <summary>
        /// 局部移动处理器列表
        /// </summary>
        [Header("局部移动处理器列表")]
        public List<ProcessorLocalConfigData> MovedProcessorLocalList;

        /// <summary>
        /// 局部删除处理器列表
        /// </summary>
        [Header("局部删除处理器列表")]
        public List<ProcessorLocalConfigData> DeletedProcessorLocalList;
    }
}