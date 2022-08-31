/*
 * Description:             AssetInfo.cs
 * Author:                  TONYTANG
 * Create Date:             2022/08/28
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetInfo.cs
    /// Asset信息(记录Asset相关路径和对应的Asset类型信息)
    /// </summary>
    [Serializable]
    public class AssetInfo
    {
        /// <summary>
        /// Asset路径
        /// </summary>
        [Header("Asset路径")]
        public string AssetPath;

        /// <summary>
        /// Json Asset路径
        /// </summary>
        [Header("Json Asset路径")]
        public string JsonAssetPath;

        /// <summary>
        /// Asset类型全名
        /// </summary>
        [Header("Asset类型全名")]
        public string AssetTypeFullName;

        public AssetInfo(string assetPath, string assetTypeFullName)
        {
            AssetPath = assetPath;
            JsonAssetPath = Path.ChangeExtension(assetPath, "json");
            AssetTypeFullName = assetTypeFullName;
        }
    }
}