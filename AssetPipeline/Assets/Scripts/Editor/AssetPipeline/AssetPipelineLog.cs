/*
 * Description:             AssetPipelineLog.cs
 * Author:                  TONYTANG
 * Create Date:             2022/07/01
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TAssetPipeline
{
    /// <summary>
    /// AssetPipelineLog.cs
    /// Asset管线Log工具
    /// </summary>
    public static class AssetPipelineLog
    {
        /// <summary>
        /// Log开关
        /// </summary>
        public static bool Switch;

        /// <summary>
        /// Log打印
        /// </summary>
        /// <param name="content"></param>
        public static void Log(string content)
        {
            if(Switch)
            {
                Debug.Log(content);
            }
        }

        /// <summary>
        /// LogWarning打印
        /// </summary>
        /// <param name="content"></param>
        public static void LogWarning(string content)
        {
            if (Switch)
            {
                Debug.LogWarning(content);
            }
        }

        /// <summary>
        /// LogError打印
        /// </summary>
        /// <param name="content"></param>
        public static void LogError(string content)
        {
            if (Switch)
            {
                Debug.LogError(content);
            }
        }
    }
}