/*
 * Description:             AssetProcessorLocalData.cs
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
    /// AssetProcessorLocalData.cs
    /// Asset处理器局部数据
    /// </summary>
    [CreateAssetMenu(fileName = "AssetProcessorLocalData", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/AssetProcessorLocalData", order = 2)]
    public class AssetProcessorLocalData : ScriptableObject
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

        /// <summary>
        /// 更新所有处理器数据的Icon信息
        /// </summary>
        public void UpdateAllProcessorIconDatas()
        {
            foreach(var preProcessorData in PreProcessorDataList)
            {
                preProcessorData.UpdaterProcessorIcon();
            }
            foreach (var postProcessorData in PostProcessorDataList)
            {
                postProcessorData.UpdaterProcessorIcon();
            }
            foreach (var movedProcessorData in MovedProcessorDataList)
            {
                movedProcessorData.UpdaterProcessorIcon();
            }
            foreach (var deletedProcessorData in DeletedProcessorDataList)
            {
                deletedProcessorData.UpdaterProcessorIcon();
            }
        }

        /// <summary>
        /// 排序所有数据
        /// </summary>
        public void SortAllData()
        {
            foreach (var preProcessorData in PreProcessorDataList)
            {
                preProcessorData.SortAllData();
            }
            foreach (var postProcessorData in PostProcessorDataList)
            {
                postProcessorData.SortAllData();
            }
            foreach (var movedProcessorData in MovedProcessorDataList)
            {
                movedProcessorData.SortAllData();
            }
            foreach (var deletedProcessorData in DeletedProcessorDataList)
            {
                deletedProcessorData.SortAllData();
            }
        }

        /// <summary>
        /// 刷新成员值
        /// </summary>
        public void RefreshMemberValue()
        {
            RefreshMemberValueByLocalDataList(PreProcessorDataList);
            RefreshMemberValueByLocalDataList(PostProcessorDataList);
            RefreshMemberValueByLocalDataList(MovedProcessorDataList);
            RefreshMemberValueByLocalDataList(DeletedProcessorDataList);
        }

        /// <summary>
        /// 刷新指定处理器数据列表成员值
        /// </summary>
        /// <param name="processorDataList"></param>
        private void RefreshMemberValueByLocalDataList(List<ProcessorLocalData> processorDataList)
        {
            foreach (var processorData in processorDataList)
            {
                foreach (var processorSettingData in processorData.ProcessorDataList)
                {
                    string processorAssetPath = null;
                    if (processorSettingData.Processor != null)
                    {
                        processorAssetPath = AssetDatabase.GetAssetPath(processorSettingData.Processor);
                        if(string.IsNullOrEmpty(processorAssetPath))
                        {
                            Debug.LogError($"找不到处理器:{processorSettingData.Processor.name}的Asset路径!");
                        }
                    }
                    processorSettingData.ProcessorAssetPath = processorAssetPath;
                }
            }
        }
    }
}