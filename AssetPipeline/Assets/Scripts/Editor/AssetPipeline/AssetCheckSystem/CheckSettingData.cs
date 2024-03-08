/*
 * Description:             AssetCheckLocalDataJson.cs
 * Author:                  TONYTANG
 * Create Date:             2023/10/19
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// CheckSettingData.cs
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
        /// 检查器Asset路径(保存时刷新导出，Asset管线运行时用)
        /// </summary>
        [Header("检查器Asset路径")]
        public string CheckAssetPath;

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

        #region 编辑器部分
        /// <summary>
        /// 检查是否有无效检查器配置
        /// </summary>
        /// <returns></returns>
        public bool CheckInvalideCheckConfig()
        {
            return Check == null;
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
        #endregion

        #region 系统运行部分
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
            var check = AssetCheckSystem.GetCheckByAssetPath(CheckAssetPath);
            return check != null ? check.IsValideAssetType(assetType) : false;
        }

        /// <summary>
        /// 是否是有效处理Asset管线处理类型
        /// </summary>
        /// <param name="assetProcessType"></param>
        /// <returns></returns>
        public bool IsValideAssetProcessType(AssetProcessType assetProcessType)
        {
            var check = AssetCheckSystem.GetCheckByAssetPath(CheckAssetPath);
            return check != null ? check.IsValideAssetProcessType(assetProcessType) : false;
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
        #endregion
    }
}
