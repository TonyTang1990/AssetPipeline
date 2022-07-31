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
    [CreateAssetMenu(fileName = "AECopy", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/PostProcessor/AECopy", order = 1102)]
    public class AECopy : BasePostProcessor
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
                return AssetPipelineSystem.GetAllCommonAssetType();
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
            // 每次修改MD5后会导致Asset处于未保存状态
            // 会导致再次出发PostImported导入流程
            // 为避免不断循环触发AE拷贝，拷贝前先判定是否存在不重复触发
            var targetAssetPath = assetPath.Replace($"/{AssetPipelineConst.A_FOLDER_NAME}/", $"/{AssetPipelineConst.E_FOLDER_NAME}/");
            var targetAsset = AssetDatabase.LoadAssetAtPath<Object>(targetAssetPath);
            if (AssetDatabase.CopyAsset(assetPath, targetAssetPath))
            {
                AssetPipelineLog.Log($"执行AssetPath:{assetPath}拷贝到:{targetAssetPath}".WithColor(Color.yellow));
            }
            else
            {
                Debug.LogError($"AssetPath:{assetPath}拷贝到:{targetAssetPath}失败!");
            }
        }
    }
}