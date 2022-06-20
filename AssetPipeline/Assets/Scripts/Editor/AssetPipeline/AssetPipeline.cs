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
        /// 预处理所有Asset
        /// </summary>
        private static void OnPreprocessAsset()
        {
            Debug.Log($"AssetPipeline:OnPreprocessAsset()");
        }

        /// <summary>
        /// 预处理Asset
        /// </summary>
        private static void OnPreprocessAnimation()
        {
            Debug.Log($"AssetPipeline:OnPreprocessAnimation()");
        }

        /// <summary>
        /// 预处理音效Asset
        /// </summary>
        private static void OnPreprocessAudio()
        {
            Debug.Log($"AssetPipeline:OnPreprocessAudio()");
        }

        /// <summary>
        /// 预处理模型Asset
        /// </summary>
        private static void OnPreprocessModel()
        {
            Debug.Log($"AssetPipeline:OnPreprocessModel()");
        }

        /// <summary>
        /// 预处理纹理Asset
        /// </summary>
        private static void OnPreprocessTexture()
        {
            Debug.Log($"AssetPipeline:OnPreprocessTexture()");
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
            Debug.Log($"AssetPipeline:OnPostprocessAllAssets()");
            foreach (string str in importedAssets)
            {
                Debug.Log("Reimported Asset: " + str);
            }
            foreach (string str in deletedAssets)
            {
                Debug.Log("Deleted Asset: " + str);
            }

            for (int i = 0; i < movedAssets.Length; i++)
            {
                Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
            }

        }

        /// <summary>
        /// 后处理动画
        /// </summary>
        /// <param name="">go</param>
        /// <param name="">clip</param>
        private static void OnPostprocessAnimation(GameObject go, AnimationClip animClip)
        {
            Debug.Log($"AssetPipeline:OnPostprocessAnimation({go.name}, {animClip.name})");
        }

        /// <summary>
        /// 后处理模型
        /// </summary>
        /// <param name="go"></param>
        private static void OnPostprocessModel(GameObject go)
        {
            Debug.Log($"AssetPipeline:OnPostprocessModel({go.name})");
        }

        /// <summary>
        /// 后处理材质
        /// </summary>
        /// <param name="mat"></param>
        private static void OnPostprocessMaterial(Material mat)
        {
            Debug.Log($"AssetPipeline:OnPostprocessMaterial({mat.name})");
        }

        /// <summary>
        /// 后处理预制件
        /// </summary>
        /// <param name="root"></param>
        private static void OnPostprocessPrefab(GameObject root)
        {
            Debug.Log($"AssetPipeline:OnPostprocessPrefab({root.name})");
        }

        /// <summary>
        /// 后处理纹理
        /// </summary>
        /// <param name="texture"></param>
        private static void OnPostprocessTexture(Texture2D texture)
        {
            Debug.Log($"AssetPipeline:OnPostprocessTexture({texture.name})");
        }

        /// <summary>
        /// 后处理音效
        /// </summary>
        /// <param name="audioClip"></param>
        private static void OnPostprocessAudio(AudioClip audioClip)
        {
            Debug.Log($"AssetPipeline:OnPostprocessAudio({audioClip.name})");
        }
    }
}