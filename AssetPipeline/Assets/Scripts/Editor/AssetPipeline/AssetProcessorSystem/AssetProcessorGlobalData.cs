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
        /// 预处理器列表
        /// </summary>
        [Header("预处理器列表")]
        public List<BaseProcessor> PreProcessorList;

        /// <summary>
        /// 后处理器列表
        /// </summary>
        [Header("后处理器列表")]
        public List<BaseProcessor> PostProcessorList;

        /// <summary>
        /// 移动处理器列表
        /// </summary>
        [Header("移动处理器列表")]
        public List<BaseProcessor> MovedProcessorList;

        /// <summary>
        /// 删除处理器列表
        /// </summary>
        [Header("删除处理器列表")]
        public List<BaseProcessor> DeletedProcessorList;

        /// <summary>
        /// 预处理器选择列表(只使用第一个)
        /// </summary>
        [NonSerialized]
        public List<BaseProcessor> PreProcessorChosenList = new List<BaseProcessor>(1) { null };

        /// <summary>
        /// 后处理器选择列表(只使用第一个)
        /// </summary>
        [NonSerialized]
        public List<BaseProcessor> PostProcessorChosenList = new List<BaseProcessor>(1) { null };

        /// <summary>
        /// 移动处理器选择列表(只使用第一个)
        /// </summary>
        [NonSerialized]
        public List<BaseProcessor> MovedProcessorChosenList = new List<BaseProcessor>(1) { null };

        /// <summary>
        /// 删除处理器选择列表(只使用第一个)
        /// </summary>
        [NonSerialized]
        public List<BaseProcessor> DeletedProcessorChosenList = new List<BaseProcessor>(1) { null };
    }
}