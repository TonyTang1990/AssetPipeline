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
    [CreateAssetMenu(fileName = "GenerateABName", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/GenerateABName", order = 1001)]
    public class GenerateABName : BaseProcessor
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
            string md5 = null;
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
            if(asset == null)
            {
                Debug.LogError($"AssetPath:{assetPath}的Asset不存在,标记AB名失败!");
                return;
            }
            md5 = FileUtilities.GetFilePathMD5(assetPath);
            var assetImporter = AssetImporter.GetAtPath(assetPath);
            assetImporter.assetBundleName = md5;
            AssetPipelineLog.Log($"标记AssetPath:{assetPath} MD5:{md5}".WithColor(Color.yellow));
        }
    }
}