/*
 * Description:             AndroidETC2Set.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/19
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// AndroidETC2Set.cs
    /// Android ETC2设置
    /// </summary>
    [CreateAssetMenu(fileName = "AndroidETC2Set", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/AndroidETC2Set", order = 1002)]
    public class AndroidETC2Set : BaseProcessor
    {
        /// <summary>
        /// 检查器名
        /// </summary>
        public override string Name
        {
            get
            {
                return "Android ETC2设置";
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