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
    /// Asset检查器信息数据
    /// Json记录所有检查器相关路径和类型信息
    /// 反序列化构建所有检查器对象用
    /// </summary>
    public class AssetCheckInfoData
    {
        /// <summary>
        /// 所有检查器Asset信息
        /// </summary>
        [Header("所有检查器Asset信息")]
        public List<AssetInfo> AllCheckAssetInfo;

        public AssetCheckInfoData()
        {
            AllCheckAssetInfo = new List<AssetInfo>();
        }

        /// <summary>
        /// 添加一个检查器信息
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        public bool AddCheckInfo(BaseCheck check)
        {
            if (check == null)
            {
                Debug.LogWarning($"不添加空检查器信息!");
                return false;
            }
            var checkPath = AssetDatabase.GetAssetPath(check);
            var findCheck = AllCheckAssetInfo.Find((assetInfo) => string.Equals(assetInfo.AssetPath, checkPath));
            if (findCheck != null)
            {
                Debug.LogError($"重复添加检查器路径:{checkPath}的Asset信息，添加失败!");
                return false;
            }
            var checkFullName = check.GetType().FullName;
            AllCheckAssetInfo.Add(new AssetInfo(checkPath, checkFullName));
            return true;
        }
    }
}