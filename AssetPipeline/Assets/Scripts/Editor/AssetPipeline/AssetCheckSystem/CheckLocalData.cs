/*
 * Description:             CheckLocalData.cs
 * Author:                  TONYTANG
 * Create Date:             2023/10/19
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// Asset检查器局部数据
    /// </summary>
    [Serializable]
    public class CheckLocalData
    {
        /// <summary>
        /// 目录路径
        /// </summary>
        [Header("目录路径")]
        public string FolderPath;

        /// <summary>
        /// 检查器设置数据列表
        /// </summary>
        public List<CheckSettingData> CheckDataList = new List<CheckSettingData>();

        /// <summary>
        /// 检查器选择列表(只使用第一个)
        /// </summary>
        [NonSerialized]
        public List<BaseCheck> CheckChosenList = new List<BaseCheck>(1) { null };

        /// <summary>
        /// 是否展开
        /// </summary>
        [NonSerialized]
        public bool IsUnFold = false;

        /// <summary>
        /// 处理器Icon列表
        /// </summary>
        public List<GUIContent> CheckIconList
        {
            get;
            private set;
        }

        /// <summary>
        /// Asset类型Icon映射Map<Asset类型, Asset Icon>
        /// </summary>
        private Dictionary<AssetType, GUIContent> mCheckAssetIconMap;

        public CheckLocalData()
        {
            FolderPath = "";
            CheckDataList = new List<CheckSettingData>();
            CheckChosenList = new List<BaseCheck>(1) { null };
            CheckIconList = new List<GUIContent>();
            mCheckAssetIconMap = new Dictionary<AssetType, GUIContent>();
            IsUnFold = false;
        }

        public CheckLocalData(bool isUnfold)
        {
            FolderPath = "";
            CheckDataList = new List<CheckSettingData>();
            CheckChosenList = new List<BaseCheck>(1) { null };
            CheckIconList = new List<GUIContent>();
            mCheckAssetIconMap = new Dictionary<AssetType, GUIContent>();
            IsUnFold = isUnfold;
        }

        /// <summary>
        /// 更新检查器Icon列表
        /// </summary>
        public void UpdaterCheckIcon()
        {
            mCheckAssetIconMap.Clear();
            CheckIconList.Clear();
            foreach (var checkData in CheckDataList)
            {
                if (checkData.Check != null &&
                    !mCheckAssetIconMap.ContainsKey(checkData.Check.TargetAssetType))
                {
                    var targetAssetType = checkData.Check.TargetAssetType;
                    var assetIcon = AssetPipelineSystem.GetAssetIconByAssetType(targetAssetType);
                    mCheckAssetIconMap.Add(targetAssetType, assetIcon);
                    CheckIconList.Add(assetIcon);
                }
            }
        }

        /// <summary>
        /// 排序所有数据
        /// </summary>
        public void SortAllData()
        {
            CheckDataList.Sort(AssetPipelineUtilities.SortCheckData);
        }

        /// <summary>
        /// 添加检查器数据
        /// </summary>
        /// <param name="check"></param>
        public bool AddCheckData(BaseCheck check)
        {
            if (check == null)
            {
                Debug.LogError($"不允许添加空的检查器数据!");
                return false;
            }
            var findCheckData = CheckDataList.Find(delegate (CheckSettingData checkData)
            {
                return checkData.Check != null && checkData.Check.TypeName.Equals(check.TypeName);
            });
            if (findCheckData != null)
            {
                Debug.LogError($"不允许重复添加相同检查器:{findCheckData.Check.AssetPath}数据!");
                return false;
            }
            CheckSettingData checkData = new CheckSettingData(check);
            CheckDataList.Add(checkData);
            CheckDataList.Sort(AssetPipelineUtilities.SortCheckData);
            UpdaterCheckIcon();
            return true;
        }

        /// <summary>
        /// 移除指定索引的检查器配置数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RemoveCheckDataByIndex(int index)
        {
            if (index < 0 || index >= CheckDataList.Count)
            {
                Debug.LogError($"移除检查器数据索引:{index}不在检查器数据有效长度:{CheckDataList.Count}内,移除检查器数据失败!");
                return false;
            }
            CheckDataList.RemoveAt(index);
            UpdaterCheckIcon();
            return true;
        }

        /// <summary>
        /// 指定Asset路径是否在目标目录下
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public bool IsInTargetFolder(string assetPath)
        {
            if (string.IsNullOrEmpty(FolderPath) || string.IsNullOrEmpty(assetPath))
            {
                return false;
            }
            return assetPath.StartsWith(FolderPath);
        }
    }
}
