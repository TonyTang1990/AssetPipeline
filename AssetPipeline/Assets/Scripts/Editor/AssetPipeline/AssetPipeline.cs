/*
 * Description:             AssetPipeline.cs
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
    /// AssetPipeline.cs
    /// Asset管线入口
    /// </summary>
    public class AssetPipeline : AssetPostprocessor
    {
        /// <summary>
        /// 自定义默认支持的导入预处理Asset类型Map(避免同一Asset类型触发两次预处理)
        /// </summary>
        private static Dictionary<AssetType, bool> CustomSupportedPreAssetTypeMap = new Dictionary<AssetType, bool>
        {
            { AssetType.AnimationClip, true },
            { AssetType.AudioClip, true },
            { AssetType.FBX, true },
            { AssetType.Texture, true },
        };

        /// <summary>
        /// 自定义默认支持的导入后处理Asset类型Map(避免同一Asset类型触发两次后处理)
        /// </summary>
        private static Dictionary<AssetType, bool> CustomSupportedPostAssetTypeMap = new Dictionary<AssetType, bool>
        {
            { AssetType.AnimationClip, true },
            { AssetType.FBX, true },
            { AssetType.Material, true },
            { AssetType.Prefab, true },
            { AssetType.Texture, true },
            { AssetType.AudioClip, true },
        };

        /// <summary>
        /// 是否是自定义支持的导入预处理Asset类型
        /// </summary>
        /// <param name="assetType"></param>
        /// <returns></returns>
        private static bool IsComstomSupportedPreAssetType(AssetType assetType)
        {
            return CustomSupportedPreAssetTypeMap.ContainsKey(assetType);
        }

        /// <summary>
        /// 是否是自定义支持的导入后处理Asset类型
        /// </summary>
        /// <param name="assetType"></param>
        /// <returns></returns>
        private static bool IsComstomSupportedPostAssetType(AssetType assetType)
        {
            return CustomSupportedPostAssetTypeMap.ContainsKey(assetType);
        }

        /// <summary>
        /// 预处理所有Asset
        /// </summary>
        private void OnPreprocessAsset()
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPreprocessAsset()");
            var assetType = AssetPipelineSystem.GetAssetTypeByPath(this.assetPath);
            // 避免相同类型触发多次导入预处理
            if(!IsComstomSupportedPreAssetType(assetType))
            {
                AssetPipelineSystem.OnPreprocessByAssetType(assetType, this);
            }
        }

        /// <summary>
        /// 预处理Asset
        /// </summary>
        private void OnPreprocessAnimation()
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPreprocessAnimation()");
            AssetPipelineSystem.OnPreprocessByAssetType(AssetType.AnimationClip, this);
        }

        /// <summary>
        /// 预处理音效Asset
        /// </summary>
        private void OnPreprocessAudio()
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPreprocessAudio()");
            AssetPipelineSystem.OnPreprocessByAssetType(AssetType.AudioClip, this);
        }

        /// <summary>
        /// 预处理模型Asset
        /// </summary>
        private void OnPreprocessModel()
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPreprocessModel()");
            AssetPipelineSystem.OnPreprocessByAssetType(AssetType.FBX, this);
        }

        /// <summary>
        /// 预处理纹理Asset
        /// </summary>
        private void OnPreprocessTexture()
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPreprocessTexture()");
            AssetPipelineSystem.OnPreprocessByAssetType(AssetType.Texture, this);
        }

        /// <summary>
        /// 后处理所有Asset
        /// </summary>
        /// <param name="importedAssets"></param>
        /// <param name="deletedAssets"></param>
        /// <param name="movedAssets"></param>
        /// <param name="movedFromAssetPaths"></param>
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPostprocessAllAssets()");
            for (int i = 0, length = importedAssets.Length; i < length; i++)
            {
                AssetPipelineLog.Log("Imported Asset: " + importedAssets[i]);
                // 避免相同类型触发多次导后预处理
                var assetType = AssetPipelineSystem.GetAssetTypeByPath(importedAssets[i]);
                if (!IsComstomSupportedPostAssetType(assetType))
                {
                    AssetPipelineSystem.OnPostprocessImportedAsset(importedAssets[i]);
                }
            }

            for (int i = 0, length = deletedAssets.Length; i < length; i++)
            {
                AssetPipelineLog.Log("Deleted Asset: " + deletedAssets[i]);
                AssetPipelineSystem.OnPostprocessDeletedAsset(importedAssets[i]);
            }

            for (int i = 0, length = movedAssets.Length; i < length; i++)
            {
                AssetPipelineLog.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
                AssetPipelineSystem.OnPostprocessMovedAsset(importedAssets[i], movedFromAssetPaths[i]);
            }
        }

        /// <summary>
        /// 后处理动画
        /// </summary>
        /// <param name="">go</param>
        /// <param name="">clip</param>
        private void OnPostprocessAnimation(GameObject go, AnimationClip animClip)
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPostprocessAnimation({go.name}, {animClip.name})");
            AssetPipelineSystem.OnPostprocessByAssetType(AssetType.AnimationClip, this);
        }

        /// <summary>
        /// 后处理模型
        /// </summary>
        /// <param name="go"></param>
        private void OnPostprocessModel(GameObject go)
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPostprocessModel({go.name})");
            AssetPipelineSystem.OnPostprocessByAssetType(AssetType.FBX, this);
        }

        /// <summary>
        /// 后处理材质
        /// </summary>
        /// <param name="mat"></param>
        private void OnPostprocessMaterial(Material mat)
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPostprocessMaterial({mat.name})");
            AssetPipelineSystem.OnPostprocessByAssetType(AssetType.Material, this);
        }

        /// <summary>
        /// 后处理预制件
        /// </summary>
        /// <param name="root"></param>
        private void OnPostprocessPrefab(GameObject root)
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPostprocessPrefab({root.name})");
            AssetPipelineSystem.OnPostprocessByAssetType(AssetType.Prefab, this);
        }

        /// <summary>
        /// 后处理纹理
        /// </summary>
        /// <param name="texture"></param>
        private void OnPostprocessTexture(Texture2D texture)
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPostprocessTexture({texture.name})");
            AssetPipelineSystem.OnPostprocessByAssetType(AssetType.Texture, this);
        }

        /// <summary>
        /// 后处理音效
        /// </summary>
        /// <param name="audioClip"></param>
        private void OnPostprocessAudio(AudioClip audioClip)
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPostprocessAudio({audioClip.name})");
            AssetPipelineSystem.OnPostprocessByAssetType(AssetType.AudioClip, this);
        }
    }
}