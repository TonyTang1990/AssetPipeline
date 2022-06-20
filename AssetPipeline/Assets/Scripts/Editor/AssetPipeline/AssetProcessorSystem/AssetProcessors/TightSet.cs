/*
 * Description:             TightSet.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/19
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// TightSet.cs
    /// Tight设置
    /// </summary>
    [CreateAssetMenu(fileName = "TightSet", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/TightSet", order = 1006)]
    public class TightSet : BaseProcessor
    {
        /// <summary>
        /// 检查器名
        /// </summary>
        public override string Name
        {
            get
            {
                return "Tight设置";
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
    }
}