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
        /// Asset类型和GUI Icon映射Map<Asset类型, GUI Icon>
        /// </summary>
        private static Dictionary<AssetType, GUIContent> AssetTypeGUIIconMap = new Dictionary<AssetType, GUIContent>
        {
            { AssetType.None, AssetPipelineGUIContent.FavoriteIcon },
            { AssetType.Texture, AssetPipelineGUIContent.Texture2DIcon },
            { AssetType.Material, AssetPipelineGUIContent.MaterialIcon },
            { AssetType.SpriteAtlas, AssetPipelineGUIContent.SpriteAtlasIcon },
            { AssetType.FBX, AssetPipelineGUIContent.FBXIcon },
            { AssetType.AudioClip, AssetPipelineGUIContent.AudioClipIcon },
            { AssetType.Font, AssetPipelineGUIContent.FontIcon },
            { AssetType.Shader, AssetPipelineGUIContent.ShaderIcon },
            { AssetType.Prefab, AssetPipelineGUIContent.PrefabIcon },
            { AssetType.ScriptableObject, AssetPipelineGUIContent.ScriptableObjectIocn },
            { AssetType.TextAsset, AssetPipelineGUIContent.TextAssetIcon },
            { AssetType.Scene, AssetPipelineGUIContent.SceneIcon },
            { AssetType.AnimationClip, AssetPipelineGUIContent.AnimationClipIcon },
            { AssetType.Mesh, AssetPipelineGUIContent.MeshIcon },
            { AssetType.Script, AssetPipelineGUIContent.ScriptIcon },
            { AssetType.Folder, AssetPipelineGUIContent.FolderIcon },
            { AssetType.Other, AssetPipelineGUIContent.HelpIcon },
        };

        /// <summary>
        /// 存储相对路径(相对Application.dataPath)
        /// </summary>
        private const string SAVE_FOLDER_RELATIVE_PATH = "Assets/Editor/AssetPipeline/Config";

        /// <summary>
        /// Asset管线数据文件名
        /// </summary>
        private const string AssetPipelineSettingDataName = "AssetPipelineSettingData";

        /// <summary>
        /// Asset管线设置数据
        /// </summary>
        private static AssetPipelineSettingData SettingData;

        /// <summary>
        /// Asset管线开关
        /// </summary>
        public static bool Switch
        {
            get
            {
                return SettingData.Switch;
            }
        }

        /// <summary>
        /// Asset黑名单Map<Asset路径, 是否在黑名单>
        /// </summary>
        private static Dictionary<string, bool> BlackListAssetPathMap = new Dictionary<string, bool>
        {
            { GetSettingDataRelativePath(), true },
        };

        // 存储目录结构展示:
        // -- Assets
        //    -- Editor
        //        -- AssetPipeline(AssetPipeline存储根目录)
        //            -- Config
        //                -- AssetPipelineSettingData.asset(Asset管线设置数据)
        //                -- Strategy(配置对应平台策略名)
        //                    -- AssetProcessor(策略Asset处理器数据保存目录)
        //                        -- AssetProcessorGlobalData.asset(策略Asset处理器全局数据)
        //                        -- AssetProcessorLocalData.asset(策略Asset处理器局部数据)
        //                    -- AssetCheck(策略Asset检查器数据保存目录)
        //                        -- AssetCheckGlobalData.asset(策略Asset检查器全局数据)
        //                        -- AssetCheckLocalData.asset(策略Asset检查器局部数据)
        //                -- AssetProcessors(自定处理器ScriptableObject数据)
        //                -- AssetChecks(自定检查器ScriptableObject数据)

        // Asset管线流程:
        // 1. Asset管线初始化(Unity启动时)
        // 2. 初始化Asset管线系统(Asset处理器系统和Asset检查系统)
        // 3. Asset导入,移动或删除等操作
        // 4. 触发Asset导入全局预检查(先触发Object类型,后触发非Obejct类型)
        // 5. 触发Asset导入局部预检查(先触发Object类型,后触发非Obejct类型)
        // 6. 触发Asset导入全局预处理(先触发Object类型,后触发非Obejct类型)
        // 7. 触发Asset导入局部预处理(先触发Object类型,后触发非Obejct类型)
        // 8. 触发Asset导入全局后处理(先触发非Object类型,后触发Object类型)
        // 9. 触发Asset导入局部后处理(先触发非Object类型,后触发Object类型)
        // 10. 触发Asset导入全局后检查(先触发非Object类型,后触发Object类型)
        // 11. 触发Asset导入局部后检查(先触发非Object类型,后触发Object类型)

        // Note:
        // 1. Asset管线默认不支持处理脚本Asset
        // 2. Asset管线先执行全局后局部,局部Asset处理由外到内执行(覆盖关系)
        // 3. Asset管线移动Asset当做重新导入Asset处理，确保Asset移动后得到正确的Asset管线处理
        // 4. Asset管线不满足条件的不会触发(比如: 1. 不在目标资源目录下 2. 是脚本Asset 3. 在黑名单列表里)

        static AssetPipelineSystem()
        {
            Debug.Log($"AssetPipelineSystem()");
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            Debug.Log($"Asset管线系统初始化".WithColor(Color.red));
            MakeSureSaveFolderExist();
            SettingData = LoadSettingData();
            var activeTarget = EditorUserBuildSettings.activeBuildTarget;
            var strategyName = GetActiveTargetStrategyName();
            Debug.Log($"Asset管线开关:{SettingData.Switch}".WithColor(Color.red));
            Debug.Log($"Asset管线Log开关:{SettingData.LogSwitch}".WithColor(Color.red));
            Debug.Log($"加载当前激活平台:{activeTarget}的Asset管线策略:{strategyName}".WithColor(Color.red));
            AssetPipelineLog.Switch = SettingData.LogSwitch;
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
            // Note:
            // 文件目录删除时判定不出正确的目录类型
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                return AssetType.Folder;
            }
            return GetAssetTypeByPostFix(postFix);
        }

        /// <summary>
        /// 获取指定后缀名的Asset类型
        /// </summary>
        /// <param name="postFix"></param>
        /// <returns></returns>
        private static AssetType GetAssetTypeByPostFix(string postFix)
        {
            postFix = postFix.ToLower();
            AssetType assetType;
            if (PostFixAssetTypeMap.TryGetValue(postFix, out assetType))
            {
                return assetType;
            }
            // 找不到默认当做Other类型处理
            Debug.LogWarning($"找不到后缀:{postFix}的Asset类型!");
            return AssetType.Other;
        }

        /// <summary>
        /// 获取指定Asset类型的Asset icon
        /// </summary>
        /// <param name="assetType"></param>
        /// <returns></returns>
        public static GUIContent GetAssetIconByAssetType(AssetType assetType)
        {
            GUIContent guiContent;
            if (AssetTypeGUIIconMap.TryGetValue(assetType, out guiContent))
            {
                return guiContent;
            }
            return AssetPipelineGUIContent.HelpIcon;
        }

        /// <summary>
        /// 获取所有通用的Asset类型组合
        /// </summary>
        /// <returns></returns>
        public static AssetType GetAllCommonAssetType()
        {
            return AssetType.Texture | AssetType.Material | AssetType.SpriteAtlas |
                       AssetType.FBX | AssetType.AudioClip | AssetType.Font |
                       AssetType.Shader | AssetType.Prefab | AssetType.ScriptableObject |
                       AssetType.TextAsset | AssetType.Scene | AssetType.AnimationClip |
                       AssetType.Mesh;
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
                Debug.Log($"创建新的Asset管线配置数据!".WithColor(Color.green));
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
        /// 指定Asset相对路径是否有效
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        private static bool IsValideByAssetPath(string assetPath)
        {
            if(string.IsNullOrEmpty(assetPath))
            {
                return false;
            }
            if(!IsUnderResourceFolder(assetPath))
            {
                return false;
            }
            var assetType = GetAssetTypeByPath(assetPath);
            if(assetType == AssetType.Script)
            {
                return false;
            }
            if(IsAssetPathInBlackList(assetPath))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 指定Asset相对路径是否在黑名单列表
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        private static bool IsAssetPathInBlackList(string assetPath)
        {
            bool result;
            if (!BlackListAssetPathMap.TryGetValue(assetPath, out result))
            {
                return false;
            }
            return result;
        }

        /// <summary>
        /// 指定Asset路径是否在目标资源目录下
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        private static bool IsUnderResourceFolder(string assetPath)
        {
            return assetPath.StartsWith(SettingData.ResourceFolder);
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

        #region Asset管线流程
        /// <summary>
        /// 指定Asset类型预处理
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="assetPostProcessor"></param>
        /// <param name="paramList">不定长参数列表</param>
        public static void OnPreprocessByAssetType(AssetType assetType, AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            AssetPipelineLog.Log($"AssetPipelineSystem:OnPreprocessByAssetType({assetType})");
            if(IsValideByAssetPath(assetPostProcessor.assetPath))
            {
                // 预处理先执行Asset检查系统，后执行Asset处理系统
                AssetCheckSystem.OnPreCheckByAssetType(assetType, assetPostProcessor, paramList);
                AssetProcessorSystem.OnPreprocessByAssetType(assetType, assetPostProcessor, paramList);
            }
        }

        /// <summary>
        /// 后处理导入Asset
        /// </summary>
        /// <param name="importedAsset"></param>
        public static void OnPostprocessImportedAsset(string importedAsset)
        {
            AssetPipelineLog.Log($"AssetPipelineSystem:OnPostprocessImportedAsset()");
            if (Switch)
            {
                if (IsValideByAssetPath(importedAsset))
                {
                    // 后处理先执行Asset处理系统，后执行Asset检查系统
                    var assetType = AssetPipelineSystem.GetAssetTypeByPath(importedAsset);
                    AssetProcessorSystem.OnPostprocessByAssetType2(assetType, importedAsset);
                    AssetCheckSystem.OnPostCheckByAssetType2(assetType, importedAsset);
                }
            }
        }

        /// 后处理移除Asset
        /// </summary>
        /// <param name="deletedAsset"></param>
        public static void OnPostprocessDeletedAsset(string deletedAsset)
        {
            AssetPipelineLog.Log($"AssetPipelineSystem:OnPostprocessDeletedAsset()");
            if (Switch)
            {
                if (IsValideByAssetPath(deletedAsset))
                {
                    // 删除后统根据Asset路径对应类型来触发
                    AssetProcessorSystem.OnPostprocessDeletedByAssetPath(deletedAsset);
                }
            }
        }

        /// 后处理移动Asset
        /// </summary>
        /// <param name="movedAsset">
        /// <param name="paramList">不定长参数列表(未来用于支持Unity更多的AssetPostprocessor接口传参)</param>
        public static void OnPostprocessMovedAsset(string movedAsset, params object[] paramList)
        {
            AssetPipelineLog.Log($"AssetPipelineSystem:OnPostprocessMovedAsset()");
            if (Switch)
            {
                if (IsValideByAssetPath(movedAsset))
                {
                    // 移动后统根据Asset路径对应类型来触发
                    AssetProcessorSystem.OnPostprocessMovedByAssetPath(movedAsset, paramList);
                    // 移动统一当做重新导入处理,确保Asset移动后Asset管线流程处理正确
                    AssetDatabase.ImportAsset(movedAsset);
                }
            }
        }

        /// <summary>
        /// 后处理指定Asset类型的AssetPostprocessor
        /// </summary>
        /// <param name="">assetType</param>
        /// <param name="">assetPostProcessor</param>
        /// <param name="paramList">不定长参数列表(未来用于支持Unity更多的AssetPostprocessor接口传参)</param>
        public static void OnPostprocessByAssetType(AssetType assetType, AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            AssetPipelineLog.Log($"AssetPipelineSystem:OnPostprocessByAssetType({assetType})");
            if (IsValideByAssetPath(assetPostProcessor.assetPath))
            {
                // 后处理先执行Asset处理系统，后执行Asset检查系统
                AssetProcessorSystem.OnPostprocessByAssetType(assetType, assetPostProcessor, paramList);
                AssetCheckSystem.OnPostCheckByAssetType(assetType, assetPostProcessor, paramList);
            }
        }
        #endregion
    }
}