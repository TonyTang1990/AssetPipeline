/*
 * Description:             ProcessorLocalData.cs
 * Author:                  TONYTANG
 * Create Date:             2023/10/19
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAssetPipeline
{
    /// <summary>
    /// ProcessorLocalData.cs
    /// Asset处理器局部数据
    /// </summary>
    [Serializable]
    public class ProcessorLocalData
    {
        /// <summary>
        /// 目录路径
        /// </summary>
        [Header("目录路径")]
        public string FolderPath;

        /// <summary>
        /// 处理器设置数据列表
        /// </summary>
        public List<ProcessorSettingData> ProcessorDataList;

        /// <summary>
        /// 处理器选择列表(只使用第一个)
        /// </summary>
        [NonSerialized]
        public List<BaseProcessor> ProcessorChosenList;

        /// <summary>
        /// 是否展开
        /// </summary>
        [NonSerialized]
        public bool IsUnFold = false;

        /// <summary>
        /// 处理器Icon列表
        /// </summary>
        public List<GUIContent> ProcessorIconList
        {
            get;
            private set;
        }

        /// <summary>
        /// Asset类型Icon映射Map<Asset类型, Asset Icon>
        /// </summary>
        private Dictionary<AssetType, GUIContent> mProcessorAssetIconMap;

        public ProcessorLocalData()
        {
            FolderPath = "";
            ProcessorDataList = new List<ProcessorSettingData>();
            ProcessorChosenList = new List<BaseProcessor>(1) { null };
            ProcessorIconList = new List<GUIContent>();
            mProcessorAssetIconMap = new Dictionary<AssetType, GUIContent>();
            IsUnFold = false;
        }

        public ProcessorLocalData(bool isUnfold)
        {
            FolderPath = "";
            ProcessorDataList = new List<ProcessorSettingData>();
            ProcessorChosenList = new List<BaseProcessor>(1) { null };
            ProcessorIconList = new List<GUIContent>();
            mProcessorAssetIconMap = new Dictionary<AssetType, GUIContent>();
            IsUnFold = isUnfold;
        }

        /// <summary>
        /// 更新处理器Icon列表
        /// </summary>
        public void UpdaterProcessorIcon()
        {
            mProcessorAssetIconMap.Clear();
            ProcessorIconList.Clear();
            foreach (var processorData in ProcessorDataList)
            {
                if (processorData.Processor != null &&
                    !mProcessorAssetIconMap.ContainsKey(processorData.Processor.TargetAssetType))
                {
                    var targetAssetType = processorData.Processor.TargetAssetType;
                    var assetIcon = AssetPipelineSystem.GetAssetIconByAssetType(targetAssetType);
                    mProcessorAssetIconMap.Add(targetAssetType, assetIcon);
                    ProcessorIconList.Add(assetIcon);
                }
            }
        }

        /// <summary>
        /// 排序所有处理器
        /// </summary>
        public void SortAllData()
        {
            ProcessorDataList.Sort(AssetPipelineUtilities.SortProcessorData);
        }

        /// <summary>
        /// 是否包含指定处理器
        /// </summary>
        /// <param name="processor"></param>
        /// <returns></returns>
        public bool ContainProcessor(BaseProcessor processor)
        {
            var findProcessorData = ProcessorDataList.Find(delegate (ProcessorSettingData processorData)
            {
                return processorData.Processor != null && processorData.Processor.AssetPath.Equals(processor.AssetPath);
            });
            return findProcessorData != null;
        }

        /// <summary>
        /// 添加处理器数据
        /// </summary>
        /// <param name="processor"></param>
        public bool AddProcessorData(BaseProcessor processor)
        {
            if (processor == null)
            {
                Debug.LogError($"不允许添加空的处理器数据!");
                return false;
            }
            var findProcessorData = ProcessorDataList.Find(delegate (ProcessorSettingData processorData)
            {
                return processorData.Processor != null && processorData.Processor.AssetPath.Equals(processor.AssetPath);
            });
            if (findProcessorData != null)
            {
                Debug.LogError($"不允许重复添加相同的处理器:{findProcessorData.Processor.AssetPath}数据!");
                return false;
            }
            ProcessorSettingData processorData = new ProcessorSettingData(processor);
            ProcessorDataList.Add(processorData);
            ProcessorDataList.Sort(AssetPipelineUtilities.SortProcessorData);
            UpdaterProcessorIcon();
            return true;
        }

        /// <summary>
        /// 移除指定索引的处理器配置数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RemoveProcessorDataByIndex(int index)
        {
            if (index < 0 || index >= ProcessorDataList.Count)
            {
                Debug.LogError($"移除处理器数据索引:{index}不在处理器数据有效长度:{ProcessorDataList.Count}内,移除处理器数据失败!");
                return false;
            }
            ProcessorDataList.RemoveAt(index);
            UpdaterProcessorIcon();
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
