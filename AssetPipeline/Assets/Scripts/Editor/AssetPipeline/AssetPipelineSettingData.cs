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
    public class AssetPipelineSettingData : ScriptableObject
    {
        /// <summary>
        /// 平台策略数据
        /// </summary>
        [Serializable]
        public class PlatformStrategyData
        {
            /// <summary>
            /// 目标平台
            /// </summary>
            [Header("目标平台")]
            public BuildTarget Target;

            /// <summary>
            /// 策略名
            /// </summary>
            [Header("策略名")]
            public string StrategyName;
            
            public PlatformStrategyData(BuildTarget buildTarget, string strategyName)
            {
                Target = buildTarget;
                StrategyName = strategyName;
            }
        }

        /// <summary>
        /// 资源目录
        /// </summary
        [Header("资源目录")]
        public string ResourceFolder;

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