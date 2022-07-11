/*
 * Description:             AECopy.cs
 * Author:                  TONYTANG
 * Create Date:             2022/07/10
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// AECopy.cs
    /// AE目录拷贝处理器
    /// </summary>
    [CreateAssetMenu(fileName = "AECopy", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/AECopy", order = 1007)]
    public class AECopy : BaseProcessor
    {
        /// <summary>
        /// 检查器名
        /// </summary>
        public override string Name
        {
            get
            {
                return "AE目录拷贝";
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
        /// 执行处理器处理
        /// </summary>
        /// <param name="assetPostProcessor"></param>
        /// <param name="paramList">不定长参数列表</param>
        protected override void DoProcessor(AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            DoAECopy(assetPostProcessor.assetPath);
        }

        /// <summary>
        /// 执行指定路径的处理器处理
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        protected override void DoProcessorByPath(string assetPath, params object[] paramList)
        {
            DoAECopy(assetPath);
        }

        /// <summary>
        /// 执行AE拷贝
        /// </summary>
        /// <param name="assetPath"></param>
        private void DoAECopy(string assetPath)
        {
            var targetAssetPath = assetPath.Replace($"/{AssetPipelineConst.A_FOLDER_NAME}/", $"/{AssetPipelineConst.E_FOLDER_NAME}/");
            AssetDatabase.CopyAsset(assetPath, targetAssetPath);
            AssetPipelineLog.Log($"执行AssetPath:{assetPath}拷贝到:{targetAssetPath}".WithColor(Color.yellow));
        }
    }
}