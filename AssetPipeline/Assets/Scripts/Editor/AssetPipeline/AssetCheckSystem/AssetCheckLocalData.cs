/*
 * Description:             AssetCheckLocalData.cs
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
    /// AssetCheckConfigData.cs
    /// Asset检查局部数据
    /// </summary>
    public class AssetCheckLocalData : ScriptableObject
    {
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

        /// <summary>
        /// 排序所有数据
        /// </summary>
        public void SortAllData()
        {
            foreach (var preCheckData in PreCheckDataList)
            {
                preCheckData.SortAllData();
            }
            foreach (var postCheckData in PostCheckDataList)
            {
                postCheckData.SortAllData();
            }
        }

        /// <summary>
        /// 刷新成员值
        /// </summary>
        public void RefreshMemberValue()
        {
            RefreshMemberValueByLocalDataList(PreCheckDataList);
            RefreshMemberValueByLocalDataList(PostCheckDataList);
        }

        /// <summary>
        /// 刷新指定检查器数据列表成员值
        /// </summary>
        /// <param name="checkDataList"></param>
        private void RefreshMemberValueByLocalDataList(List<CheckLocalData> checkDataList)
        {
            foreach (var checkData in checkDataList)
            {
                foreach (var checkSettingData in checkData.CheckDataList)
                {
                    string checkAssetPath = null;
                    if (checkSettingData.Check != null)
                    {
                        checkAssetPath = AssetDatabase.GetAssetPath(checkSettingData.Check);
                        if (string.IsNullOrEmpty(checkAssetPath))
                        {
                            Debug.LogError($"找不到检查器:{checkSettingData.Check.name}的Asset路径!");
                        }
                    }
                    checkSettingData.CheckAssetPath = checkAssetPath;
                }
            }
        }
    }
}