﻿/*
 * Description:             AssetCheckSystem.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/17
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static TAssetPipeline.AssetCheckLocalData;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetCheckSystem.cs
    /// Asset检查系统
    /// </summary>
    public static class AssetCheckSystem
    {
        /// <summary>
        /// Asset检查器全局数据文件名
        /// </summary>
        private const string AssetCheckGlobalDataName = "AssetCheckGlobalData";

        /// <summary>
        /// Asset检查器局部数据文件名
        /// </summary>
        private const string AssetCheckLocalDataName = "AssetCheckLocalData";

        /// <summary>
        /// Asset检查器全局数据
        /// </summary>
        private static AssetCheckGlobalData GlobalData;

        /// <summary>
        /// Asset检查器局部数据
        /// </summary>
        private static AssetCheckLocalData LocalData;

        /// <summary>
        /// 所有检查器列表
        /// </summary>
        private static List<BaseCheck> AllChecks;

        /// <summary>
        /// 全局预检查器映射Map<目标Asset类型, 检查器列表>
        /// </summary>
        private static Dictionary<AssetType, List<BaseCheck>> GlobalPreCheckMap;

        /// <summary>
        /// 全局后检查器映射Map<目标Asset类型, 检查器列表>
        /// </summary>
        private static Dictionary<AssetType, List<BaseCheck>> GlobalPostCheckMap;

        /// <summary>
        /// 局部预检查器映射Map<目标目录, <目标Asset类型, 检查器列表>>
        /// </summary>
        private static Dictionary<string, Dictionary<AssetType, List<BaseCheck>>> LocalPreCheckMap;

        /// <summary>
        /// 局部后检查器映射Map<目标目录, <目标Asset类型, 检查器列表>>
        /// </summary>
        private static Dictionary<string, Dictionary<AssetType, List<BaseCheck>>> LocalPostCheckMap;

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            Debug.Log($"Asset检查器系统初始化".WithColor(Color.red));
            MakeSureCheckSaveFolderExit();
            MakeSureStrategyFolderExist();
            var activeStrategyName = AssetPipelineSystem.GetActiveTargetStrategyName();
            GlobalData = LoadGlobalDataByStrategy(activeStrategyName);
            LocalData = LoadLocalDataByStrategy(activeStrategyName);
            InitAllChecks();
            InitGlobalCheckData();
            InitLocalCheckData();
        }

        /// <summary>
        /// 确保自定义检查器目录存在
        /// </summary>
        private static void MakeSureCheckSaveFolderExit()
        {
            var checkSaveRelativePath = GetCheckSaveRelativePath();
            var checkSaveFullPath = Path.GetFullPath(checkSaveRelativePath);
            FolderUtilities.CheckAndCreateSpecificFolder(checkSaveFullPath);
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
        /// 初始化所有检查器
        /// </summary>
        private static void InitAllChecks()
        {
            AllChecks = GetAllChecks();
        }

        /// <summary>
        /// 获取所有检查器
        /// </summary>
        /// <returns></returns>
        public static List<BaseCheck> GetAllChecks()
        {
            List<BaseCheck> allCheckList = new List<BaseCheck>();
            var checkSaveRelativePath = GetCheckSaveRelativePath();
            var allCheckGuids = AssetDatabase.FindAssets($"t:{AssetPipelineConst.BASE_PROCESSOR_TYPE}", new[] { checkSaveRelativePath });
            foreach (var checkGuid in allCheckGuids)
            {
                var checkAssetPath = AssetDatabase.GUIDToAssetPath(checkGuid);
                var checkAsset = AssetDatabase.LoadAssetAtPath<BaseCheck>(checkAssetPath);
                allCheckList.Add(checkAsset);
            }
            return allCheckList;
        }

        /// <summary>
        /// 初始化所有全局检查器信息
        /// </summary>
        private static void InitGlobalCheckData()
        {
            GlobalPreCheckMap = GetGlobalPreCheckMap();
            GlobalPostCheckMap = GetGlobalPostCheckMap();
        }

        /// <summary>
        /// 获取全局预检查器Map<目标Asset类型, 检查器列表>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<AssetType, List<BaseCheck>> GetGlobalPreCheckMap()
        {
            return GetGlobalCheckMap(GlobalData.PreCheckData.CheckList);
        }

        /// <summary>
        /// 获取全局后检查器Map<目标Asset类型, 检查器列表>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<AssetType, List<BaseCheck>> GetGlobalPostCheckMap()
        {
            return GetGlobalCheckMap(GlobalData.PostCheckData.CheckList);
        }

        /// <summary>
        /// 获取全局指定检查器的Map<目标Asset类型, 检查器列表>
        /// </summary>
        /// <param name="checkList"></param>
        /// <returns></returns>
        private static Dictionary<AssetType, List<BaseCheck>> GetGlobalCheckMap(List<BaseCheck> checkList)
        {
            Dictionary<AssetType, List<BaseCheck>> globalCheckMap = new Dictionary<AssetType, List<BaseCheck>>();
            foreach (var check in checkList)
            {
                List<BaseCheck> assetTypeCheckList;
                if (!globalCheckMap.TryGetValue(check.TargetAssetType, out assetTypeCheckList))
                {
                    assetTypeCheckList = new List<BaseCheck>();
                    globalCheckMap.Add(check.TargetAssetType, assetTypeCheckList);
                }
                assetTypeCheckList.Add(check);
            }
            return globalCheckMap;
        }

        /// <summary>
        /// 初始化所有局部检查器信息
        /// </summary>
        private static void InitLocalCheckData()
        {
            LocalPreCheckMap = GetLocalPreCheckMap();
            LocalPostCheckMap = GetLocalPostCheckMap();
        }

        /// <summary>
        /// 获取局部预检查器Map<目标目录, <目标Asset类型, 检查器列表>>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<AssetType, List<BaseCheck>>> GetLocalPreCheckMap()
        {
            return GetLocalCheckMap(LocalData.PreCheckDataList);
        }

        /// <summary>
        /// 获取局部后检查器Map<目标目录, <目标Asset类型, 检查器列表>>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<AssetType, List<BaseCheck>>> GetLocalPostCheckMap()
        {
            return GetLocalCheckMap(LocalData.PostCheckDataList);
        }

        /// <summary>
        /// 获取局部指定检查器的Map<目标目录, </目标目录>目标Asset类型, 检查器列表>>
        /// </summary>
        /// <param name="checkLocalDataList"></param>
        /// <returns></returns>
        private static Dictionary<string, Dictionary<AssetType, List<BaseCheck>>> GetLocalCheckMap(List<CheckLocalData> checkLocalDataList)
        {
            Dictionary<string, Dictionary<AssetType, List<BaseCheck>>> localCheckMap = new Dictionary<string, Dictionary<AssetType, List<BaseCheck>>>();
            foreach (var checkLocalData in checkLocalDataList)
            {
                Dictionary<AssetType, List<BaseCheck>> assetTypeCheckMap;
                if (!localCheckMap.TryGetValue(checkLocalData.FolderPath, out assetTypeCheckMap))
                {
                    assetTypeCheckMap = new Dictionary<AssetType, List<BaseCheck>>();
                    localCheckMap.Add(checkLocalData.FolderPath, assetTypeCheckMap);
                }
                foreach (var check in checkLocalData.CheckList)
                {
                    List<BaseCheck> assetTypeCheckList;
                    if (!assetTypeCheckMap.TryGetValue(check.TargetAssetType, out assetTypeCheckList))
                    {
                        assetTypeCheckList = new List<BaseCheck>();
                        assetTypeCheckMap.Add(check.TargetAssetType, assetTypeCheckList);
                    }
                    assetTypeCheckList.Add(check);
                }
            }
            return localCheckMap;
        }

        /// <summary>
        /// 获取指定Asset口岸处器Map里的指定Asset类型的检查器列表
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="checkMap"></param>
        /// <returns></returns>
        private static List<BaseCheck> GetCheckListByAssetType(AssetType assetType, Dictionary<AssetType, List<BaseCheck>> checkMap)
        {
            if (checkMap == null)
            {
                return null;
            }
            List<BaseCheck> checkList;
            if (!checkMap.TryGetValue(assetType, out checkList))
            {
                return null;
            }
            return checkList;
        }

        /// <summary>
        /// 获取自定义Asset检查器存储相对路径
        /// </summary>
        /// <returns></returns>
        private static string GetCheckSaveRelativePath()
        {
            var saveFolderRelativePath = AssetPipelineSystem.GetSaveFolderRelativePath();
            return $"{saveFolderRelativePath}/AssetChecks";
        }

        /// <summary>
        /// 获取指定策略的数据保存相对路径
        /// </summary>
        /// <param name="strategyName"></param>
        /// <returns></returns>
        public static string GetDataSaveFolderByStrategy(string strategyName)
        {
            var strategyDataFolderRelativePath = $"{AssetPipelineSystem.GetSaveFolderRelativePath()}/{strategyName}/AssetCheck";
            return strategyDataFolderRelativePath;
        }

        /// <summary>
        /// 获取指定策略的Asset检查器全局数据相对路径
        /// </summary>
        /// <param name="strategyName"></param>
        /// <returns></returns>
        private static string GetGlobalDataRelativePathByStartegy(string strategyName)
        {
            var saveFolderRelativePath = GetDataSaveFolderByStrategy(strategyName);
            return $"{saveFolderRelativePath}/{AssetCheckGlobalDataName}.asset";
        }

        /// <summary>
        /// 获取指定策略的Asset检查器局部数据相对路径
        /// </summary>
        /// <param name="strategyName"></param>
        /// <returns></returns>
        private static string GetLocalDataRelativePathByStrategy(string strategyName)
        {
            var saveFolderRelativePath = GetDataSaveFolderByStrategy(strategyName);
            return $"{saveFolderRelativePath}/{AssetCheckLocalDataName}.asset";
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
        /// 加载当前激活平台Asset检查器全局数据
        /// </summary>
        /// <param name="strategyName"></param>
        /// <returns></returns>
        public static AssetCheckGlobalData LoadGlobalDataByStrategy(string strategyName)
        {
            var globalDataRelativePath = GetGlobalDataRelativePathByStartegy(strategyName);
            var globalData = AssetDatabase.LoadAssetAtPath<AssetCheckGlobalData>(globalDataRelativePath);
            if (globalData == null)
            {
                globalData = ScriptableObject.CreateInstance<AssetCheckGlobalData>();
                AssetDatabase.CreateAsset(globalData, globalDataRelativePath);
            }
            return globalData;
        }

        /// <summary>
        /// 加载当前激活平台Asset检查器局部数据
        /// </summary>
        /// <param name="strategyName"></param>
        /// <returns></returns>
        public static AssetCheckLocalData LoadLocalDataByStrategy(string strategyName)
        {
            var localDataRelativePath = GetLocalDataRelativePathByStrategy(strategyName);
            var localData = AssetDatabase.LoadAssetAtPath<AssetCheckLocalData>(localDataRelativePath);
            if (localData == null)
            {
                localData = ScriptableObject.CreateInstance<AssetCheckLocalData>();
                AssetDatabase.CreateAsset(localData, localDataRelativePath);
            }
            return localData;
        }

        #region Asset管线处理器处理部分
        // 检查器触发规则:
        // 1. 优先全局配置
        // 2. 后触发局部配置
        // 3. 局部配置由外到内触发所有满足条件检查器

        /// <summary>
        /// 预处理指定Asset类型
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="assetPostProcessor"></param>
        public static bool OnPreCheckByAssetType(AssetType assetType, AssetPostprocessor assetPostProcessor)
        {
            if(!ExecuteGlobalCheckMapByAssetType(GlobalPreCheckMap, assetType, assetPostProcessor))
            {
                return false;
            }
            if(!ExecuteLocalCheckMapByAssetType(LocalPreCheckMap, assetType, assetPostProcessor))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 后处理导入指定Asset类型
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="assetPostProcessor"></param>
        public static bool OnPostCheckByAssetType(AssetType assetType, AssetPostprocessor assetPostProcessor)
        {
            if(!ExecuteGlobalCheckMapByAssetType(GlobalPostCheckMap, assetType, assetPostProcessor))
            {
                return false;
            }
            if(ExecuteLocalCheckMapByAssetType(LocalPostCheckMap, assetType, assetPostProcessor))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 后处理导入指定Asset路径
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="assetPath"></param>
        public static bool OnPostCheckByAssetPath(AssetType assetType, string assetPath)
        {
            if(!ExecuteGlobalCheckMapByAssetPath(GlobalPostCheckMap, assetPath))
            {
                return false;
            }
            if(!ExecuteLocalCheckMapByAssetPath(LocalPostCheckMap, assetPath))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 执行指定全局检查器Map指定Asset类型的检查器
        /// </summary>
        /// <param name="checkMap"></param>
        /// <param name="assetType"></param>
        /// <param name="assetPostProcessor"></param>
        private static bool ExecuteGlobalCheckMapByAssetType(Dictionary<AssetType, List<BaseCheck>> checkMap, AssetType assetType, AssetPostprocessor assetPostProcessor)
        {
            var checkList = GetCheckListByAssetType(assetType, checkMap);
            if (checkList != null)
            {
                foreach (var check in checkList)
                {
                    if(!check.ExecuteCheck(assetPostProcessor))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 执行指定全局检查器Map指定Asset路径的检查器
        /// </summary>
        /// <param name="checkMap"></param>
        /// <param name="assetPath"></param>
        private static bool ExecuteGlobalCheckMapByAssetPath(Dictionary<AssetType, List<BaseCheck>> checkMap, string assetPath)
        {
            var assetType = AssetPipelineSystem.GetAssetTypeByPath(assetPath);
            var checkList = GetCheckListByAssetType(assetType, checkMap);
            if (checkList != null)
            {
                foreach (var check in checkList)
                {
                    if(!check.ExecuteCheckByPath(assetPath))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 执行指定局部检查器Map指定Asset类型的检查器
        /// </summary>
        /// <param name="localCheckMap"></param>
        /// <param name="assetType"></param>
        /// <param name="assetPostProcessor"></param>
        private static bool ExecuteLocalCheckMapByAssetType(Dictionary<string, Dictionary<AssetType, List<BaseCheck>>> localCheckMap, AssetType assetType, AssetPostprocessor assetPostProcessor)
        {
            return true;
        }

        /// <summary>
        /// 执行指定局部检查器Map指定Asset路径的检查器
        /// </summary>
        /// <param name="localCheckMap"></param>
        /// <param name="assetPath"></param>
        private static bool ExecuteLocalCheckMapByAssetPath(Dictionary<string, Dictionary<AssetType, List<BaseCheck>>> localCheckMap, string assetPath)
        {
            var assetType = AssetPipelineSystem.GetAssetTypeByPath(assetPath);
            return true;
        }
        #endregion
    }
}