/*
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
        /// 全局预检查器映射Map<Asset管线处理类型, <目标Asset类型, 检查器列表>>
        /// </summary>
        private static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseCheck>>> GlobalPreCheckMap;

        /// <summary>
        /// 全局后检查器映射Map<Asset管线处理类型, <目标Asset类型, 检查器列表>>
        /// </summary>
        private static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseCheck>>> GlobalPostCheckMap;

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
            var allCheckGuids = AssetDatabase.FindAssets($"t:{AssetPipelineConst.BASE_CHECK_TYPE}", new[] { checkSaveRelativePath });
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
        /// 获取全局预检查器Map<Asset管线处理类型, <目标Asset类型, 检查器列表>>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseCheck>>> GetGlobalPreCheckMap()
        {
            return GetGlobalCheckMap(GlobalData.PreCheckData.CheckList, "预");
        }

        /// <summary>
        /// 获取全局后检查器Map<Asset管线处理类型, <目标Asset类型, 检查器列表>>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseCheck>>> GetGlobalPostCheckMap()
        {
            return GetGlobalCheckMap(GlobalData.PostCheckData.CheckList, "后");
        }

        /// <summary>
        /// 获取全局指定检查器的Map<Asset管线处理类型, <目标Asset类型, 检查器列表>>
        /// </summary>
        /// <param name="checkList"></param>
        /// <param name="tip"></param>
        /// <returns></returns>
        private static Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseCheck>>> GetGlobalCheckMap(List<BaseCheck> checkList, string tip = "")
        {
            Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseCheck>>> globalCheckMap = new Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseCheck>>>();
            foreach (var check in checkList)
            {
                if(check != null)
                {
                    Dictionary<AssetType, List<BaseCheck>> assetTypeCheckMap;
                    if (!globalCheckMap.TryGetValue(check.TargetAssetProcessType, out assetTypeCheckMap))
                    {
                        assetTypeCheckMap = new Dictionary<AssetType, List<BaseCheck>>();
                        globalCheckMap.Add(check.TargetAssetProcessType, assetTypeCheckMap);
                    }
                    foreach (var assetTypeValue in AssetPipelineConst.ASSET_TYPE_VALUES)
                    {
                        var assetType = (AssetType)assetTypeValue;
                        if ((check.TargetAssetType & assetType) != AssetType.None)
                        {
                            List<BaseCheck> assetTypeCheckList;
                            if (!assetTypeCheckMap.TryGetValue(assetType, out assetTypeCheckList))
                            {
                                assetTypeCheckList = new List<BaseCheck>();
                                assetTypeCheckMap.Add(assetType, assetTypeCheckList);
                            }
                            assetTypeCheckList.Add(check);
                            AssetPipelineLog.Log($"添加Asset管线处理类型:{check.TargetAssetProcessType},Asset类型:{assetType}的全局{tip}检查器:{check.Name}!".WithColor(Color.yellow));
                        }
                    }
                }
            }
            return globalCheckMap;
        }

        /// <summary>
        /// 初始化所有局部检查器信息
        /// </summary>
        private static void InitLocalCheckData()
        {
            PrintCheckLocalData(LocalData.PreCheckDataList, "预");
            PrintCheckLocalData(LocalData.PostCheckDataList, "后");
        }

        /// <summary>
        /// 打印局部检查器数据列表
        /// </summary>
        /// <param name="checkLocalDataList"></param>
        /// <param name="tip"></param>
        private static void PrintCheckLocalData(List<CheckLocalData> checkLocalDataList, string tip = "")
        {
            foreach (var checkLocalData in checkLocalDataList)
            {
                AssetPipelineLog.Log($"局部{tip}检查器目标目录:{checkLocalData.FolderPath}".WithColor(Color.yellow));
                foreach (var checkData in checkLocalData.CheckDataList)
                {
                    if (checkData.Check != null)
                    {
                        AssetPipelineLog.Log($"添加局部检查器:{checkData.Check.Name}!".WithColor(Color.yellow));
                    }
                }
            }
        }

        /// <summary>
        /// 获取指定Asset处理器Map里的指定Asset类型的检查器列表
        /// </summary>
        /// <param name="assetProcessType"></param>
        /// <param name="assetType"></param>
        /// <param name="checkMap"></param>
        /// <returns></returns>
        private static List<BaseCheck> GetCheckListByAssetType(AssetProcessType assetProcessType, AssetType assetType,
                                                                Dictionary<AssetProcessType,  Dictionary<AssetType, List<BaseCheck>>> checkMap)
        {
            if (checkMap == null)
            {
                return null;
            }
            Dictionary<AssetType, List<BaseCheck>> assetTypeCheckMap;
            if(!checkMap.TryGetValue(assetProcessType, out assetTypeCheckMap))
            {
                return null;
            }
            List<BaseCheck> checkList;
            if (!assetTypeCheckMap.TryGetValue(assetType, out checkList))
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
                Debug.Log($"创建新的Asset检查器全局数据!".WithColor(Color.green));
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
                Debug.Log($"创建新的Asset检查器局部数据!".WithColor(Color.green));
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
        /// <param name="assetProcessType"></param>
        /// <param name="assetType"></param>
        /// <param name="assetPostProcessor"></param>
        /// <param name="paramList">不定长参数列表</param>
        public static bool OnPreCheckByAssetType(AssetProcessType assetProcessType, AssetType assetType, AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            AssetPipelineLog.Log($"开始执行预检查全局处理器:".WithColor(Color.yellow));
            if (!ExecuteGlobalCheckMapByAssetType(GlobalPreCheckMap, assetProcessType, assetType, assetPostProcessor, paramList))
            {
                return false;
            }
            AssetPipelineLog.Log($"开始执行预检查全局处理器:".WithColor(Color.yellow));
            if (!ExecuteLocalCheckMapByAssetType(LocalData.PreCheckDataList, assetProcessType, assetType, assetPostProcessor, paramList))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 后处理导入指定Asset类型
        /// </summary>
        /// <param name="assetProcessType"></param>
        /// <param name="assetType"></param>
        /// <param name="assetPostProcessor"></param>
        /// <param name="paramList">不定长参数列表</param>
        public static bool OnPostCheckByAssetType(AssetProcessType assetProcessType, AssetType assetType, AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            AssetPipelineLog.Log($"开始执行后处理assetPostProcessor全局处理器:".WithColor(Color.yellow));
            if (!ExecuteGlobalCheckMapByAssetType(GlobalPostCheckMap, assetProcessType, assetType, assetPostProcessor, paramList))
            {
                return false;
            }
            AssetPipelineLog.Log($"开始执行后处理assetPostProcessor局部处理器:".WithColor(Color.yellow));
            if (ExecuteLocalCheckMapByAssetType(LocalData.PostCheckDataList, assetProcessType, assetType, assetPostProcessor, paramList))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 后处理导入指定Asset类型和指定Asset路径
        /// </summary>
        /// <param name="assetProcessType"></param>
        /// <param name="assetType"></param>
        /// <param name="assetPath"></param>
        public static bool OnPostCheckByAssetType2(AssetProcessType assetProcessType, AssetType assetType, string assetPath)
        {
            AssetPipelineLog.Log($"开始执行后处理assetPath全局处理器:".WithColor(Color.yellow));
            if (!ExecuteGlobalCheckMapByAssetType2(GlobalPostCheckMap, assetProcessType, assetType, assetPath))
            {
                return false;
            }
            AssetPipelineLog.Log($"开始执行后处理assetPath局部处理器:".WithColor(Color.yellow));
            if (!ExecuteLocalCheckMapByAssetType2(LocalData.PostCheckDataList, assetProcessType, assetType, assetPath))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 执行指定全局检查器Map指定Asset类型的检查器
        /// </summary>
        /// <param name="checkMap"></param>
        /// <param name="assetProcessType"></param>
        /// <param name="assetType"></param>
        /// <param name="assetPostProcessor"></param>
        /// <param name="paramList">不定长参数列表</param>
        private static bool ExecuteGlobalCheckMapByAssetType(Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseCheck>>> checkMap,
                                                                AssetProcessType assetProcessType, AssetType assetType, AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            var checkList = GetCheckListByAssetType(assetProcessType, assetType, checkMap);
            return ExecuteChecksByAssetPostprocessor(checkList, assetPostProcessor, paramList);
        }

        /// <summary>
        /// 执行指定全局检查器Map指定Asset类型和指定Asset路径的检查器
        /// </summary>
        /// <param name="checkMap"></param>
        /// <param name="assetType"></param>
        /// <param name="assetPath"></param>
        private static bool ExecuteGlobalCheckMapByAssetType2(Dictionary<AssetProcessType, Dictionary<AssetType, List<BaseCheck>>> checkMap,
                                                                 AssetProcessType assetProcessType, AssetType assetType, string assetPath)
        {
            var checkList = GetCheckListByAssetType(assetProcessType, assetType, checkMap);
            return ExecuteChecksByAssetPath(checkList, assetPath);
        }

        // 局部检查器触发规则:
        // 1. 由外往内寻找符合目标目录设置的局部检查器配置触发

        /// <summary>
        /// 执行指定局部检查器Map指定Asset类型的检查器
        /// </summary>
        /// <param name="checkLocalDataList"></param>
        /// <param name="assetProcessType">
        /// <param name="assetType"></param>
        /// <param name="assetPostProcessor"></param>
        /// <param name="paramList">不定长参数列表</param>
        private static bool ExecuteLocalCheckMapByAssetType(List<CheckLocalData> checkLocalDataList, AssetProcessType assetProcessType,
                                                                AssetType assetType, AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            var assetPath = assetPostProcessor.assetPath;
            foreach (var checkLocalData in checkLocalDataList)
            {
                if (!checkLocalData.IsInTargetFolder(assetPath))
                {
                    continue;
                }
                foreach (var checkData in checkLocalData.CheckDataList)
                {
                    if (checkData.IsValideAssetProcessType(assetProcessType) &&
                            checkData.IsValideAssetType(assetType) && !checkData.IsInBlackList(assetPath))
                    {
                        ExecuteCheckByAssetPostprocessor(checkData.Check, assetPostProcessor, paramList);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 执行指定局部检查器Map指定Asset类型和指定Asset路径的检查器
        /// </summary>
        /// <param name="checkLocalDataList"></param>
        /// <param name="assetProcessType"></param>
        /// <param name="assetType"></param>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        private static bool ExecuteLocalCheckMapByAssetType2(List<CheckLocalData> checkLocalDataList, AssetProcessType assetProcessType, AssetType assetType,
                                                                string assetPath, params object[] paramList)
        {
            foreach (var checkLocalData in checkLocalDataList)
            {
                if (!checkLocalData.IsInTargetFolder(assetPath))
                {
                    continue;
                }
                foreach (var checkData in checkLocalData.CheckDataList)
                {
                    if (checkData.IsValideAssetProcessType(assetProcessType) &&
                            checkData.IsValideAssetType(assetType) && !checkData.IsInBlackList(assetPath))
                    {
                        if (!ExecuteCheckByAssetPath(checkData.Check, assetPath, paramList))
                        {
                            // 未来支持检查不满足就返回的情况可以打开这里
                            //return false;
                        }
                    }
                }
            }
            return true;
        }

        ///// <summary>
        ///// 执行指定局部检查器Map指定Asset路径的检查器
        ///// </summary>
        ///// <param name="checkLocalDataList"></param>
        ///// <param name="assetPath"></param>
        //private static bool ExecuteLocalCheckMapByAssetPath(List<CheckLocalData> checkLocalDataList, string assetPath)
        //{
        //    var assetType = AssetPipelineSystem.GetAssetTypeByPath(assetPath);
        //    return ExecuteLocalCheckMapByAssetType2(checkLocalDataList, assetType, assetPath);
        //}

        /// <summary>
        /// 触发指定Asset路径检查器
        /// </summary>
        /// <param name="checkList"></param>
        /// <param name="assetPath"></param>
        private static bool ExecuteChecksByAssetPath(List<BaseCheck> checkList, string assetPath)
        {
            if (checkList != null && !string.IsNullOrEmpty(assetPath))
            {
                foreach (var check in checkList)
                {
                    if (!ExecuteCheckByAssetPath(check, assetPath))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 指定Asset路径触发检查器
        /// </summary>
        /// <param name="check"></param>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        private static bool ExecuteCheckByAssetPath(BaseCheck check, string assetPath, params object[] paramList)
        {
            if (check != null && !string.IsNullOrEmpty(assetPath))
            {
                if(!check.ExecuteCheckByPath(assetPath, paramList))
                {
                    // 未来支持检查不满足就返回的情况可以打开这里
                    //return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 触发指定Asset路径的检查器
        /// </summary>
        /// <param name="checkList"></param>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        private static bool ExecuteChecksByAssetPostprocessor(List<BaseCheck> checkList, AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            if (checkList != null && assetPostProcessor != null)
            {
                foreach (var check in checkList)
                {
                    if(!ExecuteCheckByAssetPostprocessor(check, assetPostProcessor, paramList))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 指定AssetPostprocessor触发检查器
        /// </summary>
        /// <param name="check"></param>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        private static bool ExecuteCheckByAssetPostprocessor(BaseCheck check, AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            if (check != null && assetPostProcessor != null)
            {
                if(!check.ExecuteCheck(assetPostProcessor, paramList))
                {
                    // 未来支持检查不满足就返回的情况可以打开这里
                    //return false;
                }
            }
            return true;
        }
        #endregion
    }
}