/*
 * Description:             CheckFileSize.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/19
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// CheckFileSize.cs
    /// 检查文件大小检查器
    /// </summary>
    [CreateAssetMenu(fileName = "CheckFileSize", menuName = "ScriptableObjects/AssetPipeline/AssetCheck/PreCheck/Mix/CheckFileSize", order = 2002)]
    public class CheckFileSize : BasePreCheck
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
                return AssetPipelineSystem.GetAllCommonAssetType();
            }
        }

        /// <summary>
        /// 目标Asset管线处理类型
        /// </summary>
        public override AssetProcessType TargetAssetProcessType
        {
            get
            {
                return AssetProcessType.CommonPreprocess;
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

        /// <summary>
        /// 文件大小限制
        /// </summary>
        [Header("文件大小限制")]
        public int FileSizeLimit = 1024 * 1024 * 8;
    }
}