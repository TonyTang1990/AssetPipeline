/*
 * Description:             AssetPipelineSettingData.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/19
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetPipelineSettingData.cs
    /// AssetPipeline配置数据
    /// </summary>
    [Serializable]
    public class AssetPipelineSettingData
    {
        /// <summary>
        /// 资源目录
        /// </summary
        [Header("资源目录")]
        public string ResourceFolder = "Assets/";

        /// <summary>
        /// Asset管线开关
        /// </summary>
        [Header("Asset管线开关")]
        public bool Switch = true;

        /// <summary>
        /// 处理器系统开关
        /// </summary>
        [Header("处理器系统开关")]
        public bool ProcessorSystemSwitch = true;

        /// <summary>
        /// 检查器系统开关
        /// </summary>
        [Header("检查器系统开关")]
        public bool CheckSystemSwitch = true;

        /// <summary>
        /// Asset管线Log开关
        /// </summary>
        [Header("Asset管线Log开关")]
        public bool LogSwitch = true; 

        /// <summary>
        /// 可配置策略列表
        /// </summary>
        [Header("可配置策略列表")]
        public List<string> StrategyList = new List<string>() { AssetPipelineConst.DEFAULT_STRATEGY_NAME };

        /// <summary>
        /// 平台策略数据列表
        /// </summary>
        public List<PlatformStrategyData> PlatformStrategyDataList = new List<PlatformStrategyData>()
        {
            new PlatformStrategyData(BuildTarget.Android, AssetPipelineConst.DEFAULT_STRATEGY_NAME),
            new PlatformStrategyData(BuildTarget.iOS, AssetPipelineConst.DEFAULT_STRATEGY_NAME),
            new PlatformStrategyData(BuildTarget.StandaloneWindows, AssetPipelineConst.DEFAULT_STRATEGY_NAME),
            new PlatformStrategyData(BuildTarget.StandaloneWindows64, AssetPipelineConst.DEFAULT_STRATEGY_NAME),
            new PlatformStrategyData(BuildTarget.StandaloneOSX, AssetPipelineConst.DEFAULT_STRATEGY_NAME),
        };
    }
}