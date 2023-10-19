/*
 * Description:             MipmapSetJson.cs
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
    /// MipmapSetJson.cs
    /// Mipmap设置预处理器
    /// </summary>
    [Serializable]
    public class MipmapSetJson : BasePreProcessorJson
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
