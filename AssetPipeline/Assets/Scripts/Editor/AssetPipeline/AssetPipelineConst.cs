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
        /// 处理器基类类型
        /// </summary>
        public static Type BASE_PROCESSOR_TYPE = typeof(BaseProcessor);

        /// <summary>
        /// 检查器基类类型
        /// </summary>
        public static Type BASE_CHECK_TYPE = typeof(BaseCheck);


    }
}