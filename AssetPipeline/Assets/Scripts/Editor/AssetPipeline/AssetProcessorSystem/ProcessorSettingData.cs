/*
 * Description:             ProcessorSettingData.cs
 * Author:                  TONYTANG
 * Create Date:             2023/10/19
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// ProcessorSettingData.cs
    /// 处理器设置数据
    /// </summary>
    [Serializable]
    public class ProcessorSettingData
    {
        /// <summary>
        /// 处理器
        /// </summary>
        [Header("处理器")]
        public BaseProcessor Processor;

        /// <summary>
        /// 处理器Asset路径(保存时刷新导出，Asset管线运行时用)
        /// </summary>
        [Header("处理器Asset路径")]
        public string ProcessorAssetPath;

        /// <summary>
        /// 黑名单路径列表
        /// </summary>
        [Header("黑名单路径列表")]
        public List<string> BlackListFolderPathList;

        private ProcessorSettingData()
        {
            BlackListFolderPathList = new List<string>();
        }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="processor"></param>
        public ProcessorSettingData(BaseProcessor processor)
        {
            Processor = processor;
            BlackListFolderPathList = new List<string>();
        }

        #region 编辑器配置部分
        /// <summary>
        /// 检查是否有无效处理器配置
        /// </summary>
        /// <returns></returns>
        public bool CheckInvalideProecssorConfig()
        {
            return Processor == null;
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
                Debug.LogError($"不能允许添加空目录路径作为处理器黑名单路径!");
                return false;
            }
            if (BlackListFolderPathList.Contains(folderPath))
            {
                Debug.LogWarning($"添加重复的目录路径作为处理器黑名单路径,添加失败!");
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
                    AssetPipelineLog.Log(($"Asset:{assetPath}在处理器:{Processor.Name}的黑名单目录列表里!").WithColor(Color.gray));
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否是有效处理器
        /// </summary>
        /// <returns></returns>
        public bool IsValideProcessor()
        {
            // 删除处理器Asset会导致引用丢失，配置处理器Asset找不到的情况
            var processorJson = AssetProcessorSystem.GetProcessorByAssetPath(ProcessorAssetPath);
            return processorJson != null;
        }

        /// <summary>
        /// 是否是有效处理Asset类型
        /// </summary>
        /// <param name="assetType"></param>
        /// <returns></returns>
        public bool IsValideAssetType(AssetType assetType)
        {
            var processorJson = AssetProcessorSystem.GetProcessorByAssetPath(ProcessorAssetPath);
            return processorJson.IsValideAssetType(assetType);
        }

        /// <summary>
        /// 是否是有效处理Asset管线处理类型
        /// </summary>
        /// <param name="assetProcessType"></param>
        /// <returns></returns>
        public bool IsValideAssetProcessType(AssetProcessType assetProcessType)
        {
            var processorJson = AssetProcessorSystem.GetProcessorByAssetPath(ProcessorAssetPath);
            return processorJson.IsValideAssetProcessType(assetProcessType);
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
