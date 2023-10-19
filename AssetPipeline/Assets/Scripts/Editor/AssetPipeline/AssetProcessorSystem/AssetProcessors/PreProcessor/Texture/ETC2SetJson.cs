/*
 * Description:             ETC2SetJson.cs
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
    /// ETC2SetJson.cs
    /// ETC2设置预处理器Json
    /// </summary>
    [Serializable]
    public class ETC2SetJson : BasePreProcessorJson
    {
        /// <summary>
        /// 检查器名
        /// </summary>
        public override string Name
        {
            get
            {
                return "ETC2设置";
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
                return 2;
            }
        }

        /// <summary>
        /// 执行处理器处理
        /// </summary>
        /// <param name="assetPostProcessor"></param>
        /// <param name="paramList">不定长参数列表</param>
        protected override void DoProcessor(AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            var assetImporter = assetPostProcessor.assetImporter;
            DoETC2Set(assetImporter);
        }

        /// <summary>
        /// 执行指定路径的处理器处理
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        protected override void DoProcessorByPath(string assetPath, params object[] paramList)
        {
            var assetImporter = AssetImporter.GetAtPath(assetPath);
            DoETC2Set(assetImporter);
        }

        /// <summary>
        /// 执行ETC2设置
        /// </summary>
        /// <param name="assetImporter"></param>
        private void DoETC2Set(AssetImporter assetImporter)
        {
            var textureImporter = assetImporter as TextureImporter;
            var actiivePlatformName = EditorUtilities.GetPlatformNameByTarget(EditorUserBuildSettings.activeBuildTarget);
            var platformTextureSettings = textureImporter.GetPlatformTextureSettings(actiivePlatformName);
            platformTextureSettings.overridden = true;
            var textureFormat = textureImporter.DoesSourceTextureHaveAlpha() ? TextureImporterFormat.ETC2_RGBA8 : TextureImporterFormat.ETC2_RGB4;
            platformTextureSettings.format = textureFormat;
            textureImporter.SetPlatformTextureSettings(platformTextureSettings);
            AssetPipelineLog.Log($"设置AssetPath:{assetImporter.assetPath}纹理压缩格式:{textureFormat}".WithColor(Color.yellow));
        }
    }
}
