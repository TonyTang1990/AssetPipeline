/*
 * Description:             AssetProcessorInfoData.cs
 * Author:                  TONYTANG
 * Create Date:             2022/08/28
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetProcessorInfoData.cs
    /// Asset处理器信息数据(记录所有Asset处理器信息)
    /// </summary>
    public class AssetProcessorInfoData
    {
        /// <summary>
        /// 所有处理器Asset信息
        /// </summary>
        [Header("所有处理器Asset信息")]
        public List<AssetInfo> AllProcessorAssetInfo;

        public AssetProcessorInfoData()
        {
            AllProcessorAssetInfo = new List<AssetInfo>();
        }

        /// <summary>
        /// 添加一个处理器信息
        /// </summary>
        /// <param name="processor"></param>
        /// <returns></returns>
        public bool AddProcessorInfo(BaseProcessor processor)
        {
            if(processor == null)
            {
                Debug.LogWarning($"不添加空处理器信息!");
                return false;
            }
            var processorPath = AssetDatabase.GetAssetPath(processor);
            var findProcessor = AllProcessorAssetInfo.Find((assetInfo) => string.Equals(assetInfo.AssetPath, processorPath));
            if (findProcessor != null)
            {
                Debug.LogError($"重复添加处理器路径:{processorPath}的Asset信息，添加失败!");
                return false;
            }
            var processorFullName = processor.GetType().FullName;
            AllProcessorAssetInfo.Add(new AssetInfo(processorPath, processorFullName));
            return true;
        }
    }
}