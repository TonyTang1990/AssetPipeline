/*
 * Description:             AssetProcessorSystem.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/17
 */

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
        /// 所有处理器列表
        /// </summary>
        private static List<BaseProcessor> AllProcessors;

        /// <summary>
        /// 全局预处理器映射Map<目标Asset类型, 处理器列表>
        /// </summary>
        private static Dictionary<AssetType, List<BaseProcessor>> GlobalPreProcessorMap;

        /// <summary>
        /// 全局后处理器映射Map<目标Asset类型, 处理器列表>
        /// </summary>
        private static Dictionary<AssetType, List<BaseProcessor>> GlobalPostProcessorMap;

        /// <summary>
        /// 全局移动处理器映射Map<目标Asset类型, 处理器列表>
        /// </summary>
        private static Dictionary<AssetType, List<BaseProcessor>> GlobalMovedProcessorMap;

        /// <summary>
        /// 全局删除处理器映射Map<目标Asset类型, 处理器列表>
        /// </summary>
        private static Dictionary<AssetType, List<BaseProcessor>> GlobalDeletedProcessorMap;

        /// <summary>
        /// 局部预处理器映射Map<目标目录, <目标Asset类型, 处理器列表>>
        /// </summary>
        private static Dictionary<string, Dictionary<AssetType, List<BaseProcessor>>> LocalPreProcessorMap;

        /// <summary>
        /// 局部后处理器映射Map<目标目录, <目标Asset类型, 处理器列表>>
        /// </summary>
        private static Dictionary<string, Dictionary<AssetType, List<BaseProcessor>>> LocalPostProcessorMap;

        /// <summary>
        /// 局部移动处理器映射Map<目标目录, <目标Asset类型, 处理器列表>>
        /// </summary>
        private static Dictionary<string, Dictionary<AssetType, List<BaseProcessor>>> LocalMovedProcessorMap;

        /// <summary>
        /// 局部删除处理器映射Map<目标目录, <目标Asset类型, 处理器列表>>
        /// </summary>
        private static Dictionary<string, Dictionary<AssetType, List<BaseProcessor>>> LocalDeletedProcessorMap;

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            Debug.Log($"AssetProcessorSystem :Init()");
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
            AllProcessors = GetAllProcessors();
        }

        /// <summary>
        /// 获取所有处理器
        /// </summary>
        /// <returns></returns>
        public static List<BaseProcessor> GetAllProcessors()
        {
            List<BaseProcessor> allProcessorList = new List<BaseProcessor>();
            var processorSaveRelativePath = GetProcessorSaveRelativePath();
            var allProcessorsGuids = AssetDatabase.FindAssets($"t:{AssetPipelineConst.BASE_PROCESSOR_TYPE}", new[] { processorSaveRelativePath });
            foreach(var processGuid in allProcessorsGuids)
            {
                var processorAssetPath = AssetDatabase.GUIDToAssetPath(processGuid);
                var processorAsset = AssetDatabase.LoadAssetAtPath<BaseProcessor>(processorAssetPath);
                allProcessorList.Add(processorAsset);
            }
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
        /// 获取全局预处理器Map<目标Asset类型, 处理器列表>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<AssetType, List<BaseProcessor>> GetGlobalPreProcessorMap()
        {
            return GetGlobalProcessorMap(GlobalData.PreProcessorData.ProcessorList);
        }

        /// <summary>
        /// 获取全局后处理器Map<目标Asset类型, 处理器列表>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<AssetType, List<BaseProcessor>> GetGlobalPostProcessorMap()
        {
            return GetGlobalProcessorMap(GlobalData.PostProcessorData.ProcessorList);
        }

        /// <summary>
        /// 获取全局移动处理器Map<目标Asset类型, 处理器列表>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<AssetType, List<BaseProcessor>> GetGlobalMovedProcessorMap()
        {
            return GetGlobalProcessorMap(GlobalData.MovedProcessorData.ProcessorList);
        }

        /// <summary>
        /// 获取全局删除处理器Map<目标Asset类型, 处理器列表>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<AssetType, List<BaseProcessor>> GetGlobalDeletedProcessorMap()
        {
            return GetGlobalProcessorMap(GlobalData.DeletedProcessorData.ProcessorList);
        }

        /// <summary>
        /// 获取全局指定处理器的Map<目标Asset类型, 处理器列表>
        /// </summary>
        /// <param name="processorList"></param>
        /// <returns></returns>
        private static Dictionary<AssetType, List<BaseProcessor>> GetGlobalProcessorMap(List<BaseProcessor> processorList)
        {
            Dictionary<AssetType, List<BaseProcessor>> globalProcessorMap = new Dictionary<AssetType, List<BaseProcessor>>();
            foreach(var processor in processorList)
            {
                List<BaseProcessor> assetTypeProcessorList;
                if(!globalProcessorMap.TryGetValue(processor.TargetAssetType, out assetTypeProcessorList))
                {
                    assetTypeProcessorList = new List<BaseProcessor>();
                    globalProcessorMap.Add(processor.TargetAssetType, assetTypeProcessorList);
                }
                assetTypeProcessorList.Add(processor);
            }
            return globalProcessorMap;
        }

        /// <summary>
        /// 初始化所有局部处理器信息
        /// </summary>
        private static void InitLocalProcessorData()
        {
            LocalPreProcessorMap = GetLocalPreProcessorMap();
            LocalPostProcessorMap = GetLocalPostProcessorMap();
            LocalMovedProcessorMap = GetLocalMovedProcessorMap();
            LocalDeletedProcessorMap = GetLocalDeletedProcessorMap();
        }

        /// <summary>
        /// 获取局部预处理器Map<目标目录, <目标Asset类型, 处理器列表>>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string ,Dictionary<AssetType, List<BaseProcessor>>> GetLocalPreProcessorMap()
        {
            return GetLocalProcessorMap(LocalData.PreProcessorDataList);
        }

        /// <summary>
        /// 获取局部后处理器Map<目标目录, <目标Asset类型, 处理器列表>>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<AssetType, List<BaseProcessor>>> GetLocalPostProcessorMap()
        {
            return GetLocalProcessorMap(LocalData.PostProcessorDataList);
        }

        /// <summary>
        /// 获取局部移动处理器Map<目标目录, <目标Asset类型, 处理器列表>>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<AssetType, List<BaseProcessor>>> GetLocalMovedProcessorMap()
        {
            return GetLocalProcessorMap(LocalData.MovedProcessorDataList);
        }

        /// <summary>
        /// 获取局部删除处理器Map<目标目录, <目标Asset类型, 处理器列表>>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<AssetType, List<BaseProcessor>>> GetLocalDeletedProcessorMap()
        {
            return GetLocalProcessorMap(LocalData.DeletedProcessorDataList);
        }

        /// <summary>
        /// 获取局部指定处理器的Map<目标目录, </目标目录>目标Asset类型, 处理器列表>>
        /// </summary>
        /// <param name="processorList"></param>
        /// <returns></returns>
        private static Dictionary<string, Dictionary<AssetType, List<BaseProcessor>>> GetLocalProcessorMap(List<ProcessorLocalData> processorLocalDataList)
        {
            Dictionary<string, Dictionary<AssetType, List<BaseProcessor>>> localProcessorMap = new Dictionary<string, Dictionary<AssetType, List<BaseProcessor>>>();
            foreach (var processorLocalData in processorLocalDataList)
            {
                Dictionary<AssetType, List<BaseProcessor>> assetTypeProcessorMap;
                if (!localProcessorMap.TryGetValue(processorLocalData.FolderPath, out assetTypeProcessorMap))
                {
                    assetTypeProcessorMap = new Dictionary<AssetType, List<BaseProcessor>>();
                    localProcessorMap.Add(processorLocalData.FolderPath, assetTypeProcessorMap);
                }
                foreach(var processor in processorLocalData.ProcessorList)
                {
                    List<BaseProcessor> assetTypeProcessorList;
                    if (!assetTypeProcessorMap.TryGetValue(processor.TargetAssetType, out assetTypeProcessorList))
                    {
                        assetTypeProcessorList = new List<BaseProcessor>();
                        assetTypeProcessorMap.Add(processor.TargetAssetType, assetTypeProcessorList);
                    }
                    assetTypeProcessorList.Add(processor);
                }
            }
            return localProcessorMap;
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
        /// 获取指定策略的Asset处理器全局数据相对路径
        /// </summary>
        /// <param name="strategyName"></param>
        /// <returns></returns>
        private static string GetGlobalDataRelativePathByStartegy(string strategyName)
        {
            var saveFolderRelativePath = GetDataSaveFolderByStrategy(strategyName);
            return $"{saveFolderRelativePath}/{AssetProcessorGlobalDataName}.asset";
        }

        /// <summary>
        /// 获取指定策略的Asset处理器局部数据相对路径
        /// </summary>
        /// <param name="strategyName"></param>
        /// <returns></returns>
        private static string GetLocalDataRelativePathByStrategy(string strategyName)
        {
            var saveFolderRelativePath = GetDataSaveFolderByStrategy(strategyName);
            return $"{saveFolderRelativePath}/{AssetProcessorLocalDataName}.asset";
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
                localData = ScriptableObject.CreateInstance<AssetProcessorLocalData>();
                AssetDatabase.CreateAsset(localData, localDataRelativePath);
            }
            return localData;
        }
    }
}