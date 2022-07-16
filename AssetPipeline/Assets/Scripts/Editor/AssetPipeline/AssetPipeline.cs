/*
 * Description:             AssetPipeline.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/17
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
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
        /// 预处理所有Asset
        /// </summary>
        private void OnPreprocessAsset()
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPreprocessAsset()");
            var assetType = AssetPipelineSystem.GetAssetTypeByPath(this.assetPath);
            AssetPipelineSystem.OnPreprocessByAssetType(AssetProcessType.CommonPreprocess, assetType, this);
        }

        /// <summary>
        /// 预处理Asset
        /// </summary>
        private void OnPreprocessAnimation()
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPreprocessAnimation()");
            AssetPipelineSystem.OnPreprocessByAssetType(AssetProcessType.PreprocessAnimation, AssetType.AnimationClip, this);
        }

        /// <summary>
        /// 预处理音效Asset
        /// </summary>
        private void OnPreprocessAudio()
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPreprocessAudio()");
            AssetPipelineSystem.OnPreprocessByAssetType(AssetProcessType.PreprocessAudio, AssetType.AudioClip, this);
        }

        /// <summary>
        /// 预处理模型Asset
        /// </summary>
        private void OnPreprocessModel()
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPreprocessModel()");
            AssetPipelineSystem.OnPreprocessByAssetType(AssetProcessType.PreprocessModel, AssetType.FBX, this);
        }

        /// <summary>
        /// 预处理纹理Asset
        /// </summary>
        private void OnPreprocessTexture()
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPreprocessTexture()");
            AssetPipelineSystem.OnPreprocessByAssetType(AssetProcessType.PreprocessTexture, AssetType.Texture, this);
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
            try
            {
                AssetDatabase.StartAssetEditing();
                for (int i = 0, length = importedAssets.Length; i < length; i++)
                {
                    AssetPipelineLog.Log("Imported Asset: " + importedAssets[i]);
                    AssetPipelineSystem.OnPostprocessImportedAsset(AssetProcessType.CommonPostprocess, importedAssets[i]);
                }

                for (int i = 0, length = deletedAssets.Length; i < length; i++)
                {
                    AssetPipelineLog.Log("Deleted Asset: " + deletedAssets[i]);
                    AssetPipelineSystem.OnPostprocessDeletedAsset(AssetProcessType.CommonPostprocess, deletedAssets[i]);
                }

                for (int i = 0, length = movedAssets.Length; i < length; i++)
                {
                    AssetPipelineLog.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
                    AssetPipelineSystem.OnPostprocessMovedAsset(AssetProcessType.CommonPostprocess, movedAssets[i], movedFromAssetPaths[i]);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
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
            AssetPipelineSystem.OnPostprocessByAssetType(AssetProcessType.PostprocessAnimation, AssetType.AnimationClip, this, go, animClip);
        }

        /// <summary>
        /// 后处理模型
        /// </summary>
        /// <param name="go"></param>
        private void OnPostprocessModel(GameObject go)
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPostprocessModel({go.name})");
            AssetPipelineSystem.OnPostprocessByAssetType(AssetProcessType.PostprocessModel, AssetType.FBX, this, go);
        }

        /// <summary>
        /// 后处理材质
        /// </summary>
        /// <param name="mat"></param>
        private void OnPostprocessMaterial(Material mat)
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPostprocessMaterial({mat.name})");
            AssetPipelineSystem.OnPostprocessByAssetType(AssetProcessType.PostprocessMaterial, AssetType.Material, this, mat);
        }

        /// <summary>
        /// 后处理预制件
        /// </summary>
        /// <param name="root"></param>
        private void OnPostprocessPrefab(GameObject root)
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPostprocessPrefab({root.name})");
            AssetPipelineSystem.OnPostprocessByAssetType(AssetProcessType.PostprocessPrefab, AssetType.Prefab, this, root);
        }

        /// <summary>
        /// 后处理纹理
        /// </summary>
        /// <param name="texture"></param>
        private void OnPostprocessTexture(Texture2D texture)
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPostprocessTexture({texture.name})");
            AssetPipelineSystem.OnPostprocessByAssetType(AssetProcessType.PostprocessTexture, AssetType.Texture, this, texture);
        }

        /// <summary>
        /// 后处理音效
        /// </summary>
        /// <param name="audioClip"></param>
        private void OnPostprocessAudio(AudioClip audioClip)
        {
            AssetPipelineLog.Log($"AssetPipeline:OnPostprocessAudio({audioClip.name})");
            AssetPipelineSystem.OnPostprocessByAssetType(AssetProcessType.PostprocessAudio, AssetType.AudioClip, this, audioClip);
        }

        public int callbackOrder
        {
            get
            {
                return 0;
            }
        }
    }
}