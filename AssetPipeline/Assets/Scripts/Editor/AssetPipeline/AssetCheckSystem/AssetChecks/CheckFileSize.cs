/*
 * Description:             CheckFileSize.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/19
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// CheckFileSize.cs
    /// 检查文件大小
    /// </summary>
    [CreateAssetMenu(fileName = "CheckFileSize", menuName = "ScriptableObjects/AssetPipeline/AssetCheck/CheckFileSize", order = 2002)]
    public class CheckFileSize : BasePreCheck
    {
        /// <summary>
        /// 检查器名
        /// </summary>
        public override string Name
        {
            get
            {
                return "检查文件大小";
            }
        }

        /// <summary>
        /// 目标Asset类型
        /// </summary>
        public override AssetType TargetAssetType
        {
            get
            {
                return AssetPipelineSystem.GetAllCommonAssetType();
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
        /// 文件大小限制
        /// </summary>
        [Header("文件大小限制")]
        public int FileSizeLimit = 1024 * 1024 * 8;

        /// <summary>
        /// 执行检查器处理
        /// </summary>
        /// <param name="assetPostProcessor"></param>
        /// <param name="paramList">不定长参数</param>
        protected override bool DoCheck(AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            return DoCheckFileSize(assetPostProcessor.assetPath);
        }

        /// <summary>
        /// 执行指定路径的检查器处理
        /// </summary>
        /// <param name="assetPath"></param>
        protected override bool DoCheckByPath(string assetPath, params object[] paramList)
        {
            return DoCheckFileSize(assetPath);
        }

        /// <summary>
        /// 执行文件大小检查
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        private bool DoCheckFileSize(string assetPath)
        {
            var assetFullPath = PathUtilities.GetAssetFullPath(assetPath);
            using(FileStream fs = File.Open(assetFullPath, FileMode.Open))
            {
                AssetPipelineLog.Log($"AssetPath:{assetPath}文件大小检查,实际大小:{fs.Length / 1024f / 1024f}M,限制大小:{FileSizeLimit / 1024f / 1024f}M".WithColor(Color.yellow));
                return fs.Length <= FileSizeLimit;
            }
        }
    }
}