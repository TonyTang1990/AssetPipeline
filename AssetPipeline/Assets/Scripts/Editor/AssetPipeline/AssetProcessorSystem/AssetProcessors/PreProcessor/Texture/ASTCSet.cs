/*
 * Description:             ASTCSet.cs
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
    /// ASTCSet.cs
    /// ASTC设置预处理器
    /// </summary>
    [CreateAssetMenu(fileName = "ASTCSet", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/PreProcessor/Texture/ASTCSet", order = 1001)]
    public class ASTCSet : BasePreProcessor
    {
        /// <summary>
        /// 检查器名
        /// </summary>
        public override string Name
        {
            get
            {
                return "ASTC设置";
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
        /// 处理器触发排序Order
        /// </summary>
        public override int Order
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// 目标纹理格式
        /// </summary>
        [Header("目标纹理格式")]
        public TextureImporterFormat TargetTextureFormat = TextureImporterFormat.ASTC_4x4;
    }
}