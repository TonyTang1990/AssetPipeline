/*
 * Description:             AssetPipelineSystem.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/17
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static TAssetPipeline.AssetPipelineSettingData;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetPipelineSystem.cs
    /// Asset管线系统
    /// </summary>
    [InitializeOnLoad]
    public static class AssetPipelineSystem
    {
        /// <summary>
        /// 文件后缀名和Asset类型映射Map<文件后缀名, Asset类型>
        /// </summary>
        private static Dictionary<string, AssetType> PostFixAssetTypeMap = new Dictionary<string, AssetType>
        {
            { ".png", AssetType.Texture },
            { ".jpg", AssetType.Texture },
            { ".ps", AssetType.Texture },
            { ".tif", AssetType.Texture },
            { ".mat", AssetType.Material },
            { ".spriteatlas", AssetType.SpriteAtlas },
            { ".fbx", AssetType.FBX },
            { ".mp3", AssetType.AudioClip },
            { ".ogg", AssetType.AudioClip },
            { ".wav", AssetType.AudioClip },
            { ".ttf", AssetType.Font },
            { ".shader", AssetType.Shader },
            { ".prefab", AssetType.Prefab },
            { ".asset", AssetType.ScriptableObject },
            { ".txt", AssetType.TextAsset },
            { ".unity", AssetType.Scene },
            { ".anim", AssetType.AnimationClip },
            { ".mesh", AssetType.Mesh },
            { ".cs", AssetType.Script },
            { ".dll", AssetType.Script },
            { ".java", AssetType.Script },
        };

        /// <summary>
        /// 存储相对路径(相对Application.dataPath)
        /// </summary>
        private const string SAVE_FOLDER_RELATIVE_PATH = "Assets/Editor/AssetPipeline";

        /// <summary>
        /// Asset管线数据文件名
        /// </summary>
        private const string AssetPipelineSettingDataName = "AssetPipelineSettingData";

        /// <summary>
        /// Asset管线设置数据
        /// </summary>
        private static AssetPipelineSettingData SettingData;

        // 存储目录结构展示:
        // -- Assets
        //    -- Editor
        //        -- AssetPipeline(AssetPipeline存储根目录)
        //            -- AssetPipelineSettingData.asset(Asset管线设置数据)
        //            -- Strategy(配置对应平台策略名)
        //                -- AssetProcessor(策略Asset处理器数据保存目录)
        //                    -- AssetProcessorGlobalData.asset(策略Asset处理器全局数据)
        //                    -- AssetProcessorLocalData.asset(策略Asset处理器局部数据)
        //                -- AssetCheck(策略Asset检查器数据保存目录)
        //                    -- AssetCheckGlobalData.asset(策略Asset检查器全局数据)
        //                    -- AssetCheckLocalData.asset(策略Asset检查器局部数据)
        //            -- AssetProcessors(自定处理器ScriptableObject数据)
        //            -- AssetChecks(自定检查器ScriptableObject数据)

        static AssetPipelineSystem()
        {
            Debug.Log($"AssetPipelineSystem()");
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private static void Init()
        {
            Debug.Log($"AssetPipelineSystem:Init()");
            MakeSureSaveFolderExist();
            SettingData = LoadSettingData();
            MakeActiveTargetStrategyFolderExist();
            AssetProcessorSystem.Init();
            AssetCheckSystem.Init();
        }

        /// <summary>
        /// 获取指定Asset路径的Asset类型
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static AssetType GetAssetTypeByPath(string assetPath)
        {
            var postFix = Path.GetExtension(assetPath);
            return GetAssetTypeByPostFix(postFix);
        }

        /// <summary>
        /// 获取指定后缀名的Asset类型
        /// </summary>
        /// <param name="postFix"></param>
        /// <returns></returns>
        public static AssetType GetAssetTypeByPostFix(string postFix)
        {
            postFix = postFix.ToLower();
            AssetType assetType = AssetType.None;
            if (PostFixAssetTypeMap.TryGetValue(postFix, out assetType))
            {
                return assetType;
            }
            Debug.LogError($"找不到后缀:{postFix}的Asset类型!");
            return AssetType.None;
        }

        /// <summary>
        /// 获取保存目录相对路径
        /// </summary>
        /// <returns></returns>
        public static string GetSaveFolderRelativePath()
        {
            return SAVE_FOLDER_RELATIVE_PATH;
        }

        /// <summary>
        /// 获取保存目录全路径路径
        /// </summary>
        /// <returns></returns>
        public static string GetSaveFolderFullPath()
        {
            var saveFolderRelativePath = GetSaveFolderRelativePath();
            return Path.GetFullPath(saveFolderRelativePath);
        }

        /// <summary>
        /// 确保保存目录存在
        /// </summary>
        /// <returns></returns>
        private static void MakeSureSaveFolderExist()
        {
            var saveFolderFullPath = GetSaveFolderFullPath();
            FolderUtilities.CheckAndCreateSpecificFolder(saveFolderFullPath);
        }

        /// <summary>
        /// 确保激活平台配置策略目录存在
        /// </summary>
        private static void MakeActiveTargetStrategyFolderExist()
        {
            var strategyName = GetActiveTargetStrategyName();
            MakeSureSaveFolderExistByStrategy(strategyName);
        }

        /// <summary>
        /// 加载设置数据
        /// </summary>
        public static AssetPipelineSettingData LoadSettingData()
        {
            var settingDataRelativePath = GetSettingDataRelativePath();
            var settingData = AssetDatabase.LoadAssetAtPath<AssetPipelineSettingData>(settingDataRelativePath);
            if(settingData == null)
            {
                settingData = ScriptableObject.CreateInstance<AssetPipelineSettingData>();
                AssetDatabase.CreateAsset(settingData, settingDataRelativePath);
            }
            return settingData;
        }

        /// <summary>
        /// 获取Asset管线设置数据相对路径
        /// </summary>
        /// <returns></returns>
        private static string GetSettingDataRelativePath()
        {
            var saveFolderRelativePath = GetSaveFolderRelativePath();
            return $"{saveFolderRelativePath}/{AssetPipelineSettingDataName}.asset";
        }

        /// <summary>
        /// 获取当前激活平台配置策略名
        /// </summary>
        /// <returns></returns>
        public static string GetActiveTargetStrategyName()
        {
            var activeTarget = EditorUserBuildSettings.activeBuildTarget;
            var activeTargetPlatformData = SettingData.PlatformStrategyDataList.Find(platformStrategyData => platformStrategyData.Target == activeTarget);
            return activeTargetPlatformData.StrategyName;
        }

        /// <summary>
        /// 获取指定策略的数据保存相对路径
        /// </summary>
        /// <param name="strategyName"></param>
        /// <returns></returns>
        public static string GetDataSaveFolderByStrategy(string strategyName)
        {
            var strategyDataFolderRelativePath = $"{GetSaveFolderRelativePath()}/{strategyName}";
            return strategyDataFolderRelativePath;
        }

        /// <summary>
        /// 确保指定策略目录存在
        /// </summary>
        /// <param name="strategyName"></param>
        public static void MakeSureSaveFolderExistByStrategy(string strategyName)
        {
            var strategyFolderRelativePath = GetDataSaveFolderByStrategy(strategyName);
            var strategyFolderFullPath = Path.GetFullPath(strategyFolderRelativePath);
            FolderUtilities.CheckAndCreateSpecificFolder(strategyFolderFullPath);
        }
    }
}