/*
 * Description:             AssetProcessorSystem.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/17
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static TAssetPipeline.AssetProcessorLocalData;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetProcessorSystem.cs
    /// Asset处理系统
    /// </summary>
    public static class AssetProcessorSystem
    {
        /// <summary>
        /// Asset处理器全局数据文件名
        /// </summary>
        private const string AssetProcessorGlobalDataName = "AssetProcessorGlobalData";

        /// <summary>
        /// Asset处理器局部数据文件名
        /// </summary>
        private const string AssetProcessorLocalDataName = "AssetProcessorLocalData";

        /// <summary>
        /// Asset处理器全局数据
        /// </summary>
        private static AssetProcessorGlobalData GlobalData;

        /// <summary>
        /// Asset处理器局部数据
        /// </summary>
        private static AssetProcessorLocalData LocalData;

        /// <summary>
        /// 所有预处理器列表
        /// </summary>
        private static List<BasePreProcessor> AllPreProcessors;

        /// <summary>
        /// 所有后处理器列表
        /// </summary>
        private static List<BasePostProcessor> AllPostProcessors;

        /// <summary>
        /// 全局预处理器映射Map<目标Asset管线处理类型, <目标Asset类型, 处理器列表>>
        /// </summary>
        private static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessor>>> GlobalPreProcessorMap;

        /// <summary>
        /// 全局后处理器映射Map<目标Asset管线处理类型, <目标Asset类型, 处理器列表>>
        /// </summary>
        private static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessor>>> GlobalPostProcessorMap;

        /// <summary>
        /// 全局移动处理器映射Map<目标Asset管线处理类型, <目标Asset类型, 处理器列表>>
        /// </summary>
        private static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessor>>> GlobalMovedProcessorMap;

        /// <summary>
        /// 全局删除处理器映射Map<目标Asset管线处理类型, <目标Asset类型, 处理器列表>>
        /// </summary>
        private static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessor>>> GlobalDeletedProcessorMap;

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            Debug.Log($"Asset处理器系统初始化".WithColor(Color.red));
            MakeSureProcessorSaveFolderExit();
            MakeSureStrategyFolderExist();
            var activeStrategyName = AssetPipelineSystem.GetActiveTargetStrategyName();
            GlobalData = LoadGlobalDataByStrategy(activeStrategyName);
            LocalData = LoadLocalDataByStrategy(activeStrategyName);
            InitAllProcessors();
            InitGlobalProcessorData();
            InitLocalProcessorData();
        }

        /// <summary>
        /// 确保自定义处理器目录存在
        /// </summary>
        private static void MakeSureProcessorSaveFolderExit()
        {
            var processorSaveRelativePath = GetProcessorSaveRelativePath();
            var processorSaveFullPath = Path.GetFullPath(processorSaveRelativePath);
            FolderUtilities.CheckAndCreateSpecificFolder(processorSaveFullPath);
        }

        /// <summary>
        /// 确保激活平台配置策略目录存在
        /// </summary>
        private static void MakeSureStrategyFolderExist()
        {
            var activeStrategyName = AssetPipelineSystem.GetActiveTargetStrategyName();
            MakeSureStrategyFolderExistByStrategy(activeStrategyName);
        }

        /// <summary>
        /// 初始化所有处理器
        /// </summary>
        private static void InitAllProcessors()
        {
            AllPreProcessors = GetAllProcessors<BasePreProcessor>();
            AllPostProcessors = GetAllProcessors<BasePostProcessor>();
        }

        /// <summary>
        /// 获取所有指定类型的处理器
        /// </summary>
        /// <returns></returns>
        public static List<T> GetAllProcessors<T>() where T : BaseProcessor
        {
            List<T> allProcessorList = new List<T>();
            var targetProcessorType = typeof(T);
            var processorSaveRelativePath = GetProcessorSaveRelativePath();
            var allProcessorsGuids = AssetDatabase.FindAssets($"t:{targetProcessorType}", new[] { processorSaveRelativePath });
            foreach(var processGuid in allProcessorsGuids)
            {
                var processorAssetPath = AssetDatabase.GUIDToAssetPath(processGuid);
                var processorAsset = AssetDatabase.LoadAssetAtPath<T>(processorAssetPath);
                allProcessorList.Add(processorAsset);
            }
            allProcessorList.Sort(AssetPipelineUtilities.SortProcessor);
            return allProcessorList;
        }

        /// <summary>
        /// 初始化所有全局处理器信息
        /// </summary>
        private static void InitGlobalProcessorData()
        {
            GlobalPreProcessorMap = GetGlobalPreProcessorMap();
            GlobalPostProcessorMap = GetGlobalPostProcessorMap();
            GlobalMovedProcessorMap = GetGlobalMovedProcessorMap();
            GlobalDeletedProcessorMap = GetGlobalDeletedProcessorMap();
        }

        /// <summary>
        /// 获取全局预处理器Map<Asset管线处理类型个, <目标Asset类型, 处理器列表>>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessor>>> GetGlobalPreProcessorMap()
        {
            return GetGlobalProcessorMap(GlobalData.PreProcessorData.ProcessorList, "预");
        }

        /// <summary>
        /// 获取全局后处理器Map<Asset管线处理类型, <目标Asset类型, 处理器列表>>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessor>>> GetGlobalPostProcessorMap()
        {
            return GetGlobalProcessorMap(GlobalData.PostProcessorData.ProcessorList, "后");
        }

        /// <summary>
        /// 获取全局移动处理器Map<Asset管线处理类型, <目标Asset类型, 处理器列表>>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessor>>> GetGlobalMovedProcessorMap()
        {
            return GetGlobalProcessorMap(GlobalData.MovedProcessorData.ProcessorList, "移动");
        }

        /// <summary>
        /// 获取全局删除处理器Map<Asset管线处理类型, <目标Asset类型, 处理器列表>>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessor>>> GetGlobalDeletedProcessorMap()
        {
            return GetGlobalProcessorMap(GlobalData.DeletedProcessorData.ProcessorList, "删除");
        }

        /// <summary>
        /// 获取全局指定处理器的Map<Asset管线处理类型, <目标Asset类型, 处理器列表>>
        /// </summary>
        /// <param name="processorList"></param>
        /// <param name="tip""/></param>
        /// <returns></returns>
        private static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessor>>> GetGlobalProcessorMap(List<BaseProcessor> processorList, string tip = "")
        {
            Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessor>>> globalProcessorMap = new Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessor>>>();
            foreach(var processor in processorList)
            {
                if(processor != null)
                {
                    Dictionary<AssetType, List<BaseProcessor>> assetTypeProcessorMap;
                    if(!globalProcessorMap.TryGetValue(processor.TargetAssetProcessType, out assetTypeProcessorMap))
                    {
                        assetTypeProcessorMap = new Dictionary<AssetType, List<BaseProcessor>>();
                        globalProcessorMap.Add(processor.TargetAssetProcessType, assetTypeProcessorMap);
                    }
                    foreach (var assetTypeValue in AssetPipelineConst.ASSET_TYPE_VALUES)
                    {
                        var assetType = (AssetType)assetTypeValue;
                        if (assetType == AssetType.All)
                        {
                            continue;
                        }
                        if((processor.TargetAssetType & assetType) != AssetType.None)
                        {
                            List<BaseProcessor> assetTypeProcessorList;
                            if (!assetTypeProcessorMap.TryGetValue(assetType, out assetTypeProcessorList))
                            {
                                assetTypeProcessorList = new List<BaseProcessor>();
                                assetTypeProcessorMap.Add(assetType, assetTypeProcessorList);
                            }
                            assetTypeProcessorList.Add(processor);
                            AssetPipelineLog.Log($"添加Asset管线处理类型:{processor.TargetAssetProcessType},Asset类型:{assetType}的全局{tip}处理器:{processor.Name}!".WithColor(Color.yellow));
                        }
                    }
                }
            }
            return globalProcessorMap;
        }

        /// <summary>
        /// 初始化所有局部处理器信息
        /// </summary>
        private static void InitLocalProcessorData()
        {
            PrintProcessorLocalData(LocalData.PreProcessorDataList, "预");
            PrintProcessorLocalData(LocalData.PostProcessorDataList, "后");
            PrintProcessorLocalData(LocalData.MovedProcessorDataList, "移动");
            PrintProcessorLocalData(LocalData.DeletedProcessorDataList, "删除");
        }

        /// <summary>
        /// 打印局部处理器数据列表
        /// </summary>
        /// <param name="processorLocalDataList"></param>
        /// <param name="tip"></param>
        private static void PrintProcessorLocalData(List<ProcessorLocalData> processorLocalDataList, string tip = "")
        {
            foreach(var processorLocalData in processorLocalDataList)
            {
                foreach (var processorData in processorLocalData.ProcessorDataList)
                {
                    if(processorData.Processor != null)
                    {
                        AssetPipelineLog.Log($"局部{tip}处理器目标目录:{processorLocalData.FolderPath},Asset管线处理类型:{processorData.Processor.TargetAssetProcessType},添加局部处理器:{processorData.Processor.Name}!".WithColor(Color.yellow));
                        processorData.PrintAllBlackListFolder();
                    }
                }
            }
        }

        /// <summary>
        /// 获取指定Asset处理器Map里的指定Asset类型的处理器列表
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="processorMap"></param>
        /// <returns></returns>
        private static List<BaseProcessor> GetProcessorListByAssetType(AssetProcessType assetProcessType, AssetType assetType,
                                                                        Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessor>>> processorMap)
        {
            if(processorMap == null)
            {
                return null;
            }
            Dictionary<AssetType, List<BaseProcessor>> assetTypeProcessorMap;
            if(!processorMap.TryGetValue(assetProcessType, out assetTypeProcessorMap))
            {
                return null;
            }
            List<BaseProcessor> processorList;
            if(!assetTypeProcessorMap.TryGetValue(assetType, out processorList))
            {
                return null;
            }
            return processorList;
        }

        /// <summary>
        /// 获取自定义Asset处理器存储相对路径
        /// </summary>
        /// <returns></returns>
        private static string GetProcessorSaveRelativePath()
        {
            var saveFolderRelativePath = AssetPipelineSystem.GetSaveFolderRelativePath();
            return PathUtilities.GetRegularPath($"{saveFolderRelativePath}/AssetProcessors");
        }

        /// <summary>
        /// 获取指定策略的数据保存相对路径
        /// </summary>
        /// <param name="strategyName"></param>
        /// <returns></returns>
        public static string GetDataSaveFolderByStrategy(string strategyName)
        {
            var strategyDataFolderRelativePath = $"{AssetPipelineSystem.GetSaveFolderRelativePath()}/{strategyName}/AssetProcessor";
            return PathUtilities.GetRegularPath(strategyDataFolderRelativePath);
        }

        /// <summary>
        /// 获取Asset处理器全局数据Asset名
        /// </summary>
        /// <returns></returns>
        public static string GetGlobalDataAssetName()
        {
            return $"{AssetProcessorGlobalDataName}.asset";
        }

        /// <summary>
        /// 获取Asset处理器局部数据Asset名
        /// </summary>
        /// <returns></returns>
        public static string GetLocalDataAssetName()
        {
            return $"{AssetProcessorLocalDataName}.asset";
        }

        /// <summary>
        /// 获取指定策略的Asset处理器局部数据Asset名
        /// </summary>
        /// <param name="strategyName"></param>
        /// <returns></returns>
        private static string GetGlobalDataRelativePathByStartegy(string strategyName)
        {
            var saveFolderRelativePath = GetDataSaveFolderByStrategy(strategyName);
            return $"{saveFolderRelativePath}/{GetGlobalDataAssetName()}";
        }

        /// <summary>
        /// 获取指定策略的Asset处理器局部数据相对路径
        /// </summary>
        /// <param name="strategyName"></param>
        /// <returns></returns>
        private static string GetLocalDataRelativePathByStrategy(string strategyName)
        {
            var saveFolderRelativePath = GetDataSaveFolderByStrategy(strategyName);
            return $"{saveFolderRelativePath}/{GetLocalDataAssetName()}";
        }

        /// <summary>
        /// 确保指定策略目录存在
        /// </summary>
        /// <param name="strategyName"></param>
        public static void MakeSureStrategyFolderExistByStrategy(string strategyName)
        {
            var strategyDataFolderRelativePath = GetDataSaveFolderByStrategy(strategyName);
            var strategyDataFolderFullPath = Path.GetFullPath(strategyDataFolderRelativePath);
            FolderUtilities.CheckAndCreateSpecificFolder(strategyDataFolderFullPath);
        }

        /// <summary>
        /// 加载当前激活平台Asset处理器全局数据
        /// </summary>
        /// <param name="strategyName"></param>
        /// <returns></returns>
        public static AssetProcessorGlobalData LoadGlobalDataByStrategy(string strategyName)
        {
            var globalDataRelativePath = GetGlobalDataRelativePathByStartegy(strategyName);
            var globalData = AssetDatabase.LoadAssetAtPath<AssetProcessorGlobalData>(globalDataRelativePath);
            if (globalData == null)
            {
                Debug.Log($"创建新的Asset处理器全局数据!".WithColor(Color.green));
                globalData = ScriptableObject.CreateInstance<AssetProcessorGlobalData>();
                AssetDatabase.CreateAsset(globalData, globalDataRelativePath);
            }
            return globalData;
        }

        /// <summary>
        /// 加载当前激活平台Asset处理器局部数据
        /// </summary>
        /// <param name="strategyName"></param>
        /// <returns></returns>
        public static AssetProcessorLocalData LoadLocalDataByStrategy(string strategyName)
        {
            var localDataRelativePath = GetLocalDataRelativePathByStrategy(strategyName);
            var localData = AssetDatabase.LoadAssetAtPath<AssetProcessorLocalData>(localDataRelativePath);
            if (localData == null)
            {
                Debug.Log($"创建新的Asset处理器局部数据!".WithColor(Color.green));
                localData = ScriptableObject.CreateInstance<AssetProcessorLocalData>();
                AssetDatabase.CreateAsset(localData, localDataRelativePath);
            }
            return localData;
        }

        #region Asset管线处理器处理部分
        // 处理器触发规则:
        // 1. 优先全局配置
        // 2. 后触发局部配置
        // 3. 局部配置由外到内触发所有满足条件处理器

        /// <summary>
        /// 预处理指定Asset类型
        /// </summary>
        /// <param name="assetProcessType"></param>
        /// <param name="assetType"></param>
        /// <param name="assetPostProcessor"></param>
        /// <param name="paramList">不定长参数列表</param>
        public static void OnPreprocessByAssetType(AssetProcessType assetProcessType, AssetType assetType, AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            AssetPipelineLog.Log($"开始执行预处理全局处理器:".WithColor(Color.yellow));
            ExecuteGlobalProcessorMapByAssetType(GlobalPreProcessorMap, assetProcessType, assetType, assetPostProcessor, paramList);
            AssetPipelineLog.Log($"开始执行预处理局部处理器:".WithColor(Color.yellow));
            ExecuteLocalProcessorMapByAssetType(LocalData.PreProcessorDataList, assetProcessType, assetType, assetPostProcessor, paramList);
        }

        /// <summary>
        /// 后处理导入指定Asset类型
        /// </summary>
        /// <param name="assetProcessType"></param>
        /// <param name="assetType"></param>
        /// <param name="assetPostProcessor"></param>
        /// <param name="paramList">不定长参数列表</param>
        public static void OnPostprocessByAssetType(AssetProcessType assetProcessType, AssetType assetType, AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            AssetPipelineLog.Log($"开始执行后处理assetPostProcessor全局处理器:".WithColor(Color.yellow));
            ExecuteGlobalProcessorMapByAssetType(GlobalPostProcessorMap, assetProcessType, assetType, assetPostProcessor, paramList);
            AssetPipelineLog.Log($"开始执行后处理assetPostProcessor局部处理器:".WithColor(Color.yellow));
            ExecuteLocalProcessorMapByAssetType(LocalData.PostProcessorDataList, assetProcessType, assetType, assetPostProcessor, paramList);
        }

        /// <summary>
        /// 后处理导入指定Asset类型和指定Asset路径
        /// </summary>
        /// <param name="assetProcessType"></param>
        /// <param name="assetType"></param>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        public static void OnPostprocessByAssetType2(AssetProcessType assetProcessType, AssetType assetType, string assetPath, params object[] paramList)
        {
            AssetPipelineLog.Log($"开始执行后处理assetPath全局处理器:".WithColor(Color.yellow));
            ExecuteGlobalProcessorMapByAssetType2(GlobalPostProcessorMap, assetProcessType, assetType, assetPath, paramList);
            AssetPipelineLog.Log($"开始执行后处理assetPath局部处理器:".WithColor(Color.yellow));
            ExecuteLocalProcessorMapByAssetType2(LocalData.PostProcessorDataList, assetProcessType, assetType, assetPath, paramList);
        }

        /// <summary>
        /// 后处理移动指定Asset路径
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="assetProcessType"></param>
        /// <param name="paramList">不定长参数列表</param>
        public static void OnPostprocessMovedByAssetPath(string assetPath, AssetProcessType assetProcessType, params object[] paramList)
        {
            AssetPipelineLog.Log($"开始执行后处理全局移动处理器:".WithColor(Color.yellow));
            ExecuteGlobalProcessorMapByAssetPath(GlobalMovedProcessorMap, assetProcessType, assetPath, paramList);
            AssetPipelineLog.Log($"开始执行后处理局部移动处理器:".WithColor(Color.yellow));
            ExecuteLocalProcessorMapByAssetPath(LocalData.MovedProcessorDataList, assetProcessType, assetPath, paramList);
        }

        /// <summary>
        /// 后处理删除指定Asset路径
        /// </summary>
        /// <param name="assetProcessType"></param>
        /// <param name="assetPath"></param>
        public static void OnPostprocessDeletedByAssetPath(string assetPath, AssetProcessType assetProcessType)
        {
            AssetPipelineLog.Log($"开始执行后处理全局删除移动处理器:".WithColor(Color.yellow));
            ExecuteGlobalProcessorMapByAssetPath(GlobalDeletedProcessorMap, assetProcessType, assetPath);
            AssetPipelineLog.Log($"开始执行后处理局部删除处理器:".WithColor(Color.yellow));
            ExecuteLocalProcessorMapByAssetPath(LocalData.DeletedProcessorDataList, assetProcessType, assetPath);
        }

        /// <summary>
        /// 执行指定全局处理器Map指定Asset类型的处理器
        /// </summary>
        /// <param name="processorMap"></param>
        /// <param name="assetProcessType"></param>
        /// <param name="assetType"></param>
        /// <param name="assetPostProcessor"></param>
        /// <param name="paramList">不定长参数列表</param>
        private static void ExecuteGlobalProcessorMapByAssetType(Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessor>>> processorMap,
                                                                    AssetProcessType assetProcessType, AssetType assetType,
                                                                    AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            var processorList = GetProcessorListByAssetType(assetProcessType, assetType, processorMap);
            ExecuteProcessorsByAssetPostprocessor(processorList, assetPostProcessor, paramList);
        }

        /// <summary>
        /// 执行指定全局处理器Map指定Asset类型指定Asset路径的处理器2
        /// </summary>
        /// <param name="assetProcessType"></para>
        /// <param name="processorMap"></param>
        /// <param name="assetType"></param>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        private static void ExecuteGlobalProcessorMapByAssetType2(Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessor>>> processorMap,
                                                                    AssetProcessType assetProcessType, AssetType assetType,
                                                                        string assetPath, params object[] paramList)
        {
            var processorList = GetProcessorListByAssetType(assetProcessType, assetType, processorMap);
            ExecuteProcessorsByAssetPath(processorList, assetPath);
        }

        /// <summary>
        /// 执行指定全局处理器Map指定Asset路径的处理器
        /// </summary>
        /// <param name="processorMap"></param>
        /// <param name="assetProcessType"></param>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        private static void ExecuteGlobalProcessorMapByAssetPath(Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessor>>> processorMap,
                                                                    AssetProcessType assetProcessType, string assetPath, params object[] paramList)
        {
            var assetType = AssetPipelineSystem.GetAssetTypeByPath(assetPath);
            ExecuteGlobalProcessorMapByAssetType2(processorMap, assetProcessType, assetType, assetPath);
        }

        // 局部处理器触发规则:
        // 1. 由外往内寻找符合目标目录设置的局部处理器配置触发

        /// <summary>
        /// 执行指定局部处理器Map指定Asset类型的处理器
        /// </summary>
        /// <param name="processorLocalDataList"></param>
        /// <param name="assetProcessType"></param>
        /// <param name="assetType"></param>
        /// <param name="assetPostProcessor"></param>
        /// <param name="paramList">不定长参数列表</param>
        private static void ExecuteLocalProcessorMapByAssetType(List<ProcessorLocalData> processorLocalDataList, AssetProcessType assetProcessType,
                                                                    AssetType assetType, AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            var assetPath = assetPostProcessor.assetPath;
            foreach (var processorLocalData in processorLocalDataList)
            {
                if (!processorLocalData.IsInTargetFolder(assetPath))
                {
                    continue;
                }
                AssetPipelineLog.Log($"局部处理器目录:{processorLocalData.FolderPath}".WithColor(Color.red));
                foreach (var processorData in processorLocalData.ProcessorDataList)
                {
                    if(processorData.IsValideAssetProcessType(assetProcessType) &&
                            processorData.IsValideAssetType(assetType) && !processorData.IsInBlackList(assetPath))
                    {
                        ExecuteProcessorByAssetPostprocessor(processorData.Processor, assetPostProcessor, paramList);
                    }
                }
            }
        }

        /// <summary>
        /// 执行指定局部处理器Map指定Asset类型和指定Asset路径的处理器2
        /// </summary>
        /// <param name="processorLocalDataList"></param>
        /// <param name="assetProcessType"></param>
        /// <param name="assetType"></param>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        private static void ExecuteLocalProcessorMapByAssetType2(List<ProcessorLocalData> processorLocalDataList, AssetProcessType assetProcessType,
                                                                    AssetType assetType, string assetPath, params object[] paramList)
        {
            foreach (var processorLocalData in processorLocalDataList)
            {
                if (!processorLocalData.IsInTargetFolder(assetPath))
                {
                    continue;
                }
                AssetPipelineLog.Log($"局部处理器目录:{processorLocalData.FolderPath}".WithColor(Color.red));
                foreach (var processorData in processorLocalData.ProcessorDataList)
                {
                    if (processorData.IsValideAssetProcessType(assetProcessType) &&
                            processorData.IsValideAssetType(assetType) && !processorData.IsInBlackList(assetPath))
                    {
                        ExecuteProcessorByAssetPath(processorData.Processor, assetPath, paramList);
                    }
                }
            }
        }

        /// <summary>
        /// 执行指定局部处理器Map指定Asset路径的处理器
        /// </summary>
        /// <param name="processorLocalDataList"></param>
        /// <param name="assetProcessType"></param>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        private static void ExecuteLocalProcessorMapByAssetPath(List<ProcessorLocalData> processorLocalDataList, AssetProcessType assetProcessType,
                                                                    string assetPath, params object[] paramList)
        {
            var assetType = AssetPipelineSystem.GetAssetTypeByPath(assetPath);
            ExecuteLocalProcessorMapByAssetType2(processorLocalDataList, assetProcessType, assetType, assetPath, paramList);
        }

        /// <summary>
        /// 指定Asset路径触发处理器列表
        /// </summary>
        /// <param name="processorList"></param>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        private static void ExecuteProcessorsByAssetPath(List<BaseProcessor> processorList, string assetPath, params object[] paramList)
        {
            if (processorList != null && !string.IsNullOrEmpty(assetPath))
            {
                foreach (var processor in processorList)
                {
                    processor.ExecuteProcessorByPath(assetPath, paramList);
                }
            }
        }

        /// <summary>
        /// 指定Asset路径触发处理器
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        private static void ExecuteProcessorByAssetPath(BaseProcessor processor, string assetPath, params object[] paramList)
        {
            if (processor != null && !string.IsNullOrEmpty(assetPath))
            {
                processor.ExecuteProcessorByPath(assetPath, paramList);
            }
        }

        /// <summary>
        /// 指定AssetPostprocessor触发处理器列表
        /// </summary>
        /// <param name="processorList"></param>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        private static void ExecuteProcessorsByAssetPostprocessor(List<BaseProcessor> processorList, AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            if (processorList != null && assetPostProcessor != null)
            {
                foreach (var processor in processorList)
                {
                    ExecuteProcessorByAssetPostprocessor(processor, assetPostProcessor, paramList);
                }
            }
        }

        /// <summary>
        /// 指定AssetPostprocessor触发处理器
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        private static void ExecuteProcessorByAssetPostprocessor(BaseProcessor processor, AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            if (processor != null && assetPostProcessor != null)
            {
                processor.ExecuteProcessor(assetPostProcessor, paramList);
            }
        }
        #endregion
    }
}