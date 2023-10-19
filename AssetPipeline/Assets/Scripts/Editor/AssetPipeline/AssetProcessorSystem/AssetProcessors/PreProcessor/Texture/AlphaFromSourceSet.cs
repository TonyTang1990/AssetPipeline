/*
 * Description:             AlphaFromSourceSet.cs
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
    /// AlphaFromSourceSet.cs
    /// AlphaFromSource设置预处理器
    /// </summary>
    [CreateAssetMenu(fileName = "AlphaFromSourceSet", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/PreProcessor/Texture/AlphaFromSourceSet", order = 1003)]
    public class AlphaFromSourceSet : BasePreProcessor
    {
        /// <summary>
        /// 检查器名
        /// </summary>
        public override string Name
        {
            get
            {
                return "AlphaFromSource设置";
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
    }
}