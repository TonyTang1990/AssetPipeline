/*
 * Description:             AlphaFromSourceSet.cs
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
    /// AlphaFromSourceSet.cs
    /// AlphaFromSource设置
    /// </summary>
    [CreateAssetMenu(fileName = "AlphaFromSourceSet", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/PreProcessor/AlphaFromSourceSet", order = 1003)]
    public class AlphaFromSourceSet : BasePreProcessor
    {
        /// <summary>
        /// 检查器名
        /// </summary>
        public override string Name
        {
            get
            {
                return "AlphaFromSource设置";
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
        /// 执行处理器处理
        /// </summary>
        /// <param name="assetPostProcessor"></param>
        /// <param name="paramList">不定长参数列表</param>
        protected override void DoProcessor(AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            DoAlphaFromSourceSet(assetPostProcessor.assetImporter);
        }

        /// <summary>
        /// 执行指定路径的处理器处理
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        protected override void DoProcessorByPath(string assetPath, params object[] paramList)
        {
            var assetImporter = AssetImporter.GetAtPath(assetPath);
            DoAlphaFromSourceSet(assetImporter);
        }

        /// <summary>
        /// 执行AlphaFromSource设置
        /// </summary>
        /// <param name="assetImporter"></param>
        private void DoAlphaFromSourceSet(AssetImporter assetImporter)
        {
            var textureImporter = assetImporter as TextureImporter;
            var hasAlpha = textureImporter.DoesSourceTextureHaveAlpha() ? true : false;
            textureImporter.alphaIsTransparency = hasAlpha;
            var alphaSource = hasAlpha ? TextureImporterAlphaSource.FromInput : TextureImporterAlphaSource.None;
            textureImporter.alphaSource = alphaSource;
            AssetPipelineLog.Log($"设置AssetPath:{assetImporter.assetPath} alphaIsTransparency:{hasAlpha} alphaSource:{alphaSource}".WithColor(Color.yellow));
        }
    }
}