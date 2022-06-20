/*
 * Description:             CheckFileSize.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/19
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// CheckFileSize.cs
    /// 检查文件大小
    /// </summary>
    [CreateAssetMenu(fileName = "CheckFileSize", menuName = "ScriptableObjects/AssetPipeline/AssetCheck/CheckFileSize", order = 2002)]
    public class CheckFileSize : BaseCheck
    {
        /// <summary>
        /// 检查器名
        /// </summary>
        public override string Name
        {
            get
            {
                return "检查文件大小";
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
    }
}