/*
 * Description:             PlatformStrategyData.cs
 * Author:                  TONYTANG
 * Create Date:             2023/10/19
 */

using System;
using UnityEditor;
using UnityEngine;

namespace TAssetPipeline
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
}

