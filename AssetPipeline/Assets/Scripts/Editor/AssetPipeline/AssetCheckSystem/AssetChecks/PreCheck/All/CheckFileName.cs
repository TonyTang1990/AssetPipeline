/*
 * Description:             CheckFileName.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/19
 */

using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// CheckFileName.cs
    /// 检查文件名检查器
    /// </summary>
    [CreateAssetMenu(fileName = "CheckFileName", menuName = "ScriptableObjects/AssetPipeline/AssetCheck/PreCheck/All/CheckFileName", order = 2001)]
    public class CheckFileName : BasePreCheck
    {
        /// <summary>
        /// 检查器名
        /// </summary>
        public override string Name
        {
            get
            {
                return "检查文件名";
            }
        }

        /// <summary>
        /// 目标Asset类型
        /// </summary>
        public override AssetType TargetAssetType
        {
            get
            {
                return AssetType.All;
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
                return 1;
            }
        }
    }
}