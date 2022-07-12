/*
 * Description:             AssetPipelineUtilities.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/17
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TAssetPipeline.AssetCheckLocalData;
using static TAssetPipeline.AssetProcessorLocalData;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetPipelineUtilities.cs
    /// Asset管线工具类
    /// </summary>
    public static class AssetPipelineUtilities
    {
        /// <summary>
        /// 处理器排序
        /// </summary>
        /// <param name="processor1"></param>
        /// <param name="processor2"></param>
        /// <returns></returns>
        public static int SortProcessor(BaseProcessor processor1, BaseProcessor processor2)
        {
            if (processor1 == null && processor2 != null)
            {
                return -1;
            }
            if (processor1 != null && processor2 == null)
            {
                return 1;
            }
            if (processor1 == null && processor2 == null)
            {
                return 0;
            }
            if (processor1.TargetAssetType != processor2.TargetAssetType)
            {
                return processor1.TargetAssetType.CompareTo(processor2.TargetAssetType);
            }
            return processor1.TargetAssetProcessType.CompareTo(processor2.TargetAssetProcessType);
        }

        /// <summary>
        /// 处理器数据排序
        /// </summary>
        /// <param name="processorData1"></param>
        /// <param name="processorData2"></param>
        /// <returns></returns>
        public static int SortProcessorData(ProcessorSettingData processorData1, ProcessorSettingData processorData2)
        {
            if (processorData1.Processor == null && processorData2.Processor != null)
            {
                return -1;
            }
            if (processorData1.Processor != null && processorData2.Processor == null)
            {
                return 1;
            }
            if (processorData1.Processor == null && processorData2.Processor == null)
            {
                return 0;
            }
            if (processorData1.Processor.TargetAssetType != processorData2.Processor.TargetAssetType)
            {
                return processorData1.Processor.TargetAssetType.CompareTo(processorData2.Processor.TargetAssetType);
            }
            return processorData1.Processor.TargetAssetProcessType.CompareTo(processorData2.Processor.TargetAssetProcessType);
        }

        /// <summary>
        /// 检查器排序
        /// </summary>
        /// <param name="check1"></param>
        /// <param name="check2"></param>
        /// <returns></returns>
        public static int SortCheck(BaseCheck check1, BaseCheck check2)
        {
            if (check1 == null && check2 != null)
            {
                return -1;
            }
            if (check1 != null && check2 == null)
            {
                return 1;
            }
            if (check1 == null && check2 == null)
            {
                return 0;
            }
            if (check1.TargetAssetType != check2.TargetAssetType)
            {
                return check1.TargetAssetType.CompareTo(check2.TargetAssetType);
            }
            return check1.TargetAssetProcessType.CompareTo(check2.TargetAssetProcessType);
        }

        /// <summary>
        /// 检查器数据排序
        /// </summary>
        /// <param name="check1"></param>
        /// <param name="checkData2"></param>
        /// <returns></returns>
        public static int SortCheckData(CheckSettingData checkData1, CheckSettingData checkData2)
        {
            if (checkData1.Check == null && checkData2.Check != null)
            {
                return -1;
            }
            if (checkData1.Check != null && checkData2.Check == null)
            {
                return 1;
            }
            if (checkData1.Check == null && checkData2.Check == null)
            {
                return 0;
            }
            if (checkData1.Check.TargetAssetType != checkData2.Check.TargetAssetType)
            {
                return checkData1.Check.TargetAssetType.CompareTo(checkData2.Check.TargetAssetType);
            }
            return checkData1.Check.TargetAssetProcessType.CompareTo(checkData2.Check.TargetAssetProcessType);
        }
    }
}