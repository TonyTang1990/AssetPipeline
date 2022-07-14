/*
 * Description:             AssetCheckLocalData.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/17
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetCheckConfigData.cs
    /// Asset检查局部数据
    /// </summary>
    public class AssetCheckLocalData : ScriptableObject
    {
        /// <summary>
        /// 检查器设置数据
        /// </summary>
        [Serializable]
        public class CheckSettingData
        {
            /// <summary>
            /// 检查器
            /// </summary>
            [Header("检查器")]
            public BaseCheck Check;

            /// <summary>
            /// 黑名单路径列表
            /// </summary>
            [Header("黑名单路径列表")]
            public List<string> BlackListFolderPathList;

            private CheckSettingData()
            {
                BlackListFolderPathList = new List<string>();
            }

            /// <summary>
            /// 带参构造函数
            /// </summary>
            /// <param name="check"></param>
            public CheckSettingData(BaseCheck check)
            {
                Check = check;
                BlackListFolderPathList = new List<string>();
            }

            /// <summary>
            /// 添加黑名单列表
            /// </summary>
            /// <param name="folderPath"></param>
            /// <returns></returns>
            public bool AddBlackListFolderPath(string folderPath)
            {
                if (string.IsNullOrEmpty(folderPath))
                {
                    Debug.LogError($"不能允许添加空目录路径作为检查器黑名单路径!");
                    return false;
                }
                if (BlackListFolderPathList.Contains(folderPath))
                {
                    Debug.LogWarning($"添加重复的目录路径作为检查器黑名单路径,添加失败!");
                    return false;
                }
                BlackListFolderPathList.Add(folderPath);
                return true;
            }

            /// <summary>
            /// 移除指定索引的黑名单目录设置
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public bool RemoveBlackListByIndex(int index)
            {
                if (index < 0 || index >= BlackListFolderPathList.Count)
                {
                    Debug.LogError($"移除黑名单索引:{index}不在黑名单有效长度:{BlackListFolderPathList.Count}内,移除黑名单目录失败!");
                    return false;
                }
                BlackListFolderPathList.RemoveAt(index);
                return true;
            }

            /// <summary>
            /// 指定Asset路径是否在黑名单列表里
            /// </summary>
            /// <param name="assetPath"></param>
            /// <returns></returns>
            public bool IsInBlackList(string assetPath)
            {
                if (BlackListFolderPathList.Count == 0)
                {
                    return false;
                }
                foreach (var blackListFolderPath in BlackListFolderPathList)
                {
                    if (assetPath.StartsWith(blackListFolderPath))
                    {
                        AssetPipelineLog.Log(($"Asset:{assetPath}在检查器:{Check.Name}的黑名单目录列表里!").WithColor(Color.gray));
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// 是否是有效处理Asset类型
            /// </summary>
            /// <param name="assetType"></param>
            /// <returns></returns>
            public bool IsValideAssetType(AssetType assetType)
            {
                return Check.IsValideAssetType(assetType);
            }

            /// <summary>
            /// 是否是有效处理Asset管线处理类型
            /// </summary>
            /// <param name="assetProcessType"></param>
            /// <returns></returns>
            public bool IsValideAssetProcessType(AssetProcessType assetProcessType)
            {
                return Check.IsValideAssetProcessType(assetProcessType);
            }

            /// <summary>
            /// 打印所有黑名单目录
            /// </summary>
            public void PrintAllBlackListFolder()
            {
                foreach (var blackListFolderPath in BlackListFolderPathList)
                {
                    AssetPipelineLog.Log($"黑名单目录:{blackListFolderPath}".WithColor(Color.yellow));
                }
            }
        }

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

        /// <summary>
        /// 局部预检查器数据
        /// </summary>
        [Header("局部预检查器数据")]
        public List<CheckLocalData> PreCheckDataList = new List<CheckLocalData>();

        /// <summary>
        /// 局部后检查器数据
        /// </summary>
        [Header("局部后检查器数据")]
        public List<CheckLocalData> PostCheckDataList = new List<CheckLocalData>();

        /// <summary>
        /// 更新所有检查器数据的Icon信息
        /// </summary>
        public void UpdateAllCheckIconDatas()
        {
            foreach (var preCheckData in PreCheckDataList)
            {
                preCheckData.UpdaterCheckIcon();
            }
            foreach (var postCheckData in PostCheckDataList)
            {
                postCheckData.UpdaterCheckIcon();
            }
        }
    }
}