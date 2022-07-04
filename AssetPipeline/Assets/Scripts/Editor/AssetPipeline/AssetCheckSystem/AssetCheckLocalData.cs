/*
 * Description:             AssetCheckLocalData.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/17
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetCheckConfigData.cs
    /// Asset检查局部数据
    /// </summary>
    public class AssetCheckLocalData : ScriptableObject
    {
        /// <summary>
        /// Asset检查器局部数据
        /// </summary>
        [Serializable]
        public class CheckLocalData
        {
            /// <summary>
            /// 目录路径
            /// </summary>
            [Header("目录路径")]
            public string FolderPath;

            /// <summary>
            /// 检查器设置列表
            /// </summary>
            public List<BaseCheck> CheckList = new List<BaseCheck>();

            /// <summary>
            /// 检查器选择列表(只使用第一个)
            /// </summary>
            [NonSerialized]
            public List<BaseCheck> CheckChosenList = new List<BaseCheck>(1) { null };

            /// <summary>
            /// 是否展开
            /// </summary>
            [NonSerialized]
            public bool IsUnFold = false;
        }

        /// <summary>
        /// 局部预检查器数据
        /// </summary>
        [Header("局部预检查器数据")]
        public List<CheckLocalData> PreCheckDataList = new List<CheckLocalData>();

        /// <summary>
        /// 局部后检查器数据
        /// </summary>
        [Header("局部后检查器数据")]
        public List<CheckLocalData> PostCheckDataList = new List<CheckLocalData>();
    }
}