/*
 * Description:             CheckFileName.cs
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
    /// CheckFileName.cs
    /// 检查文件名
    /// </summary>
    [CreateAssetMenu(fileName = "CheckFileName", menuName = "ScriptableObjects/AssetPipeline/AssetCheck/CheckFileName", order = 2001)]
    public class CheckFileName : BaseCheck
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
                return AssetType.Object;
            }
        }

        /// <summary>
        /// 执行检查器处理
        /// </summary>
        /// <param name="assetPostProcessor"></param>
        protected override bool DoCheck(AssetPostprocessor assetPostProcessor)
        {
            return true;
        }

        /// <summary>
        /// 执行指定路径的检查器处理
        /// </summary>
        /// <param name="assetPath"></param>
        protected override bool DoCheckByPath(string assetPath)
        {
            return true;
        }
    }
}