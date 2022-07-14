/*
 * Description:             AEDelete.cs
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
    /// AEDelete.cs
    /// AE目录删除处理器
    /// </summary>
    [CreateAssetMenu(fileName = "AEDelete", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/AEDelete", order = 1008)]
    public class AEDelete : BasePostProcessor
    {
        /// <summary>
        /// 检查器名
        /// </summary>
        public override string Name
        {
            get
            {
                return "AE目录删除";
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
                return AssetProcessType.CommonPostprocess;
            }
        }

        /// <summary>
        /// 执行处理器处理
        /// </summary>
        /// <param name="assetPostProcessor"></param>
        /// <param name="paramList">不定长参数列表</param>
        protected override void DoProcessor(AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            DoAEDelete(assetPostProcessor.assetPath);
        }

        /// <summary>
        /// 执行指定路径的处理器处理
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        protected override void DoProcessorByPath(string assetPath, params object[] paramList)
        {
            DoAEDelete(assetPath);
        }

        /// <summary>
        /// 执行AE删除
        /// </summary>
        /// <param name="assetPath"></param>
        private void DoAEDelete(string assetPath)
        {
            var targetAssetPath = assetPath.Replace($"/{AssetPipelineConst.A_FOLDER_NAME}/", $"/{AssetPipelineConst.E_FOLDER_NAME}/");
            AssetDatabase.DeleteAsset(targetAssetPath);
            AssetPipelineLog.Log($"AssetPath:{assetPath}被删除，执行AssetPath:{targetAssetPath}删除".WithColor(Color.yellow));
        }
    }
}