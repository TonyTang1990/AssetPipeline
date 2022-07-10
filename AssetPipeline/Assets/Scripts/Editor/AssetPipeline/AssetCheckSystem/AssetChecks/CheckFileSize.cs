/*
 * Description:             CheckFileSize.cs
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
    /// CheckFileSize.cs
    /// 检查文件大小
    /// </summary>
    [CreateAssetMenu(fileName = "CheckFileSize", menuName = "ScriptableObjects/AssetPipeline/AssetCheck/CheckFileSize", order = 2002)]
    public class CheckFileSize : BaseCheck
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
        /// 执行检查器处理
        /// </summary>
        /// <param name="assetPostProcessor"></param>
        /// <param name="paramList">不定长参数</param>
        protected override bool DoCheck(AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            return true;
        }

        /// <summary>
        /// 执行指定路径的检查器处理
        /// </summary>
        /// <param name="assetPath"></param>
        protected override bool DoCheckByPath(string assetPath, params object[] paramList)
        {
            return true;
        }
    }
}