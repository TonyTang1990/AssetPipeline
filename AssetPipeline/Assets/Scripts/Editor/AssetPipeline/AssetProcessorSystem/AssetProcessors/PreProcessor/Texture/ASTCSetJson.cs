/*
 * Description:             ASTCSetJson.cs
 * Author:                  TONYTANG
 * Create Date:             2023/10/19
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAssetPipeline
{
    /// <summary>
    /// ASTCSetJson.cs
    /// ASTC设置预处理器Json
    /// </summary>
    [Serializable]
    public class ASTCSetJson : BasePreProcessorJson
    {
        /// <summary>
        /// 检查器名
        /// </summary>
        public override string Name
        {
            get
            {
                return "ASTC设置";
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
        /// 处理器触发排序Order
        /// </summary>
        public override int Order
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// 目标纹理格式
        /// </summary>
        public TextureImporterFormat TargetTextureFormat = TextureImporterFormat.ASTC_4x4;

        /// <summary>
        /// 执行处理器处理
        /// </summary>
        /// <param name="assetPostProcessor"></param>
        /// <param name="paramList">不定长参数列表</param>
        protected override void DoProcessor(AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            var assetImporter = assetPostProcessor.assetImporter;
            DoASTCSet(assetImporter);
        }

        /// <summary>
        /// 执行指定路径的处理器处理
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        protected override void DoProcessorByPath(string assetPath, params object[] paramList)
        {
            var assetImporter = AssetImporter.GetAtPath(assetPath);
            DoASTCSet(assetImporter);
        }

        /// <summary>
        /// 执行ASTC设置
        /// </summary>
        /// <param name="assetImporter"></param>
        private void DoASTCSet(AssetImporter assetImporter)
        {
            var textureImporter = assetImporter as TextureImporter;
            var actiivePlatformName = EditorUtilities.GetPlatformNameByTarget(EditorUserBuildSettings.activeBuildTarget);
            var platformTextureSettings = textureImporter.GetPlatformTextureSettings(actiivePlatformName);
            var automaticFormat = textureImporter.GetAutomaticFormat(actiivePlatformName);
            var isAutomaticASTC = ResourceUtilities.IsASTCFormat(automaticFormat);
            var textureFormat = isAutomaticASTC ? automaticFormat : TargetTextureFormat;
            platformTextureSettings.overridden = true;
            platformTextureSettings.format = textureFormat;
            textureImporter.SetPlatformTextureSettings(platformTextureSettings);
            AssetPipelineLog.Log($"设置AssetPath:{assetImporter.assetPath}纹理压缩格式:{textureFormat}".WithColor(Color.yellow));
        }
    }
}
