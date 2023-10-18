/*
 * Description:             AssetCheckGlobalData.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/17
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetCheckGlobalData.cs
    /// Asset检查全局数据
    /// </summary>
    [Serializable]
    public class AssetCheckGlobalData
    {
        [MenuItem("Assets/Create/ScriptableObjects/AssetPipeline/AssetCheck/AssetCheckGlobalData", false, 1)]
        private static void CreateAssetCheckGlobalData()
        {
            var selectionFolderPath = AssetPipelineUtilities.GetCurrentSelectionFolderPath();
            var assetCheckGlobalData = new AssetCheckGlobalData();
            var assetCheckGlobalDataAssetPath = Path.Combine(selectionFolderPath, "AssetCheckGlobalData.json");
            var assetCheckGlobalDataJsonContent = JsonUtility.ToJson(assetCheckGlobalData);
            File.WriteAllText(assetCheckGlobalDataAssetPath, assetCheckGlobalDataJsonContent, System.Text.Encoding.UTF8);
            Debug.Log($"在目录:{assetCheckGlobalDataAssetPath}位置创建AssetCheckGlobalData!");
        }

        /// <summary>
        /// Asset检查器全局数据
        /// </summary>
        [Serializable]
        public class CheckGlobalData
        {
            /// <summary>
            /// 检查器设置列表
            /// </summary>
            public List<BaseCheck> CheckList = new List<BaseCheck>();

            /// <summary>
            /// 检查器Asset路径列表(保存时刷新导出，Asset管线运行时用，和CheckList一一对应)
            /// </summary>
            [Header("检查器Asset路径列表")]
            public List<string> CheckAssetPathList = new List<string>();

            /// <summary>
            /// 检查器选择列表(只使用第一个)
            /// </summary>
            [NonSerialized]
            public List<BaseCheck> CheckChosenList = new List<BaseCheck>(1) { null };
        }

        /// <summary>
        /// 预检查器数据
        /// </summary>
        [Header("预检查器数据")]
        public CheckGlobalData PreCheckData = new CheckGlobalData();

        /// <summary>
        /// 后检查器数据
        /// </summary>
        [Header("后检查器数据")]
        public CheckGlobalData PostCheckData = new CheckGlobalData();

        /// <summary>
        /// 排序所有数据
        /// </summary>
        public void SortAllData()
        {
            PreCheckData.CheckList.Sort(AssetPipelineUtilities.SortCheck);
            PostCheckData.CheckList.Sort(AssetPipelineUtilities.SortCheck);
        }

        /// <summary>
        /// 刷新成员值
        /// </summary>
        public void RefreshMemberValue()
        {
            RefreshMemberValueByGlobalData(PreCheckData);
            RefreshMemberValueByGlobalData(PostCheckData);
        }

        /// <summary>
        /// 刷新指定检查器数据列表成员值
        /// </summary>
        /// <param name="checkGlobalData"></param>
        private void RefreshMemberValueByGlobalData(CheckGlobalData checkGlobalData)
        {
            checkGlobalData.CheckAssetPathList.Clear();
            foreach (var check in checkGlobalData.CheckList)
            {
                string checkAssetPath = null;
                if (check != null)
                {
                    checkAssetPath = AssetDatabase.GetAssetPath(check);
                    if (string.IsNullOrEmpty(checkAssetPath))
                    {
                        Debug.LogError($"找不到检查器:{check.name}的Asset路径!");
                    }
                }
                checkGlobalData.CheckAssetPathList.Add(checkAssetPath);
            }
        }
    }
}