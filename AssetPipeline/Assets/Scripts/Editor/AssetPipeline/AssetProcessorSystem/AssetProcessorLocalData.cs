/*
 * Description:             AssetProcessorLocalData.cs
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
    /// AssetProcessorLocalData.cs
    /// Asset处理器局部数据
    /// </summary>
    public class AssetProcessorLocalData : ScriptableObject
    {
        /// <summary>
        /// Asset处理器局部数据
        /// </summary>
        [Serializable]
        public class ProcessorLocalData
        {
            /// <summary>
            /// 目录路径
            /// </summary>
            [Header("目录路径")]
            public string FolderPath = "Assets/";

            /// <summary>
            /// 处理器设置列表
            /// </summary>
            public List<BaseProcessor> ProcessorList = new List<BaseProcessor>();

            /// <summary>
            /// 处理器选择列表(只使用第一个)
            /// </summary>
            [NonSerialized]
            public List<BaseProcessor> ProcessorChosenList = new List<BaseProcessor>(1) { null };

            /// <summary>
            /// 是否展开
            /// </summary>
            [NonSerialized]
            public bool IsUnFold = false;
        }

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