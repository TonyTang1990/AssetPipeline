/*
 * Description:             AssetPipelineConst.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/19
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetPipelineConst.cs
    /// Asset管线常量
    /// </summary>
    public static class AssetPipelineConst
    {
        /// <summary>
        /// 默认策略名
        /// </summary>
        public const string DEFAULT_STRATEGY_NAME = "Default";

        /// <summary>
        /// AssetType类型
        /// </summary>
        public static Type ASSET_TYPE = typeof(AssetType);

        /// <summary>
        /// AssetProcessType类型
        /// </summary>
        public static Type ASSET_PROCESS_TYPE = typeof(AssetProcessType);

        /// <summary>
        /// 处理器基类类型
        /// </summary>
        public static Type BASE_PROCESSOR_TYPE = typeof(BaseProcessor);

        /// <summary>
        /// 检查器基类类型
        /// </summary>
        public static Type BASE_CHECK_TYPE = typeof(BaseCheck);

        /// <summary>
        /// A资源目录名
        /// </summary>
        public static string A_FOLDER_NAME = "a";

        /// <summary>
        /// E资源目录名
        /// </summary>
        public static string E_FOLDER_NAME = "e";

        /// <summary>
        /// AssetType所有值
        /// </summary>
        public static Array ASSET_TYPE_VALUES = Enum.GetValues(AssetPipelineConst.ASSET_TYPE);

        /// <summary>
        /// AssetProcessType所有值
        /// </summary>
        public static Array ASSET_PROCESS_TYPE_VALUES = Enum.GetValues(AssetPipelineConst.ASSET_PROCESS_TYPE);
    }
}