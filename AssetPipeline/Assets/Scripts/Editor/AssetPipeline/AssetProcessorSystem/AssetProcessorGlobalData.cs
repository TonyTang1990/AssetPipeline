/*
 * Description:             AssetProcessorGlobalData.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/17
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetProcessorGlobalData.cs
    /// Asset处理器全局数据
    /// </summary>
    [CreateAssetMenu(fileName = "AssetProcessorGlobalData", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/AssetProcessorGlobalData", order = 1)]
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
            /// 处理器器Asset路径列表(保存时刷新导出，Asset管线运行时用，和ProcessorList一一对应)
            /// </summary>
            [Header("处理器器Asset路径列表")]
            public List<string> ProcessorAssetPathList = new List<string>();

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

        /// <summary>
        /// 排序所有数据
        /// </summary>
        public void SortAllData()
        {
            PreProcessorData.ProcessorList.Sort(AssetPipelineUtilities.SortProcessor);
            PostProcessorData.ProcessorList.Sort(AssetPipelineUtilities.SortProcessor);
            MovedProcessorData.ProcessorList.Sort(AssetPipelineUtilities.SortProcessor);
            DeletedProcessorData.ProcessorList.Sort(AssetPipelineUtilities.SortProcessor);
        }

        /// <summary>
        /// 刷新成员值
        /// </summary>
        public void RefreshMemberValue()
        {
            RefreshMemberValueByGlobalData(PreProcessorData);
            RefreshMemberValueByGlobalData(PostProcessorData);
            RefreshMemberValueByGlobalData(MovedProcessorData);
            RefreshMemberValueByGlobalData(DeletedProcessorData);
        }

        /// <summary>
        /// 刷新指定处理器数据列表成员值
        /// </summary>
        /// <param name="processorGlobalData"></param>
        private void RefreshMemberValueByGlobalData(ProcessorGlobalData processorGlobalData)
        {
            processorGlobalData.ProcessorAssetPathList.Clear();
            foreach (var processor in processorGlobalData.ProcessorList)
            {
                string processorAssetPath = null;
                if (processor != null)
                {
                    processorAssetPath = AssetDatabase.GetAssetPath(processor);
                    if (string.IsNullOrEmpty(processorAssetPath))
                    {
                        Debug.LogError($"找不到处理器:{processor.name}的Asset路径!");
                    }
                }
                processorGlobalData.ProcessorAssetPathList.Add(processorAssetPath);
            }
        }
    }
}