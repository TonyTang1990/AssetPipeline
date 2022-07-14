/*
 * Description:             CheckFileName.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/19
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// CheckFileName.cs
    /// 检查文件名
    /// </summary>
    [CreateAssetMenu(fileName = "CheckFileName", menuName = "ScriptableObjects/AssetPipeline/AssetCheck/CheckFileName", order = 2001)]
    public class CheckFileName : BasePreCheck
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
            AssetPipelineLog.Log($"检查AssetPath:{assetPath}文件名结果:{result}".WithColor(Color.yellow));
            return result;
        }
    }
}