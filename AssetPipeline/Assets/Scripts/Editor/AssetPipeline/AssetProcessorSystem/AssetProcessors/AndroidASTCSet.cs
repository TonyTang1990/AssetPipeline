/*
 * Description:             AndroidASTCSet.cs
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
    /// AndroidASTCSet.cs
    /// Android ASTC设置
    /// </summary>
    [CreateAssetMenu(fileName = "AndroidASTCSet", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/AndroidASTCSet", order = 1003)]
    public class AndroidASTCSet : BaseProcessor
    {
        /// <summary>
        /// 检查器名
        /// </summary>
        public override string Name
        {
            get
            {
                return "Android ASTC设置";
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