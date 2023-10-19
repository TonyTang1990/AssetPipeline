/*
 * Description:             SpriteMeshTypeSet.cs
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
    /// SpriteMeshTypeSet.cs
    /// SpriteMeshType设置预处理器
    /// </summary>
    [CreateAssetMenu(fileName = "SpriteMeshTypeSet", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/PreProcessor/Texture/SpriteMeshTypeSet", order = 1005)]
    public class SpriteMeshTypeSet : BasePreProcessor
    {
        /// <summary>
        /// 检查器名
        /// </summary>
        public override string Name
        {
            get
            {
                return "SpriteMeshType设置";
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
        /// SpriteMeshType
        /// </summary>
        [Header("SpriteMeshType选择")]
        public SpriteMeshType MeshType = SpriteMeshType.FullRect;
    }
}