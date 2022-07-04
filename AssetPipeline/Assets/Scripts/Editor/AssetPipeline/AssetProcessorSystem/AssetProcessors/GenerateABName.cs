/*
 * Description:             GenerateABName.cs
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
    /// GenerateABName.cs
    /// 生成AB名处理器
    /// </summary>
    [CreateAssetMenu(fileName = "GenerateABName", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/GenerateABName", order = 1001)]
    public class GenerateABName : BaseProcessor
    {
        /// <summary>
        /// 检查器名
        /// </summary>
        public override string Name
        {
            get
            {
                return "生成AB名";
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