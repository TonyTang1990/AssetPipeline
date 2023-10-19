/*
 * Description:             CheckFileNameJson.cs
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
    /// CheckFileName.cs
    /// 检查文件名检查器Json
    /// </summary>
    [Serializable]
    public class CheckFileNameJson : BasePreCheckJson
    {
        /// <summary>
        /// 检查器名
        /// </summary>
        public override string Name
        {
            get
            {
                return "检查文件名";
            }
        }

        /// <summary>
        /// 目标Asset类型
        /// </summary>
        public override AssetType TargetAssetType
        {
            get
            {
                return AssetType.All;
            }
        }

        /// <summary>
        /// 目标Asset管线处理类型
        /// </summary>
        public override AssetProcessType TargetAssetProcessType
        {
            get
            {
                return AssetProcessType.CommonPreprocess;
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
        /// 文件名正则匹配
        /// </summary>
        private Regex mFileNameRegex = new Regex("~[!@#$%^&*()_+-=|]");

        /// <summary>
        /// 执行检查器处理
        /// </summary>
        /// <param name="assetPostProcessor"></param>
        /// <param name="paramList">不定长参数列表</param>
        protected override bool DoCheck(AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            return DoCheckFileName(assetPostProcessor.assetPath);
        }

        /// <summary>
        /// 执行指定路径的检查器处理
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        protected override bool DoCheckByPath(string assetPath, params object[] paramList)
        {
            return DoCheckFileName(assetPath);
        }

        /// <summary>
        /// 检查文件名
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        private bool DoCheckFileName(string assetPath)
        {
            var fileName = Path.GetFileName(assetPath);
            var result = mFileNameRegex.IsMatch(fileName);
            if (!result)
            {
                AssetPipelineLog.Log($"检查AssetPath:{assetPath}文件名匹配结果:{result}".WithColor(Color.yellow));
            }
            else
            {
                AssetPipelineLog.LogError($"检查AssetPath:{assetPath}文件名匹配结果:{result}".WithColor(Color.yellow));
            }
            return result;
        }
    }
}
