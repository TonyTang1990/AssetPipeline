/*
 * Description:             MipmapSet.cs
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
    /// MipmapSet.cs
    /// Mipmap设置
    /// </summary>
    [CreateAssetMenu(fileName = "MipmapSet", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/MipmapSet", order = 1005)]
    public class MipmapSet : BaseProcessor
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
        /// 执行处理器处理
        /// </summary>
        /// <param name="assetPostProcessor"></param>
        protected override void DoProcessor(AssetPostprocessor assetPostProcessor)
        {

        }

        /// <summary>
        /// 执行指定路径的处理器处理
        /// </summary>
        /// <param name="assetPath"></param>
        protected override void DoProcessorByPath(string assetPath)
        {

        }
    }
}