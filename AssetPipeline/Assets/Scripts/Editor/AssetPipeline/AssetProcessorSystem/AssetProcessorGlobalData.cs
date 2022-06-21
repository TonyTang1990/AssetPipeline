/*
 * Description:             AssetProcessorGlobalData.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/17
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetProcessorGlobalData.cs
    /// Asset处理器全局数据
    /// </summary>
    public class AssetProcessorGlobalData : ScriptableObject
    {
        /// <summary>
        /// Asset处理器全局数据
        /// </summary>
        [Serializable]
        public class ProcessorGlobalData
        {
            /// <summary>
            /// 处理器设置列表
            /// </summary>
            public List<BaseProcessor> ProcessorList = new List<BaseProcessor>();

            /// <summary>
            /// 处理器选择列表(只使用第一个)
            /// </summary>
            [NonSerialized]
            public List<BaseProcessor> ProcessorChosenList = new List<BaseProcessor>(1) { null };
        }

        /// <summary>
        /// 预处理器数据
        /// </summary>
        [Header("预处理器数据")]
        public ProcessorGlobalData PreProcessorData = new ProcessorGlobalData();

        /// <summary>
        /// 后处理器数据
        /// </summary>
        [Header("后处理器数据")]
        public ProcessorGlobalData PostProcessorData = new ProcessorGlobalData();

        /// <summary>
        /// 移动处理器数据
        /// </summary>
        [Header("移动处理器数据")]
        public ProcessorGlobalData MovedProcessorData = new ProcessorGlobalData();

        /// <summary>
        /// 删除处理器数据
        /// </summary>
        [Header("删除处理器数据")]
        public ProcessorGlobalData DeletedProcessorData = new ProcessorGlobalData();
    }
}