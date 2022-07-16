/*
 * Description:             GenerateABName.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/19
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// GenerateABName.cs
    /// 生成AB名处理器
    /// </summary>
    [CreateAssetMenu(fileName = "GenerateABName", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/PostProcessor/GenerateABName", order = 1101)]
    public class GenerateABName : BasePostProcessor
    {
        /// <summary>
        /// 检查器名
        /// </summary>
        public override string Name
        {
            get
            {
                return "生成AB名";
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
            MarkAssetBundleName(assetPostProcessor.assetPath);
        }

        /// <summary>
        /// 执行指定路径的处理器处理
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        protected override void DoProcessorByPath(string assetPath, params object[] paramList)
        {
            MarkAssetBundleName(assetPath);
        }

        /// <summary>
        /// 标记指定Asset路径的AB名
        /// </summary>
        /// <param name="assetPath"></param>
        private void MarkAssetBundleName(string assetPath)
        {
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
            if(asset == null)
            {
                Debug.LogError($"AssetPath:{assetPath}的Asset不存在,标记AB名失败!");
                return;
            }
            var md5 = FileUtilities.GetFilePathMD5(assetPath).ToLower();
            var assetImporter = AssetImporter.GetAtPath(assetPath);
            // 每次修改MD5后会导致Asset处于未保存状态
            // 会导致再次出发PostImported导入流程
            // 为避免不断循环触发PostImpoerted导入流程，MD5不变不触发修改
            // Note:
            // assetBundleName赋值后都是小写
            if (!string.Equals(assetImporter.assetBundleName, md5))
            {
                AssetPipelineLog.Log($"标记AssetPath:{assetPath} MD5:{md5} 原MD5:{assetImporter.assetBundleName}".WithColor(Color.yellow));
                assetImporter.assetBundleName = md5;
            }
            else
            {
                AssetPipelineLog.Log($"AssetPath:{assetPath} MD5相同，不需要更改!".WithColor(Color.yellow));
            }
        }
    }
}