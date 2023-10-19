/*
 * Description:             AssetCheckGlobalData.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/17
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetCheckGlobalData.cs
    /// Asset检查全局数据
    /// </summary>
    public class AssetCheckGlobalData : ScriptableObject
    {
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