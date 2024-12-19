/*
 * Description:             AssetPipelineConst.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/19
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
        /// ScriptableObejct Asset后缀
        /// </summary>
        public const string SCRIPTABLE_OBJECT_ASSET_POST_FIX = "asset";

        /// <summary>
        /// Json后缀
        /// </summary>
        public const string JSON_POST_FIX = "json";

        /// <summary>
        /// Json类型后缀名
        /// </summary>
        public const string JSON_TYPE_POST_FIX = "Json";

        /// <summary>
        /// 默认策略名
        /// </summary>
        public const string DEFAULT_STRATEGY_NAME = "Default";

        /// <summary>
        /// AssetType类型
        /// </summary>
        public static Type ASSET_TYPE = typeof(AssetType);

        /// <summary>
        /// Asset管线脚本所处的程序集
        /// </summary>
        public static Assembly ASSET_PIPELINE_ASSEMBLY = ASSET_TYPE.Assembly;

        /// <summary>
        /// AssetProcessType类型
        /// </summary>
        public static Type ASSET_PROCESS_TYPE = typeof(AssetProcessType);

        /// <summary>
        /// 处理器基类类型
        /// </summary>
        public static Type BASE_PROCESSOR_TYPE = typeof(BaseProcessor);

        /// <summary>
        /// 预处理器基类类型
        /// </summary>
        public static Type BASE_PRE_PROCESSOR_TYPE = typeof(BasePreProcessor);

        /// <summary>
        /// 处后理器基类类型
        /// </summary>
        public static Type BASE_POST_PROCESSOR_TYPE = typeof(BasePostProcessor);

        /// <summary>
        /// 检查器基类类型
        /// </summary>
        public static Type BASE_CHECK_TYPE = typeof(BaseCheck);

        /// <summary>
        /// 预检查器基类类型
        /// </summary>
        public static Type BASE_PRE_CHECK_TYPE = typeof(BasePreCheck);

        /// <summary>
        /// 后检查器基类类型
        /// </summary>
        public static Type BASE_POST_CHECK_TYPE = typeof(BasePostCheck);

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