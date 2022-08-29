/*
 * Description:             AssetCheckInfoData.cs
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
    /// AssetCheckInfoData.cs
    /// </summary>
    public class AssetCheckInfoData
    {
        /// <summary>
        /// 所有检查器Asset信息
        /// </summary>
        [Header("所有检查器Asset信息")]
        public List<AssetInfo> AllCheckAssetInfo;

        /// <summary>
        /// 所有检查器路径和类型信息Map<Asset路径, Asset类型信息>
        /// </summary>
        private Dictionary<string, string> mAllCheckAssetPathTypeMap;

        public AssetCheckInfoData()
        {
            AllCheckAssetInfo = new List<AssetInfo>();
            mAllCheckAssetPathTypeMap = new Dictionary<string, string>();
        }

        /// <summary>
        /// 添加一个检查器信息
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        public bool AddCheckInfo(BaseCheck check)
        {
            var checkPath = AssetDatabase.GetAssetPath(check);
            if (mAllCheckAssetPathTypeMap.ContainsKey(checkPath))
            {
                Debug.LogError($"重复添加检查器路径:{checkPath}的Asset信息，添加失败!");
                return false;
            }
            var checkFullName = check.GetType().FullName;
            AllCheckAssetInfo.Add(new AssetInfo(checkPath, checkFullName));
            mAllCheckAssetPathTypeMap.Add(checkPath, checkFullName);
            return true;
        }

        /// <summary>
        /// 初始化Asset路径和类型信息Map
        /// </summary>
        public void InitAssetPathTypeMap()
        {
            mAllCheckAssetPathTypeMap.Clear();
            foreach (var processorAssetInfo in AllCheckAssetInfo)
            {
                if (mAllCheckAssetPathTypeMap.ContainsKey(processorAssetInfo.AssetPath))
                {
                    continue;
                }
                mAllCheckAssetPathTypeMap.Add(processorAssetInfo.AssetPath, processorAssetInfo.AssetTypeFullName);
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
            if (!mAllCheckAssetPathTypeMap.TryGetValue(assetPath, out assetTypeFullName))
            {
                Debug.LogError($"找不到Asset检查器:{assetPath}的Asset类型信息!");
            }
            return assetTypeFullName;
        }
    }
}