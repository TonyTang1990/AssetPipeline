/*
 * Description:             SpriteMeshTypeSet.cs
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
    /// SpriteMeshTypeSet.cs
    /// SpriteMeshType设置
    /// </summary>
    [CreateAssetMenu(fileName = "SpriteMeshTypeSet", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/PreProcessor/SpriteMeshTypeSet", order = 1005)]
    public class SpriteMeshTypeSet : BasePreProcessor
    {
        /// <summary>
        /// 检查器名
        /// </summary>
        public override string Name
        {
            get
            {
                return "SpriteMeshType设置";
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
        /// SpriteMeshType
        /// </summary>
        [Header("SpriteMeshType选择")]
        public SpriteMeshType MeshType = SpriteMeshType.FullRect;

        /// <summary>
        /// 执行处理器处理
        /// </summary>
        /// <param name="assetPostProcessor"></param>
        /// <param name="paramList">不定长参数列表</param>
        protected override void DoProcessor(AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            DoTightSet(assetPostProcessor.assetImporter);
        }

        /// <summary>
        /// 执行指定路径的处理器处理
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        protected override void DoProcessorByPath(string assetPath, params object[] paramList)
        {
            var assetImporter = AssetImporter.GetAtPath(assetPath);
            DoTightSet(assetImporter);
        }

        /// <summary>
        /// 执行Tight设置
        /// </summary>
        /// <param name="assetImporter"></param>
        private void DoTightSet(AssetImporter assetImporter)
        {
            var textureImporter = assetImporter as TextureImporter;
            TextureImporterSettings textureImporterSetting = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(textureImporterSetting);
            textureImporterSetting.spriteMeshType = MeshType;
            textureImporter.SetTextureSettings(textureImporterSetting);
            AssetPipelineLog.Log($"设置AssetPath:{assetImporter.assetPath},spriteMeshType:{MeshType}".WithColor(Color.yellow));
        }
    }
}