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
        /// 处理器设置数据
        /// </summary>
        [Serializable]
        public class ProcessorSettingData
        {
            /// <summary>
            /// 处理器
            /// </summary>
            [Header("处理器")]
            public BaseProcessor Processor;

            /// <summary>
            /// 黑名单路径列表
            /// </summary>
            [Header("黑名单路径列表")]
            public List<string> BlackListFolderPathList;

            private ProcessorSettingData()
            {
                BlackListFolderPathList = new List<string>();
            }

            /// <summary>
            /// 带参构造函数
            /// </summary>
            /// <param name="processor"></param>
            public ProcessorSettingData(BaseProcessor processor)
            {
                Processor = processor;
                BlackListFolderPathList = new List<string>();
            }

            /// <summary>
            /// 添加黑名单列表
            /// </summary>
            /// <param name="folderPath"></param>
            /// <returns></returns>
            public bool AddBlackListFolderPath(string folderPath)
            {
                if(string.IsNullOrEmpty(folderPath))
                {
                    Debug.LogError($"不能允许添加空目录路径作为处理器黑名单路径!");
                    return false;
                }
                if(BlackListFolderPathList.Contains(folderPath))
                {
                    Debug.LogWarning($"添加重复的目录路径作为处理器黑名单路径,添加失败!");
                    return false;
                }
                BlackListFolderPathList.Add(folderPath);
                return true;
            }

            /// <summary>
            /// 移除指定索引的黑名单目录设置
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public bool RemoveBlackListByIndex(int index)
            {
                if(index < 0 || index >= BlackListFolderPathList.Count)
                {
                    Debug.LogError($"移除黑名单索引:{index}不在黑名单有效长度:{BlackListFolderPathList.Count}内,移除黑名单目录失败!");
                    return false;
                }
                BlackListFolderPathList.RemoveAt(index);
                return true;
            }

            /// <summary>
            /// 指定Asset路径是否在黑名单列表里
            /// </summary>
            /// <param name="assetPath"></param>
            /// <returns></returns>
            public bool IsInBlackList(string assetPath)
            {
                if(BlackListFolderPathList.Count == 0)
                {
                    return false;
                }
                foreach(var blackListFolderPath in BlackListFolderPathList)
                {
                    if(assetPath.StartsWith(blackListFolderPath))
                    {
                        AssetPipelineLog.Log(($"Asset:{assetPath}在处理器:{Processor.Name}的黑名单目录列表里!").WithColor(Color.gray));
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// 是否是有效处理Asset类型
            /// </summary>
            /// <param name="assetType"></param>
            /// <returns></returns>
            public bool IsValideAssetType(AssetType assetType)
            {
                return Processor.IsValideAssetType(assetType);
            }

            /// <summary>
            /// 是否是有效处理Asset管线处理类型
            /// </summary>
            /// <param name="assetProcessType"></param>
            /// <returns></returns>
            public bool IsValideAssetProcessType(AssetProcessType assetProcessType)
            {
                return Processor.IsValideAssetProcessType(assetProcessType);
            }

            /// <summary>
            /// 打印所有黑名单目录
            /// </summary>
            public void PrintAllBlackListFolder()
            {
                foreach (var blackListFolderPath in BlackListFolderPathList)
                {
                    AssetPipelineLog.Log($"黑名单目录:{blackListFolderPath}".WithColor(Color.yellow));
                }
            }
        }

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
            public string FolderPath;

            /// <summary>
            /// 处理器设置数据列表
            /// </summary>
            public List<ProcessorSettingData> ProcessorDataList;

            /// <summary>
            /// 处理器选择列表(只使用第一个)
            /// </summary>
            [NonSerialized]
            public List<BaseProcessor> ProcessorChosenList;

            /// <summary>
            /// 是否展开
            /// </summary>
            [NonSerialized]
            public bool IsUnFold = false;

            /// <summary>
            /// 处理器Icon列表
            /// </summary>
            public List<GUIContent> ProcessorIconList
            {
                get;
                private set;
            }

            /// <summary>
            /// Asset类型Icon映射Map<Asset类型, Asset Icon>
            /// </summary>
            private Dictionary<AssetType, GUIContent> mProcessorAssetIconMap;

            public ProcessorLocalData()
            {
                FolderPath = "";
                ProcessorDataList = new List<ProcessorSettingData>();
                ProcessorChosenList = new List<BaseProcessor>(1) { null };
                ProcessorIconList = new List<GUIContent>();
                mProcessorAssetIconMap = new Dictionary<AssetType, GUIContent>();
                IsUnFold = false;
            }

            public ProcessorLocalData(bool isUnfold)
            {
                FolderPath = "";
                ProcessorDataList = new List<ProcessorSettingData>();
                ProcessorChosenList = new List<BaseProcessor>(1) { null };
                ProcessorIconList = new List<GUIContent>();
                mProcessorAssetIconMap = new Dictionary<AssetType, GUIContent>();
                IsUnFold = isUnfold;
            }

            /// <summary>
            /// 更新处理器Icon列表
            /// </summary>
            public void UpdaterProcessorIcon()
            {
                mProcessorAssetIconMap.Clear();
                ProcessorIconList.Clear();
                foreach (var processorData in ProcessorDataList)
                {
                    if (processorData.Processor != null &&
                        !mProcessorAssetIconMap.ContainsKey(processorData.Processor.TargetAssetType))
                    {
                        var targetAssetType = processorData.Processor.TargetAssetType;
                        var assetIcon = AssetPipelineSystem.GetAssetIconByAssetType(targetAssetType);
                        mProcessorAssetIconMap.Add(targetAssetType, assetIcon);
                        ProcessorIconList.Add(assetIcon);
                    }
                }
            }

            /// <summary>
            /// 排序所有处理器
            /// </summary>
            public void SortAllData()
            {
                ProcessorDataList.Sort(AssetPipelineUtilities.SortProcessorData);
            }

            /// <summary>
            /// 是否包含指定处理器
            /// </summary>
            /// <param name="processor"></param>
            /// <returns></returns>
            public bool ContainProcessor(BaseProcessor processor)
            {
                var findProcessorData = ProcessorDataList.Find(delegate (ProcessorSettingData processorData)
                {
                    return processorData.Processor != null && processorData.Processor.AssetPath.Equals(processor.AssetPath);
                });
                return findProcessorData != null;
            }

            /// <summary>
            /// 添加处理器数据
            /// </summary>
            /// <param name="processor"></param>
            public bool AddProcessorData(BaseProcessor processor)
            {
                if (processor == null)
                {
                    Debug.LogError($"不允许添加空的处理器数据!");
                    return false;
                }
                var findProcessorData = ProcessorDataList.Find(delegate (ProcessorSettingData processorData)
                {
                    return processorData.Processor != null && processorData.Processor.AssetPath.Equals(processor.AssetPath);
                });
                if (findProcessorData != null)
                {
                    Debug.LogError($"不允许重复添加相同的处理器:{findProcessorData.Processor.AssetPath}数据!");
                    return false;
                }
                ProcessorSettingData processorData = new ProcessorSettingData(processor);
                ProcessorDataList.Add(processorData);
                ProcessorDataList.Sort(AssetPipelineUtilities.SortProcessorData);
                UpdaterProcessorIcon();
                return true;
            }

            /// <summary>
            /// 移除指定索引的处理器配置数据
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public bool RemoveProcessorDataByIndex(int index)
            {
                if (index < 0 || index >= ProcessorDataList.Count)
                {
                    Debug.LogError($"移除处理器数据索引:{index}不在处理器数据有效长度:{ProcessorDataList.Count}内,移除处理器数据失败!");
                    return false;
                }
                ProcessorDataList.RemoveAt(index);
                UpdaterProcessorIcon();
                return true;
            }

            /// <summary>
            /// 指定Asset路径是否在目标目录下
            /// </summary>
            /// <param name="assetPath"></param>
            /// <returns></returns>
            public bool IsInTargetFolder(string assetPath)
            {
                if (string.IsNullOrEmpty(FolderPath) || string.IsNullOrEmpty(assetPath))
                {
                    return false;
                }
                return assetPath.StartsWith(FolderPath);
            }
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
    }
}