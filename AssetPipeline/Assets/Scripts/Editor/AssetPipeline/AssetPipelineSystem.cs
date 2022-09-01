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
    public static class AssetPipelineSystem
    {
        /// <summary>
        /// 文件后缀名和Asset类型映射Map<文件后缀名, Asset类型>
        /// </summary>
        private static Dictionary<string, AssetType> PostFixAssetTypeMap = new Dictionary<string, AssetType>
        {
            { ".png", AssetType.Texture },
            { ".jpg", AssetType.Texture },
            { ".psd", AssetType.Texture },
            { ".tif", AssetType.Texture },
            { ".exr", AssetType.Texture },
            { ".mat", AssetType.Material },
            { ".spriteatlas", AssetType.SpriteAtlas },
            { ".fbx", AssetType.FBX },
            { ".mp3", AssetType.AudioClip },
            { ".ogg", AssetType.AudioClip },
            { ".wav", AssetType.AudioClip },
            { ".ttf", AssetType.Font },
            { ".shader", AssetType.Shader },
            { ".compute", AssetType.Shader },
            { ".shadervariants", AssetType.ShaderVariantCollection },
            { ".prefab", AssetType.Prefab },
            { ".asset", AssetType.ScriptableObject },
            { ".txt", AssetType.TextAsset },
            { ".json", AssetType.TextAsset },
            { ".bytes", AssetType.TextAsset },
            { ".cginc", AssetType.TextAsset },
            { ".glslinc", AssetType.TextAsset },
            { ".uxml", AssetType.TextAsset },
            { ".xml", AssetType.TextAsset },
            { ".unity", AssetType.Scene },
            { ".anim", AssetType.AnimationClip },
            { ".controller", AssetType.AnimatorController },
            { ".overridecontroller", AssetType.AnimatorOverrideController },
            { ".mesh", AssetType.Mesh },
            { ".mp4", AssetType.VideoClip },
            { ".rendertexture", AssetType.RenderTexture },
            { ".playable", AssetType.TimelineAsset },
            { ".lighting", AssetType.LightingSetting },
            { ".cs", AssetType.Script },
            { ".guiskin", AssetType.GUISkin },
            { ".preset", AssetType.Preset },
            { ".asmdef", AssetType.AssemblyDefinitionAsset },
            { ".uss", AssetType.StyleSheet },
            { ".java", AssetType.DefaultAsset },
            { ".h", AssetType.DefaultAsset },
            { ".mm", AssetType.DefaultAsset },
            { ".cpp", AssetType.DefaultAsset },
            { ".py", AssetType.DefaultAsset },
            { ".dll", AssetType.DefaultAsset },
            { ".a", AssetType.DefaultAsset },
            { ".so", AssetType.DefaultAsset },
            { ".exe", AssetType.DefaultAsset },
            { ".prefs", AssetType.DefaultAsset },
            { ".plist", AssetType.DefaultAsset },
            { ".md", AssetType.DefaultAsset },
            { ".tpsheet", AssetType.DefaultAsset },
            { ".api", AssetType.DefaultAsset },
            { ".unitypackage", AssetType.DefaultAsset },
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
            { AssetType.AnimatorController, AssetPipelineGUIContent.AnimatorControllerIcon },
            { AssetType.AnimatorOverrideController, AssetPipelineGUIContent.AnimatorOverrideControllerIcon },
            { AssetType.Mesh, AssetPipelineGUIContent.MeshIcon },
            { AssetType.VideoClip, AssetPipelineGUIContent.VideoClipIcon },
            { AssetType.RenderTexture, AssetPipelineGUIContent.RenderTextureIcon },
            { AssetType.TimelineAsset, AssetPipelineGUIContent.TimelineAssetIcon },
            { AssetType.LightingSetting, AssetPipelineGUIContent.LightingSettingsIcon },
            { AssetType.Script, AssetPipelineGUIContent.ScriptIcon },
            { AssetType.Folder, AssetPipelineGUIContent.FolderIcon },
            { AssetType.GUISkin, AssetPipelineGUIContent.GUISkinIcon },
            { AssetType.Preset, AssetPipelineGUIContent.PresetIcon },
            { AssetType.AssemblyDefinitionAsset, AssetPipelineGUIContent.AssemblyDefinitionAssetIcon },
            { AssetType.DefaultAsset, AssetPipelineGUIContent.DefaultAssetIcon },
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
                return SettingData != null ? SettingData.Switch : false;
            }
        }

        /// <summary>
        /// 处理器系统开关
        /// </summary>
        public static bool ProcessorSystemSwitch
        {
            get
            {
                return SettingData != null ? SettingData.ProcessorSystemSwitch : false;
            }
        }

        /// <summary>
        /// 检查器系统开关
        /// </summary>
        public static bool CheckSystemSwitch
        {
            get
            {
                return SettingData != null ? SettingData.CheckSystemSwitch : false;
            }
        }

        /// <summary>
        /// 已加载平台
        /// </summary>
        public static BuildTarget LoadedTarget
        {
            get;
            private set;
        }

        /// <summary>
        /// 已加载策略
        /// </summary>
        public static string LoadedStrategyName
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Asset黑名单Map<Asset名, 是否在黑名单>
        /// </summary>
        private static Dictionary<string, bool> BlackListAssetNameMap = new Dictionary<string, bool>
        {
            { GetSettingDataAssetName(), true },
            { AssetProcessorSystem.GetGlobalDataAssetName(), true },
            { AssetProcessorSystem.GetLocalDataAssetName(), true },
            { AssetCheckSystem.GetGlobalDataAssetName(), true },
            { AssetCheckSystem.GetLocalDataAssetName(), true },
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
        // 4. 触发Asset导入全局预检查
        // 5. 触发Asset导入局部预检查
        // 6. 触发Asset导入全局预处理
        // 7. 触发Asset导入局部预处理
        // 8. 触发Asset导入全局后处理
        // 9. 触发Asset导入局部后处理
        // 10. 触发Asset导入全局后检查
        // 11. 触发Asset导入局部后检查

        // Note:
        // 1. Asset管线先执行全局后局部,局部Asset处理由外到内执行(覆盖关系)
        // 2. Asset管线移动Asset当做重新导入Asset处理，确保Asset移动后得到正确的Asset管线处理
        // 3. Asset管线不满足条件的不会触发(比如: 1. 不在目标资源目录下 2. 在黑名单列表里)

        // 特殊保护设定:
        // 在切换平台瞬间有可能触发的Asset导入会先于InitializeOnLoadMethodAttribute标签的执行
        // 导致在Asset管线没有正确加载当前平台策略前触发Asset管线流程
        // 因此为了确保切换平台瞬间Asset管线处理正确
        // 在激活平台和Asset管线初始化平台不一致时不处理Asset管线流程

        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethodAttribute]
        public static void Init()
        {
            Debug.Log($"Asset管线系统初始化".WithColor(Color.red));
            MakeSureSaveFolderExist();
            SettingData = LoadJsonSettingData();
            LoadedTarget = EditorUserBuildSettings.activeBuildTarget;
            LoadedStrategyName = GetActiveTargetStrategyName();
            Debug.Log($"Asset管线开关:{SettingData.Switch}".WithColor(Color.red));
            Debug.Log($"Asset管线Log开关:{SettingData.LogSwitch}".WithColor(Color.red));
            Debug.Log($"加载当前激活平台:{LoadedTarget}的Asset管线策略:{LoadedStrategyName}".WithColor(Color.red));
            AssetPipelineLog.Switch = SettingData.LogSwitch;
            MakeActiveTargetStrategyFolderExist();
            AssetProcessorSystem.Init();
            AssetCheckSystem.Init();
        }

        /// <summary>
        /// 当前激活平台是否已加载
        /// </summary>
        /// <returns></returns>
        public static bool IsActiveTargetLoaded()
        {
            return LoadedTarget == EditorUserBuildSettings.activeBuildTarget;
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
            Debug.LogWarning($"找不到后缀:{postFix}的Asset类型,默认当做Other类型处理!");
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
        /// Note:
        /// 排除以下Asset类型:
        /// 1. Script -- 代码Asset
        /// 2. Folder -- 目录
        /// 3. Editor资源(e.g. GUISkin, Preset, AssemblyDefinitionAsset......)
        /// 4. DefaultAsset -- Unity不识别类型(e.g. dll, exe, so, a......)
        /// 5. Other -- 未找到对应Asset类型
        /// </summary>
        /// <returns></returns>
        public static AssetType GetAllCommonAssetType()
        {
            return AssetType.Texture | AssetType.Material | AssetType.SpriteAtlas |
                       AssetType.FBX | AssetType.AudioClip | AssetType.Font |
                       AssetType.Shader | AssetType.ShaderVariantCollection | AssetType.Prefab |
                       AssetType.ScriptableObject | AssetType.TextAsset | AssetType.Scene |
                       AssetType.AnimationClip | AssetType.AnimatorController |
                       AssetType.AnimatorOverrideController | AssetType.Mesh |
                       AssetType.VideoClip | AssetType.RenderTexture | AssetType.TimelineAsset |
                       AssetType.LightingSetting;
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
            var settingDataRelativePath = $"{GetSettingDataRelativePath()}.asset";
            var settingData = AssetDatabase.LoadAssetAtPath<AssetPipelineSettingData>(settingDataRelativePath);
            if(settingData == null)
            {
                Debug.LogWarning($"找不到Asset管线配置数据:{settingDataRelativePath}，创建默认Asset管线配置数据!".WithColor(Color.yellow));
                settingData = ScriptableObject.CreateInstance<AssetPipelineSettingData>();
                AssetDatabase.CreateAsset(settingData, settingDataRelativePath);
            }
            return settingData;
        }

        /// <summary>
        /// 加载Json设置数据
        /// </summary>
        public static AssetPipelineSettingData LoadJsonSettingData()
        {
            var settingDataRelativePath = $"{GetSettingDataRelativePath()}.json";
            AssetPipelineSettingData settingData = ScriptableObject.CreateInstance<AssetPipelineSettingData>();
            if (!File.Exists(settingDataRelativePath))
            {
                Debug.LogWarning($"找不到Asset管线Json配置数据:{settingDataRelativePath},创建默认Asset管线配置数据!".WithColor(Color.yellow));
                return settingData;
            }
            var settingDataJsonContent = File.ReadAllText(settingDataRelativePath);
            JsonUtility.FromJsonOverwrite(settingDataJsonContent, settingData);
            Debug.Log($"加载Asset管线Json配置数据:{settingDataRelativePath}完成!".WithColor(Color.green));
            return settingData;
        }

        /// <summary>
        /// 保存Assset管线配置数据到Json
        /// </summary>
        /// <param name="assetPipelineSettingData"></param>
        /// <returns></returns>
        public static bool SaveSettingDataToJson(AssetPipelineSettingData assetPipelineSettingData)
        {
            if(assetPipelineSettingData == null)
            {
                Debug.LogError($"不保存空Asset管线配置数据!");
                return false;
            }
            var settingDataJsonPath = $"{AssetPipelineSystem.GetSettingDataRelativePath()}.json";
            var settingDataJsonContent = JsonUtility.ToJson(assetPipelineSettingData, true);
            File.WriteAllText(settingDataJsonPath, settingDataJsonContent);
            Debug.Log($"保存Asset管线配置的Json数据:{settingDataJsonPath}完成!".WithColor(Color.green));
            return true;
        }

        /// <summary>
        /// 获取Asset管线设置数据相对路径
        /// </summary>
        /// <returns></returns>
        public static string GetSettingDataRelativePath()
        {
            var saveFolderRelativePath = GetSaveFolderRelativePath();
            return $"{saveFolderRelativePath}/{GetSettingDataAssetName()}";
        }

        /// <summary>
        /// 获取Asset管线设置数据Asset名字
        /// </summary>
        /// <returns></returns>
        private static string GetSettingDataAssetName()
        {
            return $"{AssetPipelineSettingDataName}";
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
            var assetName = Path.GetFileName(assetPath);
            if (!BlackListAssetNameMap.TryGetValue(assetName, out result))
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
        /// <param name="assetProcessType">Asset管线处理类型</param>
        /// <param name="assetType">Asset类型</param>
        /// <param name="assetPostProcessor">Asset PostProcessor</param>
        /// <param name="paramList">不定长参数列表</param>
        public static void OnPreprocessByAssetType(AssetProcessType assetProcessType, AssetType assetType, AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            AssetPipelineLog.Log($"AssetPipelineSystem:OnPreprocessByAssetType({assetType})");
            if (AssetPipelineSystem.IsActiveTargetLoaded())
            {
                if (Switch && IsValideByAssetPath(assetPostProcessor.assetPath))
                {
                    AssetPipelineLog.Log($"AssetPath:{assetPostProcessor.assetPath} AssetProcessType:{assetProcessType} AssetType:{assetType}".WithColor(Color.red));
                    // 预处理先执行Asset检查系统，后执行Asset处理系统
                    if (CheckSystemSwitch)
                    {
                        AssetCheckSystem.OnPreCheckByAssetType(assetProcessType, assetType, assetPostProcessor, paramList);
                    }
                    if(ProcessorSystemSwitch)
                    {
                        AssetProcessorSystem.OnPreprocessByAssetType(assetProcessType, assetType, assetPostProcessor, paramList);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"当前平台:{EditorUserBuildSettings.activeBuildTarget}和当前已加载平台:{AssetPipelineSystem.LoadedTarget}不一致，关闭Asset管线系统!".WithColor(Color.yellow));
            }
        }

        /// <summary>
        /// 后处理导入Asset
        /// </summary>
        /// <param name="assetProcessType"></param>
        /// <param name="importedAsset"></param>
        public static void OnPostprocessImportedAsset(AssetProcessType assetProcessType, string importedAsset)
        {
            AssetPipelineLog.Log($"AssetPipelineSystem:OnPostprocessImportedAsset()");
            if (AssetPipelineSystem.IsActiveTargetLoaded())
            {
                if (Switch && IsValideByAssetPath(importedAsset))
                {
                    var assetType = AssetPipelineSystem.GetAssetTypeByPath(importedAsset);
                    AssetPipelineLog.Log($"AssetPath:{importedAsset} AssetProcessType:{assetProcessType} AssetType:{assetType}".WithColor(Color.red));
                    // 后处理先执行Asset处理系统，后执行Asset检查系统
                    if (ProcessorSystemSwitch)
                    {
                        AssetProcessorSystem.OnPostprocessByAssetType2(assetProcessType, assetType, importedAsset);
                    }
                    if (CheckSystemSwitch)
                    {
                        AssetCheckSystem.OnPostCheckByAssetType2(assetProcessType, assetType, importedAsset);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"当前平台:{EditorUserBuildSettings.activeBuildTarget}和当前已加载平台:{AssetPipelineSystem.LoadedTarget}不一致，关闭Asset管线系统!".WithColor(Color.yellow));
            }
        }

        /// 后处理移除Asset
        /// </summary>
        /// <param name="assetProcessType"></param>
        /// <param name="deletedAsset"></param>
        public static void OnPostprocessDeletedAsset(AssetProcessType assetProcessType, string deletedAsset)
        {
            AssetPipelineLog.Log($"AssetPipelineSystem:OnPostprocessDeletedAsset()");
            if (AssetPipelineSystem.IsActiveTargetLoaded())
            {
                if (Switch && IsValideByAssetPath(deletedAsset))
                {
                    var assetType = AssetPipelineSystem.GetAssetTypeByPath(deletedAsset);
                    AssetPipelineLog.Log($"AssetPath:{deletedAsset} AssetProcessType:{assetProcessType} AssetType:{assetType}".WithColor(Color.red));
                    // 删除后统根据Asset路径对应类型来触发
                    if (ProcessorSystemSwitch)
                    {
                        AssetProcessorSystem.OnPostprocessDeletedByAssetPath(deletedAsset, assetProcessType);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"当前平台:{EditorUserBuildSettings.activeBuildTarget}和当前已加载平台:{AssetPipelineSystem.LoadedTarget}不一致，关闭Asset管线系统!".WithColor(Color.yellow));
            }
        }

        /// 后处理移动Asset
        /// </summary>
        /// <param name="assetProcessType"></param>
        /// <param name="movedAsset"></param>
        /// <param name="paramList">不定长参数列表(未来用于支持Unity更多的AssetPostprocessor接口传参)</param>
        public static void OnPostprocessMovedAsset(AssetProcessType assetProcessType, string movedAsset, params object[] paramList)
        {
            AssetPipelineLog.Log($"AssetPipelineSystem:OnPostprocessMovedAsset()");
            if (AssetPipelineSystem.IsActiveTargetLoaded())
            {
                if (Switch && IsValideByAssetPath(movedAsset))
                {
                    var assetType = AssetPipelineSystem.GetAssetTypeByPath(movedAsset);
                    AssetPipelineLog.Log($"AssetPath:{movedAsset} AssetProcessType:{assetProcessType} AssetType:{assetType}".WithColor(Color.red));
                    // 移动后统根据Asset路径对应类型来触发
                    if (ProcessorSystemSwitch)
                    {
                        AssetProcessorSystem.OnPostprocessMovedByAssetPath(movedAsset, assetProcessType, paramList);
                    }
                    // 移动统一当做重新导入处理,确保Asset移动后Asset管线流程处理正确
                    AssetDatabase.ImportAsset(movedAsset);
                }
            }
            else
            {
                Debug.LogWarning($"当前平台:{EditorUserBuildSettings.activeBuildTarget}和当前已加载平台:{AssetPipelineSystem.LoadedTarget}不一致，关闭Asset管线系统!".WithColor(Color.yellow));
            }
        }

        /// <summary>
        /// 后处理指定Asset类型的AssetPostprocessor
        /// </summary>
        /// <param name="assetProcessType">Asset管线处理类型</param>
        /// <param name="assetType">Asset类型</param>
        /// <param name="assetPostProcessor">Asset PostProcessor</param>
        /// <param name="paramList">不定长参数列表(未来用于支持Unity更多的AssetPostprocessor接口传参)</param>
        public static void OnPostprocessByAssetType(AssetProcessType assetProcessType, AssetType assetType, AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            AssetPipelineLog.Log($"AssetPipelineSystem:OnPostprocessByAssetType({assetType})");
            if (AssetPipelineSystem.IsActiveTargetLoaded())
            {
                if (Switch && IsValideByAssetPath(assetPostProcessor.assetPath))
                {
                    AssetPipelineLog.Log($"AssetPath:{assetPostProcessor.assetPath} AssetType:{assetType}".WithColor(Color.red));
                    // 后处理先执行Asset处理系统，后执行Asset检查系统
                    if (ProcessorSystemSwitch)
                    {
                        AssetProcessorSystem.OnPostprocessByAssetType(assetProcessType, assetType, assetPostProcessor, paramList);
                    }
                    if (CheckSystemSwitch)
                    {
                        AssetCheckSystem.OnPostCheckByAssetType(assetProcessType, assetType, assetPostProcessor, paramList);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"当前平台:{EditorUserBuildSettings.activeBuildTarget}和当前已加载平台:{AssetPipelineSystem.LoadedTarget}不一致，关闭Asset管线系统!".WithColor(Color.yellow));
            }
        }
        #endregion
    }
}