/*
 * Description:             AssetProcessorInfoData.cs
 * Author:                  TONYTANG
 * Create Date:             2022/08/28
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetProcessorInfoData.cs
    /// Asset处理器信息数据
    /// Json记录所有处理器相关路径和类型信息
    /// 反序列化构建所有处理器对象用
    /// </summary>
    [Serializable]
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
            if(string.IsNullOrEmpty(processorPath))
            {
                Debug.LogError($"找不到处理器:{processor.name}的Asset路径,添加处理器信息失败!");
                return false;
            }
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