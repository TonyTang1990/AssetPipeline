/*
 * Description:             AssetProcessorSystem.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/17
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        /// Asset处理器信息数据文件名(仅Json需要)
        /// </summary>
        private const string AssetProcessorInfoDataName = "AssetProcessorInfoData";

        /// <summary>
        /// Asset处理器全局数据
        /// </summary>
        private static AssetProcessorGlobalDataJson GlobalData;

        /// <summary>
        /// Asset处理器局部数据
        /// </summary>
        private static AssetProcessorLocalDataJson LocalData;

        /// <summary>
        /// 所有Asset路径处理器Map<Asset路径, Json Asset处理器实例对象>
        /// </summary>
        private static Dictionary<string, BaseProcessorJson> AllAssetProcessorMap;

        /// <summary>
        /// 全局预处理器映射Map<目标Asset管线处理类型, <目标Asset类型, Json处理器列表>>
        /// </summary>
        private static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessorJson>>> GlobalPreProcessorMap;

        /// <summary>
        /// 全局后处理器映射Map<目标Asset管线处理类型, <目标Asset类型, Json处理器列表>>
        /// </summary>
        private static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessorJson>>> GlobalPostProcessorMap;

        /// <summary>
        /// 全局移动处理器映射Map<目标Asset管线处理类型, <目标Asset类型, Json处理器列表>>
        /// </summary>
        private static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessorJson>>> GlobalMovedProcessorMap;

        /// <summary>
        /// 全局删除处理器映射Map<目标Asset管线处理类型, <目标Asset类型, Json处理器列表>>
        /// </summary>
        private static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessorJson>>> GlobalDeletedProcessorMap;

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            AssetPipelineLog.Log($"Asset处理器系统初始化".WithColor(Color.red));
            MakeSureProcessorSaveFolderExit();
            MakeSureStrategyFolderExist();
            var activeStrategyName = AssetPipelineSystem.GetActiveTargetStrategyName();
            GlobalData = LoadJsonGlobalDataByStrategy(activeStrategyName);
            LocalData = LoadJsonLocalDataByStrategy(activeStrategyName);
            InitAllJsonProcessors();
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
        /// 初始化所有处理器(Json)
        /// </summary>
        private static void InitAllJsonProcessors()
        {
            AllAssetProcessorMap = GetAllJsonAssetProcessorMap();
        }

        /// <summary>
        /// 获取所有Asset路径处理器Map<Asset路径, Asset Json处理器实例对象>
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, BaseProcessorJson> GetAllJsonAssetProcessorMap()
        {
            var allAssetJsonProcessorMap = new Dictionary<string, BaseProcessorJson>();
            var assetProcessorInfoData = LoadAssetProcessorInfoJsonData();
            if(assetProcessorInfoData == null)
            {
                return allAssetJsonProcessorMap;
            }
            foreach (var assetInfo in assetProcessorInfoData.AllProcessorAssetInfo)
            {
                var jsonProcessorTypeName = AssetPipelineUtilities.GetJsonTypeName(assetInfo.AssetTypeFullName);
                var assetJsonProcessorType = AssetPipelineConst.ASSET_PIPELINE_ASSEMBLY.GetType(jsonProcessorTypeName);
                if(assetJsonProcessorType == null)
                {
                    Debug.LogError($"找不到Asset处理器类型全名:{jsonProcessorTypeName},构建处理器类型:{jsonProcessorTypeName}失败!");
                    continue;
                }
                if(!File.Exists(assetInfo.JsonAssetPath))
                {
                    Debug.LogError($"找不到Json Asset文件:{assetInfo.JsonAssetPath},构建处理器类型:{jsonProcessorTypeName}失败!");
                    continue;
                }
                if (allAssetJsonProcessorMap.ContainsKey(assetInfo.AssetPath))
                {
                    Debug.LogError($"不应该添加重复的Asset路径:{assetInfo.AssetTypeFullName}的Asset Json类型全名:{jsonProcessorTypeName}");
                    continue;
                }
                var jsonContent = File.ReadAllText(assetInfo.JsonAssetPath, Encoding.UTF8);
                var assetJsonProcessorInstance = JsonUtility.FromJson(jsonContent, assetJsonProcessorType) as BaseProcessorJson;
                allAssetJsonProcessorMap.Add(assetInfo.AssetPath, assetJsonProcessorInstance);
            }
            return allAssetJsonProcessorMap;
        }

        /// <summary>
        /// 获取指定Asset路径的处理器Json
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static BaseProcessorJson GetProcessorByAssetPath(string assetPath)
        {
            BaseProcessorJson processor = null;
            if(!AllAssetProcessorMap.TryGetValue(assetPath, out processor))
            {
                Debug.LogError($"找不到Asset路径:{assetPath}的处理器Json实例对象!");
                return null;
            }
            return processor;
        }

        /// <summary>
        /// 加载Asset处理器信息数据(Json)
        /// </summary>
        /// <returns></returns>
        private static AssetProcessorInfoData LoadAssetProcessorInfoJsonData()
        {
            AssetProcessorInfoData assetProcessorInfoData;
            var assetProcessorInfoDataSavePath = $"{AssetProcessorSystem.GetProcessorInfoDataSaveRelativePath()}.json";
            if(!File.Exists(assetProcessorInfoDataSavePath))
            {
                Debug.LogWarning($"Asset处理器信息数据文件:{assetProcessorInfoDataSavePath}不存在，创建默认Asset处理器信息数据!".WithColor(Color.yellow));
                assetProcessorInfoData = new AssetProcessorInfoData();
                return assetProcessorInfoData;
            }
            var assetProcessorInfoDataJsonContent = File.ReadAllText(assetProcessorInfoDataSavePath, Encoding.UTF8);
            assetProcessorInfoData = JsonUtility.FromJson<AssetProcessorInfoData>(assetProcessorInfoDataJsonContent);
            AssetPipelineLog.Log($"加载Asset处理器信息Json数据:{assetProcessorInfoDataSavePath}完成，处理器总数量:{assetProcessorInfoData.AllProcessorAssetInfo.Count}!".WithColor(Color.green));
            return assetProcessorInfoData;
        }

        /// <summary>
        /// 保存Asset处理器信息数据(Json)
        /// </summary>
        /// <returns></returns>
        public static bool SaveAssetProcessorInfoData(AssetProcessorInfoData assetProcessorInfoData)
        {
            if(assetProcessorInfoData == null)
            {
                Debug.LogError($"不允许保存空的Asset处理器信息数据，保存Asset处理器信息数据失败，请检查代码!");
                return false;
            }
            var assetProcessorInfoDataSavePath = $"{AssetProcessorSystem.GetProcessorInfoDataSaveRelativePath()}.json";
            var assetProcessorInfoDataJsonContent = JsonUtility.ToJson(assetProcessorInfoData, true);
            File.WriteAllText(assetProcessorInfoDataSavePath, assetProcessorInfoDataJsonContent);
            Debug.Log($"保存所有处理器信息的Json数据:{assetProcessorInfoDataSavePath}完成!".WithColor(Color.green));
            return true;
        }

        /// <summary>
        /// 保存指定处理器到Json
        /// </summary>
        /// <param name="processor"></param>
        /// <returns></returns>
        public static bool SaveProcessorToJson(BaseProcessor processor)
        {
            if (processor == null)
            {
                Debug.LogWarning($"不保存空处理器Json!");
                return false;
            }
            var processorAssetPath = AssetDatabase.GetAssetPath(processor);
            if(string.IsNullOrEmpty(processorAssetPath))
            {
                Debug.LogError($"找不到处理器:{processor.name}的Asset路径,保存处理器到Json失败!");
                return false;
            }
            var processorJsonPath = Path.ChangeExtension(processorAssetPath, "json");
            var processorJsonContent = JsonUtility.ToJson(processor, true);
            File.WriteAllText(processorJsonPath, processorJsonContent, Encoding.UTF8);
            return true;
        }

        #region 编辑器配置部分
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
        #endregion

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
        /// 获取全局预处理器Map<Asset管线处理类型个, <目标Asset类型, Json处理器列表>>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessorJson>>> GetGlobalPreProcessorMap()
        {
            return GetGlobalProcessorMap(GlobalData.PreProcessorData.ProcessorAssetPathList, "预");
        }

        /// <summary>
        /// 获取全局后处理器Map<Asset管线处理类型, <目标Asset类型, Json处理器列表>>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessorJson>>> GetGlobalPostProcessorMap()
        {
            return GetGlobalProcessorMap(GlobalData.PostProcessorData.ProcessorAssetPathList, "后");
        }

        /// <summary>
        /// 获取全局移动处理器Map<Asset管线处理类型, <目标Asset类型, Json处理器列表>>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessorJson>>> GetGlobalMovedProcessorMap()
        {
            return GetGlobalProcessorMap(GlobalData.MovedProcessorData.ProcessorAssetPathList, "移动");
        }

        /// <summary>
        /// 获取全局删除处理器Map<Asset管线处理类型, <目标Asset类型, Json处理器列表>>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessorJson>>> GetGlobalDeletedProcessorMap()
        {
            return GetGlobalProcessorMap(GlobalData.DeletedProcessorData.ProcessorAssetPathList, "删除");
        }

        /// <summary>
        /// 获取全局指定处理器的Map<Asset管线处理类型, <目标Asset类型, Json处理器列表>>
        /// </summary>
        /// <param name="processorAssetPathList"></param>
        /// <param name="tip""/></param>
        /// <returns></returns>
        private static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessorJson>>> GetGlobalProcessorMap(List<string> processorAssetPathList, string tip = "")
        {
            Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessorJson>>> globalProcessorMap = new Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessorJson>>>();
            foreach(var processorAssetPath in processorAssetPathList)
            {
                if(!string.IsNullOrEmpty(processorAssetPath))
                {
                    Dictionary<AssetType, List<BaseProcessorJson>> assetTypeProcessorMap;
                    var processor = GetProcessorByAssetPath(processorAssetPath);
                    if(processor == null)
                    {
                        Debug.LogError($"找不到处理器路径:{processorAssetPath}的处理器实例对象,请检查是否上传和导出不匹配,获取全局处理器失败!");
                        continue;
                    }
                    if(!globalProcessorMap.TryGetValue(processor.TargetAssetProcessType, out assetTypeProcessorMap))
                    {
                        assetTypeProcessorMap = new Dictionary<AssetType, List<BaseProcessorJson>>();
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
                            List<BaseProcessorJson> assetTypeProcessorList;
                            if (!assetTypeProcessorMap.TryGetValue(assetType, out assetTypeProcessorList))
                            {
                                assetTypeProcessorList = new List<BaseProcessorJson>();
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
                    if(string.IsNullOrEmpty(processorData.ProcessorAssetPath))
                    {
                        continue;
                    }
                    var processor = GetProcessorByAssetPath(processorData.ProcessorAssetPath);
                    if (processor == null)
                    {
                        Debug.LogError($"找不到处理器路径:{processorData.ProcessorAssetPath}的处理器对象,请检查是否上传和导出不匹配,打印局部处理器失败!");
                        continue;
                    }
                    if (processor != null)
                    {
                        AssetPipelineLog.Log($"局部{tip}处理器目标目录:{processorLocalData.FolderPath},Asset管线处理类型:{processor.TargetAssetProcessType},添加局部处理器:{processor.Name}!".WithColor(Color.yellow));
                        processorData.PrintAllBlackListFolder();
                    }
                }
            }
        }

        /// <summary>
        /// 获取指定Asset处理器Map里的指定Asset类型的Json处理器列表
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="processorMap"></param>
        /// <returns></returns>
        private static List<BaseProcessorJson> GetProcessorListByAssetType(AssetProcessType assetProcessType, AssetType assetType,
                                                                        Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessorJson>>> processorMap)
        {
            if(processorMap == null)
            {
                return null;
            }
            Dictionary<AssetType, List<BaseProcessorJson>> assetTypeProcessorMap;
            if(!processorMap.TryGetValue(assetProcessType, out assetTypeProcessorMap))
            {
                return null;
            }
            List<BaseProcessorJson> processorList;
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
        /// 获取自定义Asset处理器信息数据存储相对路径
        /// </summary>
        /// <returns></returns>
        private static string GetProcessorInfoDataSaveRelativePath()
        {
            var saveFolderRelativePath = AssetPipelineSystem.GetSaveFolderRelativePath();
            return PathUtilities.GetRegularPath($"{saveFolderRelativePath}/AssetProcessors/{AssetProcessorInfoDataName}");
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
            return $"{AssetProcessorGlobalDataName}";
        }

        /// <summary>
        /// 获取Asset处理器局部数据Asset名
        /// </summary>
        /// <returns></returns>
        public static string GetLocalDataAssetName()
        {
            return $"{AssetProcessorLocalDataName}";
        }

        /// <summary>
        /// 获取Asset处理器信息数据Asset名
        /// </summary>
        /// <returns></returns>
        public static string GetProcessorInfoDataAssetName()
        {
            return $"{AssetProcessorLocalDataName}";
        }

        /// <summary>
        /// 获取指定策略的Asset处理器局部数据Asset名
        /// </summary>
        /// <param name="strategyName"></param>
        /// <returns></returns>
        public static string GetGlobalDataRelativePathByStartegy(string strategyName)
        {
            var saveFolderRelativePath = GetDataSaveFolderByStrategy(strategyName);
            return $"{saveFolderRelativePath}/{GetGlobalDataAssetName()}";
        }

        /// <summary>
        /// 获取指定策略的Asset处理器局部数据相对路径
        /// </summary>
        /// <param name="strategyName"></param>
        /// <returns></returns>
        public static string GetLocalDataRelativePathByStrategy(string strategyName)
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
        /// 加载指定平台Asset处理器全局配置数据
        /// </summary>
        /// <param name="strategyName"></param>
        /// <returns></returns>
        public static AssetProcessorGlobalData LoadGlobalDataByStrategy(string strategyName)
        {
            var globalDataRelativePath = $"{GetGlobalDataRelativePathByStartegy(strategyName)}.asset";
            var globalData = AssetDatabase.LoadAssetAtPath<AssetProcessorGlobalData>(globalDataRelativePath);
            if (globalData == null)
            {
                Debug.LogWarning($"找不到Asset处理器全局配置数据:{globalDataRelativePath}，创建默认Asset处理器全局配置数据!".WithColor(Color.yellow));
                globalData = ScriptableObject.CreateInstance<AssetProcessorGlobalData>();
                AssetDatabase.CreateAsset(globalData, globalDataRelativePath);
            }
            return globalData;
        }

        /// <summary>
        /// 加载指定策略Json处理器全局配置数据
        /// </summary>
        /// <param name="strategyName"></param>
        /// <returns></returns>
        public static AssetProcessorGlobalDataJson LoadJsonGlobalDataByStrategy(string strategyName)
        {
            var globalDataRelativePath = $"{GetGlobalDataRelativePathByStartegy(strategyName)}.json";
            AssetProcessorGlobalDataJson globalData;
            if (!File.Exists(globalDataRelativePath))
            {
                Debug.LogWarning($"找不到Asset处理器全局配置Json数据:{globalDataRelativePath}，清闲配置Asset处理器全局配置数据!".WithColor(Color.yellow));
                globalData = new AssetProcessorGlobalDataJson();
            }
            else
            {
                var globalDataJsonContent = File.ReadAllText(globalDataRelativePath, Encoding.UTF8);
                globalData = JsonUtility.FromJson<AssetProcessorGlobalDataJson>(globalDataJsonContent);
            }
            AssetPipelineLog.Log($"加载Asset处理器全局配置Json数据:{globalDataRelativePath}完成!".WithColor(Color.green));
            return globalData;
        }

        /// <summary>
        /// 保存指定策略的Asset处理器全局配置数据(Json)
        /// </summary>
        /// <param name="globalData"></param>
        /// <param name="strategyName"></param>
        /// <returns></returns>
        public static bool SaveGlobalDataToJsonByStrategy(AssetProcessorGlobalData globalData, string strategyName)
        {
            if(globalData == null)
            {
                Debug.LogError($"不允许保存空的Asset处理器全局配置数据，保存Asset处理器全局配置数据失败，请检查代码!");
                return false;
            }
            var globalDataSavePath = $"{AssetProcessorSystem.GetGlobalDataRelativePathByStartegy(strategyName)}.json";
            var globalDataJsonContent = JsonUtility.ToJson(globalData, true);
            File.WriteAllText(globalDataSavePath, globalDataJsonContent, Encoding.UTF8);
            Debug.Log($"保存Asset处理器全局配置的Json数据:{globalDataSavePath}完成!".WithColor(Color.green));
            return true;
        }

        /// <summary>
        /// 加载指定策略Asset处理器局部配置数据
        /// </summary>
        /// <param name="strategyName"></param>
        /// <returns></returns>
        public static AssetProcessorLocalData LoadLocalDataByStrategy(string strategyName)
        {
            var localDataRelativePath = $"{GetLocalDataRelativePathByStrategy(strategyName)}.asset";
            var localData = AssetDatabase.LoadAssetAtPath<AssetProcessorLocalData>(localDataRelativePath);
            if (localData == null)
            {
                Debug.LogWarning($"找不到Asset处理器局部配置数据:{localDataRelativePath}，创建默认Asset处理器局部配置数据!".WithColor(Color.yellow));
                localData = ScriptableObject.CreateInstance<AssetProcessorLocalData>();
                AssetDatabase.CreateAsset(localData, localDataRelativePath);
            }
            return localData;
        }

        /// <summary>
        /// 加载指定策略Json处理器局部配置数据
        /// </summary>
        /// <param name="strategyName"></param>
        /// <returns></returns>
        public static AssetProcessorLocalDataJson LoadJsonLocalDataByStrategy(string strategyName)
        {
            var localDataRelativePath = $"{GetLocalDataRelativePathByStrategy(strategyName)}.json";
            AssetProcessorLocalDataJson localData;
            if (!File.Exists(localDataRelativePath))
            {
                Debug.LogWarning($"找不到Asset处理器局部配置的Json数据:{localDataRelativePath},请先配置Asset处理器局部配置数据!".WithColor(Color.yellow));
                localData = new AssetProcessorLocalDataJson();
            }
            else
            {
                var localDataJsonContent = File.ReadAllText(localDataRelativePath, Encoding.UTF8);
                localData = JsonUtility.FromJson<AssetProcessorLocalDataJson>(localDataJsonContent);
            }
            AssetPipelineLog.Log($"加载Asset处理器局部配置的Json数据:{localDataRelativePath}完成!".WithColor(Color.green));
            return localData;
        }

        /// <summary>
        /// 保存指定策略的Asset处理器局部配置数据(Json)
        /// </summary>
        /// <param name="localData"></param>
        /// <param name="strategyName"></param>
        /// <returns></returns>
        public static bool SaveLocalDataToJsonByStrategy(AssetProcessorLocalData localData, string strategyName)
        {
            if (localData == null)
            {
                Debug.LogError($"不允许保存空的Asset处理器局部配置数据，保存Asset处理器局部配置数据失败，请检查代码!");
                return false;
            }
            var localDataSavePath = $"{AssetProcessorSystem.GetLocalDataRelativePathByStrategy(strategyName)}.json";
            var localDataJsonContent = JsonUtility.ToJson(localData, true);
            File.WriteAllText(localDataSavePath, localDataJsonContent);
            Debug.Log($"保存Asset处理器局部配置的Json数据:{localDataSavePath}完成!".WithColor(Color.green));
            return true;
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
        private static void ExecuteGlobalProcessorMapByAssetType(Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessorJson>>> processorMap,
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
        private static void ExecuteGlobalProcessorMapByAssetType2(Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessorJson>>> processorMap,
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
        private static void ExecuteGlobalProcessorMapByAssetPath(Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseProcessorJson>>> processorMap,
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
                        if (string.IsNullOrEmpty(processorData.ProcessorAssetPath))
                        {
                            continue;
                        }
                        var processor = GetProcessorByAssetPath(processorData.ProcessorAssetPath);
                        if (processor == null)
                        {
                            Debug.LogError($"找不到处理器路径:{processorData.ProcessorAssetPath}的处理器对象,请检查是否上传和导出不匹配，触发局部处理器失败!!");
                            continue;
                        }
                        ExecuteProcessorByAssetPostprocessor(processor, assetPostProcessor, paramList);
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
                        var processorJson = GetProcessorByAssetPath(processorData.ProcessorAssetPath);
                        ExecuteProcessorByAssetPath(processorJson, assetPath, paramList);
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
        private static void ExecuteProcessorsByAssetPath(List<BaseProcessorJson> processorList, string assetPath, params object[] paramList)
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
        private static void ExecuteProcessorByAssetPath(BaseProcessorJson processor, string assetPath, params object[] paramList)
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
        private static void ExecuteProcessorsByAssetPostprocessor(List<BaseProcessorJson> processorList, AssetPostprocessor assetPostProcessor, params object[] paramList)
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
        private static void ExecuteProcessorByAssetPostprocessor(BaseProcessorJson processor, AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            if (processor != null && assetPostProcessor != null)
            {
                processor.ExecuteProcessor(assetPostProcessor, paramList);
            }
        }
        #endregion
    }
}