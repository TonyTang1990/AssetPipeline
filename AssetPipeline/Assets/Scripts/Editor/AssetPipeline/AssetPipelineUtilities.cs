﻿/*
 * Description:             AssetPipelineUtilities.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/17
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
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
        /// 获取当前选中Asset目录(如果选中Asset则为该Asset所在目录)
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentSelectionFolderPath()
        {
            var currentSelectionFolderPath = "Assets/";
            if (Selection.activeObject == null)
            {
                return currentSelectionFolderPath;
            }
            var selectionAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if(string.IsNullOrEmpty(selectionAssetPath))
            {
                return currentSelectionFolderPath;
            }
            if(AssetDatabase.IsValidFolder(selectionAssetPath))
            {
                return selectionAssetPath;
            }
            return Path.GetDirectoryName(selectionAssetPath);
        }

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
            if(processor1.TargetAssetProcessType != processor2.TargetAssetProcessType)
            {
                return processor1.TargetAssetProcessType.CompareTo(processor2.TargetAssetProcessType);
            }
            if(processor1.Order != processor2.Order)
            {
                return processor1.Order.CompareTo(processor2.Order);
            }
            return processor1.TypeName.CompareTo(processor2.TypeName);
        }

        /// <summary>
        /// 处理器数据排序
        /// </summary>
        /// <param name="processorData1"></param>
        /// <param name="processorData2"></param>
        /// <returns></returns>
        public static int SortProcessorData(ProcessorSettingData processorData1, ProcessorSettingData processorData2)
        {
            return SortProcessor(processorData1.Processor, processorData2.Processor);
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
            if (check1.TargetAssetProcessType != check2.TargetAssetProcessType)
            {
                return check1.TargetAssetProcessType.CompareTo(check2.TargetAssetProcessType);
            }
            if (check1.Order != check2.Order)
            {
                return check1.Order.CompareTo(check2.Order);
            }
            return check1.TypeName.CompareTo(check2.TypeName);
        }

        /// <summary>
        /// 检查器数据排序
        /// </summary>
        /// <param name="check1"></param>
        /// <param name="checkData2"></param>
        /// <returns></returns>
        public static int SortCheckData(CheckSettingData checkData1, CheckSettingData checkData2)
        {
            return SortCheck(checkData1.Check, checkData2.Check);
        }
    }
}