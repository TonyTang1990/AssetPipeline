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

        /// <summary>
        /// 所有处理器路径和类型信息Map<Asset路径, Asset类型信息>
        /// </summary>
        private Dictionary<string, string> mAllProcessorAssetPathTypeMap;

        public AssetProcessorInfoData()
        {
            AllProcessorAssetInfo = new List<AssetInfo>();
            mAllProcessorAssetPathTypeMap = new Dictionary<string, string>();
        }

        /// <summary>
        /// 添加一个处理器信息
        /// </summary>
        /// <param name="processor"></param>
        /// <returns></returns>
        public bool AddProcessorInfo(BaseProcessor processor)
        {
            var processorPath = AssetDatabase.GetAssetPath(processor);
            if(mAllProcessorAssetPathTypeMap.ContainsKey(processorPath))
            {
                Debug.LogError($"重复添加处理器路径:{processorPath}的Asset信息，添加失败!");
                return false;
            }
            var processorFullName = processor.GetType().FullName;
            AllProcessorAssetInfo.Add(new AssetInfo(processorPath, processorFullName));
            mAllProcessorAssetPathTypeMap.Add(processorPath, processorFullName);
            return true;
        }

        /// <summary>
        /// 初始化Asset路径和类型信息Map
        /// </summary>
        public void InitAssetPathTypeMap()
        {
            mAllProcessorAssetPathTypeMap.Clear();
            foreach (var processorAssetInfo in AllProcessorAssetInfo)
            {
                if (mAllProcessorAssetPathTypeMap.ContainsKey(processorAssetInfo.AssetPath))
                {
                    continue;
                }
                mAllProcessorAssetPathTypeMap.Add(processorAssetInfo.AssetPath, processorAssetInfo.AssetTypeFullName);
            }
        }

        /// <summary>
        /// 获取指定Asset路径的类型信息
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public string GetAssetTypeByPath(string assetPath)
        {
            string assetTypeFullName = null;
            if (!mAllProcessorAssetPathTypeMap.TryGetValue(assetPath, out assetTypeFullName))
            {
                Debug.LogError($"找不到Asset处理器:{assetPath}的Asset类型信息!");
            }
            return assetTypeFullName;
        }
    }
}