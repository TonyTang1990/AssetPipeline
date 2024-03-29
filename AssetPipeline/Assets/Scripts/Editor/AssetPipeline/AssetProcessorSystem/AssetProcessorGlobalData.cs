﻿/*
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
        /// 检查是否有无效处理器配置
        /// </summary>
        public void CheckInvalideProcessorConfigs()
        {
            if (PreProcessorData.CheckInvalideProcessorConfig())
            {
                Debug.LogError("全局预处理器有无效处理器配置！");
            }
            if (PostProcessorData.CheckInvalideProcessorConfig())
            {
                Debug.LogError("全局后处理器有无效处理器配置！");
            }
            if (MovedProcessorData.CheckInvalideProcessorConfig())
            {
                Debug.LogError("全局移动处理器有无效处理器配置！");
            }
            if (DeletedProcessorData.CheckInvalideProcessorConfig())
            {
                Debug.LogError("全局删除处理器有无效处理器配置！");
            }
        }

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