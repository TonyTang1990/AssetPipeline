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
    /// Mipmap设置预处理器
    /// </summary>
    [CreateAssetMenu(fileName = "MipmapSet", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/PreProcessor/Texture/MipmapSet", order = 1004)]
    public class MipmapSet : BasePreProcessor
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
        /// 目标Asset管线处理类型
        /// </summary>
        public override AssetProcessType TargetAssetProcessType
        {
            get
            {
                return AssetProcessType.PreprocessTexture;
            }
        }

        /// <summary>
        /// 是否打开MipMap
        /// </summary>
        [Header("是否打开MipMap")]
        public bool EnableMipMap = false;
    }
}