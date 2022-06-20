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
        /// 初始化
        /// </summary>
        public static void Init()
        {
            Debug.Log($"AssetCheckSystem:Init()");
            MakeSureCheckSaveFolderExit();
            MakeSureStrategyFolderExist();
            var activeStrategyName = AssetPipelineSystem.GetActiveTargetStrategyName();
            GlobalData = LoadGlobalDataByStrategy(activeStrategyName);
            LocalData = LoadLocalDataByStrategy(activeStrategyName);
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
    }
}