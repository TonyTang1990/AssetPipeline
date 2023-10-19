/*
 * Description:             ProcessorGlobalData.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/17
 */

using System;
using System.Collections.Generic;
using UnityEditor;

namespace TAssetPipeline
{
    /// <summary>
    /// ProcessorGlobalData.cs
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
        /// 处理器Asset路径列表(保存时刷新导出，Asset管线运行时用，和ProcessorList一一对应)
        /// </summary>
        [Header("处理器Asset路径列表")]
        public List<string> ProcessorAssetPathList = new List<string>();

        /// <summary>
        /// 处理器选择列表(只使用第一个)
        /// </summary>
        [NonSerialized]
        public List<BaseProcessor> ProcessorChosenList = new List<BaseProcessor>(1) { null };
    }
}
