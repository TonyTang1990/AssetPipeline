/*
 * Description:             ETC2Set.cs
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
    /// ETC2Set.cs
    /// ETC2设置预处理器
    /// </summary>
    [CreateAssetMenu(fileName = "ETC2Set", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/PreProcessor/Texture/ETC2Set", order = 1002)]
    public class ETC2Set : BasePreProcessor
    {
        /// <summary>
        /// 检查器名
        /// </summary>
        public override string Name
        {
            get
            {
                return "ETC2设置";
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
                return 2;
            }
        }
    }
}