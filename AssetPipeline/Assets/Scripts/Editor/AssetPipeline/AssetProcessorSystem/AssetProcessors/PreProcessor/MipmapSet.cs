/*
 * Description:             MipmapSet.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/19
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// MipmapSet.cs
    /// Mipmap设置
    /// </summary>
    [CreateAssetMenu(fileName = "MipmapSet", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/PreProcessor/MipmapSet", order = 1004)]
    public class MipmapSet : BasePreProcessor
    {
        /// <summary>
        /// 检查器名
        /// </summary>
        public override string Name
        {
            get
            {
                return "Mipmap设置";
            }
        }

        /// <summary>
        /// 目标Asset类型
        /// </summary>
        public override AssetType TargetAssetType
        {
            get
            {
                return AssetType.Texture;
            }
        }

        /// <summary>
        /// 目标Asset管线处理类型
        /// </summary>
        public override AssetProcessType TargetAssetProcessType
        {
            get
            {
                return AssetProcessType.PreprocessTexture;
            }
        }

        /// <summary>
        /// 是否打开MipMap
        /// </summary>
        [Header("是否打开MipMap")]
        public bool EnableMipMap = false;

        /// <summary>
        /// 执行处理器处理
        /// </summary>
        /// <param name="assetPostProcessor"></param>
        /// <param name="paramList">不定长参数列表</param>
        protected override void DoProcessor(AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            DoMipMapSet(assetPostProcessor.assetImporter);
        }

        /// <summary>
        /// 执行指定路径的处理器处理
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        protected override void DoProcessorByPath(string assetPath, params object[] paramList)
        {
            var assetImporter = AssetImporter.GetAtPath(assetPath);
            DoMipMapSet(assetImporter);
        }

        /// <summary>
        /// 执行MipMap设置
        /// </summary>
        /// <param name="assetImporter"></param>
        private void DoMipMapSet(AssetImporter assetImporter)
        {
            var textureImporter = assetImporter as TextureImporter;
            textureImporter.mipmapEnabled = EnableMipMap;
            AssetPipelineLog.Log($"设置AssetPath:{assetImporter.assetPath}mipmapEnabled:{EnableMipMap}".WithColor(Color.yellow));
        }
    }
}